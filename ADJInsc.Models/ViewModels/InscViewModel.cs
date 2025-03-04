namespace ADJInsc.Models.ViewModels
{
    using ADJInsc.Models.Models.DBInsc;
    using ADJInsc.Models.ViewModels.AdhesionVM;
    using ADJInsc.Models.ViewModels.UpLoad;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.Collections.Generic;

    public class InscViewModel: Inscriptos
    {
        public InscViewModel()
        {
           GrupoFamiliar = new List<GrupoFamiliarViewModel>();
        }
        
        public int DepartamentoKey { get; set; }
        public string DepartamentoDesc { get; set; }
        
        public int LocalidadKey { get; set; }
        public string LocalidadDesc { get; set; }

        public int ParentescoKey { get; set; }
        public string ParentescoDesc { get; set; }

        public int InsFFamiliaKey { get; set; }
        public IList<GrupoFamiliarViewModel> GrupoFamiliar { get; set; }

        public string FechaAltaViewModel {
            get
            {
                return string.Format("{0:dd-MM-yyyy}", InsFecalt);
            } 
        }

        public string Usuario { get; set; }
        public string Clave { get; set; }

        public string Direccion { get; set; }
        public string Barrio { get; set; }
       // public int DomDepartamentoKey { get; set; }
        public string DomLocalidadKey { get; set; }

        public int SituacionLaboralId { get; set; }
        public int TipoEmpleoKey { get; set; }
        public string TipoRevistaDesc { get; set; }
        public int TipoRevistaKey { get; set; }
        public string IngresoNeto { get; set; }
        public string NombreEmpleo { get; set; }

        public bool Minero {
            get
            {
                return InsMinero == 1 ? true : false;
            }
        }
        public bool Veterano
        {
            get
            {
                return InsVeterano == 1 ? true : false;
            }
        }
        public bool Discapacitado
        {
            get
            {
                return InsDiscapacitado == 1 ? true : false;
            }
        }               

        public IEnumerable<SelectListItem> TipoFamiliaList { get; set; }
        public IEnumerable<SelectListItem> LocalidadesList { get; set; }
        public IEnumerable<SelectListItem> DepartamentosList { get; set; }
        public IEnumerable<SelectListItem> ParentescoList { get; set; }
        public IEnumerable<SelectListItem> TipoEmpleoList { get; set; }

        public IEnumerable<SelectListItem> TipoRevistaList { get; set; }
        
        public AdhesionViewModel AdhesionViewModel { get; set; } = new AdhesionViewModel();
        public AdherirViewModel AdherirViewModel { get; set; } = new AdherirViewModel();
        public List<FileUploadViewModel> FileUploadViewModel { get; set; } = new List<FileUploadViewModel>();
    }
}