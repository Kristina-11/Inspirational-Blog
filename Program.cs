using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Blog
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            try
            { 
                var scope = host.Services.CreateScope();
                var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                ctx.Database.EnsureCreated();

                var adminRole = new IdentityRole("Admin");

                if (!ctx.Roles.Any())
                {
                    // Create a role
                    roleManager.CreateAsync(adminRole).GetAwaiter().GetResult();
                }

                if (!ctx.Users.Any(u => u.UserName == "admin"))
                {
                    // Create an admin
                    var adminUser = new IdentityUser
                    {
                        UserName = "admin",
                        Email = "admin@test.com"
                    };

                    var result = userManager.CreateAsync(adminUser, "password").GetAwaiter().GetResult();

                    // Add role to a user
                    userManager.AddToRoleAsync(adminUser, adminRole.Name).GetAwaiter().GetResult();
                }
            } catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
