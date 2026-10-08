window.cinemonCarrito = (function () {
    var clave = 'cinemon.carrito';

    function leer() {
        try {
            var crudo = window.localStorage.getItem(clave);

            if (!crudo) {
                return [];
            }

            var items = JSON.parse(crudo);

            return Array.isArray(items) ? items : [];
        } catch (e) {
            return [];
        }
    }

    function guardar(items) {
        try {
            window.localStorage.setItem(clave, JSON.stringify(items));
        } catch (e) {
        }
    }

    function vaciar() {
        try {
            window.localStorage.removeItem(clave);
        } catch (e) {
        }
    }

    return {
        leer: leer,
        guardar: guardar,
        vaciar: vaciar
    };
})();
