graph TD
    User((Usuário))
    
    subgraph "Camada de Apresentação (Views/Public)"
        HTML[Interface HTML/CSS]
        JS[Interatividade JS]
    end
    
    subgraph "Camada de Aplicação (Backend)"
        Auth[Autenticação (Auth)]
        Logic[Lógica de Negócio (PHP)]
    end
    
    subgraph "Camada de Dados (Persistência)"
        Config[Conexão (Database.php)]
        DB[(Banco de Dados MySQL)]
    end

    User -->|Acessa| HTML
    HTML -->|Envia Requisição| Logic
    Logic -->|Verifica Sessão| Auth
    Logic -->|Query SQL| Config
    Config -->|Persistência| DB
    DB -->|Retorna Dados| Config
    Config -->|Dados| Logic
    Logic -->|Renderiza| HTML
