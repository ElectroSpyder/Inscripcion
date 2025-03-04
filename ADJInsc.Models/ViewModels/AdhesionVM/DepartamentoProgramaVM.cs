using System;

namespace ADJInsc.Models.ViewModels.AdhesionVM
{
    public class DepartamentoProgramaVM
    {
        public int DepartamentoProgramaId { get; set; }
        public int ProgramaId { get; set; }
        public int DepartamentoId { get; set; }
        public int LocalidadId { get; set; }
        public DateTime FechaHoy {
            get { return DateTime.Now; } 
         }
    }
}
