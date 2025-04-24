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
        private readonly string _bearerToken = "AAAAAAAAAAAAAAAAAAAAAA1F0wEAAAAAfTTAy%2BnNzFAlZ5jIQQDsnjwnzN0%3DwV8jOOOTevVo4AebS0jLfVtbDlenf2JLlFixxOMMpEEqik2v1O";

        public XApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken);
        }

        public async Task<List<Tweet>> GetRecentTweetsAsync(string query)
        {
            var response = await _httpClient.GetAsync($"https://api.x.com/2/tweets/search/recent?query={query}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var tweets = JsonConvert.DeserializeObject<TweetResponse>(json);
            return tweets.Data;
        }
    }

    public class Tweet
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class TweetResponse
    {
        public List<Tweet> Data { get; set; }
    }
}