using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using App.Common.Models;
using Users.Common.Entities;

namespace Users.Common.Interfaces
{
    public interface IUserService {
        Task<AppWrapper<User>> AuthenticateAsync(string username, string password);
        Task<AppWrapper<User>> GetUserAsync(string username);
        Task<AppWrapper<IList<User>>> GetAllUsersAsync();
        Task<AppWrapper<int>> SaveUserAsync(User user);
    }
}