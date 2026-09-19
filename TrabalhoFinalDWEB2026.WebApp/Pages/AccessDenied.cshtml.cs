using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TrabalhoFinalDWEB2026.WebApp.Pages {
    public class AccessDeniedModel : PageModel {
        private readonly ILogger<AccessDeniedModel> _logger;

        public AccessDeniedModel(ILogger<AccessDeniedModel> logger) {
            _logger = logger;
        }

        public void OnGet() {
            _logger.LogWarning("Utilizador tentou aceder a um recurso sem permissão. IP: {RemoteIP}", HttpContext.Connection.RemoteIpAddress);
        }
    }
}
