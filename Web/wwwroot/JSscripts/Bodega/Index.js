window.Componente = {
    UrlControlador: "/Bodega/"
};



PoblarTablaBodegas();

function PoblarTablaBodegas() {
    if ($.fn.DataTable.isDataTable('#TablaBodegas')) {
        $('#TablaBodegas').DataTable().destroy();
    }

    $.ajax({
        url: Componente.UrlControlador + "ObtenerlistaBodegas",
        type: 'POST',
        success: function (response) {
            if (response.error) {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: response.error
                });
                return;
            }

            table = $('#TablaBodegas').DataTable({
                data: response.data,
                columns: [
                    { data: 'NombreBodega' },
                    {
                        data: 'BodegaId',
                        render: function (data, type, row) {
                            return `
                                <div class="d-flex justify-content-between">
                                    <div class="btn-group">
                                        <button class="btn btn-primary editar-btn" data-idbodega="${row.bodegaid}">
                                            <i class="fa-solid fa-pen-to-square"></i>
                                        </button>
                                        <button class="btn btn-danger eliminar-btn" data-idbodega="${row.bodegaid}">
                                            <i class="fa-solid fa-trash"></i>
                                        </button>
                                    </div>
                                </div>
                            `;
                        }
                    }
                ],
                order: [[0, 'asc']],
                responsive: true,

                // 💡 Distribución personalizada del header
                dom:
                    "<'row mb-3'<'col-sm-6 d-flex align-items-center'B><'col-sm-6 d-flex justify-content-end'f>>" +
                    "rt" +
                    "<'row mt-3'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>",

                buttons: [
                    {
                        text: 'Nuevo',
                        className: 'btn btn-primary',
                        action: function () {
                            $('#DivAgregarBodega').show();
                            $('#DivTablaBodegas').hide();
                        }
                    },
                    { extend: 'excel', text: '<i class="fa-solid fa-file-excel"></i> Excel', className: 'btn btn-success' },
                    { extend: 'pdf', text: '<i class="fa-solid fa-file-pdf"></i> PDF', className: 'btn btn-danger' },
                    { extend: 'print', text: '<i class="fa-solid fa-print"></i> Imprimir', className: 'btn btn-secondary' }
                ]
            });
        },
        error: function (xhr, status, error) {
            Swal.fire({
                icon: 'error',
                title: 'Error en la solicitud AJAX',
                text: error
            });
        }
    });
}

$('#cancelButton').on('click', function () {
    $('#DivAgregarBodega').hide();
    $('#DivTablaBodegas').show();
    $('#AgregarBodegaForm')[0].reset();
});







// Al hacer clic en el botón "+"

$('#btnAgregarFila').on('click', function () {
    const nuevaFila = `
        <tr>
            <td><input type="text" class="form-control nombreProducto" placeholder="Ingrese nombre"></td>
            <td>
                <div class="input-group">
                    <span class="input-group-text">C$</span>
                    <input type="number" class="form-control precioProducto" step="0.01" min="0" placeholder="0.00">
                </div>
            </td>
            <td class="text-center">
                <button type="button" class="btn btn-success btn-sm btnGuardarFila" title="Guardar">
                    <i class="fa-solid fa-floppy-disk"></i>
                </button>
                <button type="button" class="btn btn-primary btn-sm btnEditarFila" title="Editar" style="display:none;">
                    <i class="fa-solid fa-pen-to-square"></i>
                </button>
                <button type="button" class="btn btn-danger btn-sm btnEliminarFila" title="Eliminar">
                    <i class="fa-solid fa-trash"></i>
                </button>
            </td>
        </tr>
    `;

    $('#TablaProductosBodega tbody').append(nuevaFila);
});


// Guardar fila → convierte inputs a texto
$(document).on('click', '.btnGuardarFila', function () {
    const fila = $(this).closest('tr');
    const nombre = fila.find('.nombreProducto').val().trim();
    const precio = fila.find('.precioProducto').val();

    if (nombre === '' || precio === '' || parseFloat(precio) < 0) {
        Swal.fire({
            icon: 'warning',
            title: 'Campos inválidos',
            text: 'Debe ingresar un nombre y un precio válido.'
        });
        return;
    }

    // Sustituir los inputs por texto
    fila.find('td:eq(0)').html(`<span class="nombreTexto">${nombre}</span>`);
    fila.find('td:eq(1)').html(`<span class="precioTexto">C$ ${parseFloat(precio).toFixed(2)}</span>`);

    // Cambiar botones visibles
    fila.find('.btnGuardarFila').hide();
    fila.find('.btnEditarFila').show();
});


// Editar fila → vuelve los textos a inputs
$(document).on('click', '.btnEditarFila', function () {
    const fila = $(this).closest('tr');
    const nombre = fila.find('.nombreTexto').text();
    const precio = fila.find('.precioTexto').text().replace('C$ ', '');

    fila.find('td:eq(0)').html(`<input type="text" class="form-control nombreProducto" value="${nombre}">`);
    fila.find('td:eq(1)').html(`
        <div class="input-group">
            <span class="input-group-text">C$</span>
            <input type="number" class="form-control precioProducto" step="0.01" min="0" value="${precio}">
        </div>
    `);

    // Cambiar botones visibles
    fila.find('.btnEditarFila').hide();
    fila.find('.btnGuardarFila').show();
});


// Eliminar fila
$(document).on('click', '.btnEliminarFila', function () {
    const fila = $(this).closest('tr');
    Swal.fire({
        title: '¿Eliminar producto?',
        text: "Esta acción no se puede deshacer.",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Sí, eliminar',
        cancelButtonText: 'Cancelar'
    }).then((result) => {
        if (result.isConfirmed) {
            fila.remove();
        }
    });
});

