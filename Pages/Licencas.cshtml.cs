using CafeAlvoradaCSharp.Data;
using CafeAlvoradaCSharp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace CafeAlvoradaCSharp.Pages
{
    // ViewModel para a tabela de licenças (com status calculado)
    public class LicencaViewModel : Licenca
    {
        [NotMapped]
        public string StatusNotificacao { get; set; } = "N/A";
        [NotMapped]
        public string StatusClass { get; set; } = "";
    }
    
    public class LicencasModel : PageModel
    {
        private readonly CafeAlvoradaContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public LicencasModel(CafeAlvoradaContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Propriedades para a Página
        public string NomeUsuario { get; set; } = "Usuário";
        public int TotalLicencas { get; set; }
        public List<LicencaViewModel> Licencas { get; set; } = new List<LicencaViewModel>();
        
        // Propriedades de Filtro
        [BindProperty(SupportsGet = true)]
        public string? FiltroLicenca { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? FiltroCnpj { get; set; }
        
        public SelectList NomesLicencasSL { get; set; }
        public SelectList CnpjsLicencasSL { get; set; }

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null) NomeUsuario = user.UserName ?? "Usuário";

            // Carregar dropdowns de filtro
            var nomesLicencas = await _context.Licencas.Select(l => l.NomeLicenca).Distinct().OrderBy(n => n).ToListAsync();
            NomesLicencasSL = new SelectList(nomesLicencas, FiltroLicenca);
            
            var cnpjsLicencas = await _context.Licencas.Select(l => l.Cnpj).Distinct().OrderBy(c => c).ToListAsync();
            CnpjsLicencasSL = new SelectList(cnpjsLicencas, FiltroCnpj);

            // Query base
            var query = _context.Licencas.AsQueryable();

            // Aplicar filtros
            if (!string.IsNullOrEmpty(FiltroLicenca))
            {
                query = query.Where(l => l.NomeLicenca == FiltroLicenca);
            }
            if (!string.IsNullOrEmpty(FiltroCnpj))
            {
                query = query.Where(l => l.Cnpj == FiltroCnpj);
            }

            var licencasDb = await query.OrderBy(l => l.DataValidade).ToListAsync();

            TotalLicencas = licencasDb.Count;

            // Calcular Status (lógica do PHP)
            var hoje = DateOnly.FromDateTime(DateTime.Now);
            foreach (var licenca in licencasDb)
            {
                var vm = new LicencaViewModel
                {
                    // Copia todas as propriedades da licença do BD
                    LicencaId = licenca.LicencaId,
                    NomeLicenca = licenca.NomeLicenca,
                    DataValidade = licenca.DataValidade,
                    OrgaoResponsavel = licenca.OrgaoResponsavel,
                    Cnpj = licenca.Cnpj,
                    PrazoExpiracao = licenca.PrazoExpiracao,
                    CaminhoDocumento = licenca.CaminhoDocumento
                };

                int diasRestantes = licenca.DataValidade.DayNumber - hoje.DayNumber;
                int.TryParse(licenca.PrazoExpiracao, out int prazoInt);

                if (licenca.DataValidade < hoje)
                {
                    vm.StatusNotificacao = "Vencida";
                    vm.StatusClass = "vencida";
                }
                else if (diasRestantes <= prazoInt)
                {
                    vm.StatusNotificacao = $"Faltam {diasRestantes} dias";
                    vm.StatusClass = "proximo-vencimento";
                }
                else
                {
                    vm.StatusNotificacao = "Ativa";
                    vm.StatusClass = "ativa";
                }
                Licencas.Add(vm);
            }
        }
    }
}