using System.Text.Json.Serialization;

namespace APIServerMFE.Events
{

    /*****************************************************************************************
    * 
    * **************************************************************************************/
    public class EventRobotIdentityPayload
    {
        [JsonPropertyName("is-snapshot")]
        public bool IsSnapshot { get; set; }

        [JsonPropertyName("robot-identity-events")]
        public List<RobotIdentityEventItem> RobotIdentityEvents { get; set; }
    }


    /*****************************************************************************************
     * 
     * **************************************************************************************/
    public class RobotIdentityEventItem
    {
        [JsonPropertyName("robot-id")]
        public string RobotId { get; set; }

        [JsonPropertyName("serial-number")]
        public string SerialNumber { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("software-version")]
        public string SoftwareVersion { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

    }

}//namespace
