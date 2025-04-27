using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using FuriaKnowYourFan.Models;

namespace FuriaKnowYourFan.Services
{
    public class XApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public XApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<List<Tweet>> GetRecentTweetsAsync(string query)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("XApiClient");
                var url = $"https://api.x.com/2/tweets/search/recent?query={Uri.EscapeDataString(query)}&max_results=10&tweet.fields=created_at";
                Console.WriteLine($"Enviando requisição para: {url}");
                if (client.DefaultRequestHeaders.Contains("Authorization"))
                {
                    Console.WriteLine("Header Authorization presente na requisição.");
                }
                else
                {
                    Console.WriteLine("Header Authorization NÃO presente na requisição!");
                }

                var response = await client.GetAsync(url);
                Console.WriteLine($"Resposta recebida: Status {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Erro na resposta da API: {errorContent}");
                    throw new HttpRequestException($"Response status code does not indicate success: {(int)response.StatusCode} ({response.ReasonPhrase}). Detalhes: {errorContent}");
                }

                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Resposta da API: {json}");
                var jsonDoc = JsonDocument.Parse(json);

                var tweets = new List<Tweet>();
                if (jsonDoc.RootElement.TryGetProperty("data", out var data))
                {
                    foreach (var tweetElement in data.EnumerateArray())
                    {
                        tweets.Add(new Tweet
                        {
                            Id = tweetElement.GetProperty("id").GetString(),
                            Text = tweetElement.GetProperty("text").GetString(),
                            CreatedAt = tweetElement.GetProperty("created_at").GetString()
                        });
                    }
                }

                Console.WriteLine($"Tweets processados: {tweets.Count}");
                return tweets;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Erro ao acessar API do X: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro inesperado ao acessar API do X: {ex.Message}");
                throw;
            }
        }
    }
}