using APIServerMFE_DeLonghi;
using APIServerMFE_DeLonghi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using APIServerMFE_DeLonghi.Models;

namespace APIServerMFE_DeLonghi.Pages
{
    public class SerialOrderTransferWithBarcodeToWarehouseEwmModel: PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<SerialOrderWaitModel> _logger;
        private readonly AppSettings _settings;

        public SerialOrderTransferWithBarcodeToWarehouseEwmModel(IHttpClientFactory httpClientFactory, ILogger<SerialOrderWaitModel> logger, IOptions<AppSettings> options)
        {
            _httpClient = httpClientFactory.CreateClient("MFEUrl");
            _logger   = logger;
            _settings = options.Value;
        }

        [BindProperty]
        public string StartPosition { get; set; } // valore di default

        [BindProperty]
        public string StartPositionEntry { get; set; } // valore di default

        [BindProperty]
        public string EndPosition { get; set; } // valore di default

        [BindProperty]
        public string Priority { get; set; }

        [BindProperty]
        public List<string> CodesHU { get; set; } = new List<string>();

      
        public string Message { get; set; } = "";

 
        /**************************************************************************
         * 
         * ***********************************************************************/
        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                Message = "Errore nel binding del form!";
                _logger.LogWarning("ModelState non valido su SerialOrderTransferWithBarcodeToWarehouseEwmModel");
                return Page();
            }
       
            try
            {
                _logger.LogInformation("Avvio missione Transfer with Barcode to Warehouse.Priority={Priority},StartPosition={StartPosition},StartPositionEntry={StartPositionEntry},EndPosition={EndPosition} ,CodiciHU={CodesHU}", Priority,StartPosition,StartPositionEntry,EndPosition ,CodesHU);
                
                await StartMissionTransferWithBarcodeToWarehouseEwm(Priority,StartPosition,StartPositionEntry, EndPosition, CodesHU);
                Message = $"Missione avviata Transfer With Barcode To Warehouse Ewm";

                
                _logger.LogInformation("Missione Transfer With Barcode To Warehouse Ewm avviata correttamente. Priority={Priority},StartPosition={StartPosition},StartPositionEntry={StartPositionEntry},EndPosition={EndPosition},CodiciHU={CodesHU}", Priority, StartPosition, StartPositionEntry, EndPosition, CodesHU);
            }
            catch (Exception ex)
            {
                Message = $"Errore avvio missione: {ex.Message}";
               
                _logger.LogError(ex, "Errore durante l'avvio della missione Transfer With Barcode To Warehouse Ewm. Priority={Priority},StartPosition={StartPosition},StartPositionEntry={StartPositionEntry},EndPosition={EndPosition},CodiciHU={CodesHU}", Priority, StartPosition, StartPositionEntry, EndPosition, CodesHU);
            }
            return Page();
        }


        /**********************************************************************************************************
         * EndPosition "default-value": "aa917529-241c-4a6b-80fc-90640d1cba43"
         * StartPositionEntry "default-value": "339e7751-adb1-4f4f-9b4e-7473f281ab36",
         * StartPosition "default-value": "339e7751-adb1-4f4f-9b4e-7473f281ab36",
         **********************************************************************************************************/
        private async Task StartMissionTransferWithBarcodeToWarehouseEwm(string priority, string startPosition,
                        string startPositionEntry,string endPosition,List<string> CodesHU)

        {
            string missionId = _settings.Mission_Transfer_With_Barcode_To_Warehouse_Ewm_id;

            var arguments = new List<Dictionary<string, object>>();

            arguments.Add(new Dictionary<string, object>
            {
                ["name"] = "StartPosition",
                ["value"] = startPosition
            });

            //add StartPositionEntry
            arguments.Add(new Dictionary<string, object>
            {
                ["name"] = "StartPositionEntry",
                ["value"] = startPositionEntry
                
            });

            //add EndPosition
            arguments.Add(new Dictionary<string, object>
            {
                ["name"] = "EndPosition",
                ["value"] = endPosition
            });

            //add codici HU
            for (int i = 0; i < CodesHU.Count; i++)
            {
                string asciiValue;

                if (!string.IsNullOrEmpty(CodesHU[i]) && CodesHU[i].All(char.IsLetterOrDigit))
                {
                    // Se tutti i caratteri sono validi (lettera o numero), converto in ASCII
                    asciiValue = string.Concat(CodesHU[i].Select(c => ((int)c).ToString()));
                }
                else
                {
                    // Se contiene caratteri speciali o è vuoto → scrivo "0"
                    asciiValue = "0";
                }

                arguments.Add(new Dictionary<string, object>
                {
                    ["name"] = _settings.GetPLCRegister(i + 1),
                    ["value"] = asciiValue
                });
            }


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
                            ["arguments"] = arguments
                              
                       }
                    }
                }
            };


            _logger.LogInformation("Preparazione payload missione Transfer With Barcode Warehouse Ewm. MissionId={MissionId}, Priority={Priority},StartPosition={StartPosition},StartPositionEntry={StartPositionEntry},EndPosition={EndPosition} CodesHU={CodesHU}",
                                           missionId, priority,startPosition, startPositionEntry,endPosition,CodesHU);

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
