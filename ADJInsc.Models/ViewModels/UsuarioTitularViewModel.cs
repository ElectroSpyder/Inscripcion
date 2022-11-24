namespace ADJInsc.Models.ViewModels
{    
    using ADJInsc.Models.Models.DBInsc;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class UsuarioTitularViewModel : Inscriptos
    {         
        public string Usuario { get; set; }   //email
        public string Clave { get; set; }

        public bool Existe { get; set; }

        public string Direccion { get; set; }
        public string Barrio { get; set; }
        public int DomDepartamentoKey { get; set; }
        public int DomLocalidadKey { get; set; }
        public string DomEstado { get; set; }

        public int SituacionLaboralId { get; set; }
        public int TipoEmpleoKey { get; set; }
        public int TipoRevistaKey { get; set; }
        public string IngresoNeto { get; set; }
        public string NombreEmpleo { get; set; }

        public int Minero { get; set; }
        public int Veterano { get; set; }
        public int Discapacitado { get; set; }

        public IEnumerable<SelectListItem> TipoFamiliaList { get; set; }
        public IList<GrupoFamiliarViewModel> GrupoFamiliar { get; set; }

        public string MensajeModel { get; set; }
        public List<TipoFamilia> GetListado()
        {
            var tipoFamiliaList = new List<TipoFamilia>();
            tipoFamiliaList.Add(new TipoFamilia
            {
                TipoFamiliaKey = 0,
                TipoFamiliaDesc = " "
            });
            tipoFamiliaList.Add(new TipoFamilia
            {
                TipoFamiliaKey = 1,
                TipoFamiliaDesc = "SOLTERO/A"
            });
            tipoFamiliaList.Add(new TipoFamilia
            {
                TipoFamiliaKey = 2,
                TipoFamiliaDesc = "CASADO/A"
            });
            tipoFamiliaList.Add(new TipoFamilia
            {
                TipoFamiliaKey = 3,
                TipoFamiliaDesc = "UNIÓN CONVIVENCIAL (Insc.en Reg.Civil)"
            });
            tipoFamiliaList.Add(new TipoFamilia
            {
                TipoFamiliaKey = 4,
                TipoFamiliaDesc = "UNIÓN CONVIVENCIAL DE HECHO(No Insc.en Reg.Civil)"
            });
            tipoFamiliaList.Add(new TipoFamilia
            {
                TipoFamiliaKey = 5,
                TipoFamiliaDesc = "DIVORCIADO/A"
            });
            tipoFamiliaList.Add(new TipoFamilia
            {
                TipoFamiliaKey = 6,
                TipoFamiliaDesc = "VIUDO/A"
            });
            tipoFamiliaList.Add(new TipoFamilia
            {
                TipoFamiliaKey = 7,
                TipoFamiliaDesc = "SEPARADO/A DE HECHO"
            });
            return tipoFamiliaList;
        }
    }
}
