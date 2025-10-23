$(document).ready(function () {
    // Guardamos la URL base del controlador
    window.Componente = {
        UrlControlador: "/Producto/"
    };

    // -----------------------------
    // 🟢 BOTÓN GUARDAR PRODUCTO
    // -----------------------------
    $('#btnGuardarProducto').on('click', function (e) {
        e.preventDefault();

        // Creamos un objeto con los datos del formulario
        const producto = {
            ProductoId: $('#ProductoId').val() || null, // por si es edición
            Nombre: $('#Nombre').val(),
            Precio: $('#Precio').val()
        };

        // Validación simple en cliente
        if (!producto.Nombre || producto.Nombre.trim() === "") {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'Ingrese un nombre al producto.'
            })
            //swal("Error", "Debe ingresar un nombre de producto.", "error");
            return;
        }
        if (!producto.Precio || producto.Precio <= 0) {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'El precio debe ser un número positivo.'
            })
            //swal("Error", "El precio debe ser un número positivo.", "error");
            return;
        }
       
        //// Enviamos los datos al servidor
        $.ajax({
            url: Componente.UrlControlador + "Guardar",
            type: "POST",
            data:producto,
            success: function (response) {
                if (response.estado) {
                    //alert(response.estado)
                    
                 
                    Swal.fire({

                        icon: "success",
                        title: "Your work has been saved",
                        showConfirmButton: false,
                        timer: 1500
                    }).then(() => {
                        $("#TablaProducto").DataTable().ajax.reload();
                        regresar();
                    })
                } else {
                    //alert('aca')
                    // Mostrar errores de validación si vienen del servidor
                    $.each(response.formErrors, function () {
                        $("[data-valmsg-for=" + this.key + "]").html(this.errors.join());
                    });
                    //if (response.formErrors) {
                    //    let mensajes = "";
                    //    $.each(response.formErrors, function (index, item) {
                    //        mensajes += item.errors.join("<br>");
                    //    });
                    //    swal("Errores de validación", mensajes, "warning");
                    //} else {
                    //    swal("Error", response.error || "No se pudo guardar el producto.", "error");
                    //}
                }
            },
            error: function () {
                swal("Error", "Ocurrió un error al intentar guardar el producto.", "error");
            }
        });
    });


    // -----------------------------
    // 🔴 BOTÓN REGRESAR
    // -----------------------------
    $('#btnRegresarProducto').on('click', function () {
        //$('#div-tabla-productos').show();
        //$('#form-producto').hide();
        regresar();
    });


    // -----------------------------
    // 🟡 BOTÓN EDITAR
    // -----------------------------
    $(document).on('click', '.editar', function () {
        const id = $(this).attr('id');

        //// Cargamos el formulario con los datos existentes
        //$.get(Componente.UrlControlador + "Formulario", function (html) {
        //    $('#form-producto').show();
        //    $('#form-producto').html(html);
        //    $('#div-tabla-productos').hide();

        //    // Buscamos los datos del producto en la tabla
        //    const data = $("#TablaProducto").DataTable().row($(this).parents('tr')).data();

        //    // Rellenamos los campos del formulario
        //    $('#ProductoId').val(data.productoId);
        //    $('#Nombre').val(data.nombre);
        //    $('#Precio').val(data.precio);
        //});
    });

    const regresar = () => {
        $('#div-tabla-productos').show();
        $('#form-producto').hide();
        $('#form-producto').html('');
    }
})
