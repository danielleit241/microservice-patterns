using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Play.Common;
using Play.Identity.Service.Dtos;
using Play.Identity.Service.Entities;

namespace Play.Identity.Service.Controllers
{

    [ApiController]
    [Route("api/users")]
    public class UserController(IRepository<User> userRepository, JwtTokenService jwtService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest dto)
        {
            var user = await userRepository.GetAsync(u => u.Username == dto.Username);
            if (user != null)
            {
                return BadRequest("Username is already taken");
            }
            user = new User
            {
                Username = dto.Username,
                Nickname = dto.Nickname,
                Password = dto.Password
            };
            await userRepository.CreateAsync(user);
            return Ok(user.AsDto());
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest dto)
        {
            var user = await userRepository.GetAsync(u => u.Username == dto.Username && u.Password == dto.Password);
            if (user == null)
            {
                return Unauthorized();
            }
            var token = jwtService.GenerateToken(user);
            return Ok(new { Token = token });
        }

        [HttpGet("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await userRepository.GetAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user.AsDto());
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var users = await userRepository.GetAllAsync();
            var usersDto = users.Select(u => u.AsDto());
            return Ok(usersDto);
        }

        [HttpPost("{id:guid}/debit")]
        [Authorize]
        public async Task<IActionResult> DebitBalance(Guid id, DebitRequestDto dto)
        {
            if (dto.Amount <= 0)
            {
                return BadRequest("Amount must be greater than zero");
            }
            var user = await userRepository.GetAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            if (user.Balance < dto.Amount)
            {
                return BadRequest("Insufficient balance");
            }
            user.Balance -= dto.Amount;
            await userRepository.UpdateAsync(user);
            return Ok();
        }

        [HttpPost("{id:guid}/credit")]
        [Authorize]
        public async Task<IActionResult> CreditBalance(Guid id, DebitRequestDto dto)
        {
            if (dto.Amount <= 0)
            {
                return BadRequest("Amount must be greater than zero");
            }
            var user = await userRepository.GetAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            user.Balance += dto.Amount;
            await userRepository.UpdateAsync(user);
            return Ok();
        }
    }
}
