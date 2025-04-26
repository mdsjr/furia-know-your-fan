using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using FuriaKnowYourFan.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Adicionar serviços
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddHttpClient<XApiService>(client =>
{
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
}).AddTypedClient<XApiService>((httpClient, sp) =>
    new XApiService(httpClient, sp.GetRequiredService<IConfiguration>()));
builder.Services.AddSingleton<FanAnalysisService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FURIA Know Your Fan API", Version = "v1" });
});

var app = builder.Build();

// Configurar pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthorization();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();