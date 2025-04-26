using Microsoft.AspNetCore.Mvc;
using FuriaKnowYourFan.Services;
using System.Threading.Tasks;

namespace FuriaKnowYourFan.Controllers
{
    [ApiController]
    [Route("api/profile")]
    public class ProfileController : ControllerBase
    {
        private readonly XApiService _xApiService;

        public ProfileController(XApiService xApiService)
        {
            _xApiService = xApiService;
        }

        [HttpGet("validate/{handle}")]
        public async Task<IActionResult> ValidateProfile(string handle)
        {
            try
            {
                // Simulação: verificar se o usuário segue @FURIA ou interage com #FURIACS
                var tweets = await _xApiService.GetRecentTweetsAsync($"from:{handle} #FURIACS");
                var followsFuria = tweets != null && tweets.Any(t => t.Text.ToLower().Contains("@furia"));

                return Ok(new
                {
                    Handle = handle,
                    IsValid = tweets != null && (tweets.Any() || followsFuria),
                    Message = tweets != null && (tweets.Any() || followsFuria)
                        ? "Perfil válido! Usuário interage com FURIA ou #FURIACS."
                        : "Perfil não validado. Nenhuma interação com FURIA ou #FURIACS encontrada."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao validar perfil: {ex.Message}");
            }
        }
    }
}