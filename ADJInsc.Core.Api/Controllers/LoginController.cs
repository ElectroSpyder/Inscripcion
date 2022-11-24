namespace ADJInsc.Core.Api.Controllers
{
    using System;
    using System.Threading.Tasks;
    using ADJInsc.Core.Api.Service.Login;
    using ADJInsc.Core.Logica.Service.Interface;
    using ADJInsc.Models.ViewModels;
    //using ADJInsc.Core.Logica.Service.Login;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
        
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        public IConfiguration Configuration { get; }
        public string _connectionString { get; set; }
        //private readonly IMailService _mailService;
        private readonly LoginService loginService;

        public LoginController(IConfiguration configuration)
        {
            Configuration = configuration;
            _connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            //_mailService = mailService;
            loginService = new LoginService(_connectionString);
        }

        [HttpPost("/login/GetLogin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<InscViewModel>> GetLogin(ModeloCarga modeloCarga)
        {
            //var service = new LoginService(_connectionString);
            var result = loginService.GetLogin(modeloCarga.usuario.Trim(), modeloCarga.clave.Trim()).Result;
            await Task.Delay(500).ConfigureAwait(false);

            return result;
        }

        [HttpGet("/login/GetVerificador")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseViewModel>> GetVerificador(string id)
        {
           // var service = new LoginService(_connectionString);
            var result = loginService.VerificarGuid(new Guid(id)).Result;
            await Task.Delay(100).ConfigureAwait(false);

            if (result)
            {
                return new ResponseViewModel()
                {
                    CodigoVerificador = Guid.Empty,
                    Existe = true,
                    InscriptoId = 0,
                    UsuarioId = 0
                };
            }
            else
            {
                return new ResponseViewModel()
                {
                    CodigoVerificador = Guid.Empty,
                    Existe = false,
                    InscriptoId = 0,
                    UsuarioId = 0
                };
            }
        }

        [HttpGet("/login/GetResetPasword")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetResetPasword(string email)
        {
            //var service = new LoginService(_connectionString);
            var result = loginService.ResetPasword(email).Result;
            await Task.Delay(100).ConfigureAwait(false);

            if (!result.Equals(Guid.Empty))
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost("/login/Recuperar")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string>> Recuperar(ModeloCarga modeloCarga)
        {
            try
            {
                var dni = int.Parse(modeloCarga.dni);
                if (dni < 1) return NotFound("Dni incorrecto");
                if (string.IsNullOrEmpty(modeloCarga.usuario)) return NotFound("Email vacío");

                var result = await loginService.Recuperar(modeloCarga.usuario.Trim(), dni);
                //await Task.Delay(500).ConfigureAwait(false);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error : {ex.Message}");
            }
           
        }
    }
}
