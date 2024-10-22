using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using TiendaOnline.Enums;
using TiendaOnline.Models;
using TiendaOnline.ViewModels;
using System.Text.Json;
using System;
using Microsoft.AspNetCore.Localization;
using System.Drawing;


namespace TiendaOnline.Controllers
{
    public class ProductoController : Controller
    {
        private readonly string apiUrl = "https://localhost:7194/api/";
        private readonly HttpClient _httpClient;

        public ProductoController()
        {
            _httpClient = new HttpClient();
        }

        #region Views

        // GET: ProductoController
        /// <summary>
        /// Accedes a la vista Index donde muestra todos los productos
        /// </summary>
        /// <param name="idTipoProducto">Sirve para filtrar los productos</param>
        /// <returns></returns>
        public async Task<ActionResult> Index(int? idTipoProducto)
        {
            ProductosViewModel productos = new ProductosViewModel()
            {
                Productos = await ObtenerProductos(idTipoProducto),
                TipoProductos = await ObtenerTipoProductos()
            };

            return View(productos);
        }

        /// <summary>
        /// Accedes a la vista para crear un productos
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> AddProducto()
        {
            ProductoViewModel model = new ProductoViewModel()
            {
                Producto = new ProductoModel(),
                TipoProductos = await ObtenerTipoProductos()
            };

            return View(model);
        }

        /// <summary>
        /// Accedes a la vista para modificar un producto
        /// </summary>
        /// <param name="IdProducto">Id del producto</param>
        /// <returns></returns>
        public async Task<ActionResult> UpdateProducto(int IdProducto)
        {
            ProductoViewModel model = new ProductoViewModel()
            {
                Producto = await ObtenerProducto(IdProducto),
                TipoProductos = await ObtenerTipoProductos()
            };

            return View(model);
        }

        #endregion

