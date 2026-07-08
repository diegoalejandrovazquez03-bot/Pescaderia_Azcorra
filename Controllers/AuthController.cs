using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PescaderiaApi.Interfaces;
using PescaderiaApi.Models;

namespace PescaderiaApi.Controllers
{
    /// <summary>
    /// Controlador para autenticación y gestión de usuarios.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public AuthController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { message = "Email y contraseña requeridos." });
            }

            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null || user.PasswordHash != request.Password) // En producción usaríamos hash seguro
            {
                return Unauthorized(new { message = "Email o contraseña incorrectos." });
            }

            // Crear respuesta simulada con un token de acceso
            var response = new AuthResponse
            {
                Token = $"mock-jwt-token-for-{user.Id}-{user.Role}",
                User = new User
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                    Role = user.Role,
                    Phone = user.Phone,
                    Address = user.Address
                }
            };

            return Ok(response);
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password) || string.IsNullOrEmpty(request.Name))
            {
                return BadRequest(new { message = "Los campos email, contraseña y nombre son requeridos." });
            }

            var existing = await _userRepository.GetByEmailAsync(request.Email);
            if (existing != null)
            {
                return Conflict(new { message = "El correo ya está registrado." });
            }

            var newUser = new User
            {
                Email = request.Email,
                PasswordHash = request.Password, // Simulación directa
                Name = request.Name,
                Phone = request.Phone,
                Address = request.Address,
                Role = "customer" // Por defecto son clientes
            };

            await _userRepository.AddAsync(newUser);

            var response = new AuthResponse
            {
                Token = $"mock-jwt-token-for-{newUser.Id}-{newUser.Role}",
                User = new User
                {
                    Id = newUser.Id,
                    Email = newUser.Email,
                    Name = newUser.Name,
                    Role = newUser.Role,
                    Phone = newUser.Phone,
                    Address = newUser.Address
                }
            };

            return Ok(response);
        }

        // GET: api/auth/profile/{id}
        [HttpGet("profile/{id}")]
        public async Task<ActionResult<User>> GetProfile(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "Usuario no encontrado." });
            }

            // Retornamos sin la contraseña para mantener la seguridad (seguridad OOP y encapsulamiento)
            return Ok(new User
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                Role = user.Role,
                Phone = user.Phone,
                Address = user.Address
            });
        }
    }
}
