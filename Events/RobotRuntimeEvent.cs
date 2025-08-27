using System.Text.Json.Serialization;

namespace APIServerMFE.Events
{

    /******************************************************************************
     * 
     * ***************************************************************************/
    public class RobotRuntimeEvent
    {
        [JsonPropertyName("robot-id")]
        public string RobotId { get; set; }

        [JsonPropertyName("pose-x")]
        public double PoseX { get; set; }

        [JsonPropertyName("pose-y")]
        public double PoseY { get; set; }

        [JsonPropertyName("pose-orientation")]
        public double PoseOrientation { get; set; }

        [JsonPropertyName("map-id")]
        public string MapId { get; set; }

        [JsonPropertyName("in-emergency-stop")]
        public bool InEmergencyStop { get; set; }

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; }
    }

    /*****************************************************************************************
    * 
   * **************************************************************************************/
    public class RobotRuntimeEventPayload
    {
        [JsonPropertyName("RobotRuntime Event")]
        public RobotRuntimeEvent RobotRuntimeEvent { get; set; }
    }
}//namespace
