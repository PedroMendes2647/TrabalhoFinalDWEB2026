using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrabalhoFinalDWEB2026.WebApp.Data;
using TrabalhoFinalDWEB2026.WebApp.Models;

namespace TrabalhoFinalDWEB2026.WebApp.Pages.Management {
    [Authorize(Roles = "Doutor,Farmaceuta")]
    public class ViewUtentesModel : PageModel {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Utente> _userManager;
        private readonly ILogger<ViewUtentesModel> _logger;

        public List<Utente>? Utentes { get; set; }

        public ViewUtentesModel(
            ApplicationDbContext context,
            UserManager<Utente> userManager,
            ILogger<ViewUtentesModel> logger) {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task OnGetAsync() {
            var currentUser = await _userManager.GetUserAsync(User);

            // Get all users that have the Utente role
            var utenteRole = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == "Utente");

            if (utenteRole != null) {
                Utentes = await _context.UserRoles
                    .Where(ur => ur.RoleId == utenteRole.Id)
                    .Join(
                        _context.Utentes,
                        ur => ur.UserId,
                        u => u.Id,
                        (ur, u) => u)
                    .OrderBy(u => u.Nome)
                    .ToListAsync();
            }
            else {
                Utentes = new List<Utente>();
            }

            if (currentUser != null) {
                _logger.LogInformation("Utilizador {NumeroUtente} visualizou a lista de utentes",
                    currentUser.NumeroUtente);
            }
        }
    }
}
