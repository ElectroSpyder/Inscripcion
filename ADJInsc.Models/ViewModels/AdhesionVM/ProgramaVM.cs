using System;
using System.ComponentModel.DataAnnotations;

namespace ADJInsc.Models.ViewModels.AdhesionVM
{
    public class ProgramaVM
    {
        public int ProgramaId { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaLimite { get; set; }
        public int Estado { get; set; }
        public DateTime FechaCortePrograma { get; set; }
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal MontoAdhesion { get; set; }
    }
}
