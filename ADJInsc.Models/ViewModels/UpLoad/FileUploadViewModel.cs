namespace ADJInsc.Models.ViewModels.UpLoad
{
    public class FileUploadViewModel
    {
        public int ArchivoId { get; set; } // Número para el directorio
        public string Fecha { get; set; } // Fecha de subida
        public string NombreArchivo { get; set; } // Nombre del archivo
        public long Tamano { get; set; } // Tamaño del archivo en bytes
        public string TipoContenido { get; set; } // Tipo MIME del archivo
        public int InscriptoId { get; set; }
        public string RutaArchivo { get; set; }
        public byte[] FileContent { get; set; } // Guardamos el archivo en memoria
        public int AdheridoId { get; set; }
    }

}
