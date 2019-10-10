using System;
using System.Collections.Generic;
using Users.Common.Entities;

namespace Users.Common.Interfaces
{
    public interface IUserService {
        User Authenticate(string username, string password);
        IEnumerable<User> GetAllUsers();
    }
}