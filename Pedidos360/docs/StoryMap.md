# Story map — Pedidos360

Aplicación web MVC para una pyme: catálogo, clientes, inventario y pedidos con cálculo de totales.

**Roles:** Admin (acceso total) · Ventas (crea y consulta pedidos, ve el catálogo, no elimina productos) · Operaciones (ajusta stock, ve pedidos).

## 1. Mapa general

La fila de arriba son las actividades (de izquierda a derecha, en el orden en que se usa el sistema). Cada columna baja en historias. El corte horizontal separa lo que entra en cada hito.

| | A. Acceso y seguridad | B. Catálogo | C. Clientes | D. Pedidos | E. Inventario | F. Calidad y errores |
|---|---|---|---|---|---|---|
| **Hito 1** (avance funcional) | | H-01 Gestionar categorías · H-02 Gestionar productos · H-03 Listar y filtrar productos · H-04 Cargar datos iniciales | H-05 Gestionar clientes · H-06 Buscar clientes | | | H-07 Validar formularios (cliente y servidor) · H-08 Mensajes de éxito y error |
| **Hito 2** (MVP completo) | H-09 Iniciar sesión · H-10 Restringir acciones por rol | | | H-11 Crear pedido · H-12 Buscar producto en tiempo real · H-13 Calcular totales en tiempo real · H-14 Confirmar pedido · H-15 Ver detalle de pedido | H-16 Ajustar stock | H-17 Páginas de error 404/500 |
| **Opcional** | | | | H-18 Exportar pedido a PDF/Excel | | |

## 2. Criterios de aceptación

Formato: **Dado** (contexto) · **Cuando** (acción) · **Entonces** (resultado).

### A. Acceso y seguridad

**H-09 — Como usuario del sistema quiero iniciar sesión para usar solo lo que me corresponde.** *(Hito 2)*
- Dado que tengo una cuenta, cuando ingreso mi correo y contraseña correctos, entonces entro al sistema y veo mi nombre en el menú.
- Dado que ingreso una contraseña incorrecta, cuando envío el formulario, entonces veo un mensaje de error y no entro.
- Dado que no he iniciado sesión, cuando intento abrir una página protegida, entonces el sistema me lleva al inicio de sesión.

**H-10 — Como administrador quiero que cada rol tenga permisos distintos para proteger la información.** *(Hito 2)*
- Dado que soy Ventas, cuando abro el listado de productos, entonces no veo el botón Eliminar.
- Dado que soy Ventas y escribo en el navegador la dirección para eliminar un producto, cuando la abro, entonces el sistema me niega el acceso.
- Dado que soy Operaciones, cuando abro un pedido, entonces puedo verlo pero no modificarlo.
- Dado que soy Admin, cuando navego por el sistema, entonces tengo acceso a todas las opciones.

### B. Catálogo

**H-01 — Como administrador quiero gestionar categorías para ordenar el catálogo.** *(Hito 1)*
- Dado que estoy en Nueva categoría, cuando guardo con el nombre vacío, entonces veo "El nombre es requerido." y no se crea.
- Dado que ya existe la categoría "Oficina", cuando creo otra llamada "oficina" (con cualquier combinación de mayúsculas), entonces veo "Ya existe una categoría con ese nombre." y no se crea.
- Dado que hay más de 10 categorías, cuando abro el listado, entonces veo 10 por página y los botones de paginación.
- Dado que una categoría tiene productos, cuando intento eliminarla, entonces vuelvo al listado, la categoría sigue ahí y veo un mensaje de error en la barra de alertas con el motivo.
- Dado que una categoría no tiene productos, cuando confirmo su eliminación, entonces desaparece del listado y veo un mensaje de éxito.

**H-02 — Como administrador quiero crear, editar y eliminar productos para mantener el catálogo al día.** *(Hito 1)*
- Dado que estoy creando un producto, cuando no selecciono una imagen, entonces el sistema me pide la imagen y no guarda.
- Dado que ingreso precio 0 o negativo, cuando guardo, entonces veo "El precio debe ser mayor a 0.".
- Dado que ingreso stock negativo, cuando guardo, entonces veo "El stock no puede ser negativo.".
- Dado que edito un producto, cuando no subo una imagen nueva, entonces se conserva la imagen actual.
- Dado que un producto ya tiene pedidos, cuando intento eliminarlo, entonces el sistema lo impide y explica el motivo.

**H-03 — Como vendedor quiero listar y filtrar productos para encontrar rápido lo que busco.** *(Hito 1)*
- Dado que hay productos en el catálogo, cuando abro el listado, entonces veo 10 por página con su imagen, precio y stock.
- Dado que escribo parte del nombre, cuando aplico el filtro, entonces solo veo los productos cuyo nombre lo contiene.
- Dado que elijo una categoría, cuando aplico el filtro, entonces solo veo productos de esa categoría.
- Dado que apliqué un filtro, cuando cambio de página, entonces el filtro se mantiene.

**H-04 — Como integrante del equipo quiero datos iniciales para probar y hacer la demo sin cargar nada a mano.** *(Hito 1)*
- Dado que la base de datos está vacía, cuando arranca la aplicación, entonces se crean 2 categorías, 12 productos con imagen y 8 clientes.
- Dado que las tablas ya tienen datos, cuando la aplicación arranca de nuevo, entonces no se inserta nada ni se duplica nada.
- Dado que una sola tabla está vacía (por ejemplo Clientes), cuando arranca la aplicación, entonces solo se llena esa tabla.
- Dado que se cargaron los productos, cuando abro el listado, entonces cada producto muestra su imagen.

### C. Clientes

