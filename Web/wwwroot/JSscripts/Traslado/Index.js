window.Componente = {
    UrlControlador: "/Traslado/"
}




let tablaTraslados;
$(document).ready(function () {
    inicializarTabla();
});

function inicializarTabla() {
    if ($.fn.DataTable.isDataTable('#TablaTraslados')) {
        $('#TablaTraslados').DataTable().destroy();
        $('#TablaTraslados tbody').empty();
    }

    tablaTraslados = $('#TablaTraslados').DataTable({
        ajax: {
            url: Componente.UrlControlador + 'ObtenerListaTraslados',
            type: 'GET',
            data: function (d) {
                // merge DataTables params con filtros (window.TrasladosFilters puede estar vacío)
                return $.extend({}, d, window.TrasladosFilters);
            },
            dataSrc: function (json) {
                if (!json.success) {
                    Swal.fire('Error', json.error || 'No se pudieron cargar los traslados.', 'error');
                    return [];
                }
                return json.data || [];
            }
        },
        columns: [
            { data: 'nombreProducto', defaultContent: '' },
            { data: 'cantidad', className: 'text-center', defaultContent: '' },
            { data: 'nombreBodegaOrigen', defaultContent: '' },
            { data: 'nombreBodegaDestino', defaultContent: '' },
            {
                data: 'fechaTraslado',
                render: function (data) {
                    if (!data) return '';
                    const d = new Date(data);
                    return d.toLocaleDateString() + ' ' + d.toLocaleTimeString();
                }
            },
            {
                data: null,
                orderable: false,
                searchable: false,
                render: function (row) {
                    return `<button class="btn btn-sm btn-info btn-ver" data-id="${row.trasladoID || 0}"><i class="fa-solid fa-eye"></i></button>`;
                }
            }
        ],
        order: [[4, 'desc']],
        responsive: true,
        pageLength: 10,
        dom: "<'row mb-2'<'col-sm-6'B><'col-sm-6 text-end'f>>rt<'row mt-3'<'col-sm-6'i><'col-sm-6'p>>",
        buttons: [
            {
                text: 'Nuevo',
                className: 'btn btn-primary',
                action: function () {
                    $.get(Componente.UrlControlador + "Formulario", function (html) {
                        //NavegacionTituloMenu('Ingreso Registro');
                        $('#form-traslado').show();
                        $('#form-traslado').html(html)
                        $('#DivTablaTraslados').hide();
                    }).fail(function (data) {
                        swal("Error", "Error al realizar la petición", "error")
                    });
                }
            },
            { extend: 'excelHtml5', className: 'btn btn-success btn-sm', text: '<i class="fa-solid fa-file-excel"></i> Excel' },
            { extend: 'pdfHtml5', className: 'btn btn-danger btn-sm', text: '<i class="fa-solid fa-file-pdf"></i> PDF' },
            { extend: 'print', className: 'btn btn-secondary btn-sm', text: '<i class="fa-solid fa-print"></i> Imprimir' }
        ],
        //language: { url: '//cdn.datatables.net/plug-ins/1.13.6/i18n/es-ES.json' }
    });

    //// Delegated event example
    //$('#TablaTraslados tbody').on('click', '.btn-ver', function () {
    //    const data = tablaTraslados.row($(this).closest('tr')).data();
    //    verDetalleTraslado(data);
    //})

}