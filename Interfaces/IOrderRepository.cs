using System.Collections.Generic;
using System.Threading.Tasks;
using PescaderiaApi.Models;

namespace PescaderiaApi.Interfaces
{
    /// <summary>
    /// Interfaz para operaciones relacionadas con pedidos.
    /// Contrato OOP.
    /// </summary>
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllAsync();
        Task<IEnumerable<Order>> GetByUserIdAsync(string userId);
        Task<Order?> GetByIdAsync(string id);
        Task AddAsync(Order order);
        Task UpdateStatusAsync(string id, string status);
    }
}
