using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace FuriaKnowYourFan.Services
{
    public class XApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _bearerToken;

        public XApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _bearerToken = configuration["XApiToken"] ?? throw new ArgumentNullException("XApiToken não configurado");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken);
        }

        public async Task<List<Tweet>?> GetRecentTweetsAsync(string query)
        {
            const int maxRetries = 3;
            int retryCount = 0;
            int delayMs = 1000; // Início com 1 segundo

            while (retryCount < maxRetries)
            {
                try
                {
                    var url = $"https://api.x.com/2/tweets/search/recent?query={Uri.EscapeDataString(query)}&tweet.fields=created_at";
                    var response = await _httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    var json = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Resposta da API: {json}");
                    var tweets = JsonConvert.DeserializeObject<TweetResponse>(json);
                    return tweets?.Data ?? new List<Tweet>();
                }
                catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    retryCount++;
                    if (retryCount >= maxRetries)
                    {
                        Console.WriteLine("Máximo de tentativas atingido para API do X (429).");
                        return new List<Tweet>();
                    }
                    Console.WriteLine($"Erro 429: Aguardando {delayMs}ms antes de tentar novamente...");
                    await Task.Delay(delayMs);
                    delayMs *= 2; // Aumenta o delay exponencialmente
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Erro na API do X: {ex.Message}");
                    return new List<Tweet>();
                }
            }
            return new List<Tweet>();
        }
    }

    public class Tweet
    {
        public string? Id { get; set; }
        public string? Text { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [JsonProperty("edit_history_tweet_ids")]
        public List<string>? EditHistoryTweetIds { get; set; }
    }

    public class TweetResponse
    {
        public List<Tweet>? Data { get; set; }
    }
}