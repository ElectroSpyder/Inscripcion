using System;
using System.Collections.Generic;

namespace ADJInsc.Models.Models.DBAdhesion
{
    public partial class Modulo
    {
        public int ModuloId { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public int Plazo { get; set; }
        public decimal Costo { get;set; }
        public decimal CostoSinEntrega { get; set; }
        public decimal CostoConEntrega { get; set; }
        public decimal IngresoMinimo { get; set; }
        public Int16 Estado { get; set; }
        public virtual ProgramaAdhesion ProgramaId { get; set; }     
        public virtual ICollection<Adhesion> Adhesiones { get; set; }
    }
}
