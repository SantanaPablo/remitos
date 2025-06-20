using Dominio;
using iText;
using iText.Forms;
using iText.Kernel.Pdf;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using System.IO;
using System.Threading.Tasks;

public class GenerarPDFSalida 
{
    public byte[] GenerarRemitoEnMemoria(NotaSalida nota, List<ItemSalida> items, string rutaPlantilla)
    {
        using var plantillaStream = File.OpenRead(rutaPlantilla);
        using var reader = new PdfReader(plantillaStream);
        using var memoryStream = new MemoryStream();
        using var writer = new PdfWriter(memoryStream);
        using var pdfDoc = new PdfDocument(reader, writer);
        var form = PdfAcroForm.GetAcroForm(pdfDoc, true);
       

        SafeSet(form, "fecha", nota.Fecha.ToShortDateString());
        SafeSet(form, "tecnico", nota.Tecnico);
        SafeSet(form, "autorizante", nota.Autorizante.Nombre);
        SafeSet(form, "legajo", nota.Autorizante.Legajo);
        SafeSet(form, "recibido", nota.Recibido ? "Sí" : "No");
        SafeSet(form, "destinatario", nota.DirigidaA);

        for (int i = 0; i < items.Count && i < 10; i++)
        {
            int fila = i + 1;
            var item = items[i];

            SafeSet(form, $"unidad{fila}", item.Unidad.ToString());
            SafeSet(form, $"equipo{fila}", item.Equipo);
            SafeSet(form, $"serial{fila}", item.Serial);
            SafeSet(form, $"usuario{fila}", item.Usuario);
            SafeSet(form, $"sd{fila}", item.SD);
        }

        form.FlattenFields();
        pdfDoc.Close();
        return memoryStream.ToArray();
    }

    private void SafeSet(PdfAcroForm form, string fieldName, string value)
    {
        var fields = form.GetAllFormFields();
        if (fields.ContainsKey(fieldName))
        {
            var field = fields[fieldName];
            var font = PdfFontFactory.CreateFont(StandardFonts.COURIER);
            field.SetFont(font);
            field.SetFontSize(11);
            fields[fieldName].SetValue(value ?? "");
        }
        else
        {
            File.AppendAllText("errores_pdf.log", $"Campo no encontrado: {fieldName}\n");
        }
    }
}
  

