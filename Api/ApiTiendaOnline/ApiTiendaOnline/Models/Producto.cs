using System;
using System.Collections.Generic;

namespace ApiTiendaOnline.Models
{
    public partial class Producto
    {
        public int IdProducto { get; set; }
        public string? Nombre { get; set; }
        public string? Tallas { get; set; }
        public string? Color { get; set; }
        public decimal? Precio { get; set; }
        public string? Descripcion { get; set; }
        public string? Imagen { get; set; }
        public int? Stock { get; set; }
        public int? IdTipoProducto { get; set; }

        public virtual TipoProducto? TipoProducto { get; set; }
    }
}
