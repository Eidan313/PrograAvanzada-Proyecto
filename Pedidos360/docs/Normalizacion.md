# Normalizacion del modelo de datos - Pedidos360


Diagrama: Diagrama-EDR.drawio
Script: ./Database/01_Crear_Pedidos360.sql

## Tablas

- Categorias (PK Id, Nombre unico)
- Productos (PK Id, FK CategoriaId, Codigo unico)
- Clientes (PK Id, Cedula unica)
- Pedidos (PK Id, FK ClienteId, FK UsuarioId)
- PedidoDetalles (PK Id, FK PedidoId, FK ProductoId, unico PedidoId + ProductoId)
- Usuarios, Roles y UsuarioRoles (tablas de Identity)

## Dependencias funcionales

- Categorias: Id -> Nombre
- Productos: Id -> Codigo, Nombre, CategoriaId, Precio, ImpuestoPorc, Stock, Imagen, ImagenTipo, Activo
- Clientes: Id -> TipoIdentificacion, Cedula, Nombre, Correo, Telefono, Direccion
- Pedidos: Id -> ClienteId, UsuarioId, Fecha, Estado, Subtotal, Impuestos, Total
- PedidoDetalles: Id -> PedidoId, ProductoId, Cantidad, PrecioUnit, TipoDescuento, Descuento, ImpuestoPorc, TotalLinea

## Primera forma normal

Todas las tablas tienen llave primaria y cada campo guarda un solo valor.

- Los productos de un pedido no se guardan en columnas repetidas (Producto1, Producto2), cada uno es una fila en PedidoDetalles.
- Los roles de un usuario no se guardan separados por comas, cada uno es una fila en UsuarioRoles.
- Direccion se deja en un solo campo porque el sistema no consulta por provincia, canton o distrito.
- Cada cliente tiene un solo telefono y un solo correo, como pide el enunciado.

## Segunda forma normal

Las tablas usan llave primaria simple (Id), por lo que no hay dependencias parciales.

La unica llave candidata compuesta es PedidoId + ProductoId en PedidoDetalles. Ningun campo depende solo de una parte:

- Cantidad y Descuento cambian en cada pedido, no dependen solo del producto.
- PrecioUnit e ImpuestoPorc son los valores del momento de la venta. El precio del producto puede cambiar despues, por eso no dependen solo de ProductoId.

## Tercera forma normal

No hay campos que dependan de otro campo que no sea llave.

- Productos guarda CategoriaId y no el nombre de la categoria.
- Pedidos guarda ClienteId y UsuarioId y no los datos del cliente o del usuario.
- ImpuestoPorc esta en Productos porque cada producto tiene su propia tarifa (13% o 0%).
- ImagenTipo describe la imagen del producto, depende del producto.
- TipoIdentificacion, TipoDescuento y Estado son valores fijos sin atributos propios. Se guardan como entero con un CHECK en lugar de una tabla aparte.

PrecioUnit e ImpuestoPorc en PedidoDetalles no son copias de Productos. Productos guarda el precio actual y PedidoDetalles el precio con el que se vendio. Si se leyera desde Productos, un cambio de precio modificaria pedidos ya cobrados.

## Excepciones

### Totales del pedido

TotalLinea se calcula con Cantidad, PrecioUnit, Descuento e ImpuestoPorc de la misma fila. Total se calcula con Subtotal + Impuestos. Por esto Pedidos y PedidoDetalles quedan en 2FN.

Para evitar datos inconsistentes:

- CHECK (Total = Subtotal + Impuestos) en Pedidos.
- CHECK de rangos en los montos.
- El calculo se hace en un solo lugar del codigo.

Formulas:

    Bruto          = Cantidad * PrecioUnit
    DescuentoMonto = Bruto * Descuento / 100 (porcentaje) o Descuento (monto fijo)
    Base           = Bruto - DescuentoMonto
    ImpuestoLinea  = redondear(Base * ImpuestoPorc / 100, 2)
    TotalLinea     = Base + ImpuestoLinea

    Subtotal  = suma de Base
    Impuestos = suma de ImpuestoLinea
    Total     = Subtotal + Impuestos

### Columnas de Identity

NormalizedUserName, NormalizedEmail y NormalizedName son el nombre o correo en mayusculas, por lo que dependen de otra columna. Las crea ASP.NET Core Identity para buscar sin distinguir mayusculas y el mismo framework las mantiene actualizadas.

## Integridad

- Llaves foraneas con borrado restringido, excepto PedidoDetalles -> Pedidos (cascada).
- CHECK: Precio > 0, Stock >= 0, ImpuestoPorc entre 0 y 100, Cantidad > 0.
- Unicos: Productos.Codigo, Clientes.Cedula, Categorias.Nombre y PedidoId + ProductoId.

## Conclusion

Todas las tablas cumplen 1FN y 2FN. Categorias, Productos, Clientes y UsuarioRoles cumplen 3FN. Pedidos y PedidoDetalles quedan en 2FN solo por los totales que exige el enunciado. Usuarios y Roles se apartan de 3FN solo por las columnas que define Identity.
