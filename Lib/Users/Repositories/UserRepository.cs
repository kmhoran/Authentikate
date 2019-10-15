using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Users.Common.Entities;
using Users.Common.Interfaces;
using Users.Repositories.Contexts;

namespace Users.Repositories
{
    public class UserRepository : IUserRepository
    {
        private string _connectionString;

        public UserRepository(IOptions<AppSecret> appSecret)
        {
            _connectionString = appSecret.Value.MySqlConnectionString;
        }

        public async Task<AppWrapper<User>> GetUserAsync(string username)
        {
            try
            {
                using (var context = new UserContext(_connectionString))
                {
                    var set = context.Set<User>().AsQueryable();
                    return new AppWrapper<User>(await set.FirstOrDefaultAsync(x => x.Username == username));
                }
            }
            catch (Exception ex)
            {
                return new AppWrapper<User>(ex, $"Unable to GetUser");
            }
        }

        public async Task<AppWrapper<IList<User>>> GetAllUsersAsync()
        {
            try
            {
                using (var context = new UserContext(_connectionString))
                {
                    var set = context.Set<User>().AsQueryable();
                    return new AppWrapper<IList<User>>(await set.ToListAsync());
                }
            }
            catch (Exception ex)
            {
                return new AppWrapper<IList<User>>(ex, "Unable to GetAllUsers");
            }
        }

        public async Task<AppWrapper<int>> SaveUserAsync(User toSave)
        {
            try
            {
                using (var context = new UserContext(_connectionString))
                {
                    var isNew = toSave.UserId == 0;
                    context.Entry(toSave).State = isNew ? EntityState.Added : EntityState.Modified;
                    return new AppWrapper<int>(await context.SaveChangesAsync());
                }
            }
            catch (Exception ex)
            {
                return new AppWrapper<int>(ex, "Unable to SaveUser");
            }
        }
    }
}
