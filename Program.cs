using APIServerMFE_DeLonghi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using APIServerMFE_DeLonghi.Events;
using System.Reflection;
using APIServerMFE_DeLonghi.Controllers;
using System.Xml.Linq;
using APIServerMFE_DeLonghi.Models;


/**************************************************************************************
 * Da PLC Register 50 a PLC Register 69 (20 PLCRegister) per codici HU
 * PLCRegister 70 conferma lettua codice HU 1=ok 99=error 0=reset 
 * PLCRegiater 71 avvio trigger lettura
 * "map-id": "cddf5c8f-1224-4dcd-b916-dc0877b28fc3",,
 * *************************************************************************************/


/***************************************************************************************
 * ASP.NET Core Web Application Entry Point
 * 
 * ************************************************************************************/

var builder = WebApplication.CreateBuilder(args);

/**************************************************************************************
* Load configuration from appsettings.json 
*
***************************************************************************************/
builder.Configuration
    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();  // consente override via environment variables

// Bind configuration to strongly-typed settings
builder.Services.Configure<AppSettings>(builder.Configuration);

/*****************************************************************************************
 * Configure Logging 
 * 
 * **************************************************************************************/
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddFile("Logs/app-log-{Date}.txt", minimumLevel: LogLevel.Information);

/*******************************************************************************************
 *  Register Services 
 * 
 * *****************************************************************************************/
builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Domain services
builder.Services.AddSingleton<MissionMirManager>();
builder.Services.AddSingleton<SerialOrderStatusService>();
builder.Services.AddSingleton<AutochargingOrderStatusService>();
builder.Services.AddSingleton<RobotEnumerators>();


// Named HttpClient for MFE
builder.Services.AddHttpClient("MFEUrl", (sp, client) =>
{
    var settings = sp.GetRequiredService<IConfiguration>();
    var baseUrl = settings["MFEUrl"] ?? throw new Exception("Missing configuration key 'MFEUrl'");
    client.BaseAddress = new Uri(baseUrl.StartsWith("http") ? baseUrl : $"http://{baseUrl}");
});

// Default HttpClient
builder.Services.AddHttpClient();


/***************************************************************************************
 * Build Application
 ***************************************************************************************/
var app = builder.Build();


/***************************************************************************************
 * Configure Webhook Server URL
 ***************************************************************************************/
var config = app.Services.GetRequiredService<IConfiguration>();
//string webhookUrl = config["WebHookUrl"] ?? throw new Exception("Missing 'WebHookUrl'");
//string webhookPort = config["WebHookPort"] ?? "5000"; // default if not provided
//app.Urls.Add($"{webhookUrl}:{webhookPort}");

string webhookUrl  = config["WebHookUrl"] ?? "http://localhost";
string webhookPort = config["WebHookPort"];

try
{
    // Aggiungi URL solo se non stai girando sotto IIS (cioè in self-host / debug)
    var isIIS = Environment.GetEnvironmentVariable("ASPNETCORE_HOSTINGSTARTUPASSEMBLIES")?.Contains("Microsoft.AspNetCore.Server.IIS") == true;

    if (!isIIS && !app.Environment.IsDevelopment())
    {
        if (!string.IsNullOrWhiteSpace(webhookPort))
            app.Urls.Add($"{webhookUrl}:{webhookPort}");
        else
            app.Urls.Add(webhookUrl);
    }
    //if (!string.IsNullOrWhiteSpace(webhookPort))d
    //    app.Urls.Add($"{webhookUrl}:{webhookPort}");
    //else
    //    app.Urls.Add(webhookUrl);
}
catch (Exception ex)
{
    Console.WriteLine($"Warning: unable to set app URL ({ex.Message})");
}


/***************************************************************************************
 * Event Subscription / Unsubscription before starting the server
 ***************************************************************************************/
using (var scope = app.Services.CreateScope())
{
    var httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient();

    string mfeUrl = config["MFEUrl"] ?? throw new Exception("Missing 'MFEUrl'");
    string apiKey = config["XApiKey"] ?? "";

    //var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<SubscriptionEventsService>>();

    // Unsubscribe from all events if configured
    if (config.GetValue<bool>("Subscription_DeleteAllEvents"))
    {
        var unsubscriptionService = new UnsubscriptionAllEventsService(httpClient, mfeUrl, webhookUrl, webhookPort, apiKey);
        await unsubscriptionService.SubscribeAsync();
        logger.LogInformation("Unsubscribed from all events");
    }

    // Subscribe to specific events
    var subscriptionService = new SubscriptionEventsService(
        logger,
        httpClient, 
        mfeUrl, 
        webhookUrl, 
        webhookPort, 
        apiKey,
        config.GetValue<bool>("Subscription_AlertEvent"),
        config.GetValue<bool>("Subscription_RobotRuntimeEvent"),
        config.GetValue<bool>("Subscription_SerialOrderStatusEvent"),
        config.GetValue<bool>("Subscription_ErrorEvent"),
        config.GetValue<bool>("Subscription_RobotIdentityEvent"),
        config.GetValue<bool>("Subscription_RobotStateEvent")
    );

    await subscriptionService.SubscribeAsync();
    logger.LogInformation("Subscribed to events");
}


/***************************************************************************************
 * Configure HTTP Request Pipeline
 ***************************************************************************************/
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();


// Redirect root -> /serialorders
app.MapGet("/", context =>
{
    context.Response.Redirect("/Index");
    return Task.CompletedTask;
});

/***************************************************************************************
 * Run the application
 ***************************************************************************************/
app.Run();


/***********************************************************************************
 * DTOs
 * *********************************************************************************/
public class SubscriptionRequest
{
    [JsonPropertyName("base-url")]
    public string BaseUrl { get; set; }

    [JsonPropertyName("ignore-certificate-errors")]
    public bool IgnoreCertificateError { get; set; }

    [JsonPropertyName("endpoints")]
    public Endpoint[] Endpoints { get; set; }
}

public class Endpoint
{
    [JsonPropertyName("event-type")]
    public string EventType { get; set; }

    [JsonPropertyName("endpoint-paths")]
    public string[] EndpointPaths { get; set; }
}

