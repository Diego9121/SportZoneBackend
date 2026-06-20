// =============================================
// SISTEMA POS - SPORT ZONE
// Esquema corregido - listo para dbdiagram.io
// =============================================

// NOTA BASE: Todos los campos de auditoría siguen este patrón:
//   create_by_id → ID del usuario que creó el registro
//   update_by_id → ID del usuario que modificó el registro
//   delete_by_id → ID del usuario que eliminó lógicamente el registro
//   created_at   → Fecha/hora de creación
//   updated_at   → Fecha/hora de última modificación
//   deleted_at   → Fecha de baja lógica; NULL = registro activo

Table roles {
  id          int          [pk, increment, note: 'Identificador único del rol']
  nombre      varchar(50)  [not null, note: 'Nivel de acceso: ADMIN (acceso total) | VENDEDOR (POS y consultas) | ALMACEN (ingreso de mercancía)']
  descripcion varchar(255) [null, note: 'Descripción detallada de los permisos asociados al rol']

  create_by_id int      [not null, note: 'ID del usuario que creó el registro']
  update_by_id int      [null,     note: 'ID del usuario que modificó el registro']
  delete_by_id int      [null,     note: 'ID del usuario que eliminó lógicamente el registro']
  created_at   datetime [not null, default: `getdate()`, note: 'Fecha y hora de creación del registro']
  updated_at   datetime [null,     note: 'Fecha y hora de la última modificación']
  deleted_at   datetime [null,     note: 'Fecha de baja lógica; NULL indica que el registro está activo']
}

Table usuarios {
  id            int          [pk, increment, note: 'Identificador único del usuario del sistema']
  rol_id        int          [not null, ref: > roles.id, note: 'Rol asignado que determina los permisos de acceso en todo el sistema']
  nombre        varchar(150) [not null, note: 'Nombre completo del usuario para identificación interna']
  email         varchar(100) [unique, not null, note: 'Correo electrónico único; usado como credencial de inicio de sesión']
  password_hash varchar(255) [not null, note: 'Contraseña cifrada con bcrypt; nunca se almacena en texto plano']
  token_refresh varchar(500) [null,     note: 'Token JWT de refresco para renovar sesión sin reautenticar; se invalida al cerrar sesión']
  activo        bit          [not null, default: 1, note: 'Estado del usuario: 1 = puede acceder al sistema, 0 = cuenta deshabilitada']
  ultimo_acceso datetime     [null,     note: 'Fecha y hora del último inicio de sesión exitoso; registrado en bitácora también']

  create_by_id int      [not null, note: 'ID del usuario que creó el registro']
  update_by_id int      [null,     note: 'ID del usuario que modificó el registro']
  delete_by_id int      [null,     note: 'ID del usuario que eliminó lógicamente el registro']
  created_at   datetime [not null, default: `getdate()`, note: 'Fecha y hora de creación del registro']
  updated_at   datetime [null,     note: 'Fecha y hora de la última modificación']
  deleted_at   datetime [null,     note: 'Fecha de baja lógica; NULL indica que el registro está activo']
}

Table marcas {
  id          int          [pk, increment, note: 'Identificador único de la marca deportiva']
  nombre      varchar(100) [not null, note: 'Nombre comercial de la marca (ej. Nike, Adidas, Puma, Under Armour, New Balance)']
  descripcion varchar(255) [null,     note: 'Información adicional sobre la marca o sus líneas de productos comercializados']
  logo        varchar(500) [null,     note: 'Ruta o URL del logo de la marca; usado para visualización en reportes y etiquetas']

  create_by_id int      [not null, note: 'ID del usuario que creó el registro']
  update_by_id int      [null,     note: 'ID del usuario que modificó el registro']
  delete_by_id int      [null,     note: 'ID del usuario que eliminó lógicamente el registro']
  created_at   datetime [not null, default: `getdate()`, note: 'Fecha y hora de creación del registro']
  updated_at   datetime [null,     note: 'Fecha y hora de la última modificación']
  deleted_at   datetime [null,     note: 'Fecha de baja lógica; NULL indica que el registro está activo']
}

Table categorias {
  id          int          [pk, increment, note: 'Identificador único de la categoría de producto']
  nombre      varchar(100) [not null, note: 'Nombre de la categoría (ej. Calzado Deportivo, Mochilas, Medias, Poleras, Shorts)']
  descripcion varchar(255) [null,     note: 'Descripción del tipo de artículos que agrupa esta categoría']

  create_by_id int      [not null, note: 'ID del usuario que creó el registro']
  update_by_id int      [null,     note: 'ID del usuario que modificó el registro']
  delete_by_id int      [null,     note: 'ID del usuario que eliminó lógicamente el registro']
  created_at   datetime [not null, default: `getdate()`, note: 'Fecha y hora de creación del registro']
  updated_at   datetime [null,     note: 'Fecha y hora de la última modificación']
  deleted_at   datetime [null,     note: 'Fecha de baja lógica; NULL indica que el registro está activo']
}

