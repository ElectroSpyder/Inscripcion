namespace ADJInsc.Models.ViewModels.AdhesionVM
{
    using System.ComponentModel.DataAnnotations;

    public class ModulosVM
    {
        public int ModuloId { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public int Plazo { get; set; }
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal Costo { get; set; }
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal IngresoMinimo { get; set; }
        public int Estado { get; set; }
        public virtual int ProgramaId { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal CostoSinEntrega { get; set; }
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal CostoConEntrega { get; set; }
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal CostoUvis { get; set; }
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal IngresoMinimoUvis { get; set; }
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal CostoSinEntregaUvis { get; set; }
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal CostoConEntregaUvis { get; set; }
    }
}
