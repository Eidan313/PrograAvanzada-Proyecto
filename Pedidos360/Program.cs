using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pedidos360.Data;
using Pedidos360.Services;
using Pedidos360.Services.Interfaces;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();


builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Acceso/Denegado";
});

var app = builder.Build();

await DatosIniciales.InicializarAsync(app.Services);
await InicializadorRoles.InicializarAsync(app.Services, app.Configuration);


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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

