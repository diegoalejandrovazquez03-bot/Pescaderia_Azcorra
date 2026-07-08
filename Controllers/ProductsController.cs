using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PescaderiaApi.Interfaces;
using PescaderiaApi.Models;

namespace PescaderiaApi.Controllers
{
    /// <summary>
    /// Controlador para la gestión de productos y sus reseñas.
    /// Inyecta IProductRepository usando inyección de dependencias (.NET IoC container).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        // Constructor que inyecta la interfaz (OOP y desacoplamiento)
        public ProductsController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        // GET: api/products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAll()
        {
            var products = await _productRepository.GetAllAsync();
            return Ok(products);
        }

        // GET: api/products/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetById(string id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound(new { message = $"Producto con ID {id} no encontrado." });
            }
            return Ok(product);
        }

        // POST: api/products
        [HttpPost]
        public async Task<ActionResult<Product>> Create([FromBody] Product product)
        {
            if (product == null) return BadRequest();

            await _productRepository.AddAsync(product);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        // PUT: api/products/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] Product product)
        {
            if (product == null || product.Id != id)
            {
                return BadRequest(new { message = "Datos inválidos o IDs no coinciden." });
            }

            var existing = await _productRepository.GetByIdAsync(id);
            if (existing == null)
            {
                return NotFound(new { message = "Producto no encontrado." });
            }

            await _productRepository.UpdateAsync(product);
            return NoContent();
        }

        // DELETE: api/products/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existing = await _productRepository.GetByIdAsync(id);
            if (existing == null)
            {
                return NotFound(new { message = "Producto no encontrado." });
            }

            await _productRepository.DeleteAsync(id);
            return NoContent();
        }

        // GET: api/products/{id}/reviews
        [HttpGet("{id}/reviews")]
        public async Task<ActionResult<IEnumerable<Review>>> GetReviews(string id)
        {
            var reviews = await _productRepository.GetReviewsByProductIdAsync(id);
            return Ok(reviews);
        }

        // POST: api/products/{id}/reviews
        [HttpPost("{id}/reviews")]
        public async Task<ActionResult<Review>> AddReview(string id, [FromBody] Review review)
        {
            if (review == null) return BadRequest();

            review.ProductId = id;
            try
            {
                await _productRepository.AddReviewAsync(review);
                return Ok(review);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
