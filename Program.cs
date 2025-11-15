using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CafeAlvoradaCSharp.Data;
using CafeAlvoradaCSharp.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurar a conexão com o BD (lê do appsettings.json)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// 2. Adicionar o DbContext (substituto do conexao.php)
builder.Services.AddDbContext<CafeAlvoradaContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// 3. Adicionar o sistema de Identidade (substituto do seu login.php e protect.php)
builder.Services.AddDefaultIdentity<ApplicationUser>(options => 
    {
        options.SignIn.RequireConfirmedAccount = false;
        // Simplificar senhas para teste (em produção, remova isso)
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequiredLength = 4; 
    })
    .AddEntityFrameworkStores<CafeAlvoradaContext>();

// 4. Adicionar Razor Pages
builder.Services.AddRazorPages(options =>
{
    // Protege todas as páginas por padrão (substituto do protect.php)
    // Você pode adicionar [AllowAnonymous] nas páginas de Login/Registro
    options.Conventions.AuthorizeFolder("/");
    options.Conventions.AllowAnonymousToPage("/Account/Login");
    options.Conventions.AllowAnonymousToPage("/Account/Register");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Habilita o uso de CSS, JS, Imagens da pasta wwwroot

app.UseRouting();

app.UseAuthentication(); // Habilita o login
app.UseAuthorization();  // Habilita a proteção de páginas

app.MapRazorPages(); // Mapeia as páginas (ex: /Clientes)

app.Run();