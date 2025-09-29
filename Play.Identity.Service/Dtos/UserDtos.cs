namespace Play.Identity.Service.Dtos
{
    public record UserDto(Guid Id, string Username, string Nickname, double Balance);
    public record RegisterRequest(string Username, string Password, string Nickname);
    public record LoginRequest(string Username, string Password);
    public record DebitRequestDto(double Amount, string IdempotencyKey);
}
