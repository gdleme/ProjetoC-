using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace CafeAlvoradaCSharp.Models
{
    // Herda de IdentityUser para ter Login, Senha, etc.
    // Mapeia para a tabela 'usuarios'
    [Table("usuarios")]
    public class ApplicationUser : IdentityUser<int> // Usa <int> para bater com seu usuario_id INT
    {
        // Renomeia as colunas para bater com seu BD
        [PersonalData]
        [Column("nome_usuario")]
        public override string? UserName { get; set; }

        [Column("senha")]
        public override string? PasswordHash { get; set; }
        
        [Column("tipo_usuario")]
        public string? TipoUsuario { get; set; } // enum('admin','usuario','gerente')
        
        // Mapeia o ID
        [Column("usuario_id")]
        public override int Id { get; set; }
    }
}