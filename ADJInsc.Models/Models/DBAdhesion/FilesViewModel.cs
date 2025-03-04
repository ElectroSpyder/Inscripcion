namespace ADJInsc.Models.Models.DBAdhesion
{
    using Microsoft.AspNetCore.Http;
    using System.Collections.Generic;
    public class FilesViewModel
    {
        public List<IFormFile> FilesModel { get; set; } = new List<IFormFile>();
    }
}
