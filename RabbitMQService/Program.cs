//RabbitMQ Settings
using Microsoft.Extensions.Logging;
using static RabbitMQService.RabbitMQSettings;

using Microsoft.Extensions.Configuration;
using RabbitMQService.Utils;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using RabbitMQService;
using Topshelf;
using RabbitMQService.Consumers;

internal class Program
{
    private static void Main(string[] args)
    {
        IConfiguration Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json")
                                                          .AddEnvironmentVariables()
                                                          .Build();


        var cadena = Configuration.GetConnectionString("DevCnx");
        SqlTools.CnxServer = cadena;

        // Crear un nuevo ServiceCollection para registrar los servicios
        var serviceProvider = new ServiceCollection();

        serviceProvider.AddSingleton(Configuration);
        serviceProvider.AddSingleton<TSK_Receiver_Message>();

        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");

        var servProvider = serviceProvider.AddLogging()
                                                  .BuildServiceProvider();

        ILoggerFactory loggerFactory = new LoggerFactory();
        MQClient.settings = new RabbitMQSettings();
        MQClient.Logger = loggerFactory.CreateLogger(typeof(MQClient));
        MQClient.LogCnx = Configuration.GetConnectionString("LogCnx");
        Configuration.GetSection("RabbitMQ").Bind(MQClient.settings);

        ConsumerSettings.ConsumerList = new List<string>();
        Configuration.GetSection("ConsumerList").Bind(ConsumerSettings.ConsumerList);

        HostFactory.Run(x => //1
        {
            x.Service<MQService>(s => //2
            {
                s.ConstructUsing(name => new MQService(servProvider)); //3
                s.WhenStarted(tc => tc.StartMQ()); //4
                s.WhenStopped(tc => tc.Stop()); //5
            });
            x.RunAsLocalSystem(); //6

            x.SetDescription(Configuration.GetSection("TopshelfService:Description").Value); //7
            x.SetDisplayName(Configuration.GetSection("TopshelfService:Name").Value); //8
            x.SetServiceName(Configuration.GetSection("TopshelfService:Name").Value); //9
        });
    }
}