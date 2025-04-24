using Microsoft.AspNetCore.Mvc;
using FuriaKnowYourFan.Services;

namespace FuriaKnowYourFan.Controllers
{
    [ApiController]
    [Route("api/fan")]
    public class FanController : ControllerBase
    {
        private readonly XApiService _xApiService;
        private readonly FanAnalysisService _analysisService;

        public FanController(XApiService xApiService, FanAnalysisService analysisService)
        {
            _xApiService = xApiService;
            _analysisService = analysisService;
        }

        [HttpGet("analyze")]
        public async Task<AnalysisResult> Analyze()
        {
            var tweets = await _xApiService.GetRecentTweetsAsync("#FURIACS lang:pt");
            Console.WriteLine($"Tweets recebidos: {tweets?.Count ?? 0}");
            return _analysisService.AnalyzeTweets(tweets);
        }
    }
}