<p align="center">
  <img src="wwwroot/img/logo.png" alt="Alvorada" width="200"/>
</p>

<h1 align="center">Café Alvorada - Versão C# (ASP.NET Core)</h1>

Este é um projeto de migração do ERP "Café Alvorada", originalmente escrito em PHP procedural, para uma aplicação web moderna usando C#, ASP.NET Core 8 e Entity Framework Core.

O sistema foi reescrito para utilizar o padrão **Razor Pages**, injeção de dependência e autenticação moderna (ASP.NET Identity), mantendo todas as regras de negócio e funcionalidades do projeto original.

## ✨ Funcionalidades Principais

O sistema é um ERP focado que centraliza a gestão da cafeteria em 4 módulos principais:

### 🔐 Autenticação e Segurança
* Sistema completo de registro e login de usuários (ASP.NET Core Identity).
* Hashing de senhas automático e gerenciamento seguro de sessões.
* Proteção de rotas em todas as páginas (substituindo o `protect.php` original).

### 📊 Dashboard (Página Inicial)
* Página de entrada após o login que apresenta o usuário.
* Menu de navegação principal para todos os módulos do sistema.

### 👥 Gestão de Clientes
* CRUD (Criar, Ler, Atualizar, Excluir) completo de clientes.
* Exibe a data da última compra e a quantidade total de pedidos de cada cliente.
* **Regra de Negócio:** Impede a exclusão de um cliente se ele possuir pedidos com status "aberto".

### 📦 Gestão de Estoque
* CRUD (Criar, Ler, Atualizar, Excluir) completo de produtos no estoque.
* Controle de "Entrada" e "Saída" manual de itens do estoque.
* Alertas visuais de estoque baixo (quando a quantidade atinge o limite mínimo definido).
* Permite a exclusão em massa de produtos selecionados.

### ☕ Gestão de Pedidos
* CRUD completo de pedidos, exibindo apenas os que estão com status "aberto".
* Permite "Concluir" pedidos (mudando seu status e removendo-os da tela principal).
* **Lógica Transacional de Estoque:**
    * Ao **criar ou editar** um pedido, o sistema dá baixa no estoque dos produtos selecionados automaticamente.
    * Ao **excluir** um pedido, o sistema **reverte** o estoque, devolvendo os itens para o sistema.

### 📜 Gestão de Licenças
* CRUD de alvarás, licenças e outros documentos regulatórios.
* **Upload de Arquivos:** Permite anexar documentos (PDF, JPG, PNG) durante o cadastro da licença.
* **Download de Arquivos:** Permite baixar os documentos anexados de forma segura.
* **Monitoramento de Validade:** O sistema exibe um status visual (`Ativa`, `Próximo Vencimento`, `Vencida`) com base na data de validade e no prazo de notificação (30, 60, 90 ou 140 dias).

## 🚀 Stack de Tecnologia (C#)

* **Framework:** .NET 8 (ASP.NET Core)
* **Padrão de UI:** Razor Pages
* **ORM (Acesso a Dados):** Entity Framework (EF) Core 8
* **Autenticação:** ASP.NET Core Identity
* **Banco de Dados:** MySQL (utilizando o provider `Pomelo.EntityFrameworkCore.MySql`)
* **Front-End:** HTML5, CSS3 (estilos originais do projeto PHP), JavaScript (vanilla)

## 🏁 Como Executar Localmente

Siga estes passos para rodar o projeto em sua máquina.

### Pré-requisitos
1.  **[.NET 8 SDK](https://dotnet.microsoft.com/pt-br/download/dotnet/8.0):** Necessário para compilar e rodar o projeto.
2.  **Servidor MySQL:** Um servidor de banco de dados (como o do XAMPP, WAMP ou Docker) deve estar em execução.
3.  **Editor de Código:** Visual Studio 2022 ou VS Code (com a extensão C# Dev Kit).

### 1. Configuração do Banco de Dados
Este projeto usa o Entity Framework Core "Code-First" para gerenciar o banco.

1.  Abra seu gerenciador de MySQL (phpMyAdmin, etc.).
2.  Crie um **banco de dados novo e vazio**:
    ```sql
    CREATE DATABASE cafealvorada_csharp;
    ```
    *(Não execute o `.sql` do projeto PHP. O C# criará as tabelas sozinho).*

### 2. Configuração da Aplicação
1.  Abra o arquivo `appsettings.json`.
2.  Edite a `ConnectionStrings` para que corresponda ao seu usuário e senha do MySQL:
    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=localhost;Database=cafealvorada_csharp;User=root;Password=SUA_SENHA_AQUI;"
    }
    ```
    *(Se você não usa senha no root, deixe `Password=;`)*

### 3. Executar as Migrações (Criar Tabelas)
O Entity Framework precisa criar as tabelas no seu banco de dados vazio.

1.  Abra um terminal na pasta raiz do projeto (onde está o arquivo `.csproj`).
2.  Instale a ferramenta de linha de comando do EF (se ainda não a tiver):
    ```sh
    dotnet tool install --global dotnet-ef
    ```
3.  Crie o script de migração (isso lê seus `Models/`):
    ```sh
    dotnet ef migrations add InitialCreate
    ```
4.  Execute o script no banco de dados:
    ```sh
    dotnet ef database update
    ```
    Isto criará as tabelas `clientes`, `estoque`, `pedidos`, `licencas` e as tabelas de login do ASP.NET (`AspNetUsers`, etc.).

### 4. Executar o Projeto
No mesmo terminal, execute:
```sh
dotnet run
