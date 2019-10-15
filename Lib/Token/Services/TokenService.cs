using System;
using System.Security.Principal;
using System.Threading.Tasks;
using App.Common.Models;
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

        public TokenService(ITokenRepository repo, IOptions<AppSecret> appSecret)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(ITokenRepository));
            if (appSecret.Value == null || string.IsNullOrEmpty(appSecret.Value.JwtSecret))
                throw new ArgumentNullException("Auth Secret");
            _secretKey = appSecret.Value.JwtSecret;
        }

        public async Task<AppWrapper<JwtToken>> LoadTokenRecordAsync(string token)
        {
            try
            {
                var principal = Jwt.GetPrincipalFromToken(token, this._secretKey);
                var username = principal.Identity.Name;

                var loadResponse = await _repo.LoadTokenRecordAsync(token);
                if (loadResponse == null || !loadResponse.Success) return loadResponse;

                var savedRecord = loadResponse.Data;
                if (savedRecord == null) return new AppWrapper<JwtToken>(null);
                if (savedRecord.Username != username)
                    return new AppWrapper<JwtToken>(new SecurityTokenException("Wrong User"), "Invalid Token");

                return new AppWrapper<JwtToken>(savedRecord);
            }
            catch (Exception ex)
            {
                return new AppWrapper<JwtToken>(ex, "Unable to LoadTokenRecordAsync");
            }
        }

        private DateTime GetTokenExpiration() => DateTime.UtcNow.AddHours(2);
        private DateTime GetRefreshExpiration() => DateTime.UtcNow.AddDays(1);

        public async Task<AppWrapper<TokenSet>> GenerateTokenSetAsync(string username)
        {
            try
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

                var saveResult = await _repo.SaveJwtTokenAsync(tokenRecord);
                if (!saveResult.Success)
                    return new AppWrapper<TokenSet>(saveResult.Exception, saveResult.Message);

                return new AppWrapper<TokenSet>(new TokenSet
                {
                    Token = token,
                    RefreshToken = refreshToken,
                    ExpirationUtc = expiration.ToISOString()
                });
            }
            catch (Exception ex)
            {
                return new AppWrapper<TokenSet>(ex, "Unable to GenerateTokenSetAsync");
            }
        }

        public async Task<AppWrapper<IPrincipal>> ValidateTokenAsync(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                    return new AppWrapper<IPrincipal>(new SecurityTokenException("Null token given"), "Invalid Token");

                var loadResponse = await LoadTokenRecordAsync(token);
                if (!loadResponse.Success)
                    return (AppWrapper)loadResponse as AppWrapper<IPrincipal>;

                var record = loadResponse.Data;
                if (record == null)
                    return new AppWrapper<IPrincipal>(new SecurityTokenException("No token record"), "Invalid Token");

                if (record.TokenExpirationUtc <= DateTime.UtcNow)
                    return new AppWrapper<IPrincipal>(new SecurityTokenException("Token Expired"), "Invalid Token");

                var principal = Jwt.GetPrincipalFromToken(token, this._secretKey);

                return new AppWrapper<IPrincipal>(principal);
            }
            catch (Exception ex)
            {
                return new AppWrapper<IPrincipal>(ex, "Unable to ValidateTokenAsync");
            }
        }

        public async Task<AppWrapper<TokenSet>> RefreshUserTokenAsync(string token, string refreshToken)
        {
            var loadResponse = await LoadTokenRecordAsync(token);
            if (!loadResponse.Success)
                return (AppWrapper)loadResponse as AppWrapper<TokenSet>;

            var savedRecord = loadResponse.Data;

            if (savedRecord == null)
                return new AppWrapper<TokenSet>(new SecurityTokenException("No token record"), "Invalid Token");
            if (savedRecord.RefreshToken != refreshToken)
                return new AppWrapper<TokenSet>(
                    new SecurityTokenException("Invalid refresh token. Will Not Refresh"), "Invalid Token");
            if (savedRecord.RefreshExpirationUtc < DateTime.UtcNow)
                return new AppWrapper<TokenSet>(new SecurityTokenException("Expired RefreshToken"), "Invalid Token");

            var removalResponse = await _repo.RemoveTokenRecordAsync(token);
            if (!removalResponse.Success) return removalResponse as AppWrapper<TokenSet>;

            return await GenerateTokenSetAsync(savedRecord.Username);
        }
    }
}
