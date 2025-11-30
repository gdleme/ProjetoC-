CAPA

NOME DA INSTITUIÇÃO
(Ex: UNIVERSIDADE NOVE DE JULHO - UNINOVE)







NOME(S) DO(S) AUTOR(ES)
GABRIEL LEME DANTAS
ALEXANDRE HENRIQUE DORFLER SCHOPF
RUAN PABLO SILVA
GISLAINE YASMIN MOREIRA DA SILVA







CAFÉ ALVORADA: SISTEMA ERP PARA CAFETERIA







SÃO PAULO - SP
2025

FOLHA DE ROSTO

NOME(S) DO(S) AUTOR(ES)

CAFÉ ALVORADA: SISTEMA ERP PARA CAFETERIA

Trabalho de Conclusão de Curso (TCC) apresentado como requisito parcial para obtenção do título de Bacharel em [Seu Curso].

Área de Concentração: Desenvolvimento de Sistemas.
Orientador(a): [Nome do Orientador]





SÃO PAULO - SP
2025

SUMÁRIO

INTRODUÇÃO

COLABORADORES

ARQUITETURA DO SISTEMA

MODELO DE DADOS

REQUISITOS FUNCIONAIS

REQUISITOS NÃO FUNCIONAIS

REGRAS DE NEGÓCIO

CASOS DE USO

MANUAL DE INSTALAÇÃO

MANUAL DO USUÁRIO

PLANO DE TESTES

CONSIDERAÇÕES FINAIS
REFERÊNCIAS

1. INTRODUÇÃO

1.1 Contextualização

O Café Alvorada é um sistema do tipo ERP (Enterprise Resource Planning) desenvolvido para atender às necessidades operacionais de cafeterias. O projeto nasceu da necessidade de centralizar informações de clientes, pedidos, estoque e licenças, eliminando processos manuais e inconsistências de dados.

1.2 Problema

Empresas do segmento de alimentação sofrem frequentemente com:

Falta de controle de estoque eficiente;

Informações dispersas sobre clientes e pedidos;

Riscos operacionais e legais ao operar com licenças vencidas;

Falta de padronização e segurança nos processos internos.

1.3 Objetivos

O Objetivo Geral é criar um sistema totalmente web, simples e intuitivo, capaz de integrar e gerenciar as principais operações de uma cafeteria.

Os Objetivos Específicos incluem:

Registrar pedidos e atualizar o estoque automaticamente;

Gerenciar informações de clientes e histórico de compras;

Controlar licenças e emitir alertas de vencimento;

Garantir segurança, integridade e confiabilidade dos dados.

1.4 Escopo

O sistema contempla: Cadastro e controle de clientes; Emissão e gerenciamento de pedidos; Controle detalhado de estoque; Gestão de licenças e documentos obrigatórios.

O sistema não contempla: Pagamentos online; Integração com sistema fiscal (NFC-e); Aplicativo mobile; Gestão financeira avançada (contas a pagar/receber, fluxo de caixa).

2. COLABORADORES

A equipe de desenvolvimento do projeto é composta pelos seguintes membros:

Nome

RA

E-mail

Gabriel Leme Dantas

2224101466

biel-leme@uni9.edu.br

Alexandre Henrique Dorfler Schopf

2224101576

schopf.alexandre@uni9.edu.br

Ruan Pablo Silva

2224104275

ruan.pablo@uni9.edu.br

Gislaine Yasmin Moreira da Silva

2223204113

yasminmoreira495@uni9.edu.br

3. ARQUITETURA DO SISTEMA

3.1 Tecnologias Utilizadas

Back-end: PHP.

Banco de Dados: MySQL (via extensão MySQLi).

Front-end: HTML5, CSS3, JavaScript.

Design: Fontes web Instrument Sans e Inter.

3.2 Arquitetura Geral

O sistema segue um modelo em camadas para garantir a organização e manutenibilidade:

Camada de Apresentação (views/ e public/): Arquivos responsáveis por renderizar a interface.

Camada de Lógica de Negócio: Estrutura presente nos arquivos de cada módulo.

Camada de Persistência (config/database.php): Conexão com o MySQL e execução de queries seguras.

Camada de Autenticação (auth/): Gerencia sessões, login e logout.

Figura 1 – Diagrama da Arquitetura do Sistema

[INSERIR AQUI O DIAGRAMA DE ARQUITETURA]

4. MODELO DE DADOS

O sistema utiliza um banco de dados relacional MySQL composto por seis tabelas principais:

usuarios: Gerencia o acesso ao sistema (usuario_id, nome_usuario, senha, tipo_usuario).

clientes: Armazena dados dos consumidores (cliente_id, nome, email, telefone, cpf).

estoque: Controla os produtos (produto_id, nome, quantidade, valor, limite_alerta).

pedidos: Cabeçalho dos pedidos (pedido_id, cliente_id, data, valor, status).

item_pedido: Relaciona produtos aos pedidos (item_pedido_id, pedido_id, produto_id, quantidade).

licencas: Monitora a validade de documentos (licenca_id, nome, validade, caminho_documento).

Figura 2 – Diagrama de Entidade-Relacionamento

[INSERIR AQUI O DIAGRAMA DER]

5. REQUISITOS FUNCIONAIS

5.1 Módulo Clientes

RF-C-001: Cadastrar novos clientes.

RF-C-002: Verificar duplicidade de CPF automaticamente.

RF-C-003: Exibir histórico de compras do cliente.

RF-C-004: Impedir exclusão de clientes com pedidos em aberto.

5.2 Módulo Pedidos

