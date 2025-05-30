using ADJInsc.Models.ViewModels;
using System.Threading.Tasks;
using ADJInsc.Core.Logica;
using ADJInsc.Core.Api.Service.Interface;

namespace ADJInsc.Core.Api.Service.Inscripcion
{    
    

    public class InscripcionService
    {
        public string _connectionString { get; set; }
        //private SqlConnection con;
        private LogicHelper helper;
        IMailService _mailService;

        public InscripcionService(string connectionString, IMailService mailService)
        {
            _connectionString = connectionString;
            _mailService = mailService;
            helper = new LogicHelper(_connectionString);
            
        }
     
        public async Task<ListadosViewModel> GetListadosInciales()
        {
            var result = await helper.GetListadosInciales();
            return  result; 
        }

        
        public async Task<ResponseViewModel> PostServerModelo(ModeloCarga model)
        {
            var result = await helper.PostServerModelo(model);
            if(result.CodigoVerificador == null) return null;


            var Helper = new Helper.EmailSender(result.CodigoVerificador.ToString(), model.email);

            await Helper.SendEmail();
            return result;
        }

        public async Task<ResponseViewModel> PostInscViewModel(InscViewModel model)
        {
            var result = await helper.PostInscViewModel(model);
            
            return result;
        }

        public async Task<UsuarioTitularViewModel> GetInscripto(int dni)
        {
            var result = await helper.GetInscripto(dni);
            return result;
        }

        public async Task<UsuarioTitularViewModel> GetInscriptoGF(int dni)
        {
            var result = await helper.GetInscriptoGrupo(dni);
            return result;
        }

    }
}
