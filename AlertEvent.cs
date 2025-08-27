using System.Text.Json.Serialization;

namespace APIServerMFE
{

    /*************************************************************************************************
     * 
     *  id: Identification string of the alert.
     *  name:   "DeadlockDetected": Occurs when MiR Fleet detects a deadlock in the system.
     *          "RobotError": Occurs when a connected robot reports an error.
     *          "RobotEStop": Occurs when a connected robot goes in Protective or Emergency stop.
     *  message: More information about the alert. This is the same as the message shown in the interface.
     *  timestamp: The time the alert was recorded
     * *****:***************************************************************************************/
    public class AlertEvent
    {
        [JsonPropertyName("id")] 
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; }

    }


    /*****************************************************************************************
    * 
   * **************************************************************************************/
    public class AlertEventPayload
    {
        [JsonPropertyName("Alert Event")]
        public AlertEvent AlertEvent { get; set; }

    }

}//namespace
