using System.Text.Json.Serialization;

namespace APIServerMFE.Events
{

    /******************************************************************************
     * 
     * ***************************************************************************/
    public class ErrorEvent
    {
        [JsonPropertyName("action-type")]
        public string ActionType { get; set; }

        [JsonPropertyName("entity-type")]
        public string EntityType { get; set; }

        [JsonPropertyName("entity-id")]
        public string EntityId { get; set; }

        [JsonPropertyName("error-type")]
        public string ErrorType { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; }

    }


    /*****************************************************************************************
    * 
   * **************************************************************************************/
    public class ErrorEventPayload
    {
        [JsonPropertyName("Error Event")]
        public ErrorEvent ErrorEvent { get; set; }
    }

}//namespace
