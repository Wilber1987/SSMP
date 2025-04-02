using APPCORE;
using System.Text.Json.Serialization;
using BackgroundJob.Cron.Jobs;
using APPCORE.Cron.Jobs;
using Microsoft.AspNetCore.ResponseCompression;
using BusinessLogic.Rastreo.Model;
using CAPA_NEGOCIO.MAPEO;
using UI;
using BusinessLogic.Notificaciones_Mensajeria.Gestion_Notificaciones.Operations;
using APPCORE.Security;
using System.Runtime;
using APPCORE.Services;


new BDConnection().IniciarMainConecction();
//Security_Permissions.PrepareDefaultPermissions();
Cat_Dependencias.PrepareDefaultDependencys();//crea las dependencias por defecto TODO REPARAR EN PRODUCTIVO
//SqlADOConexion.IniciarConexion("sa", "admin", "localhost", "PROYECT_MANAGER_BD");

NotificationSenderOperation.SendNotificationReport();
var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
builder.Services.AddRazorPages();


#region CONFIGURACIONES PARA API
builder.Services.AddControllers()
	.AddJsonOptions(JsonOptions => JsonOptions.JsonSerializerOptions.PropertyNamingPolicy = null)// retorna los nombres reales de las propiedades
	.AddJsonOptions(options => options.JsonSerializerOptions.WriteIndented = false)// Desactiva la indentación
	.AddJsonOptions(options =>  options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddResponseCompression(options =>
{
	options.EnableForHttps = true; // Activa la compresión también para HTTPS
	options.Providers.Add<GzipCompressionProvider>(); // Usar Gzip
	options.Providers.Add<BrotliCompressionProvider>(); // Usar Brotli (más eficiente)
});
builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
	options.Level = System.IO.Compression.CompressionLevel.Fastest; // Puedes ajustar la compresión
});
builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
	options.Level = System.IO.Compression.CompressionLevel.Fastest; // Nivel de compresión para Brotli
});
#endregion

builder.Services.AddControllersWithViews();
//builder.Services.AddWebOptimizer();
builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(60);
});

builder.Services.AddCronJob<CreateAutomaticsCaseSchedulerJob>(options =>
{
	// Corre 20 cada minutoS
	options.CronExpression = "*/20 * * * *";
	options.TimeZone = TimeZoneInfo.Local;
});

builder.Services.AddCronJob<ReportSenderSchedulerJob>(options =>
{
	// Corre una vez al día
	options.CronExpression = "0 0 * * *";
	options.TimeZone = TimeZoneInfo.Local;
});

builder.Services.AddCronJob<SendMailNotificationsSchedulerJob>(options =>
{
	// Corre cada minuto
	options.CronExpression = "* * * * *";
	options.TimeZone = TimeZoneInfo.Local;
});


var app = builder.Build();
//app.UseMiddleware<RequestLoggingMiddleware>();
// builder.Services.AddSession(options =>
// {
//     options.Cookie.Name = ".AdventureWorks.Session";
//     options.IdleTimeout = TimeSpan.FromSeconds(10);
//     options.Cookie.IsEssential = true;
// });

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

//app.UseWebOptimizer();
app.UseStaticFiles();
app.UseDefaultFiles();
app.UseResponseCompression(); // Usa la compresión en la aplicación

app.UseRouting();

app.UseAuthorization();

app.UseSession();
app.Use(async (context, next) =>
{
    string? sessionKey = context.Session.GetString("sessionKey");

    await next(); // Ejecutar la solicitud

    if (string.IsNullOrEmpty(sessionKey)) // Si la sesión ya expiró
    {
        SessionServices.ClearSeason(sessionKey); // Limpia la sesión en memoria
    }
});



app.MapRazorPages();
app.MapControllers();

// 🔥 Liberar memoria antes de iniciar la aplicación
int requestCounter = 0;
const int requestThreshold = 1000; // Ajusta el número de solicitudes antes de liberar memoria

app.Use(async (context, next) =>
{
    Interlocked.Increment(ref requestCounter);

    await next(); // Ejecuta la petición

    if (requestCounter >= requestThreshold)
    {
        requestCounter = 0;
        GC.Collect();
        GC.WaitForPendingFinalizers();
    }
});

app.Run();
