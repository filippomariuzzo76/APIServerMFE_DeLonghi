using System.Text.Json.Serialization;

namespace APIServerMFE_DeLonghi.Events
{

    /*****************************************************************************************
    * 
    * **************************************************************************************/
    public class EventSerialOrderPayload
    {
        [JsonPropertyName("is-snapshot")]
        public bool IsSnapshot { get; set; }

        [JsonPropertyName("serial-order-status-events")]
        public List<SerialOrderStatusEventItem> SerialOrderStatusEvents { get; set; }
    }


    /*****************************************************************************************
     * 
     * **************************************************************************************/
    public class SerialOrderStatusEventItem
    {
        [JsonPropertyName("serial-order-id")]
        public string SerialOrderId { get; set; }

        [JsonPropertyName("phase-index")]
        public int? PhaseIndex { get; set; }

        [JsonPropertyName("mission-id")]
        public string MissionId { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }

        [JsonPropertyName("state-change-timestamp")]
        public DateTime StateChangeTimestamp { get; set; }

        [JsonPropertyName("robot-id")]
        public string RobotId { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("is-fallback")]
        public bool? IsFallback { get; set; }

        [JsonPropertyName("order-type")]
        public string OrderType { get; set; }
    }
}//namespace
