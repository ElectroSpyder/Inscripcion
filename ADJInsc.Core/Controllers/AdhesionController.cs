﻿namespace ADJInsc.Core.Controllers
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
        // GET: AdhesionController

        public AdhesionController(IConfiguration configuration, IApiService apiService)
        {
            Configuration = configuration;
            _connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            // this.mailService = mailService;
            this._apiService = apiService;
        }
        public JsonResult Adherir(int programaId, int moduloId, int insId)
        {
            // public int InsId { get; set; }
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            var modeloVM = HttpContext.Session.GetObjectFromJson<InscViewModel>("viewModelo");
            var _memoryArchivoVM = HttpContext.Session.GetObjectFromJson<List<FileUploadViewModel>>("_filesInMemory");

            var modeloAdherir = new AdherirViewModel
            {
                InscriptoId = insId,
                ModuloId = moduloId,
                ProgramaId = programaId,
                FechaAdhesion = DateTime.Now                
            };

            modeloVM.AdherirViewModel = modeloAdherir;
            // 🔴 Agregar archivos almacenados en memoria a `modeloVM`
            if (_memoryArchivoVM != null && _memoryArchivoVM.Any())
            {
                modeloVM.FileUploadViewModel = _memoryArchivoVM;
            }
            else
            {
                return Json(new
                {
                    redirectUrl = "",
                    isRedirect = false,
                    ob = "Vuelva a cargar los archivos"
                });
            }

                HttpContext.Session.SetObjectAsJson<InscViewModel>("viewModelo", modeloVM);

            var service = this._apiService.PostAdhesionAsync<ResponseViewModel>("/Test.Insc.Api/adhesion/", "PostModeloAdhesion", modeloVM, token).Result;
            if (service.IsSuccess)
            {
                var respuesta = (InscViewModel)service.Result;

                if (respuesta.AdhesionViewModel.Success)
                {

                    var modeloNuevo = HttpContext.Session.GetObjectFromJson<InscViewModel>("viewModelo");
                    modeloNuevo.AdherirViewModel = respuesta.AdherirViewModel;  //idAdhesion  -> probar
                                        
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

        public IActionResult GetPdfAdhesion()
        {
            var modelo = HttpContext.Session.GetObjectFromJson<InscViewModel>("viewModelo");

            return new ViewAsPdf("_ReporteAdhesion", modelo);

        }
    }
}
