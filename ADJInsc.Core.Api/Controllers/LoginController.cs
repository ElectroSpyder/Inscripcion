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
            if(modeloCarga.usuario == null) return BadRequest("Usuario es requerido.");
            if(modeloCarga.clave == null) return BadRequest("Contraseña es requerido.");

            var result = loginService.GetLogin(modeloCarga.usuario.Trim(), modeloCarga.clave.Trim()).Result;
            await Task.Delay(500).ConfigureAwait(false);

            return result;
        }

        [HttpGet("/login/GetVerificador")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseViewModel>> GetVerificador(string id)
        {
            if(id == null) return BadRequest("Error, Guid incorrecto");
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
            if (email == null) return BadRequest("Debe proporcionar un email valido");

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
        public async Task<ActionResult<ResponseViewModel>> Recuperar(ModeloCarga modeloCarga)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(modeloCarga.dni))
                {
                    return BadRequest("El DNI es obligatorio.");
                }

                if (!int.TryParse(modeloCarga.dni, out int dni))
                {
                    return BadRequest("El DNI debe ser un número válido.");
                }
               
                if (dni < 1) return NotFound("Dni incorrecto");
                if (string.IsNullOrEmpty(modeloCarga.usuario)) return NotFound("Email vacío");

                var result = await loginService.Recuperar(modeloCarga.usuario.Trim(), dni);
                await Task.Delay(100).ConfigureAwait(false);

                if (!result.Equals(Guid.Empty))
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
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error : {ex.Message}");
            }
           
        }
    }
}
