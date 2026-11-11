
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pedidos360.Data;
using Pedidos360.Services;
using Pedidos360.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Controladores y protección de formularios
builder.Services.AddControllersWithViews(options =>
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));

builder.Services.AddAntiforgery(options =>
    options.HeaderName = "RequestVerificationToken");

// Base de datos
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Usuarios y roles
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
    options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Servicios
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IBusquedaProductoService, BusquedaProductoService>();
builder.Services.AddScoped<ICalculoPedidoService, CalculoPedidoService>();

// Configuración de acceso
builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Acceso/Denegado";

    options.Events.OnRedirectToLogin = contexto =>
    {
        if (contexto.Request.Path.StartsWithSegments("/api"))
        {
            contexto.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        }

        contexto.Response.Redirect(contexto.RedirectUri);
        return Task.CompletedTask;
    };

    options.Events.OnRedirectToAccessDenied = contexto =>
    {
        if (contexto.Request.Path.StartsWithSegments("/api"))
        {
            contexto.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        }

        contexto.Response.Redirect(contexto.RedirectUri);
        return Task.CompletedTask;
    };
});

var app = builder.Build();

// Datos iniciales y roles
await DatosIniciales.InicializarAsync(app.Services);
await InicializadorRoles.InicializarAsync(app.Services, app.Configuration);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages();

app.Run();