Table articulos {
  id            int            [pk, increment, note: 'Identificador único del modelo de producto (independiente de talla o color)']
  categoria_id  int            [not null, ref: > categorias.id, note: 'Categoría a la que pertenece el artículo; permite filtrar y agrupar en reportes']
  marca_id      int            [not null, ref: > marcas.id,     note: 'Marca del artículo; permite filtrar ventas e inventario por marca']
  codigo        varchar(50)    [unique, not null, note: 'Código SKU interno del modelo; identifica el producto sin importar talla ni color']
  nombre        varchar(150)   [not null, note: 'Nombre descriptivo del modelo (ej. Nike Air Max 270, Adidas Ultraboost 22)']
  descripcion   text           [null,     note: 'Descripción detallada del producto: características, materiales y uso recomendado']
  imagen        varchar(500)   [null,     note: 'Ruta o URL de la imagen principal del modelo; mostrada en el módulo de consulta móvil']
  
  create_by_id int      [not null, note: 'ID del usuario que creó el registro']
  update_by_id int      [null,     note: 'ID del usuario que modificó el registro']
  delete_by_id int      [null,     note: 'ID del usuario que eliminó lógicamente el registro']
  created_at   datetime [not null, default: `getdate()`, note: 'Fecha y hora de creación del registro']
  updated_at   datetime [null,     note: 'Fecha y hora de la última modificación']
  deleted_at   datetime [null,     note: 'Fecha de baja lógica; NULL indica que el registro está activo']
}

Table articulo_variantes {
  id                    int           [pk, increment, note: 'Identificador único de la variante; representa una combinación específica de talla y color']
  articulo_id           int           [not null, ref: > articulos.id, note: 'Artículo padre al que pertenece esta variante de talla/color']
  talla_us              varchar(10)   [null, note: 'Talla en sistema estadounidense US (ej. 7, 7.5, 8, 9, 10, 11); NULL para artículos sin variante de talla']
  talla_eu              varchar(10)   [null, note: 'Talla en sistema europeo EU (ej. 40, 40.5, 41, 42, 43); usada en el conversor de tallas']
  talla_uk              varchar(10)   [null, note: 'Talla en sistema británico UK (ej. 6, 6.5, 7, 7.5, 8); usada en el conversor de tallas']
  talla_cm              varchar(10)   [null, note: 'Longitud del pie en centímetros (ej. 25.5, 26.0); referencia directa para el conversor de tallas']
  color                 varchar(50)   [null, note: 'Color de esta variante (ej. Negro, Blanco, Rojo Coral); NULL para artículos sin variante de color']
  codigo_barras         varchar(100)  [unique, null, note: 'Código de barras único por variante; escaneado en el POS y en el módulo de consulta móvil']
  stock                 int           [not null, default: 0, note: 'Unidades disponibles de esta variante específica; se actualiza automáticamente con ventas, ingresos y devoluciones']
  stock_minimo          int           [not null, default: 5,  note: 'Umbral mínimo de stock; al alcanzar este valor el sistema emite una alerta de reposición']
  precio_venta  decimal(10,2)  [not null, note: 'Precio de venta base del artículo; puede ser sobreescrito por el precio de cada variante']
  precio_costo  decimal(10,2)  [not null, note: 'Precio de costo promedio del artículo; usado para calcular márgenes y valor total del inventario']

  indexes {
    (articulo_id, talla_us, color) [unique, note: 'Evita registrar dos veces la misma combinación de talla y color para un mismo artículo']
  }

  create_by_id int      [not null, note: 'ID del usuario que creó el registro']
  update_by_id int      [null,     note: 'ID del usuario que modificó el registro']
  delete_by_id int      [null,     note: 'ID del usuario que eliminó lógicamente el registro']
  created_at   datetime [not null, default: `getdate()`, note: 'Fecha y hora de creación del registro']
  updated_at   datetime [null,     note: 'Fecha y hora de la última modificación']
  deleted_at   datetime [null,     note: 'Fecha de baja lógica; NULL indica que el registro está activo']
}

