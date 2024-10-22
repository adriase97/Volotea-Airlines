using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiTiendaOnline.Models;
using Microsoft.AspNetCore.Cors;

namespace ApiTiendaOnline.Controllers
{
    [EnableCors("ReglasCors")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        public readonly TiendaOnlineContext _context;

        public ProductoController(TiendaOnlineContext context)
        {
        _context = context; 
        }

        [HttpGet]
        [Route("Lista")]
        public IActionResult Lista([FromQuery] int? idTipoProducto)
        {
            List<Producto> products = new List<Producto>();
            try
            {
                // Consulta que incluye la relación con TipoProducto
                var query = _context.Productos.Include(tp => tp.TipoProducto).AsQueryable();

                // Filtra si se proporciona el idTipoProducto
                if (idTipoProducto.HasValue)
                {
                    query = query.Where(p => p.IdTipoProducto == idTipoProducto.Value);
                }

                // Ejecuta la consulta
                products = query.ToList();

                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok", response = products });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = ex.Message, response = products });
            }
        }



        [HttpGet]
        [Route("Obtener/{IdProducto:int}")]
        public IActionResult Obtener(int IdProducto)
        {
            Producto product = _context.Productos.Find(IdProducto);

            if (product == null) { 
                return BadRequest("Producto no encontrado");
            }


            try
            {
                product = _context.Productos.Include(tp => tp.TipoProducto).Where(p => p.IdProducto == IdProducto).FirstOrDefault();

                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok", response = product });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status200OK, new { mensaje = ex.Message, response = product });

            }
        }

        [HttpPost]
        [Route("Insertar")]
        public IActionResult Insertar([FromBody] Producto product)
        {
            try
            {
                _context.Productos.Add(product);
                _context.SaveChanges();

                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok" });

            }
            catch (Exception ex) {
                return StatusCode(StatusCodes.Status200OK, new { mensaje = ex.Message });
            }
        }

        [HttpPut]
        [Route("Modificar")]
        public IActionResult Modificar([FromBody] Producto p)
        {
            Producto product = _context.Productos.Find(p.IdProducto);

            if (product == null)
            {
                return BadRequest("Producto no encontrado");
            }


            try
            {
                product.Nombre = p.Nombre is null ? product.Nombre : p.Nombre;
                product.Tallas = p.Tallas is null ? product.Tallas : p.Tallas;
                product.Color = p.Color is null ? product.Color : p.Color;
                product.Precio = p.Precio is null ? product.Precio : p.Precio;
                product.Descripcion = p.Descripcion is null ? product.Descripcion : p.Descripcion;
                product.Imagen = p.Imagen is null ? product.Imagen : p.Imagen;
                product.Stock = p.Stock is null ? product.Stock : p.Stock;
                product.IdTipoProducto = p.IdTipoProducto is null ? product.IdTipoProducto : p.IdTipoProducto;

                _context.Productos.Update(product);
                _context.SaveChanges();

                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok" });

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status200OK, new { mensaje = ex.Message });
            }
        }

        [HttpDelete]
        [Route("Eliminar/{IdProducto:int}")]
        public IActionResult Eliminar(int IdProducto)
        {
            Producto product = _context.Productos.Find(IdProducto);

            if (product == null)
            {
                return BadRequest("Producto no encontrado");
            }


            try
            {

                _context.Productos.Remove(product);
                _context.SaveChanges();

                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok" });

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status200OK, new { mensaje = ex.Message });
            }
        }

    }
}
