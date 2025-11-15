using CafeAlvoradaCSharp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CafeAlvoradaCSharp.Pages
{
    // Esta página não tem HTML, ela apenas retorna um arquivo.
    public class DownloadLicencaModel : PageModel
    {
        private readonly CafeAlvoradaContext _context;
        private readonly IWebHostEnvironment _environment;

        public DownloadLicencaModel(CafeAlvoradaContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var licenca = await _context.Licencas.FindAsync(id);

            if (licenca == null || string.IsNullOrEmpty(licenca.CaminhoDocumento))
            {
                return NotFound("Licença ou documento não encontrado.");
            }

            try
            {
                // Constrói o caminho absoluto do arquivo (ex: C:\projeto\wwwroot\uploads\licencas\arquivo.pdf)
                string caminhoAbsoluto = Path.Combine(_environment.WebRootPath, licenca.CaminhoDocumento);

                if (!System.IO.File.Exists(caminhoAbsoluto))
                {
                    return NotFound("Arquivo não encontrado no servidor.");
                }

                // Lê os bytes do arquivo
                var fileBytes = await System.IO.File.ReadAllBytesAsync(caminhoAbsoluto);
                
                // Retorna o arquivo para o navegador (força o download)
                return File(fileBytes, "application/octet-stream", Path.GetFileName(caminhoAbsoluto));
            }
            catch (Exception)
            {
                return new ContentResult { Content = "Erro ao ler o arquivo.", StatusCode = 500 };
            }
        }
    }
}