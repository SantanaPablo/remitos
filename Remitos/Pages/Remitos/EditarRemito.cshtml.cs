using Dominio;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Negocio;
using Newtonsoft.Json;

public class EditarRemitoModel : PageModel
{
    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public EditarRemitoModel(AppDbContext context, IHttpContextAccessor accessor)
    {
        _context = context;
        _httpContextAccessor = accessor;
    }

    [BindProperty]
    public Remito Remito { get; set; }

    [BindProperty]
    public List<ItemRemito> Items { get; set; } = new();

    public IActionResult OnGet(int id)
    {
        Remito = _context.Remitos.Find(id);
        if (Remito == null)
            return NotFound();

        Items = _context.ItemsRemito
            .Where(i => i.id_remito == id)
            .OrderBy(i => i.numero_item)
            .ToList();

        //RecuperarItemsDeSession();
        return Page();
    }

    public IActionResult OnPostAddItem()
    {
        //RecuperarItemsDeSession();
        Items.Add(new ItemRemito
        {
            cantidad = 1,
            numero_item = Items.Count + 1
        });

        //GuardarItemsEnSession();
        ModelState.Clear();
        return Page();
    }

    public IActionResult OnPostRemoveItem(int index)
    {
        //RecuperarItemsDeSession();

        if (index >= 0 && index < Items.Count)
            Items.RemoveAt(index);

        for (int i = 0; i < Items.Count; i++)
        {
            Items[i].numero_item = i + 1;
        }

        //GuardarItemsEnSession();
        ModelState.Clear();
        return Page();
    }

    public async Task<IActionResult> OnPostGuardarAsync()
    {

        if (string.IsNullOrWhiteSpace(Remito.Numero))
            ModelState.AddModelError("Remito.Numero", "El número de remito es obligatorio");

        if (string.IsNullOrWhiteSpace(Remito.Destino))
            ModelState.AddModelError("Remito.Destino", "El destino es obligatorio");

        if (Items == null || Items.Count < 1)
        {
            ModelState.AddModelError("", "Debe agregar al menos un ítem.");
        }

        for (int i = 0; i < Items.Count; i++)
        {
            if (string.IsNullOrWhiteSpace(Items[i].descripcion))
                ModelState.AddModelError($"Items[{i}].descripcion", "La descripción es obligatoria.");

            if (Items[i].cantidad <= 0)
                ModelState.AddModelError($"Items[{i}].cantidad", "La cantidad debe ser mayor a cero.");
        }

        if (!ModelState.IsValid)
            return Page();

        try
        {
            // Actualizar remito
            _context.Attach(Remito).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // Eliminar ítems anteriores
            var existentes = _context.ItemsRemito.Where(x => x.id_remito == Remito.Id);
            _context.ItemsRemito.RemoveRange(existentes);
            await _context.SaveChangesAsync();

            // Insertar ítems nuevos
            foreach (var item in Items)
            {
                item.id_remito = Remito.Id;
                _context.ItemsRemito.Add(item);
            }

            await _context.SaveChangesAsync();
            LimpiarSession();

            return RedirectToPage("VerDetalleRemito", new { id = Remito.Id });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Error al guardar los cambios: " + ex.Message);
            return Page();
        }
    }

    private void GuardarItemsEnSession()
    {
        var json = JsonConvert.SerializeObject(Items);
        _httpContextAccessor.HttpContext.Session.SetString("ItemsEditarRemito", json);
    }

    private void RecuperarItemsDeSession()
    {
        var json = _httpContextAccessor.HttpContext.Session.GetString("ItemsEditarRemito");
        if (!string.IsNullOrEmpty(json))
            Items = JsonConvert.DeserializeObject<List<ItemRemito>>(json);
    }

    private void LimpiarSession()
    {
        _httpContextAccessor.HttpContext.Session.Remove("ItemsEditarRemito");
    }
}