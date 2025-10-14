$('#cancelButtonEditar').on('click', function () {
    RetornarAIndexDesdeEditar();
/*    bloquearControlesEditar('desbloquear');*/
});



function RetornarAIndexDesdeEditar() {
    // Ocultar y limpiar completamente el div de edición
    $('#DivEditarOInfo').hide().empty();

    // Mostrar nuevamente la tabla principal
    $('#DivTablaBodegas').show();
}

window.Componente = {
    UrlControlador: "/Bodega/"
};

ConsultarProductosEditar();

let productosSeleccionadoseditar = [];
let ListaProductoseditar = [];


function ConsultarProductosEditar() {
    $.ajax({
        url: '/Bodega/ObtenerListaProductos',
        type: 'GET',
        success: function (response) {
            if (response.data) {
                ListaProductoseditar = response.data;
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: response.error || 'No se pudieron cargar los productos.'
                });
            }
        },
        error: function (xhr, status, error) {
            Swal.fire({
                icon: 'error',
                title: 'Error en la solicitud AJAX',
                text: error
            });
        }
    });
};





function bloquearControlesEditar(modo) {
    const disabled = modo === 'bloquear';
    $('#btnAgregarFilaEditar').prop('disabled', disabled);
    $('#EditarDatosBodega').prop('disabled', disabled);
    $('.btnEditarFilaBodegaEditar').prop('disabled', disabled);
    $('.btnEliminarFilaBodegaEditar').prop('disabled', disabled);
}




$('#btnAgregarFilaEditar').on('click', function () {
    // Evitar agregar si hay fila en edición
    const filaEditable = $('#TablaProductosBodegaEditar tbody tr').filter(function () {
        return $(this).find('.btnGuardarFilaEditar').is(':visible');
    });
    if (filaEditable.length > 0) {
        Swal.fire({
            icon: 'warning',
            title: 'Atención',
            text: 'Debe guardar la fila actual antes de agregar una nueva.'
        });
        return;
    }

    // Validar lista de productos
    if (!ListaProductoseditar || ListaProductoseditar.length === 0) {
        Swal.fire({
            icon: 'warning',
            title: 'Sin productos',
            text: 'No hay productos cargados.'
        });
        return;
    }


    productosSeleccionados = [];
    $('#TablaProductosBodegaEditar tbody tr').each(function () {
        const id = $(this).find('.productoTexto').data('id');
        if (id) productosSeleccionados.push(parseInt(id));
    });

    // Construir opciones filtradas
    let opciones = '<option value="">Seleccione un producto</option>';
    ListaProductoseditar.forEach(p => {
        if (!productosSeleccionados.includes(p.productoId)) {
            opciones += `<option value="${p.productoId}">${p.nombre}</option>`;
        }
    });

    if (opciones === '<option value="">Seleccione un producto</option>') {
        Swal.fire({
            icon: 'info',
            title: 'Productos agotados',
            text: 'Ya ha agregado todos los productos disponibles.'
        });
        return;
    }

    // Crear la nueva fila
    const nuevaFila = `
        <tr>
            <td>
                <select class="selectProducto form-select" name="productoId" data-live-search="true">
                    ${opciones}
                </select>
            </td>
            <td>
                <input type="number" class="form-control cantidadProducto" min="1" step="1" placeholder="Cantidad">
            </td>
            <td class="text-center">
                <button type="button" class="btn btn-success btn-sm btnGuardarFilaEditar" title="Guardar">
                    <i class="fa-solid fa-floppy-disk"></i>
                </button>
                <button type="button" class="btn btn-primary btn-sm btnEditarFilaBodegaEditar" title="Editar" style="display:none;">
                    <i class="fa-solid fa-pen-to-square"></i>
                </button>
                <button type="button" class="btn btn-danger btn-sm btnEliminarFilaBodegaEditar" title="Eliminar">
                    <i class="fa-solid fa-trash"></i>
                </button>
            </td>
        </tr>`.trim();

    const $tbody = $('#TablaProductosBodegaEditar tbody');
    $tbody.append(nuevaFila);

    // Inicializar solo el último select
    const $ultimoSelect = $tbody.find('tr:last .selectProducto');
    $ultimoSelect.selectpicker({
        liveSearch: true,
        width: '100%'
    });

    // Bloquear todos los demás controles mientras esta fila esté en edición
    bloquearControlesEditar('bloquear');
});

