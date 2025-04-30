using jwtproject.Data;
using jwtproject.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(Option =>
Option.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(Option=>
{
    Option.Password.RequiredLength = 6;
    Option.Password.RequireNonAlphanumeric = false;
    Option.Password.RequireDigit = false;
    Option.Password.RequireLowercase = false;
    Option.Password.RequireUppercase = false;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(Option =>
{
    Option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    Option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    Option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(Option =>
    {
        Option.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization(Option =>
{
    Option.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
    Option.AddPolicy("UserPolicy", policy => policy.RequireRole("User"));
});



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(Options => Options.AddPolicy("MyTestCORS", policy =>
{
    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSwagger(x =>x.SerializeAsV2 = true);
app.UseHttpsRedirection();

app.UseCors("MyTestCORS");


app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
