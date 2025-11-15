using CafeAlvoradaCSharp.Data;
using CafeAlvoradaCSharp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CafeAlvoradaCSharp.Pages
{
    public class PedidosModel : PageModel
    {
        private readonly CafeAlvoradaContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PedidosModel(CafeAlvoradaContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Propriedades para a Página
        public string NomeUsuario { get; set; } = "Usuário";
        [TempData]
        public string MensagemFeedback { get; set; }
        public List<Pedido> Pedidos { get; set; } = new List<Pedido>();

        // Propriedades para os Filtros e Dropdowns
        public SelectList ClientesSelectList { get; set; }
        public SelectList ProdutosSelectList { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? FiltroNrPedido { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? FiltroClienteId { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? FiltroDataPedido { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? FiltroMesa { get; set; }


        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null) NomeUsuario = user.UserName ?? "Usuário";

            // Carregar dados para dropdowns
            await CarregarSelectLists();

            // Query base (lógica 'READ')
            var query = _context.Pedidos
                .Include(p => p.Cliente) // JOIN com Clientes
                .Include(p => p.ItensPedido) // JOIN com ItensPedido
                .Where(p => p.Status == "aberto") // Apenas pedidos abertos
                .AsQueryable();

            // Aplicar Filtros
            if (!string.IsNullOrEmpty(FiltroNrPedido) && int.TryParse(FiltroNrPedido, out int pedidoId))
            {
                query = query.Where(p => p.PedidoId == pedidoId);
            }
            if (FiltroClienteId.HasValue)
            {
                query = query.Where(p => p.ClienteId == FiltroClienteId.Value);
            }
            if (!string.IsNullOrEmpty(FiltroDataPedido) && DateOnly.TryParse(FiltroDataPedido, out DateOnly data))
            {
                query = query.Where(p => p.DataPedido == data);
            }
            if (!string.IsNullOrEmpty(FiltroMesa))
            {
                query = query.Where(p => p.Mesa.Contains(FiltroMesa));
            }

            Pedidos = await query.OrderByDescending(p => p.PedidoId).ToListAsync();
        }

        // Handler para 'salvar_pedido'
        public async Task<IActionResult> OnPostSalvarPedidoAsync(int clienteId, string mesa, List<int> produtoIds, List<int> quantidades)
        {
            if (clienteId <= 0 || !produtoIds.Any() || !quantidades.Any() || produtoIds.Count != quantidades.Count)
            {
                MensagemFeedback = "Erro: Dados do pedido inválidos.";
                return RedirectToPage();
            }

            // Inicia a transação (equivalente a $mysqli->begin_transaction())
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                decimal valorTotalPedido = 0;
                var itensParaSalvar = new List<ItemPedido>();
                
                // 1. Validar estoque e calcular valor
                for (int i = 0; i < produtoIds.Count; i++)
                {
                    var produto = await _context.Produtos.FindAsync(produtoIds[i]);
                    if (produto == null) throw new Exception($"Produto ID {produtoIds[i]} não encontrado.");
                    if (produto.Quantidade < quantidades[i]) throw new Exception($"Estoque insuficiente para {produto.Nome}.");

                    // 2. Dar baixa no estoque
                    produto.Quantidade -= quantidades[i];
                    _context.Produtos.Update(produto);
                    
                    valorTotalPedido += produto.Valor * quantidades[i];
                    
                    itensParaSalvar.Add(new ItemPedido
                    {
                        ProdutoId = produto.ProdutoId,
                        Quantidade = quantidades[i],
                        ProdutoNome = produto.Nome
                    });
                }
                
                // 3. Criar o Pedido
                var novoPedido = new Pedido
                {
                    ClienteId = clienteId,
                    Mesa = mesa,
                    DataPedido = DateOnly.FromDateTime(DateTime.Now),
                    Status = "aberto",
                    Valor = valorTotalPedido,
                    ItensPedido = itensParaSalvar // EF Core associa os itens ao pedido
                };

                _context.Pedidos.Add(novoPedido);
                
                // 4. Salvar tudo (Pedido, Itens, Estoque)
                await _context.SaveChangesAsync();
                
                // 5. Commit da transação
                await transaction.CommitAsync();
                
                MensagemFeedback = $"Pedido criado com sucesso! (ID: {novoPedido.PedidoId})";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(); // Equivalente a $mysqli->rollback()
                MensagemFeedback = "Erro ao criar pedido: " + ex.Message;
            }

            return RedirectToPage();
        }
        
        // Handler para 'concluir_pedido'
        public async Task<IActionResult> OnPostConcluirPedidoAsync(int pedidoId)
        {
            var pedido = await _context.Pedidos.FindAsync(pedidoId);
            if (pedido == null)
            {
                MensagemFeedback = "Erro: Pedido não encontrado.";
                return RedirectToPage();
            }

            pedido.Status = "concluido";
            _context.Pedidos.Update(pedido);
            await _context.SaveChangesAsync();
            
            MensagemFeedback = $"Pedido #{pedidoId} concluído com sucesso!";
            return RedirectToPage();
        }
        
        // Handler para 'excluir_pedido'
        public async Task<IActionResult> OnPostExcluirPedidoAsync(int pedidoId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var pedido = await _context.Pedidos
                    .Include(p => p.ItensPedido)
                    .FirstOrDefaultAsync(p => p.PedidoId == pedidoId);

                if (pedido == null) throw new Exception("Pedido não encontrado.");

                // 1. Reverter estoque
                foreach (var item in pedido.ItensPedido)
                {
                    var produto = await _context.Produtos.FindAsync(item.ProdutoId);
                    if (produto != null)
                    {
                        produto.Quantidade += item.Quantidade;
                        _context.Produtos.Update(produto);
                    }
                }
                
                // 2. Excluir Itens e Pedido (EF Core faz isso em cascata se configurado,
                // mas é mais seguro remover os itens primeiro)
                _context.ItensPedido.RemoveRange(pedido.ItensPedido);
                _context.Pedidos.Remove(pedido);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                MensagemFeedback = "Pedido excluído com sucesso!";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                MensagemFeedback = "Erro ao excluir pedido: " + ex.Message;
            }
            
            return RedirectToPage();
        }

        // Handler para 'editar_pedido' (combina lógicas de salvar e excluir)
        public async Task<IActionResult> OnPostEditarPedidoAsync(int pedidoId, int clienteId, string mesa, List<int> produtoIds, List<int> quantidades)
        {
            if (pedidoId <= 0 || clienteId <= 0 || !produtoIds.Any())
            {
                MensagemFeedback = "Erro: Dados do pedido inválidos.";
                return RedirectToPage();
            }
            
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var pedidoExistente = await _context.Pedidos
                    .Include(p => p.ItensPedido)
                    .FirstOrDefaultAsync(p => p.PedidoId == pedidoId);
                
                if (pedidoExistente == null) throw new Exception("Pedido não encontrado para edição.");

                // 1. Reverter estoque dos itens ANTIGOS
                foreach (var itemAntigo in pedidoExistente.ItensPedido)
                {
                    var produtoAntigo = await _context.Produtos.FindAsync(itemAntigo.ProdutoId);
                    if (produtoAntigo != null)
                    {
                        produtoAntigo.Quantidade += itemAntigo.Quantidade;
                        _context.Produtos.Update(produtoAntigo);
                    }
                }
                
                // 2. Remover itens ANTIGOS do pedido
                _context.ItensPedido.RemoveRange(pedidoExistente.ItensPedido);
                // Salva a remoção e reversão de estoque
                await _context.SaveChangesAsync();

                // 3. Processar NOVOS itens
                decimal novoValorTotal = 0;
                var novosItens = new List<ItemPedido>();
                for (int i = 0; i < produtoIds.Count; i++)
                {
                    var produtoNovo = await _context.Produtos.FindAsync(produtoIds[i]);
                    if (produtoNovo == null) throw new Exception($"Produto ID {produtoIds[i]} não encontrado.");
                    if (produtoNovo.Quantidade < quantidades[i]) throw new Exception($"Estoque insuficiente para {produtoNovo.Nome}.");

                    // 4. Dar baixa no estoque dos NOVOS itens
                    produtoNovo.Quantidade -= quantidades[i];
                    _context.Produtos.Update(produtoNovo);
                    
                    novoValorTotal += produtoNovo.Valor * quantidades[i];
                    
                    novosItens.Add(new ItemPedido
                    {
                        ProdutoId = produtoNovo.ProdutoId,
                        Quantidade = quantidades[i],
                        ProdutoNome = produtoNovo.Nome
                    });
                }
                
                // 5. Atualizar o pedido principal
                pedidoExistente.ClienteId = clienteId;
                pedidoExistente.Mesa = mesa;
                pedidoExistente.Valor = novoValorTotal;
                pedidoExistente.ItensPedido = novosItens; // Substitui a coleção de itens

                _context.Pedidos.Update(pedidoExistente);
                
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                
                MensagemFeedback = "Pedido atualizado com sucesso!";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                MensagemFeedback = "Erro ao atualizar pedido: " + ex.Message;
            }

            return RedirectToPage();
        }

        // Função auxiliar para carregar dropdowns
        private async Task CarregarSelectLists()
        {
            var clientes = await _context.Clientes.OrderBy(c => c.Nome).ToListAsync();
            ClientesSelectList = new SelectList(clientes, "ClienteId", "Nome", FiltroClienteId);
            
            var produtos = await _context.Produtos.OrderBy(p => p.Nome).ToListAsync();
            ProdutosSelectList = new SelectList(produtos, "ProdutoId", "Nome");
        }
    }
}