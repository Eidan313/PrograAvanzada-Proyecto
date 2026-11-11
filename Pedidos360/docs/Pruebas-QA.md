# Pruebas funcionales mínimas y checklist de calidad — Hito 2

Responsable: QA (rama `feature/calidad-errores`).
Historias relacionadas: H-08 (mensajes), H-10 (roles), H-17 (404/500).

## Preparación

1. `dotnet ef database update` (o `Update-Database`) sobre una base limpia.
2. Ejecutar la app con `ASPNETCORE_ENVIRONMENT=Development` para pruebas de flujo.
3. Para probar la página 500 hay que usar `ASPNETCORE_ENVIRONMENT=Production` (en Development se muestra la página técnica de ASP.NET, no la personalizada).
4. Usuarios de prueba: admin / ventas / operaciones (Pendiente).

## Casos de prueba

| ID | Caso | Pasos | Resultado esperado | Historia |
|---|---|---|---|---|
| TC-01 | 404 en URL inexistente | Abrir `/esta-ruta-no-existe` | Página "Página no encontrada", código HTTP 404, enlace "Volver al inicio" funciona | H-17 |
| TC-02 | 404 en detalle inexistente | Abrir `/Productos/Details/99999` | Página 404 personalizada (no pantalla en blanco) | H-17 |
| TC-03 | 404 registrado en log | Repetir TC-01 y revisar consola | Línea `Warning` con "Recurso no encontrado" y referencia | Logging |
| TC-04 | 500 con página amigable | Production + provocar excepción | Página "Algo salió mal", código HTTP 500, sin texto técnico ni stacktrace | H-17 |
| TC-05 | 500 registrado en log | Repetir TC-04 y revisar consola | Línea `Error` con la excepción completa y la misma referencia que muestra la página | Logging |
| TC-06 | Referencia coherente | Comparar la referencia de la página de error con la del log | Coinciden | Logging |
| TC-07 | API sin sesión | `GET /api/productos/buscar?q=ab` sin login | HTTP 401 y **sin HTML** (la API no usa la página 404/500) | H-12 |
| TC-08 | Acceso denegado por rol | Ventas abre URL sin acceso | Acceso denegado (no 404) | H-10 |
| TC-09 | Mensaje de éxito | Crear una categoría válida | Alerta verde en `_Alertas` | H-08 |
| TC-10 | Mensaje de error | Eliminar categoría con productos | Alerta roja con el motivo | H-08 |
| TC-11 | Validación cliente | Dejar nombre de producto vacío y salir del campo | Mensaje junto al campo, sin recargar | H-07 |
| TC-12 | Validación servidor | Desactivar JavaScript y enviar datos inválidos | Servidor devuelve el formulario con mensajes | H-07 |
| TC-13 | Stock no negativo | Confirmar pedido con cantidad mayor al stock | Se bloquea, stock sin cambios | H-14 | -- Pendiente
| TC-14 | Totales en vivo | Cambiar cantidad de una línea | Subtotal, impuestos y total se actualizan sin recargar | H-13 |
| TC-15 | Flujo completo de pedido | Ventas: buscar producto → agregar → confirmar | Pedido guardado, stock bajó, detalle muestra totales | H-11 a H-15 |


## Checklist de calidad (Hito 2)

### Errores y logging
- [x] 404 muestra página personalizada con enlace al inicio (TC-01, TC-02)
- [x] 404 devuelve código HTTP 404, no 200 (TC-01)
- [x] 500 muestra página personalizada sin detalles técnicos (TC-04)
- [x] 500 devuelve código HTTP 500, no 200 (TC-04)
- [x] Toda excepción no controlada queda en el log con referencia (TC-05, TC-06)
- [ ] Ninguna pantalla muestra texto en inglés ni stacktrace en producción
- [x] La API no devuelve HTML en errores (TC-07)

### Seguridad y roles
- [x] Endpoints `/api/*` responden 401 sin sesión (TC-07)
- [ ] Ventas no puede eliminar productos (TC-08)
- [P] Operaciones no puede crear pedidos (PENDIENTE)

### Funcional
- [x] Mensajes de éxito y error aparecen en `_Alertas` (TC-09, TC-10)
- [ ] Mensaje no se duplica en el listado de productos (BUG-02)
- [x] Validaciones cliente y servidor funcionan (TC-11, TC-12)
- [ ] Flujo de pedido completo sin errores (TC-15)

### Build y despliegue
- [x] Compila sin errores ni advertencias nuevas
- [x] Corre después de `Update-Database` sobre base vacía
- [ ] Pasos de instalación probados por alguien que no programó el cambio

## Registro de resultados

## Registro de resultados

| Fecha | Tester | Caso | Estado | Observación |
|---|---|---|---|---|
| 11/11/2026 | (Mónica) | TC-01 | OK | 404 en URL inexistente muestra página personalizada. Falta confirmar código 404 en F12 → Red |
| 11/11/2026 | (Mónica) | TC-02 | OK | `/Productos/Details/99999` como Admin muestra página 404 personalizada |
| 11/11/2026 | (Mónica) | TC-03 | No ejecutado | Pendiente revisar línea `warn:` en consola tras un 404 |
| 11/11/2026 | (Mónica) | TC-04 | OK | Página "Algo salió mal" con código 500 (perfil https-qa) |
| 11/11/2026 | (Mónica) | TC-05 | OK | Excepción registrada en consola con la referencia |
| 11/11/2026 | (Mónica) | TC-06 | OK | Referencia idéntica en página y consola |
| 11/11/2026 | (Mónica) | TC-07 | OK | 401 sin HTML en /api/productos/buscar. Precondición: ventana de incógnito sin cookies |
| 11/11/2026 | (Mónica) | TC-08 | No ejecutado | Pendiente probar con usuario Vendedor en /Productos/Delete/1 |
| 11/11/2026 | (Mónica) | TC-09 | OK | Alerta verde "Categoría creada correctamente." |
| 11/11/2026 | (Mónica) | TC-10 | OK | Alerta roja con el motivo; la categoría sigue en el listado |
| 11/11/2026 | (Mónica) | TC-11 | OK | Validación de cliente en Nombre vacío. Requirió workaround local (BUG-01) |
| 11/11/2026 | (Mónica) | TC-12 | OK | Con JavaScript desactivado, el POST devuelve "El nombre es requerido." Producto no guardado |
| 11/11/2026 | (Mónica) | TC-13 | Pendiente | PedidoService vacío; no existe acción de confirmar pedido |
| 11/11/2026 | (Mónica) | TC-14 | Pendiente | Pantalla de nuevo pedido sin acción en el controlador |
| 11/11/2026 | (Mónica) | TC-15 | Pendiente | Depende de crear y confirmar pedido |
| 11/11/2026 | (Mónica) | TC-16 | Pendiente | Rol Operaciones no implementado |
| 11/11/2026 | (Mónica) | TC-17 | Pendiente | Rol Operaciones no implementado |

## Hallazgos

| ID | Descripción | Responsable | Estado |
|---|---|---|---|
| BUG-01 | El formulario de productos falla con configuración regional de separador decimal coma. Falta `ParseLimitsInInvariantCulture = true` en `ProductoFormViewModel` | Genesis (Productos) | Abierto. Workaround local: variable `DOTNET_SYSTEM_GLOBALIZATION_INVARIANT` |
| BUG-02 | Posible duplicado de alertas en el listado de productos (`_Alertas` está en el layout y en `Productos/Index`) | Genesis (Productos) | Por verificar |