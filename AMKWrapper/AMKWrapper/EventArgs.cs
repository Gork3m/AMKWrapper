using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

using AMKWrapper.Http;
using AMKWrapper.Types;

namespace AMKWrapper.EventArgs
{
    public class MessageCreateEventArgs {
        public DiscordMessage message { get; set; }
        public Client client { get; set; }

        public DiscordMessage Reply(string _message) {
           
            DiscordRequest discordRequest = Requests.API.SendMessage(message.channel_id, JsonConvert.SerializeObject(new DiscordMessage() {
                content = _message,
                message_reference = new MessageReference() {
                    channel_id = message.channel_id,
                    guild_id = message.guild_id,
                    message_id = message.id
                }
            }), client.token, client.clientType);
            if (discordRequest.Succeeded) {
                return JsonConvert.DeserializeObject<DiscordMessage>(discordRequest.ResponseBody);
            }
            else {
                throw new Exception(discordRequest.ResponseBody);
            }
        }
    }   


}
