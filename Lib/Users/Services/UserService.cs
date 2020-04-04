using System;
using Microsoft.Extensions.Options;
using Users.Common.Interfaces;
using Utils.Encrypt;
using Users.Common.Entities;
using System.Collections.Generic;
using App.Common.Models;
using System.Threading.Tasks;

namespace Users.Services
{
    public class UserService : IUserService
    {
        private IUserRepository _repo;
        private string _secretKey;

        public UserService(IUserRepository repo, IOptions<AppSecret> appSecret)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(IUserRepository));
            if (appSecret.Value == null || string.IsNullOrEmpty(appSecret.Value.UserSecret))
                throw new ArgumentNullException("Auth Secret");
            _secretKey = appSecret.Value.UserSecret;
        }

        public async Task<AppWrapper<User>> AuthenticateAsync(string username, string password)
        {
            var encodedPass = Cryptic.Encrypt(password, this._secretKey);
            var queryResponse = await _repo.GetUserAsync(username);

            if (!queryResponse.Success) return queryResponse;

            var user = queryResponse.Data;
            if (user == null) return new AppWrapper<User>(null);

            if (user.Password != encodedPass)
            {
                Console.WriteLine($"DB PASS: {user.Password}");
                Console.WriteLine($"LOGIN PASS: {encodedPass}");
                return new AppWrapper<User>(
                    new ArgumentException("Password does not match the one found on file"), "Bad credentials");
            }

            return new AppWrapper<User>(user);
        }

        public async Task<AppWrapper<User>> GetUserAsync(string username)
        {
            return await _repo.GetUserAsync(username);
        }

        public async Task<AppWrapper<IList<User>>> GetAllUsersAsync()
        {
            return await _repo.GetAllUsersAsync();
        }

        public async Task<AppWrapper<int>> SaveUserAsync(User user)
        {
            return await _repo.SaveUserAsync(user);
        }
    }
}
