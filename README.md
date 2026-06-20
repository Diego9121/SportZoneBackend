# SportZone — Backend

Backend del sistema de Punto de Venta (POS) con control de inventario para la tienda deportiva **Sport Zone**, desarrollado como proyecto de defensa de grado.

## Tecnologías

- **C# / .NET 8**
- **ASP.NET Core Web API**
- **Entity Framework Core 8** (Code First)
- **SQL Server** (Express)
- **Swagger / OpenAPI**
- **Clean Architecture**

## Arquitectura

El proyecto está organizado en 4 capas, donde las dependencias siempre apuntan hacia adentro:

```
SportZone.API            → Controllers, Swagger, middleware de errores
        ↓
SportZone.Infrastructure → EF Core, DbContext, Repositorios, migraciones
        ↓
SportZone.Application    → DTOs, interfaces, casos de uso (servicios)
        ↓
SportZone.Domain         → Entidades, reglas de negocio puras
```

- **Domain** no depende de ninguna otra capa.
- **Application** solo conoce a Domain (DTOs + interfaces que Infrastructure implementa).
- **Infrastructure** implementa los contratos de Application usando EF Core y SQL Server.
- **API** expone los casos de uso de Application vía HTTP.

## Estructura de carpetas

```
backend/
├── SportZone.sln
├── DER.md                          → esquema de base de datos (DBML)
└── src/
    ├── SportZone.Domain/
    │   ├── Common/BaseEntity.cs    → Id, auditoría (CreateById/UpdateById/DeleteById), Soft Delete
    │   ├── Entities/               → Rol, Usuario, Categoria, Marca, Articulo, Venta, etc.
    │   └── Interfaces/IActivable.cs
    ├── SportZone.Application/
    │   ├── DTOs/                   → uno por entidad (Xxx, CreateXxx, UpdateXxx)
    │   ├── DTOs/Common/             → PagedResultDto, PaginacionQueryDto
    │   ├── Interfaces/              → IRepository<T>, ICurrentUserService, repos específicos
    │   ├── Interfaces/Servicios/    → contratos de los casos de uso
    │   ├── Servicios/               → implementación de los casos de uso
    │   └── Common/Exceptions/       → NotFoundException, ValidationException
    ├── SportZone.Infrastructure/
    │   ├── Persistence/             → ApplicationDbContext + Fluent API (Configurations/)
    │   ├── Repositories/            → Repository<T> genérico + repos específicos
    │   ├── Services/                → CurrentUserService
    │   └── Migrations/
    └── SportZone.API/
        ├── Controllers/             → BaseController + un controller por entidad
        ├── Middleware/               → ExceptionHandlingMiddleware
        └── Program.cs
```

## Requisitos previos

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server (Express, Developer o LocalDB)
- Herramienta de migraciones: `dotnet tool install --global dotnet-ef --version 8.0.13`

## Puesta en marcha

1. Clona el repositorio y entra a la carpeta `backend`.
2. Ajusta la cadena de conexión en `src/SportZone.API/appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=.\\SQLEXPRESS;Database=SportZoneDB;Trusted_Connection=true;TrustServerCertificate=true"
   }
   ```
3. Aplica las migraciones para crear la base de datos:
   ```bash
   dotnet ef database update --project src/SportZone.Infrastructure --startup-project src/SportZone.API
   ```
4. Ejecuta la API:
   ```bash
   dotnet run --project src/SportZone.API
   ```
5. Abre Swagger en el navegador:
   ```
   http://localhost:<puerto>/swagger
   ```

## Comandos del proyecto

### Ejecutar la API

```bash
# Compilar toda la solución
dotnet build

# Ejecutar (toma el puerto de launchSettings.json)
dotnet run --project src/SportZone.API

# Ejecutar en un puerto fijo
dotnet run --project src/SportZone.API --urls "http://localhost:5080"

# Ejecutar con recarga automática al guardar cambios
dotnet watch run --project src/SportZone.API
```

Todos los comandos `dotnet ef` siguientes usan los mismos dos parámetros:
- `--project src/SportZone.Infrastructure` → ahí vive el `ApplicationDbContext` y las migraciones
- `--startup-project src/SportZone.API` → ahí está la cadena de conexión (`appsettings.json`)

### Migraciones

