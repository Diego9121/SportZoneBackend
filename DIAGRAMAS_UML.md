# Diagramas UML — Sistema SportZone

Este documento complementa a `CASOS_DE_USO.md` con la representación gráfica del comportamiento y la estructura del sistema: diagramas de secuencia de los flujos más representativos, un diagrama de actividades del proceso central de venta, y el diagrama de clases del modelo de dominio.

---

## 1. Diagramas de Secuencia

Se representan los cuatro flujos más representativos del sistema: autenticación, el proceso de venta, el proceso de compra y la interacción con el servicio externo de imágenes.

### 1.1. CU-01: Iniciar Sesión

```mermaid
sequenceDiagram
    actor Usuario
    participant API as AuthController
    participant Servicio as AuthServicio
    participant Repo as UsuarioRepository
    participant BD as Base de Datos

    Usuario->>API: POST /api/Auth/login (email, password)
    API->>Servicio: LoginAsync(credenciales)
    Servicio->>Repo: Buscar usuario por email (con su Rol)
    Repo->>BD: Consultar Usuario
    BD-->>Repo: Usuario encontrado
    Repo-->>Servicio: Usuario
    Servicio->>Servicio: Verificar contraseña

    alt Credenciales válidas
        Servicio->>Servicio: Generar token de sesión
        Servicio->>Repo: Actualizar último acceso
        Servicio-->>API: Token + datos del usuario
        API-->>Usuario: 200 OK
    else Credenciales inválidas
        Servicio-->>API: Error de validación
        API-->>Usuario: 400 Credenciales inválidas
    end
```

### 1.2. CU-12: Registrar Venta

```mermaid
sequenceDiagram
    actor Vendedor
    participant API as VentasController
    participant Servicio as VentaServicio
    participant RepoVariante as ArticuloVarianteRepository
    participant RepoVenta as VentaRepository
    participant BD as Base de Datos

    Vendedor->>API: POST /api/Ventas (cliente, detalle de productos)
    API->>Servicio: CreateAsync(venta)

    loop Por cada producto del detalle
        Servicio->>RepoVariante: Obtener variante (precio y stock actual)
        RepoVariante->>BD: Consultar Variante
        BD-->>RepoVariante: Variante
        RepoVariante-->>Servicio: Variante
        Servicio->>Servicio: Validar stock y calcular subtotal
    end

    Servicio->>Servicio: Calcular totales de la venta
    Servicio->>RepoVenta: Registrar venta con su detalle
    RepoVenta->>BD: Descontar stock y registrar movimiento de salida
    BD-->>RepoVenta: Confirmación
    RepoVenta-->>Servicio: Venta registrada
    Servicio-->>API: Datos de la venta
    API-->>Vendedor: 201 Venta registrada
```

### 1.3. CU-11: Registrar Ingreso de Mercancía

```mermaid
sequenceDiagram
    actor Almacen as Encargado de Almacén
    participant API as IngresosController
    participant Servicio as IngresoServicio
    participant RepoIngreso as IngresoRepository
    participant BD as Base de Datos

    Almacen->>API: POST /api/Ingresos (proveedor, detalle de compra)
    API->>Servicio: CreateAsync(ingreso)
    Servicio->>Servicio: Validar proveedor y variantes
    Servicio->>Servicio: Calcular subtotales y total
    Servicio->>RepoIngreso: Registrar ingreso con su detalle
    RepoIngreso->>BD: Incrementar stock, actualizar costo y registrar movimiento de entrada
    BD-->>RepoIngreso: Confirmación
    RepoIngreso-->>Servicio: Ingreso registrado
    Servicio-->>API: Datos del ingreso
    API-->>Almacen: 201 Ingreso registrado
```

### 1.4. CU-08: Subir Imagen a la Nube

```mermaid
sequenceDiagram
    actor Admin as Administrador
    participant API as ImagenesController
    participant Servicio as IImagenService
    participant Cloud as Cloudinary

    Admin->>API: POST /api/Imagenes (archivo)
    API->>API: Validar tipo y tamaño del archivo
    API->>Servicio: Subir archivo
    Servicio->>Cloud: Enviar imagen
    Cloud-->>Servicio: URL pública de la imagen
    Servicio-->>API: URL
    API-->>Admin: 200 OK { url }
```

---

## 2. Diagrama de Actividades

### 2.1. Proceso de Venta

Describe, a alto nivel, el flujo completo de una venta en el punto de venta, desde la búsqueda del producto hasta el registro final.

