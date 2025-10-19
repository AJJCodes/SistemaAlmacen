

$(document).ready(function () {

    window.Componente = {
        UrlControlador: "/Producto/"
    };


    //alert('Hola mundo')

    tabla = $("#TablaProducto").DataTable({
        "ajax": {
            "url": Componente.UrlControlador + 'ObtenerlistaProductos',
            "type": 'post',
            "dataSrc": function (json) {
                console.log(json)
                if (json.status) {
                    return json.data;
                } else {
                    swal("Oops", json.error, "error")
                    return [];
                }
            }
        },
     
        "columns": [
            {
                "data": "nombre",
                className: 'text-center',
                title: 'Nombre'
            },
            {
                "data": "precio",
                className: 'text-center',
                title: 'Precio'
            },
           
            {
                "data": "productoId",
                "className": "text-center",
                title: 'Opciones',
                "orderable": false,
                "render": function (data, type, row, meta) {

                    console.log(data)
                    return '<button class="btn btn-info btn-icon editar mar-rgt" title="Actualizar registro"  data-id="' + data + '"><i class="la la-pencil icon-lg"></i></button>'
                        + '<button class="btn btn-danger btn-icon eliminar" data-id="' + data + '" title ="Eliminar registro" ><i class="la la-trash icon-lg"></i></button>';
                }
            }
        ],
        // 💡 Distribución personalizada del header
        dom:
            "<'row mb-3'<'col-sm-6 d-flex align-items-center'B><'col-sm-6 d-flex justify-content-end'f>>" +
            "rt" +
            "<'row mt-3'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>",

        "buttons": [
            {
                text: 'Nuevo',
                className: 'btn btn-primary',
                action: function () {
                    $.get(Componente.UrlControlador + "Formulario", function (html) {
                        //NavegacionTituloMenu('Ingreso Registro');
                        $('#form-producto').show();
                        $('#form-producto').html(html)
                        $('#div-tabla-productos').hide();
                    }).fail(function (data) {
                        swal("Error", "Error al realizar la petición", "error")
                    });
                }
            },
            { extend: 'excel', text: '<i class="fa-solid fa-file-excel"></i> Excel', className: 'btn btn-success' },
            { extend: 'pdf', text: '<i class="fa-solid fa-file-pdf"></i> PDF', className: 'btn btn-danger' },
            { extend: 'print', text: '<i class="fa-solid fa-print"></i> Imprimir', className: 'btn btn-secondary' }
            //{
            //    extend: "excel",
            //    text: "Excel",
            //    titleAttr: 'Exportar datos a excel'
            //},
            //{
            //    extend: "print",
            //    text: "Imprimir",
            //    className: 'trn',
            //    attr: {
            //        'data-trn-key': 'FormButton.btnImprimir',
            //        title: 'Imprimir datos'
            //    }
            //}
        ]
    });


    // 🔹 Evento click para Editar
    $('#TablaProducto').on('click', 'button.editar', function () {
        const id = $(this).data('id');
        
        //// Llamada GET al controlador para traer el formulario con datos
        $.get(Componente.UrlControlador +  "Formulario", { ProductoId: id }, function (html) {
            $('#form-producto').show().html(html);
            $('#div-tabla-productos').hide();
        }).fail(function () {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'No se pudo cargar el formulario de edición.'
            })
            //swal("Error", "No se pudo cargar el formulario de edición", "error");
        });
    });

    $('#TablaProducto').on('click', 'button.eliminar', function () {
        const id = $(this).data('id');

        Swal.fire({
            title: '¿Estás seguro?',
            text: "El producto será desactivado (eliminación lógica).",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Sí, eliminar',
            cancelButtonText: 'Cancelar'
        }).then((result) => {
            if (result.isConfirmed) {

                $.ajax({
                    url: Componente.UrlControlador + "EliminarProducto",
                    type: "POST",
                    data: { id: id },
                    success: function (response) {
                        if (response.estado) {
                            Swal.fire({
                                icon: 'success',
                                title: 'Eliminado',
                                text: 'El producto ha sido eliminado correctamente.',
                                timer: 2000,
                                showConfirmButton: false
                            });
                            // 🔄 Recargar tabla
                            tabla.ajax.reload(null, false);
                        } else {
                            Swal.fire({
                                icon: 'error',
                                title: 'Error',
                                text: response.errorMessage || 'No se pudo eliminar el producto.'
                            });
                        }
                    },
                    error: function () {
                        Swal.fire({
                            icon: 'error',
                            title: 'Error',
                            text: 'Ocurrió un problema al intentar eliminar el producto.'
                        });
                    }
                });
            }
        });
    });
})