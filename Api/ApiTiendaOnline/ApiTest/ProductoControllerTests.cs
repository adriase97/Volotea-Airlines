using ApiTiendaOnline.Controllers;
using ApiTiendaOnline.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTest
{
    [TestClass]
    public class ProductoControllerTests
    {
        private TiendaOnlineContext _context;
        private ProductoController _controller;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<TiendaOnlineContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new TiendaOnlineContext(options);
            _controller = new ProductoController(_context);
        }

        [TestMethod]
        public void Lista_ReturnsOkResult_WithProducts()
        {
            var products = new List<Producto>
            {
                new Producto { IdProducto = 1, Nombre = "Producto1", Precio = 10, Stock = 5 },
                new Producto { IdProducto = 2, Nombre = "Producto2", Precio = 20, Stock = 10 },
                new Producto { IdProducto = 3, Nombre = "Producto3", Precio = 30, Stock = 15 }
            };

            _context.Productos.AddRange(products);
            _context.SaveChanges();

            var result = _controller.Lista(null) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            var response = result.Value as dynamic;
            Assert.AreEqual("ok", response.mensaje);
            Assert.AreEqual(3, ((List<Producto>)response.response).Count);
        }

        [TestMethod]
        public void Obtener_ReturnsOkResult_WithExistingProduct()
        {
            var product = new Producto { IdProducto = 1, Nombre = "Producto1", Precio = 10, Stock = 5 };
            _context.Productos.Add(product);
            _context.SaveChanges();

            var result = _controller.Obtener(1) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            var response = result.Value as dynamic;
            Assert.AreEqual("ok", response.mensaje);
            Assert.AreEqual(1, ((Producto)response.response).IdProducto);
        }

        [TestMethod]
        public void Obtener_ReturnsBadRequest_WhenProductDoesNotExist()
        {
            var result = _controller.Obtener(999) as BadRequestObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Producto no encontrado", result.Value);
        }

        [TestMethod]
        public void Insertar_ReturnsOkResult_WhenProductIsCreated()
        {
            var newProduct = new Producto { Nombre = "Nuevo Producto", Precio = 10, Stock = 5 };

            var result = _controller.Insertar(newProduct) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual("ok", ((dynamic)result.Value).mensaje);

            var createdProduct = _context.Productos.FirstOrDefault(p => p.Nombre == "Nuevo Producto");
            Assert.IsNotNull(createdProduct);
        }

        [TestMethod]
        public void Modificar_ReturnsOkResult_WhenProductIsUpdated()
        {
            var existingProduct = new Producto { IdProducto = 1, Nombre = "Producto Original", Precio = 10, Stock = 5 };
            _context.Productos.Add(existingProduct);
            _context.SaveChanges();

            var updatedProduct = new Producto { IdProducto = 1, Nombre = "Producto Modificado", Precio = 15, Stock = 10 };

            var result = _controller.Modificar(updatedProduct) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual("ok", ((dynamic)result.Value).mensaje);

            var modifiedProduct = _context.Productos.Find(1);
            Assert.AreEqual("Producto Modificado", modifiedProduct.Nombre);
        }

        [TestMethod]
        public void Modificar_ReturnsBadRequest_WhenProductDoesNotExist()
        {
            var updatedProduct = new Producto { IdProducto = 999, Nombre = "Producto Inexistente" };

            var result = _controller.Modificar(updatedProduct) as BadRequestObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Producto no encontrado", result.Value);
        }

        [TestMethod]
        public void Eliminar_ReturnsOkResult_WhenProductIsDeleted()
        {
            var productToDelete = new Producto { IdProducto = 1, Nombre = "Producto a Eliminar", Precio = 10, Stock = 5 };
            _context.Productos.Add(productToDelete);
            _context.SaveChanges();

            var result = _controller.Eliminar(1) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual("ok", ((dynamic)result.Value).mensaje);

            var deletedProduct = _context.Productos.Find(1);
            Assert.IsNull(deletedProduct);
        }

        [TestMethod]
        public void Eliminar_ReturnsBadRequest_WhenProductDoesNotExist()
        {
            var result = _controller.Eliminar(999) as BadRequestObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Producto no encontrado", result.Value);
        }
    }
}