$(document).on('click', '.btnGuardarFilaEditar', function () {
    const fila = $(this).closest('tr');
    const $select = fila.find('.selectProducto');
    const idProducto = $select.selectpicker('val') || $select.val();
    const nombreProducto = $select.find('option:selected').text();
    const cantidad = fila.find('.cantidadProducto').val();

    if (!idProducto || cantidad === '' || parseFloat(cantidad) <= 0) {
        Swal.fire({
            icon: 'warning',
            title: 'Campos inválidos',
            text: 'Debe seleccionar un producto y una cantidad válida.'
        });
        return;
    }

    // Actualizar lista global
    if (!productosSeleccionados.includes(parseInt(idProducto))) {
        productosSeleccionados.push(parseInt(idProducto));
    }

    // Convertir inputs a texto
    fila.find('td:eq(0)').html(`<span class="productoTexto" data-id="${idProducto}">${nombreProducto}</span>`);
    fila.find('td:eq(1)').html(`<span class="cantidadTexto">${cantidad}</span>`);

    fila.find('.btnGuardarFilaEditar').hide();
    fila.find('.btnEditarFilaBodegaEditar').show();
    bloquearControlesEditar('desbloquear');
});


$(document).on('click', '.btnEditarFilaBodegaEditar', function () {
    const fila = $(this).closest('tr');
    const idProducto = parseInt(fila.find('.productoTexto').data('id'));
    const cantidad = fila.find('.cantidadTexto').text();


    let opciones = '<option value="">Seleccione un producto</option>';
    ListaProductoseditar.forEach(p => {
        if (!productosSeleccionados.includes(p.productoId) || p.productoId === idProducto) {
            const selected = (p.productoId === idProducto) ? ' selected' : '';
            opciones += `<option value="${p.productoId}"${selected}>${p.nombre}</option>`;
        }
    });

    fila.find('td:eq(0)').html(`
        <select class="selectProducto" name="productoId" data-live-search="true" data-width="100%">
            ${opciones}
        </select>
    `);
    fila.find('td:eq(1)').html(`
        <input type="number" class="form-control cantidadProducto" min="1" step="1" value="${cantidad}">
    `);

    const $select = fila.find('.selectProducto');
    fila.find('.bootstrap-select').remove();
    $select.selectpicker({ liveSearch: true, width: '100%' });

    fila.find('.btnEditarFilaBodegaEditar').hide();
    fila.find('.btnGuardarFilaEditar').show();


    const index = productosSeleccionados.indexOf(idProducto);
    if (index !== -1) productosSeleccionados.splice(index, 1);
});



$(document).on('click', '.btnEliminarFilaBodegaEditar', function () {
    const fila = $(this).closest('tr');
    const id = parseInt(fila.find('.productoTexto').data('id'));

    Swal.fire({
        title: '¿Eliminar producto?',
        text: "Esta acción no se puede deshacer.",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Sí, eliminar',
        cancelButtonText: 'Cancelar'
    }).then(result => {
        if (result.isConfirmed) {
            fila.remove();


            const index = productosSeleccionados.indexOf(id);
            if (index !== -1) productosSeleccionados.splice(index, 1);



            bloquearControlesEditar('desbloquear');
        }


    });
});


