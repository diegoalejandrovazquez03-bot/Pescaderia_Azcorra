using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PescaderiaApi.Interfaces;
using PescaderiaApi.Models;

namespace PescaderiaApi.Controllers
{
    /// <summary>
    /// Controlador para la gestión de pedidos (órdenes).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;

        public OrdersController(IOrderRepository orderRepository, IUserRepository userRepository)
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
        }

        // GET: api/orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetAll()
        {
            var orders = await _orderRepository.GetAllAsync();
            foreach(var order in orders)
            {
                if (!string.IsNullOrEmpty(order.UserId))
                {
                    var user = await _userRepository.GetByIdAsync(order.UserId);
                    if (user != null)
                    {
                        order.CustomerName = user.Name;
                    }
                }
            }
            return Ok(orders);
        }

        // GET: api/orders/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetByUserId(string userId)
        {
            var orders = await _orderRepository.GetByUserIdAsync(userId);
            return Ok(orders);
        }

        // GET: api/orders/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetById(string id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                return NotFound(new { message = $"Pedido con ID {id} no encontrado." });
            }
            return Ok(order);
        }

        // POST: api/orders
        [HttpPost]
        public async Task<ActionResult<Order>> Create([FromBody] Order order)
        {
            if (order == null) return BadRequest();

            if (!string.IsNullOrEmpty(order.UserId))
            {
                var user = await _userRepository.GetByIdAsync(order.UserId);
                if (user != null)
                {
                    order.CustomerName = user.Name;
                }
            }

            await _orderRepository.AddAsync(order);
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }

        // PUT: api/orders/{id}/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(string id, [FromBody] StatusUpdateRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Status))
            {
                return BadRequest(new { message = "Estado inválido." });
            }

            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                return NotFound(new { message = "Pedido no encontrado." });
            }

            await _orderRepository.UpdateStatusAsync(id, request.Status);
            return NoContent();
        }
    }

    public class StatusUpdateRequest
    {
        public string Status { get; set; } = string.Empty;
    }
}
