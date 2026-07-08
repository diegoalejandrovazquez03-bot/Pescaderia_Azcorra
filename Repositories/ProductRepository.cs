using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PescaderiaApi.Interfaces;
using PescaderiaApi.Models;

namespace PescaderiaApi.Repositories
{
    /// <summary>
    /// Implementación en memoria del repositorio de productos.
    /// Almacena los productos y reseñas en listas estáticas en el Heap.
    /// Se aplican bloqueos (lock) para garantizar la seguridad de hilos (thread safety).
    /// </summary>
    public class ProductRepository : IProductRepository
    {
        private static readonly List<Product> _products = new();
        private static readonly List<Review> _reviews = new();
        private static readonly object _lock = new();

        static ProductRepository()
        {
            // Inicializar productos (Seeding data)
            _products.AddRange(new List<Product>
            {
                new() {
                    Id = "1",
                    Name = "Pescado Empanizado",
                    Description = "Incluye pescado empanizado acompañado de arroz, frijol, tortillas y guarnición.",
                    Price = 120,
                    Category = "Platillos",
                    Image = "https://images.unsplash.com/photo-1544551763-46a013bb70d5?w=600&h=400&fit=crop",
                    Available = true,
                    AvailableForPickup = true,
                    AvailableForDelivery = true,
                    Rating = 5.0,
                    Reviews = 0,
                    Weight = "Orden"
                },
                new() {
                    Id = "2",
                    Name = "Pescado Empanizado",
                    Description = "Incluye arroz, frijol, tortillas y guarnición. La única diferencia es una porción menor de pescado.",
                    Price = 90,
                    Category = "Platillos",
                    Image = "https://images.unsplash.com/photo-1544551763-46a013bb70d5?w=600&h=400&fit=crop",
                    Available = true,
                    AvailableForPickup = true,
                    AvailableForDelivery = true,
                    Rating = 5.0,
                    Reviews = 0,
                    Weight = "Media Orden"
                },
                new() {
                    Id = "3",
                    Name = "Camarón Empanizado",
                    Description = "Incluye camarones empanizados, arroz, frijol, tortillas y guarnición.",
                    Price = 170,
                    Category = "Platillos",
                    Image = "https://images.unsplash.com/photo-1565680018434-b513d5e5fd47?w=600&h=400&fit=crop",
                    Available = true,
                    AvailableForPickup = true,
                    AvailableForDelivery = true,
                    Rating = 5.0,
                    Reviews = 0,
                    Weight = "Orden"
                },
                new() {
                    Id = "4",
                    Name = "Camarón Empanizado",
                    Description = "Incluye arroz, frijol, tortillas y guarnición. La única diferencia es una porción menor de camarones.",
                    Price = 120,
                    Category = "Platillos",
                    Image = "https://images.unsplash.com/photo-1565680018434-b513d5e5fd47?w=600&h=400&fit=crop",
                    Available = true,
                    AvailableForPickup = true,
                    AvailableForDelivery = true,
                    Rating = 5.0,
                    Reviews = 0,
                    Weight = "Media Orden"
                },
                new() {
                    Id = "5",
                    Name = "Cóctel de Camarón (Marisco o Mixto)",
                    Description = "Cóctel preparado de camarón con opción marisco o mixto.",
                    Price = 160,
                    Category = "Platillos",
                    Image = "https://images.unsplash.com/photo-1534604973900-c43ab4c2e0ab?w=600&h=400&fit=crop",
                    Available = true,
                    AvailableForPickup = true,
                    AvailableForDelivery = true,
                    Rating = 5.0,
                    Reviews = 0,
                    Weight = "Copa"
                },
                new() {
                    Id = "6",
                    Name = "Cazón Frito",
                    Description = "Disponible únicamente miércoles y viernes.",
                    Price = 110,
                    Category = "Platillos",
                    Image = "https://images.unsplash.com/photo-1580476262798-bddd9f4b7369?w=600&h=400&fit=crop",
                    Available = true,
                    AvailableForPickup = true,
                    AvailableForDelivery = true,
                    Rating = 5.0,
                    Reviews = 0,
                    Weight = "Orden"
                },
                new() {
                    Id = "7",
                    Name = "Cazón Frito",
                    Description = "Disponible únicamente miércoles y viernes.",
                    Price = 75,
                    Category = "Platillos",
                    Image = "https://images.unsplash.com/photo-1580476262798-bddd9f4b7369?w=600&h=400&fit=crop",
                    Available = true,
                    AvailableForPickup = true,
                    AvailableForDelivery = true,
                    Rating = 5.0,
                    Reviews = 0,
                    Weight = "Media Orden"
                },
                new() {
                    Id = "8",
                    Name = "Cazón Entomatado",
                    Description = "Disponible únicamente miércoles y viernes.",
                    Price = 110,
                    Category = "Platillos",
                    Image = "https://images.unsplash.com/photo-1580476262798-bddd9f4b7369?w=600&h=400&fit=crop",
                    Available = true,
                    AvailableForPickup = true,
                    AvailableForDelivery = true,
                    Rating = 5.0,
                    Reviews = 0,
                    Weight = "Orden"
                },
                new() {
                    Id = "9",
                    Name = "Cazón Entomatado",
                    Description = "Disponible únicamente miércoles y viernes.",
                    Price = 75,
                    Category = "Platillos",
                    Image = "https://images.unsplash.com/photo-1580476262798-bddd9f4b7369?w=600&h=400&fit=crop",
                    Available = true,
                    AvailableForPickup = true,
                    AvailableForDelivery = true,
                    Rating = 5.0,
                    Reviews = 0,
                    Weight = "Media Orden"
                },
                new() {
                    Id = "10",
                    Name = "Camarón Fresco Crudo sin Cáscara y Cabeza",
                    Description = "Medida 21/25.",
                    Price = 260,
                    Category = "Mariscos Frescos",
                    Image = "https://images.unsplash.com/photo-1565680018434-b513d5e5fd47?w=600&h=400&fit=crop",
                    Available = true,
                    AvailableForPickup = true,
                    AvailableForDelivery = true,
                    Rating = 5.0,
                    Reviews = 0,
                    Weight = "Kilogramo"
                },
                new() {
                    Id = "11",
                    Name = "Camarón Cocido",
                    Description = "Con cabeza. Medida 41/50.",
                    Price = 150,
                    Category = "Mariscos Frescos",
                    Image = "https://images.unsplash.com/photo-1565680018434-b513d5e5fd47?w=600&h=400&fit=crop",
                    Available = true,
                    AvailableForPickup = true,
                    AvailableForDelivery = true,
                    Rating = 5.0,
                    Reviews = 0,
                    Weight = "Kilogramo"
                },
                new() {
                    Id = "12",
                    Name = "Cazón Fresco o Rebanado",
                    Description = "Producto fresco listo para preparar.",
                    Price = 120,
                    Category = "Mariscos Frescos",
                    Image = "https://images.unsplash.com/photo-1580476262798-bddd9f4b7369?w=600&h=400&fit=crop",
                    Available = true,
                    AvailableForPickup = true,
                    AvailableForDelivery = true,
                    Rating = 5.0,
                    Reviews = 0,
                    Weight = "Kilogramo"
                },
                new() {
                    Id = "13",
                    Name = "Tostada Bolsa Grande",
                    Description = "Contenido de 400 gramos.",
                    Price = 50,
                    Category = "Tostadas",
                    Image = "https://images.unsplash.com/photo-1546069901-ba9599a7e63c?w=600&h=400&fit=crop",
                    Available = true,
                    AvailableForPickup = true,
                    AvailableForDelivery = true,
                    Rating = 5.0,
                    Reviews = 0,
                    Weight = "Bolsa"
                },
                new() {
                    Id = "14",
                    Name = "Tostada Bolsa Mediana",
                    Description = "Contenido de 150 gramos.",
                    Price = 18,
                    Category = "Tostadas",
                    Image = "https://images.unsplash.com/photo-1546069901-ba9599a7e63c?w=600&h=400&fit=crop",
                    Available = true,
                    AvailableForPickup = true,
                    AvailableForDelivery = true,
                    Rating = 5.0,
                    Reviews = 0,
                    Weight = "Bolsa"
                },
                new() {
                    Id = "15",
                    Name = "Postas de Camarón Fritas",
                    Description = "Venta por kilogramo ya frito.",
                    Price = 360,
                    Category = "Frituras",
                    Image = "https://images.unsplash.com/photo-1625944525533-473f1a3d54e7?w=600&h=400&fit=crop",
                    Available = true,
                    AvailableForPickup = true,
                    AvailableForDelivery = true,
                    Rating = 5.0,
                    Reviews = 0,
                    Weight = "Kilogramo"
                },
                new() {
                    Id = "16",
                    Name = "Pescado Frito Corvinal",
                    Description = "Venta por kilogramo.",
                    Price = 330,
                    Category = "Frituras",
                    Image = "https://images.unsplash.com/photo-1519708227418-c8fd9a32b7a2?w=600&h=400&fit=crop",
                    Available = true,
                    AvailableForPickup = true,
                    AvailableForDelivery = true,
                    Rating = 5.0,
                    Reviews = 0,
                    Weight = "Kilogramo"
                },
                new() {
                    Id = "17",
                    Name = "Pecho Frito de Mero",
                    Description = "Venta por kilogramo.",
                    Price = 330,
                    Category = "Frituras",
                    Image = "https://images.unsplash.com/photo-1519708227418-c8fd9a32b7a2?w=600&h=400&fit=crop",
                    Available = true,
                    AvailableForPickup = true,
                    AvailableForDelivery = true,
                    Rating = 5.0,
                    Reviews = 0,
                    Weight = "Kilogramo"
                }
            });

            // Inicializar reseñas vacías (Seeding reviews vacías)
            _reviews.AddRange(new List<Review>());
        }

