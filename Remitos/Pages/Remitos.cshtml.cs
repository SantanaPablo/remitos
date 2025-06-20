using Microsoft.AspNetCore.Mvc.RazorPages;
using Negocio;
using Dominio;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

public class RemitoModel : PageModel
{
    private readonly RemitoNegocio _remitoNegocio;

    public RemitoModel(RemitoNegocio remitoNegocio)
    {
        _remitoNegocio = remitoNegocio;
    }

    public List<Remito> Remitos { get; set; }

    [BindProperty(SupportsGet = true)]
    public string Filtro { get; set; }

    public void OnGet()
    {
        var remitos = _remitoNegocio.ObtenerTodos();
        Remitos = string.IsNullOrEmpty(Filtro)
            ? remitos
            : remitos.Where(r =>
                    r.Numero.Contains(Filtro) ||
                    r.Destino.Contains(Filtro))
                    .ToList();
    }
}