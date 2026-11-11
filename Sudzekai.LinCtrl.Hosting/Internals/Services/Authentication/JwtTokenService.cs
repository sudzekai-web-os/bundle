using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Sudzekai.LinCtrl.Hosting.Domain.Authentication;
using System.Globalization;

namespace Sudzekai.LinCtrl.Hosting
{
    internal sealed class JwtTokenService(TokenValidationParameters validation)
    {
        private const int _expiresIn = 900;
        private readonly JsonWebTokenHandler _tokens = new();

        public LoginToken Issue(User user)
        {
            var now = DateTime.UtcNow;
            var token = _tokens.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = validation.ValidIssuer,
                Audience = validation.ValidAudience,
                IssuedAt = now,
                NotBefore = now,
                Expires = now.AddSeconds(_expiresIn),
                SigningCredentials = new(validation.IssuerSigningKey, SecurityAlgorithms.HmacSha256),
                Claims = new Dictionary<string, object>
                {
                    ["sub"] = user.Id.ToString(CultureInfo.InvariantCulture),
                    ["name"] = user.Username,
                    ["role"] = user.Role.ToString()
                }
            });
            return new(token, "Bearer", _expiresIn);
        }
    }
}
