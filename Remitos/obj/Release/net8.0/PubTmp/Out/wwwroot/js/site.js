document.addEventListener('DOMContentLoaded', function () {
    // Configurar botón de Remito
    const btnRemito = document.getElementById("btnConfirmarGuardarRemito");
    if (btnRemito) {
        btnRemito.addEventListener("click", handleRemitoSubmit);
    }

    // Configurar botón de Nota
    const btnNota = document.getElementById("btnConfirmarGuardarSalida");
    if (btnNota) {
        btnNota.addEventListener("click", handleNotaSubmit);
    }

    function handleRemitoSubmit() {
        showConfirmationDialog(
            "Se guardará el remito con sus ítems.",
            "formCrearRemito"
        );
    }

    function handleNotaSubmit() {
        showConfirmationDialog(
            "Se guardará la nota con sus ítems.",
            "formCrearNota"
        );
    }

    function showConfirmationDialog(message, formId) {
        Swal.fire({
            title: '¿Confirmar guardado?',
            text: message,
            icon: 'question',
            showCancelButton: true,
            confirmButtonColor: '#28a745',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Sí, guardar',
            cancelButtonText: 'Cancelar'
        }).then((result) => {
            if (result.isConfirmed) {
                const tempButton = document.createElement('button');
                tempButton.type = 'submit';
                tempButton.formAction = '?handler=Guardar';
                tempButton.style.display = 'none';
                document.getElementById(formId).appendChild(tempButton);
                tempButton.click();
                tempButton.remove();
            }
        });
    }
});

document.getElementById("btnConfirmarGuardarSalida").addEventListener("click", function () {
    Swal.fire({
        title: '¿Confirmar guardado?',
        text: "Se guardará la nota con sus ítems.",
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#28a745',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Sí, guardar',
        cancelButtonText: 'Cancelar'
    }).then((result) => {
        if (result.isConfirmed) {
            // Crear un botón temporal para enviar el formulario con el handler Guardar
            const tempButton = document.createElement('button');
            tempButton.type = 'submit';
            tempButton.formAction = '?handler=Guardar';
            tempButton.style.display = 'none';

            document.getElementById('formCrearNota').appendChild(tempButton);
            tempButton.click();
            tempButton.remove();
        }
    });
});

