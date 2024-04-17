namespace ADJInsc.Core.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Helper;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using ADJInsc.Core.Service.Interface;
    using System;
    using System.Threading;
    using System.Linq;
    using Microsoft.AspNetCore.Authorization;
    using ADJInsc.Models.ViewModels;
    using System.Collections.Generic;

    public class InscripcionController : Controller
    {
        public IConfiguration Configuration { get; }
        public string _connectionString { get; set; }
        //private readonly IMailService mailService;
        private readonly IApiService apiAservice;

        public InscripcionController(IConfiguration configuration, IApiService apiAservice)
        {
            Configuration = configuration;
            _connectionString = Configuration["ConnectionStrings:DefaultConnection"];
           // this.mailService = mailService;
            this.apiAservice = apiAservice;
        }
               
        public IActionResult Index()
        {
            var modelo = HttpContext.Session.GetObjectFromJson<AltaTitularViewModel>("altaTitular");

            List<SelectListItem> tipoFamilia = modelo.GetListado().Select
                      (r => new SelectListItem
                      {
                          Value = $"{r.TipoFamiliaKey}",
                          Text = r.TipoFamiliaDesc
                      })
                      .OrderBy(o => o.Text)
                      .ToList();

            modelo.TipoFamiliaList = tipoFamilia;

            return View(modelo);
            
        }

        public IActionResult Inscripcion()
        {
            var model = new UsuarioTitularViewModel();
            var modelo = HttpContext.Session.GetObjectFromJson<UsuarioTitularViewModel>("viewTitularModelo");
            if (modelo == null)
            {

            }
            else
            {
                model = modelo;
            }           

            List<SelectListItem> tipoFamilia = model.GetListado().Select
                       (r => new SelectListItem
                       {
                           Value = $"{r.TipoFamiliaKey}",
                           Text = r.TipoFamiliaDesc
                       })
                       .OrderBy(o => o.Text)
                       .ToList();

            model.TipoFamiliaList = tipoFamilia;
            model.Existe = false;   //inicialiso para no mostrar el formulario
            HttpContext.Session.SetObjectAsJson<UsuarioTitularViewModel>("viewTitularModelo", model); //cargo en cache el resultado

            return View("Inscripcion");
        }



        [HttpPost]
        [AllowAnonymous]
        public IActionResult BuscarPersona(string numDni)
        {

            var convert = int.TryParse(numDni, out int numeroDniEntero);

            if (convert)
            {

                var tokenSource = new CancellationTokenSource();
                var token = tokenSource.Token;
                /*  Produccion 
                 *  
                 *   var service = this.apiAservice.GetAsync<UsuarioTitularViewModel>("/Test.Insc.Api/helper/", "GetInscripto" + "?id=" + numDni, token).Result;
                   var service = this.apiAservice.GetAsync<UsuarioTitularViewModel>("/Insc.Api/helper/", "GetInscripto" + "?id=" + numDni, token).Result;
                 */

                var service = this.apiAservice.GetAsync<UsuarioTitularViewModel>("/Insc.Api/helper/", "GetInscripto" + "?id=" + numDni, token).Result;
                var result = (UsuarioTitularViewModel)service.Result;

                var model = new UsuarioTitularViewModel();

                result.TipoFamiliaList = model.GetListado().Select
                  (r => new SelectListItem
                  {
                      Value = $"{r.TipoFamiliaKey}",
                      Text = r.TipoFamiliaDesc
                  })
                  .OrderBy(o => o.Text)
                  .ToList();


                if (result.InsId > 0)
                {
                    result.Existe = true;   //porque existe    
                                            // si el estado es E => mostrar login modal

                    if (result.InsEstado == "N")   //N significa que el titular existe en grupo familiar
                    {
                        var existeT = new UsuarioTitularViewModel
                        {
                            TipoFamiliaList = model.GetListado().Select
                                        (r => new SelectListItem
                                        {
                                            Value = $"{r.TipoFamiliaKey}",
                                            Text = r.TipoFamiliaDesc
                                        })
                                   .OrderBy(o => o.Text)
                                   .ToList(),
                            //actualizar el nuevo modelo con datos del nuevo titular
                            InsNumdoc = numDni
                        };
                        foreach (var item in result.GrupoFamiliar)
                        {
                            existeT.InsNombre = item.InsfNombre;
                        }
                        existeT.MensajeModel = "Según nuestros registros, Usted ya se encuentra inscripto/a en un Grupo Familiar de " + result.InsNombre + " ,D.N.I.: " + result.InsNumdoc +
                            " ¿Desea crear un nuevo grupo familiar colocándose como Titular?  " +
                            " Si su respuesta es afirmativa, complete los datos para crear grupo familiar " +
                            " ,si su respuesta es negativa, deberá 'Cerrar y Volver'.";
                        HttpContext.Session.SetObjectAsJson<UsuarioTitularViewModel>("viewTitularModelo", existeT); //cargo en cache el resultado

                        //limpio el grupo familiar
                        existeT.GrupoFamiliar = new List<GrupoFamiliarViewModel>();

                        //aqui creo el modelo para mostrar en index
                        var alta = new AltaTitularViewModel
                        {
                            InsNumdoc = numDni,
                            MensajeModel = existeT.MensajeModel,
                            InsNombre = existeT.InsNombre
                        };

                        HttpContext.Session.SetObjectAsJson<AltaTitularViewModel>("altaTitular", alta);



                        //return RedirectToAction("AltaTitular");    //prueba para que funcione en celular
                        return RedirectToAction("Index");
                        // ya existe en base de grupoFamiliar
                        //se debe mostrar mensaje o un popup que diga que se debe hacercar al ivuj

                    }

                    if (result.InsEstado == "S")
                    {
                        //string[] words = result.InsEmail.Split('@');
                        var model1 = new LoginViewModel
                        {
                            Email = "El DNI: " + result.InsNumdoc.ToString() + " se encuentra bajo Evaluación de Requisitos, consulte al IVUJ para mayor información. Gracias.",
                            Password = "3"
                        };
                        return View("Login", model1);
                    }

                    if (result.InsEstado == "E" || result.InsEstado == "I")
                    {
                        string[] words1 = result.InsEmail.Split('@');
                        var model1 = new LoginViewModel
                        {
                            Email = "****@" + words1[1],
                            Password = "1"
                        };
                        return View("Login", model1);

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
                            string[] words = result.InsEmail.Split('@');
                            var model1 = new LoginViewModel
                            {
                                Email = "****@" + words[1],
                                Password = "2"
                            };
                            return View("Login", model1);
                        }
                        else
                        {
                            //esto porque existe el usuario y su estado es M (no inscripto, pero si existe)
                            result.InsNumdoc = numDni;
                            result.Existe = false;
                            //aqui agregar el dni al modelo de carga
                            var alta = new AltaTitularViewModel
                            {
                                InsNumdoc = numDni,
                                InsNombre = result.InsNombre,
                                InsEmail = result.InsEmail,
                                GrupoFamiliar = result.GrupoFamiliar,
                                InsTelef = result.InsTelef,
                                InsTipflia = result.InsTipflia,
                                //CuitCuilUno = int.Parse(result.CuitCuilUno.Trim()),
                                //CuitCuilDos = int.Parse(result.CuitCuilDos.Trim()),
                                Discapacitado = result.Discapacitado,
                                Minero = result.Minero,
                                Veterano = result.Veterano,                              
                                TipoFamiliaList = result.TipoFamiliaList,
                                IdDomicilio = result.IdDomicilio
                                
                            };

                            HttpContext.Session.SetObjectAsJson<AltaTitularViewModel>("altaTitular", alta);
                        }
                    }
                }
                else
                {
                    /*
                     * Esto es viejo porque 
                     * if (result.InsId == 0)
                    {
                        return View("ExisteInsc", result);
                        //si insId e menor que cero significa que eldni ya existe en base de grupoFamiliar
                        //se debe mostrar mensaje o un popup que diga que se debe hacercar al ivuj
                    }
                     */
                    result.InsNumdoc = numDni;
                    result.Existe = false;
                    //aqui agregar el dni al modelo de carga
                    var alta = new AltaTitularViewModel();
                    alta.InsNumdoc = numDni;
                    HttpContext.Session.SetObjectAsJson<AltaTitularViewModel>("altaTitular", alta);
                }

                //HttpContext.Session.SetObjectAsJson<UsuarioTitularViewModel>("viewTitularModelo", result); //cargo en cache el resultado
                //ViewBag.Header = string.Empty;
                //return RedirectToAction("AltaTitular");   //para que funcione en celular


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


        //[HttpPost]   para probar nueva pagina
        [HttpPost]       
        public IActionResult GuardarModelo(AltaTitularViewModel model)
        {
            var modelo = HttpContext.Session.GetObjectFromJson<AltaTitularViewModel>("altaTitular");

            try
            {
                var modeloCarga = new ModeloCarga 
                {
                     email=model.Usuario,
                     clave = model.Clave,
                     tipoFamilia = model.InsTipflia,
                     usuario = model.Usuario,
                     telefono = model.InsTelef ?? "0",
                     dni = modelo.InsNumdoc,   //solo para obtener el dni
                     cuitUno = model.CuitCuilUno.ToString(),
                     cuitTres = model.CuitCuilDos.ToString(),
                     nombre= model.InsNombre,
                     IdDomicilio = modelo.IdDomicilio ?? 0  //para obtener idDomicilio
                };
                
                var tokenSource = new CancellationTokenSource();
                var token = tokenSource.Token;
                /*     Produccion
                var result = this.apiAservice.PostAsync<ResponseViewModel>
                                 ("/Insc.Api/helper/", "PostModelo", modeloCarga, null, token).Result;
                

                var result = this.apiAservice.PostAsync<ResponseViewModel>
                                  ("/Test.Insc.Api/helper/", "PostModelo", modeloCarga, null, token).Result;
                 
                 */
                var result = this.apiAservice.PostAsync<ResponseViewModel>
                                 ("/Insc.Api/helper/", "PostModelo", modeloCarga, null, token).Result;


                if (result.IsSuccess)
                {
                    var modeloResponse = (ResponseViewModel)result.Result;
                    //si ok => Enviar Email

                    if (modeloResponse.Existe)
                    {
                        if (modeloResponse.InscriptoEnGrupoId > 0)
                        {
                            var mensaje = new UsuarioTitularViewModel();
                            mensaje.MensajeModel = "Usted Pertenece a un grupo familiar, por favor dirigirse al I.V.U.J. para regularizar su situación.";
                            return RedirectToAction("Mensaje", mensaje);
                            //return Json("Usted Pertenece a un grupo familiar, por favor dirigirse al I.V.U.J. para regularizar su situación.");
                        }
                        if (modeloResponse.UsuarioId > 0)
                        {
                            var mensaje = new UsuarioTitularViewModel();
                            mensaje.MensajeModel = "El Correo Electrónico ya esta en uso, por favor elija otro!";
                            return RedirectToAction("Mensaje", mensaje);

                            //return Json("El Correo Electrónico ya esta en uso, por favor elija otro!");
                        }
                        else
                        {
                            if (modeloResponse.InscriptoId == 1)
                            {
                                var mensaje = new UsuarioTitularViewModel();
                                mensaje.MensajeModel = "El correo ingresado ya esta en uso!";
                                return RedirectToAction("Mensaje", mensaje);

                                //return Json("El correo ingresado ya esta en uso!");
                            }
                            else
                            {
                                var mensaje = new UsuarioTitularViewModel();
                                mensaje.MensajeModel = "Disculpe las molestias, vuelva a cargar la página";
                                return RedirectToAction("Mensaje", mensaje);

                                //return Json("Disculpe las molestias, vuelva a cargar la página");
                            }
                        }

                    }
                    else
                    {
                        if (modeloResponse.UsuarioId == 0 || modeloResponse.InscriptoId == 0)
                        {
                            var mensaje = new UsuarioTitularViewModel();
                            mensaje.MensajeModel = "Disculpe las molestias, vuelva a cargar los datos";
                            return RedirectToAction("Mensaje", mensaje);

                            //return Json("Disculpe las molestias, vuelva a cargar los datos");
                        }
                        else
                        {
                            if (modeloResponse.UsuarioId > 0 && modeloResponse.InscriptoId > 0)
                            {
                                var mensaje = new UsuarioTitularViewModel();
                                mensaje.MensajeModel = "Se ha registrado con éxito como usuario/a del Sistema de Inscripción / Actualización de Datos del IVUJ. En la casilla que declaró recibirá un correo para validación puede demorar hasta 24hs. y luego podrá empezar a cargar datos.";
                                return RedirectToAction("Mensaje", mensaje);

                                //return Json("OK");
                                //RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                var mensaje = new UsuarioTitularViewModel();
                                mensaje.MensajeModel = "El correo ingresa ya esta en uso.";
                                return RedirectToAction("Mensaje", mensaje);

                               // return Json("El correo ingresa ya esta en uso.");
                            }
                        }
                    }

                }
                else
                {
                    var mensaje = new UsuarioTitularViewModel();
                    mensaje.MensajeModel = "Disculpe las molestias, vuelva a cargar la página";
                    return RedirectToAction("Mensaje", mensaje);

                    //return Json("Error");
                }

            }
            catch (Exception ex)
            {
                return Json("Operacion no Procesada " + ex.Message.Trim());
            }
        }

        [HttpPost]
        public IActionResult GoHome()
        {

            return RedirectToAction("Inscripcion");
        }

        
        public IActionResult Login(LoginViewModel model)
        {           
            return View(model);
        }

        public IActionResult Mensaje(UsuarioTitularViewModel mensaje)
        {
            return View("_msgModal", mensaje);
        }

        public IActionResult AltaTitular()
        {
            var modelo = HttpContext.Session.GetObjectFromJson<UsuarioTitularViewModel>("viewTitularModelo");

            if (modelo != null)
            {
                return View(modelo);
            }
            else
            {
                return View(new UsuarioTitularViewModel());
            }
        }

        
        public IActionResult AltaNuevoTitular()
        {
            return LocalRedirect("/Inscripcion/AltaTitular");
        }
    }
}







#region Metodos viejos comentados
/*
[HttpPost]
public async Task<IActionResult> SendMail([FromForm] MailRequest request)
{
    try
    {
        await mailService.SendEmailAsync(request);
        return Ok();
    }
    catch (Exception ex)
    {
        return Json("Operacion no Procesada " + ex.Message.Trim());
    }

}
*/
#endregion

