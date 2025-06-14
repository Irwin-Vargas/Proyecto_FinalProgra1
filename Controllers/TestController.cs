using Microsoft.AspNetCore.Mvc;

namespace Proyecto_FinalProgra1.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        // GET: api/test
        [HttpGet]
        public IActionResult GetMensaje()
        {
            return Ok(new
            {
                mensaje = "¡Swagger funciona correctamente!",
                fecha = DateTime.Now
            });
        }

        // GET: api/test/saludo?nombre=Jean
        [HttpGet("saludo")]
        public IActionResult GetSaludo(string nombre)
        {
            return Ok(new
            {
                saludo = $"Hola, {nombre}! Bienvenido a la API de Fast Food 😄"
            });
        }
    }
}
