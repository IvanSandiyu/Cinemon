function actualizarFechaHora() {
    const ahora = new Date();

    const fecha = ahora.toLocaleDateString('es-AR', {
        weekday: 'long',
        day: '2-digit',
        month: 'long'
    });

    const hora = ahora.toLocaleTimeString('es-AR', {
        hour: '2-digit',
        minute: '2-digit',
        second: '2-digit',
        hour12: false
    });

    const elemento = document.getElementById('fecha-hora');

    if (elemento) {
        elemento.textContent =
            `${fecha.charAt(0).toUpperCase() + fecha.slice(1)}, ${hora}`;
    }
}

actualizarFechaHora();
setInterval(actualizarFechaHora, 1000);