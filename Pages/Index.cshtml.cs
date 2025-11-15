using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CafeAlvoradaCSharp.Models;
using Microsoft.AspNetCore.Authorization; // Importante!

namespace CafeAlvoradaCSharp.Pages
{
    // [Authorize] é o substituto do seu protect.php
    // Isso garante que ninguém possa ver esta página sem estar logado.
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public string NomeUsuario { get; set; } = "Usuário";

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                // Isso não deve acontecer se [Authorize] estiver ativo,
                // mas é uma boa prática de segurança.
                return NotFound($"Não foi possível carregar o usuário com ID '{_userManager.GetUserId(User)}'.");
            }

            NomeUsuario = user.UserName ?? "Usuário";
            return Page();
        }
    }
}