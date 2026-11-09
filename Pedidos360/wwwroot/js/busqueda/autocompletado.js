// Autocompletado de productos reutilizable (jQuery).
// Uso:
//   $('#buscarProducto').autocompletarProducto({
//       alSeleccionar: function (producto) { ... }   // producto = { id, codigo, nombre, precio, impuestoPorc, stock, imagenUrl }
//   });
(function ($) {
    'use strict';

    const URL_BUSQUEDA = '/api/productos/buscar';
    const MINIMO_CARACTERES = 2;
    const ESPERA_MS = 300;

    function formatearMonto(valor) {
        return Number(valor).toLocaleString('es-CR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    }

    function escapar(texto) {
        return $('<div>').text(texto == null ? '' : texto).html();
    }

    $.fn.autocompletarProducto = function (opciones) {
        const configuracion = $.extend({
            alSeleccionar: function () { },
            maximo: 10,
            permitirSinStock: false
        }, opciones);

        return this.each(function () {
            const $entrada = $(this);
            const $contenedor = $('<div class="position-relative"></div>');
            const $lista = $('<div class="list-group position-absolute w-100 shadow-sm" style="z-index:1050; max-height:320px; overflow-y:auto;"></div>').hide();

            $entrada.attr('autocomplete', 'off').wrap($contenedor).after($lista);

            let temporizador = null;
            let peticionActual = null;
            let resultados = [];
            let indiceActivo = -1;

            function cerrar() {
                $lista.hide().empty();
                indiceActivo = -1;
            }

            function mostrarMensaje(texto, clase) {
                $lista.html('<div class="list-group-item small ' + (clase || 'text-muted') + '">' + escapar(texto) + '</div>').show();
            }

            function pintar() {
                $lista.empty();
                if (resultados.length === 0) {
                    mostrarMensaje('No se encontraron productos activos.');
                    return;
                }

                resultados.forEach(function (producto, indice) {
                    const sinStock = producto.stock <= 0;
                    const deshabilitado = sinStock && !configuracion.permitirSinStock;
                    const insignia = sinStock
                        ? '<span class="badge bg-danger">Sin stock</span>'
                        : '<span class="badge bg-success">Stock: ' + producto.stock + '</span>';

                    const $opcion = $(
                        '<button type="button" class="list-group-item list-group-item-action d-flex align-items-center gap-2">' +
                        '<img src="' + escapar(producto.imagenUrl) + '" alt="" width="40" height="40" style="object-fit:cover" class="rounded">' +
                        '<div class="flex-grow-1 text-start">' +
                        '<div class="fw-semibold">' + escapar(producto.nombre) + '</div>' +
                        '<div class="small text-muted">' + escapar(producto.codigo) + (producto.categoria ? ' · ' + escapar(producto.categoria) : '') +
                        ' · ₡' + formatearMonto(producto.precio) + '</div>' +
                        '</div>' + insignia +
                        '</button>');

                    if (deshabilitado) {
                        $opcion.addClass('disabled').attr('aria-disabled', 'true');
                    }
                    if (indice === indiceActivo) {
                        $opcion.addClass('active');
                    }

                    $opcion.on('mousedown', function (evento) {
                        evento.preventDefault();
                        if (!deshabilitado) {
                            seleccionar(indice);
                        }
                    });

                    $lista.append($opcion);
                });
                $lista.show();
            }

            function seleccionar(indice) {
                const producto = resultados[indice];
                if (!producto) {
                    return;
                }
                $entrada.val(producto.codigo + ' - ' + producto.nombre);
                cerrar();
                configuracion.alSeleccionar(producto);
            }

            function buscar(texto) {
                if (peticionActual) {
                    peticionActual.abort();
                }
                mostrarMensaje('Buscando...');
                peticionActual = $.ajax({
                    url: URL_BUSQUEDA,
                    type: 'GET',
                    data: { q: texto, max: configuracion.maximo },
                    dataType: 'json'
                }).done(function (datos) {
                    resultados = datos || [];
                    indiceActivo = -1;
                    pintar();
                }).fail(function (xhr, estado) {
                    if (estado === 'abort') {
                        return;
                    }
                    if (xhr.status === 401 || xhr.status === 302) {
                        mostrarMensaje('Su sesión expiró. Inicie sesión de nuevo.', 'text-danger');
                    } else {
                        mostrarMensaje('No se pudo realizar la búsqueda.', 'text-danger');
                    }
                });
            }

            $entrada.on('input', function () {
                const texto = $.trim($entrada.val());
                clearTimeout(temporizador);
                if (texto.length < MINIMO_CARACTERES) {
                    cerrar();
                    return;
                }
                temporizador = setTimeout(function () { buscar(texto); }, ESPERA_MS);
            });

            // Navegación con el teclado: flechas, Enter y Escape.
            $entrada.on('keydown', function (evento) {
                if (!$lista.is(':visible') || resultados.length === 0) {
                    return;
                }
                if (evento.key === 'ArrowDown') {
                    evento.preventDefault();
                    indiceActivo = Math.min(indiceActivo + 1, resultados.length - 1);
                    pintar();
                } else if (evento.key === 'ArrowUp') {
                    evento.preventDefault();
                    indiceActivo = Math.max(indiceActivo - 1, 0);
                    pintar();
                } else if (evento.key === 'Enter') {
                    evento.preventDefault();
                    if (indiceActivo >= 0) {
                        seleccionar(indiceActivo);
                    }
                } else if (evento.key === 'Escape') {
                    cerrar();
                }
            });

            $entrada.on('blur', function () {
                setTimeout(cerrar, 150);
            });
        });
    };

    // Consulta el stock de un producto para una cantidad. Devuelve una promesa jQuery.
    $.validarStockProducto = function (productoId, cantidad) {
        return $.ajax({
            url: '/api/productos/' + encodeURIComponent(productoId) + '/stock',
            type: 'GET',
            data: { cantidad: cantidad },
            dataType: 'json'
        });
    };

    // Valida todas las líneas de un pedido (POST con antiforgery).
    $.validarStockPedido = function (lineas) {
        return $.ajax({
            url: '/api/productos/validar-stock',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(lineas),
            dataType: 'json'
        });
    };
})(jQuery);
