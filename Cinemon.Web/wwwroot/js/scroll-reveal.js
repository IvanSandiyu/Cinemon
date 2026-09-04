(function () {
    'use strict';

    // Respeta la preferencia de "reducir movimiento" del usuario
    if (window.matchMedia('(prefers-reduced-motion: reduce)').matches) return;

    var revealObserver = new IntersectionObserver(function (entries) {
        for (var i = 0; i < entries.length; i++) {
            if (entries[i].isIntersecting) {
                entries[i].target.classList.add('is-revealed');
                revealObserver.unobserve(entries[i].target);
            }
        }
    }, { threshold: 0.12, rootMargin: '0px 0px -40px 0px' });

    function observar() {
        var elementos = document.querySelectorAll('.reveal:not(.is-revealed)');
        for (var i = 0; i < elementos.length; i++) {
            revealObserver.observe(elementos[i]);
        }
    }

    // Observa elementos nuevos que Blazor agrega al DOM dinámicamente
    var mutObserver = new MutationObserver(observar);

    function init() {
        observar();
        mutObserver.observe(document.body, { childList: true, subtree: true });
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }
})();