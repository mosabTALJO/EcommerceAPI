using EcommerceAPI.Data.Models;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Shared.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EcommerceAPI.Core.Auths
{
    public class TokenGenerator
    {
        public static async Task<TokenViewModel> CreateToken(AppUser user, UserManager<AppUser> _userManager, IConfiguration _configuration)
        {
            var claims = await _userManager.GetClaimsAsync(user) ?? new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, user.Id));

            if (user.UserType == UserTypes.Customer)
                claims.Add(new Claim(ClaimTypes.Role, "Customer"));
            if (user.UserType == UserTypes.Admin)
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));

            var signinKey = new SymmetricSecurityKey(
              Encoding.UTF8.GetBytes(_configuration["Jwt:SigningKey"]));

            var token = new JwtSecurityToken(
              issuer: _configuration["Jwt:Site"],
              audience: _configuration["Jwt:Site"],
              expires: DateTime.UtcNow.AddDays(60),
              claims: claims,
              signingCredentials: new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256)
            );

            return new TokenViewModel
            {
                userID = user.Id,
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo,
                UserName = user.UserName,
                DisplayName = user.DisplayName,
                Country = user.Country,
                Claims = (await _userManager.GetClaimsAsync(user)).Select(x => (Policies)int.Parse(x.Type)).ToList()
            };
        }
        public static string GeneratePassword(int passLength)
        {
            var chars = "abcdefghijklmnopqrstuvwxyz@#$&ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, passLength)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
            return result;
        }
    }
}
