using System.Text.Json.Serialization;

namespace APIServerMFE_DeLonghi.Events
{

    /*********************************************************************************
     * Classe per rappresentare l'evento "ReaderBarcodeResult Event" 
     * 
     * 
     * 
     * ********************************************************************************/
    //public class ReaderBarcodeResultEventPayload
    //{
    //    [JsonPropertyName("ReaderBarcodeResult Event")]
    //    public ReaderBarcodeResultPayload ReaderBarcodeResultEvent { get; set; }
    //}

    /*****************************************************************************************
     * 
     * **************************************************************************************/
    public class ReaderBarcodeResultEventPayload
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("payload")]
        public PayloadData Payload { get; set; }

        [JsonPropertyName("robot-id")]
        public string RobotId { get; set; }

        [JsonPropertyName("serial-order-id")]
        public string SerialOrderId { get; set; }

        [JsonPropertyName("phase-index")]
        public int? PhaseIndex { get; set; }

        [JsonPropertyName("is-fallback")]
        public bool? IsFallback { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

    }

    /*****************************************************************************************
     * 
     * **************************************************************************************/
    public class PayloadData
    {
        [JsonPropertyName("plc1")]
        public string Plc1 { get; set; }

        [JsonPropertyName("plc2")]
        public string Plc2 { get; set; }
    }

}//namespace
