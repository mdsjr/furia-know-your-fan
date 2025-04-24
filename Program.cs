using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using FuriaKnowYourFan.Services;

var builder = WebApplication.CreateBuilder(args);

// Adicionar serviços
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddHttpClient<XApiService>(client =>
{
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
}).AddTypedClient<XApiService>((httpClient, sp) =>
    new XApiService(httpClient, sp.GetRequiredService<IConfiguration>()));
builder.Services.AddSingleton<FanAnalysisService>();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configurar pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthorization();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();