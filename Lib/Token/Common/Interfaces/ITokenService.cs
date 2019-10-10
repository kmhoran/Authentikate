using System;
using System.Security.Claims;
using Token.Common.Models;

namespace Token.Common.Interfaces
{
    public interface ITokenService {
        TokenSet GenerateTokenSet(string username);
        TokenSet RefreshUserToken(string token, string refreshToken);
    }
}