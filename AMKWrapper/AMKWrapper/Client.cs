using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;


using AMKWrapper.Http;
using AMKWrapper.Debugging;
using AMKWrapper.Types;
using AMKWrapper.EventArgs;

namespace AMKWrapper {
    

    /// <summary>
    /// A new discord client (bot, user etc.) BUT PREFER BOTS!
    /// </summary>
    public partial class Client {

        

        /// <summary>
        /// Sends a message to the specified channel
        /// </summary>
        /// <param name="message"></param>
        /// <param name="channelid"></param>
        /// <returns></returns>
        public DiscordMessage SendMessage(string message, string channelid, Button.ButtonContainer[] components = null) {
            DiscordRequest discordRequest = Requests.API.SendMessage(channelid, JsonConvert.SerializeObject(new SocketTypes.DiscordMessage_Send() { content = message, components = components }), token, clientType);
            if (discordRequest.Succeeded) {
                return JsonConvert.DeserializeObject<DiscordMessage>(discordRequest.ResponseBody);
            }
            else {
                throw new Exception(discordRequest.ResponseBody);
            }
        }
        /// <summary>
        /// Sends an embed to specified channel
        /// </summary>
        /// <param name="embed"></param>
        /// <param name="channelid"></param>
        /// <returns></returns>
        public DiscordMessage SendMessage(Embed.DiscordEmbed embed, string channelid, Button.ButtonContainer[] components = null) {
            DiscordRequest discordRequest = Requests.API.SendMessage(channelid, JsonConvert.SerializeObject(new SocketTypes.DiscordMessage_Send() { embed = embed, components = components }), token, clientType);
            if (discordRequest.Succeeded) {
                return JsonConvert.DeserializeObject<DiscordMessage>(discordRequest.ResponseBody);
            }
            else {
                throw new Exception(discordRequest.ResponseBody);
            }
        }

        /// <summary>
        /// Edits a message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageid"></param>
        /// <param name="channelid"></param>
        /// <returns></returns>
        public DiscordMessage EditMessage(string newMessage, DiscordMessage message) {
            DiscordRequest discordRequest = Requests.API.EditMessage(message.channel_id, message.id, JsonConvert.SerializeObject(new DiscordMessage() { content = newMessage }), token, clientType);
            if (discordRequest.Succeeded) {
                return JsonConvert.DeserializeObject<DiscordMessage>(discordRequest.ResponseBody);
            }
            else {
                throw new Exception(discordRequest.ResponseBody);
            }
        }
        /// <summary>
        /// Sends an embed to specified channel
        /// </summary>
        /// <param name="embed"></param>
        /// <param name="channelid"></param>
        /// <returns></returns>
        public DiscordMessage EditMessage(Embed.DiscordEmbed embed, DiscordMessage message) {
            DiscordRequest discordRequest = Requests.API.EditMessage(message.channel_id, message.id, "{ \"embed\": " + JsonConvert.SerializeObject(embed) + " }", token, clientType);
            if (discordRequest.Succeeded) {
                return JsonConvert.DeserializeObject<DiscordMessage>(discordRequest.ResponseBody);
            }
            else {
                throw new Exception(discordRequest.ResponseBody);
            }
        }
        public static Random rnd = new Random();
        
        
        public string token { get; set; }
        public static Gateway gateway { get; set; }
        public Requests.TokenType clientType { get; set; }
        public Client _self { get; set; }
        public void Disconnect() {
            gateway.Disconnect();
        }
        public void Initialize() {
            _self = new Client() {
                token = token,
                clientType = clientType
            };
            gateway = new Gateway() {
                token = token,
                TokenType = clientType,
                socket = new System.Net.WebSockets.ClientWebSocket(),
                _client = _self
                
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
