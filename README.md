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