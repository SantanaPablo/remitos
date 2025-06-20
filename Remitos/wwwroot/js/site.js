document.getElementById("btnConfirmarGuardar").addEventListener("click", function () {
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