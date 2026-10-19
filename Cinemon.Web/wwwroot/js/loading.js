(function () {
    var placeholderId = 'cinemon-page-loading';
    var observer = null;

    function removePlaceholder() {
        var placeholder = document.getElementById(placeholderId);

        if (observer) {
            observer.disconnect();
            observer = null;
        }

        if (placeholder && placeholder.parentNode) {
            placeholder.parentNode.removeChild(placeholder);
        }
    }

    function pageContentReady() {
        var main = document.querySelector('.cinemon-body');

        if (!main) {
            return false;
        }

        var placeholder = document.getElementById(placeholderId);
        var children = Array.prototype.slice.call(main.children);
        var hasContent = children.some(function (child) {
            return child !== placeholder;
        });

        if (hasContent) {
            return true;
        }

        return !placeholder && main.textContent.trim().length > 0;
    }

    function check() {
        if (pageContentReady()) {
            removePlaceholder();
        }
    }

    function watch() {
        if (observer) {
            observer.disconnect();
            observer = null;
        }

        var main = document.querySelector('.cinemon-body');

        if (!main || !document.getElementById(placeholderId)) {
            return;
        }

        check();

        if (document.getElementById(placeholderId)) {
            observer = new MutationObserver(check);
            observer.observe(main, { childList: true, subtree: true, characterData: true });
        }
    }

    window.cinemonPageReady = watch;

    // También en navegación mejorada: el layout estático vuelve a incluir el
    // placeholder y hay que quitarlo cuando llega el contenido de la página.
    if (window.Blazor && window.Blazor.addEventListener) {
        window.Blazor.addEventListener('enhancedload', function () {
            window.setTimeout(watch, 0);
        });
    }

    watch();

    // Si el circuito no llega a renderizar nada, liberamos el área de contenido.
    window.addEventListener('load', function () {
        window.setTimeout(function () {
            if (document.getElementById(placeholderId)) {
                removePlaceholder();
            }
        }, 15000);
    });
})();
