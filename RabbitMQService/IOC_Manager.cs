using Microsoft.Extensions.DependencyInjection;
using RabbitMQService.Consumers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQService
{
    public class IOC_Manager
    {
        public static void AddServices(IServiceCollection services)
        {
            services.AddScoped<TSK_Receiver_Message>();
            services.AddTransient<TSK_Receiver_Message>();

        }
    }
}
