using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TiendaOnline.Enums;

namespace TiendaOnline.Models
{
    public class ProductoModel
    {
        [JsonIgnore]
        public int IdProducto {  get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Las tallas son obligatorias.")]
        public string Tallas { get; set; }

        [Required(ErrorMessage = "Los colores son obligatorios.")]
        public string Color {  get; set; }

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que cero.")]
        public double Precio {  get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [MinLength(10, ErrorMessage = "La descripción debe tener al menos 10 caracteres.")]
        public string Descripcion {  get; set; }

        public string? Imagen {  get; set; } = "https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Fwww.motoblouz.es%2Fenjoytheride%2Fwp-content%2Fuploads%2F2017%2F01%2Fchaqueta-moto-dainese.jpg&f=1&nofb=1&ipt=9f4671292e1ea31bfafc2218f3a15f71df2b5e2b652e899867f971b4aac3281f&ipo=images";

        [Required(ErrorMessage = "El stock es obligatorio.")]
        [Range(0, double.MaxValue, ErrorMessage = "El stock debe ser mayor o igual que cero.")]
        public int Stock {  get; set; }

        [Required(ErrorMessage = "El tipo de producto es obligatoria.")]
        public int IdTipoProducto {  get; set; }
        [JsonIgnore]
        public TipoProductoModel? TipoProducto { get; set; }
    }
}
