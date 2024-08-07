$(document).ready(function () {

    $("#frmCliente").validate({
        rules: {
            txtidentificacion: {
                required: true,
                maxlength: 14,
                maxlength: 14
            },
            cbtipo: {
                required: true
            },
            txtprimer_nombre: {
                required: true,
                maxlength: 50
            },
            txtprimer_apellido: {
                required: true,
                maxlength: 50
            },
            cbdireccion: {
                required: true
            }
        },
        messages: {
            txtidentificacion: {
                required: "Por favor, ingrese una identificación.",
                minlength: "La identificación debe tener minimo 14 caracteres."

                maxlength: "La identificación debe tener máximo 16 caracteres."
            },
            cbtipo: {
                required: "Por favor, seleccione un tipo de identificación."
            },
            txtprimer_nombre: {
                required: "Por favor, ingrese el primer nombre.",
                maxlength: "El primer nombre debe tener máximo 50 caracteres."
            },
            txtprimer_apellido: {
                required: "Por favor, ingrese el primer apellido.",
                maxlength: "El primer apellido debe tener máximo 50 caracteres."
            },
            cbdireccion: {
                required: "Por favor, seleccione una dirección."
            }
        },
        submitHandler: function (form) {
            var oForm = $(form).serialize();
            $.ajax({
                type: "POST",
                url: "/Client/AccionCliente",
                data: oForm,
                success: function () {
                    window.location.reload(); // Recargar la página
                },
                error: function (xhr, status, error) {
                    console.error('Error en la solicitud AJAX:', error);
                }
            });
        }
    });

    function resetForm() {
        $('#frmCliente')[0].reset();
        $('#frmCliente input, #frmCliente select').removeAttr('readonly');
        $('#btguardar').removeAttr('disabled');
    }

    function disableForm() {
        $('#frmCliente input, #frmCliente select').attr('readonly', true);
        $('#btguardar').attr('disabled', true);
    }

    function enableForm() {
        $('#frmCliente input, #frmCliente select').attr('readonly', false);
        $('#btguardar').attr('disabled', false);
    }

    $('#btn-agregar').click(function () {
        resetForm();
    });

    $('.btnedit').click(function () {
        var data = $(this).data();
        $("#id").val(data.id);
        $("#txtidentificacion").val(data.identificacion);
        $("#cbtipo").val(data.tipo).change();
        $("#txtprimer_nombre").val(data.pnombre);
        $("#txtsegundo_nombre").val(data.snombre);
        $("#txtprimer_apellido").val(data.papellido);
        $("#txtsegundo_apellido").val(data.sapellido);
        $("#cbdireccion").val(data.cbdireccion).change();
        $("#txtestado").val(data.estado);

        if (data.estado === 'Inactivo') {
            disableForm();
        } else {
            enableForm();
        }
    });
 
    $(".btnbaja").click(function () {
        var clientId = $(this).data('id'); // Obtén el ID del botón

        Swal.fire({
            title: "¿Esta seguro que desea dar de baja a este cliente?",
            showDenyButton: true,
            confirmButtonText: "SI",
            denyButtonText: `No`
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    type: "POST",
                    url: "/Client/BajaCliente",
                    data: JSON.stringify({ id: clientId }),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        if (response.success) {
                            window.location.reload(); // Recargar la página
                        } else {
                            Swal.fire('Error', response.message, 'error');
                        }                    },
                    error: function (xhr, status, error) {
                        console.error('Error en la solicitud AJAX:', error);
                    }
                });
            }
        });
    });
});
