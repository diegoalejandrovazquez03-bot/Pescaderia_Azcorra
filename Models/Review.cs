using System;

namespace PescaderiaApi.Models
{
    /// <summary>
    /// Representa una reseña realizada por un usuario.
    /// Tipo de referencia almacenado en el Heap.
    /// </summary>
    public class Review
    {
        public string Id { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string UserAvatar { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
    }
}