```bash
# Crear una nueva migración después de modificar una entidad o su Fluent API
dotnet ef migrations add NombreDeLaMigracion --project src/SportZone.Infrastructure --startup-project src/SportZone.API

# Listar todas las migraciones y cuáles ya se aplicaron a la base
dotnet ef migrations list --project src/SportZone.Infrastructure --startup-project src/SportZone.API

# Deshacer la última migración (solo si AÚN NO se aplicó con database update)
dotnet ef migrations remove --project src/SportZone.Infrastructure --startup-project src/SportZone.API

# Generar el script SQL de una migración sin ejecutarlo (útil para revisar antes de aplicar)
dotnet ef migrations script --project src/SportZone.Infrastructure --startup-project src/SportZone.API
```

### Actualizar la base de datos (`update`)

```bash
# Aplicar todas las migraciones pendientes (crea la BD si no existe)
dotnet ef database update --project src/SportZone.Infrastructure --startup-project src/SportZone.API

# Revertir la base hasta una migración específica (deshace las posteriores)
dotnet ef database update NombreDeLaMigracionAnterior --project src/SportZone.Infrastructure --startup-project src/SportZone.API

# Revertir TODAS las migraciones (deja la base sin tablas, pero la base sigue existiendo)
dotnet ef database update 0 --project src/SportZone.Infrastructure --startup-project src/SportZone.API
```

### Eliminar la base de datos (`drop`)

```bash
# Elimina la base de datos completa (pide confirmación)
dotnet ef database drop --project src/SportZone.Infrastructure --startup-project src/SportZone.API

# Sin confirmación interactiva (úsalo con cuidado)
dotnet ef database drop --force --project src/SportZone.Infrastructure --startup-project src/SportZone.API
```

> ⚠️ `database drop` borra TODOS los datos de `SportZoneDB` sin posibilidad de recuperación. Después de un drop, vuelve a ejecutar `dotnet ef database update` para recrear las tablas desde las migraciones existentes.

## Patrones implementados

| Patrón | Descripción |
|---|---|
| **Soft Delete** | Ninguna fila se borra físicamente. `DeletedAt` marca el registro como inactivo; un Global Query Filter lo excluye automáticamente de toda consulta. |
| **Repository genérico** | `IRepository<T>` cubre el CRUD básico de cualquier entidad. Entidades con relaciones (ej. `Usuario`) usan un repositorio específico (`IUsuarioRepository`) que añade consultas con `Include`. |
| **Auditoría automática** | `ICurrentUserService` resuelve quién es el usuario actual; el Repository asigna `CreateById`/`UpdateById`/`DeleteById` sin que el Servicio lo haga manualmente. Hoy devuelve un valor fijo; se conecta al usuario real cuando exista login con JWT. |
| **Paginación** | `PaginacionQueryDto` (Page/PageSize autodefendidos) + `PagedResultDto<T>` genérico. El cálculo de `Skip/Take/Count` vive en Infrastructure para no filtrar EF Core hacia Application. |
| **Respuestas estandarizadas** | Todo Controller hereda de `BaseController`, que envuelve las respuestas en `{ success, message, data }`. |
| **Manejo global de errores** | `ExceptionHandlingMiddleware` traduce excepciones de negocio (`NotFoundException` → 404, `ValidationException` → 400) y oculta cualquier otro error interno. |
| **Polimorfismo** | `IActivable` lo implementan las entidades que pueden activarse/desactivarse (ej. `Usuario`), permitiendo tratarlas de forma genérica sin conocer la clase concreta. |

## Módulos disponibles

| Módulo | Endpoints | Estado |
|---|---|---|
| Roles | `GET/POST/PUT/DELETE /api/Roles` (paginado) | ✅ |
| Usuarios | `GET/POST/PUT/DELETE /api/Usuarios` (paginado, relación con Rol) | ✅ |
| Categorías | `GET/POST/PUT/DELETE /api/Categorias` | ✅ |
| Marcas | `GET/POST/PUT/DELETE /api/Marcas` | ✅ |
| Artículos y variantes | — | ⏳ pendiente |
| Clientes / Proveedores | — | ⏳ pendiente |
| Ventas / Ingresos / Devoluciones | — | ⏳ pendiente |
| Autenticación (login + JWT) | — | ⏳ pendiente |

## Notas de seguridad pendientes

- El hash de contraseñas usa SHA256 sin sal como medida temporal — se debe reemplazar por **BCrypt** antes de cualquier uso real, al construir el módulo de autenticación.
- `ICurrentUserService` devuelve siempre el usuario Id `1`; debe conectarse a los claims del JWT una vez exista login.
