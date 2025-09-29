using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Play.Identity.Service.Settings
{
    public class JwtBearerConfigurationOptionsSettings(IConfiguration configuration) : IConfigureNamedOptions<JwtBearerOptions>
    {
        private const string SectionName = "JWTBearer";
        public void Configure(JwtBearerOptions options)
        {
            configuration.GetSection(SectionName).Bind(options);
            var secretKey = configuration[$"{SectionName}:SecretKey"];
            if (!string.IsNullOrEmpty(secretKey))
            {
                options.TokenValidationParameters.IssuerSigningKey =
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            }
        }

        public void Configure(string? Name, JwtBearerOptions options)
        {
            Configure(options);
        }
    }
}
