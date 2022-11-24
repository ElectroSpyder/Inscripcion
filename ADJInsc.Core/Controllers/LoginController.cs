namespace ADJInsc.Core.Controllers
{    
    using System.Threading;
    using ADJInsc.Core.Service.Interface;
    using ADJInsc.Models.ViewModels;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    public class LoginController : Controller
    {
        public IConfiguration Configuration { get; }
        public string _connectionString { get; set; }
        //private readonly IMailService mailService;
        private readonly IApiService _apiService;

        public LoginController(IConfiguration configuration, IApiService apiAservice)
        {
            Configuration = configuration;
            _connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            // this.mailService = mailService;
            _apiService = apiAservice;
        }
               
        [HttpPost]
        public ActionResult Login(string inputUserName, string inputPassword)
        {
            var modelOut = new ModeloCarga
            {
                usuario = inputUserName,
                clave = inputPassword,
                dni = "0",
                nombre = "0",
                //apellido = "0",
                email = "0",
                tipoFamilia = "0"
            };

            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            /*Produccion
              var service = _apiService.PostAsync<ResponseViewModel>("/Test.Insc.Api/login/", "GetLogin", modelOut, null, token).Result;

            var service = _apiService.PostAsync<ResponseViewModel>("/Insc.Api/login/", "GetLogin", modelOut, null, token).Result;
             ;
            Test.Insc.Api
             */

            var service = _apiService.PostAsync<ResponseViewModel>("/Insc.Api/login/", "GetLogin", modelOut, null, token).Result;

            if (service.IsSuccess)
            {
                var modelo = (InscViewModel)service.Result;
                TempData["data"] = modelo;
                return View();
               
            }
            else
            {
                return View();
            }
            
        }

        // GET api/<VerificadorController>/5
        [HttpGet("{email}")]
        public IActionResult GetReset(string email)
        {
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            /*Produccion   
                // 
   // var service = _apiService.GetAsync<bool>("/Test.Insc.Api/api/Login/", "GetResetPasword" + "?email=" + email, token).Result;

             var service = _apiService.GetAsync<bool>("/Insc.Api/api/Login/", "GetResetPasword" + "?email=" + email, token).Result;
            Test.Insc.Api 
             */
            var service = _apiService.GetAsync<bool>("/Insc.Api/api/Login/", "GetResetPasword" + "?email=" + email, token).Result;

            if (!service.IsSuccess)
            {
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Respuesta", "Home");
        }

        [HttpPost()]
        public IActionResult Recuperar(string inputRecuperarUserName, int inputRecuperarDni)
        {
            var modelOut = new ModeloCarga
            {
                usuario = inputRecuperarUserName.Trim(),
                clave = string.Empty,
                dni = inputRecuperarDni.ToString().Trim(),
                nombre = "0",
                //apellido = "0",
                email = "0",
                tipoFamilia = "0"
            };

            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
           
              var service = _apiService.PostAsync<ResponseViewModel>("/Insc.Api/login/", "Recuperar", modelOut, null, token).Result;

            if (service.IsSuccess)
            {
                var modelo = service.Result.ToString();
                var model = new LoginViewModel
                {
                    Email = inputRecuperarUserName.Trim(),
                    Password = "2"
                };

                if (modelo == "Ok") model.Password = "1";               

                return View("Index", model);  //mostrar mensaje y volver a inicio

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
