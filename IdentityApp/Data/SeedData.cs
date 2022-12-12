using IdentityApp.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace IdentityApp.Data
{
    public class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, string password="Abcd12345!")
        {
            using (var context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                //Manager
                var managerUid = await EnsureUser(serviceProvider, "manager@test.com", password);
                await EnsureRole(serviceProvider, managerUid, Constants.InvoiceManagersRole);

                //Administrator
                var adminUid = await EnsureUser(serviceProvider, "admin@test.com", password);
                await EnsureRole(serviceProvider, adminUid, Constants.InvoiceAdminRole);
            }
        }
        private static async Task<string> EnsureUser(IServiceProvider serviceProvider, string userName, string initialPw)
        {
            //Service provider to grab user manager.
            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();
            
            //Checks if user is already created with that user name.
            var user = await userManager.FindByNameAsync(userName);
           
            //If user is not created already, create it in the database.
            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = userName,
                    Email = userName,
                    EmailConfirmed = true
                };
                var result= await userManager.CreateAsync(user, initialPw);
            }
            //Checks for errors, returns success or unsucessful.
            if(user == null)
            {
                throw new Exception("Error, user was not created.");
            }
            return user.Id;            
        }
        private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider, string uid, string role)
        {
            var roleManager= serviceProvider.GetService<RoleManager<IdentityRole>>();
            
            IdentityResult ir;
            
            //Checks if this specific role exists in the database, if not, create the role.
            if(await roleManager.RoleExistsAsync(role) == false)
            {
                ir = await roleManager.CreateAsync(new IdentityRole(role));
            }

            //Checks if user exist in the database by user ID.
            var userManager=serviceProvider.GetService<UserManager<IdentityUser>>();
            var user = await userManager.FindByIdAsync(uid);
            
            if (user == null)
            {
                throw new Exception("User does not exist");
            }

            //Adds user to this specific role.
            ir=await userManager.AddToRoleAsync(user, role);

            return ir;
        }

    }
}
