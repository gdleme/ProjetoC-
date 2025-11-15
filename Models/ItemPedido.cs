using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CafeAlvoradaCSharp.Models
{
    [Table("item_pedido")]
    public class ItemPedido
    {
        [Key]
        [Column("item_pedido_id")]
        public int ItemPedidoId { get; set; }

        [Column("produto_id")]
        public int ProdutoId { get; set; }

        [Column("pedido_id")]
        public int PedidoId { get; set; }

        [Column("quantidade")]
        public int Quantidade { get; set; }

        [Column("produto_nome")]
        public string? ProdutoNome { get; set; }
        
        // Propriedades de Navegação
        [ForeignKey("PedidoId")]
        public virtual Pedido Pedido { get; set; }

        [ForeignKey("ProdutoId")]
        public virtual Produto Produto { get; set; }
    }
}