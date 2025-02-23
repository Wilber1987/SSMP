# SSMP

https://github.com/Wilber1987/SSMP.git

clonar con todos los submodulos: 
´´git clone --recurse-submodules -j8 https://github.com/Wilber1987/SSMP.git ´´

api de llama http://localhost:11434/api/chat


http:190.99.117.243

http://190.99.117.243/webapp/webchat


dotnet publish -c Release -o C:\wwwroot\ssmp
ssmpV20241127

IA MODELS - SMALL
"llama3.2:1b"
ollama run llama3.2:1b
https://ollama.com/library/llama3.2

IA MODELS -  STANDARD
"phi3:3.8b"
ollama run phi3:3.8b
https://ollama.com/library/phi3:3.8b

include web chat in anypage
 ``<div class="ssmp-chat-container">
		<style>
			body {
				margin: 0;
				font-family: Arial, sans-serif;
			}

			/* Botón flotante */
			.toggle-button {
				position: fixed;
				bottom: 10px;
				right: 10px;
				padding: 10px 15px;
				background-color: #007bff;
				color: white;
				border: none;
				border-radius: 5px;
				cursor: pointer;
				font-size: 14px;
				box-shadow: 0 4px 6px rgba(0, 0, 0, 0.2);
			}

			/* Contenedor flotante del chat */
			.chat-wrapper {
				position: fixed;
				bottom: 50px;
				/* Separado del botón */
				right: 10px;
				width: 400px;
				height: 600px;
				background-color: #f1f1f1;
				border-radius: 10px;
				box-shadow: 0 4px 6px rgba(0, 0, 0, 0.2);
				display: none;
				overflow: hidden;
				border-radius: 10px;
				/* Oculto por defecto */                
			}
			.chat-wrapper iframe {
				width: 100%;
				height: 100%;
			}
		</style>
		<button class="toggle-button" onclick="toggleChat()">Chat</button>
		<!-- Contenedor del chat -->
		<div class="chat-wrapper" id="chatWrapper">
			<iframe src="http://190.99.117.243/webapp/webchat" frameborder="0"></iframe>
		</div>
		<script>
			function toggleChat() {
				const chatWrapper = document.getElementById('chatWrapper');
				if (chatWrapper.style.display === 'none' || chatWrapper.style.display === '') {
					chatWrapper.style.display = 'block';
				} else {
					chatWrapper.style.display = 'none';
				}
			}
		</script>       
	</div> 
``



sc create MyRabbitC binPath= "C:\rabbit\RabbitMQService.exe"
Ronny Alberto Vega Mendieta
9:56
sc config MyRabbitC start= auto
sc start MyRabbitC
sc delete MyRabbitC


"TemplateImageHeader": "https://localhost:5101/media/image/whatsapp_template_header.png"

{
  "messaging_product": "whatsapp",
  "to": "+50588078386",
  "type": "template",
  "template": {
	"name": "notificacion_paquete",
	"language": { "code": "es" },
	"components": [
	  {
		"type": "header",
		"parameters": [
		  {
			"type": "IMAGE",
			"image": { "link": "https://chatbot.correos.gob.gt:8443/Media/img/logo.png" }
		  }
		]
	  },
	  {
		"type": "body",
		"parameters": [
		  { "type": "TEXT", "text": "Juan Pérez" },  
		  { "type": "TEXT", "text": "123456789" },  
		  { "type": "TEXT", "text": "GT-09876" }  
		]
	  }
	]
  }
}

se hará uso de la mensajería, para conectarlo a una plataforma  de control de casos, con los que se podrá dar seguimiento de forma eficiente de las consultas que los clientes hagan y de esa forma darles respuesta de forma mas rápida y eficiente.

debe ingresar a la plataforma : "https://chatbot.correos.gob.gt:8443/"
con el usuario "prueba@prueba.net" contraseña "prueba%2025".
desde ahi podra observar: 
1 home: desde aqui podra ver el estado de la dependencia que administra los casos
2 solicitudes pendientes: en caso de encontrar solicitudes que requieran peticiones
3 casos en proceso:  en este menu estara la opcion de ver detalle desde el cual se podra ver la cantidad de tareas creadas para esa consulta, asi mismo se podra observar el historial de mensajes intercambiados por el cliente y el bot que es capaz de recepcionar consultas desde el correo meta whatsapp y messenger(facbook api)


nuevo token: EAARmKPZCahegBO6bQPehSfbqlKGWFuDiEgTQPIwg0pZAVNY50XzKAH8bwiZA24qjEWu1qdl90Lst6Ib8CGWr1g7x3b3fbofBEYs928tu3TZCKlDDOMdtZBLr3ZByetGoOu5mcDX0rRnte2Eskv6amZBENMjvy12X0DRz3fFPZBwYJQDQ6KJYk0vgaj8gZC641t68QVAZDZD


se hará uso de la mensajería, para conectarlo a una plataforma  de control de casos, con los que se podrá dar seguimiento de forma eficiente de las consultas que los clientes hagan y de esa forma darles respuesta de forma mas rápida y eficiente. como se ha mencionado en la descripcion correos de guatemala tiene diversos sistemas de consultas para que los usuarios puedan obtener informacion sobre sus paquetes, uno de ellos es el sistema de seguimiento, la intencion de nuestro desarrollo es integrar la informacion del seguimiento con nuestro chatbot que permita de forma organica desde nuestras redes sociales (facebook, whatsapp he instagram) dar la informacion a los clientes, aliviando la carga de estos al no tener que consultar en muchos sitios para obtener la informacion desada, la plataforma de chatbot respondera a todas esas consultas y las gestionara desde el backend de la app permitiendo a correos de guatemala procesar la informacion de forma eficiente, en la captura de pantalla agregada muestra claramente como el panel de administracion puede recepcionar consultas de clientes diversos, agruparlos en casos y permite tanto al asistente virtual como a los asistentes de soporte tecnico atender a los clientes de forma rapida con la informacion que estos requieren a la mano.