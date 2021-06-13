using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;


using AMKWrapper.Http;
using AMKWrapper.Debugging;
using AMKWrapper.Types;


namespace AMKWrapper {
    /// <summary>
    /// A new discord client (bot, user etc.) BUT PREFER BOTS!
    /// </summary>
    public partial class Client {
        
        public DiscordMessage SendMessage(string message, string channelid) {
            DiscordRequest discordRequest = Requests.API.SendMessage(channelid, JsonConvert.SerializeObject(new DiscordMessage() { content = message }), token, clientType);
            if (discordRequest.Succeeded) {
                return JsonConvert.DeserializeObject<DiscordMessage>(discordRequest.ResponseBody);
            } else {
                throw new Exception(discordRequest.ResponseBody);
                return null;
            }
        }

        public string token { get; set; }
        private Gateway gateway { get; set; }
        public Requests.TokenType clientType { get; set; }
        public void Disconnect() {
            gateway.Disconnect();
        }
        public void Initialize() {
            gateway = new Gateway() {
                token = token,
                TokenType = clientType,
                socket = new System.Net.WebSockets.ClientWebSocket()
            };
            Debug.Log("[1/6] Connecting to gateway", ConsoleColor.DarkYellow);
            if (gateway.Connect()) {
                Debug.Log("[6/6] Connected to gateway", ConsoleColor.Green);

            } else {
                Debug.Log("Client initialization failed", ConsoleColor.Red);
            }

            
        }
    }
}
