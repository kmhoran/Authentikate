using System;

namespace Auth.Common.Models
{
    public class AuthSecret
    {
        public string UserSecret { get; set; }
        public string JwtSecret { get; set; }
        public string ConnectionString { get; set; }
    }
}