RF-P-001: Criar novos pedidos associados a clientes e mesas.

RF-P-002: Atualizar o estoque automaticamente na criação do pedido.

RF-P-003: Reverter o estoque automaticamente ao excluir um pedido.

RF-P-004: Alterar status do pedido (Aberto/Concluído).

RF-P-005: Filtrar pedidos por mesa, cliente ou data.

5.3 Módulo Estoque

RF-E-001: Realizar CRUD (Criar, Ler, Atualizar, Deletar) de produtos.

RF-E-002: Registrar entradas de estoque.

RF-E-003: Realizar exclusão múltipla de produtos via checkbox.

5.4 Módulo Licenças

RF-L-001: Cadastrar licenças com data de validade.

RF-L-002: Realizar upload de documentos comprovatórios.

RF-L-003: Exibir status de vencimento (Vencida, Próximo Vencimento, Ativa).

6. REQUISITOS NÃO FUNCIONAIS

6.1 Segurança

RNF-S-001: Todas as páginas internas devem ser protegidas por autenticação de sessão.

RNF-S-002: O sistema deve utilizar Prepared Statements para prevenir SQL Injection.

RNF-S-003: O sistema deve validar a existência física de arquivos antes da exclusão.

6.2 Usabilidade

RNF-U-001: O sistema deve possuir interface limpa com navegação lateral fixa.

RNF-A-001: O sistema deve exibir alertas visuais para produtos com estoque baixo.

7. REGRAS DE NEGÓCIO

O CPF deve ser único em toda a base de clientes.

Um cliente que possui um pedido com status "Aberto" não pode ser excluído do sistema.

Ao criar um pedido, a quantidade dos itens deve ser subtraída do estoque imediatamente.

Ao excluir um pedido, a quantidade dos itens deve ser devolvida (somada) ao estoque.

As licenças são classificadas automaticamente:

Vencida: Data de validade anterior à data atual.

Próximo Vencimento: Dentro do prazo de alerta configurado.

Ativa: Fora do prazo de alerta.

Produtos com quantidade igual ou inferior ao limite_alerta devem exibir um aviso de destaque no painel de estoque.

8. CASOS DE USO (RESUMIDOS)

UC01 – Gerenciar Clientes: O Administrador acessa o módulo, cadastra um cliente e o sistema valida o CPF.

UC02 – Gerar Pedido: O Atendente seleciona um cliente e uma mesa, adiciona itens ao carrinho e salva. O sistema baixa o estoque.

UC03 – Controlar Estoque: O Gerente cadastra um produto novo ou lança uma entrada de mercadoria.

UC04 – Gerenciar Licenças: O Administrador cadastra uma licença, faz o upload do PDF e o sistema monitora a data de validade.

9. MANUAL DE INSTALAÇÃO

Instalar um servidor local (XAMPP, WAMP ou equivalente).

Clonar o repositório do projeto na pasta pública do servidor (htdocs ou www).

Importar o arquivo database.sql utilizando o phpMyAdmin ou terminal MySQL.

Configurar o arquivo config/database.php com as credenciais do banco de dados local.

Garantir permissões de escrita na pasta /uploads/licencas.

Acessar o sistema pelo navegador: http://localhost/Cafe_Alvorada/public/.

10. MANUAL DO USUÁRIO

Login: Acesse a tela inicial e informe seu usuário e senha.

Clientes: Use o menu lateral para acessar "Clientes". Clique em "Novo Cliente" para cadastrar ou nos ícones de lápis/lixeira para editar/excluir.

Pedidos: Em "Pedidos", clique em "Novo Pedido". Selecione o cliente, adicione produtos e salve. Para finalizar, clique em "Concluir".

Estoque: Acompanhe a lista de produtos. Itens em amarelo indicam estoque baixo. Use os botões de ação para dar entrada ou saída manual.

Licenças: Cadastre as licenças do estabelecimento. O sistema avisará com etiquetas coloridas sobre a proximidade do vencimento.

11. PLANO DE TESTES

Abaixo, os principais casos de teste executados para validação do sistema:

ID

Caso de Teste

Ação Executada

Resultado Esperado

CT-01

Cadastro de Cliente Duplicado

Tentar cadastrar cliente com CPF já existente.

Sistema exibe erro e bloqueia cadastro.

CT-02

Baixa de Estoque

Criar um pedido com 5 unidades de um produto.

Estoque do produto reduz em 5 unidades.

CT-03

Reversão de Estoque

Excluir o pedido criado no CT-02.

Estoque do produto aumenta em 5 unidades.

CT-04

Alerta de Licença

Cadastrar licença com data de ontem.

Status exibe "Vencida" em vermelho.

CT-05

Segurança de Acesso

Tentar acessar dashboard.php sem logar.

Redirecionamento para login.php.

12. CONSIDERAÇÕES FINAIS

O sistema Café Alvorada atende às necessidades essenciais de gestão de uma cafeteria, integrando pedidos, estoque, clientes e licenças de forma eficiente. A arquitetura simples e modular facilita a manutenção e permite futuras expansões, como a inclusão de módulos financeiros, integração fiscal e desenvolvimento de aplicativo móvel.

REFERÊNCIAS

ALEXANDRE, S. et al. Café Alvorada - Repositório de Código. GitHub. Disponível em: https://github.com/AleexDevel0per/Cafe_Alvorada. Acesso em: 2025.

ASSOCIAÇÃO BRASILEIRA DE NORMAS TÉCNICAS. NBR 14724: Informação e documentação: Trabalhos acadêmicos: Apresentação. Rio de Janeiro, 2011.
