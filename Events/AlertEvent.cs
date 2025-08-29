using System.Text.Json.Serialization;

namespace APIServerMFE.Events
{

    /*********************************************************************************
     * receive an event when an alert is triggered in the system
     * "DeadlockDetected": Occurs when MiR Fleet detects a deadlock in the system.
     * "RobotError": Occurs when a connected robot reports an error.
     * "RobotEStop": Occurs when a connected robot goes in Protective or Emergency stop
     * ********************************************************************************/
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
