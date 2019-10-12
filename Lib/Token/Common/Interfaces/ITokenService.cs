using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using App.Common.Models;
using Token.Common.Entities;
using Token.Common.Models;

namespace Token.Common.Interfaces
{
    public interface ITokenService {
        Task<AppWrapper<JwtToken>> LoadTokenRecordAsync(string token);
        Task<AppWrapper<IPrincipal>> ValidateTokenAsync(string token);
        Task<AppWrapper<TokenSet>> GenerateTokenSetAsync(string username);
        Task<AppWrapper<TokenSet>> RefreshUserTokenAsync(string token, string refreshToken);
    }
}