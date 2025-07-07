# Processo Seletivo da Bellosoft

# Especificações

Para o processo seletivo da empresa Bellosoft Programas de Computador Eireli, solicitou-se a criação de uma Web API em .NET 8.0, que integre com uma API de terceiros à escolha do candidato, incluindo placeholders, para demonstração de proficiência na tecnologia e boas práticas. O sistema deve conter um sistema de autenticação e autorização, ao menos um controlador que solicite informações de uma API de terceiros, processamento/transformação de respostas, persistência em um banco de dados SQL Server, com abordagem preferencialmente code-first, e testes utilizando Swagger.

Para esta aplicação, dentro das instruções, optou-se pelas seguintes escolhas:

- Autenticação e autorização através de cookies.
- Banco de Dados SQL Server Express LocalDB, para testes de desenvolvimento, sendo uma variação acessível do SQL Server Express, a opção gratuita do SQL Server.
- Uso da API externa [https://deckofcardsapi.com](https://deckofcardsapi.com), que permite montar um baralho comum (francês), fornece uma chave (ID) por baralho, permite pescar cartas, dentre outras funcionalidades.
- "Soft delete" para todas as operações, onde o registro não será apagado da base de dados, apenas terá a data de exclusão ajustada.
- Para fins do processo, a autenticação **não** incluirá verificação do e-mail, recuperação de senha, autenticação social e autenticação de dois fatores. Apenas autenticação com e-mail e senha sem verificação serão suportadas.

# Apresentação

Esta Web API criada em .NET 8.0 permite ao usuário interagir através de requisições diretas para a API, testáveis pelo Swagger. Na API, o usuário poderá:
1. Criar uma conta a partir de um e-mail, protegida por senha,
2. Fazer logon e logout,
3. Criar um baralho através da API externa,
4. Pescar uma quantidade arbitrária de cartas, sendo isso gerenciado pela API externa,
5. Excluir um baralho (i.e. a relação do usuário com o baralho),
6. Listar os baralhos criados para si.

# Requisitos de Teste

Para executar o projeto, espera-se os seguintes requisitos:

- Neste momento, foi testado apenas no sistema operacional Windows (embora deva funcionar em um deploy Linux, desde que as configurações sejam ajustadas de acordo)
- Visual Studio 2022, com pacote ASP.NET instalado, incluindo LocalDB, e suporte a .NET 8.0

# Instalação do Teste

*Aqui será adicionado um tutorial de como configurar e preparar o projeto para execução*.
