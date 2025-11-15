using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CafeAlvoradaCSharp.Models
{
    [Table("licencas")]
    public class Licenca
    {
        [Key]
        [Column("licenca_id")]
        public int LicencaId { get; set; }

        [Column("nome_licenca")]
        public string NomeLicenca { get; set; }

        [Column("cnpj")]
        public string Cnpj { get; set; }

        [Column("orgao_responsavel")]
        public string OrgaoResponsavel { get; set; }

        [Column("data_validade")]
        public DateOnly DataValidade { get; set; }

        [Column("prazo_expiracao")]
        public string PrazoExpiracao { get; set; } // enum('30','60','90','140')

        [Column("caminho_documento")]
        public string? CaminhoDocumento { get; set; }
    }
}