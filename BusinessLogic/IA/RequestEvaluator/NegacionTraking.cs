using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CAPA_NEGOCIO.MAPEO;

namespace BusinessLogic.IA.RequestEvaluator
{
    public class NegacionTraking
    {

        public static bool UsuarioNoTieneTracking(string mensaje)
        {
            if (string.IsNullOrWhiteSpace(mensaje)) return false;

            mensaje = mensaje.ToLower().Trim();  // 🔹 Elimina espacios al inicio y fin

            Console.WriteLine($"Mensaje recibido: '{mensaje}'"); // Depuración

            bool resultado = Negaciones.Any(frase =>
            {
                bool contiene = mensaje.Contains(frase);
                if (contiene)
                {
                    Console.WriteLine($"Coincidencia encontrada: '{frase}'");
                }
                return contiene;
            });

            Console.WriteLine($"Resultado final: {resultado}");  // Ver resultado
            return resultado;
        }


        public static (bool, string?) ProcesarNegationTrackingResponse(string mensaje, string services, bool isInvalidTrackingMessage)
        {
            if (UsuarioNoTieneTracking(mensaje) && services == DefaultServices_DptConsultasSeguimientos.RASTREO_Y_SEGUIMIENTOS.ToString() && !isInvalidTrackingMessage)
            {
                // Respuestas variadas con el formato de tracking incluido
                string[] responses =
                {
                    "No tienes un número de tracking. Este fue proporcionado por la agencia o proveedor al momento de la compra. Busca un código con 2 letras, 9 números y 2 letras finales.",
                    "No encontré un número de seguimiento en tu mensaje. El tracking debe tener 2 letras al inicio, 9 números en el medio y 2 letras al final. Verifica tu correo o recibo de compra.",
                    "Entiendo que no cuentas con un número de tracking. Este suele ser proporcionado en la confirmación del pedido. Busca un código con el formato: 2 letras, 9 números, 2 letras.",
                    "Si no tienes un número de tracking, te recomiendo revisar la documentación de tu compra. Debe ser un código similar a XX123456789XX.",
                    "Parece que no tienes un número de rastreo. Generalmente, este es entregado en el comprobante de compra y sigue el formato: 2 letras, 9 números, 2 letras.",
                    "Si no encuentras tu número de seguimiento, revisa tu factura o correo electrónico. Debe tener la estructura XX123456789XX.",
                    "No tienes un código de rastreo en tu mensaje. Si realizaste una compra, tu número de tracking debe verse como XX123456789XX. Revisa tu comprobante o consulta con tu proveedor.",
                    "No parece que tengas un número de seguimiento. Si tu paquete fue enviado, el tracking debe verse como 2 letras al inicio, seguido de 9 números y 2 letras al final.",
                    "Para rastrear tu paquete, necesitas un número de tracking con el siguiente formato: 2 letras al inicio, 9 números y 2 letras al final. Este se encuentra en el recibo de compra.",
                    "Si no tienes un número de rastreo, revisa los documentos de compra o contacta a la agencia de envío. El código debe cumplir este formato: 2 letras + 9 números + 2 letras."
                };

                // Seleccionar una respuesta aleatoria
                Random random = new Random();
                int index = random.Next(responses.Length);

                string finalMessage = responses[index] + "\n\nEscribe \"5\" para solicitar asistencia o \"Menu\" para otras consultas.";

                return (true, finalMessage);
            }

            return (false, null);
        }
        private static readonly string[] Negaciones =
        {
            "no tengo", "no cuento con", "no poseo", "no dispongo", "no sé", "no lo sé",
            "no lo tengo", "no recuerdo", "perdí", "extravié", "se me olvidó", "no me dieron",
            "no recibí", "no aparece", "no encuentro", "nunca me llegó", "no tengo número de tracking",
            "sin número de tracking", "sin código de rastreo", "no me proporcionaron tracking",
            "no me enviaron tracking", "no tengo un número de seguimiento",
            "no me dieron número de seguimiento", "no recibí número de rastreo",
            "no veo el número de rastreo", "no veo el código de seguimiento", "no tengo ese dato",
            "no cuento con esa información", "no tengo forma de rastrearlo",
            "no sé cuál es mi número de tracking", "no tengo un número asignado",
            "no tengo la referencia", "no tengo código de seguimiento",
            "me enviaron un paquete pero no tengo tracking", "me dijeron que lo enviaban pero no tengo tracking",
            "el remitente no me dio tracking", "la persona que lo envió no me pasó tracking",
            "no encuentro mi código de rastreo", "perdí mi número de seguimiento",
            "extravié mi código de rastreo", "no sé cuál es el código de seguimiento",
            "me falta el número de rastreo", "no tengo información del tracking",
            "me dijeron que lo enviaron pero sin tracking", "no tengo cómo rastrear mi paquete",
            "no tengo el comprobante de tracking", "no tengo la guía", "no tengo el número de guía",
            "sin número de guía", "no cuento con el número de guía",
            "el remitente no me proporcionó número de seguimiento", "no recibí detalles del tracking",
            "no tengo manera de rastrearlo", "no tengo el tracking a la mano", "no tengo acceso al tracking ahora",
            "no tengo mi tracking disponible", "perdí el número de rastreo",
            "no me aparece el tracking en el correo", "no me enviaron número de rastreo",
            "no me llegó el tracking", "me mandaron el paquete pero no tengo número de rastreo",
            "me dijeron que lo mandaban pero no sé el tracking", "no encuentro el tracking en mi correo",
            "no sé dónde buscar mi tracking", "no tengo el tracking registrado",
            "no tengo mi número de tracking a la mano", "se me olvidó anotar el número de tracking",
            "no me mostraron número de seguimiento", "no me mandaron el tracking",
            "me confirmaron el envío pero no tengo número de rastreo",
            "solo sé que lo enviaron, pero no tengo número de seguimiento"
        };
    }
}