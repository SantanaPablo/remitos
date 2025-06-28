using Dominio;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Negocio;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class RemitoModel : PageModel
{
    private readonly AppDbContext _context;

    public RemitoModel(AppDbContext context)
    {
        _context = context;
    }

    public List<Remito> Remitos { get; set; }

    public int PaginaActual { get; set; } = 1;
    public int TotalPaginas { get; set; }
    public int TamañoPagina { get; set; } = 10;

    [BindProperty(SupportsGet = true)]
    public string Filtro { get; set; }

    public async Task OnGetAsync(int pagina = 1)
    {
        TamañoPagina = 10;
        PaginaActual = pagina;

        var query = _context.Remitos
            .Include(r => r.Items)
            .OrderByDescending(r => r.Fecha)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(Filtro))
        {
            query = query.Where(r =>
                r.Numero.Contains(Filtro) ||
                r.Destino.Contains(Filtro) ||
                r.Items.Any(i =>
                    i.descripcion.Contains(Filtro) ||
                    i.serial.Contains(Filtro) ||
                    i.usuario.Contains(Filtro))
            );
        }

        int totalRemitos = await query.CountAsync();
        TotalPaginas = (int)Math.Ceiling(totalRemitos / (double)TamañoPagina);

        Remitos = await query
            .Skip((PaginaActual - 1) * TamañoPagina)
            .Take(TamañoPagina)
            .ToListAsync();
    }
}
