using EcommerceAPI.Core.Extentions;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Shared.Helpers;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
namespace EcommerceAPI.Seeds
{
    public class MySeedInitializer
    {
        public static void SeedData(UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            SeedRoles(roleManager).Wait();
            SeedUsers(userManager).Wait();
        }

        private static async Task SeedUsers(UserManager<AppUser> userManager)
        {
            var user = userManager.Users.FirstOrDefault(x => x.Email == "admin@trgatestore.com");
            if (user is null)
            {
                user = new AppUser { UserName = "admin", DisplayName = "Admin", Email = "admin@trgatestore.com", IsActive = true, SecurityStamp = Guid.NewGuid().ToString() };
                await userManager.CreateAsync(user, "P@ssw0rd");
            }
            //Remove currenct policies
            await userManager.RemoveClaimsAsync(user, await userManager.GetClaimsAsync(user));
            //Add all policies
            var claims = new List<Claim>();
            foreach (Policies p in Enum.GetValues(typeof(Policies)))
                claims.Add(new Claim(p.GetPolicyNumber(), p.GetPolicyNumber()));
            await userManager.AddClaimsAsync(user, claims);


            var user1 = userManager.Users.FirstOrDefault(x => x.Email == "msh@trgatestore.com");
            if (user1 is null)
            {
                user1 = new AppUser { UserName = "msh", DisplayName = "MSH", Email = "msh@trgatestore.com", IsActive = true, SecurityStamp = Guid.NewGuid().ToString() };
                await userManager.CreateAsync(user1, "P@ssw0rd111");
            }
            //Remove currenct policies
            await userManager.RemoveClaimsAsync(user1, await userManager.GetClaimsAsync(user1));
            //Add all policies
            claims = new List<Claim>();
            foreach (Policies p in Enum.GetValues(typeof(Policies)))
                claims.Add(new Claim(p.GetPolicyNumber(), p.GetPolicyNumber()));
            await userManager.AddClaimsAsync(user1, claims);


            var user2 = userManager.Users.FirstOrDefault(x => x.Email == "apple@apple.apple");
            if (user2 is null)
            {
                user2 = new AppUser { UserName = "apple", DisplayName = "apple", Email = "apple@apple.apple", IsActive = true, SecurityStamp = Guid.NewGuid().ToString() };
                await userManager.CreateAsync(user2, "123456");
                await userManager.AddToRoleAsync(user2, "Customer");
            }
            else
            {
                await userManager.RemovePasswordAsync(user2);
                await userManager.AddPasswordAsync(user2, "123456");
            }
        }
        private static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("Customer"))
                await roleManager.CreateAsync(new IdentityRole("Customer"));
        }
    }
}
