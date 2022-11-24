namespace ADJInsc.Core.Controllers
{
    using ADJInsc.Core.Helper;
    using ADJInsc.Core.Service.Interface;
    using ADJInsc.Models.ViewModels;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    public class _ADJControlController : Controller
    {
        public IConfiguration Configuration { get; }
        public string _connectionString { get; set; }
        //private readonly IMailService mailService;
        private readonly IApiService apiAservice;

        public _ADJControlController(IConfiguration configuration, IApiService apiAservice)
        {
            Configuration = configuration;
            _connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            // this.mailService = mailService;
            this.apiAservice = apiAservice;
        }

        
        public IActionResult Index()
        {
            var model = new UsuarioTitularViewModel();

            return View(model);
        }

            [HttpPost]
        [AllowAnonymous]
        public IActionResult _BPersona(string numDni)
        {
            var model = new UsuarioTitularViewModel();
           

            List<SelectListItem> tipoFamilia = model.GetListado().Select
                       (r => new SelectListItem
                       {
                           Value = $"{r.TipoFamiliaKey}",
                           Text = r.TipoFamiliaDesc
                       })
                       .OrderBy(o => o.Text)
                       .ToList();

            model.TipoFamiliaList = tipoFamilia;
           

            var convert = int.TryParse(numDni, out int numeroDniEntero);

            if (convert)
            {

                var tokenSource = new CancellationTokenSource();
                var token = tokenSource.Token;
                /*  Produccion 
                 *  
                 *   var service = this.apiAservice.GetAsync<UsuarioTitularViewModel>("/Test.Insc.Api/helper/", "GetInscriptoGF" + "?id=" + numDni, token).Result;
                   var service = this.apiAservice.GetAsync<UsuarioTitularViewModel>("/Insc.Api/helper/", "GetInscriptoGF" + "?id=" + numDni, token).Result;
                 */

                var service = this.apiAservice.GetAsync<UsuarioTitularViewModel>("/Test.Insc.Api/helper/", "GetInscriptoGF" + "?id=" + numDni, token).Result;
                var result = (UsuarioTitularViewModel)service.Result;
               
                if (result.InsId > 0)
                {
                    result.Existe = true;   //porque existe    
                                            // si el estado es E => mostrar login modal

                    return View("_Panel", result);

                    /*
                    if (result.InsEstado == "N")   //N significa que el titular existe en grupo familiar
                    {                       
                        

                    }

                    if (result.InsEstado == "S")
                    {
                       
                    }

                    if (result.InsEstado == "E" || result.InsEstado == "I")
                    {
                       

                        //si el estado es A => esta cargado pero no esta validado
                        // si el estado es E => esta validado
                        // si el estado es nulo => esta migrado y hay que actualizar y no tiene usuario
                        //mostrar para que ingrese con correo contraseña
                        // en utlimo caso si no existe debo cargar el formulario porque es nuevo y InsEstado == "N"

                    }
                    else
                    {
                        if (result.InsEstado == "A")
                        {
                            
                        }
                        else
                        {
                            //esto porque esta inscripto, estado M
                            
                        }
                    }

                    */
                }
                else
                {
                   //no existe en base
                }                             


                return RedirectToAction("Index");
                //return View("AltaTitular", result); 
            }
            else
            {
                var mensaje = new UsuarioTitularViewModel
                {
                    MensajeModel = "Debe ingresar un número!"
                };
                return RedirectToAction("Mensaje", mensaje);

            }

        }
    }
}
