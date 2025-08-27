using System.Text.Json.Serialization;

namespace APIServerMFE.Events
{

    /******************************************************************************
     * 
     * ***************************************************************************/
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
