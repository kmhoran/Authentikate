using System;

namespace Token.Common.Entities
{
    public class JwtToken
    {
        public int TokenId { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }
        public DateTime TokenExpirationUtc { get; set; }
        public string RefreshToken { get; set; }        
        public DateTime RefreshExpirationUtc { get; set; }
        public DateTime DateIssuedUtc { get; set; }
        public DateTime? DateRefreshedUtc { get; set; }
    }
}