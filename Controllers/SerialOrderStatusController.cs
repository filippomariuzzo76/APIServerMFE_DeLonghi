using APIServerMFE_DeLonghi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace APIServerMFE_DeLonghi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SerialOrderController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public SerialOrderController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendSerialOrder([FromBody] SerialOrderRequest request)
        {
            if (request?.SerialOrder == null || string.IsNullOrWhiteSpace(request.TargetUrl))
                return BadRequest("Payload mancante o TargetUrl non valido.");

            var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // qui usi l’URL che arriva dal body
            var response = await _httpClient.PostAsync(request.TargetUrl, content);

            var result = await response.Content.ReadAsStringAsync();

            return Ok(new { status = response.StatusCode, response = result });
        }
    }
}
