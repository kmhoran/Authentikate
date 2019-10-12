using System;
using System.Threading.Tasks;
using App.Common.Models;
using Token.Common.Entities;

namespace Token.Common.Interfaces
{
    public interface ITokenRepository {
        Task<AppWrapper<JwtToken>> SaveJwtTokenAsync(JwtToken token);
        Task<AppWrapper<JwtToken>> LoadTokenRecordAsync(string token);
        Task<AppWrapper> RemoveTokenRecordAsync(string token);
    }
}