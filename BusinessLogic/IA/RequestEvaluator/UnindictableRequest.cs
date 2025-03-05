using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CAPA_NEGOCIO.MAPEO;

namespace BusinessLogic.IA.RequestEvaluator
{
    public class UnindictableRequest
    {


        public static (bool, string?) ProcessRequest()
        {
            // Seleccionar una respuesta aleatoria
            Random random = new Random();
            int index = random.Next(responses.Length);

            string finalMessage = responses[index] + " \n \npuedes escribir \"5\" para solicitar asistencia o \"Menu\" para otras consultas.";

            return (true, finalMessage);

        }
        static string[] responses =
        {
            "Lo lamento, como asistente de correos no puedo procesar su consulta.",
            "Disculpa, pero como asistente virtual de correos, no puedo atender esta solicitud.",
            "No puedo procesar tu consulta. ¿Hay algo más en lo que pueda ayudarte?",
            "Lo siento, pero no tengo la capacidad de responder esta solicitud.",
            "Como asistente de correos, mi función es brindar información sobre envíos. No puedo procesar esta consulta.",
            "Lamento informarte que no puedo gestionar esta solicitud.",
            "No puedo procesar tu solicitud en este momento. ¿Puedo ayudarte con otra consulta?",
            "Disculpa, pero mi función es brindar soporte sobre envíos y no puedo atender esta solicitud.",
            "No tengo la capacidad de procesar esta solicitud. ¿Puedo asistirte en algo más?",
            "Lo lamento, pero no puedo procesar esta consulta en este momento.",
            "No tengo información sobre esta consulta. ¿Puedo ayudarte con algo más?",
            "Disculpa, pero mi función es brindar información sobre paquetes y rastreos. No puedo procesar esta solicitud.",
            "Lo siento, pero no tengo acceso a esa información.",
            "No puedo completar esta solicitud. ¿Puedo asistirte en algo relacionado con envíos?",
            "Mi función es ayudarte con información sobre envíos. No puedo procesar esta consulta.",
            "Lamento informarte que no tengo la capacidad de responder esta solicitud.",
            "Como asistente de correos, no puedo procesar este tipo de consultas.",
            "Disculpa, pero no tengo la capacidad de responder a esta solicitud.",
            "No puedo gestionar esta consulta. ¿Puedo ayudarte con algo más?",
            "Lo siento, pero no puedo responder a esta solicitud en este momento.",
            "Como asistente virtual de correos, solo puedo ayudarte con información sobre envíos y rastreo.",
            "No tengo la información necesaria para procesar esta solicitud. ¿Puedo ayudarte con otra consulta?",
            "Lamento la inconveniencia, pero no puedo atender esta solicitud.",
            "No puedo procesar tu consulta en este momento. ¿Puedo ayudarte con algo más?",
            "Como asistente de correos, mi función es ayudar con envíos. No puedo atender esta solicitud.",
            "No tengo la capacidad de responder esta solicitud. ¿Hay algo más en lo que pueda ayudarte?",
            "Lamento no poder procesar esta consulta. ¿Puedo asistirte con algo más?",
            "Mi sistema no puede procesar esta solicitud. ¿Necesitas ayuda con un envío?",
            "Disculpa, pero no tengo la información necesaria para responder esta solicitud.",
            "Lo lamento, pero mi función es brindar soporte sobre envíos. No puedo procesar esta solicitud.",
            "No puedo procesar tu consulta en este momento. ¿Puedo ayudarte con información sobre envíos?",
            "Como asistente virtual de correos, no tengo acceso a esa información.",
            "Disculpa, pero no puedo procesar esta solicitud en este momento."
        };


    }
}