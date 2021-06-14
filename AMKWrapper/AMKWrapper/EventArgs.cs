using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

using AMKWrapper.Http;
using AMKWrapper.Types;

namespace AMKWrapper.EventArgs
{
    public class InteractionCreateEventArgs {
        public Interaction interaction { get; set; }
        public Client client { get; set; }

        public bool Ack(Types.SocketTypes.InteractionReplyType ackType, string message = null, Embed.DiscordEmbed embed = null, Button.ButtonContainer[] components = null) {
            DiscordRequest discordRequest = Requests.API.AckInteraction(JsonConvert.SerializeObject(new SocketTypes.InteractionReply(ackType,components,message,embed)), interaction.token, interaction.id, client.token, client.clientType);
            if (discordRequest.Succeeded) {
                return true;
            }
            else {
                throw new Exception(discordRequest.ResponseBody);
            }
        }
    }

    public class MessageCreateEventArgs {
        public DiscordMessage message { get; set; }
        public Client client { get; set; }

        public DiscordMessage Reply(string _message = null, Embed.DiscordEmbed embed = null, Button.ButtonContainer[] components = null) {
           
            DiscordRequest discordRequest = Requests.API.SendMessage(message.channel_id, JsonConvert.SerializeObject(new SocketTypes.DiscordMessage_Send() {
                content = _message,
                embed = embed,
                components = components,
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
