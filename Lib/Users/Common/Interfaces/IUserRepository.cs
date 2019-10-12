using System;
using System.Collections.Generic;
using App.Common.Models;
using Users.Common.Entities;

namespace Users.Common.Interfaces
{
    public interface IUserRepository {
        AppWrapper<User> Authenticate(string username, string password);
        AppWrapper<IList<User>> GetAllUsers();
    }
}