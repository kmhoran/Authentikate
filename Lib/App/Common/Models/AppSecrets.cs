using System;

namespace App.Common.Models
{
    public class AppSecret
    {
        public string UserSecret { get; set; }
        public string JwtSecret { get; set; }
        public string MySqlConnectionString { get; set; }
        public string RedisConfig { get; set; }
    }
}