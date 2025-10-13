$('#cancelButtonEditar').on('click', function () {
    RetornarAIndexDesdeEditar();
/*    bloquearControles('desbloquear');*/
});



function RetornarAIndexDesdeEditar() {
    // Ocultar y limpiar completamente el div de edición
    $('#DivEditarOInfo').hide().empty();

    // Mostrar nuevamente la tabla principal
    $('#DivTablaBodegas').show();
}

