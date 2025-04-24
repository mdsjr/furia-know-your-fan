// FuriaKnowYourFan/Services/XApiService.cs
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FuriaKnowYourFan.Services
{
    public class XApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _bearerToken = Environment.GetEnvironmentVariable("X_API_TOKEN") ?? "AAAAAAAAAAAAAAAAAAAAAA1F0wEAAAAAfTTAy%2BnNzFAlZ5jIQQDsnjwnzN0%3DwV8jOOOTevVo4AebS0jLfVtbDlenf2JLlFixxOMMpEEqik2v1O";

        public XApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken);
        }

        public async Task<List<Tweet>?> GetRecentTweetsAsync(string query)
        {
            try
            {
                var response = await _httpClient.GetAsync($"https://api.x.com/2/tweets/search/recent?query={Uri.EscapeDataString(query)}");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var tweets = JsonConvert.DeserializeObject<TweetResponse>(json);
                return tweets?.Data; // Verifica nulidade
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Erro na API do X: {ex.Message}");
                return null;
            }
        }
    }

    public class Tweet
    {
        public string? Id { get; set; }
        public string? Text { get; set; }
        public DateTime CreatedAt { get; set; } // DateTime é um tipo de valor, não precisa de ?
    }

    public class TweetResponse
    {
        public List<Tweet>? Data { get; set; }
    }
}