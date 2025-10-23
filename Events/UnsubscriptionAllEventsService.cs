namespace APIServerMFE_DeLonghi.Events
{
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;

    /*******************************************************************************************************************************
     * Class implements POST Unaubsciption of All Events on MFE server 
     * 
     * ****************************************************************************************************************************/
    public class UnsubscriptionAllEventsService
    {
        private readonly HttpClient _httpClient;
        private readonly string _subscriptionUrl;
        private readonly string _webhookUrl;
        private readonly string _webhookPort;
        private readonly string _apiKey;

        /************************************************************************************
         * 
         * **********************************************************************************/
        public UnsubscriptionAllEventsService(HttpClient httpClient, string mfeUrl, string webhookUrl, string webhookPort, string apiKey)
        {
            _httpClient = httpClient;
            _subscriptionUrl = $"{mfeUrl}:{webhookPort}/api/v1/subscription";
            _webhookUrl = webhookUrl;
            _webhookPort = webhookPort;
            _apiKey = apiKey;
        }

        /************************************************************************************
         * 
         * **********************************************************************************/
        public async Task SubscribeAsync()
        {
            var endpoints = new List<Endpoint>();
        
            var postData = new SubscriptionRequest
            {
                BaseUrl = $"{_webhookUrl}:{_webhookPort}",
                IgnoreCertificateError = true,
                Endpoints = endpoints.ToArray()
               
            };

            //clear header
            _httpClient.DefaultRequestHeaders.Clear();

            // Add header
            _httpClient.DefaultRequestHeaders.Add("accept", "*/*");
            _httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);

            try
            {
                var response = await _httpClient.DeleteAsync(_subscriptionUrl);
                string responseBody = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Response status: {response.StatusCode}");
                Console.WriteLine($"Response body: {responseBody}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending DELETE subscription request: {ex.Message}");
            }
        }
    }

}//end namespace
