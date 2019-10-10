using System;

namespace Token.Common.Models
{
    public class TokenSet
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string ExpirationUtc { get; set; }
    }
}