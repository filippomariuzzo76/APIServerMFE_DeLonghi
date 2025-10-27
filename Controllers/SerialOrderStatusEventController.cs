using System;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Threading.Tasks;
using APIServerMFE_DeLonghi.Events;

namespace APIServerMFE_DeLonghi.Controllers
{
    [ApiController]
    //[Route("events/serial-order-status")]
    [Route("events/serialorderstatus")]
    public class SerialOrderStatusEventController : ControllerBase
    {    
        private readonly ILogger<SerialOrderStatusEventController> _logger;
        private readonly MissionMirManager _missionManager;
        private readonly IConfiguration _appSettings;
        private readonly SerialOrderStatusService _serialOrderStatusService;
        private readonly AutochargingOrderStatusService _autochargingOrderStatusService;
        private readonly RobotEnumerators _robotEnum;

        public SerialOrderStatusEventController(
            ILogger<SerialOrderStatusEventController> logger, 
            MissionMirManager missionManager,
            IConfiguration appSettings,
            SerialOrderStatusService serialOrderStatusService,
            AutochargingOrderStatusService autochargingOrderStatusService,
            RobotEnumerators robotEnum)
        {
            _logger = logger;
            _missionManager = missionManager;
            _appSettings = appSettings;
            _serialOrderStatusService = serialOrderStatusService;
            _autochargingOrderStatusService = autochargingOrderStatusService;
            _robotEnum = robotEnum;
        }

        /*****************************************************************************************************
         * [HttpGet]
         * ***************************************************************************************************
        

        /*****************************************************************************************************
         * Gestisce gli eventi di stato degli ordini seriali.
         * <param name="payload">Payload dell'evento ricevuto</param>
         * <returns> Risultato dell'elaborazione</returns>
         * ***************************************************************************************************/
        [HttpPost]
       
