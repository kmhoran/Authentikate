using System;
using System.Threading.Tasks;
using App.Common.Models;
using Microsoft.Extensions.Caching.Distributed;
using Token.Common.Entities;
using Token.Common.Interfaces;
using Utils.Extentions;

namespace Token.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private IDistributedCache _cache;

        public TokenRepository(IDistributedCache cache)
        {
            _cache = cache;
        }
        public async Task<AppWrapper<JwtToken>> SaveJwtTokenAsync(JwtToken token)
        {
            try
            {
                await _cache.SetAsync($"TOKEN:{token.Token}", token.ToBytes());
                return new AppWrapper<JwtToken>(token);
            }
            catch (Exception ex)
            {
                return new AppWrapper<JwtToken>(ex, "Unable to SaveJwtTokenAsync");
            }
        }

        public async Task<AppWrapper<JwtToken>> LoadTokenRecordAsync(string token)
        {
            try
            {
                var encodedToken = await _cache.GetAsync($"TOKEN:{token}");
                return new AppWrapper<JwtToken>(encodedToken.FromBytes<JwtToken>());
            }
            catch (Exception ex)
            {
                return new AppWrapper<JwtToken>(ex, "Unable to LoadTokenRecordAsync");
            }
        }

        public async Task<AppWrapper> RemoveTokenRecordAsync(string token)
        {
            try
            {
                await _cache.RemoveAsync($"TOKEN:{token}");
                return new AppWrapper();
            }
            catch (Exception ex)
            {
                return new AppWrapper(ex, "Unable to RemoveTokenRecordAsync");
            }
        }
    }
}
