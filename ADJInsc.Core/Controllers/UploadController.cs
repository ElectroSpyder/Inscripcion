using ADJInsc.Core.Service.Interface;
using ADJInsc.Models.ViewModels.UpLoad;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;
using System;
using System.Threading.Tasks;
using ADJInsc.Core.Helper;

namespace ADJInsc.Core.Controllers
{

    public class UploadController : Controller
    {       
        public IConfiguration Configuration { get; }
        public string _connectionString { get; set; }
        //private readonly IMailService mailService;
        private readonly IApiService _apiService;
        // GET: AdhesionController
        public UploadController(IConfiguration configuration, IApiService apiService)
        {
            Configuration = configuration;
            _connectionString = Configuration["ConnectionStrings:DefaultConnection"];

            this._apiService = apiService;
        }

        [HttpPost]
        public async Task<JsonResult> UploadImages()
        {
            if (Request.Form.Files.Count == 0)
            {
                return Json(new { success = false, message = "❌ No se recibió ningún archivo." });
            }

            string fecha = DateTime.Now.ToString("yyyyMMdd");
            string tipoIngreso = Request.Form["TipoIngreso"]; // Obtener el string enviado

            try
            {
                //porque se repite
                //var _memoryArchivoVM = HttpContext.Session.GetObjectFromJson<List<FileUploadViewModel>>("_filesInMemory") ?? new List<FileUploadViewModel>();
                List<string> archivosGuardados = new List<string>();
                var _memoryArchivoVM = new List<FileUploadViewModel>();
                    foreach (var file in Request.Form.Files)
                    {
                        using var memoryStream = new MemoryStream();
                        await file.CopyToAsync(memoryStream);
                        //porque se repite
                        _memoryArchivoVM.Add(new FileUploadViewModel
                        {
                            NombreArchivo = file.FileName,
                            Tamano = file.Length,
                            Fecha = fecha,
                            FileContent = memoryStream.ToArray(),
                            TipoContenido = file.ContentType,
                            TipoIngreso = tipoIngreso
                        });

                        archivosGuardados.Add(file.FileName);
                    }
              

                    //_filesInMemory = _memoryArchivoVM;
                    HttpContext.Session.SetObjectAsJson<List<FileUploadViewModel>>("_filesInMemory", _memoryArchivoVM);
                //HttpContext.Session.SetObjectAsJson<List<FileUploadViewModel>>("_filesInMemory", _filesInMemory);
                return Json(new { success = true, message = "✅ Archivos subidos correctamente: " + string.Join(", ", archivosGuardados) });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "❌ Error: " + ex.Message });
            }
        }

    }
}