```mermaid
flowchart TD
    Inicio([Inicio]) --> Buscar[Buscar producto por código de barras o catálogo]
    Buscar --> Agregar[Agregar producto y cantidad al detalle de venta]
    Agregar --> Otro{¿Agregar otro producto?}
    Otro -- Sí --> Buscar
    Otro -- No --> Confirmar[Confirmar venta]
    Confirmar --> StockOk{¿Stock suficiente en todas las líneas?}
    StockOk -- No --> Rechazar[Rechazar venta e informar producto sin stock]
    Rechazar --> Fin1([Fin])
    StockOk -- Sí --> Calcular[Calcular precios, subtotales y total]
    Calcular --> Descontar[Descontar stock y registrar movimiento de salida]
    Descontar --> Registrar[Registrar venta como pagada]
    Registrar --> Fin2([Fin])
```

---

## 3. Diagrama de Clases

### 3.1. Modelo de Dominio

Representa las entidades principales del sistema y sus relaciones. Todas las entidades heredan de `BaseEntity`, que centraliza el identificador y la auditoría (creación, modificación y eliminación lógica).

```mermaid
classDiagram
    class BaseEntity {
        -int Id
        -DateTime CreatedAt
        -DateTime? UpdatedAt
        -DateTime? DeletedAt
        -int CreateById
        -int? UpdateById
        -int? DeleteById
    }

    class Rol {
        -string Nombre
        -string? Descripcion
        +consultar() void
        +registrar() void
        +modificar() void
    }
    class Usuario {
        -int RolId
        -string Nombre
        -string Email
        -string PasswordHash
        -bool Activo
        -DateTime? UltimoAcceso
        +consultar() void
        +registrar() void
        +modificar() void
        +iniciarSesion() void
    }
    class Categoria {
        -string Nombre
        -string? Descripcion
        +consultar() void
        +registrar() void
        +modificar() void
    }
    class Marca {
        -string Nombre
        -string? Logo
        +consultar() void
        +registrar() void
        +modificar() void
    }
    class Articulo {
        -int CategoriaId
        -int MarcaId
        -string Codigo
        -string Nombre
        -string? Descripcion
        -string? Imagen
        +consultar() void
        +registrar() void
        +modificar() void
    }
    class ArticuloVariante {
        -int ArticuloId
        -string? TallaUs
        -string? TallaEu
        -string? TallaUk
        -string? TallaCm
        -string? Color
        -string? CodigoBarras
        -string? ImagenUrl
        -int Stock
        -int StockMinimo
        -decimal PrecioVenta
        -decimal PrecioCosto
        +consultar() void
        +registrar() void
        +modificar() void
        +ajustarStock() void
    }
    class Proveedor {
        -string Nombre
        -string? Contacto
        -string? Telefono
        -string? Email
        -string? Direccion
        +consultar() void
        +registrar() void
        +modificar() void
    }
    class Ingreso {
        -int ProveedorId
        -string? NumeroDoc
        -decimal Total
        -string? Observacion
        +consultar() void
        +registrar() void
    }
    class IngresoDetalle {
        -int IngresoId
        -int VarianteId
        -int Cantidad
        -decimal PrecioCosto
        -decimal Subtotal
    }
    class Cliente {
        -string? TipoDocumento
        -string? Documento
        -string Nombre
        -string? Telefono
        -string? Email
        -string? Direccion
        +consultar() void
        +registrar() void
        +modificar() void
    }
    class Venta {
        -int? ClienteId
        -string NumeroDoc
        -string TipoComprobante
        -decimal Subtotal
        -decimal Descuento
        -decimal Total
        -string Estado
        -string? Observacion
        +consultar() void
        +registrar() void
        +anular() void
    }
    class VentaDetalle {
        -int VentaId
        -int VarianteId
        -int Cantidad
        -decimal PrecioUnitario
        -decimal PrecioCosto
        -decimal Descuento
        -decimal Subtotal
    }
    class MovimientoStock {
        -int ArticuloVarianteId
        -int? IngresoId
        -int? VentaId
        -string TipoMovimiento
        -int Cantidad
        -string? NumeroDoc
        +consultar() void
    }

    BaseEntity <|-- Rol
    BaseEntity <|-- Usuario
    BaseEntity <|-- Categoria
    BaseEntity <|-- Marca
    BaseEntity <|-- Articulo
    BaseEntity <|-- ArticuloVariante
    BaseEntity <|-- Proveedor
    BaseEntity <|-- Ingreso
    BaseEntity <|-- IngresoDetalle
    BaseEntity <|-- Cliente
    BaseEntity <|-- Venta
    BaseEntity <|-- VentaDetalle
    BaseEntity <|-- MovimientoStock

    Rol "1" --> "0..*" Usuario
    Categoria "1" --> "0..*" Articulo
    Marca "1" --> "0..*" Articulo
    Articulo "1" --> "0..*" ArticuloVariante
    Proveedor "1" --> "0..*" Ingreso
    Ingreso "1" --> "1..*" IngresoDetalle
    ArticuloVariante "1" --> "0..*" IngresoDetalle
    Cliente "0..1" --> "0..*" Venta
    Venta "1" --> "1..*" VentaDetalle
    ArticuloVariante "1" --> "0..*" VentaDetalle
    ArticuloVariante "1" --> "0..*" MovimientoStock
    Ingreso "0..1" --> "0..*" MovimientoStock
    Venta "0..1" --> "0..*" MovimientoStock
```

