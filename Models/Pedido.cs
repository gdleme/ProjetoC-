using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CafeAlvoradaCSharp.Models
{
    [Table("pedidos")]
    public class Pedido
    {
        [Key]
        [Column("pedido_id")]
        public int PedidoId { get; set; }

        [Column("cliente_id")]
        public int ClienteId { get; set; }

        [Column("data_pedido")]
        public DateOnly DataPedido { get; set; }

        [Column("valor", TypeName = "decimal(10, 2)")]
        public decimal Valor { get; set; }

        [Column("mesa")]
        public string? Mesa { get; set; }

        [Column("status")]
        public string? Status { get; set; } // enum('aberto','concluido')

        // Propriedades de Navegação
        [ForeignKey("ClienteId")]
        public virtual Cliente Cliente { get; set; }
        public virtual ICollection<ItemPedido> ItensPedido { get; set; } = new List<ItemPedido>();
    }
}