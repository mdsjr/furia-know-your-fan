# FuriaKnowYourFan

FuriaKnowYourFan é uma aplicação web desenvolvida para o Challenge #2 da FURIA, com o objetivo de analisar a base de fãs do time de Counter-Strike (CS) através de dados coletados da API do X. A aplicação exibe análises de tweets com a hashtag `#FURIACS`, incluindo Top Palavras, Posts por Dia, Sentimento e Tweets associados, além de permitir a validação de perfis de fãs com base em interações com a FURIA.

## Funcionalidades

### Página Inicial:

#### Gráficos interativos com:

* **Top Palavras:** Palavras mais frequentes nos tweets (ex.: furia, furiacs, primeiro).
* **Posts por Dia:** Distribuição de tweets por data (ex.: 27/04/2025: 10).
* **Sentimento:** Análise de sentimento dos tweets (positivo, negativo, neutro).
* **Tweets com Top Palavras:** Até 3 tweets por palavra-chave.

* Botão "Exportar Dados" para baixar a análise em formato JSON (`furia_analysis.json`).
* Navegação para a página de perfil.

### Página de Perfil:

* Formulário para cadastro de informações pessoais (nome, endereço, CPF, interesses).
* Upload de arquivos (PNG, JPEG, PDF) com validação.
* Validação de handle do X (ex.: `@MoacirDomingos5`) com base em interações com `#FURIACS` ou `@FURIA`.
* Mensagens de validação (ex.: "Perfil validado com sucesso!" ou "Nenhuma interação encontrada.").

### Backend:

* Integração com a API do X para buscar tweets recentes.
* **Endpoints:**
    * `GET /api/fan/analyze`: Retorna análise de tweets.
    * `GET /api/profile/validate/{handle}`: Valida um handle do X.

## Tecnologias Utilizadas

* **Backend:** ASP.NET Core (C#), `HttpClient` com `IHttpClientFactory` para chamadas à API do X.
* **Frontend:** HTML, JavaScript, Tailwind CSS (via CDN), Chart.js para gráficos.
* **API:** X API v2 (`/2/tweets/search/recent`) com autenticação via Bearer Token.
* **Outros:** Git para controle de versão, Postman para testes de API.

## Pré-requisitos

* .NET 8.0 SDK
* Conta na API do X com Bearer Token (tier gratuito ou superior).
* Navegador moderno (Chrome, Firefox, Edge).
* Postman (opcional, para testes).

## Configuração

1.  **Clone o Repositório:**
    ```bash
    git clone [https://github.com/](https://github.com/)<seu-usuario>/FuriaKnowYourFan.git
    cd FuriaKnowYourFan
    ```

2.  **Configure o Token da API do X:**
    * Crie ou edite o arquivo `appsettings.json` em `D:\FuriaKnowYourFan`:

        ```json
        {
          "XApiBearerToken": "SEU_BEARER_TOKEN_AQUI"
        }
        ```

    * Substitua `SEU_BEARER_TOKEN_AQUI` pelo seu token da API do X.

3.  **Restaure Dependências:**
    ```bash
    dotnet restore
    ```

## Executando a Aplicação

1.  **Compile e Execute:**
    ```bash
    dotnet build
    dotnet run
    ```

2.  **Acesse a Aplicação:**
    * Abra `http://localhost:5001` no navegador.

### Funcionalidades:

* **Página Inicial:** Clique em "Atualizar Dados" para carregar análises. Use "Exportar Dados" para baixar o JSON.
* **Página de Perfil:** Preencha o formulário, faça upload de um arquivo, e valide um handle (ex.: `@MoacirDomingos5`).

## Testes

### Endpoints:

* `GET http://localhost:5001/api/fan/analyze`: Retorna análise de tweets.
* `GET http://localhost:5001/api/profile/validate/MoacirDomingos5`: Valida um handle.

### Postman:

* Importe as requisições do arquivo `FuriaKnowYourFan.postman_collection.json` (se disponível).
* Teste com o token configurado.

### Navegador:

* Verifique os gráficos na página inicial.
* Teste a validação de handles na página de perfil.

## Limitações

* **Limite da API do X:** O tier gratuito impõe limites de requisições (ex.: 50 por 15 minutos), podendo causar erros 429 (Too Many Requests).
* **Frontend:** Usa Tailwind CSS via CDN e Babel no navegador, não otimizados para produção.
* **Favicon:** Erro 404 para `favicon.ico` (não afeta funcionalidade).

## Estrutura do Projeto
```
FuriaKnowYourFan/  
├── Controllers/  
│   ├── FanController.cs  
│   ├── ProfileController.cs  
├── Models/  
│   ├── Tweet.cs  
├── Services/  
│   ├── FanAnalysisService.cs  
│   ├── XApiService.cs  
├── wwwroot/  
│   ├── index.html  
│   ├── profile.html  
├── appsettings.json
├── GET-5001-api-fan-analyze.json  
├── Program.cs  
├── README.md  

```
**Demo**:  
https://www.loom.com/share/90c6c671c94b4e2aad279ecd152f10bf?sid=ab462096-7bed-4280-9a97-b52c65db0d60


Desenvolvido com 💪 para a FURIA! `#DIADEFURIA`
