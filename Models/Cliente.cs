using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CafeAlvoradaCSharp.Models
{
    [Table("clientes")]
    public class Cliente
    {
        [Key]
        [Column("cliente_id")]
        public int ClienteId { get; set; }

        [Column("nome")]
        public string Nome { get; set; }

        [Column("email")]
        public string? Email { get; set; }

        [Column("telefone")]
        public string? Telefone { get; set; }

        [Column("cpf")]
        public string? Cpf { get; set; }

        // Propriedade de Navegação: Um Cliente pode ter vários Pedidos
        public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    }
}