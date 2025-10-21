using APIServerMFE;
using APIServerMFE.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace APIServerMFE_DeLonghi.Pages
{
    public class SerialOrderWaitModel: PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<SerialOrderWaitModel> _logger;
        private readonly AppSettings _settings;


        public SerialOrderWaitModel(IHttpClientFactory httpClientFactory, ILogger<SerialOrderWaitModel> logger, IOptions<AppSettings> options)
        {
            _httpClient = httpClientFactory.CreateClient("MFEUrl");
            _logger   = logger;
            _settings = options.Value;
        }

        [BindProperty]
        public int TimeToWait { get; set; } // valore di default

        [BindProperty]
        public string Priority { get; set; }

        //[BindProperty]
        //public string MissionId { get; set; }

        public string Message { get; set; } = "";

 
        /**************************************************************************
         * 
         * ***********************************************************************/
        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                Message = "Errore nel binding del form!";
                _logger.LogWarning("ModelState non valido su SerialOrderWaitModel");
                return Page();
            }
       
            try
            {
                _logger.LogInformation("Avvio missione Wait.Priority={Priority}, TimeToWait={TimeToWait}", Priority, TimeToWait);

                //await StartMissionWait(MissionId, Priority, TimeToWait);
                await StartMissionWait(Priority, TimeToWait);
                Message = $"Missione Wait avviata! TimeToWait = {TimeToWait}";

                _logger.LogInformation("Missione Wait avviata correttamente. TimeToWait={TimeToWait}", TimeToWait);
            }
            catch (Exception ex)
            {
                Message = $"Errore avvio missione: {ex.Message}";
                _logger.LogError(ex, "Errore durante l'avvio della missione Wait. TimeToWait={TimeToWait}", TimeToWait);
            }
            return Page();
        }


        /**********************************************************************************************************
         * 
         **********************************************************************************************************/
        //private async Task StartMissionWait(string missionId, string priority, int timeToWait)
        private async Task StartMissionWait(string priority, int timeToWait)
        {
            string formatted = $"00:00:{timeToWait:D2}";
            string missionId = _settings.Mission_WaitTest_id;

            var payload = new Dictionary<string, object>
            {
                ["serial-order"] = new Dictionary<string, object>
                {
                    ["id"] = missionId,
                    ["priority"] = priority,
                    ["earliest-start-time"] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss") + "Z",
                    ["phases"] = new[]
                    {
                       new Dictionary<string, object>
                       {
                            ["mission-id"] = missionId,
                            ["arguments"] = new[]
                            {
                                new Dictionary<string, object>
                                {
                                    ["name"] = "TimeToWait",
                                    ["value"] = formatted
                                }
                            }
                       }
                    }
                }
            };

            _logger.LogInformation("Preparazione payload missione Wait. MissionId={MissionId}, Priority={Priority}, TimeToWait={TimeToWait}",
                               missionId, priority, timeToWait);


            // Serializza in JSON
            var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = null, 
                WriteIndented = true 
            });
            
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Aggiungi l'header x-api-key
            _httpClient.DefaultRequestHeaders.Remove("x-api-key");
            _httpClient.DefaultRequestHeaders.Add("x-api-key", _settings.XApiKey);

            _logger.LogInformation("Invio richiesta POST a serial-order con payload: {Payload}", json);
            _logger.LogInformation("Sto inviando x-api-key: {ApiKey}", _settings.XApiKey);

            // qui uso il BaseAddress dal client nominato
            var response = await _httpClient.PostAsync("/api/v1/serial-order", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError("Errore API robot. StatusCode={StatusCode}, Content={Content}", response.StatusCode, error);
                throw new ApplicationException($"Errore chiamata API robot: {response.StatusCode} - {error}");
            }
            _logger.LogInformation("Chiamata API robot completata con successo. MissionId={MissionId}", missionId);
        }
    }
}//namespace
