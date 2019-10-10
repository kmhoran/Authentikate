using System;
using Microsoft.Extensions.Options;
using Users.Common.Interfaces;
using Utils.Encrypt;
using Auth.Common.Models;
using Users.Common.Entities;
using System.Collections.Generic;

namespace Users.Services
{
    public class UserService : IUserService
    {
        private IUserRepository _repo;
        private string _secretKey;

        public UserService(IUserRepository repo, IOptions<AuthSecret> authSecret)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(IUserRepository));
            if (authSecret.Value == null || string.IsNullOrEmpty(authSecret.Value.UserSecret))
                throw new ArgumentNullException("Auth Secret");
            _secretKey = authSecret.Value.UserSecret;
        }

        public User Authenticate(string username, string password)
        {
            var encodedPass = Cryptic.Encrypt(password, this._secretKey);
            return _repo.Authenticate(username, encodedPass);
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _repo.GetAllUsers();
        }
    }
}
