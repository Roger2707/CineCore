using IdentityService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Data
{
    public class AuthDbContext : IdentityDbContext<User, Role, Guid>
    {
        public AuthDbContext(DbContextOptions options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            #region Seed Users

            var seedUsers = new List<User>()
            {
                new User
                {
                    Id = Guid.NewGuid(),
                    UserName = "spadmin",
                    NormalizedUserName = "SPADMIN",
                    FullName = "SuperAdmin",
                    Email = "spadmin@example.com",
                    NormalizedEmail = "SPADMIN@EXAMPLE.COM",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    UserName = "admin1",
                    FullName = "Admin1",
                    NormalizedUserName = "ADMIN1",
                    Email = "admin1@example.com",
                    NormalizedEmail = "ADMIN1@EXAMPLE.COM",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    UserName = "test",
                    FullName = "Test Customer",
                    NormalizedUserName = "TEST",
                    Email = "test@example.com",
                    NormalizedEmail = "TEST@EXAMPLE.COM",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                }
            };

            // Hash mật khẩu
            PasswordHasher<User> hasher = new PasswordHasher<User>();
            seedUsers.ForEach(u => u.PasswordHash = hasher.HashPassword(u, $"{u.UserName}@123"));
            builder.Entity<User>().HasData(seedUsers);

            #endregion

            #region Seed Roles

            var seedRoles = new List<Role>()
            {
                new Role { Id = Guid.NewGuid(), Name = "SuperAdmin", NormalizedName = "SUPERADMIN" },
                new Role { Id = Guid.NewGuid(), Name = "Admin", NormalizedName = "ADMIN" },
                new Role { Id = Guid.NewGuid(), Name = "Customer", NormalizedName = "CUSTOMER" }
            };
            builder.Entity<Role>().HasData(seedRoles);

            #endregion

            #region Seed UserRoles

            var userUserRoles = new List<IdentityUserRole<Guid>>
            {
                new IdentityUserRole<Guid>
                {
                    UserId = seedUsers[0].Id,
                    RoleId = seedRoles[0].Id
                },
                new IdentityUserRole<Guid>
                {
                    UserId = seedUsers[1].Id,
                    RoleId = seedRoles[1].Id
                },
                new IdentityUserRole<Guid>
                {
                    UserId = seedUsers[2].Id,
                    RoleId = seedRoles[2].Id
                },
            };
            builder.Entity<IdentityUserRole<Guid>>().HasData(userUserRoles);

            #endregion
        }
    }
}