Table clientes {
  id                     int           [pk, increment, note: 'Identificador único del cliente registrado']
  tipo_documento         varchar(20)   [null, note: 'Tipo de documento de identidad: CI | RUC | PASAPORTE; requerido para emisión de factura']
  documento              varchar(20)   [unique, null, note: 'Número del documento de identidad del cliente; único en el sistema']
  nombre                 varchar(150)  [not null, note: 'Nombre completo del cliente']
  telefono               varchar(20)   [null, note: 'Número de teléfono de contacto para comunicaciones y seguimiento']
  email                  varchar(100)  [null, note: 'Correo electrónico del cliente; usado para envío de comprobantes digitales']
  direccion              varchar(255)  [null, note: 'Dirección física del cliente; obligatoria cuando se emite factura']
 
  create_by_id int      [not null, note: 'ID del usuario que creó el registro']
  update_by_id int      [null,     note: 'ID del usuario que modificó el registro']
  delete_by_id int      [null,     note: 'ID del usuario que eliminó lógicamente el registro']
  created_at   datetime [not null, default: `getdate()`, note: 'Fecha y hora de creación del registro']
  updated_at   datetime [null,     note: 'Fecha y hora de la última modificación']
  deleted_at   datetime [null,     note: 'Fecha de baja lógica; NULL indica que el registro está activo']
}

Table proveedores {
  id                      int          [pk, increment, note: 'Identificador único del proveedor']
  nombre                  varchar(150) [not null, note: 'Nombre o razón social del proveedor']
  contacto                varchar(100) [null, note: 'Nombre de la persona de contacto dentro de la empresa proveedora']
  telefono                varchar(20)  [null, note: 'Teléfono principal para gestión de pedidos y coordinación de entregas']
  email                   varchar(100) [null, note: 'Correo electrónico del proveedor para envío de órdenes de compra']
  direccion               varchar(255) [null, note: 'Dirección física o comercial del proveedor para registros y auditoría']

  create_by_id int      [not null, note: 'ID del usuario que creó el registro']
  update_by_id int      [null,     note: 'ID del usuario que modificó el registro']
  delete_by_id int      [null,     note: 'ID del usuario que eliminó lógicamente el registro']
  created_at   datetime [not null, default: `getdate()`, note: 'Fecha y hora de creación del registro']
  updated_at   datetime [null,     note: 'Fecha y hora de la última modificación']
  deleted_at   datetime [null,     note: 'Fecha de baja lógica; NULL indica que el registro está activo']
}

Table ingresos {
  id           int           [pk, increment, note: 'Identificador único del ingreso o recepción de mercancía']
  proveedor_id int           [not null, ref: > proveedores.id, note: 'Proveedor que envió el lote de mercancía recibido']
  numero_doc   varchar(50)   [null, note: 'Número de factura o remisión del proveedor; permite rastrear el lote hasta su origen']
  total        decimal(10,2) [not null, note: 'Valor total del ingreso; debe coincidir con la suma de subtotales del detalle']
  observacion  text          [null, note: 'Notas sobre el ingreso: discrepancias en cantidades, estado del lote, condiciones de entrega']

  create_by_id int      [not null, note: 'ID del usuario que creó el registro']
  update_by_id int      [null,     note: 'ID del usuario que modificó el registro']
  delete_by_id int      [null,     note: 'ID del usuario que eliminó lógicamente el registro']
  created_at   datetime [not null, default: `getdate()`, note: 'Fecha y hora en que se registró el ingreso en el sistema']
  updated_at   datetime [null,     note: 'Fecha y hora de la última modificación']
  deleted_at   datetime [null,     note: 'Fecha de baja lógica; NULL indica que el registro está activo']
}

Table ingreso_detalle {
  id           int           [pk, increment, note: 'Identificador único del ítem de detalle del ingreso']
  ingreso_id   int           [not null, ref: > ingresos.id,           note: 'Ingreso al que pertenece este ítem de detalle']
  variante_id  int           [not null, ref: > articulo_variantes.id, note: 'Variante específica recibida (modelo + talla + color); al confirmar el ingreso incrementa el stock de esa variante']
  cantidad     int           [not null, note: 'Número de unidades recibidas de esta variante en el lote']
  precio_costo decimal(10,2) [not null, note: 'Precio de costo unitario negociado con el proveedor para este lote específico']
  subtotal     decimal(10,2) [not null, note: 'Valor total de este ítem: cantidad × precio_costo']

  create_by_id int      [not null, note: 'ID del usuario que creó el registro']
  update_by_id int      [null,     note: 'ID del usuario que modificó el registro']
  delete_by_id int      [null,     note: 'ID del usuario que eliminó lógicamente el registro']
  created_at   datetime [not null, default: `getdate()`, note: 'Fecha y hora de creación del registro']
  updated_at   datetime [null,     note: 'Fecha y hora de la última modificación']
  deleted_at   datetime [null,     note: 'Fecha de baja lógica; NULL indica que el registro está activo']
}

