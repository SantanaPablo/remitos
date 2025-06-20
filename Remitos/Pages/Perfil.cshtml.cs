using Dominio;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Negocio;
using System.Security.Claims;

public class PerfilModel : PageModel
{
    private readonly AppDbContext _context;

    public PerfilModel(AppDbContext context)
    {
        _context = context;
    }

    public string NombreUsuario { get; set; }
    public string Legajo { get; set; }

    [BindProperty]
    public CambiarContrasenaModel Cambio { get; set; } = new();

    public class CambiarContrasenaModel
    {
        public string ContrasenaActual { get; set; }
        public string NuevaContrasena { get; set; }
        public string RepetirContrasena { get; set; }
    }

    public void OnGet()
    {
        NombreUsuario = User.Identity?.Name ?? "";
        Legajo = User.Claims.FirstOrDefault(c => c.Type == "Legajo")?.Value ?? "";
    }

    public IActionResult OnPost()
    {
        NombreUsuario = User.Identity?.Name ?? "";
        Legajo = User.Claims.FirstOrDefault(c => c.Type == "Legajo")?.Value ?? "";

        if (!ModelState.IsValid)
            return Page();

        var usuarioIdStr = User.Claims.FirstOrDefault(c => c.Type == "UsuarioId")?.Value;
        if (!int.TryParse(usuarioIdStr, out int usuarioId))
        {
            ModelState.AddModelError("", "No se pudo validar el usuario.");
            return Page();
        }

        var usuario = _context.Usuarios.FirstOrDefault(u => u.Id == usuarioId);
        if (usuario == null)
        {
            ModelState.AddModelError("", "Usuario no encontrado.");
            return Page();
        }

        if (usuario.Contrasena != Cambio.ContrasenaActual)
        {
            ModelState.AddModelError("Cambio.ContrasenaActual", "La contraseña actual es incorrecta.");
            return Page();
        }

        if (Cambio.NuevaContrasena != Cambio.RepetirContrasena)
        {
            ModelState.AddModelError("Cambio.RepetirContrasena", "Las contraseñas nuevas no coinciden.");
            return Page();
        }

        usuario.Contrasena = Cambio.NuevaContrasena;
        _context.SaveChanges();

        TempData["MensajeExito"] = "Contraseña actualizada correctamente.";
        return RedirectToPage();
    }
}
