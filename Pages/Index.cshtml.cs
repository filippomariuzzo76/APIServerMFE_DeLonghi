using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace APIServerMFE_DeLonghi.Pages
{
    public class IndexModel : PageModel
    {
        public string Message { get; set; } = "";

        [BindProperty]
        public string NomeInput { get; set; }

        public void OnPost()
        {
            //NomeInput contiene il valore inviato dal form
           if (!string.IsNullOrEmpty(NomeInput))
            {
                // esempio: trasformazione del valore
                NomeInput = NomeInput.ToUpper();
                
            }
        }
    }

}
