namespace ADJInsc.Core.Controllers
{
    using ADJInsc.Core.Helper;
    using ADJInsc.Core.Service.Interface;
    using ADJInsc.Models.ViewModels.AdhesionVM;
    using ADJInsc.Models.ViewModels;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using System.Collections.Generic;
    using System.Threading;
    using System;
    using Rotativa.AspNetCore;
    using ADJInsc.Models.ViewModels.UpLoad;
    using System.Linq;

    public class AdhesionController : Controller
    {
        public IConfiguration Configuration { get; }
        public string _connectionString { get; set; }
        //private readonly IMailService mailService;
        private readonly IApiService _apiService;
        private DateTime Fecha => DateTime.Now;

        public AdhesionController(IConfiguration configuration, IApiService apiService)
        {
            Configuration = configuration;
            _connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            // this.mailService = mailService;
            this._apiService = apiService;
        }
        public JsonResult Adherir(int programaId, int moduloId, int insId, string modeloDetalle)
        {
            // public int InsId { get; set; }
            var tokenSource = new CancellationTokenSource();
           if(programaId == 0 || moduloId == 0 || insId == 0 || string.IsNullOrEmpty(modeloDetalle))
                return Json(new
                {
                    redirectUrl = "",
                    isRedirect = false,
                    ob = "No se seleccionaron datos, vuelva a intentarlo o comuníquese con IVUJ."
                });

            var token = tokenSource.Token;
            
            // Obtener datos de la sesión
            var modeloVM = HttpContext.Session.GetObjectFromJson<InscViewModel>("viewModelo") ?? new InscViewModel();
            var _memoryArchivoVM = HttpContext.Session.GetObjectFromJson<List<FileUploadViewModel>>("_filesInMemory") ?? new List<FileUploadViewModel>();

            // Validar si hay archivos en memoria antes de continuar
            if (!_memoryArchivoVM.Any())
            {
                return Json(new
                {
                    redirectUrl = "",
                    isRedirect = false,
                    ob = "Vuelva a cargar los archivos"
                });
            }

            // Crear el modelo de adhesión
            modeloVM.AdherirViewModel = new AdherirViewModel
            {
                InscriptoId = insId,
                ModuloId = moduloId,
                ModuloDescripcion = modeloDetalle?.Trim() ?? " ",
                ProgramaId = programaId,
                FechaAdhesion = Fecha.ToString()
            };

            // Asignar archivos a `modeloVM`
            modeloVM.FileUploadViewModel = _memoryArchivoVM;

            HttpContext.Session.SetObjectAsJson<InscViewModel>("viewModelo", modeloVM);
            //Insc.Api
            /*
             var service = this._apiService.PostAdhesionAsync<ResponseViewModel>("/Insc.Api/adhesion/", "PostModeloAdhesion", modeloVM,null, token).Result;
            var service = this._apiService.PostAdhesionAsync<ResponseViewModel>("/Test.Insc.Api/adhesion/", "PostModeloAdhesion", modeloVM,null, token).Result;
             */
            var service = this._apiService.PostAdhesionAsync<ResponseViewModel>("/Insc.Api/adhesion/", "PostModeloAdhesion", modeloVM, null, token).Result;

            if (service.IsSuccess)
            {
                var respuesta = (InscViewModel)service.Result;

                if (respuesta.AdhesionViewModel.Success)
                {
                    if (respuesta.AdherirViewModel.AdhesionId > 0 &&
                       (respuesta.FileUploadViewModel == null || !respuesta.FileUploadViewModel.Any()))
                    {
                        return Json(new
                        {
                            redirectUrl = "",
                            isRedirect = false,
                            ob = respuesta.InsNombre.Trim() + " esta adherido con número " + respuesta.AdherirViewModel.AdhesionId.ToString()
                        });
                    }

                    var modeloNuevo = HttpContext.Session.GetObjectFromJson<InscViewModel>("viewModelo");
                    modeloNuevo.AdherirViewModel = respuesta.AdherirViewModel;  //idAdhesion  -> probar
                                                                                //aqui debo setear el nombre de la localidad
                    string descripcionLocalidad = string.Empty;

                    foreach (var item in modeloNuevo.LocalidadesList)
                    {
                        if(int.Parse(item.Value.Split('-')[0]) == modeloNuevo.DepartamentoKey && int.Parse(item.Value.Split('-')[1]) == modeloNuevo.LocalidadKey)
                        {
                            modeloNuevo.LocalidadDesc = item.Text.Trim();
                            break;
                        }                        
                    }

                    HttpContext.Session.SetObjectAsJson<InscViewModel>("viewModelo", modeloNuevo);

                    //4_ rotativa_ retorno el json pero sin valor
                    var redirectUrl1 = Url.Action("GetPdfAdhesion", "Adhesion");
                    return Json(new
                    {
                        redirectUrl = redirectUrl1,
                        isRedirect = true,
                        ob = respuesta.AdhesionViewModel.Success
                    });

                }
                else
                {
                    return Json(new
                    {
                        redirectUrl = "",
                        isRedirect = false,
                        ob = respuesta.AdhesionViewModel.Success
                    });
                }
            }
            else
            {
                return Json(new
                {
                    redirectUrl = "",
                    isRedirect = false,
                    ob = "no llego respuesta"
                });
            }
        }
        //public IActionResult ErrorPage()
        //{
        //    return View("ErrorPage");
        //}
        public IActionResult GetPdfAdhesion(string pdfUrl)
        {

            if (string.IsNullOrEmpty(pdfUrl))
            {
                var modeloView = HttpContext.Session.GetObjectFromJson<InscViewModel>("viewModelo");
                var modeloSesion = new ModelPdf
                {
                    AdheridoId = modeloView.AdherirViewModel.AdhesionId.ToString(),
                    Correo = modeloView.InsEmail,
                    CuitAdherido = modeloView.CuitCuil,
                    DniAdherido = modeloView.InsNumdoc,
                    FechaAdhesion = modeloView.AdherirViewModel.FechaAdhesion,
                    Localidad = modeloView.LocalidadDesc,
                    InscriptoId = modeloView.InsId.ToString(),
                    NombreAdherido = modeloView.InsNombre,
                    TipologiaDescripcion = modeloView.AdherirViewModel.Descripcion
                };
                return new ViewAsPdf("_ReporteAdhesion", modeloSesion);
            }
            else
            {
                var tokenSource = new CancellationTokenSource();
                var token = tokenSource.Token;
                var modeloVM = new ModelPdf { InscriptoId = pdfUrl.Trim() };
                /*
                var modelo = this._apiService.PostPdfAdhesionAsync<ResponseViewModel>("/Insc.Api/adhesion/", "GetModelPdf", modeloVM, token).Result;
                var modelo = this._apiService.PostPdfAdhesionAsync<ResponseViewModel>("/Test.Insc.Api/adhesion/", "GetModelPdf", modeloVM, token).Result;
                 */

                var modelo = this._apiService.PostPdfAdhesionAsync<ResponseViewModel>("/Insc.Api/adhesion/", "GetModelPdf", modeloVM, token).Result;
                if (modelo.IsSuccess)
                    return new ViewAsPdf("_ReporteAdhesion", (ModelPdf)modelo.Result);
                return RedirectToAction("ErrorPage");
            }
        }
    }
}
