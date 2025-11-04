using APIServerMFE_DeLonghi.Events;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace APIServerMFE_DeLonghi.Controllers
{
    [ApiController]
    [Route("events/robotruntime")]
    public class RobotRuntimeEventController : ControllerBase
    {
        /*****************************************************************************************************
         * [HttpGet]
         * ***************************************************************************************************
        

        /*****************************************************************************************************
         * HttpPost
         * ***************************************************************************************************/
        [HttpPost]
        public IActionResult ReceiveRobotRuntimetEvent([FromBody] RobotRuntimeEventPayload payload)
        {
            if (payload == null || payload.RobotRuntimeEvent == null)
            {
                return BadRequest("Payload non valido.");
            }

            // Crea una StringBuilder per costruire la stringa
            StringBuilder logBuilder = new StringBuilder();

            // Log o elaborazione dell'evento
            logBuilder.AppendLine($"Runtime: RobotId = {payload.RobotRuntimeEvent.RobotId},Timestamp = {payload.RobotRuntimeEvent.Timestamp}");
          
            // Converti tutto in una singola stringa
            string logMessage = logBuilder.ToString();


            Console.WriteLine($"Runtime: RobotId = {payload.RobotRuntimeEvent.RobotId},Timestamp = {payload.RobotRuntimeEvent.Timestamp}");

            // Risposta OK
            //return Ok(new { message = "Evento ricevuto con successo!" });
            return Ok(logMessage);
        }

    }//class
}//namespacee