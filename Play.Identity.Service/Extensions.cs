using Play.Identity.Service.Dtos;
using Play.Identity.Service.Entities;

namespace Play.Identity.Service
{
    public static class Extensions
    {
        public static UserDto AsDto(this User user)
        {
            return new UserDto(user.Id, user.Username, user.Nickname, user.Balance);
        }

        public static User AsUser(this RegisterRequest registerRequest)
        {
            return new User
            {
                Username = registerRequest.Username,
                Password = registerRequest.Password,
                Nickname = registerRequest.Nickname
            };
        }

        public static User AsUser(this LoginRequest loginRequest)
        {
            return new User
            {
                Username = loginRequest.Username,
                Password = loginRequest.Password
            };
        }
    }
}
