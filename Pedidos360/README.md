# Pedidos360 — Inventario y Pedidos B2B

**Tecnologías:** ASP.NET Core MVC (.NET 9), Entity Framework Core 9 (Code First + Migrations), SQL Server, Identity, Bootstrap 5 y jQuery.

## Configuración del ambiente

### Requisitos previos

- **.NET 9.0** (Standard Term Support). Verificar con `dotnet --list-sdks`. Descarga: https://dotnet.microsoft.com/download/dotnet/9.0
- **Visual Studio 2026**
- **SQL Server** (Express o Developer) y **SQL Server Management Studio**
- (Opcional) Herramienta `dotnet-ef`:

```powershell
dotnet tool install --global dotnet-ef --version 9.0.6
```

### Cargas de trabajo de Visual Studio

El archivo [`.vsconfig`](.vsconfig) documenta las cargas de trabajo y componentes usados:

| Categoría | Carga de trabajo |
|---|---|
| Web y nube | Desarrollo de ASP.NET y web |
| Web y nube | Desarrollo de Python |
| Móviles y de escritorio | Desarrollo de la interfaz de usuario de aplicaciones multiplataforma de .NET |
| Móviles y de escritorio | Desarrollo de escritorio de .NET |
| Otros conjuntos de herramientas | Almacenamiento y procesamiento de datos |

Instalación:

- **Automática:** importar la configuración del archivo `.vsconfig` en el Visual Studio Installer.
- **Manual:** selección manual en *Visual Studio Installer*.

### Instalación de paquetes

Los paquetes ya están registrados en `Pedidos360.csproj` y se restauran solos al compilar. Estos comandos solo se usan si se crea el proyecto desde cero. Desde la terminal de Visual Studio (**Ver → Terminal**), en la carpeta del proyecto (`Pedidos360`):

```powershell
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 9.0.6
dotnet add package Microsoft.EntityFrameworkCore.Design --version 9.0.6
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 9.0.6
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore --version 9.0.6
dotnet add package Microsoft.AspNetCore.Identity.UI --version 9.0.6
dotnet add package Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore --version 9.0.6
```

Verificar la instalación con:

```powershell
dotnet list package
```

Todos los paquetes `Microsoft.*` deben tener la misma versión (9.0.6). No se actualizan paquetes de forma individual.

## Arquitectura

| Carpeta | Contenido |
|---|---|
| `Controllers/` | Controladores MVC; solo reciben la petición y llaman al servicio |
| `Services/` | Lógica de negocio y acceso a datos; cada servicio implementa su interfaz en `Services/Interfaces/` |
| `Models/` | Entidades de la base de datos con DataAnnotations |
| `ViewModels/` | Datos que necesita cada vista (filtros, paginación, formularios) |
| `Data/` | `ApplicationDbContext`, migraciones y `DatosIniciales` |
| `Views/` | Vistas Razor; `Shared/` contiene el layout y los partials `_Alertas` y `_Paginacion` |
| `Database/` | Script SQL de creación de la base |
| `docs/` | Diagrama ER, normalización, story map y prototipo |

## Reglas de trabajo en equipo

- `main` está protegida: todo cambio entra por Pull Request con al menos 1 aprobación.
- Cada integrante trabaja en su rama (`feature/<tema>`) y solo modifica sus archivos.
- Antes de abrir un PR: traer `main` a la rama, compilar y probar.
- Solo Esteban ejecuta `Add-Migration`. Las migraciones que ya están en `main` no se editan ni se borran.
- Después de cada `pull` de `main`, ejecutar `Update-Database`.

---

# Hito 1 — Avance funcional

## Responsabilidades

| Rol | Integrante | Responsabilidad | Rama |
|---|---|---|---|
| Coordinación del proyecto | Alberto José Ulate Velásquez | Solución base, repositorio, layout y partials, diagrama ER, scripts SQL, README, revisión de PRs | `feature/base-layout` |
| Base de datos | Esteban Eduardo Gamboa Román | Entidades, contexto, migración inicial, paginación reutilizable, normalización | `feature/modelo-datos` |
| Diseño de pantallas | Genesis Obando Cruz | CRUD de Productos y su prototipo | `feature/crud-productos` |
| Integración y búsquedas | Eidan Fabián Álvarez Gómez | CRUD de Clientes y su prototipo | `feature/crud-clientes` |
| Calidad y datos de prueba | Mónica Michelle García García | CRUD de Categorías, datos iniciales, story map y guion de la demo | `feature/categorias-datos-iniciales` |

