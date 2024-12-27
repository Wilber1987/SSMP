using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CAPA_DATOS;

namespace UI
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Leer el contenido del cuerpo de la solicitud
            context.Request.EnableBuffering(); // Habilita el almacenamiento en búfer
            var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
            context.Request.Body.Position = 0; // Reinicia la posición del cuerpo

            // Registrar detalles de la solicitud
            LoggerServices.AddMessageInfo("Request Path: {Path}: " +  context.Request.Path);
            LoggerServices.AddMessageInfo("Request Method: {Method}: " +  context.Request.Method);
            LoggerServices.AddMessageInfo("Request Headers: {Headers}: " +  context.Request.Headers);
            LoggerServices.AddMessageInfo("Request Body: {Body}: " +  requestBody);

            // Continuar con el siguiente middleware
            await _next(context);
        }
    }
}