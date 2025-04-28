## Detalhamento dos Arquivos

### 1. `Program.cs`

- **Caminho:** `D:\FuriaKnowYourFan\Program.cs`
- **Funcionalidade:** Configura e inicializa a aplicação ASP.NET Core.
- **Funções Principais:**
  - Configura serviços (ex.: `IHttpClientFactory` para chamadas HTTP).
  - Define o pipeline de middleware (ex.: roteamento, estáticos).
  - Carrega configurações de `appsettings.json` (ex.: `XApiBearerToken`).
- **Código Principal:**

  ```csharp
  var builder = WebApplication.CreateBuilder(args);
  builder.Services.AddControllers();
  builder.Services.AddHttpClient("XApiClient", client => {
      client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", builder.Configuration["XApiBearerToken"]);
  });
  var app = builder.Build();
  app.UseStaticFiles();
  app.MapControllers();
  app.Run();
  ```

- **Tecnologias:** ASP.NET Core, `IHttpClientFactory`.
- **Observações:** Usa `IHttpClientFactory` para gerenciar instâncias de `HttpClient`, evitando problemas de esgotamento de sockets.

### 2. `appsettings.json`

- **Caminho:** `D:\FuriaKnowYourFan\appsettings.json`
- **Funcionalidade:** Armazena configurações da aplicação, como o token da API do X.
- **Conteúdo:**

  ```json
  {
    "XApiBearerToken": "SEU_BEARER_TOKEN_AQUI"
  }
  ```

- **Funções Principais:** Fornece o `XApiBearerToken` para autenticação nas chamadas à API do X.
- **Tecnologias:** JSON.
- **Observações:** O token deve ser substituído por um válido da API do X (tier gratuito ou superior).

### 3. `Controllers/FanController.cs`

- **Caminho:** `D:\FuriaKnowYourFan\Controllers\FanController.cs`
- **Funcionalidade:** Gerencia o endpoint `/api/fan/analyze` para análise de tweets.
- **Funções Principais:**
  - `AnalyzeTweetsAsync`: Busca tweets via `XApiService` e processa análises via `FanAnalysisService`.
  - Retorna um objeto JSON com `topWords`, `sentiment`, `postsByDay`, e `topWordsTweets`.
- **Código Principal:**

  ```csharp
  [HttpGet("analyze")]
  public async Task<IActionResult> AnalyzeTweets()
  {
      var tweets = await _xApiService.GetRecentTweetsAsync("#FURIACS lang:pt");
      var analysis = _fanAnalysisService.AnalyzeTweets(tweets);
      return Ok(analysis);
  }
  ```

- **Tecnologias:** ASP.NET Core, C#, Dependency Injection.
- **Observações:** Depende de `XApiService` para buscar tweets e `FanAnalysisService` para análise.

### 4. `Controllers/ProfileController.cs`

- **Caminho:** `D:\FuriaKnowYourFan\Controllers\ProfileController.cs`
- **Funcionalidade:** Gerencia o endpoint `/api/profile/validate/{handle}` para validação de handles do X.
- **Funções Principais:**
  - `ValidateHandleAsync`: Verifica se um handle postou tweets com `#FURIACS`.
  - Trata erros como 429 (Too Many Requests) e retorna JSON com `handle`, `isValid`, e `message`.
- **Código Principal:**

  ```csharp
  [HttpGet("validate/{handle}")]
  public async Task<IActionResult> ValidateHandle(string handle)
  {
      try
      {
          var cleanHandle = handle.StartsWith("@") ? handle.Substring(1) : handle;
          var tweets = await _xApiService.GetRecentTweetsAsync($"from:{cleanHandle} #FURIACS");
          if (tweets == null || tweets.Count == 0)
              return Ok(new { handle = cleanHandle, isValid = false, message = "Perfil não validado." });
          return Ok(new { handle = cleanHandle, isValid = true, message = "Perfil validado com sucesso!" });
      }
      catch (HttpRequestException ex) when (ex.Message.Contains("429"))
      {
          return StatusCode(429, new { handle, isValid = false, message = "Limite de requisições excedido." });
      }
  }
  ```

- **Tecnologias:** ASP.NET Core, C#, Exception Handling.
- **Observações:** Inclui tratamento robusto de erros para evitar respostas malformadas.

### 5. `Models/Tweet.cs`

