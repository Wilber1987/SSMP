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