// Agrega el token antiforgery a todas las peticiones AJAX de jQuery que modifican datos.
(function ($) {
    'use strict';

    const token = $('meta[name="antiforgery-token"]').attr('content');
    const encabezado = $('meta[name="antiforgery-header"]').attr('content') || 'RequestVerificationToken';
    const metodosSeguros = ['GET', 'HEAD', 'OPTIONS', 'TRACE'];

    if (!token) {
        return;
    }

    $.ajaxSetup({
        beforeSend: function (xhr, opciones) {
            const metodo = (opciones.type || 'GET').toUpperCase();
            if (metodosSeguros.indexOf(metodo) === -1) {
                xhr.setRequestHeader(encabezado, token);
            }
        }
    });

    // Para quien use fetch() en lugar de jQuery.
    window.Pedidos360 = window.Pedidos360 || {};
    window.Pedidos360.encabezadosAntiforgery = function () {
        const encabezados = { 'Content-Type': 'application/json' };
        encabezados[encabezado] = token;
        return encabezados;
    };
})(jQuery);
