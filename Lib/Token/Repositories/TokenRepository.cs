using System;
using Token.Common.Entities;
using Token.Common.Interfaces;

namespace Token.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        public JwtToken SaveJwtToken(JwtToken token)
        {
            return null;
        }

        public JwtToken LoadTokenRecord(string token)
        {
            return null;
        }
    }
}
