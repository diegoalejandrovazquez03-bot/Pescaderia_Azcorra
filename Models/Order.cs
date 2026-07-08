using System;
using System.Collections.Generic;

namespace PescaderiaApi.Models
{
    /// <summary>
    /// Representa un pedido realizado en la Pescadería.
    /// Almacenado en el Heap.
    /// </summary>
    public class Order
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string Status { get; set; } = "received"; // received, preparing, packed, shipping, delivered
        public List<OrderItem> Items { get; set; } = new();
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Shipping { get; set; }
        public decimal Total { get; set; }
        public string PaymentMethod { get; set; } = "credit"; // transfer, credit, debit
        public string ShippingAddress { get; set; } = string.Empty;
    }

    /// <summary>
    /// Representa un elemento dentro del pedido.
    /// Usamos una clase aquí para que la estructura sea mutable y extensible en el Heap.
    /// </summary>
    public class OrderItem
    {
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
