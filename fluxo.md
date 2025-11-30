erDiagram
    CLIENTES ||--o{ PEDIDOS : "realiza (1:N)"
    PEDIDOS ||--|{ ITEM_PEDIDO : "contém (1:N)"
    ESTOQUE ||--o{ ITEM_PEDIDO : "compõe (1:N)"
    
    USUARIOS {
        int usuario_id PK
        string nome_usuario
        string senha
        enum tipo_usuario
    }
    
    CLIENTES {
        int cliente_id PK
        string nome
        string cpf UK
        string email
    }
    
    PEDIDOS {
        int pedido_id PK
        int cliente_id FK
        date data_pedido
        float valor
        string status
    }

    ESTOQUE {
        int produto_id PK
        string nome
        int quantidade
        float valor
    }

    LICENCAS {
        int licenca_id PK
        string nome_licenca
        date data_validade
        string status_notificacao
    }