         public async Task<IActionResult> ReceiveEvent()
         {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd");
            string fileName = $"{timestamp}_SerialOrderstatusEvent.txt";
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Logs", fileName);

            _logger.LogInformation("Path of log file : {FilePath}", filePath);

            // Logga il contenuto del payload come JSON
            string rawPayload = await new StreamReader(Request.Body).ReadToEndAsync();
            _logger.LogInformation("Serial Order Status Raw Payload Recived: {RawPayload}", rawPayload);

            EventSerialOrderPayload payload = null;

            try
            {
               
                payload = JsonSerializer.Deserialize<EventSerialOrderPayload>(rawPayload, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (payload == null || payload.SerialOrderStatusEvents == null)
                {
                    _logger.LogWarning("SerialOrderStatusEvent Payload not valid received from IP: {ClientIP}", HttpContext.Connection.RemoteIpAddress);
                    return BadRequest("Payload invalid.");               
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
                StringBuilder serialOrderStatusBuilder = new StringBuilder();

                /********************************************************************************************************************************
                 * Create Export Log Missions
                 * *****************************************************************************************************************************/
                 ExportMissionsInLogFile(payload, _missionManager, _appSettings);

                /********************************************************************************************************************************
                 * Create Export Log Missions Autocharging 
                 * *****************************************************************************************************************************/
                 //ExportAutochargingInLogFile(payload, _missionManager, _appSettings);

                /********************************************************************************************************************************
                 * Create Log Files All Events
                 * *****************************************************************************************************************************/

                // Log o elaborazione dell'evento
                serialOrderStatusBuilder.AppendLine($"Event Serial Order Status riceved: IsSnapshot = {payload.IsSnapshot}");

                foreach (var statusEvent in payload.SerialOrderStatusEvents)
                {
                    #if DEBUG
                    Console.WriteLine($"SerialOrderId: {statusEvent.SerialOrderId}, State: {statusEvent.State}");
                    Debug.WriteLine($"SerialOrderId: {statusEvent.SerialOrderId}, State: {statusEvent.State}");
                    Console.WriteLine($"RobotId: {statusEvent.RobotId}");
                    #endif
                    serialOrderStatusBuilder.AppendLine(
                        $"SerialOrderId: {statusEvent.SerialOrderId}," +
                        $"PhaseIndex: {statusEvent.PhaseIndex}," +
                        $"State: {statusEvent.State}," +
                        $"StateChangeTimestamp: {statusEvent.StateChangeTimestamp}," +
                        $"RobotId: {statusEvent.RobotId}," +
                        $"Message: {statusEvent.Message}," +
                        $"IsFallback: {statusEvent.IsFallback}," +
                        $"OrderType: {statusEvent.OrderType} ");
                }//foreach

                // Converti tutto in una singola stringa
                string stringSerialOrderStatus = serialOrderStatusBuilder.ToString();

                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                await System.IO.File.AppendAllTextAsync(filePath, stringSerialOrderStatus + Environment.NewLine);

                _logger.LogInformation(stringSerialOrderStatus);

                // Risposta OK
                return Ok(stringSerialOrderStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on save SerialOrderStarus.txt");
                return StatusCode(500, "Internal Error during saving file");
            }
        }//receiveevent


        /**************************************************************************************************************************************
         * Export in Daily Mission Log File
         * 
         * ***********************************************************************************************************************************/
        public bool ExportMissionsInLogFile(EventSerialOrderPayload payload, MissionMirManager _missionManager, IConfiguration _appSetting)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd");
            string folderPath = _appSetting["ExportDirectoryName"];
            string fileName = $"{timestamp}_{_appSetting["ExportFileMissionName"] ?? "Missions.txt"}";
            string filePath = Path.Combine(folderPath, fileName);

            StringBuilder serialOrderStatusBuilder = new StringBuilder();
 
            try
            {
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                    _logger.LogInformation("Success Create Directory: {folderPath}", folderPath);
                }

                List<SerialOrderStatusEventItem> serialOrdersList = _serialOrderStatusService.GetOrders();

                foreach (var statusEvent in payload.SerialOrderStatusEvents)
                {
                    string serialOrderId = statusEvent.SerialOrderId;
                    bool find = serialOrdersList.Any(order => order.SerialOrderId == serialOrderId);

                    if (find && 
                       (statusEvent.State == Enumerators.MissionState.Finished.ToString() || 
                        statusEvent.State == Enumerators.MissionState.Aborted.ToString()))
                    {
                        MissionMir mission = _missionManager.GetMissionById(statusEvent.MissionId);
                        string robotName = _robotEnum.GetRobotName(statusEvent.RobotId);

                        if (mission != null)
                        {
                            serialOrderStatusBuilder.AppendLine(
                                 $"{statusEvent.StateChangeTimestamp};" +  //StateChangeTimestamp
                                 $"{mission.TypeMission};" +  //Type of mission
                                 $"{mission.ProductionLine}{mission.ProductionSubLine};" + //Start point
                                 $"{statusEvent.State};" + //State
                                 $"{robotName};" +
                                 $"{statusEvent.SerialOrderId}"); 
                            
                            // Converti tutto in una singola stringa
                            string stringSerialOrderStatus = serialOrderStatusBuilder.ToString();
                            System.IO.File.AppendAllTextAsync(filePath, stringSerialOrderStatus.TrimEnd() + Environment.NewLine);

                            //remove from list
                            serialOrdersList.RemoveAll(order => order.SerialOrderId == serialOrderId);
                        }                      
                    }
                    else if (statusEvent.OrderType == Enumerators.OrderType.Ui.ToString() || 
                             statusEvent.OrderType == Enumerators.OrderType.Api.ToString())
                    {
                        if (statusEvent.State == Enumerators.MissionState.Created.ToString())
                        {
                            serialOrdersList.Add(statusEvent);
                        }
                    }
                }//foreach              
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on ExportLogMissions");
                return false;
            }
            return true;
        }//end

        /**************************************************************************************************************************************
          * Export in Daily Mission Autocharging Log File
          * 
          * ***********************************************************************************************************************************/
        public bool ExportAutochargingInLogFile(EventSerialOrderPayload payload, MissionMirManager _missionManager, IConfiguration _appSetting)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd");
            string folderPath = _appSetting["ExportDirectoryName"];
            string fileName = $"{timestamp}_{_appSetting["ExportFileAutochargingName"] ?? "Autocharging.txt"}";
            string filePath = Path.Combine(folderPath, fileName);

            StringBuilder autochargingOrderStatusBuilder_Start = new StringBuilder();
            StringBuilder autochargingOrderStatusBuilder_End = new StringBuilder();

            try
            {
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                    _logger.LogInformation("Success Create Directory: {folderPath}", folderPath);
                }
                var autochargingOrdersList = _autochargingOrderStatusService.GetOrders()?.ToList(); 

                foreach (var statusEvent in payload.SerialOrderStatusEvents)
                {                 
                    var autochargingOrderFound = autochargingOrdersList.FirstOrDefault(order => order.SerialOrderId == statusEvent.SerialOrderId && order.RobotId == statusEvent.RobotId);

                    if (autochargingOrderFound == null && statusEvent.OrderType != Enumerators.OrderType.Autocharging.ToString())
                    {
                            string robotName = _robotEnum.GetRobotName(autochargingOrdersList.First().RobotId);
                            double durationMinutes = Math.Round((statusEvent.StateChangeTimestamp - autochargingOrdersList.First().StateChangeTimestamp).TotalMinutes);
                  

                            autochargingOrderStatusBuilder_End.AppendLine(
                                         $"{statusEvent.StateChangeTimestamp};" +  //StateChangeTimestamp
                                         $"{autochargingOrdersList.First().OrderType};" +  //Type of order
                                         $"{durationMinutes};" + //duration of autocharging
                                         $"{robotName};" +
                                         $"{autochargingOrdersList.First().SerialOrderId}");

 
                            string stringAutochargingOrderStatus_end = autochargingOrderStatusBuilder_End.ToString();
                            System.IO.File.AppendAllTextAsync(filePath, stringAutochargingOrderStatus_end.TrimEnd() + Environment.NewLine).Wait();
                        //remove from list
                        _autochargingOrderStatusService.RemoveOrder(autochargingOrdersList.First().SerialOrderId);                            
                    }

                    if (statusEvent.OrderType == Enumerators.OrderType.Autocharging.ToString() && statusEvent.RobotId.Trim() != String.Empty)
                    {
                        _autochargingOrderStatusService.AddOrder(statusEvent);
                    }
                }//foreach              
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on ExportAutochargingInLogFile");
                return false;
            }
            return true;
        }//end

    }//class

    /***********************************************************************************************************************
     * Class to represent Object SerialOrderStatusService
     * 
     * *********************************************************************************************************************/
    public class SerialOrderStatusService
    {
        private readonly List<SerialOrderStatusEventItem> _serialOrdersList = new List<SerialOrderStatusEventItem>();

        public List<SerialOrderStatusEventItem> GetOrders() => _serialOrdersList;

        public void AddOrder(SerialOrderStatusEventItem order)
        {
            if (!_serialOrdersList.Any(o => o.SerialOrderId == order.SerialOrderId))
            {
                _serialOrdersList.Add(order);
            }
        }
        public void RemoveOrder(string serialOrderId)
        {
            _serialOrdersList.RemoveAll(o => o.SerialOrderId == serialOrderId);
        }
    }//class

    /***********************************************************************************************************************
     * Class to represent Object Autocharging
     * 
     * *********************************************************************************************************************/
    public class AutochargingOrderStatusService
    {
        private readonly List<SerialOrderStatusEventItem> _autochargingOrdersList = new List<SerialOrderStatusEventItem>();

        public List<SerialOrderStatusEventItem> GetOrders() => _autochargingOrdersList;

        public void AddOrder(SerialOrderStatusEventItem order)
        {
            if (!_autochargingOrdersList.Any(o => o.SerialOrderId == order.SerialOrderId))
            {
                _autochargingOrdersList.Add(order);
            }
        }
        public void RemoveOrder(string serialOrderId)
        {
            _autochargingOrdersList.RemoveAll(o => o.SerialOrderId == serialOrderId);
        }
    }//class

}//namespacee


