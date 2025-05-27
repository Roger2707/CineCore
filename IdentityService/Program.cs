using IdentityService.Data;
using IdentityService.Models;
using IdentityService.Services;
using IdentityService.Services.IServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

#region Connect DB

builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServer")));

#endregion

#region Authentication and Authorization and JWT Bearer

builder.Services
    .AddIdentity<User, Role>(opt =>
    {
        opt.Password.RequiredLength = 7;
        opt.Password.RequireDigit = false;
        opt.Password.RequireUppercase = false;
        opt.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<AuthDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<DataProtectionTokenProviderOptions>(options => {
    options.TokenLifespan = TimeSpan.FromHours(2);
});

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection("Jwt");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"])),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

#endregion


#region Services

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IIdentityService, IdentityService.Services.IdentityService>();

#endregion 

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
