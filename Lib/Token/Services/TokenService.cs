using System;
using Auth.Common.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Token.Common.Entities;
using Token.Common.Interfaces;
using Token.Common.Models;
using Utils.Encrypt;
using Utils.Extentions;

namespace Token.Services
{
    public class TokenService : ITokenService
    {
        private string _secretKey;
        private ITokenRepository _repo;

        public TokenService(ITokenRepository repo, IOptions<AuthSecret> authSecret)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(ITokenRepository));
            if (authSecret.Value == null || string.IsNullOrEmpty(authSecret.Value.JwtSecret))
                throw new ArgumentNullException("Auth Secret");
            _secretKey = authSecret.Value.JwtSecret;
        }
        private DateTime GetTokenExpiration() => DateTime.UtcNow.AddHours(2);
        private DateTime GetRefreshExpiration() => DateTime.UtcNow.AddDays(1);

        public TokenSet GenerateTokenSet(string username)
        {
            var expiration = GetTokenExpiration();
            var token = Jwt.GenerateJwt(username, this._secretKey, expiration);

            var refreshExpiration = GetRefreshExpiration();
            var refreshToken = Jwt.GenerateRefreshToken();

            var tokenRecord = new JwtToken
            {
                Username = username,
                Token = token,
                TokenExpirationUtc = expiration,
                RefreshToken = refreshToken,
                RefreshExpirationUtc = refreshExpiration,
                DateIssuedUtc = DateTime.UtcNow
            };

            _repo.SaveJwtToken(tokenRecord);

            return new TokenSet
            {
                Token = token,
                RefreshToken = refreshToken,
                ExpirationUtc = expiration.ToISOString()
            };
        }

        public string ValidateToken(string token){
            return token;
        }

        public TokenSet RefreshUserToken(string token, string refreshToken)
        {
            var principal = Jwt.GetPrincipalFromToken(token, this._secretKey);
            var username = principal.Identity.Name;
            var savedRecord = _repo.LoadTokenRecord(token);

            if (savedRecord == null || savedRecord.RefreshToken != refreshToken)
                throw new SecurityTokenException("Invalid Token");
            if (savedRecord.DateRefreshedUtc != null) throw new SecurityTokenException("Refesh Token Already Used");
            if (savedRecord.RefreshExpirationUtc < DateTime.UtcNow) throw new SecurityTokenException("Expired RefreshToken");
            if (savedRecord.Username != username) throw new SecurityTokenException("Wrong User");
            
            savedRecord.DateRefreshedUtc = DateTime.UtcNow;
            _repo.SaveJwtToken(savedRecord);

            return GenerateTokenSet(username);
        }
    }
}
