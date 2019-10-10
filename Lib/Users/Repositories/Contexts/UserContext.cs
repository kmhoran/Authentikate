using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Users.Common.Entities;

namespace Users.Repositories.Contexts
{
    public class UserContext : DbContext
    {
        public string ConnectionString;
        private Action<MySqlDbContextOptionsBuilder> _sqlOptions;
        public UserContext(string connectionString, Action<MySqlDbContextOptionsBuilder> options = null)
        : base()
        {
            ConnectionString = connectionString;
            _sqlOptions = options;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionBuilder)
        {
            optionBuilder.UseMySql(ConnectionString, _sqlOptions);
            base.OnConfiguring(optionBuilder);
        }

        public DbSet<User> Users { get; set; }
    }
}