using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

public class TestHandlerModel : PageModel
{
    public string Message { get; set; }

    public IActionResult OnPostFoo()
    {
        Message = "Handler OnPostFoo chiamato!";
        return Page();
    }
}

