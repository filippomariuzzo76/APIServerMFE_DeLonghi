namespace APIServerMFE_DeLonghi.Events
{
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;

    /*******************************************************************************************************************************
     * Class implements POST Subsciption of Events on MFE server 
     * 
     * ****************************************************************************************************************************/

    public class SubscriptionEventsService
    {
        private readonly ILogger<SubscriptionEventsService> _logger;
        private readonly HttpClient _httpClient;

        private readonly string _mfeUrl;          // URL del server MFE
        private readonly string _subscriptionUrl;//endpoint POST per la sottoscrizione
        private readonly string _webhookUrl; //URL del webhook
        private readonly string _webhookPort;  // Porta webhook
        private readonly string _apiKey;

        private readonly bool _isAlertEvent;
        private readonly bool _isRobotRuntimeEvent;
        private readonly bool _isSerialOrderStatusEvent;
        private readonly bool _isErrorEvent;
        private readonly bool _isRobotIdentityEvent;
        private readonly bool _isRobotStateEvent;
        private readonly bool _isReaderBarcodeResultEvent;

        /***************************************************************************************************************************************
         * 
         * *************************************************************************************************************************************/
        public SubscriptionEventsService(
            ILogger<SubscriptionEventsService> logger, 
            HttpClient httpClient, 
            string mfeUrl, 
            string webhookUrl, 
            string webhookPort, 
            string apiKey, 
            bool isAlertEvent, 
            bool isRobotRuntimeEvent, 
            bool isSerialOrderStatusEvent, 
            bool isErrorEvent, 
            bool isRobotIdentityEvent, 
            bool isRobotStateEvent,
            bool isReaderBarcodeResultEvent)
        {
            _logger = logger;
            _httpClient = httpClient;

            _mfeUrl = mfeUrl.TrimEnd('/');
            //_subscriptionUrl = $"{mfeUrl}:{webhookPort}/api/v1/subscription";
            _subscriptionUrl = $"{_mfeUrl}/api/v1/subscription";

            _webhookUrl = webhookUrl;
            _webhookPort = webhookPort;           
            _apiKey = apiKey;

            _isAlertEvent = isAlertEvent;
            _isRobotRuntimeEvent = isRobotRuntimeEvent;
            _isSerialOrderStatusEvent = isSerialOrderStatusEvent;
            _isErrorEvent = isErrorEvent;
            _isRobotIdentityEvent = isRobotIdentityEvent;
            _isRobotStateEvent = isRobotStateEvent;
            _isReaderBarcodeResultEvent = isReaderBarcodeResultEvent;
        }

        /************************************************************************************
         * 
         * **********************************************************************************/
        public async Task SubscribeAsync()
        {
            var endpoints = new List<Endpoint>();

            if (_isAlertEvent)
            {
                endpoints.Add(new Endpoint
                {
                    EventType = "Alert",
                    EndpointPaths = new[] { "events/alert" }
                });
            }

            if (_isRobotRuntimeEvent)
            {
                endpoints.Add(new Endpoint
                {
                    EventType = "RobotRuntime",
                    EndpointPaths = new[] { "events/robotruntime" }
                });
            }

            if (_isSerialOrderStatusEvent)
            {
                endpoints.Add(new Endpoint
                {
                    EventType = "SerialOrderStatus",
                    EndpointPaths = new[] { "events/serialorderstatus" }
                });
            }

            if (_isErrorEvent)
            {
                endpoints.Add(new Endpoint
                {
                    EventType = "Error",
                    EndpointPaths = new[] { "events/error" }
                });
            }

            if (_isRobotIdentityEvent)
            {
                endpoints.Add(new Endpoint
                {
                    EventType = "RobotIdentity",
                    EndpointPaths = new[] { "events/robotidentity" }
                });
            }

            if (_isRobotStateEvent)
            {
                endpoints.Add(new Endpoint
                {
                    EventType = "RobotState",
                    EndpointPaths = new[] { "events/robotstate" }
                });
            }

            if (_isReaderBarcodeResultEvent)
            {
                endpoints.Add(new Endpoint
                {
                    EventType = "Payload",
                    EndpointPaths = new[] { "events/readerbarcoderesult" }
                });
            }

            var postData = new SubscriptionRequest
            {
                BaseUrl = $"{_webhookUrl}:{_webhookPort}",
                IgnoreCertificateError = true,
                Endpoints = endpoints.ToArray()               
            };

            var jsonString = JsonSerializer.Serialize(postData, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase, 
                WriteIndented = true
            });

            _logger.LogInformation("SubscriptionEventsService JSON sent:\n" + jsonString);
             System.Diagnostics.Debug.WriteLine("JSON sent:\n" + jsonString);


            var jsonContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

            // Add header
            _httpClient.DefaultRequestHeaders.Add("accept", "*/*");
            _httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);

            try
            {
                var response = await _httpClient.PostAsync(_subscriptionUrl, jsonContent);
                string responseBody = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"Response status: {response.StatusCode}");
                _logger.LogInformation($"Response body: {responseBody}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending subscription request: {ex.Message}");
            }
        }
    }

}//end namespace
