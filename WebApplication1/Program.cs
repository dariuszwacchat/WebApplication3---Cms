using Data.Repos;
using Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Application.Services;
using Data.Repos.Abs;
using Application.Services.Abs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using Domain.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers (); 
builder.Services.AddEndpointsApiExplorer ();
builder.Services.AddSwaggerGen ();


// Po��czenie z baz� danych 
builder.Services.AddDbContext<ApplicationDbContext> (options =>
    options.UseSqlServer (builder.Configuration.GetConnectionString ("DefaultConnection")));

 
builder.Services.AddDefaultIdentity<ApplicationUser> (options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 10;

    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
    .AddRoles<ApplicationRole> ()
    .AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>> (TokenOptions.DefaultProvider)
    .AddEntityFrameworkStores<ApplicationDbContext> ();

builder.Services.AddAuthentication (o =>
{
    // zabezpieczenia token�w mailowych wysy�anych do potwierdzenia konta
    /*o.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
    o.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
    o.DefaultSignInScheme = IdentityConstants.ExternalScheme;*/
     
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer (options =>
    {
        var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]);
        var issuer = builder.Configuration["Jwt:Issuer"];
        var audience = builder.Configuration["Jwt:Audience"];

        options.TokenValidationParameters = new TokenValidationParameters ()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey (key)
        };
    });


builder.Services.AddAuthentication ()
    .AddCookie (cookie =>
    {
        cookie.LoginPath = "/Account/Login";
        cookie.AccessDeniedPath = "/Index/Home";
    });


builder.Services.AddAuthorization (options =>
{
    options.AddPolicy("Administrator", policy =>
    {
        policy.RequireRole ("Administrator");
    });
});


builder.Services.AddSession (options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromMinutes (30);
});


builder.Services.AddScoped <IModelRepository<Category>, CategoriesRepository> ();
builder.Services.AddScoped <IModelRepository<Subcategory>, SubcategoriesRepository> (); 
builder.Services.AddScoped <IModelRepository<Subsubcategory>, SubsubcategoriesRepository> ();
builder.Services.AddScoped <IModelRepository<Product>, ProductsRepository> ();
builder.Services.AddScoped <IModelRepository<PhotoProduct>, PhotoProducts> ();
builder.Services.AddScoped <IModelRepository<Marka>, MarkiRepository> ();
builder.Services.AddScoped <IModelRepository <RejestratorLogowania>, RejestratorLogowaniaRepository> ();
builder.Services.AddScoped <IModelRepository <LogException>, LogExceptionsRepository> ();

builder.Services.AddScoped<IRolesService<ApplicationRole>, RolesService> ();
builder.Services.AddScoped <AccountService> ();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment ())
{
    app.UseSwagger ();
    app.UseSwaggerUI ();
}

app.UseHttpsRedirection ();

app.UseAuthentication ();
app.UseAuthorization ();


app.UseCors (options =>
{
    options.WithOrigins ("http://localhost:10029")
    .AllowAnyMethod ()
    .AllowAnyHeader ();
});



app.MapControllers ();

app.Run ();