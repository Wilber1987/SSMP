using Microsoft.Extensions.DependencyInjection;
using RabbitMQService.Consumers;
using RabbitMQService.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQService
{
    public class MQService
    {
        private IServiceProvider _servProvider;

        public MQService(IServiceProvider servProvider)
        {
            _servProvider = servProvider;
        }

        public void Stop()
        {

        }

        public void StartMQ()
        { 
            var receiver = (TSK_Receiver_Message)_servProvider.GetRequiredService(typeof(TSK_Receiver_Message));
            //Iniciar el listener para cada consumidor
            if (ConsumerSettings.ConsumerList.Contains("TSK_Receiver_Message"))
                receiver.StartListener();
        }

    }
}