### 3.2. Diagrama Entidad-Relación (Base de Datos)

Representa el esquema físico tal como queda en SQL Server: una tabla por entidad (sin jerarquía de herencia), con sus columnas de auditoría propias en cada una (`CreatedAt`, `UpdatedAt`, `DeletedAt`, `CreateById`, `UpdateById`, `DeleteById`), tipos de dato y largo exactos según la configuración Fluent API, y las claves foráneas entre tablas.

```mermaid
erDiagram
    Roles ||--o{ Usuarios : "tiene"
    Categorias ||--o{ Articulos : "clasifica"
    Marcas ||--o{ Articulos : "fabrica"
    Articulos ||--o{ ArticuloVariantes : "tiene"
    Proveedores ||--o{ Ingresos : "suministra"
    Ingresos ||--o{ IngresoDetalles : "contiene"
    ArticuloVariantes ||--o{ IngresoDetalles : "compra"
    Clientes o|--o{ Ventas : "realiza"
    Ventas ||--o{ VentaDetalles : "contiene"
    ArticuloVariantes ||--o{ VentaDetalles : "vende"
    ArticuloVariantes ||--o{ MovimientosStock : "registra"
    Ingresos o|--o{ MovimientosStock : "genera"
    Ventas o|--o{ MovimientosStock : "genera"

    Roles {
        int Id PK
        nvarchar Nombre "50, requerido"
        nvarchar Descripcion "255, opcional"
        datetime2 CreatedAt
        datetime2 UpdatedAt "opcional"
        datetime2 DeletedAt "opcional"
        int CreateById
        int UpdateById "opcional"
        int DeleteById "opcional"
    }

    Usuarios {
        int Id PK
        int RolId FK
        nvarchar Nombre "150, requerido"
        nvarchar Email UK "100, requerido"
        nvarchar PasswordHash "255, requerido"
        nvarchar TokenRefresh "500, opcional"
        bit Activo
        datetime2 UltimoAcceso "opcional"
        datetime2 CreatedAt
        datetime2 UpdatedAt "opcional"
        datetime2 DeletedAt "opcional"
        int CreateById
        int UpdateById "opcional"
        int DeleteById "opcional"
    }

    Marcas {
        int Id PK
        nvarchar Nombre "100, requerido"
        nvarchar Descripcion "255, opcional"
        nvarchar Logo "500, opcional"
        datetime2 CreatedAt
        datetime2 UpdatedAt "opcional"
        datetime2 DeletedAt "opcional"
        int CreateById
        int UpdateById "opcional"
        int DeleteById "opcional"
    }

    Categorias {
        int Id PK
        nvarchar Nombre "100, requerido"
        nvarchar Descripcion "255, opcional"
        datetime2 CreatedAt
        datetime2 UpdatedAt "opcional"
        datetime2 DeletedAt "opcional"
        int CreateById
        int UpdateById "opcional"
        int DeleteById "opcional"
    }

    Articulos {
        int Id PK
        int CategoriaId FK
        int MarcaId FK
        nvarchar Codigo UK "50, requerido"
        nvarchar Nombre "150, requerido"
        nvarchar Descripcion "max, opcional"
        nvarchar Imagen "500, opcional"
        datetime2 CreatedAt
        datetime2 UpdatedAt "opcional"
        datetime2 DeletedAt "opcional"
        int CreateById
        int UpdateById "opcional"
        int DeleteById "opcional"
    }

    ArticuloVariantes {
        int Id PK
        int ArticuloId FK
        nvarchar TallaUs UK "10, opcional, combinada con ArticuloId+Color"
        nvarchar TallaEu "10, opcional"
        nvarchar TallaUk "10, opcional"
        nvarchar TallaCm "10, opcional"
        nvarchar Color UK "50, opcional, combinada con ArticuloId+TallaUs"
        nvarchar CodigoBarras UK "100, opcional"
        nvarchar ImagenUrl "500, opcional"
        int Stock
        int StockMinimo
        decimal PrecioVenta "10,2"
        decimal PrecioCosto "10,2"
        datetime2 CreatedAt
        datetime2 UpdatedAt "opcional"
        datetime2 DeletedAt "opcional"
        int CreateById
        int UpdateById "opcional"
        int DeleteById "opcional"
    }

    Proveedores {
        int Id PK
        nvarchar Nombre "150, requerido"
        nvarchar Contacto "100, opcional"
        nvarchar Telefono "20, opcional"
        nvarchar Email "100, opcional"
        nvarchar Direccion "255, opcional"
        datetime2 CreatedAt
        datetime2 UpdatedAt "opcional"
        datetime2 DeletedAt "opcional"
        int CreateById
        int UpdateById "opcional"
        int DeleteById "opcional"
    }

    Ingresos {
        int Id PK
        int ProveedorId FK
        nvarchar NumeroDoc "50, opcional"
        decimal Total "10,2"
        nvarchar Observacion "max, opcional"
        datetime2 CreatedAt
        datetime2 UpdatedAt "opcional"
        datetime2 DeletedAt "opcional"
        int CreateById
        int UpdateById "opcional"
        int DeleteById "opcional"
    }

    IngresoDetalles {
        int Id PK
        int IngresoId FK
        int VarianteId FK
        int Cantidad
        decimal PrecioCosto "10,2"
        decimal Subtotal "10,2"
        datetime2 CreatedAt
        datetime2 UpdatedAt "opcional"
        datetime2 DeletedAt "opcional"
        int CreateById
        int UpdateById "opcional"
        int DeleteById "opcional"
    }

    Clientes {
        int Id PK
        nvarchar TipoDocumento "20, opcional"
        nvarchar Documento UK "20, opcional"
        nvarchar Nombre "150, requerido"
        nvarchar Telefono "20, opcional"
        nvarchar Email "100, opcional"
        nvarchar Direccion "255, opcional"
        datetime2 CreatedAt
        datetime2 UpdatedAt "opcional"
        datetime2 DeletedAt "opcional"
        int CreateById
        int UpdateById "opcional"
        int DeleteById "opcional"
    }

    Ventas {
        int Id PK
        int ClienteId FK "opcional"
        nvarchar NumeroDoc UK "50, requerido"
        nvarchar TipoComprobante "20, requerido"
        decimal Subtotal "10,2"
        decimal Descuento "10,2"
        decimal Total "10,2"
        nvarchar Estado "20, requerido"
        nvarchar Observacion "max, opcional"
        datetime2 CreatedAt
        datetime2 UpdatedAt "opcional"
        datetime2 DeletedAt "opcional"
        int CreateById
        int UpdateById "opcional"
        int DeleteById "opcional"
    }

    VentaDetalles {
        int Id PK
        int VentaId FK
        int VarianteId FK
        int Cantidad
        decimal PrecioUnitario "10,2"
        decimal PrecioCosto "10,2, default 0"
        decimal Descuento "10,2"
        decimal Subtotal "10,2"
        datetime2 CreatedAt
        datetime2 UpdatedAt "opcional"
        datetime2 DeletedAt "opcional"
        int CreateById
        int UpdateById "opcional"
        int DeleteById "opcional"
    }

    MovimientosStock {
        int Id PK
        int ArticuloVarianteId FK
        int IngresoId FK "opcional"
        int VentaId FK "opcional"
        nvarchar TipoMovimiento "100, requerido"
        int Cantidad
        nvarchar NumeroDoc "50, opcional"
        datetime2 CreatedAt
        datetime2 UpdatedAt "opcional"
        datetime2 DeletedAt "opcional"
        int CreateById
        int UpdateById "opcional"
        int DeleteById "opcional"
    }
```

Notas sobre el esquema:
- `Id`, `CreatedAt`, `UpdatedAt`, `DeletedAt`, `CreateById`, `UpdateById`, `DeleteById` se repiten en las 13 tablas porque vienen de `BaseEntity`; a nivel de base de datos no hay tabla de herencia, cada tabla las tiene como columnas propias.
- El borrado es lógico: `DELETE` nunca se ejecuta de verdad, se actualiza `DeletedAt` (por eso todas las consultas tienen un filtro global `WHERE DeletedAt IS NULL`).
- `ArticuloVariantes` tiene un índice único compuesto por `(ArticuloId, TallaUs, Color)`: no puede repetirse la misma talla+color para el mismo artículo.
- `CreateById`/`UpdateById`/`DeleteById` son columnas `int` simples, **sin FK real hacia `Usuarios`** (son metadato de auditoría, no una relación obligatoria).