        #region Acciones
        /// <summary>
        /// Inserta un producto a la base de datos
        /// </summary>
        /// <param name="producto">Modelo del producto</param>
        /// <param name="Imagen">Imagen que se guardará en el servidor para el producto</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CreateProducto(ProductoModel producto, IFormFile Imagen)
        {
            if (ModelState.IsValid)
            {
                producto.Tallas = UnificarTallas();

                try
                {
                    var url = apiUrl + "Producto/Insertar";

                    var json = System.Text.Json.JsonSerializer.Serialize(producto);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await _httpClient.PostAsync(url, content);
                    response.EnsureSuccessStatusCode();

                    var mensaje = await response.Content.ReadAsStringAsync();
                    var apiMensaje = JsonConvert.DeserializeObject<ApiMensajeModel>(mensaje);

                    if (apiMensaje.Mensaje.Equals("ok"))
                    {
                        return RedirectToAction("Index");

                    }
                    else
                    {
                        ModelState.AddModelError("", apiMensaje.Mensaje);
                        ProductoViewModel model = new ProductoViewModel()
                        {
                            Producto = producto,
                            TipoProductos = await ObtenerTipoProductos()
                        };
                        return View("AddProducto", model);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error al crear el producto: {ex.Message}");
                    ProductoViewModel model = new ProductoViewModel()
                    {
                        Producto = producto,
                        TipoProductos = await ObtenerTipoProductos()
                    };
                    return View("AddProducto", model);
                }
            }
            else
            {
                ProductoViewModel model = new ProductoViewModel()
                {
                    Producto = producto,
                    TipoProductos = await ObtenerTipoProductos()
                };
                return View("AddProducto", model);
            }
        }
       
        /// <summary>
        /// Guarda a la base de datos la modificación de un producto
        /// </summary>
        /// <param name="producto">El modelo del producto</param>
        /// <returns></returns>
        public async Task<ActionResult> SaveProducto(ProductoModel producto)
        {
            if (ModelState.IsValid)
            {
                producto.Tallas = UnificarTallas();
                try
                {
                    var url = apiUrl + "Producto/Modificar";

                    var json = System.Text.Json.JsonSerializer.Serialize(producto);
                    var productoDict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(json);

                    productoDict["IdProducto"] = producto.IdProducto;
                    json = System.Text.Json.JsonSerializer.Serialize(productoDict);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await _httpClient.PutAsync(url, content);
                    response.EnsureSuccessStatusCode();

                    var mensaje = await response.Content.ReadAsStringAsync();
                    var apiMensaje = JsonConvert.DeserializeObject<ApiMensajeModel>(mensaje);

                    if (apiMensaje.Mensaje.Equals("ok"))
                    {
                        return RedirectToAction("Index");

                    }
                    else
                    {
                        ModelState.AddModelError("", apiMensaje.Mensaje);
                        ProductoViewModel model = new ProductoViewModel()
                        {
                            Producto = producto,
                            TipoProductos = await ObtenerTipoProductos()
                        };
                        return View("UpdateProducto", model);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error al crear el producto: {ex.Message}");
                    ProductoViewModel model = new ProductoViewModel()
                    {
                        Producto = producto,
                        TipoProductos = await ObtenerTipoProductos()
                    };
                    return View("UpdateProducto", model);
                }
            }
            else
            {
                ProductoViewModel model = new ProductoViewModel()
                {
                    Producto = producto,
                    TipoProductos = await ObtenerTipoProductos()
                };

                return View("UpdateProducto", model);
            }
        }

        /// <summary>
        /// Elimina un producto de la base de datos
        /// </summary>
        /// <param name="IdProducto">Id del producto</param>
        /// <returns></returns>
        public async Task<ActionResult> DeleteProducto(int IdProducto)
        {
            await EliminarProducto(IdProducto);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Unifica todas las tallas seleccionadas en un único string
        /// </summary>
        /// <returns>Devuelve todas las tallas en un solo string</returns>
        private string UnificarTallas()
        {
            var tallasSeleccionadas = Request.Form["Producto.Tallas"];

            if (!string.IsNullOrEmpty(tallasSeleccionadas)) return string.Join(",", tallasSeleccionadas);
            else return string.Empty;
        }

        #endregion

        #region BBDD

        /// <summary>
        /// Obten de la base de datos todos los productos
        /// </summary>
        /// <param name="idTipoProducto">Id del tipo de producto si se quiere filtrar</param>
        /// <returns></returns>
        public async Task<List<ProductoModel>> ObtenerProductos(int? idTipoProducto)
        {
            List<ProductoModel> productos = new List<ProductoModel>();
            try
            {
                string url = "Producto/Lista";
                if (idTipoProducto.HasValue)
                {
                    url += "?idTipoProducto=" + idTipoProducto.Value;
                }

                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl + url);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();

                    var apiResponse = JsonConvert.DeserializeObject<ApiResponseModel<List<ProductoModel>>>(responseContent);

                    if (apiResponse != null && apiResponse.Response != null)
                    {
                        var productosJson = apiResponse.Response;
                        productos = productosJson;
                    }
                }

                return productos;
            }
            catch (Exception ex)
            {
                return productos;
            }
        }

        /// <summary>
        /// Obten los datos de un producto
        /// </summary>
        /// <param name="idProducto">Id del producto</param>
        /// <returns></returns>
        public async Task<ProductoModel> ObtenerProducto(int idProducto)
        {
            ProductoModel producto = new ProductoModel();

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl + $"Producto/Obtener/{idProducto}");

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponseModel<ProductoModel>>(responseContent);

                    if (apiResponse != null && apiResponse.Response != null)
                    {
                        var productoJson = apiResponse.Response;
                        producto = productoJson;
                    }
                }

                return producto;
            }
            catch (Exception ex)
            {
                return producto;
            }
        }

        /// <summary>
        /// Obten una lista de todos los tipos de productos
        /// </summary>
        /// <returns></returns>
        public async Task<List<TipoProductoModel>> ObtenerTipoProductos()
        {
            List<TipoProductoModel> tipoProductos = new List<TipoProductoModel>();

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl + "TipoProducto/Lista");

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponseModel<List<TipoProductoModel>>>(responseContent);

                    if (apiResponse != null && apiResponse.Response != null)
                    {
                        var tipoProductosJson = apiResponse.Response;
                        tipoProductos = tipoProductosJson;
                    }
                }

                return tipoProductos;
            }
            catch (Exception ex)
            {
                return tipoProductos;
            }
        }

        /// <summary>
        /// Elimina de la base de datos un producto
        /// </summary>
        /// <param name="IdProducto"></param>
        /// <returns></returns>
        public async Task<bool> EliminarProducto(int IdProducto)
        {
            try
            {
                var url = $"{apiUrl}Producto/Eliminar/{IdProducto}";
                var response = await _httpClient.DeleteAsync(url);
                response.EnsureSuccessStatusCode();

                var mensaje = await response.Content.ReadAsStringAsync();

                return true;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al eliminar el producto: {ex.Message}");
                return false;
            }
        }

        #endregion

        // GET: ProductoController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ProductoController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProductoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ProductoController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ProductoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ProductoController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ProductoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
