using iText.Kernel.Pdf;
using iText.Forms;
using iText.Forms.Fields;
using System;
using System.Collections.Generic;
using System.IO;
using Dominio;

public class GenerarReportes
{
    public void GenerarPdfRemito(Remito remito, List<ItemRemito> items, string rutaPlantilla, string rutaDestino)
    {
        try
        {
            using var reader = new PdfReader(rutaPlantilla);
            using var writer = new PdfWriter(rutaDestino);
            using var pdfDoc = new PdfDocument(reader, writer);

            var form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            var fields = form.GetAllFormFields();

            // Campos básicos del remito
            SetField(fields, "numero", remito.Numero.ToString());
            SetField(fields, "fecha", remito.Fecha.ToString("dd/MM/yyyy"));
            SetField(fields, "destino", remito.Destino);

            // Llenar campos de ítems (hasta 10 por ejemplo)
            for (int i = 0; i < items.Count && i < 10; i++)
            {
                var item = items[i];
                SetField(fields, $"descripcion", item.descripcion);
                SetField(fields, $"serie", item.descripcion);
                SetField(fields, $"usuario", item.usuario);
                SetField(fields, $"cantidad", item.cantidad.ToString());
                SetField(fields, $"detalle", item.detalle);
                SetField(fields, $"recibido", item.recibido_por);
            }

            form.FlattenFields(); // Opcional: bloquea campos para que no puedan editarse
        }
        catch (Exception ex)
        {
            File.WriteAllText("pdf_error.log", $"Error al generar PDF: {ex}");
            throw;
        }
    }

    private void SetField(IDictionary<string, PdfFormField> fields, string name, string value)
    {
        if (fields.ContainsKey(name))
        {
            fields[name].SetValue(value ?? "");
        }
        else
        {
            // Por si olvidaste crear algún campo en la plantilla
            File.AppendAllText("pdf_error.log", $"Campo no encontrado: {name}\n");
        }
    }

    public byte[] GenerarRemitoEnMemoria(NotaSalida nota, List<ItemSalida> items, string rutaPlantilla)
    {
        using var plantillaStream = File.OpenRead(rutaPlantilla);
        using var reader = new PdfReader(plantillaStream);
        using var memoryStream = new MemoryStream();
        using var writer = new PdfWriter(memoryStream);
        using var pdfDoc = new PdfDocument(reader, writer);
        var form = PdfAcroForm.GetAcroForm(pdfDoc, true);

        // Llenar campos fijos
        SafeSet(form, "Fecha", nota.Fecha.ToShortDateString());
        SafeSet(form, "Tecnico", nota.Tecnico);
        SafeSet(form, "Autorizante", nota.Autorizante.Nombre);
        SafeSet(form, "Recibido", nota.Recibido ? "Sí" : "No");
        SafeSet(form, "DirigidoA", nota.DirigidaA);

        // Llenar ítems (hasta 10)
        for (int i = 0; i < items.Count && i < 10; i++)
        {
            int fila = i + 1;
            var item = items[i];

            SafeSet(form, $"Unidad{fila}", item.Unidad.ToString());
            SafeSet(form, $"Equipo{fila}", item.Equipo);
            SafeSet(form, $"Serial{fila}", item.Serial);
            SafeSet(form, $"Usuario{fila}", item.Usuario);
            SafeSet(form, $"SD{fila}", item.SD);
        }

        form.FlattenFields(); // Opcional
        pdfDoc.Close(); // Finaliza el documento

        return memoryStream.ToArray();
    }

    private void SafeSet(PdfAcroForm form, string fieldName, string value)
    {
        var fields = form.GetAllFormFields();
        if (fields.ContainsKey(fieldName))
        {
            fields[fieldName].SetValue(value ?? "");
        }
        else
        {
            File.AppendAllText("errores_pdf.log", $"Campo no encontrado: {fieldName}\n");
        }
    }
}