Table ventas {
  id               int           [pk, increment, note: 'Identificador único de la venta']
  cliente_id       int           [null, ref: > clientes.id, note: 'Cliente asociado a la venta; NULL para ventas al público general sin registro previo']
  numero_doc       varchar(50)   [unique, not null, note: 'Número único del comprobante emitido; generado automáticamente con prefijo REC- o FAC-']
  tipo_comprobante varchar(20)   [not null, default: 'RECIBO', note: 'Tipo de documento emitido: RECIBO (venta general) | FACTURA (requiere datos fiscales del cliente)']
  subtotal         decimal(10,2) [not null, note: 'Suma de subtotales de todos los ítems del detalle antes de aplicar el descuento global']
  descuento        decimal(10,2) [not null, default: 0.00, note: 'Descuento global aplicado sobre el subtotal (por promoción, cupón o fidelización del cliente)']
  total            decimal(10,2) [not null, note: 'Monto final cobrado al cliente: subtotal - descuento']
  estado           varchar(20)   [not null, default: 'PENDIENTE', note: 'Estado de la venta: PENDIENTE (sin pago registrado) | PAGADA (pago completado) | ANULADA (cancelada)']
  observacion      text          [null, note: 'Notas adicionales sobre la venta o condiciones especiales acordadas con el cliente']

  create_by_id int      [not null, note: 'ID del usuario que creó el registro']
  update_by_id int      [null,     note: 'ID del usuario que modificó el registro']
  delete_by_id int      [null,     note: 'ID del usuario que anuló o eliminó lógicamente el registro']
  created_at   datetime [not null, default: `getdate()`, note: 'Fecha y hora de la venta; base para reportes diarios, semanales y mensuales']
  updated_at   datetime [null,     note: 'Fecha y hora de la última modificación']
  deleted_at   datetime [null,     note: 'Fecha de baja lógica; NULL indica que el registro está activo']
}

Table venta_detalle {
  id              int           [pk, increment, note: 'Identificador único del ítem de detalle de la venta']
  venta_id        int           [not null, ref: > ventas.id,               note: 'Venta a la que pertenece este ítem']
  variante_id     int           [not null, ref: > articulo_variantes.id,   note: 'Variante específica vendida (modelo + talla + color); al confirmar la venta decrementa el stock de esa variante']
  cantidad        int           [not null, note: 'Número de unidades vendidas de esta variante en la transacción']
  precio_unitario decimal(10,2) [not null, note: 'Precio de venta unitario aplicado; puede diferir del precio base si se aplicó una promoción por artículo']
  descuento       decimal(10,2) [not null, default: 0.00, note: 'Descuento aplicado a este ítem específico por promoción de producto, categoría o marca']
  subtotal        decimal(10,2) [not null, note: 'Valor neto de este ítem: (cantidad × precio_unitario) - descuento']

  create_by_id int      [not null, note: 'ID del usuario que creó el registro']
  update_by_id int      [null,     note: 'ID del usuario que modificó el registro']
  delete_by_id int      [null,     note: 'ID del usuario que eliminó lógicamente el registro']
  created_at   datetime [not null, default: `getdate()`, note: 'Fecha y hora de creación del registro']
  updated_at   datetime [null,     note: 'Fecha y hora de la última modificación']
  deleted_at   datetime [null,     note: 'Fecha de baja lógica; NULL indica que el registro está activo']
}

Table MovimientoStock {
  id               int          [pk, increment, note: 'Identificador único del evento registrado en movimiento de stock']
  articulo_variante_id  int     [not null, ref: > articulo_variantes.id, note: 'enlace al articulo variante']
  ingreso_id       int          [null, ref: > ingresos.id, note: 'enlace a ingresos']
  venta_id       int          [null, ref: > ventas.id, note: 'enlace a ventas']
  tipo_movimiento           varchar(100) [not null, note: 'tipo ENUM: 1=entrada, 2=salida']
  cantidad         int           [not null, note: 'registra entrada o salida del articulo variante']
  numero_doc       varchar(50)   [null, note: 'Número único del comprobante emitido; con prefijo REC- o FAC-']

  create_by_id int      [not null, note: 'ID del usuario que creó el registro']
  created_at       datetime     [not null, default: `getdate()`, note: 'Fecha y hora exacta en que ocurrió el evento; inmutable, no se actualiza ni elimina']
}

