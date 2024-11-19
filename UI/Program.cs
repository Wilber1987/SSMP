using CAPA_DATOS;
using System.Text.Json.Serialization;
using BackgroundJob.Cron.Jobs;
using CAPA_DATOS.Cron.Jobs;
using Microsoft.AspNetCore.ResponseCompression;

SqlADOConexion.IniciarConexion("sa", "zaxscd", "localhost", "PROYECT_MANAGER_BD");//SIASMOP USAV


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

// builder.Services.AddCronJob<SendMailNotificationsSchedulerJob>(options =>
// {
//     // Corre cada minuto
//     options.CronExpression = "* * * * *";
//     options.TimeZone = TimeZoneInfo.Local;
// });


var app = builder.Build();
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

app.MapRazorPages();
app.MapControllers();

app.Run();
