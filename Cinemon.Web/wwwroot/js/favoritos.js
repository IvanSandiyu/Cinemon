window.cinemonFavoritos = {
    obtener: function () {
        try {
            return window.localStorage.getItem('cinemon-favoritos') || '';
        } catch (e) {
            return '';
        }
    },
    guardar: function (ids) {
        try {
            window.localStorage.setItem('cinemon-favoritos', ids);
        } catch (e) {
        }
    }
};