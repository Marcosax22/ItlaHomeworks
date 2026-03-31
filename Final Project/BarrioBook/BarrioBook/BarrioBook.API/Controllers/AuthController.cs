using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BarrioBook.Application.Auth;
using BarrioBook.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarrioBook.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<CustomerDto>> Register([FromBody] RegisterCustomerDto dto)
        {
            var customer = await _authService.RegisterAsync(dto);
            return Created(string.Empty, customer);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResult), 200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<AuthResult>> Login([FromBody] LoginCustomerDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            return Ok(result);
        }

        [HttpGet("me")]
        [Authorize]
        public ActionResult<object> Me()
        {
            var user = HttpContext.User;

            var id = user.FindFirstValue(JwtRegisteredClaimNames.Sub)
                     ?? user.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? "N/A";

            var email = user.FindFirstValue(JwtRegisteredClaimNames.Email)
                        ?? user.FindFirstValue(ClaimTypes.Email)
                        ?? "N/A";

            var role = user.FindFirstValue(ClaimTypes.Role) ?? "N/A";

            return Ok(new
            {
                Id = id,
                Email = email,
                Role = role
            });
        }
    }
}