using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace FuriaKnowYourFan.Services
{
    public class XApiService
    {
        private readonly HttpClient _httpClient;

        public XApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer YOUR_X_API_BEARER_TOKEN");
        }

        public async Task<List<Tweet>> GetRecentTweetsAsync(string query)
        {
            try
            {
                var url = $"https://api.x.com/2/tweets/search/recent?query={Uri.EscapeDataString(query)}&max_results=10&tweet.fields=created_at";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
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

                Console.WriteLine($"Resposta da API: {json}");
                return tweets;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao acessar API do X: {ex.Message}");
                throw;
            }
        }
    }

    public class Tweet
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string CreatedAt { get; set; }
    }
}