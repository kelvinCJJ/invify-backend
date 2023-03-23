using Invify.Dtos;
using Invify.Dtos.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Data;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Invify.Infrastructure
{
    public class TokenGenerator
    {
        public string GenerateToken(IdentityUser user, string role)
        {
            try
            {
                var expiration =  DateTime.UtcNow.AddDays(7);
                var token = GenerateJwtToken(
                    GenerateClaims(user, role),
                    GenerateSigningCredentials(),
                    expiration
                );
                var tokenHandler = new JwtSecurityTokenHandler();
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;

            }
        }
       
        private JwtSecurityToken GenerateJwtToken(List<Claim> claims, SigningCredentials credentials,
            DateTime expiration) =>
            new(
                "InvifyAuthBackend",
                "InvifyAuthBackend",
                claims,
                expires: expiration,
                signingCredentials: credentials
            );

        private List<Claim> GenerateClaims(IdentityUser user, string role)
        {
            try
            {
                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, "InvifyAuthToken"),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)),
                    new Claim("username", user.UserName),
                    new Claim("role", role),
                };

                return claims;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        private SigningCredentials GenerateSigningCredentials()
        {
            return new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes("!InvifySecret12345!")
                ),
                SecurityAlgorithms.HmacSha256
            );
        }


    }
}
