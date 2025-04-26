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
        public async Task<IActionResult> Analyze()
        {
            if (_cachedResult != null && DateTime.UtcNow < _cacheExpiry && _cachedResult.TopWords.Any())
            {
                Console.WriteLine("Retornando resultado do cache.");
                return Ok(_cachedResult);
            }

            try
            {
                var tweets = await _xApiService.GetRecentTweetsAsync("#FURIACS lang:pt");
                Console.WriteLine($"Tweets recebidos: {tweets?.Count ?? 0}");
                var result = _analysisService.AnalyzeTweets(tweets);

                if (tweets != null && tweets.Any() && result.TopWords.Any())
                {
                    _cachedResult = result;
                    _cacheExpiry = DateTime.UtcNow.AddMinutes(5);
                    Console.WriteLine("Cache atualizado com novo resultado.");
                    return Ok(result);
                }
                else if (_cachedResult != null && _cachedResult.TopWords.Any())
                {
                    Console.WriteLine("Retornando cache devido a tweets inválidos.");
                    return Ok(_cachedResult);
                }
                else
                {
                    Console.WriteLine("Nenhum dado válido disponível.");
                    return Ok(new AnalysisResult
                    {
                        TopWords = new Dictionary<string, int>(),
                        Sentiment = new Sentiment { Positive = 0, Negative = 0, Neutral = 0 },
                        PostsByDay = new Dictionary<string, int>()
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao acessar API do X: {ex.Message}");
                if (_cachedResult != null && _cachedResult.TopWords.Any())
                {
                    Console.WriteLine("Retornando cache devido a erro na API.");
                    return Ok(_cachedResult);
                }
                return StatusCode(500, "Erro ao processar a solicitação.");
            }
        }
    }
}