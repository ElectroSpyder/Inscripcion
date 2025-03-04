namespace ADJInsc.Core.Api.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using ADJInsc.Core.Api.Service.Interface;
    using ADJInsc.Models.ViewModels;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Extensions.Configuration;
    using ADJInsc.Core.Api.Service.Inscripcion;
    using ADJInsc.Core.Api.Service.Adhesion;
    using ADJInsc.Models.ViewModels.AdhesionVM;
    using ADJInsc.Core.Api.Service.Login;

    [ApiController]
    [Route("[controller]")]
    public class AdhesionController : ControllerBase
    {
        public IConfiguration Configuration { get; }
        public string _connectionString { get; set; }

        private readonly AdhesionService service;
        public AdhesionController(IConfiguration configuration, IMailService mailService)
        {
            Configuration = configuration;
            _connectionString = Configuration["ConnectionStrings:DefaultConnection"];

            service = new AdhesionService(_connectionString, mailService);
        }

        // POST api/<HelperController>
        [HttpPost("/adhesion/PostModeloAdhesion")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<InscViewModel>> PostModeloAdhesion(InscViewModel model)
        {
            var result = await service.PostModeloAdhesion(model);           
           
            await Task.Delay(500).ConfigureAwait(false);

            return result;
        }


        [HttpGet("/adhesion/GetAdhesionModel")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<AdhesionViewModel> GetAdhesionModel()
        {
            //var helper = new InscripcionService(_connectionString, mailService);
            var model = service.GetProgramaYModulos().Result;

            await Task.Delay(100).ConfigureAwait(false);
            return model;

        }
    }
}
