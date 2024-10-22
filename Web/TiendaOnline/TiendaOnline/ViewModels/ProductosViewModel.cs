using TiendaOnline.Models;

namespace TiendaOnline.ViewModels
{
    public class ProductosViewModel
    {
        public List<ProductoModel> Productos { get; set; }
        public List<TipoProductoModel> TipoProductos { get; set; }
    }
}
