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
        +int Id
        +DateTime CreatedAt
        +DateTime? UpdatedAt
        +DateTime? DeletedAt
    }

    class Rol {
        +string Nombre
        +string? Descripcion
    }
    class Usuario {
        +int RolId
        +string Nombre
        +string Email
        +string PasswordHash
        +bool Activo
        +DateTime? UltimoAcceso
    }
    class Categoria {
        +string Nombre
        +string? Descripcion
    }
    class Marca {
        +string Nombre
        +string? Logo
    }
    class Articulo {
        +int CategoriaId
        +int MarcaId
        +string Codigo
        +string Nombre
        +string? Imagen
    }
    class ArticuloVariante {
        +int ArticuloId
        +string? TallaUs
        +string? Color
        +string? CodigoBarras
        +string? ImagenUrl
        +int Stock
        +int StockMinimo
        +decimal PrecioVenta
        +decimal PrecioCosto
    }
    class Proveedor {
        +string Nombre
        +string? Telefono
    }
    class Ingreso {
        +int ProveedorId
        +string? NumeroDoc
        +decimal Total
    }
    class IngresoDetalle {
        +int IngresoId
        +int VarianteId
        +int Cantidad
        +decimal PrecioCosto
        +decimal Subtotal
    }
    class Cliente {
        +string Nombre
        +string? Documento
    }
    class Venta {
        +int? ClienteId
        +string NumeroDoc
        +string Estado
        +decimal Total
    }
    class VentaDetalle {
        +int VentaId
        +int VarianteId
        +int Cantidad
        +decimal PrecioUnitario
        +decimal PrecioCosto
        +decimal Subtotal
    }
    class MovimientoStock {
        +int ArticuloVarianteId
        +int? IngresoId
        +int? VentaId
        +string TipoMovimiento
        +int Cantidad
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

    Rol "1" --> "*" Usuario
    Categoria "1" --> "*" Articulo
    Marca "1" --> "*" Articulo
    Articulo "1" --> "*" ArticuloVariante
    Proveedor "1" --> "*" Ingreso
    Ingreso "1" --> "*" IngresoDetalle
    IngresoDetalle "*" --> "1" ArticuloVariante
    Cliente "0..1" --> "*" Venta
    Venta "1" --> "*" VentaDetalle
    VentaDetalle "*" --> "1" ArticuloVariante
    ArticuloVariante "1" --> "*" MovimientoStock
    Ingreso "0..1" --> "*" MovimientoStock
    Venta "0..1" --> "*" MovimientoStock
```