- **Caminho:** `D:\FuriaKnowYourFan\Models\Tweet.cs`
- **Funcionalidade:** Define o modelo de dados para tweets retornados pela API do X.
- **Funções Principais:** Armazena propriedades como `Id`, `Text`, e `CreatedAt`.
- **Código Principal:**

  ```csharp
  public class Tweet
  {
      public string Id { get; set; }
      public string Text { get; set; }
      public string CreatedAt { get; set; }
  }
  ```

- **Tecnologias:** C#, POCO (Plain Old CLR Object).
- **Observações:** Mapeia diretamente os campos do JSON retornado pela API do X.

### 6. `Services/FanAnalysisService.cs`

- **Caminho:** `D:\FuriaKnowYourFan\Services\FanAnalysisService.cs`
- **Funcionalidade:** Processa tweets para gerar análises de Top Palavras, Posts por Dia, Sentimento, e Tweets associados.
- **Funções Principais:**
  - `AnalyzeTweets`: Calcula métricas a partir de uma lista de tweets.
  - **Top Palavras:** Conta frequência de palavras, ignorando stopwords.
  - **Posts por Dia:** Agrupa tweets por data.
  - **Sentimento:** Classifica tweets como positivo, negativo ou neutro (lógica simplificada).
  - **Tweets com Top Palavras:** Associa tweets às palavras mais frequentes.
- **Código Principal:**

  ```csharp
  public FanAnalysis AnalyzeTweets(List<Tweet> tweets)
  {
      var analysis = new FanAnalysis();
      analysis.TopWords = CalculateTopWords(tweets);
      analysis.PostsByDay = CalculatePostsByDay(tweets);
      analysis.Sentiment = CalculateSentiment(tweets);
      analysis.TopWordsTweets = CalculateTopWordsTweets(tweets, analysis.TopWords);
      return analysis;
  }
  ```

- **Tecnologias:** C#, LINQ.
- **Observações:** Usa uma abordagem simplificada para análise de sentimento (baseada em palavras-chave).

### 7. `Services/XApiService.cs`

- **Caminho:** `D:\FuriaKnowYourFan\Services\XApiService.cs`
- **Funcionalidade:** Integra-se com a API do X para buscar tweets recentes.
- **Funções Principais:**
  - `GetRecentTweetsAsync`: Faz chamadas ao endpoint `/2/tweets/search/recent` com query específica.
  - Trata respostas JSON e erros HTTP (ex.: 429).
