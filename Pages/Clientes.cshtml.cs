using CafeAlvoradaCSharp.Data;
using CafeAlvoradaCSharp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CafeAlvoradaCSharp.Pages
{
    // ViewModel para os dados da tabela (com contagem de pedidos)
    public class ClienteViewModel
    {
        public int ClienteId { get; set; }
        public string Nome { get; set; } = "";
        public string? Email { get; set; }
        public string? Telefone { get; set; }
        public string? Cpf { get; set; }
        public DateOnly? UltimaCompra { get; set; }
        public int QtdPedidos { get; set; }
    }
    
    public class ClientesModel : PageModel
    {
        private readonly CafeAlvoradaContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ClientesModel(CafeAlvoradaContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Propriedades para a Página
        public string NomeUsuario { get; set; } = "Usuário";
        public int TotalClientes { get; set; }
        public List<ClienteViewModel> ClientesVM { get; set; } = new List<ClienteViewModel>();

        // Propriedades para os Modais
        [BindProperty]
        public Cliente ClienteNovo { get; set; } = new Cliente();
        [BindProperty]
        public Cliente ClienteEdicao { get; set; } = new Cliente();
        
        // Propriedade para mensagens de feedback (substitui o $_SESSION['mensagem_...'])
        [TempData]
        public string MensagemFeedback { get; set; }

        // Método GET (carrega a página)
        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null) NomeUsuario = user.UserName ?? "Usuário";

            // Converte a query SQL do PHP para LINQ (C#)
            ClientesVM = await _context.Clientes
                .Include(c => c.Pedidos) // Inclui os pedidos (JOIN)
                .Select(c => new ClienteViewModel
                {
                    ClienteId = c.ClienteId,
                    Nome = c.Nome,
                    Email = c.Email,
                    Telefone = c.Telefone,
                    Cpf = c.Cpf,
                    QtdPedidos = c.Pedidos.Count(),
                    UltimaCompra = c.Pedidos.Any() ? c.Pedidos.Max(p => p.DataPedido) : (DateOnly?)null
                })
                .OrderBy(c => c.Nome)
                .ToListAsync();

            TotalClientes = ClientesVM.Count;
        }

        // Método POST para Salvar (lógica do 'salvar_cliente')
        public async Task<IActionResult> OnPostSalvarClienteAsync()
        {
            if (string.IsNullOrEmpty(ClienteNovo.Nome))
            {
                MensagemFeedback = "Erro: O nome do cliente é obrigatório!";
                return RedirectToPage();
            }

            // Verifica CPF duplicado
            bool cpfExiste = await _context.Clientes.AnyAsync(c => c.Cpf == ClienteNovo.Cpf && c.ClienteId != ClienteNovo.ClienteId);
            if (cpfExiste)
            {
                MensagemFeedback = "Erro: CPF já cadastrado para outro cliente!";
                return RedirectToPage();
            }

            try
            {
                _context.Clientes.Add(ClienteNovo);
                await _context.SaveChangesAsync();
                MensagemFeedback = "Cliente cadastrado com sucesso!";
            }
            catch (Exception ex)
            {
                MensagemFeedback = $"Erro ao cadastrar cliente: {ex.Message}";
            }
            
            return RedirectToPage();
        }

        // Método POST para Editar (lógica do 'editar_cliente')
        public async Task<IActionResult> OnPostEditarClienteAsync()
        {
            if (string.IsNullOrEmpty(ClienteEdicao.Nome))
            {
                MensagemFeedback = "Erro: O nome do cliente é obrigatório para edição!";
                return RedirectToPage();
            }

            // Verifica CPF duplicado
            bool cpfExiste = await _context.Clientes.AnyAsync(c => c.Cpf == ClienteEdicao.Cpf && c.ClienteId != ClienteEdicao.ClienteId);
            if (cpfExiste)
            {
                MensagemFeedback = "Erro: CPF já cadastrado para outro cliente!";
                return RedirectToPage();
            }

            try
            {
                _context.Clientes.Update(ClienteEdicao);
                await _context.SaveChangesAsync();
                MensagemFeedback = "Cliente atualizado com sucesso!";
            }
            catch (Exception ex)
            {
                MensagemFeedback = $"Erro ao atualizar cliente: {ex.Message}";
            }

            return RedirectToPage();
        }

        // Método POST para Excluir (lógica do 'excluir_cliente')
        public async Task<IActionResult> OnPostExcluirClienteAsync(int clienteIdParaExcluir)
        {
            // Lógica de verificação de pedidos abertos
            int numPedidosAbertos = await _context.Pedidos
                .CountAsync(p => p.ClienteId == clienteIdParaExcluir && p.Status == "aberto");

            if (numPedidosAbertos > 0)
            {
                MensagemFeedback = $"Erro: Não é possível excluir o cliente pois ele possui {numPedidosAbertos} pedido(s) EM ANDAMENTO associado(s).";
                return RedirectToPage();
            }

            try
            {
                var cliente = await _context.Clientes.FindAsync(clienteIdParaExcluir);
                if (cliente != null)
                {
                    _context.Clientes.Remove(cliente);
                    await _context.SaveChangesAsync();
                    MensagemFeedback = "Cliente excluído com sucesso!";
                }
                else
                {
                    MensagemFeedback = "Erro: Cliente não encontrado.";
                }
            }
            catch (Exception ex)
            {
                MensagemFeedback = $"Erro ao excluir cliente: {ex.Message}";
            }

            return RedirectToPage();
        }
    }
}