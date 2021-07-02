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


        public DiscordMember GetMemberFromGuild(string guild_id, string user_id) {
            DiscordRequest discordRequest = Requests.API.GetMember(guild_id, user_id, token, clientType);
            if (discordRequest.Succeeded) {
                return JsonConvert.DeserializeObject<DiscordMember>(discordRequest.ResponseBody);
            }
            else {
                throw new Exception(discordRequest.ResponseBody);
            }
        }
        /// <summary>
        /// Updates client presence (online, dnd, afk, playing, listening to etc.)
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public void UpdateStatus(Types.DiscordStatus status) {
            this.status = status;
            gateway._cliStatus = this.status;
            gateway.SendString(gateway.socket, JsonConvert.SerializeObject(new SocketTypes.ChangeStatus() { d = status, op = 3 })).Wait();
        }

        /// <summary>
        /// Sends a message to the specified channel
        /// </summary>
        /// <param name="message"></param>
        /// <param name="channelid"></param>
        /// <returns></returns>
        public DiscordMessage SendMessage(string message, string channelid, ComponentContainer[] components = null) {
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
        public DiscordMessage SendMessage(Embed.DiscordEmbed embed, string channelid, ComponentContainer[] components = null) {
            DiscordRequest discordRequest = Requests.API.SendMessage(channelid, JsonConvert.SerializeObject(new SocketTypes.DiscordMessage_Send() { embeds = new Embed.DiscordEmbed[] { embed }, components = components }), token, clientType);
            if (discordRequest.Succeeded) {
                return JsonConvert.DeserializeObject<DiscordMessage>(discordRequest.ResponseBody);
            }
            else {
                throw new Exception(discordRequest.ResponseBody);
            }
        }

        public DiscordMessage UploadFiles(string[] files, string channelid, string filename = "default") {
            

            DiscordRequest discordRequest = Requests.RawUpload(AMKWrapper.Http.Endpoints.DiscordEndpoints.Message(channelid), token, files, clientType, filename);
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
        public DiscordMessage EditMessage(Embed.DiscordEmbed embed, DiscordMessage message, ComponentContainer[] components = null) {
            DiscordRequest discordRequest = Requests.API.EditMessage(message.channel_id, message.id, "{ \"embeds\": [" + JsonConvert.SerializeObject(embed) + "] , \"components\": "+JsonConvert.SerializeObject(components)+"}", token, clientType);
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
        public DiscordStatus status { get; set; }
        public void Initialize() {
            _self = new Client() {
                token = token,
                clientType = clientType
            };
            status = new DiscordStatus(DiscordStatus.Status.Online);
            
            gateway = new Gateway() {
                token = token,
                TokenType = clientType,
                socket = new System.Net.WebSockets.ClientWebSocket(),
                _client = _self,
                _cliStatus = status
                
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
