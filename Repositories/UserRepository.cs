using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PescaderiaApi.Interfaces;
using PescaderiaApi.Models;

namespace PescaderiaApi.Repositories
{
    /// <summary>
    /// Repositorio de usuarios en memoria.
    /// Almacena usuarios en listas estáticas del Heap.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private static readonly List<User> _users = new();
        private static readonly object _lock = new();

        static UserRepository()
        {
            // Seed de usuarios
            _users.Add(new User
            {
                Id = "1",
                Email = "maria@example.com",
                PasswordHash = "password123",
                Name = "María González",
                Role = "customer",
                Phone = "555-1234",
                Address = "Calle Principal 123, Ciudad"
            });

            _users.Add(new User
            {
                Id = "2",
                Email = "juan@example.com",
                PasswordHash = "password123",
                Name = "Juan Pérez",
                Role = "customer",
                Phone = "555-5678",
                Address = "Calle Principal 123, Ciudad"
            });

            _users.Add(new User
            {
                Id = "3",
                Email = "Pescadria.fernando@gmail.com",
                PasswordHash = "202609",
                Name = "Fernando Azcorra",
                Role = "admin",
                Phone = "(999) 505-4210",
                Address = "Calle Pescadores 456, Progreso, Yucatán"
            });
        }

        public Task<User?> GetByIdAsync(string id)
        {
            lock (_lock)
            {
                var user = _users.FirstOrDefault(u => u.Id == id);
                return Task.FromResult(user);
            }
        }

        public Task<User?> GetByEmailAsync(string email)
        {
            lock (_lock)
            {
                var user = _users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
                return Task.FromResult(user);
            }
        }

        public Task AddAsync(User user)
        {
            lock (_lock)
            {
                if (string.IsNullOrEmpty(user.Id))
                {
                    user.Id = Guid.NewGuid().ToString();
                }
                _users.Add(user);
                return Task.CompletedTask;
            }
        }

        public Task UpdateAsync(User user)
        {
            lock (_lock)
            {
                var existing = _users.FirstOrDefault(u => u.Id == user.Id);
                if (existing != null)
                {
                    existing.Name = user.Name;
                    existing.Phone = user.Phone;
                    existing.Address = user.Address;
                    existing.Email = user.Email;
                    if (!string.IsNullOrEmpty(user.PasswordHash))
                    {
                        existing.PasswordHash = user.PasswordHash;
                    }
                }
                return Task.CompletedTask;
            }
        }
    }
}
