using CafeAlvoradaCSharp.Data;
using CafeAlvoradaCSharp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CafeAlvoradaCSharp.Pages
{
    public class CadastroLicencaModel : PageModel
    {
        private readonly CafeAlvoradaContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public CadastroLicencaModel(CafeAlvoradaContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment; // Serviço para descobrir o caminho da pasta wwwroot
        }

        public string NomeUsuario { get; set; } = "Usuário";
        
        [BindProperty]
        public Licenca NovaLicenca { get; set; }
        
        [BindProperty]
        public IFormFile? DocumentoUpload { get; set; } // Propriedade para receber o arquivo
        
        [TempData]
        public string MensagemFeedback { get; set; }

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null) NomeUsuario = user.UserName ?? "Usuário";
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Lógica de Upload de Arquivo
            if (DocumentoUpload != null && DocumentoUpload.Length > 0)
            {
                try
                {
                    // Caminho da pasta de upload (dentro de wwwroot)
                    string pastaUpload = Path.Combine(_environment.WebRootPath, "uploads", "licencas");
                    
                    // Cria o diretório se não existir
                    if (!Directory.Exists(pastaUpload))
                    {
                        Directory.CreateDirectory(pastaUpload);
                    }

                    // Gera um nome de arquivo único
                    string nomeArquivoUnico = Guid.NewGuid().ToString() + "_" + DocumentoUpload.FileName;
                    string caminhoCompleto = Path.Combine(pastaUpload, nomeArquivoUnico);

                    // Salva o arquivo no disco
                    await using (var fileStream = new FileStream(caminhoCompleto, FileMode.Create))
                    {
                        await DocumentoUpload.CopyToAsync(fileStream);
                    }
                    
                    // Salva o caminho RELATIVO no banco (para ser usado no download)
                    NovaLicenca.CaminhoDocumento = Path.Combine("uploads", "licencas", nomeArquivoUnico);
                }
                catch (Exception ex)
                {
                    MensagemFeedback = "Erro ao fazer upload do arquivo: " + ex.Message;
                    return Page();
                }
            }

            // Salva a licença no banco
            try
            {
                _context.Licencas.Add(NovaLicenca);
                await _context.SaveChangesAsync();
                
                // Redireciona com mensagem de sucesso
                TempData["MensagemFeedback"] = "Licença cadastrada com sucesso!";
                return RedirectToPage("/Licencas");
            }
            catch (Exception ex)
            {
                MensagemFeedback = "Erro ao salvar no banco: " + ex.Message;
                return Page();
            }
        }
    }
}