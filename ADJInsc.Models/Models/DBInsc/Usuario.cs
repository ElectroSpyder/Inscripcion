﻿using System;
using System.Collections.Generic;

namespace ADJInsc.Models.Models.DBInsc
{
    public partial class Usuario
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; }
        public string ClaveUsuario { get; set; }
        public DateTime? UsuFecalt { get; set; }
        public string UsuEstado { get; set; }
    }
}
