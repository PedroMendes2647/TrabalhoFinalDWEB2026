using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TrabalhoFinalDWEB2026.WebApp.Models;

namespace TrabalhoFinalDWEB2026.WebApp.Pages.MyAccount {
    [Authorize]
    public class EditPerfilModel : PageModel {
        private readonly UserManager<Utente> _userManager;
        private readonly ILogger<EditPerfilModel> _logger;

        public Utente? CurrentUser { get; set; }

        [BindProperty]
        public string Nome { get; set; } = string.Empty;

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        public DateTime DataNascimento { get; set; }

        public EditPerfilModel(
            UserManager<Utente> userManager,
            ILogger<EditPerfilModel> logger) {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task OnGetAsync() {
            CurrentUser = await _userManager.GetUserAsync(User);
            if (CurrentUser != null) {
                Nome = CurrentUser.Nome;
                Email = CurrentUser.Email ?? string.Empty;
                DataNascimento = CurrentUser.DataNascimento;
            }
        }

        public async Task<IActionResult> OnPostAsync(string id) {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.Id != id) {
                return Forbid();
            }

            if (!ModelState.IsValid) {
                CurrentUser = user;
                return Page();
            }

            user.Nome = Nome;
            user.Email = Email;
            user.DataNascimento = DataNascimento;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded) {
                _logger.LogInformation("Utilizador {NumeroUtente} atualizou o perfil",
                    user.NumeroUtente);
                return RedirectToPage("MyData");
            }

            foreach (var error in result.Errors) {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            CurrentUser = user;
            return Page();
        }
    }
}
