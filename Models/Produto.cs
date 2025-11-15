using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CafeAlvoradaCSharp.Models
{
    [Table("estoque")]
    public class Produto
    {
        [Key]
        [Column("produto_id")]
        public int ProdutoId { get; set; }

        [Column("nome")]
        public string Nome { get; set; }

        [Column("quantidade")]
        public int? Quantidade { get; set; }

        [Column("valor", TypeName = "decimal(10, 2)")]
        public decimal Valor { get; set; }

        [Column("unidade_medida")]
        public string? UnidadeMedida { get; set; }

        [Column("limite_alerta")]
        public int? LimiteAlerta { get; set; }
    }
}