- **Código Principal:**

      ```csharp
      public async Task<List<Tweet>> GetRecentTweetsAsync(string query)
      {
          var client = _httpClientFactory.CreateClient("XApiClient");
          var url = <span class="math-inline">"\[https\://api\.x\.com/2/tweets/search/recent?query\=\]\(https\://api\.x\.com/2/tweets/search/recent?query\=\)\{Uri\.EscapeDataString\(query\)\}&max\_results\=10&tweet\.fields\=created\_at";

  var response \= await client\.GetAsync\(url\);
  if \(\!response\.IsSuccessStatusCode\)
  throw new HttpRequestException\(</span>"Status: {response.StatusCode}");
  var json = await response.Content.ReadAsStringAsync();
  var jsonDoc = JsonDocument.Parse(json);
  var tweets = jsonDoc.RootElement.GetProperty("data").EnumerateArray()
  .Select(e => new Tweet { Id = e.GetProperty("id").GetString(), Text = e.GetProperty("text").GetString(), CreatedAt = e.GetProperty("created_at").GetString() })
  .ToList();
  return tweets;
  }

  ```

  ```

- **Tecnologias:** C#, `HttpClient`, `System.Text.Json`.
- **Observações:** Usa `IHttpClientFactory` para chamadas HTTP eficientes.

### 8. `wwwroot/index.html`

- **Caminho:** `D:\FuriaKnowYourFan\wwwroot\index.html`
- **Funcionalidade:** Página inicial que exibe gráficos de análise de tweets.
- **Funções Principais:**
  - **Carregar Dados:** Faz uma chamada Fetch para `/api/fan/analyze`.
  - **Exibir Gráficos:** Usa Chart.js para renderizar Top Palavras, Posts por Dia, e Sentimento.
  - **Exportar Dados:** Gera e baixa `furia_analysis.json`.
  - **Navegação:** Botão para acessar `profile.html`.
- **Código Principal:**

  ```html
  <button onclick="loadData()">Atualizar Dados</button>
  <button onclick="exportData()">Exportar Dados</button>
  <a href="/profile.html">Editar Perfil</a>
  <canvas id="topWordsChart"></canvas>
  <script>
    async function loadData() {
      const response = await fetch("/api/fan/analyze");
      const data = await response.json();
      renderCharts(data);
    }
    function exportData() {
      const data = JSON.stringify(currentAnalysis);
      const blob = new Blob([data], { type: "application/json" });
      const url = URL.createObjectURL(blob);
      const a = document.createElement("a");
      a.href = url;
      a.download = "furia_analysis.json";
      a.click();
    }
  </script>
  ```

- **Tecnologias:** HTML5, JavaScript, Tailwind CSS, Chart.js.
- **Observações:** Usa Tailwind via CDN, não otimizado para produção.

### 9. `wwwroot/profile.html`

- **Caminho:** `D:\FuriaKnowYourFan\wwwroot\profile.html`
- **Funcionalidade:** Página para cadastro de perfil e validação de handle.
- **Funções Principais:**
  - **Formulário:** Coleta nome, endereço, CPF, interesses.
  - **Upload de Arquivo:** Valida arquivos PNG, JPEG, e PDF.
  - **Validação de Handle:** Chama `/api/profile/validate/{handle}` via Fetch.
  - **Exibição de Mensagens:** Mostra sucesso, falha, ou erro (ex.: 429).
- **Código Principal:**

  ```html
  <input id="handle" type="text" placeholder="@seuhandle" />
  <button onclick="validateHandle()">Validar</button>
  <div id="validation-message"></div>
  <script>
    async function validateHandle() {
      const handle = document.getElementById("handle").value;
      const messageDiv = document.getElementById("validation-message");
      const response = await fetch(
        `/api/profile/validate/${encodeURIComponent(handle)}`
      );
      const data = await response.json();
      messageDiv.textContent = data.message;
      messageDiv.style.color = data.isValid
        ? "green"
        : response.status === 429
        ? "red"
        : "orange";
    }
  </script>
  ```

- **Tecnologias:** HTML5, JavaScript, Tailwind CSS.
- **Observações:** Inclui tratamento de erros para melhorar a experiência do usuário.

### 10. `Properties/launchSettings.json`

- **Caminho:** `D:\FuriaKnowYourFan\Properties\launchSettings.json`
- **Funcionalidade:** Configura o ambiente de desenvolvimento (ex.: porta do servidor).
- **Conteúdo:**

  ```json
  {
    "profiles": {
      "FuriaKnowYourFan": {
        "commandName": "Project",
        "launchBrowser": true,
        "applicationUrl": "http://localhost:5001",
        "environmentVariables": {
          "ASPNETCORE_ENVIRONMENT": "Development"
        }
      }
    }
  }
  ```

- **Tecnologias:** JSON.
- **Observações:** Define `http://localhost:5001` como URL padrão.

## Fluxo da Aplicação

1.  **Inicialização (`Program.cs`):** Carrega configurações e inicializa serviços.
2.  **Busca de Tweets (`XApiService.cs`):**
    - `/api/fan/analyze` usa `#FURIACS lang:pt`.
    - `/api/profile/validate/{handle}` usa `from:{handle} #FURIACS`.
3.  **Análise (`FanAnalysisService.cs`):** Processa tweets e gera métricas.
4.  **Exibição (`index.html`, `profile.html`):**
    - `index.html` renderiza gráficos via Chart.js.
    - `profile.html` valida handles e gerencia formulários.
5.  **Exportação:** Gera `furia_analysis.json` no frontend.

## Limitações

- **API do X:** Limite de requisições (ex.: 50 por 15 minutos no tier gratuito) pode causar erros 429.
- **Análise de Sentimento:** Simplificada, baseada em palavras-chave.
- **Frontend:** Tailwind CSS e Babel via CDN, não otimizados para produção.
- **Favicon:** Erro 404 para `favicon.ico`.

## Testes

- **Backend:** Use Postman para testar `/api/fan/analyze` e `/api/profile/validate/MoacirDomingos5`.
- **Frontend:** Acesse `http://localhost:5001` e valide `@MoacirDomingos5`.
- **Logs:** Verifique console do navegador (F12)
