using System;
using System.Collections.Generic;

namespace ADJInsc.Models.Models.DBAdhesion
{
    public partial class ProgramaAdhesion
    {
        public int ProgramaId { get; set; }        
        public string Descripcion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaLimite { get; set; }
        public int Estado { get; set; }
        public DateTime FechaCortePrograma { get; set; }
        public decimal MontoAdhesion { get; set; }
        public virtual ICollection<Modulo> Modulos { get; set; }
        public virtual ICollection<DepartamentoPrograma> DepartamentoPrograma { get; set; }
    }
}
