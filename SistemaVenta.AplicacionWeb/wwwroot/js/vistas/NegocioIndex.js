$(document).ready(function () {

    $(".card-body").LoadingOverlay("show");

    fetch("/Negocio/Obtener")
        .then(response => {
            $(".card-body").LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            console.log(responseJson)
            if (responseJson.estado) {
                const res = responseJson.objeto

                $("#txtNumeroDocumento").val(res.numeroDocumento);
                $("#txtRazonSocial").val(res.nombre)
                $("#txtCorreo").val(res.correo)
                $("#txtDireccion").val(res.direccion)
                $("#txtTelefono").val(res.telefono)
                $("#txtImpuesto").val(res.porcentajeImpuesto)
                $("#txtSimboloMoneda").val(res.simboloMoneda)
                $("#txtLogo").attr("src", res.urlLogo)
            } else {
                swal("Lo sentimos", responseJson.mensaje, "error");
            }
        })
        .catch(error => {
            console.error("Error fetching roles:", error);
        });
})

$("#btnGuardarCambios").click(function () {
    const inputs = $('input.input-validar').serializeArray();
    const inputsSinValor = inputs.filter(input => input.value.trim() === '');

    if (inputsSinValor.length > 0) {
        const mensaje = `Debe completar el campo ${inputsSinValor[0].name}`;
        toastr.warning("", mensaje);
        $(`input[name="${inputsSinValor[0].name}"]`).focus();
        return;
    }
    const modelo = {
        numeroDocumento: $("#txtNumeroDocumento").val(),
        nombre: $("#txtRazonSocial").val(),
        correo: $("#txtCorreo").val(),
        direccion: $("#txtDireccion").val(),
        telefono: $("#txtTelefono").val(),
        porcentajeImpuesto: $("#txtImpuesto").val(),
        simboloMoneda: $("#txtSimboloMoneda").val(),
    }
    const inputFoto = document.getElementById('txtLogo');
    const formData = new FormData();

    formData.append('modelo', JSON.stringify(modelo));
    formData.append('logo', inputFoto.files[0]);

    $(".card-body").LoadingOverlay("show");
    fetch("/Negocio/GuardarCambios", {
        method: 'POST',
        body: formData,
    })
        .then(response => {
            $(".card-body").LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.estado) {
                const res = responseJson.objeto
                $("#imgLogo").attr("src", res.urlLogo)

            } else {
                swal("Lo sentimos", responseJson.mensaje, "error");
            }
        })
        .catch(error => {
            console.error("Error fetching roles:", error);
        });
})