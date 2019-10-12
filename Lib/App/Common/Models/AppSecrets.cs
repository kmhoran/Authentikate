using System;

namespace App.Common.Models
{
    public class AppSecret
    {
        public string UserSecret { get; set; }
        public string JwtSecret { get; set; }
        public string ConnectionString { get; set; }
    }
}