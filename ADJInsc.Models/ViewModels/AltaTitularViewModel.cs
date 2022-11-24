namespace ADJInsc.Models.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using ADJInsc.Models.Models.DBInsc;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class AltaTitularViewModel
    {

        public string InsNumdoc { get; set; }

        [Required(ErrorMessage = "Debe ingresar Apellido y Nombre.")]
        [DataType(DataType.Text,ErrorMessage ="Debe ingresar solo letras.")]
        public string InsNombre { get; set; }

        public string InsTelef { get; set; }
        public string InsTipflia { get; set; }

        [Required(ErrorMessage = "Debe ingresar correo electrónico.")]
        [EmailAddress(ErrorMessage = "Debe ingresar un correo válido.")]      
        public string InsEmail { get; set; }

        [Required(ErrorMessage = "Debe ingresar el primer valor del CUIT/CUIL.")]
        public int CuitCuilUno { get; set; }

        [Required(ErrorMessage = "Debe ingresar el segundo valor del CUIT/CUIL.")]
        public int CuitCuilDos { get; set; }

        [Required(ErrorMessage = "Debe ingresar correo electrónico.")]
        [EmailAddress(ErrorMessage = "Debe ingresar un correo válido.")]
        [Compare("InsEmail", ErrorMessage = "El Email no coincide.")]       
        public string Usuario { get; set; }   //email
        
        [Required(ErrorMessage = "La contraseña es requerida.")]      
        [DataType(DataType.Password)]
        public string Clave { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida.")]
        [DataType(DataType.Password)]        
        [Compare("Clave", ErrorMessage = "La contraseña no coincide.")]
        public string ConfirmarClave { get; set; }

        public int Minero { get; set; }
        public int Veterano { get; set; }
        public int Discapacitado { get; set; }

        public int? IdDomicilio { get; set; }

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
