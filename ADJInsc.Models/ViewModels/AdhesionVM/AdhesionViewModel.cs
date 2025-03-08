namespace ADJInsc.Models.ViewModels.AdhesionVM
{
    using System.Collections.Generic;
    public class AdhesionViewModel
    {
        public ProgramaVM ProgramaVM { get; set; } = new ProgramaVM();
        public IEnumerable<ModulosVM> ModulosVM { get; set; } = new List<ModulosVM>();
        public IEnumerable<DepartamentoProgramaVM> DepartamentoProgramas { get; set; } = new List<DepartamentoProgramaVM>();
        public bool Success { get; set; } //Ok, Error
        public string ErrorMsg { get; set; }
        public bool Habilitar { get; set; } = false;
        public int InscriptoId { get; set; }
        public int AdhesionId { get; set; }
        public string FechaAdhesion { get; set; }
        public string DescripcionModulo { get; set; }
    }
}
