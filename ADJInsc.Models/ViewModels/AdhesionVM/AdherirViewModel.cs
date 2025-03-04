namespace ADJInsc.Models.ViewModels.AdhesionVM
{
    using System;
    public class AdherirViewModel
    {
        public int ProgramaId { get; set; }
        public int AdhesionId { get; set; }
        public int InscriptoId { get; set; }
        public int ModuloId { get; set; }
        public DateTime FechaAdhesion { get; set; }
        public string Descripcion { get; set; }
    }
}
