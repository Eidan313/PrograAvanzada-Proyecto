using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Pedidos360.Data;

public static class InicializadorRoles
{
    public static async Task InicializarAsync(IServiceProvider servicios, IConfiguration configuracion)
    {
        using var scope = servicios.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var roles = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var usuarios = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Roles");

        try
        {
            if (!await db.Database.CanConnectAsync() || (await db.Database.GetPendingMigrationsAsync()).Any())
                return;

            foreach (var nombre in new[] { "Admin", "Vendedor" })
                if (!await roles.RoleExistsAsync(nombre))
                {
                    var resultado = await roles.CreateAsync(new IdentityRole(nombre));
                    if (!resultado.Succeeded)
                        throw new InvalidOperationException(string.Join("; ", resultado.Errors.Select(e => e.Description)));
                }

            // El administrador se designa explícitamente por configuración; nunca por registro público.
            var correoAdmin = configuracion["Seguridad:CorreoAdministrador"]?.Trim();
            foreach (var usuario in await usuarios.Users.ToListAsync())
            {
                var esAdmin = !string.IsNullOrWhiteSpace(correoAdmin) &&
                    string.Equals(usuario.Email, correoAdmin, StringComparison.OrdinalIgnoreCase);
                if (esAdmin && !await usuarios.IsInRoleAsync(usuario, "Admin"))
                    await usuarios.AddToRoleAsync(usuario, "Admin");
                if (!esAdmin && !await usuarios.IsInRoleAsync(usuario, "Admin") && !await usuarios.IsInRoleAsync(usuario, "Vendedor"))
                    await usuarios.AddToRoleAsync(usuario, "Vendedor");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "No fue posible inicializar los roles. Revise la conexión y migraciones.");
        }
    }
}
