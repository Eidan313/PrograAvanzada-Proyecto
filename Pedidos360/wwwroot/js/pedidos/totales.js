// Totales en vivo del pedido

(function () {
    'use strict';

    const cuerpo = document.querySelector('#lineas');
    if (!cuerpo) {
        return;
    }

    const esperaMs = 300;
    const dinero = valor => new Intl.NumberFormat('es-CR', { style: 'currency', currency: 'CRC', maximumFractionDigits: 2 }).format(valor);
    let temporizador;
    let ultimaPeticion = 0;

    function texto(selector, valor) {
        const elemento = document.querySelector(selector);
        if (elemento) {
            elemento.textContent = valor;
        }
    }

    function mostrarError(mensaje) {
        texto('#totales-error', mensaje);
    }

    function leerLineas() {
        return [...cuerpo.querySelectorAll('tr[data-producto]')].map(fila => ({
            productoId: Number(fila.dataset.producto),
            cantidad: Number(fila.querySelector('[data-cantidad]')?.value) || 0,
            tipoDescuento: Number(fila.querySelector('[data-tipo-descuento]')?.value) || 1,
            descuento: Number(fila.querySelector('[data-descuento]')?.value) || 0
        }));
    }

    function limpiarTotales() {
        cuerpo.querySelectorAll('[data-total-linea]').forEach(celda => { celda.textContent = ''; });
        texto('#total-subtotal', dinero(0));
        texto('#total-impuestos', dinero(0));
        texto('#total-pedido', dinero(0));
    }

    function mostrarResultado(resultado) {
        resultado.lineas.forEach(linea => {
            const fila = cuerpo.querySelector(`tr[data-producto="${linea.productoId}"]`);
            const celda = fila?.querySelector('[data-total-linea]');
            if (celda) {
                celda.textContent = dinero(linea.totalLinea);
            }
        });
        texto('#total-subtotal', dinero(resultado.subtotal));
        texto('#total-impuestos', dinero(resultado.impuestos));
        texto('#total-pedido', dinero(resultado.total));
    }

    // Devuelve el mensaje de los errores de validacion
    async function leerError(respuesta) {
        try {
            const cuerpoError = await respuesta.json();
            const detalles = cuerpoError.errors ? Object.values(cuerpoError.errors).flat().join(' ') : '';
            return detalles || cuerpoError.title || 'No se pudo calcular el pedido.';
        } catch {
            return 'No se pudo calcular el pedido.';
        }
    }

    async function calcular() {
        const lineas = leerLineas();
        mostrarError('');
        if (lineas.length === 0) {
            limpiarTotales();
            return;
        }

        // Solo usa la ultima peticion
        const numero = ++ultimaPeticion;
        try {
            const respuesta = await fetch('/api/pedidos/calcular', {
                method: 'POST',
                headers: window.Pedidos360.encabezadosAntiforgery(),
                body: JSON.stringify({ lineas })
            });
            if (numero !== ultimaPeticion) {
                return;
            }
            if (!respuesta.ok) {
                mostrarError(await leerError(respuesta));
                return;
            }
            mostrarResultado(await respuesta.json());
        } catch {
            if (numero === ultimaPeticion) {
                mostrarError('No se pudo conectar con el servidor.');
            }
        }
    }

    function recalcular() {
        clearTimeout(temporizador);
        temporizador = setTimeout(calcular, esperaMs);
    }

    // Los eventos se escuchan en el cuerpo, asi funcionan tambien con las filas que se agreguen despues
    cuerpo.addEventListener('input', evento => {
        if (evento.target.matches('[data-cantidad], [data-descuento]')) {
            recalcular();
        }
    });
    cuerpo.addEventListener('change', evento => {
        if (evento.target.matches('[data-tipo-descuento]')) {
            recalcular();
        }
    });
    cuerpo.addEventListener('click', evento => {
        const boton = evento.target.closest('[data-quitar]');
        if (boton) {
            boton.closest('tr').remove();
            recalcular();
        }
    });

    window.Pedidos360 = window.Pedidos360 || {};
    window.Pedidos360.totales = { recalcular };
})();
