using Dominio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Negocio;
using System.Collections.Generic;
using System.Threading.Tasks;
public class SalidasModel : PageModel
{
    private readonly AppDbContext _context;

    public SalidasModel(AppDbContext context)
    {
        _context = context;
    }

    public List<NotaSalida> Notas { get; set; }
    public Usuario Usuario { get; set; }

    public int PaginaActual { get; set; } = 1;
    public int TotalPaginas { get; set; }
    public int TamañoPagina { get; set; } = 10;

    [BindProperty(SupportsGet = true)]
    public string Filtro { get; set; }

    public async Task OnGetAsync(int pagina = 1)
    {
        TamañoPagina = 10;
        PaginaActual = pagina;

        var query = _context.NotasSalida
            .Include(n => n.Autorizante)
            .Include(n => n.Items)
            .OrderByDescending(n => n.Fecha)
            .AsQueryable();
            

        if (!string.IsNullOrWhiteSpace(Filtro))
        {
            query = query.Where(n =>
                n.DirigidaA.Contains(Filtro) ||
                n.Tecnico.Contains(Filtro) ||
                n.Autorizante.Nombre.Contains(Filtro) ||
                n.Items.Any(i => i.Equipo.Contains(Filtro) || i.Usuario.Contains(Filtro) || i.Serial.Contains(Filtro))
            );
        }

        int totalNotas = await query.CountAsync();
        TotalPaginas = (int)Math.Ceiling(totalNotas / (double)TamañoPagina);

        Notas = await query
            .OrderByDescending(n => n.Fecha)
            .Skip((PaginaActual - 1) * TamañoPagina)
            .Take(TamañoPagina)
            .ToListAsync();
    }

    //public IActionResult OnPostGenerarPdf(int notaId)
    //{
    //    var nota = _context.NotasSalida
    //    .Include(n => n.Autorizante)
    //    .FirstOrDefault(n => n.Id == notaId);
    //    if (nota == null) return NotFound();

    //    var items = _context.ItemsSalida.Where(i => i.NotaSalidaId == notaId).ToList();

    //    string rutaPlantilla = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot", "pdf", "nota_salida.pdf");
    //    var generador = new GenerarPDFSalida();
    //    Usuario = nota.Autorizante;
    //    byte[] pdfBytes = generador.GenerarRemitoEnMemoria(nota, items, rutaPlantilla);

    //    string nombreArchivo = $"Remito_{nota.Fecha:yyyyMMdd}_{nota.Id}.pdf";
    //    return File(pdfBytes, "application/pdf", nombreArchivo);
    //}
   
}
