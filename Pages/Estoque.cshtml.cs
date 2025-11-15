using CafeAlvoradaCSharp.Data;
using CafeAlvoradaCSharp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CafeAlvoradaCSharp.Pages
{
    public class EstoqueModel : PageModel
    {
        private readonly CafeAlvoradaContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EstoqueModel(CafeAlvoradaContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Propriedades para a Página
        public string NomeUsuario { get; set; } = "Usuário";
        public List<Produto> Produtos { get; set; } = new List<Produto>();
        public List<Produto> ProdutosComAlerta { get; set; } = new List<Produto>();
        
        [TempData]
        public string MensagemFeedback { get; set; }

        // Propriedades de Filtro
        [BindProperty(SupportsGet = true)]
        public string? FiltroProduto { get; set; }

        // Propriedades de Model-Binding para os Modais
        [BindProperty]
        public Produto ProdutoNovo { get; set; } = new Produto();
        [BindProperty]
        public Produto ProdutoEdicao { get; set; } = new Produto();


        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null) NomeUsuario = user.UserName ?? "Usuário";

            // Query base
            var query = _context.Produtos.AsQueryable();

            // Aplicar filtro (lógica do 'filtrar')
            if (!string.IsNullOrEmpty(FiltroProduto))
            {
                query = query.Where(p => p.Nome.Contains(FiltroProduto));
            }

            Produtos = await query.OrderBy(p => p.Nome).ToListAsync();

            // Buscar produtos com alerta
            ProdutosComAlerta = await _context.Produtos
                .Where(p => p.Quantidade <= p.LimiteAlerta && p.LimiteAlerta > 0)
                .OrderBy(p => p.Nome)
                .ToListAsync();
        }

        // Handler para 'cadastrar_produto'
        public async Task<IActionResult> OnPostCadastrarProdutoAsync()
        {
            try
            {
                _context.Produtos.Add(ProdutoNovo);
                await _context.SaveChangesAsync();
                MensagemFeedback = "Produto cadastrado com sucesso!";
            }
            catch (Exception ex)
            {
                MensagemFeedback = "Erro ao cadastrar produto: " + ex.Message;
            }
            return RedirectToPage();
        }

        // Handler para 'saida_produto'
        public async Task<IActionResult> OnPostSaidaProdutoAsync(int produtoIdSaida, int quantidadeSaida)
        {
            var produto = await _context.Produtos.FindAsync(produtoIdSaida);
            if (produto == null)
            {
                MensagemFeedback = "Erro: Produto não encontrado!";
                return RedirectToPage();
            }

            if (produto.Quantidade < quantidadeSaida)
            {
                MensagemFeedback = "Quantidade em estoque insuficiente para esta saída!";
                return RedirectToPage();
            }

            try
            {
                produto.Quantidade -= quantidadeSaida;
                _context.Produtos.Update(produto);
                await _context.SaveChangesAsync();
                MensagemFeedback = "Saída de produto registrada com sucesso!";
            }
            catch (Exception ex)
            {
                MensagemFeedback = "Erro ao registrar saída: " + ex.Message;
            }
            return RedirectToPage();
        }

        // Handler para 'entrada_produto'
        public async Task<IActionResult> OnPostEntradaProdutoAsync(int produtoIdEntrada, int quantidadeEntrada, decimal valorUnitarioEntrada)
        {
            var produto = await _context.Produtos.FindAsync(produtoIdEntrada);
            if (produto == null)
            {
                MensagemFeedback = "Erro: Produto não encontrado!";
                return RedirectToPage();
            }

            try
            {
                produto.Quantidade += quantidadeEntrada;
                produto.Valor = valorUnitarioEntrada; // Atualiza o valor unitário
                _context.Produtos.Update(produto);
                await _context.SaveChangesAsync();
                MensagemFeedback = "Entrada de produto registrada com sucesso!";
            }
            catch (Exception ex)
            {
                MensagemFeedback = "Erro ao registrar entrada: " + ex.Message;
            }
            return RedirectToPage();
        }
        
        // Handler para 'editar_produto'
        public async Task<IActionResult> OnPostEditarProdutoAsync()
        {
            try
            {
                _context.Produtos.Update(ProdutoEdicao);
                await _context.SaveChangesAsync();
                MensagemFeedback = "Produto editado com sucesso!";
            }
            catch (Exception ex)
            {
                MensagemFeedback = "Erro ao editar produto: " + ex.Message;
            }
            return RedirectToPage();
        }

        // Handler para 'excluir_selecionados'
        public async Task<IActionResult> OnPostExcluirSelecionadosAsync(List<int> produtosIds)
        {
            if (produtosIds == null || !produtosIds.Any())
            {
                MensagemFeedback = "Erro: Nenhum produto selecionado para exclusão.";
                return RedirectToPage();
            }

            try
            {
                var produtosParaExcluir = await _context.Produtos
                    .Where(p => produtosIds.Contains(p.ProdutoId))
                    .ToListAsync();
                
                _context.Produtos.RemoveRange(produtosParaExcluir);
                await _context.SaveChangesAsync();
                MensagemFeedback = $"{produtosParaExcluir.Count} produto(s) excluído(s) com sucesso!";
            }
            catch (Exception ex)
            {
                MensagemFeedback = "Erro ao excluir produtos: " + ex.Message;
            }
            return RedirectToPage();
        }
    }
}