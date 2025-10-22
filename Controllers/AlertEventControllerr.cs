using APIServerMFE.Events;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace APIServerMFE_DeLonghi.Controllers
{
    [ApiController]
    [Route("events/alert")]
    public class AlertEventController : ControllerBase
    {
        private readonly ILogger<AlertEventController> _logger;

        public AlertEventController(ILogger<AlertEventController> logger)
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
            _logger.LogInformation("Raw Payload Recived: {RawPayload}", rawPayload);

            AlertEventPayload payload = null;

            try
            {
                payload = JsonSerializer.Deserialize<AlertEventPayload>(rawPayload, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (payload == null || payload.AlertEvent == null)
                {
                    _logger.LogWarning("Alert Event Payload not valid received from IP: {ClientIP}", HttpContext.Connection.RemoteIpAddress);
                    return BadRequest("Payload Alert Event invalid.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error nella deserializzazione del payload Alert Event.");
                return BadRequest("Error nella deserializzazione del payload Alert Event.");
            }

            try
            {
                // Create a StringBuilder 
                StringBuilder alertEventBuilder = new StringBuilder();

                // Log o elaborazione dell'evento
                alertEventBuilder.AppendLine($"Event Alert riceved: Name = {payload.AlertEvent.Name}");

#if DEBUG
                Console.WriteLine($"Timestamp: {payload.AlertEvent.Timestamp},Id: {payload.AlertEvent.Id}, Name: {payload.AlertEvent.Name}, Message: {payload.AlertEvent.Message}");
#endif
                alertEventBuilder.AppendLine($"Timestamp: {payload.AlertEvent.Timestamp},Id: {payload.AlertEvent.Id},Name: {payload.AlertEvent.Name},Message: {payload.AlertEvent.Message}");

                // Converti tutto in una singola stringa
                string stringAlerEventBuilder = alertEventBuilder.ToString();

                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                await System.IO.File.AppendAllTextAsync(filePath, stringAlerEventBuilder + Environment.NewLine);

                _logger.LogInformation(stringAlerEventBuilder);

                // Risposta OK
                return Ok(stringAlerEventBuilder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on save AlertEvent.txt");
                return StatusCode(500, "Internal Error during saving file AlertEvent.txt");
            }
        }//receiveevent

    }//class
}//namespacee