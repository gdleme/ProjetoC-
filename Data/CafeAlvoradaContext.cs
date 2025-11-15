using CafeAlvoradaCSharp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CafeAlvoradaCSharp.Data
{
    // Herda de IdentityDbContext para gerenciar Usuários E seus Modelos
    public class CafeAlvoradaContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {
        public CafeAlvoradaContext(DbContextOptions<CafeAlvoradaContext> options)
            : base(options)
        {
        }

        // Mapeia suas classes para as tabelas do BD
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<ItemPedido> ItensPedido { get; set; }
        public DbSet<Licenca> Licencas { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Renomeia as tabelas do Identity para bater com seu BD
            builder.Entity<ApplicationUser>(b =>
            {
                b.ToTable("usuarios");
                // Mapeia as chaves primárias e colunas
                b.Property(u => u.Id).HasColumnName("usuario_id");
                b.Property(u => u.UserName).HasColumnName("nome_usuario");
                b.Property(u => u.PasswordHash).HasColumnName("senha");
                b.Property(e => e.TipoUsuario).HasColumnName("tipo_usuario");
                
                // Ignora colunas do Identity que você não tem
                b.Ignore(e => e.NormalizedUserName);
                b.Ignore(e => e.Email);
                b.Ignore(e => e.NormalizedEmail);
                b.Ignore(e => e.EmailConfirmed);
                b.Ignore(e => e.SecurityStamp);
                b.Ignore(e => e.ConcurrencyStamp);
                b.Ignore(e => e.PhoneNumber);
                b.Ignore(e => e.PhoneNumberConfirmed);
                b.Ignore(e => e.TwoFactorEnabled);
                b.Ignore(e => e.LockoutEnd);
                b.Ignore(e => e.LockoutEnabled);
                b.Ignore(e => e.AccessFailedCount);
            });

            // Remove tabelas de Role, Claims, etc. que você não usa
            builder.Ignore<IdentityRole<int>>();
            builder.Ignore<IdentityUserRole<int>>();
            builder.Ignore<IdentityUserLogin<int>>();
            builder.Ignore<IdentityUserToken<int>>();
            builder.Ignore<IdentityUserClaim<int>>();
            builder.Ignore<IdentityRoleClaim<int>>();
        }
    }
}