Todos los integrantes deben tener commits y al menos un PR aprobado en `main`.

## Checklist

### Alberto José Ulate Velásquez — Solución base, ER, scripts SQL y README

- [x] Proyecto MVC .NET 9 con paquetes EF e Identity en 9.0.6
- [x] Estructura de carpetas y archivos base para cada integrante
- [x] `.gitignore` y repositorio en GitHub
- [ ] Identity configurado (contexto, `Program.cs` y login)
- [ ] `main` protegida (1 aprobación) y colaboradores invitados
- [ ] `DefaultConnection` apuntando a la base `Pedidos360`
- [ ] `_Layout` con menú (Inicio, Productos, Clientes, Categorías)
- [ ] Partial `_Paginacion` (página actual, total de páginas, conserva filtros)
- [ ] Partial `_Alertas` (mensajes de éxito y error con `TempData`)
- [ ] Diagrama ER con tablas, llaves y relaciones en `docs/`
- [ ] Script SQL de creación de la base generado con `Script-Migration -Idempotent` en `Database/`
- [ ] README con los pasos clonar → `Update-Database` → F5

### Esteban Eduardo Gamboa Román — Modelo de datos

- [ ] Entidades `Categoria`, `Producto`, `Cliente`, `Pedido` y `PedidoDetalle` con DataAnnotations
- [ ] `Producto.Imagen` como `byte[]` más tipo de archivo; montos en `decimal(18,2)`
- [ ] Relaciones con borrado restringido; índices únicos en `Cliente.Cedula` y `Categoria.Nombre`
- [ ] Migración `Inicial` en `main`; `Update-Database` funciona sobre una base vacía
- [ ] Clase de paginación reutilizable (`ListaPaginada<T>`) usada por todos los listados
- [ ] Justificación 1FN, 2FN y 3FN en `docs/`

### Genesis Obando Cruz — CRUD de Productos

- [ ] `IProductoService` y `ProductoService`; el controlador no usa el `DbContext` directamente
- [ ] Listado con filtro por nombre y categoría, 10 por página
- [ ] Crear: imagen obligatoria, JPG o PNG, máximo 2 MB
- [ ] Editar: imagen opcional; si no se sube, se conserva la actual
- [ ] Validación cliente y servidor: nombre requerido, precio > 0, stock ≥ 0, impuesto entre 0 y 100
- [ ] Eliminar bloqueado si el producto tiene pedidos, con mensaje en `_Alertas`
- [ ] Prototipo del layout, listado y formulario de Productos (capturas en `docs/prototipo/`)

### Eidan Fabián Álvarez Gómez — CRUD de Clientes

- [ ] `IClienteService` y `ClienteService`
- [ ] Listado con búsqueda por nombre o cédula, 10 por página
- [ ] Validación cliente y servidor: campos requeridos y correo válido
- [ ] Cédula duplicada muestra un mensaje claro en lugar de un error
- [ ] Eliminar bloqueado si el cliente tiene pedidos, con mensaje en `_Alertas`
- [ ] Prototipo del listado y formulario de Clientes (capturas en `docs/prototipo/`)

### Mónica Michelle García García — Categorías, datos iniciales y story map

- [ ] `ICategoriaService` y `CategoriaService`
- [ ] CRUD de Categorías: nombre requerido y único, listado paginado
- [ ] Eliminar bloqueado si la categoría tiene productos, con mensaje en `_Alertas`
- [ ] `DatosIniciales` (seeder): 2 categorías, 12 productos con imagen (`wwwroot/imagenes-iniciales/`) y 8 clientes; inserta solo si la tabla está vacía
- [ ] Story map con actividades, historias y criterios de aceptación en formato *Dado / Cuando / Entonces*
- [ ] Guion de demo de 5 a 7 minutos con el tiempo de cada integrante

### Criterio de terminado (todas las tareas)

- [ ] Compila y corre después de `Update-Database` sobre una base limpia
- [ ] PR aprobado por Alberto y fusionado en `main`
