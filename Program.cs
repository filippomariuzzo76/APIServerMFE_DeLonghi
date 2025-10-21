using APIServerMFE;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using APIServerMFE.Events;
using System.Reflection;
using APIServerMFE.Controllers;
using System.Xml.Linq;

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
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

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

string webhookUrl = config["WebHookUrl"] ?? "http://localhost";
string webhookPort = config["WebHookPort"];

try
{
    // Aggiungi URL solo se non stai girando sotto IIS (cioè in self-host / debug)
    var isIIS = Environment.GetEnvironmentVariable("ASPNETCORE_HOSTINGSTARTUPASSEMBLIES")?.Contains("Microsoft.AspNetCore.Server.IIS") == true;

    if (!isIIS)
    {
        if (!string.IsNullOrWhiteSpace(webhookPort))
            app.Urls.Add($"{webhookUrl}:{webhookPort}");
        else
            app.Urls.Add(webhookUrl);
    }
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
    string apiKey = config["x-api-key"] ?? "";

    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    // Unsubscribe from all events if configured
    if (config.GetValue<bool>("Subscription_DeleteAllEvents"))
    {
        var unsubscriptionService = new UnsubscriptionAllEventsService(httpClient, mfeUrl, webhookUrl, webhookPort, apiKey);
        await unsubscriptionService.SubscribeAsync();
        logger.LogInformation("Unsubscribed from all events");
    }

    // Subscribe to specific events
    var subscriptionService = new SubscriptionEventsService(
        httpClient, mfeUrl, webhookUrl, webhookPort, apiKey,
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
app.UseAuthorization();
app.MapControllers();
app.MapRazorPages();

// Redirect root -> /serialorders
app.MapGet("/", context =>
{
    context.Response.Redirect("/index");
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

/***************************************************************************************
 * Strongly-typed App Settings Model
 ***************************************************************************************/
public class AppSettings
{
    public string MFEUrl { get; set; }
    public string WebHookUrl { get; set; }
    public string WebHookPort { get; set; }

    public string XApiKey { get; set; }

    public bool Subscription_DeleteAllEvents { get; set; }
    public bool Subscription_AlertEvent { get; set; }
    public bool Subscription_RobotRuntimeEvent { get; set; }
    public bool Subscription_SerialOrderStatusEvent { get; set; }
    public bool Subscription_ErrorEvent { get; set; }
    public bool Subscription_RobotIdentityEvent { get; set; }
    public bool Subscription_RobotStateEvent { get; set; }
    public string Mission_WaitTest_id { get; set; }
    public string Mission_WaitParameterTest_id { get; set; }
    public string ExportDirectoryName { get; set; }
    public string ExportFileMissionName { get; set; }
    public string ExportFileAutochargingName { get; set; }
    public string ExportFileDistanceName { get; set; }
}