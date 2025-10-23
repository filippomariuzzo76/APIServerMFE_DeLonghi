using System.IO;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Threading.Tasks;
using APIServerMFE_DeLonghi.Events;

namespace APIServerMFE_DeLonghi.Controllers
{
    [ApiController]
    [Route("events/robotidentity")]
    public class RobotIdentityEventController : ControllerBase
    {
        private readonly ILogger<RobotIdentityEventController> _logger;

        public RobotIdentityEventController(ILogger<RobotIdentityEventController> logger)
        {
            _logger = logger;
        }

        /*****************************************************************************************************
         * [HttpGet]
         * ***************************************************************************************************
        

        /*****************************************************************************************************
         * Gestisce gli eventi di stato degli ordini seriali.
         * <param name="payload">Payload dell'evento ricevuto</param>
         * <returns>Risultato dell'elaborazione</returns>
         * ***************************************************************************************************/
        [HttpPost]
         
         public async Task<IActionResult> ReceiveEvent()
         {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd");
            string fileName = $"{timestamp}_RobotIdentityEvent.txt";
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Logs", fileName);

            _logger.LogInformation("Path of log file : {FilePath}", filePath);

            // Logga il contenuto del payload come JSON
            string rawPayload = await new StreamReader(Request.Body).ReadToEndAsync();
            _logger.LogInformation("Robot Identity Raw Payload Recived: {RawPayload}", rawPayload);

            EventRobotIdentityPayload payload = null;

            try
            {
               
                payload = JsonSerializer.Deserialize<EventRobotIdentityPayload>(rawPayload, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (payload == null || payload.RobotIdentityEvents == null)
                {
                    _logger.LogWarning("RobotIDentityEvent Payload not valid received from IP: {ClientIP}", HttpContext.Connection.RemoteIpAddress);
                    return BadRequest("RobotIDentityEvent Payload invalid.");               
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error nella deserializzazione del payload.");
                return BadRequest("Error nella deserializzazione del payload.");
            }

            try
            {
                // Create a StringBuilder 
                StringBuilder robotIdentityBuilder = new StringBuilder();

                // Log o elaborazione dell'evento
                robotIdentityBuilder.AppendLine($"Event Robot Identity riceved: IsSnapshot = {payload.IsSnapshot}");

                foreach (var robotIdentityEvent in payload.RobotIdentityEvents)
                {
                    #if DEBUG
                    Console.WriteLine($"Robot ID: {robotIdentityEvent.RobotId}, Serial Number: {robotIdentityEvent.SerialNumber}, Name: {robotIdentityEvent.Name}");
                    Debug.WriteLine($"Robot ID: {robotIdentityEvent.RobotId}, Serial Number: {robotIdentityEvent.SerialNumber}");
                    #endif

                    robotIdentityBuilder.AppendLine($"Robot ID: {robotIdentityEvent.RobotId},Serial Number: {robotIdentityEvent.SerialNumber},Name: {robotIdentityEvent.Name},Model: {robotIdentityEvent.Model} " +
                        $"Software: {robotIdentityEvent.SoftwareVersion}, Timestamp: {robotIdentityEvent.Timestamp} ");
                }

                // Converti tutto in una singola stringa
                string stringRobotIdentityBuilder = robotIdentityBuilder.ToString();

                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                await System.IO.File.AppendAllTextAsync(filePath, stringRobotIdentityBuilder + Environment.NewLine);

                _logger.LogInformation(stringRobotIdentityBuilder);

                // Risposta OK
                return Ok(stringRobotIdentityBuilder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on save RobotIdentityEvent.txt");
                return StatusCode(500, "Internal Error during saving file");
            }
        }//receiveevent
       
    }//class
}//namespacee