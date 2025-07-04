using Microsoft.AspNetCore.Mvc.RazorPages;
using Negocio;
using Dominio;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

public class CrearNotaSalidaModel : PageModel
{
    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    [BindProperty]
    public NotaSalida Nota { get; set; } = new();

    [BindProperty]
    public List<ItemSalida> Items { get; set; } = new();

    public List<SelectListItem> Autorizantes { get; set; } = new();
    public List<string> Tecnicos { get; set; } = new();

    public CrearNotaSalidaModel(AppDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public void OnGet()
    {
        Nota.Fecha = DateTime.Now;
        CargarListas();
        RecuperarItemsDeSession();
        if (Items.Count == 0) Items.Add(new ItemSalida());
        Items[0].Unidad = 1.ToString();
    }

    public IActionResult OnPostAddItem()
    {
        CargarListas();
     

        Items.Add(new ItemSalida{ Unidad = "1" });
 
        GuardarItemsEnSession();
        ModelState.Clear(); 
        return Page();
    }

    public IActionResult OnPostRemoveItem(int index)
    {
        CargarListas();
 

        if (index >= 0 && index < Items.Count)
            Items.RemoveAt(index);

        GuardarItemsEnSession();

        ModelState.Clear();

        return Page();
    }

    public async Task<IActionResult> OnPostGuardarAsync()
    {
        CargarListas();

        if (Items == null || Items.Count < 1)
        {
            ModelState.AddModelError("Nota.Items", "Debe agregar al menos un ítem de salida.");
        }

        for (int i = 0; i < Items.Count; i++)
        {
            if (string.IsNullOrWhiteSpace(Items[i].Equipo))
                ModelState.AddModelError($"Items[{i}].Equipo", "El campo Equipo es obligatorio.");
            if (string.IsNullOrWhiteSpace(Items[i].Serial))
                ModelState.AddModelError($"Items[{i}].Serial", "El campo Serial es obligatorio.");
        }
        // Asignar usuario logueado como autorizante
        var userIdClaim = User.FindFirst("UsuarioId")?.Value;
        if (int.TryParse(userIdClaim, out int usuarioId))
        {
            Nota.AutorizanteId = usuarioId;
        }
        else
        {
            ModelState.AddModelError("", "No se pudo identificar al usuario logueado.");
        }

        if (!ModelState.IsValid)
            return Page();

        _context.NotasSalida.Add(Nota);
        await _context.SaveChangesAsync();

        foreach (var item in Items)
        {
            item.NotaSalidaId = Nota.Id;
            _context.ItemsSalida.Add(item);
        }

        await _context.SaveChangesAsync();

        LimpiarSession();

        return RedirectToPage("VerNotaSalida", new { id = Nota.Id });
    }

    private void CargarListas()
    {
        if (!_httpContextAccessor.HttpContext.Session.TryGetValue("TecnicosJson", out _))
        {
            Tecnicos = _context.Usuarios.Select(u => u.Nombre).Distinct().ToList();
            _httpContextAccessor.HttpContext.Session.SetString("TecnicosJson", JsonConvert.SerializeObject(Tecnicos));
        }
        else
        {
            Tecnicos = JsonConvert.DeserializeObject<List<string>>(
                _httpContextAccessor.HttpContext.Session.GetString("TecnicosJson"));
        }

        if (!_httpContextAccessor.HttpContext.Session.TryGetValue("AutorizantesJson", out _))
        {
            Autorizantes = _context.Usuarios.Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = $"{u.Nombre} ({u.Legajo})"
            }).ToList();

            _httpContextAccessor.HttpContext.Session.SetString("AutorizantesJson", JsonConvert.SerializeObject(Autorizantes));
        }
        else
        {
            Autorizantes = JsonConvert.DeserializeObject<List<SelectListItem>>(
                _httpContextAccessor.HttpContext.Session.GetString("AutorizantesJson"));
        }
    }

    private void RecuperarItemsDeSession()
    {
        var json = _httpContextAccessor.HttpContext.Session.GetString("ItemsJson");
        if (!string.IsNullOrEmpty(json))
        {
            Items = JsonConvert.DeserializeObject<List<ItemSalida>>(json);
        }
    }

    private void GuardarItemsEnSession()
    {
        _httpContextAccessor.HttpContext.Session.SetString("ItemsJson", JsonConvert.SerializeObject(Items));
    }

    private void LimpiarSession()
    {
        _httpContextAccessor.HttpContext.Session.Remove("ItemsJson");
        _httpContextAccessor.HttpContext.Session.Remove("TecnicosJson");
        _httpContextAccessor.HttpContext.Session.Remove("AutorizantesJson");
    }
}