**H-05 — Como vendedor quiero registrar y mantener clientes para asociarlos a pedidos.** *(Hito 1)*
- Dado que estoy en Nuevo cliente, cuando dejo un campo requerido vacío, entonces veo el mensaje junto al campo y no se guarda.
- Dado que ingreso un correo sin formato válido, cuando guardo, entonces veo "El correo no tiene un formato válido.".
- Dado que ya existe un cliente con esa cédula, cuando intento registrar otro con la misma, entonces veo "Ya existe un cliente con esta cédula." en lugar de un error.
- Dado que un cliente tiene pedidos, cuando intento eliminarlo, entonces el sistema lo impide y muestra el motivo.

**H-06 — Como vendedor quiero buscar clientes por nombre o cédula para encontrarlos rápido.** *(Hito 1)*
- Dado que escribo parte de un nombre, cuando busco, entonces veo los clientes cuyo nombre lo contiene.
- Dado que escribo parte de una cédula, cuando busco, entonces veo los clientes cuya cédula la contiene.
- Dado que nada coincide, cuando busco, entonces veo "No hay clientes para mostrar.".

### D. Pedidos

**H-11 — Como vendedor quiero crear un pedido para un cliente.** *(Hito 2)*
- Dado que estoy en Nuevo pedido, cuando elijo un cliente de la lista, entonces queda asociado al pedido.
- Dado que no elegí cliente, cuando intento confirmar, entonces el sistema me pide seleccionarlo.
- Dado que el pedido no tiene productos, cuando intento confirmar, entonces el sistema me pide agregar al menos uno.

**H-12 — Como vendedor quiero buscar productos mientras escribo para agregarlos al pedido sin recorrer todo el catálogo.** *(Hito 2)*
- Dado que escribo al menos 2 letras del nombre o el código, cuando pasa un instante, entonces veo hasta 10 coincidencias con nombre, precio y stock.
- Dado que elijo una coincidencia, cuando la selecciono, entonces se agrega una línea con cantidad 1 y su precio unitario.
- Dado que no hay coincidencias, cuando termino de escribir, entonces veo "Sin resultados".
- Dado que no he iniciado sesión, cuando se consulta la búsqueda, entonces el servicio responde "no autorizado".

**H-13 — Como vendedor quiero ver subtotal, impuestos y total al instante para informar al cliente.** *(Hito 2)*
- Dado que cambio la cantidad de una línea, cuando salgo del campo, entonces el subtotal, los impuestos y el total se actualizan sin recargar la página.
- Dado que aplico un descuento en porcentaje o en monto fijo, cuando cambio el valor, entonces el total refleja el descuento.
- Dado que el descuento es mayor al valor de la línea, cuando lo ingreso, entonces veo un error y no se aplica.
- Dado que quito una línea, cuando la elimino, entonces los totales se recalculan.

**H-14 — Como vendedor quiero confirmar el pedido para registrar la venta.** *(Hito 2)*
- Dado que el pedido es válido, cuando lo confirmo, entonces se guarda con subtotal, impuestos y total, el usuario y la fecha.
- Dado que confirmé el pedido, cuando reviso el producto, entonces su stock bajó según la cantidad vendida.
- Dado que una cantidad supera el stock disponible, cuando intento confirmar, entonces el sistema lo impide y no descuenta nada.

**H-15 — Como usuario quiero ver el detalle de un pedido para revisar lo vendido.** *(Hito 2)*
- Dado que existe un pedido, cuando abro su detalle, entonces veo cliente, usuario, fecha, líneas y totales guardados.
- Dado que los precios del producto cambiaron después, cuando abro el pedido, entonces las líneas conservan el precio con el que se vendió.

**H-18 — Como administrador quiero exportar un pedido a PDF o Excel para compartirlo.** *(Opcional)*
- Dado que abro el detalle de un pedido, cuando elijo exportar, entonces se descarga un archivo con los mismos datos que veo en pantalla.

### E. Inventario

**H-16 — Como operador quiero ajustar el stock para corregir diferencias del inventario.** *(Hito 2)*
- Dado que soy Operaciones, cuando registro un ajuste de stock con un valor válido, entonces el stock del producto se actualiza.
- Dado que el ajuste dejaría el stock negativo, cuando intento guardar, entonces el sistema lo rechaza.
- Dado que soy Ventas, cuando intento abrir el ajuste de stock, entonces el sistema me niega el acceso.

### F. Calidad y errores

**H-07 — Como usuario quiero que los formularios me avisen de los errores para corregirlos antes de guardar.** *(Hito 1)*
- Dado que dejo un campo requerido vacío, cuando salgo del campo o envío el formulario, entonces el mensaje aparece sin recargar la página.
- Dado que se desactiva JavaScript, cuando envío datos inválidos, entonces el servidor los rechaza y devuelve el formulario con los mensajes.

**H-08 — Como usuario quiero ver mensajes claros de éxito y error para saber qué pasó.** *(Hito 1)*
- Dado que guardo, edito o elimino un registro correctamente, cuando regreso al listado, entonces veo un mensaje verde de éxito.
- Dado que una acción no se puede realizar (por ejemplo eliminar algo que está en uso), cuando regreso al listado, entonces veo un mensaje rojo con el motivo.
- Dado que cierro el mensaje o recargo la página, entonces el mensaje desaparece.

**H-17 — Como usuario quiero páginas de error amigables para no ver errores técnicos.** *(Hito 2)*
- Dado que abro una dirección que no existe, cuando carga la página, entonces veo una página 404 personalizada con un enlace al inicio.
- Dado que ocurre un error inesperado en el servidor, cuando carga la página, entonces veo una página 500 personalizada sin detalles técnicos.
