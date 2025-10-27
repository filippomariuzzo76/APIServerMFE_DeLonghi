using System;
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
    [Route("events/robotstate")]
    public class RobotStateEventController : ControllerBase
    {
        private readonly ILogger<RobotStateEventController> _logger;
        private readonly IConfiguration _appSettings;
        private readonly RobotEnumerators _robotEnum;

        public RobotStateEventController(ILogger<RobotStateEventController> logger, IConfiguration appSettings, RobotEnumerators robotEnum)
        {
            _logger = logger;
            _appSettings = appSettings;
            _robotEnum = robotEnum;
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
            string fileName = $"{timestamp}_RobotStateEvent.txt";
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Logs", fileName);

            _logger.LogInformation("Path of log file : {FilePath}", filePath);

            // Logga il contenuto del payload come JSON
            string rawPayload = await new StreamReader(Request.Body).ReadToEndAsync();
            _logger.LogInformation("Robot State Event Raw Payload Recived: {RawPayload}", rawPayload);

            RobotStateEventItem payload = null;

            try
            {               
                payload = JsonSerializer.Deserialize<RobotStateEventItem>(rawPayload, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (payload == null)
                {
                    _logger.LogWarning("RobotStateEvent Payload not valid received from IP: {ClientIP}", HttpContext.Connection.RemoteIpAddress);
                    return BadRequest("RobotStateEvent Payload invalid.");               
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error nella deserializzazione del payload Robot State Event.");
                return BadRequest("Error nella deserializzazione del payload.");
            }

            try
            {
               /********************************************************************************************************************************
                * Create Export Log of Distance Mover in Meters
                * *****************************************************************************************************************************/
                ExportDistanceMoverLogFile( payload, _appSettings);


                /********************************************************************************************************************************
                 * Export All Event RobotState in Log 
                 * *****************************************************************************************************************************/

                // Create a StringBuilder 
                StringBuilder robotStateBuilder = new StringBuilder();

                var robotStateEvent = payload;
                
                #if DEBUG
                Console.WriteLine($"Robot ID: {robotStateEvent.RobotId}, Robot State: {robotStateEvent.RobotState}");
                Debug.WriteLine($"Robot ID: {robotStateEvent.RobotId}, Robot State: {robotStateEvent.RobotState}");
                #endif

                robotStateBuilder.AppendLine($"Robot ID: {robotStateEvent.RobotId}, " +
                                             $"Robot State: {robotStateEvent.RobotState} {robotStateEvent.NotOperationalState}, " +
                                             $"Robot End State: {robotStateEvent.RobotEndState}, " +
                                             $"Battery Percentage: {robotStateEvent.BatteryPercentage}, " +
                                             $"Battery Time Remaining: {robotStateEvent.BatteryTimeRemainingInMinutes}, " +
                                             $"Total Distance: {robotStateEvent.TotalDistanceMovedinMeters}, " +
                                             $"Timestamp: {robotStateEvent.Timestamp}");

                // Converti tutto in una singola stringa
                string stringRobotStateBuilder = robotStateBuilder.ToString();

                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                await System.IO.File.AppendAllTextAsync(filePath, stringRobotStateBuilder + Environment.NewLine);

                _logger.LogInformation(stringRobotStateBuilder);

                // Risposta OK
                return Ok(stringRobotStateBuilder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on save RobotStateEvent.txt");
                return StatusCode(500, "Internal Error during saving file");
            }
        }//receiveevent



        /**************************************************************************************************************************************
         * Export in Daily Mission Autocharging Log File
         * 
         * ***********************************************************************************************************************************/
        private bool ExportDistanceMoverLogFile(RobotStateEventItem payload, IConfiguration _appSetting)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM");
            string today = DateTime.Now.ToString("yyyy-MM-dd");
            string folderPath = _appSetting["ExportDirectoryName"];
            string fileName = $"{timestamp}_{_appSetting["ExportFileDistanceName"] ?? "DistanceMover.txt"}";
            string filePath = Path.Combine(folderPath, fileName);

            try
            {
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                    _logger.LogInformation("Success Create Directory: {folderPath}", folderPath);
                }

                Dictionary<string, (double DailyDistance, double PreviousTotal, string RobotName)> dailyRecords = new();
                double previousTotal = 0;

                // Se il file esiste, leggi i dati esistenti
                if (System.IO.File.Exists(filePath))
                {
                    var lines = System.IO.File.ReadAllLines(filePath);
                    foreach (var line in lines)
                    {
                        var parts = line.Split(';');
                        if (parts.Length >= 4)
                        {
                            string date = parts[0].Trim();
                            double dailyDistance = double.Parse(parts[1].Trim());
                            double prevTotal = double.Parse(parts[2].Trim());
                            string nameRobot = parts[3].Trim();
                            dailyRecords[date] = (dailyDistance,prevTotal, nameRobot);

                            // Trova l'ultima data registrata prima di oggi
                            if (DateTime.TryParse(date, out DateTime parsedDate) && parsedDate < DateTime.Parse(today))
                            {
                                previousTotal = prevTotal;
                            }
                        }
                    }
                }
                // Ottieni il nome del robot
                string robotName = _robotEnum.GetRobotName(payload.RobotId);

                // Calcola la distanza giornaliera rispetto al valore totale di ieri
                double dailyDistanceToday = Math.Round(payload.TotalDistanceMovedinMeters - previousTotal);

                // Assicurarsi che la distanza non sia negativa (caso di reset del contatore)
                if (dailyDistanceToday < 0)
                {
                    dailyDistanceToday = Math.Round(payload.TotalDistanceMovedinMeters);
                }

                // Aggiorna il record per il giorno corrente
                dailyRecords[today] = (dailyDistanceToday, Math.Round(payload.TotalDistanceMovedinMeters), robotName);

                // Ricostruisci il file con i dati aggiornati
                using (StreamWriter writer = new StreamWriter(filePath, false)) // false per sovrascrivere
                {
                    foreach (var entry in dailyRecords)
                    {
                        writer.WriteLine($"{entry.Key}; {entry.Value.DailyDistance}; {entry.Value.PreviousTotal}; {entry.Value.RobotName}");
                    }
                }

                _logger.LogInformation("Updated Distance Log: {Date} -> {DailyDistance} meters, RobotID: {RobotId}",
                        today, dailyDistanceToday, payload.RobotId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on ExportDistanceMoverLogFile");
                return false;
            }
            return true;
        }//end
       }//class

    }//namespacee