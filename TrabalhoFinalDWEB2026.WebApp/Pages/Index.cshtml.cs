using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TrabalhoFinalDWEB2026.WebApp.Pages {
    public class IndexModel : PageModel {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger) {
            _logger = logger;
        }

        public void OnGet() {
            // Página inicial
        }
    }
}
