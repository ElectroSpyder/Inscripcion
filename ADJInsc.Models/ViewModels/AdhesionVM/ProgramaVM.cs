using System;
using System.ComponentModel.DataAnnotations;

namespace ADJInsc.Models.ViewModels.AdhesionVM
{
    public class ProgramaVM
    {
        public int ProgramaId { get; set; }
        public string Descripcion { get; set; }
        public string FechaInicio { get; set; }
        public string FechaLimite { get; set; }
        public int Estado { get; set; }
        public string FechaCortePrograma { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal MontoAdhesion { get; set; }
    }
}
