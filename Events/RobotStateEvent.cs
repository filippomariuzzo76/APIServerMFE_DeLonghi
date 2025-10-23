using System.Text.Json;
using System.Text.Json.Serialization;

namespace APIServerMFE_DeLonghi.Events
{

    /*****************************************************************************************
     * 
     * **************************************************************************************/
    public class RobotStateEventItem
    {
        [JsonPropertyName("robot-id")]
        public string RobotId { get; set; }

        [JsonPropertyName("robot-state")]
        public Dictionary<string, JsonElement> RobotStateRaw { get; set; }

        [JsonIgnore]
        public string RobotState => RobotStateRaw?.Keys.FirstOrDefault(); // Estrai lo stato dalla chiave

        [JsonIgnore]
        public string? NotOperationalState
        {
            get
            {
                if (RobotState == "not-operational" && RobotStateRaw.TryGetValue("not-operational", out var subState))
                {
                    if (subState.ValueKind == JsonValueKind.Object)
                    {
                        var subStateDict = JsonSerializer.Deserialize<Dictionary<string, object>>(subState.GetRawText());
                        return subStateDict?.Keys.FirstOrDefault();
                    }
                }
                return null;
            }
        }


        [JsonPropertyName("robot-end-state")]
        public string? RobotEndState { get; set; }

        [JsonPropertyName("battery-percentage")]
        public int BatteryPercentage { get; set; }

        [JsonPropertyName("battery-time-remaining-in-minutes")]
        public string BatteryTimeRemainingInMinutes { get; set; }

        [JsonPropertyName("total-distance-moved-in-meters")]
        public double TotalDistanceMovedinMeters { get; set; }

        [JsonPropertyName("pose-x")]
        public double PoseX { get; set; }

        [JsonPropertyName("pose-y")]
        public double PoseY { get; set; }

        [JsonPropertyName("pose-orientation")]
        public double PoseOrientation { get; set; }

        [JsonPropertyName("map-id")]
        public string MapId { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }
    }


    /*****************************************************************************************
     * 
      * **************************************************************************************/
    //public class EventRobotStatePayload
    //{

    //    [JsonPropertyName("robot-state-event")]
    //    public RobotStateEventItem RobotStateEvent { get; set; }
    //}

}//namespace
