window.Componente = {
    UrlControlador: "/Traslado/"
}


let tablaTraslados;


inicializarTabla();



function MostrarDivNuevo() {
    $('#form-trasladoNuevo').show();
    $('#DivTablaTraslados').hide();
}

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
                    MostrarDivNuevo();
                }
            },
            { extend: 'excelHtml5', className: 'btn btn-success btn-sm', text: '<i class="fa-solid fa-file-excel"></i> Excel' },
            { extend: 'pdfHtml5', className: 'btn btn-danger btn-sm', text: '<i class="fa-solid fa-file-pdf"></i> PDF' },
            { extend: 'print', className: 'btn btn-secondary btn-sm', text: '<i class="fa-solid fa-print"></i> Imprimir' }
        ],
    });

}


// Lista de productos actualmente disponibles en la bodega seleccionada
let productosDisponibles = [];

// Referencias globales
const $selectProductos = $('#ProdcutosBodega1');
const $tablaBody = $('#TablaProductosBodega tbody');
const $btnAgregar = $('#btnAgregarFila'); // botón agregar fila


// 🧠 Función auxiliar para habilitar/deshabilitar botón
function actualizarEstadoBoton() {
    const tieneOpciones = $selectProductos.find('option').length > 1;
    $btnAgregar.prop('disabled', !tieneOpciones);
}


// 🏭 Cargar productos de una bodega
$('#BodegaId').on('changed.bs.select', function () {
    const idBodega = $(this).val();

    $selectProductos.selectpicker('destroy');
    $selectProductos.empty();
    $tablaBody.empty();
    productosDisponibles = [];
    actualizarEstadoBoton();

    if (!idBodega) return;

    $.ajax({
        url: Componente.UrlControlador + 'ObtenerProductosPorBodega',
        type: 'GET',
        data: { idBodega },
        success: function (response) {
            if (response.success && response.data && response.data.productos && response.data.productos.length > 0) {
                productosDisponibles = response.data.productos;

                let opciones ;
                productosDisponibles.forEach(p => {
                    opciones += `<option value="${p.productoId}">${p.nombre} (Stock: ${p.cantidad})</option>`;
                });

                $selectProductos.html(opciones).selectpicker({
                    liveSearch: true,
                    width: '100%'
                });

                actualizarEstadoBoton();
            } else {
                Swal.fire({
                    icon: 'info',
                    title: 'Sin productos',
                    text: response.error || 'Esta bodega no tiene productos disponibles.'
                });

                $selectProductos.html('<option value="">Sin productos</option>').selectpicker({
                    liveSearch: true,
                    width: '100%'
                });

                actualizarEstadoBoton();
            }
        },
        error: function () {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'No se pudieron obtener los productos de la bodega.'
            });
        }
    });
});


// ➕ Agregar producto a la tabla
$btnAgregar.on('click', function () {
    const idProducto = $selectProductos.val();
    const nombreProducto = $selectProductos.find('option:selected').text();

    if (!idProducto) {
        Swal.fire({
            icon: 'warning',
            title: 'Seleccione un producto',
            text: 'Debe elegir un producto antes de agregarlo.'
        });
        return;
    }

    if ($tablaBody.find(`tr[data-id="${idProducto}"]`).length > 0) {
        Swal.fire({
            icon: 'info',
            title: 'Ya agregado',
            text: 'Este producto ya está en la tabla.'
        });
        return;
    }

    const producto = productosDisponibles.find(p => p.productoId == idProducto);
    if (!producto) return;

    const fila = `
        <tr data-id="${idProducto}">
            <td>${producto.nombre}</td>
            <td class="text-center">${producto.cantidad}</td>
            <td class="text-center">
                <button type="button" class="btn btn-warning btn-sm editar-fila" title="Editar cantidad">
                    <i class="fa-solid fa-pen"></i>
                </button>
                <button type="button" class="btn btn-danger btn-sm eliminar-fila" title="Eliminar">
                    <i class="fa-solid fa-trash"></i>
                </button>
            </td>
        </tr>`;

    $tablaBody.append(fila);

    $selectProductos.find(`option[value="${idProducto}"]`).remove();
    $selectProductos.selectpicker('destroy').selectpicker({
        liveSearch: true,
        width: '100%'
    });

    actualizarEstadoBoton();
});


// ✏️ Editar cantidad del producto
$(document).on('click', '.editar-fila', function () {
    const fila = $(this).closest('tr');
    const idProducto = fila.data('id');
    const producto = productosDisponibles.find(p => p.productoId == idProducto);
    const cantidadActual = parseInt(fila.find('td:nth-child(2)').text()) || 0;

    Swal.fire({
        title: `Editar cantidad de ${producto.nombre}`,
        input: 'number',
        inputLabel: `Stock disponible: ${producto.cantidad}`,
        inputValue: cantidadActual,
        inputAttributes: {
            min: 1,
            max: producto.cantidad,
            step: 1
        },
        showCancelButton: true,
        confirmButtonText: 'Guardar',
        cancelButtonText: 'Cancelar',
        inputValidator: (value) => {
            if (!value || value <= 0) {
                return 'Debe ingresar una cantidad válida.';
            }
            if (value > producto.cantidad) {
                return `No puede superar el stock disponible (${producto.cantidad}).`;
            }
        }
    }).then(result => {
        if (result.isConfirmed) {
            const nuevaCantidad = parseInt(result.value);
            fila.find('td:nth-child(2)').text(nuevaCantidad);

            Swal.fire({
                icon: 'success',
                title: 'Cantidad actualizada',
                text: `${producto.nombre}: ${nuevaCantidad}`,
                timer: 1200,
                showConfirmButton: false
            });
        }
    });
});


// 🗑️ Eliminar fila → devolver producto al selectpicker
$(document).on('click', '.eliminar-fila', function () {
    const fila = $(this).closest('tr');
    const idProducto = fila.data('id');
    fila.remove();

    const producto = productosDisponibles.find(p => p.productoId == idProducto);
    if (producto) {
        if ($selectProductos.find(`option[value="${producto.productoId}"]`).length === 0) {
            $selectProductos.append(
                `<option value="${producto.productoId}">${producto.nombre} (Stock: ${producto.cantidad})</option>`
            );
        }

        $selectProductos.selectpicker('destroy').selectpicker({
            liveSearch: true,
            width: '100%'
        });
    }

    actualizarEstadoBoton();
});
