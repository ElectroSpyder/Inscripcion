using ADJInsc.Models.Models.DBInsc;
using System;

namespace ADJInsc.Models.Models.DBAdhesion
{
    /// <summary>
    /// Tabla tipo TEA que relaciona Inscripto con Modulo
    /// </summary>
    public partial class Adhesion
    {
        public int AdhesionId { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaAdhesion { get; set; }
        public Int16 Estado { get; set; }
        public string DescripcionEstado { get; set; }        
        public Modulo Modulo { get; set; }
        public int ModuloId { get; set; }
        public Inscriptos Inscripto { get; set; }
        public int InsId { get; set; }
        public int ProgramaId { get; set; }
    }
}
