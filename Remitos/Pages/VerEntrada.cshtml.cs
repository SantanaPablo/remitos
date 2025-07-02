using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Dominio;
using Microsoft.AspNetCore.Mvc;
using Negocio;

public class VerEntradaModel : PageModel
{
    private readonly AppDbContext _context;

    public VerEntradaModel(AppDbContext context)
    {
        _context = context;
    }

    public NotaEntrada Nota { get; set; }
    public List<ItemEntrada> Items { get; set; }
    public Usuario Usuario { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Nota = await _context.NotasEntrada
            .Include(n => n.Autorizante)
            .FirstOrDefaultAsync(n => n.Id == id);

        if (Nota == null)
            return NotFound();

        Usuario = Nota.Autorizante;
        Items = await _context.ItemsEntrada
            .Where(i => i.NotaEntradaId == id)
            .ToListAsync();

        return Page();
    }

    //public IActionResult OnPostGenerarPdf(int id)
    //{
    //    var nota = _context.NotasEntrada
    //        .Include(n => n.Autorizante)
    //        .FirstOrDefault(n => n.Id == id);

    //    if (nota == null)
    //        return NotFound();

    //    var items = _context.ItemsEntrada
    //        .Where(i => i.NotaEntradaId == id)
    //        .ToList();

    //    string rutaPlantilla = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pdf", "nota_salida.pdf");
    //    var generador = new GenerarPDFSalida();
    //    var pdfBytes = generador.GenerarRemitoEnMemoria(nota, items, rutaPlantilla);

    //    string nombreArchivo = $"Remito_{nota.Fecha:yyyyMMdd}_{nota.Id}.pdf";
    //    return File(pdfBytes, "application/pdf", nombreArchivo);
    //}
}