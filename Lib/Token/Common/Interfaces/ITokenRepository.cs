using System;
using Token.Common.Entities;

namespace Token.Common.Interfaces
{
    public interface ITokenRepository {
        JwtToken SaveJwtToken(JwtToken token);
        JwtToken LoadTokenRecord(string token);
    }
}