$('#EditarDatosBodega').on('click', function () {
    // Validaciones iniciales
    var NombreBodega = $('#EditarOVisualizarBodegaForm').find('#NombreBodega').val();
    var IdBodega = $('#BodegaId').val();

    if (!NombreBodega || NombreBodega.trim() === '') {
        Swal.fire({
            icon: 'warning',
            title: 'Campo requerido',
            text: 'El nombre de la bodega es obligatorio'
        });
        $('#NombreBodega').trigger('focus');
        return;
    }

    // Verificar si hay filas en edición
    const filasEnEdicion = $('#TablaProductosBodega tbody tr').filter(function () {
        return $(this).find('.btnGuardarFilaEditar').is(':visible');
    });

    if (filasEnEdicion.length > 0) {
        Swal.fire({
            icon: 'warning',
            title: 'Fila sin guardar',
            text: 'Tiene ' + filasEnEdicion.length + ' fila(s) en modo edición. Debe guardarlas antes de continuar.'
        });
        return;
    }

    // Recorrer tabla y construir lista de productos
    var productos = [];
    var productosIds = new Set(); // Para evitar duplicados

    $('#TablaProductosBodegaEditar tbody tr').each(function () {
        const $fila = $(this);
        const $productoTexto = $fila.find('.productoTexto');
        const $cantidadTexto = $fila.find('.cantidadTexto');

        // Solo procesar filas guardadas
        if ($productoTexto.length > 0 && $cantidadTexto.length > 0) {
            const productoId = parseInt($productoTexto.data('id'));
            const nombreProducto = $productoTexto.text().trim();
            const cantidad = parseInt($cantidadTexto.text());

            // Validar datos
            if (!productoId || isNaN(cantidad) || cantidad <= 0) {
                return; // Saltar fila inválida
            }

            // Evitar duplicados
            if (productosIds.has(productoId)) {
                console.warn('Producto duplicado encontrado:', productoId);
                return;
            }

            productosIds.add(productoId);

            // Crear objeto producto según el formato del VM
            const productoVM = {
                ProductoId: productoId,
                Nombre: nombreProducto,
                cantidad: cantidad,
                Precio: 0, // Valor por defecto
                EstadoFila: true,
                FechaCreacion: null,
                FechaModificacion: null
            };

            productos.push(productoVM);
        }
    });

    // Construir objeto final
    var datosEnvio = {
        BodegaId: IdBodega,
        NombreBodega: NombreBodega.trim(),
        Productos: productos
    };


    // Mostrar confirmación antes de enviar
    Swal.fire({
        title: '¿Confirmar guardado?',
        html: `Va a crear la bodega <strong>"${NombreBodega.trim()}"</strong><br>
               ${productos.length > 0
                ? `con ${productos.length} producto(s)`
                : 'sin productos asociados'}`,
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Sí, guardar',
        cancelButtonText: 'Cancelar'
    }).then((result) => {
        if (result.isConfirmed) {
            enviarDatosAlServidor(datosEnvio);
        }
    });
});

function enviarDatosAlServidor(datos) {
    $.ajax({
        url: Componente.UrlControlador + 'EditarBodegaYProductos',
        type: 'POST',
        data: BodegaProductos = datos,
        success: function (response) {
            if (response.success) {
                const mensaje = datos.Productos.length > 0
                    ? 'La Bodega y Productos se agregaron exitosamente'
                    : 'La Bodega se creó exitosamente';

                Swal.fire({
                    icon: 'success',
                    title: 'Éxito',
                    text: mensaje,
                    showConfirmButton: true,
                }).then(function () {
                    RetornarAIndexDesdeEditar();
                    PoblarTablaBodegas();
                });
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: response.error || 'Hubo un error al procesar la solicitud'
                });
            }
        },
        error: function (xhr, status, error) {
            let errorMsg = 'Hubo un error al enviar el formulario';
            if (xhr.responseJSON && xhr.responseJSON.error) {
                errorMsg = xhr.responseJSON.error;
            }

            Swal.fire({
                icon: 'error',
                title: 'Error de conexión',
                text: errorMsg
            });
        }
    });
}

