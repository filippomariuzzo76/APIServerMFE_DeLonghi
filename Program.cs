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

/***************************************************************************************
 * ASP.NET Core Web
 * 
 * ************************************************************************************/

var builder = WebApplication.CreateBuilder(args);

/**************************************************************************************
* Read Configuration file appsettings.json and create appSettings  service to share 
*
***************************************************************************************/
//Log.Information("Start AppSettings Service");
var configurationBuilder = new ConfigurationBuilder()
    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

IConfiguration appSettings = configurationBuilder.Build();
builder.Services.AddSingleton(appSettings);

/*****************************************************************************************
 * Configuration of global logging 
 * 
 * **************************************************************************************/
builder.Logging.ClearProviders();
builder.Logging.AddConsole(); // Add provider console
builder.Logging.AddFile("Logs/app-log-{Date}.txt", minimumLevel: LogLevel.Information); // Add provider file

var logger = builder.Logging.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();


/*******************************************************************************************
 * URL  
 * 
 * *****************************************************************************************/
string? mfe_url = appSettings["MFEUrl"];//url mir fleet
string? webhook_url = appSettings["WebHookUrl"];
string? webhook_port = appSettings["WebHookPort"];
string? webhook_x_api_key = appSettings["x-api-key"];//headers


if (string.IsNullOrEmpty(mfe_url))
{
    throw new Exception("Configuration Key 'MFEUrl' not present in appsettings.json.");
}
else if (string.IsNullOrEmpty(webhook_url))
{
    throw new Exception("Configuration Key 'WebhookUrl' not present in  appsettings.json.");
}

/***************************************************************************************
 * Create receive event URL 
 * 
 * ************************************************************************************/

builder.WebHost.UseUrls($"{webhook_url}:{webhook_port}");


/***************************************************************************************
 * Define httpClient to send POST request
 * 
 * *************************************************************************************/
var httpClient = new HttpClient();

/***************************************************************************************
 * List of Mir Mission
 * ************************************************************************************/
MissionMirManager missionMirManager = new MissionMirManager();
builder.Services.AddSingleton(missionMirManager);


/**************************************************************************************
 * Send POST request to endpoint before start webhook server
 * UnSubscription ALL EVENTS
 **************************************************************************************/
// Create the service instance
var unsubscriptionAllServiceEvent = new UnsubscriptionAllEventsService(httpClient, mfe_url, webhook_url, webhook_port, webhook_x_api_key);

//Start subscription
await unsubscriptionAllServiceEvent.SubscribeAsync();

//log action
logger.LogInformation("Unsubscription ALL Events");


/**************************************************************************************
 * Send POST request to endpoint before start webhook server
 * Subscription EVENTS
 **************************************************************************************/
bool isAlertEvent = appSettings.GetValue<bool>("Subscription_AlertEvent");
bool isRobotRuntimeEvent = appSettings.GetValue<bool>("Subscription_RobotRuntimeEvent");
bool isSerialOrderStatusEvent = appSettings.GetValue<bool>("Subscription_SerialOrderStatusEvent");
bool isErrorEvent = appSettings.GetValue<bool>("Subscription_ErrorEvent");
bool isRobotIdentityEvent = appSettings.GetValue<bool>("Subscription_RobotIdentityEvent");
bool isRobotStateEvent = appSettings.GetValue<bool>("Subscription_RobotStateEvent");

 // Create the service instance
 var subscriptionServiceEvent = new SubscriptionEventsService(httpClient, mfe_url, webhook_url, webhook_port, webhook_x_api_key, isAlertEvent, isRobotRuntimeEvent,isSerialOrderStatusEvent, isErrorEvent, isRobotIdentityEvent, isRobotStateEvent);

 //Start subscription
 await subscriptionServiceEvent.SubscribeAsync();

//log action
logger.LogInformation("Subscription of Events");


/**************************************************************************************************************************************
 Add services to the container.
**************************************************************************************************************************************/
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<SerialOrderStatusService>();
builder.Services.AddSingleton<AutochargingOrderStatusService>();


// Costruisci l'applicazione
var app = builder.Build();

// Ottieni un logger
//var logger = app.Services.GetRequiredService<ILogger<Program>>();

/*************************************************************************************************************************************
*  Configure the HTTP request pipeline.
*
*************************************************************************************************************************************/

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

/**************************************************************************************************************************************
 * 
 * ***********************************************************************************************************************************/
app.Use(async (context, next) =>
{
    Console.WriteLine("Event intercept on" + context.Request.Path);
    logger.LogInformation("Intercept event on: {Path}", context.Request.Path);

    context.Request.EnableBuffering();
    var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
    logger.LogInformation("Body of request: {Body}", body);
    context.Request.Body.Position = 0;

   // Console.WriteLine($"Richiesta ricevuta: {body}");
    await next();
});

/**************************************************************************************************************************************
 * 
 * ***********************************************************************************************************************************/
//app.MapPost("/event", (Evento evento) =>
//{
//    //Console.WriteLine($"Tipo: {evento.Tipo}, Messaggio: {evento.Messaggio}");
//    System.Diagnostics.Debug.WriteLine($"Tipo: {evento.Tipo}, Messaggio: {evento.Messaggio}");

//    return Results.Ok(new { message = "Evento ricevuto con successo!" + $"Tipo: {evento.Tipo}, Messaggio: {evento.Messaggio}" });
//});


//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


/***********************************************************************************
 * 
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