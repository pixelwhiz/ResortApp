using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ResortApp.Application.Common.Interfaces;
using ResortApp.Application.Common.Utility;
using ResortApp.Domain.Entities;

namespace ResortApp.Infrastructure.Data;

public class DbInitializer : IDbInitializer
{

    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _db;

    public DbInitializer(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext db
        ) {
        _roleManager = roleManager;
        _userManager = userManager;
        _db = db;
    }

    public void Initializer()
    {
        try
        {
            if (_db.Database.GetPendingMigrations().Count() > 0)
            {
                _db.Database.Migrate();
            }

            if (!_roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).Wait();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).Wait();

                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "admin@resortapp.com",
                    Email = "admin@resortapp.com",
                    Name = "Muhammad Daffa",
                    NormalizedUserName = "ADMIN@RESORTAPP.COM",
                    NormalizedEmail = "ADMIN@RESORTAPP.COM",
                    PhoneNumber = "081808108514",
                }, "Admin123*").GetAwaiter().GetResult();

                ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "admin@resortapp.com");
                _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}