using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PescaderiaApi.Interfaces;
using PescaderiaApi.Models;

namespace PescaderiaApi.Repositories
{
    /// <summary>
    /// Repositorio de pedidos en memoria.
    /// Almacena los pedidos en listas en el Heap.
    /// </summary>
    public class OrderRepository : IOrderRepository
    {
        private static readonly List<Order> _orders = new();
        private static readonly object _lock = new();

        static OrderRepository()
        {
            // Seed de órdenes
            _orders.Add(new Order
            {
                Id = "ORD-2026-001",
                UserId = "1",
                Date = "2026-06-10T10:30:00",
                Status = "delivered",
                Items = new List<OrderItem>
                {
                    new() { ProductId = "1", ProductName = "Camarones Frescos", Quantity = 2, Price = 280 },
                    new() { ProductId = "7", ProductName = "Salmón Noruego", Quantity = 1, Price = 320 }
                },
                Subtotal = 880,
                Tax = 140.80m,
                Shipping = 0,
                Total = 1020.80m,
                PaymentMethod = "credit",
                ShippingAddress = "Calle Principal 123, Mérida, Yucatán"
            });

            _orders.Add(new Order
            {
                Id = "ORD-2026-002",
                UserId = "1",
                Date = "2026-06-12T15:45:00",
                Status = "shipping",
                Items = new List<OrderItem>
                {
                    new() { ProductId = "13", ProductName = "Ceviche de Camarón", Quantity = 3, Price = 180 }
                },
                Subtotal = 540,
                Tax = 86.40m,
                Shipping = 0,
                Total = 626.40m,
                PaymentMethod = "transfer",
                ShippingAddress = "Calle Principal 123, Mérida, Yucatán"
            });
        }

        public Task<IEnumerable<Order>> GetAllAsync()
        {
            lock (_lock)
            {
                return Task.FromResult<IEnumerable<Order>>(_orders.ToList());
            }
        }

        public Task<IEnumerable<Order>> GetByUserIdAsync(string userId)
        {
            lock (_lock)
            {
                var userOrders = _orders.Where(o => o.UserId == userId).ToList();
                return Task.FromResult<IEnumerable<Order>>(userOrders);
            }
        }

        public Task<Order?> GetByIdAsync(string id)
        {
            lock (_lock)
            {
                var order = _orders.FirstOrDefault(o => o.Id == id);
                return Task.FromResult(order);
            }
        }

        public Task AddAsync(Order order)
        {
            lock (_lock)
            {
                if (string.IsNullOrEmpty(order.Id))
                {
                    int orderNumber = (_orders.Count % 299) + 1;
                    if (string.IsNullOrEmpty(order.UserId))
                    {
                        order.Id = $"INV-2026-{orderNumber:D3}";
                        if (string.IsNullOrEmpty(order.CustomerName))
                        {
                            order.CustomerName = "Invitado";
                        }
                    }
                    else
                    {
                        order.Id = $"ORD-2026-{orderNumber:D3}";
                    }
                }
                order.Date = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                _orders.Add(order);
                return Task.CompletedTask;
            }
        }

        public Task UpdateStatusAsync(string id, string status)
        {
            lock (_lock)
            {
                var order = _orders.FirstOrDefault(o => o.Id == id);
                if (order != null)
                {
                    order.Status = status;
                }
                return Task.CompletedTask;
            }
        }
    }
}
