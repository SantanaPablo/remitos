using Dominio;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Negocio;

public class LoginModel : PageModel
{
    private readonly AppDbContext _context;

    public LoginModel(AppDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        public string Legajo { get; set; }
        public string Contrasena { get; set; }
    }
    public IActionResult OnGet()
    {
        if (User.Identity.IsAuthenticated)
        {
            return RedirectToPage("/Salidas/Index");
        }

        return Page();
    }
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var usuario = _context.Usuarios.FirstOrDefault(u =>
            u.Legajo == Input.Legajo && u.Contrasena == Input.Contrasena);

        if (usuario == null)
        {
            ModelState.AddModelError(string.Empty, "Nombre o contraseña incorrectos");
            return Page();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, usuario.Nombre),
            new Claim("UsuarioId", usuario.Id.ToString()),
            new Claim("Legajo", usuario.Legajo ?? "")
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return RedirectToPage("/Salidas/Index");
    }
}
