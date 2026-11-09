// Página de demostración: autocompletado + validación de stock en vivo + POST con antiforgery.
$(function () {
    'use strict';

    let productoActual = null;
    const lineas = [];
    let temporizadorStock = null;

    function formatearMonto(valor) {
        return Number(valor).toLocaleString('es-CR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    }

    function escapar(texto) {
        return $('<div>').text(texto == null ? '' : texto).html();
    }

    function mostrarMensajeStock(texto, esValido) {
        $('#mensajeStock')
            .removeClass('text-success text-danger')
            .addClass(esValido ? 'text-success' : 'text-danger')
            .text(texto);
        $('#agregarLinea').prop('disabled', !esValido);
    }

    function cantidadYaAgregada(productoId) {
        return lineas
            .filter(function (linea) { return linea.productoId === productoId; })
            .reduce(function (suma, linea) { return suma + linea.cantidad; }, 0);
    }

    function validarCantidad() {
        if (!productoActual) {
            return;
        }
        const cantidad = parseInt($('#cantidad').val(), 10);
        if (isNaN(cantidad) || cantidad <= 0) {
            mostrarMensajeStock('La cantidad debe ser mayor a 0.', false);
            return;
        }

        const total = cantidad + cantidadYaAgregada(productoActual.id);
        $.validarStockProducto(productoActual.id, total)
            .done(function (respuesta) {
                mostrarMensajeStock(respuesta.mensaje, respuesta.disponible);
            })
            .fail(function () {
                mostrarMensajeStock('No se pudo validar el stock.', false);
            });
    }

    $('#buscarProducto').autocompletarProducto({
        alSeleccionar: function (producto) {
            productoActual = producto;
            $('#imagenProducto').attr('src', producto.imagenUrl);
            $('#nombreProducto').text(producto.nombre);
            $('#detalleProducto').text(producto.codigo + ' · ₡' + formatearMonto(producto.precio) +
                ' · IVA ' + producto.impuestoPorc + '% · Stock ' + producto.stock);
            $('#cantidad').val(1);
            $('#productoSeleccionado').removeClass('d-none');
            validarCantidad();
        }
    });

    // Recalcula mientras el usuario escribe la cantidad (con una pequeña espera).
    $('#cantidad').on('input', function () {
        clearTimeout(temporizadorStock);
        temporizadorStock = setTimeout(validarCantidad, 250);
    });

    function pintarLineas() {
        const $cuerpo = $('#lineas').empty();
        if (lineas.length === 0) {
            $cuerpo.append('<tr><td colspan="4" class="text-center text-muted">Todavía no hay líneas.</td></tr>');
        }
        lineas.forEach(function (linea, indice) {
            $cuerpo.append(
                '<tr><td>' + escapar(linea.nombre) + '</td>' +
                '<td class="text-end">' + linea.cantidad + '</td>' +
                '<td class="text-end">' + linea.stock + '</td>' +
                '<td class="text-end"><button type="button" class="btn btn-sm btn-link text-danger quitar" data-indice="' + indice + '">Quitar</button></td></tr>');
        });
        $('#validarPedido').prop('disabled', lineas.length === 0);
    }

    $('#agregarLinea').on('click', function () {
        const cantidad = parseInt($('#cantidad').val(), 10);
        lineas.push({ productoId: productoActual.id, nombre: productoActual.nombre, cantidad: cantidad, stock: productoActual.stock });
        pintarLineas();
        validarCantidad();
    });

    $('#lineas').on('click', '.quitar', function () {
        lineas.splice($(this).data('indice'), 1);
        pintarLineas();
        validarCantidad();
    });

    // POST protegido con antiforgery: el token lo agrega antiforgery.js en el encabezado.
    $('#validarPedido').on('click', function () {
        const solicitud = lineas.map(function (linea) {
            return { productoId: linea.productoId, cantidad: linea.cantidad };
        });

        $.validarStockPedido(solicitud)
            .done(function (resultado) {
                if (resultado.esValido) {
                    $('#resultadoPedido').html('<div class="alert alert-success mb-0">Hay stock suficiente para todas las líneas.</div>');
                } else {
                    const errores = resultado.errores.map(function (error) { return '<li>' + escapar(error) + '</li>'; }).join('');
                    $('#resultadoPedido').html('<div class="alert alert-danger mb-0"><ul class="mb-0">' + errores + '</ul></div>');
                }
            })
            .fail(function (xhr) {
                const mensaje = xhr.status === 400
                    ? 'Solicitud rechazada (token antiforgery inválido o datos incorrectos).'
                    : 'No se pudo validar el pedido.';
                $('#resultadoPedido').html('<div class="alert alert-danger mb-0">' + mensaje + '</div>');
            });
    });
});
