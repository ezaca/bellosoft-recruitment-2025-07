# Processo Seletivo da Bellosoft

# Apresentação

Esta Web API criada em .NET 8.0 permite ao usuário interagir através de requisições diretas para a API, testáveis pelo Swagger. Na API, o usuário poderá:
1. Criar uma conta a partir de um e-mail, protegida por senha,
2. Fazer logon e logout,
3. Criar um baralho através da API externa,
4. Pescar uma quantidade arbitrária de cartas, sendo isso gerenciado pela API externa,
5. Excluir um baralho (i.e. a relação do usuário com o baralho),
6. Listar os baralhos criados para si.
7. Ver os baralhos em uso na API externa

# Especificações

Para o processo seletivo da empresa Bellosoft Programas de Computador Eireli, solicitou-se a criação de uma Web API em .NET 8.0, que integre com uma API de terceiros à escolha do candidato, incluindo placeholders, para demonstração de proficiência na tecnologia e boas práticas. O sistema deve conter um sistema de autenticação e autorização, ao menos um controlador que solicite informações de uma API de terceiros, processamento/transformação de respostas, persistência em um banco de dados SQL Server, com abordagem preferencialmente code-first, e testes utilizando Swagger.

Para esta aplicação, dentro das instruções, optou-se pelas seguintes escolhas:

- Autenticação e autorização através de cookies.
- Banco de Dados SQL Server Express LocalDB, para testes de desenvolvimento, sendo uma variação acessível do SQL Server Express, a opção gratuita do SQL Server.
- Uso da API externa [https://deckofcardsapi.com](https://deckofcardsapi.com), que permite montar um baralho comum (francês), fornece uma chave (ID) por baralho, permite pescar cartas, dentre outras funcionalidades.
- "Soft delete" para todas as operações, onde o registro não será apagado da base de dados, apenas terá a data de exclusão ajustada.
- Apenas autenticação com e-mail e senha sem verificação serão suportadas.
- Não será utilizado "secrets.json", pois o uso do banco de dados LocalDB utiliza autenticação Windows. Se necessário, utilize "appsettings.Development.json" ou 

# Limitações

Para fins do processo seletivo e demonstração, optou-se por não incluir algumas funcionalidades comuns em produção.
Esta é uma lista de funcionalidades que podem ser esperadas, mas precisariam ser adicionadas e configuradas no projeto, não estando opereacionais no momento.

Limitações conhecidas:

- A autenticação não verifica e-mail
- Não é possível recuperar senha através de e-mail, SMS ou outro meio que não por acesso direto ao banco de dados
- Não há opção de autenticação social (ex.: Google, Outlook)
- Não há autenticação de dois fatores ou com segurança extra
- Os relatórios *foram propositalmente* deixados públicos, sem necessitar autenticação, em produção isto não deve acontecer
- O relatório inclui uma versão SQL para demonstração de conhecimento, e uma versão LINQ para demonstração de equivalência

# Requisitos de Instalação

Para executar o projeto, espera-se os seguintes requisitos:

- Neste momento, foi testado apenas no sistema operacional Windows (embora deva funcionar em um deploy Linux, desde que as configurações sejam ajustadas de acordo)
- Visual Studio 2022, com pacote ASP.NET instalado, incluindo LocalDB, e suporte a .NET 8.0

# Como instalar?

Siga estes passos antes de iniciar o teste:

1. Crie um arquivo `BellosoftWebApi/appsettings.Development.json` com o seguinte conteúdo:
```
{
    "ConnectionStrings": {
        "DefaultConnection": "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=bellosoft-recruitment-2025-07;Integrated Security=True;"
    },
    
    "Sqids": {
        "Alphabet": "mTHivO7hx3RAbr1f586SwjNnK2lgpcUVuG09BCtekZdJ4DYFPaWoMLQEsXIqyz"
    }
}
```
2. Abra o Power Shell no diretório do **projeto** e entre o comando `dotnet ef database update`
    - Em caso de erro, pode ser necessário instalar a ferramenta: `dotnet tool install --global dotnet-ef`

# Como testar?

Para facilitar o teste, utilizaremos o Visual Studio, que instalará os certificados HTTPS necessários. É possível fazer manualmente ou hospedar, mas para fins de demonstração, o Visual Studio auxiliará em alguns aspectos.

Para executar o servidor:

1. Certifique-se de ter executado corretamente a instalação da configuração e banco de dados LocalDB
2. Abra o projeto (.sln) no Visual Studio 2022
3. Certifique-se de estar na versão correta do Visual Studio e de ter as ferramentas e requisitos instalados corretamente
4. Selecione o menu `Build > Configuration Manager` e deixe "Active solution configuration" como "Debug" ou "Release", de acordo com a preferência
5. Selecione o menu `Debug > Start Without Debugging` ou pression Ctrl+F5 (mapeamento de atalhos padrão)
6. Confirme qualquer notificação de certificado, e aguarde abrir o navegador padrão com o Swagger, ou acesse o endereço indicado no Console com a rota para o Swagger (ex.: https://localhost:5000/swagger/index.html)

Após abrir o Swagger:

1. Você poderá navegar pela API.
2. Para iniciar, crie um usuário na seção `Auth > /api/auth/signup` e clique em "Try it out". A requisição padrão atende aos requisitos mínimos de e-mail e senha, clique em "Execute". Espera-se um retorno 200 OK.
3. Na seção Auth é possível:
    1. Criar usuário em `/api/auth/signup` (feito no passo 2)
    2. Fazer login em `/api/auth/login`
    3. Fazer logout em `/api/auth/logout`
    4. Trocar a senha (inclusive por ela mesma) em `/api/auth/change-password` (requer estar logado).
4. Na seção Deck é possível (é necessário estar logado):
    1. Criar baralho na API externa em `/api/deck/create`
    2. Listar os baralhos do usuário em `/api/deck/list`
    3. Criar baralho na API externa em `/api/deck/create`
    4. Selecionar o baralho desejado em `/api/deck/select`
    5. Pescar a quantidade desejada de cartas em `/api/deck/draw`
5. Na seção Reports não é necessário login (decisão para demonstração), e é possível:
    1. Gerar o relatório SQL em `/api/reports/active-decks-sql`
    2. Gerar o relatório LINQ equivalente em `/api/reports/active-decks-linq`

Veja as seções de limitações e de limitações conhecidas para mais informações.

-----

<p align="center">
Eliakim Zacarias
<br>Bacharel em Ciência da Computação, Mestre em Engenharia Elétrica
<br>Projeto criado para 10 de julho de 2025, para o processo de seleção da empresa Bellosoft.
</p>