        public Task<IEnumerable<Product>> GetAllAsync()
        {
            lock (_lock)
            {
                return Task.FromResult<IEnumerable<Product>>(_products.ToList());
            }
        }

        public Task<Product?> GetByIdAsync(string id)
        {
            lock (_lock)
            {
                var product = _products.FirstOrDefault(p => p.Id == id);
                return Task.FromResult(product);
            }
        }

        public Task AddAsync(Product product)
        {
            lock (_lock)
            {
                if (string.IsNullOrEmpty(product.Id))
                {
                    product.Id = (_products.Count > 0 ? (_products.Max(p => int.Parse(p.Id)) + 1).ToString() : "1");
                }
                _products.Add(product);
                return Task.CompletedTask;
            }
        }

        public Task UpdateAsync(Product product)
        {
            lock (_lock)
            {
                var existing = _products.FirstOrDefault(p => p.Id == product.Id);
                if (existing != null)
                {
                    existing.Name = product.Name;
                    existing.Description = product.Description;
                    existing.Price = product.Price;
                    existing.Category = product.Category;
                    existing.Image = product.Image;
                    existing.Images = product.Images;
                    existing.Available = product.Available;
                    existing.AvailableForPickup = product.AvailableForPickup;
                    existing.AvailableForDelivery = product.AvailableForDelivery;
                    existing.Rating = product.Rating;
                    existing.Reviews = product.Reviews;
                    existing.Origin = product.Origin;
                    existing.Nutrition = product.Nutrition;
                    existing.Weight = product.Weight;
                    existing.PreparationTime = product.PreparationTime;
                    existing.Ingredients = product.Ingredients;
                }
                return Task.CompletedTask;
            }
        }

