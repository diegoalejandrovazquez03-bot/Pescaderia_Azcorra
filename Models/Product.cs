using System;
using System.Collections.Generic;

namespace PescaderiaApi.Models
{
    /// <summary>
    /// Representa un producto en la Pescadería.
    /// Al ser una Clase (class), es un Tipo de Referencia (Reference Type).
    /// En .NET, los tipos de referencia se almacenan en el Heap (Montículo),
    /// y las variables locales o parámetros que los apuntan contienen solo la dirección de memoria (en el Stack).
    /// </summary>
    public class Product
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty; // 'seafood' | 'fish' | 'prepared' | 'drinks'
        public string Image { get; set; } = string.Empty;
        public List<string> Images { get; set; } = new();
        public bool Available { get; set; }
        public bool AvailableForPickup { get; set; }
        public bool AvailableForDelivery { get; set; }
        public double Rating { get; set; }
        public int Reviews { get; set; }
        public string Origin { get; set; } = string.Empty;
        public NutritionInfo? Nutrition { get; set; }
        public string Weight { get; set; } = string.Empty;
        public string PreparationTime { get; set; } = string.Empty;
        public List<string> Ingredients { get; set; } = new();
    }

    /// <summary>
    /// Información nutricional.
    /// Nota: Podría ser un struct (Value Type) si queremos que se almacene en el stack,
    /// pero al ser parte de una clase Product que está en el Heap, se alojará en el Heap junto con ella.
    /// </summary>
    public class NutritionInfo
    {
        public int Calories { get; set; }
        public double Protein { get; set; }
        public double Fat { get; set; }
    }
}
