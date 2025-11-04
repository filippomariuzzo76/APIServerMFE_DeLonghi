using APIServerMFE_DeLonghi.Events;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace APIServerMFE_DeLonghi.Controllers
{
    [ApiController]
    [Route("events/readerbarcoderesult")]
    public class ReaderBarcodeResultEventController : ControllerBase
    {
        private readonly ILogger<ReaderBarcodeResultEventController> _logger;

        public ReaderBarcodeResultEventController(ILogger<ReaderBarcodeResultEventController> logger)
        {
            _logger = logger;
        }

    
        /*****************************************************************************************************
         * HttpPost
         * ***************************************************************************************************/
        [HttpPost]

        public async Task<IActionResult> ReceiveEvent()
        {
            /********************************************************************/

            Request.EnableBuffering();
            using var reader = new StreamReader(Request.Body, Encoding.UTF8, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            Request.Body.Position = 0;

            // 🔍 Log grezzo
            _logger.LogInformation("📦 [ReaderBarcodeResultEvent] Body ricevuto:\n{Body}", body);

            if (string.IsNullOrWhiteSpace(body))
            {
                _logger.LogWarning("⚠️ Body vuoto o nullo — nessun evento ricevuto correttamente.");
                return BadRequest("Body vuoto");
            }

            try
            {
                // tenta di deserializzare
                var data = System.Text.Json.JsonSerializer.Deserialize<ReaderBarcodeResultEventPayload>(body);
                _logger.LogInformation("✅ JSON deserializzato correttamente:\n{Name}", data?.ReaderBarcodeResultEvent?.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Errore nella deserializzazione del JSON ricevuto");
            }



            /********************************************************************/
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd");
            string fileName = $"{timestamp}_ReaderBarcodeResultEvent.txt";
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Logs", fileName);

            _logger.LogInformation("Path of log file : {FilePath}", filePath);

            // Logga il contenuto del payload come JSON
            string rawPayload = await new StreamReader(Request.Body).ReadToEndAsync();
            _logger.LogInformation("ReaderBarcodeResultEvent Raw Payload Received: {RawPayload}", rawPayload);

            ReaderBarcodeResultEventPayload payload = null;

            try
            {
                payload = JsonSerializer.Deserialize<ReaderBarcodeResultEventPayload>(rawPayload, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (payload == null || payload.ReaderBarcodeResultEvent == null)
                {
                    _logger.LogWarning("ReaderBarcodeResultEvent Payload not valid received from IP: {ClientIP}", HttpContext.Connection.RemoteIpAddress);
                    return BadRequest("Payload ReaderbarcodeResult Event invalid.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error nella deserializzazione del payload ReaderBarcodeResult Event.");
                return BadRequest("Error nella deserializzazione del payload ReaderBarcodeResult Event.");
            }

            try
            {
                // Create a StringBuilder 
                StringBuilder readerBarcodeResultEventBuilder = new StringBuilder();

                // Log o elaborazione dell'evento
                //readerBarcodeResultEventBuilder.AppendLine($"Event ReaderBarcodeResult riceved: Plc1: {payload.ReaderBarcodeResultEvent.Payload.Plc1},Plc2: {payload.ReaderBarcodeResultEvent.Plc2}");
                readerBarcodeResultEventBuilder.AppendLine($"Event ReaderBarcodeResult riceived: Name: {payload.ReaderBarcodeResultEvent.Name}, Plc1: {payload.ReaderBarcodeResultEvent.Payload.Plc1}, Plc2: {payload.ReaderBarcodeResultEvent.Payload.Plc2}");

                // Converti tutto in una singola stringa
                string stringReaderBarcodeResultEventBuilder = readerBarcodeResultEventBuilder.ToString();

                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                await System.IO.File.AppendAllTextAsync(filePath, stringReaderBarcodeResultEventBuilder + Environment.NewLine);

                _logger.LogInformation(stringReaderBarcodeResultEventBuilder);

                // Risposta OK
                return Ok(stringReaderBarcodeResultEventBuilder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on save ReaderBarcodeResultEvent.txt");
                return StatusCode(500, "Internal Error during saving file ReaderBarcodeResultEvent.txt");
            }
        }//receiveevent

    }//class
}//namespacee