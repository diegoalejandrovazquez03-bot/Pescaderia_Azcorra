using System.Collections.Generic;
using System.Threading.Tasks;
using PescaderiaApi.Models;

namespace PescaderiaApi.Interfaces
{
    /// <summary>
    /// Interfaz para operaciones relacionadas con productos.
    /// Define un contrato OOP para desacoplar el origen de los datos de su consumo.
    /// </summary>
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(string id);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(string id);
        Task<IEnumerable<Review>> GetReviewsByProductIdAsync(string productId);
        Task AddReviewAsync(Review review);
    }
}
