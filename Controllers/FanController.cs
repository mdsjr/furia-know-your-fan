using Microsoft.AspNetCore.Mvc;
using FuriaKnowYourFan.Services;
using System;
using System.Threading.Tasks;

namespace FuriaKnowYourFan.Controllers
{
    [ApiController]
    [Route("api/fan")]
    public class FanController : ControllerBase
    {
        private readonly XApiService _xApiService;
        private readonly FanAnalysisService _analysisService;
        private static AnalysisResult? _cachedResult;
        private static DateTime _cacheExpiry = DateTime.MinValue;

        public FanController(XApiService xApiService, FanAnalysisService analysisService)
        {
            _xApiService = xApiService;
            _analysisService = analysisService;
        }

        [HttpGet("analyze")]
        public async Task<AnalysisResult> Analyze()
        {
            // Verificar cache
            if (_cachedResult != null && DateTime.UtcNow < _cacheExpiry)
            {
                Console.WriteLine("Retornando resultado do cache.");
                return _cachedResult;
            }

            // Obter tweets e analisar
            var tweets = await _xApiService.GetRecentTweetsAsync("#FURIACS lang:pt");
            Console.WriteLine($"Tweets recebidos: {tweets?.Count ?? 0}");
            var result = _analysisService.AnalyzeTweets(tweets);

            // Armazenar no cache por 5 minutos
            _cachedResult = result;
            _cacheExpiry = DateTime.UtcNow.AddMinutes(5);

            return result;
        }
    }
}