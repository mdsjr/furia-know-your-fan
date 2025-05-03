using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using FuriaKnowYourFan.Services;

var builder = WebApplication.CreateBuilder(args);

// Adicionar serviços ao contêiner
builder.Services.AddControllers();

// Configurar HttpClient para XApiService usando IHttpClientFactory
builder.Services.AddHttpClient("XApiClient", client =>
{
    var bearerToken = builder.Configuration["XApiBearerToken"];
    if (string.IsNullOrEmpty(bearerToken))
    {
        throw new InvalidOperationException("XApiBearerToken não configurado em appsettings.json. Verifique o arquivo de configuração.");
    }
    Console.WriteLine($"Configurando HttpClient com token: {bearerToken.Substring(0, 10)}... (truncado para log)");
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {bearerToken}");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// Registrar serviços
builder.Services.AddScoped<XApiService>();
builder.Services.AddSingleton<FanAnalysisService>();

var app = builder.Build();

// Configurar o pipeline de requisições
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();