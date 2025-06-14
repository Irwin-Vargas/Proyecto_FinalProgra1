using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Proyecto_FinalProgra1.Services;
using System.Threading.Tasks;

namespace Proyecto_FinalProgra1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly OpenAiChatService _chatService;

        public ChatController(IConfiguration configuration)
        {
            var apiKey = configuration["OpenAI:ApiKey"];
            _chatService = new OpenAiChatService(apiKey);
        }

        [HttpPost("Ask")]
        public async Task<IActionResult> Ask([FromBody] string userMessage)
        {
            if (string.IsNullOrWhiteSpace(userMessage))
                return BadRequest("Mensaje vacío");

            var response = await _chatService.GetChatResponse(userMessage);
            return Ok(new { respuesta = response });
        }
    }
}
