using APIServerMFE;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace APIServerMFE_DeLonghi.Pages
{
    public class MissionsModel: PageModel
    {
        private readonly HttpClient _httpClient;

        public MissionsModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [BindProperty]
        public string Message { get; set; } = "";
        public string TimeToWait { get; set; } = "00.00.02"; // valore di default

        /**************************************************************************
         * 
         * ***********************************************************************/
        public async Task<IActionResult> OnPostMission1Async()
        {
            string missionId = "1bbe1651-8a06-4721-af07-80233444b8d2";
            // usa il valore passato dal form
            await StartMission(missionId, TimeToWait);

            Message = $"Missione 1 avviata! TimeToWait = {TimeToWait}";
            return Page();
        }

        /*************************************************************************
         * 
         * ***********************************************************************/
        //public async Task<IActionResult> OnPostMission2Async()
        //{
        //    string missionId = "1bbe1651-8a06-4721-af07-80233444b8d2";
        //    string timeToWait = "00.00.20";

        //    await StartMission(missionId, timeToWait);

        //    Message = "Missione 2 avviata!";
        //    return Page();
        //}


        /**********************************************************************************************************
         * 
         **********************************************************************************************************/
        private async Task StartMission(string missionId, string timeToWait)
        {
            var payload = new Dictionary<string, object>
            {
                ["serial-order"] = new Dictionary<string, object>
                {
                    ["id"] = missionId,
                    ["priority"] = "Low",
                    ["earliest-start-time"] = "2025-05-14T08:45:32Z",
                    ["robot-id"] = "763f9d9e-5d67-4dd0-a43a-ee7349f88c4a",
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
                                    ["value"] = timeToWait
                                }
                            }
                       }
                    }
                }
            };

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = null, // impedisce di trasformare in camelCase
                WriteIndented = true
            };


            var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions { PropertyNamingPolicy = null });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // qui uso il BaseAddress dal client nominato
            var response = await _httpClient.PostAsync("serial-order", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new ApplicationException($"Errore chiamata API robot: {response.StatusCode} - {error}");
            }
        }
    }


}//namespace
