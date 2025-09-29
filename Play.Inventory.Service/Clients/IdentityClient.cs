using Play.Inventory.Service.Dtos;
using System.Net.Http.Headers;

namespace Play.Inventory.Service.Clients
{
    public record DebitRequestDto(double Amount, string IdempotencyKey);

    public class IdentityClient
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IdentityClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        private void ForwardJwtBearer()
        {
            var authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        public async Task<IReadOnlyCollection<UserDto>> GetUsersAsync()
        {
            ForwardJwtBearer();
            var users = await _httpClient.GetFromJsonAsync<IReadOnlyCollection<UserDto>>("/api/users");
            return users ?? Enumerable.Empty<UserDto>().ToList();
        }

        public async Task<UserDto?> GetUserAsync(Guid userId)
        {
            ForwardJwtBearer();
            var user = await _httpClient.GetFromJsonAsync<UserDto>($"/api/users/{userId}");
            return user;
        }

        public async Task<bool> DebitAsync(Guid userId, DebitRequestDto dto)
        {
            ForwardJwtBearer();
            var response = await _httpClient.PostAsJsonAsync($"/api/users/{userId}/debit", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> CreditAsync(Guid userId, DebitRequestDto dto)
        {
            ForwardJwtBearer();
            var response = await _httpClient.PostAsJsonAsync($"/api/users/{userId}/credit", dto);
            return response.IsSuccessStatusCode;
        }
    }
}
