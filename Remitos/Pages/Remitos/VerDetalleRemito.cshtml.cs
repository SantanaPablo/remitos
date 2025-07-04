using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
using System;
using Negocio;
using Dominio;
using Microsoft.AspNetCore.Hosting;

public class DetalleModel : PageModel
{
    private readonly RemitoNegocio _remitoNegocio;
    private readonly ItemRemitoNegocio _itemRemitoNegocio;
    private readonly GenerarReportes _generarReportes;
    private readonly IWebHostEnvironment _env;

    public DetalleModel(
        RemitoNegocio remitoNegocio,
        ItemRemitoNegocio itemRemitoNegocio,
        GenerarReportes generarReportes,
        IWebHostEnvironment env)
    {
        _remitoNegocio = remitoNegocio;
        _itemRemitoNegocio = itemRemitoNegocio;
        _generarReportes = generarReportes;
        _env = env;
    }

    public Remito Remito { get; set; }
    public List<ItemRemito> Items { get; set; }

    public IActionResult OnGet(int id)
    {
        Remito = _remitoNegocio.ObtenerPorId(id);
        if (Remito == null) return NotFound();

        Items = _itemRemitoNegocio.ObtenerPorRemitoId(id);
        return Page();
    }

    public IActionResult OnGetVerPdf(int id)
    {
        var remito = _remitoNegocio.ObtenerPorId(id);
        var items = _itemRemitoNegocio.ObtenerPorRemitoId(id);

        if (remito == null) return NotFound();

        // Usar ruta dentro de wwwroot para acceso directo
        var relativePath = $"/pdf/remito_{id}.pdf";
        var generadoPath = Path.Combine(_env.WebRootPath, "pdf", $"remito_{id}.pdf");
        var plantillaPath = Path.Combine(_env.WebRootPath, "pdf", "plantilla_remito.pdf");

        // Crear directorio si no existe
        var pdfDir = Path.Combine(_env.WebRootPath, "pdf");
        if (!Directory.Exists(pdfDir)) Directory.CreateDirectory(pdfDir);

        // Generar el PDF
        _generarReportes.GenerarPdfRemito(remito, items, plantillaPath, generadoPath);

        // Devolver el archivo directamente
        return PhysicalFile(generadoPath, "application/pdf", $"Remito_{remito.Numero}.pdf");
    }
}