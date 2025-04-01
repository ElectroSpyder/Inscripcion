namespace ADJInsc.Core.Service.Interface
{
    using ADJInsc.Models.Basic;
    using ADJInsc.Models.ViewModels;
    using ADJInsc.Models.ViewModels.AdhesionVM;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IApiService
    {        
        public Task<Response> GetAsync<T>(string prefix, string controller, CancellationToken cancellationToken);
        public Task<Response> PostAsync<T>(string prefix, string controller, ModeloCarga modelo, InscViewModel inscViewModel, CancellationToken cancellationToken);
        public Task<Response> PostAdhesionAsync<T>(string prefix, string controller, InscViewModel inscViewModel = null, AdhesionViewModel model = null, CancellationToken cancellationToken = default);
        public Task<Response> PostPdfAdhesionAsync<T>(string prefix, string controller, ModelPdf modelo = null, CancellationToken cancellationToken = default);
    }
}
