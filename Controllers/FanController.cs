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
        private readonly FanAnalysisService _fanAnalysisService;

        public FanController(XApiService xApiService, FanAnalysisService fanAnalysisService)
        {
            _xApiService = xApiService ?? throw new ArgumentNullException(nameof(xApiService));
            _fanAnalysisService = fanAnalysisService ?? throw new ArgumentNullException(nameof(fanAnalysisService));
        }

        [HttpGet("analyze")]
        public async Task<IActionResult> Analyze()
        {
            try
            {
                // Buscar tweets recentes com #FURIACS em português
                var tweets = await _xApiService.GetRecentTweetsAsync("#FURIACS lang:pt");
                Console.WriteLine($"Tweets recebidos: {tweets?.Count ?? 0}");

                // Analisar tweets
                var analysisResult = _fanAnalysisService.AnalyzeTweets(tweets);
                Console.WriteLine($"Análise concluída: TopWords={analysisResult.TopWords.Count}, Sentiment=[P:{analysisResult.Sentiment.Positive}, N:{analysisResult.Sentiment.Negative}, Neu:{analysisResult.Sentiment.Neutral}]");
                return Ok(analysisResult);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Erro ao acessar API do X: {ex.Message}");
                return StatusCode(500, new { error = "Erro ao acessar a API do X. Verifique o token de autenticação.", details = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao analisar tweets: {ex.Message}");
                return StatusCode(500, new { error = "Erro interno ao analisar tweets.", details = ex.Message });
            }
        }
    }
}