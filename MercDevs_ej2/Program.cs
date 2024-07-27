using MercDevs_ej2.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Conexión a la base de datos
builder.Services.AddDbContext<MercyDeveloperContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("connection"),
    Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.4.25-mariadb")));

// Configuración de autenticación
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/Ingresar";
        options.ExpireTimeSpan = TimeSpan.FromDays(1);
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
IApplicationBuilder applicationBuilder = app.UseAuthorization();

// Configuración de rutas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Ingresar}/{id?}");

app.Run();