        public Task DeleteAsync(string id)
        {
            lock (_lock)
            {
                var product = _products.FirstOrDefault(p => p.Id == id);
                if (product != null)
                {
                    _products.Remove(product);
                }
                return Task.CompletedTask;
            }
        }

        public Task<IEnumerable<Review>> GetReviewsByProductIdAsync(string productId)
        {
            lock (_lock)
            {
                return Task.FromResult<IEnumerable<Review>>(_reviews.Where(r => r.ProductId == productId).ToList());
            }
        }

        public Task AddReviewAsync(Review review)
        {
            lock (_lock)
            {
                if (!string.IsNullOrEmpty(review.UserId) && _reviews.Any(r => r.ProductId == review.ProductId && r.UserId == review.UserId))
                {
                    throw new InvalidOperationException("Ya has dejado una reseña para este producto.");
                }

                review.Id = Guid.NewGuid().ToString();
                review.Date = DateTime.Now.ToString("yyyy-MM-dd");
                _reviews.Add(review);

                var product = _products.FirstOrDefault(p => p.Id == review.ProductId);
                if (product != null)
                {
                    var productReviews = _reviews.Where(r => r.ProductId == review.ProductId).ToList();
                    product.Reviews = productReviews.Count;
                    product.Rating = Math.Round(productReviews.Average(r => r.Rating), 1);
                }
                return Task.CompletedTask;
            }
        }
    }
}
