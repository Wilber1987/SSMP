using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CAPA_NEGOCIO.MAPEO;

namespace BusinessLogic.IA.RequestEvaluator
{
    public class TrakingUnindictableRequest
    {
        

        public static (bool, string?) ProcessRequest()
        {
            // Seleccionar una respuesta aleatoria
            Random random = new Random();
            int index = random.Next(responses.Length);

            string finalMessage = responses[index] + ProntManager.OptionalActionAsistenciaOrMenu();

            return (true, finalMessage);

        }

        
        static string[] responses =
        {
            "Lo lamento, parece que no estoy entendiendo tu consulta. ¿Podrías ingresar un número de seguimiento para ayudarte?",
            "Para poder localizar tu paquete, necesito que ingreses tu número de seguimiento.",
            "No logro entender tu solicitud. ¿Podrías proporcionarme un número de seguimiento?",
            "Parece que me falta información. Por favor, ingresa tu número de seguimiento.",
            "Estoy aquí para ayudarte. Para continuar, por favor ingresa tu número de seguimiento.",
            "Disculpa, pero no puedo procesar tu solicitud sin un número de seguimiento. ¿Podrías proporcionarlo?",
            "Si necesitas ayuda con tu paquete, por favor ingresa tu número de seguimiento.",
            "Para brindarte información sobre tu envío, necesito que ingreses tu número de seguimiento.",
            "No entendí tu mensaje. ¿Puedes indicarme tu número de seguimiento para ayudarte mejor?",
            "No estoy seguro de cómo responder. ¿Podrías proporcionar tu número de seguimiento?",
            "Parece que hay un error. ¿Podrías escribir tu número de seguimiento?",
            "Para brindarte soporte sobre tu paquete, ingresa tu número de seguimiento.",
            "No comprendo bien tu solicitud. Si es sobre un paquete, por favor ingresa el número de seguimiento.",
            "Me gustaría ayudarte, pero necesito que ingreses tu número de seguimiento.",
            "Sin un número de seguimiento, no podré brindarte información precisa. ¿Podrías ingresarlo?",
            "Si tu consulta es sobre un paquete, por favor ingresa el número de seguimiento.",
            "Lo siento, pero no puedo identificar tu solicitud. Comparte tu número de seguimiento para asistirte.",
            "No logro procesar tu mensaje. ¿Podrías indicarme el número de seguimiento de tu paquete?",
            "Para encontrar información sobre tu pedido, necesito el número de seguimiento.",
            "¿Podrías indicarme el número de seguimiento de tu paquete? Así podré ayudarte mejor.",
            "Parece que hay un malentendido. ¿Tienes un número de seguimiento que puedas compartir?",
            "Si deseas conocer el estado de tu paquete, por favor proporciona el número de seguimiento.",
            "Para verificar la información de tu paquete, ingresa el número de seguimiento.",
            "Lo lamento, pero necesito un número de seguimiento para continuar con tu solicitud.",
            "Sin un número de seguimiento, no podré ubicar tu paquete. ¿Podrías proporcionarlo?",
            "Para encontrar tu paquete en nuestro sistema, necesito el número de seguimiento.",
            "No logro entender lo que necesitas. ¿Es sobre un paquete? Si es así, ingresa el número de seguimiento.",
            "Lamento la confusión. Para asistirte mejor, por favor ingresa tu número de seguimiento.",
            "Si necesitas ayuda con el rastreo de tu paquete, ingresa el número de seguimiento.",
            "Parece que necesito más detalles. Por favor, proporciona el número de seguimiento.",
            "No comprendo tu solicitud. Si es sobre un paquete, por favor ingresa su número de seguimiento.",
            "¿Podrías proporcionarme el número de seguimiento? Así podré ayudarte con la ubicación de tu paquete.",
            "No puedo continuar sin un número de seguimiento. ¿Podrías ingresarlo?",
            "Para darte información actualizada sobre tu paquete, necesito tu número de seguimiento."
        };

    }
}