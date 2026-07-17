using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TrabalhoFinalDWEB2026.Models;

namespace TrabalhoFinalDWEB2026.Controllers {
    [AllowAnonymous]
    public class HomeController : Controller {
        /// <summary>
        /// Página inicial da aplicação
        /// </summary>
        public IActionResult Index() {
            return View();
        }

        /// <summary>
        /// Página de política de privacidade
        /// </summary>
        public IActionResult Privacy() {
            return View();
        }

        /// <summary>
        /// Página informativa sobre a aplicação
        /// </summary>
        public IActionResult About() {
            return View();
        }

        /// <summary>
        /// Página de erro da aplicação com rastreamento de requisição
        /// </summary>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
