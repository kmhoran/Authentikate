using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Utils.Extentions;

namespace Utils.Encrypt
{
    public static class Jwt
    {
        private static string _defaultAlgorithm = SecurityAlgorithms.HmacSha256;

        public static TokenValidationParameters GetAppTokenValidationParameters(byte[] secretKey)
        {
            return new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false
            };
        }


        public static string GenerateJwt(string username, string secretKey, DateTime expiration)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var keyBytes = secretKey.ToBytes();


            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, username)
                }),
                Expires = expiration,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), _defaultAlgorithm)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rando = RandomNumberGenerator.Create())
            {
                rando.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public static ClaimsPrincipal GetPrincipalFromToken(string token, string secretKey)
        {
            var secretBytes = secretKey.ToBytes();
            var validationParameters = GetAppTokenValidationParameters(secretBytes);

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, validationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            
            if (
                jwtSecurityToken == null ||
                !jwtSecurityToken.Header.Alg
                    .Equals(_defaultAlgorithm, StringComparison.InvariantCultureIgnoreCase)
                )
                throw new SecurityTokenException("Invalid Token");

            return principal;
        }


    }
}
