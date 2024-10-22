using TiendaOnline.Models;

namespace TiendaOnline.ViewModels
{
    public class ProductoViewModel
    {
        public ProductoModel Producto { get; set; }
        public List<TipoProductoModel>? TipoProductos { get; set; }

     
    }
}
