namespace ADJInsc.Models.ViewModels.AdhesionVM
{
    using System;
    public class AdherirViewModel
    {
        public int ProgramaId { get; set; }
        public int AdhesionId { get; set; }
        public int InscriptoId { get; set; }
        public int ModuloId { get; set; }
        public string ModuloDescripcion { get; set; }
        public string FechaAdhesion { get; set; }
        public string Descripcion { get; set; }
        public string CuitCuilUno { get; set; }
        public string CuitCuilDos { get; set; }
        public string InsNumdoc { get; set; }
        public string NombreApellidoCotitular { get; set; }
        public string RelacionLaboral { get; set; }
        //public string LugarLaboral { get; set; }
    }
}
