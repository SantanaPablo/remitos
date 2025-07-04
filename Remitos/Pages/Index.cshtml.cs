using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Remitos.Pages
{
    public class IndexModel : PageModel
    {
        public IActionResult OnGet()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Salidas/Index");
            }

            return Page();
        }
    }
}
