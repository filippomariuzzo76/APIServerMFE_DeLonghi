using APIServerMFE_DeLonghi.Events;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace APIServerMFE_DeLonghi.Controllers
{
    [ApiController]
    [Route("events/error")]
    public class ErrorEventController : ControllerBase
    {
        private readonly ILogger<ErrorEventController> _logger;

        public ErrorEventController(ILogger<ErrorEventController> logger)
        {
            _logger = logger;
        }

        /*****************************************************************************************************
         * [HttpGet]
         * ***************************************************************************************************
        

        /*****************************************************************************************************
         * HttpPost
         * ***************************************************************************************************/
        [HttpPost]

        public async Task<IActionResult> ReceiveEvent()
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd");
            string fileName = $"{timestamp}_AlertEvent.txt";
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Logs", fileName);

            _logger.LogInformation("Path of log file : {FilePath}", filePath);

            // Logga il contenuto del payload come JSON
            string rawPayload = await new StreamReader(Request.Body).ReadToEndAsync();
            _logger.LogInformation("Error Event Raw Payload Recived: {RawPayload}", rawPayload);

            ErrorEventPayload payload = null;

            try
            {
                payload = JsonSerializer.Deserialize<ErrorEventPayload>(rawPayload, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (payload == null || payload.ErrorEvent == null)
                {
                    _logger.LogWarning("Error Event Payload not valid received from IP: {ClientIP}", HttpContext.Connection.RemoteIpAddress);
                    return BadRequest("Payload Error Event invalid.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error nella deserializzazione del payload Error Event.");
                return BadRequest("Error nella deserializzazione del payload Error Event.");
            }

            try
            {
                // Create a StringBuilder 
                StringBuilder errorEventBuilder = new StringBuilder();

                // Log o elaborazione dell'evento
                errorEventBuilder.AppendLine($"Event Error riceved: Name = {payload.ErrorEvent.EntityType}");

#if DEBUG
                Console.WriteLine($"Timestamp: {payload.ErrorEvent.Timestamp},ActionType: {payload.ErrorEvent.ActionType},EntityType: {payload.ErrorEvent.EntityType},EntityId: {payload.ErrorEvent.ErrorType},Message: {payload.ErrorEvent.Message}");
#endif
                errorEventBuilder.AppendLine($"Timestamp: {payload.ErrorEvent.Timestamp},ActionType: {payload.ErrorEvent.ActionType},EntityType: {payload.ErrorEvent.EntityType},EntityId: {payload.ErrorEvent.ErrorType},Message: {payload.ErrorEvent.Message}");

                // Converti tutto in una singola stringa
                string stringErrorEventBuilder = errorEventBuilder.ToString();

                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                await System.IO.File.AppendAllTextAsync(filePath, stringErrorEventBuilder + Environment.NewLine);

                _logger.LogInformation(stringErrorEventBuilder);

                // Risposta OK
                return Ok(stringErrorEventBuilder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on save ErrorEvent.txt");
                return StatusCode(500, "Internal Error during saving file ErrorEvent.txt");
            }
        }//receiveevent

    }//class
}//namespacee