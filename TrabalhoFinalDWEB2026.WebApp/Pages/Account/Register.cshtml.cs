using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TrabalhoFinalDWEB2026.WebApp.Models;

namespace TrabalhoFinalDWEB2026.WebApp.Pages.Account {
    [AllowAnonymous]
    public class RegisterModel : PageModel {
        private readonly UserManager<Utente> _userManager;
        private readonly SignInManager<Utente> _signInManager;
        private readonly RoleManager<IdentityRole<string>> _roleManager;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(
            UserManager<Utente> userManager,
            SignInManager<Utente> signInManager,
            RoleManager<IdentityRole<string>> roleManager,
            ILogger<RegisterModel> logger) {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public void OnGet() {
            // Implementação do método GET
        }

        public async Task<IActionResult> OnPostAsync() {
            if (ModelState.IsValid) {
                // Validação de idade
                var age = DateTime.Today.Year - Input.DataNascimento.Year;
                if (Input.DataNascimento.Date > DateTime.Today.AddYears(-age)) {
                    age--;
                }

                if (age < 13) {
                    ModelState.AddModelError("Input.DataNascimento", "Deve ter pelo menos 13 anos para se registar.");
                    return Page();
                }

                // Verificar se o utilizador já existe
                var existingUser = await _userManager.FindByNameAsync(Input.NumeroUtente);
                if (existingUser != null) {
                    ModelState.AddModelError("Input.NumeroUtente", "Este Número de Utente já está registado.");
                    return Page();
                }

                var user = new Utente {
                    Id = Input.NumeroUtente,
                    UserName = Input.NumeroUtente,
                    NumeroUtente = Input.NumeroUtente,
                    Nome = Input.Nome,
                    DataNascimento = Input.DataNascimento,
                    Email = Input.Email
                };

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded) {
                    // Garantir que o papel "Utente" existe
                    if (!await _roleManager.RoleExistsAsync("Utente")) {
                        await _roleManager.CreateAsync(new IdentityRole<string>("Utente"));
                    }
                    await _userManager.AddToRoleAsync(user, "Utente");

                    _logger.LogInformation("Novo utilizador registado: {NumeroUtente}", Input.NumeroUtente);

                    // Fazer login automático após registo
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToPage("/Index");
                }

                foreach (var error in result.Errors) {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }

        public class InputModel {
            [Required(ErrorMessage = "O número de utente é obrigatório.")]
            [StringLength(9, MinimumLength = 9, ErrorMessage = "O número de utente deve ter exatamente 9 dígitos.")]
            [Display(Name = "Número de Utente")]
            public string NumeroUtente { get; set; } = string.Empty;

            [Required(ErrorMessage = "O nome é obrigatório.")]
            [Display(Name = "Nome Completo")]
            public string Nome { get; set; } = string.Empty;

            [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
            [Display(Name = "Data de Nascimento")]
            public DateTime DataNascimento { get; set; }

            [Required(ErrorMessage = "O e-mail é obrigatório.")]
            [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
            [Display(Name = "E-mail")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "A palavra-passe é obrigatória.")]
            [StringLength(100, ErrorMessage = "A {0} deve ter pelo menos {2} e no máximo {1} caracteres.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Palavra-passe")]
            public string Password { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "As palavras-passe não coincidem.")]
            [Display(Name = "Confirmar Palavra-passe")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }
    }
}
