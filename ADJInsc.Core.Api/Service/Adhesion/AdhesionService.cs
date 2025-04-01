namespace ADJInsc.Core.Api.Service.Adhesion
{
    using System;
    using System.Threading.Tasks;
   
    using ADJInsc.Core.Api.Service.Interface;
    using ADJInsc.Core.Logica;
    using ADJInsc.Models.ViewModels;
    using ADJInsc.Models.ViewModels.AdhesionVM;

    public class AdhesionService
    {
        public string _connectionString { get; set; }
        //private SqlConnection con;
        private AdhesionHelper helper;
        IMailService _mailService;

        public AdhesionService(string connectionString, IMailService mailService)
        {
            _connectionString = connectionString;
            _mailService = mailService;
            helper = new AdhesionHelper(_connectionString);

        }

        public async Task<AdhesionViewModel> GetProgramaYModulos(AdhesionViewModel model)
        {
            var result = await helper.GetProgramaYModulos(model);
            return result;
        }

        public async Task<InscViewModel> PostModeloAdhesion(InscViewModel inscViewModel)
        {
            var result = await helper.PostModeloAdhesion(inscViewModel);
            //var Helper = new Helper.EmailSender(result.CodigoVerificador.ToString(), model.email);
                        
            return result;
        }

        public async Task<ModelPdf> GetModelPdf(ModelPdf modelPdf)
        {
            var result = await helper.GetModelPdf(modelPdf);           

            return result;
        }
    }
}
