using ApiTiendaOnline.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiTiendaOnline.Models;

namespace ApiTiendaOnline.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TipoProductoController : ControllerBase
    {
        public readonly TiendaOnlineContext _context;

        public TipoProductoController(TiendaOnlineContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("Lista")]
        public IActionResult Lista()
        {
            List<TipoProducto> tipoProductos = new List<TipoProducto>();
            try
            {
                tipoProductos = _context.TipoProductos.ToList();

                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok", response = tipoProductos });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status200OK, new { mensaje = ex.Message, response = tipoProductos });

            }
        }
    }
}
