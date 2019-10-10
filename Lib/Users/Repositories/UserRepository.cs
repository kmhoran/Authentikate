using System;
using System.Collections.Generic;
using System.Linq;
using Auth.Common.Models;
using Microsoft.Extensions.Options;
using Users.Common.Entities;
using Users.Common.Interfaces;
using Users.Repositories.Contexts;

namespace Users.Repositories
{
    public class UserRepository : IUserRepository
    {
        private string _connectionString;

        public UserRepository(IOptions<AuthSecret> authSecret)
        {
            _connectionString = authSecret.Value.ConnectionString;
        }
        public User Authenticate(string username, string password)
        {
            return new User
            {
                UserId = 1,
                UserType = "normal-user",
                FirstName = "Joe",
                LastName = "Cool",
                Username = "j.cool@e.com",
                Password = "uriowtuirewutipoutirpoew",
            };
        }

        public IEnumerable<User> GetAllUsers()
        {
            using (var context = new UserContext(_connectionString))
            {
                var queryResult = context.Set<User>().AsQueryable();
                return queryResult.ToList();
            }
        }
    }
}
