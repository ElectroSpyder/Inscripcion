namespace ADJInsc.Core.Controllers
{
    using ADJInsc.Core.Helper;
   
    using ADJInsc.Core.Service.Interface;
    using ADJInsc.Models.ViewModels;
    using ADJInsc.Models.ViewModels.AdhesionVM;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Extensions.Configuration;
    using Rotativa.AspNetCore;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using Wkhtmltopdf.NetCore;

    public class BandejaController : Controller
    {
        public IConfiguration Configuration { get; }
        public string _connectionString { get; set; }
        private readonly IApiService _apiService;
        //readonly IGeneratePdf _generatePdf;
        private DateTime Fecha => DateTime.Now;
        public BandejaController(IConfiguration configuration, IApiService apiService, IGeneratePdf generatePdf)
        {
            Configuration = configuration;
            _connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            _apiService = apiService;

            //_generatePdf = generatePdf;
        }

        [HttpPost]
        public ActionResult Index(string inputUserName, string inputPassword)
        {
            var modelOut = new ModeloCarga
            {
                usuario = inputUserName,
                clave = inputPassword,
                dni = "0",
                nombre = "0",
                email = "0",
                tipoFamilia = "0",
                cuitTres = "0",
                cuitUno = "0",
                IdDomicilio = 0,
                telefono = "0"

            };

            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            /*    Produccion
             *    var service = _apiService.PostAsync<ResponseViewModel>("/Insc.Api/login/", "GetLogin", modelOut, null, token).Result;
             
              var service = _apiService.PostAsync<ResponseViewModel>("/Test.Insc.Api/login/", "GetLogin", modelOut, null, token).Result;
             Test.Insc.Api
             */

            var service = _apiService.PostAsync<ResponseViewModel>("/Insc.Api/login/", "GetLogin", modelOut, null, token).Result;

            if (service.IsSuccess)
            {
                //var modelo = new InscViewModel();
                var modelo = (InscViewModel)service.Result;
                var numDni = modelo.InsNumdoc;

                if (!string.IsNullOrEmpty(numDni))
                {
                   
                    if (modelo.InsEstado == "S")
                    {
                        var model1 = new LoginViewModel
                        {
                            Email = "El DNI: " + modelo.InsNumdoc.ToString() + " se encuentra bajo Evaluación de Requisitos, consulte al IVUJ para mayor información. Gracias.",
                            Password = "3"
                        };
                        return View("Login", model1);
                    }

                    if (modelo.InsEstado == "A")
                    {
                        string[] words = modelo.InsEmail.Split('@');
                        var model1 = new LoginViewModel
                        {
                            Email = "****@" + words[1],
                            Password = "2"
                        };
                        return View("Login", model1);
                    }

                    //Aqui si Estado es Inscripto entonces obtener los datos del programa
                    //el modelo adhesion agregar el viewModel de inscripto
                    
                    //if (modelo.InsEstado == "I"  )
                    //{                        
                    //    modelo.AdhesionViewModel = GetAdhesionModel(modelo.InsId);
                    //    if (Verificado(modelo))
                    //    {
                    //        modelo.AdhesionViewModel.Habilitar = true;
                    //    }
                    //    else
                    //    {
                    //        modelo.AdhesionViewModel.Habilitar = false;
                    //    }                            
                    //}

                    HttpContext.Session.SetObjectAsJson<InscViewModel>("viewModelo", modelo);

                    return View("Index", modelo);
                }
                else
                {
                    if (modelo.InsEstado == "P")   //la clave no coincide
                    {
                        var model1 = new LoginViewModel
                        {
                            Email = "Clave incorrecta.",
                            Password = "4"
                        };
                        return View("Login", model1);
                    }
                    else
                    {
                        var model1 = new LoginViewModel
                        {
                            Email = "Usuario incorrecto.",
                            Password = "5"
                        };
                        return View("Login", model1);
                    }

                    //return RedirectToAction("Inscripcion", "Inscripcion");
                }


            }
            else
            {
                return RedirectToAction("Inscripcion", "Inscripcion");
            }

        }

        public JsonResult GetLocalidadesList(string departamentoKey)
        {
            var modelo = HttpContext.Session.GetObjectFromJson<InscViewModel>("viewModelo");

            // var localidades = modelo.LocalidadesList.Where(x => (x.Value.Split('-').ToString()) == departamentoKey);
            var localidades = new List<SelectListItem>();

            foreach (var item in modelo.LocalidadesList)
            {
                // var key = string.Concat(item.Value.Split('-'));
                int position = item.Value.IndexOf("-");
                var key = item.Value.Substring(0, position);
                if (key == departamentoKey)
                {
                    localidades.Add(new SelectListItem
                    {
                        Value = item.Value,
                        Text = item.Text
                    });
                }
            }
            //return Json(localidades);
            return Json(new SelectList(localidades, "Value", "Text"));
        }

        [HttpPost]
        public JsonResult AgregarPersona(string nombre, string dni, string parentescoKey, string fechaNac, string disc, string minero, string veterano)
        {
            var modelo = HttpContext.Session.GetObjectFromJson<InscViewModel>("viewModelo");
            int.TryParse(parentescoKey, out int pKey);
            int.TryParse(disc, out int pDisc);
            int.TryParse(minero, out int pMinero);
            int.TryParse(veterano, out int pVeterano);


            if (!DateTime.TryParse(fechaNac, out DateTime fecha)) return Json(modelo);

            if (!int.TryParse(dni, out int _dni)) return Json(modelo);

            var pDesc = string.Empty;
            foreach (var item in modelo.ParentescoList)
            {
                if (pKey.ToString() == item.Value)
                {
                    pDesc = item.Text.Trim();
                    break;
                }
            }

            var individuo = new GrupoFamiliarViewModel();
            var existe = false;

            foreach (var item in modelo.GrupoFamiliar)
            {
                if (item.InsfNumdoc == _dni)
                {
                    item.InsfNombre = nombre;
                    item.InsfNumdoc = _dni;
                    item.ParentescoKey = pKey;
                    item.ParentescoDesc = pDesc;
                    item.FechaNacimiento = fecha.ToShortDateString();
                    item.InsfDiscapacitado = pDisc;
                    item.InsfMinero = pMinero;
                    item.InsfVeterano = pVeterano;
                    item.InsfTipflia = modelo.InsTipflia;
                    item.InsfTipdoc = "0";
                    existe = true;
                    item.InsfEstado = "A";
                    item.FechaNacViewModel = fecha.Day + "/" + fecha.Month + "/" + fecha.Year;
                    item.FecNacDia = fecha.Day.ToString().Trim();
                    item.FecNacMes = fecha.Month.ToString().Trim();
                    item.FecNacAnio = fecha.Year.ToString().Trim();

                    individuo = new GrupoFamiliarViewModel
                    {
                        InsfNombre = nombre,
                        InsfNumdoc = _dni,
                        ParentescoKey = pKey,
                        ParentescoDesc = pDesc,
                        FechaNacimiento = fecha.ToShortDateString(),
                        InsfDiscapacitado = pDisc,
                        InsfMinero = pMinero,
                        InsfVeterano = pVeterano,
                        InsfTipflia = modelo.InsTipflia,
                        InsfTipdoc = "0",
                        InsfEstado = "M",
                        FechaNacViewModel = fecha.Day + "/" + fecha.Month + "/" + fecha.Year,
                        FecNacDia = fecha.Day.ToString().Trim(),
                        FecNacMes = fecha.Month.ToString().Trim(),
                        FecNacAnio = fecha.Year.ToString().Trim()
                    };


                    break;
                }
            }

            if (!existe)
            {

                foreach (var item in modelo.GrupoFamiliar)
                {
                    if (item.InsfNumdoc == _dni)
                    {
                        modelo.GrupoFamiliar.Remove(item);
                        break;
                    }
                }

                individuo = new GrupoFamiliarViewModel
                {
                    InsfNombre = nombre,
                    InsfNumdoc = _dni,
                    ParentescoKey = pKey,
                    ParentescoDesc = pDesc,
                    FechaNacimiento = fecha.ToShortDateString(),
                    InsfDiscapacitado = pDisc,
                    InsfMinero = pMinero,
                    InsfVeterano = pVeterano,
                    InsfTipflia = modelo.InsTipflia,
                    InsfTipdoc = "0",
                    InsfEstado = "A",
                    FechaNacViewModel = fecha.Day + "/" + fecha.Month + "/" + fecha.Year,
                    FecNacDia = fecha.Day.ToString().Trim(),
                    FecNacMes = fecha.Month.ToString().Trim(),
                    FecNacAnio = fecha.Year.ToString().Trim()
                };
                modelo.GrupoFamiliar.Add(individuo);

                HttpContext.Session.SetObjectAsJson<InscViewModel>("viewModelo", modelo);

                //var modelo = new InscViewModel();   para test
                return Json(individuo);
            }
            else
            {
                HttpContext.Session.SetObjectAsJson<InscViewModel>("viewModelo", modelo);

                //var modelo = new InscViewModel();   para test
                return Json(individuo);
            }

        }


        [HttpPost]
        public IActionResult DeletePersona(int id)
        {
            if (int.TryParse(id.ToString(), out int idFamilia))
            {
                var modelo = HttpContext.Session.GetObjectFromJson<InscViewModel>("viewModelo");
                foreach (var item in modelo.GrupoFamiliar)
                {
                    if (item.InsfNumdoc == id)
                    {
                        item.InsfEstado = "B";   //estado dado de baja para borrar en base
                        //modelo.GrupoFamiliar.Remove(item);
                        break;
                    }
                }
                HttpContext.Session.SetObjectAsJson<InscViewModel>("viewModelo", modelo);
                return PartialView("_DatosPartial", modelo);
            }
            else
            {
                return Json("DNI incorrecto.");
            }
        }

        [HttpPost]
        public JsonResult UpdatePersona(int id)
        {
            var modelo = new InscViewModel();
            modelo = HttpContext.Session.GetObjectFromJson<InscViewModel>("viewModelo");
            var individuo = new GrupoFamiliarViewModel();

            if (modelo.GrupoFamiliar != null && modelo.GrupoFamiliar.Any()) {
                foreach (var item in modelo.GrupoFamiliar)
                {
                    if (item.InsfNumdoc == id)
                    {
                        item.InsfEstado = "M";
                        individuo = item;
                        break;
                    }
                }
            }           

            HttpContext.Session.SetObjectAsJson<InscViewModel>("viewModelo", modelo);
            return Json(individuo);
        }

        public IActionResult GetPdfHome()
        {
            var modelo = HttpContext.Session.GetObjectFromJson<InscViewModel>("viewModelo");

            return new ViewAsPdf("_Reporte", modelo);

        }
        public JsonResult List()
        {
            InscViewModel modelo = new InscViewModel();
            modelo = HttpContext.Session.GetObjectFromJson<InscViewModel>("viewModelo");
            if (modelo == null)
            {
                return Json(null);
            }

            return Json(modelo);
        }

        public void ActualizarCheck(int id, string palabra)
        {
            var modelo = HttpContext.Session.GetObjectFromJson<InscViewModel>("viewModelo");
            switch (palabra)
            {
                case "disc":
                    {
                        modelo.InsDiscapacitado = id;
                        break;
                    }
                case "vet":
                    {
                        modelo.InsVeterano = id;
                        break;
                    }
                case "min":
                    {
                        modelo.InsMinero = id;
                        break;
                    }
                default:
                    break;
            }
            HttpContext.Session.SetObjectAsJson<InscViewModel>("viewModelo", modelo);

        }
        /*System.NullReferenceException: Object reference not set to an instance of an object.
   at ADJInsc.Core.Controllers.BandejaController.GuardarData(String dni, String nombre, String tipoFamilia, Int32 disc, Int32 minero, Int32 veterano, String cuitUno, String cuitTres, String direccion, String departamento, String localidad, String lugarTrabajo, String revista, String neto, String telefono) in D:\Remoto\ADJInsc.Core\Controllers\BandejaController.cs:line 403*/
        public JsonResult GuardarData(string dni, string nombre, string tipoFamilia, int disc, int minero, int veterano, string cuitUno, string cuitTres,
                                      string direccion, string departamento, string localidad, string lugarTrabajo, string revista, string neto, string telefono)
        {
            var modelo = new InscViewModel();
            modelo = HttpContext.Session.GetObjectFromJson<InscViewModel>("viewModelo");


            if (string.IsNullOrWhiteSpace(localidad))
            {
                localidad = "12-1";
            }
            if (string.IsNullOrWhiteSpace(departamento))
            {
                departamento = "1";
            }

            var keyLoc = localidad.Split("-");
            var desKeyLoc = string.Empty;
            var desKeyDep = string.Empty;
            var tipoFamiliaDesc = string.Empty;
            var tipoRevistaDesc = string.Empty;

            if (modelo.TipoFamiliaList != null && modelo.TipoFamiliaList.Any())
            {
                foreach (var item in modelo.TipoFamiliaList)
                {
                    if (tipoFamilia == item.Value.ToString())
                    {
                        tipoFamiliaDesc = item.Text.Trim();
                        break;
                    }
                }
            }
           
            if(modelo.LocalidadesList!= null && modelo.LocalidadesList.Any())
            {
                foreach (var item in modelo.LocalidadesList)
                {
                    if (localidad == item.Value.ToString())
                    {
                        desKeyLoc = item.Text.Trim();
                        break;
                    }
                }
            }
            
            if(modelo.DepartamentosList!= null && modelo.DepartamentosList.Any())
            {
                foreach (var item in modelo.DepartamentosList)
                {
                    if (departamento == item.Value)
                    {
                        desKeyDep = item.Text.Trim();
                        break;
                    }
                }
            }
            
            if(modelo.TipoRevistaList != null && modelo.TipoRevistaList.Any())
            { 
                foreach (var item in modelo.TipoRevistaList)
                {
                    if (revista == item.Value)
                    {
                        tipoRevistaDesc = item.Text.Trim();
                        break;
                    }
                }
            }

           

            modelo.InsTipflia = tipoFamiliaDesc;
            modelo.IdTipoFamilia = int.Parse(tipoFamilia);

            if (!int.TryParse(dni, out int _dni)) return Json(modelo);

            modelo.InsNumdoc = _dni.ToString().Trim();
            modelo.InsTipdoc = "0";
            modelo.InsNombre = nombre;

            modelo.InsDiscapacitado = disc;
            modelo.InsMinero = minero;
            modelo.InsVeterano = veterano;

            modelo.CuitCuil = cuitUno + "-" + _dni.ToString().Trim() + '-' + cuitTres;
            modelo.CuitCuilUno = cuitUno.Trim();
            modelo.CuitCuilDos = cuitTres.Trim();

            if (string.IsNullOrEmpty(direccion))
            {
                modelo.Direccion = "s/d";
            }
            else
            {
                modelo.Direccion = direccion;
            }

            modelo.DepartamentoKey = int.Parse(departamento);
            modelo.DepartamentoDesc = desKeyDep;
            modelo.LocalidadKey = int.Parse(keyLoc[1]);  //1-12  ES EL 12
            modelo.LocalidadDesc = desKeyLoc;
            if (string.IsNullOrEmpty(lugarTrabajo))
            {
                modelo.NombreEmpleo = "s/n";
            }
            else
            {
                modelo.NombreEmpleo = lugarTrabajo.Trim();
            }


            modelo.TipoRevistaKey = int.Parse(revista);
            modelo.TipoRevistaDesc = tipoRevistaDesc;
            if (string.IsNullOrEmpty(neto))
            {
                modelo.IngresoNeto = "0";
            }
            else
            {
                modelo.IngresoNeto = neto;
            }

            modelo.InsTelef = telefono;
            modelo.Barrio = "0";

            modelo.InsFecins = DateTime.Now.ToShortDateString();

            modelo.InsEstado = "I";    //Usuario inscripto
            foreach (var item in modelo.GrupoFamiliar)
            {

                item.InsfFicha = modelo.InsFicha;   //no importa, vuelvo a copiar el numero de ficha
                if (item.InsfEstado == "B")
                {
                    item.FechaNacViewModel = DateTime.Now.ToShortDateString();
                }
                else
                {
                    item.FecNacDia ??= DateTime.Now.Day.ToString();
                    item.FecNacMes ??= DateTime.Now.Month.ToString();
                    item.FecNacAnio ??= DateTime.Now.Year.ToString();
                }
            }

            HttpContext.Session.SetObjectAsJson<InscViewModel>("viewModelo", modelo);

            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            /*   Produccion
                var service = this._apiService.PostAsync<ResponseViewModel>("/Insc.Api/helper/", "PostInscViewModel", null, modelo, token).Result;
             var service = this._apiService.PostAsync<ResponseViewModel>("/Test.Insc.Api/helper/", "PostInscViewModel", null, modelo, token).Result;
             Test.Insc.Api
             */

            var service = this._apiService.PostAsync<ResponseViewModel>("/Insc.Api/helper/", "PostInscViewModel", null, modelo, token).Result;

            if (service.IsSuccess)
            {
                var respuesta = (ResponseViewModel)service.Result;

                if (respuesta.Existe)
                {

                    var modeloNuevo = HttpContext.Session.GetObjectFromJson<InscViewModel>("viewModelo");
                    var grupoNuevo = new List<GrupoFamiliarViewModel>();
                    foreach (var item in modeloNuevo.GrupoFamiliar)
                    {
                        if (item.InsfEstado != "B")
                        {
                            grupoNuevo.Add(item);
                        }
                    }
                    modeloNuevo.GrupoFamiliar = grupoNuevo;
                    HttpContext.Session.SetObjectAsJson<InscViewModel>("viewModelo", modeloNuevo);

                    //4_ rotativa_ retorno el json pero sin valor
                    var redirectUrl1 = Url.Action("GetPdfHome", "Bandeja");
                    return Json(new
                    {
                        redirectUrl = redirectUrl1,
                        isRedirect = true,
                        ob = respuesta.Observacion
                    });

                }
                else
                {

                    return Json(new
                    {
                        redirectUrl = "",
                        isRedirect = false,
                        ob = respuesta.Observacion
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
                
        public AdhesionViewModel GetAdhesionModel(int insId)
        {
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            var modelOut = new AdhesionViewModel
            {
                InscriptoId = insId
            };

            /*
             var service = _apiService.PostAdhesionAsync<ResponseViewModel>("/Test.Insc.Api/adhesion/", "GetAdhesionModel",null, modelOut, token).Result;
            var service = _apiService.PostAdhesionAsync<ResponseViewModel>("/Insc.Api/adhesion/", "GetAdhesionModel",null, modelOut, token).Result;
             */
            var service = _apiService.PostAdhesionAsync<ResponseViewModel>("/Insc.Api/adhesion/", "GetAdhesionModel", null, modelOut, token).Result;
            var result = (AdhesionViewModel)service?.Result;

            if (result.Success)
                   modelOut = result;
            
            return modelOut;
        }

        public bool Verificado(InscViewModel model)
        {
            if (model.AdherirViewModel.AdhesionId != 0) return false; //si ya esta no comparo nada

            string[] formatos = { "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "yyyy-MM-dd" };                     

            if (!DateTime.TryParseExact(model.InsFecins, formatos, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fechaConvertida))
                return false;

            var fechaInicioComparar = ConvertirFechaInicio(model.AdhesionViewModel.ProgramaVM.FechaInicio); 

            bool verificado =fechaConvertida.Date <= fechaInicioComparar.Date;
                           
            if (!verificado)
                return false; // Retorna false si la fecha no está en el rango permitido

            return model.AdhesionViewModel.DepartamentoProgramas.Any(x =>
                x.DepartamentoId == model.DepartamentoKey &&
                (x.LocalidadId <= 0 || x.LocalidadId == model.LocalidadKey));

        }

        private DateTime ConvertirFechaInicio(string fechaString)
        {
            if(string.IsNullOrEmpty(fechaString)) return DateTime.MinValue;

            string[] partes = fechaString.Split('/');

            string dia = partes[0];  // "03"
            string mes = partes[1];  // "11"
            string anio = partes[2]; // "2025"

            Console.WriteLine($"Día: {dia}");
            Console.WriteLine($"Mes: {mes}");
            Console.WriteLine($"Año: {anio}");
            return DateTime.Parse(mes+"/"+ dia+"/" + anio);
        }
        public JsonResult Adherir(int ProgramaId, int moduloId)
        {
            // public int InsId { get; set; }
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            var modeloVM = HttpContext.Session.GetObjectFromJson<InscViewModel>("viewModelo");

            var modeloAdherir = new AdherirViewModel
            {
                InscriptoId = modeloVM.InsId,
                ModuloId = moduloId,
                ProgramaId = ProgramaId,
                FechaAdhesion = Fecha.ToString()
            };

            var modelo = new InscViewModel
            {
                AdherirViewModel = modeloAdherir
            };

            /*
             var service = this._apiService.PostAsync<ResponseViewModel>("/Insc.Api/helper/", "PostAdherir", null, modelo, token).Result;
            var service = this._apiService.PostAsync<ResponseViewModel>("/Test.Insc.Api/helper/", "PostAdherir", null, modelo, token).Result;
             */
            var service = this._apiService.PostAsync<ResponseViewModel>("/Insc.Api/helper/", "PostAdherir", null, modelo, token).Result;
            if (service.IsSuccess)
            {
                var respuesta = (ResponseViewModel)service.Result;

                if (respuesta.Existe)
                {

                    var modeloNuevo = HttpContext.Session.GetObjectFromJson<InscViewModel>("viewModelo");
                    var grupoNuevo = new List<GrupoFamiliarViewModel>();
                    foreach (var item in modeloNuevo.GrupoFamiliar)
                    {
                        if (item.InsfEstado != "B")
                        {
                            grupoNuevo.Add(item);
                        }
                    }
                    modeloNuevo.GrupoFamiliar = grupoNuevo;
                    HttpContext.Session.SetObjectAsJson<InscViewModel>("viewModelo", modeloNuevo);

                    //4_ rotativa_ retorno el json pero sin valor
                    var redirectUrl1 = Url.Action("GetPdfHome", "Bandeja");
                    return Json(new
                    {
                        redirectUrl = redirectUrl1,
                        isRedirect = true,
                        ob = respuesta.Observacion
                    });

                }
                else
                {
                    return Json(new
                    {
                        redirectUrl = "",
                        isRedirect = false,
                        ob = respuesta.Observacion
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
    }
}
