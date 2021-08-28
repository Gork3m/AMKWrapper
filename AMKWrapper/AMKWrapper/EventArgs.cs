using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

using AMKWrapper.Http;
using AMKWrapper.Types;

namespace AMKWrapper.EventArgs
{
    public class MemberJoinEventArgs {
        public DiscordMember member { get; set; }
        public Client client { get; set; }

        /// <summary>
        /// Kicks the user who just joined
        /// </summary>
        /// <param name="guild_id"></param>
        /// <param name="user_id"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public DiscordMember Kick(string reason = "") {
            DiscordRequest discordRequest = Requests.API.KickMember(member.guild_id, member.user.id, client.token, reason, client.clientType);
            if (discordRequest.Succeeded) {
                return JsonConvert.DeserializeObject<DiscordMember>(discordRequest.ResponseBody);
            }
            else {
                throw new Exception(discordRequest.ResponseBody);
            }
        }
        /// <summary>
        /// Bans the user who just joined
        /// </summary>
        /// <param name="reason"></param>
        /// <returns></returns>
        public DiscordMember Ban(string reason = "", int deletedays = 0) {
            DiscordRequest discordRequest = Requests.API.BanMember(member.guild_id, member.user.id, client.token, reason,deletedays, client.clientType);
            if (discordRequest.Succeeded) {
                return JsonConvert.DeserializeObject<DiscordMember>(discordRequest.ResponseBody);
            }
            else {
                throw new Exception(discordRequest.ResponseBody);
            }
        }

    }
    public class InteractionCreateEventArgs {
        public Interaction interaction { get; set; }
        public Client client { get; set; }

        public bool Ack(Types.SocketTypes.InteractionReplyType ackType, string message = null, Embed.DiscordEmbed embed = null, ComponentContainer[] components = null) {
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


        /// <summary>
        /// Kicks the user who sent the message
        /// </summary>
        /// <param name="guild_id"></param>
        /// <param name="user_id"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public DiscordMember Kick(string reason = "") {
            DiscordRequest discordRequest = Requests.API.KickMember(message.guild_id, message.author.id, client.token, reason, client.clientType);
            if (discordRequest.Succeeded) {
                return JsonConvert.DeserializeObject<DiscordMember>(discordRequest.ResponseBody);
            }
            else {
                throw new Exception(discordRequest.ResponseBody);
            }
        }
        /// <summary>
        /// Bans the user who sent the message
        /// </summary>
        /// <param name="reason"></param>
        /// <returns></returns>
        public DiscordMember Ban(string reason = "", int deletedays = 0) {
            DiscordRequest discordRequest = Requests.API.BanMember(message.guild_id, message.author.id, client.token, reason, deletedays, client.clientType);
            if (discordRequest.Succeeded) {
                return JsonConvert.DeserializeObject<DiscordMember>(discordRequest.ResponseBody);
            }
            else {
                throw new Exception(discordRequest.ResponseBody);
            }
        }

        /// <summary>
        /// Replies to the message with 'reply' feature
        /// </summary>
        /// <param name="_message"></param>
        /// <param name="embed"></param>
        /// <param name="components"></param>
        /// <returns></returns>
        public DiscordMessage Reply(string _message = null, Embed.DiscordEmbed embed = null, ComponentContainer[] components = null) {
           
            DiscordRequest discordRequest = Requests.API.SendMessage(message.channel_id, JsonConvert.SerializeObject(new SocketTypes.DiscordMessage_Send() {
                content = _message,
                embeds = (embed == null ? new Embed.DiscordEmbed[] { } : new Embed.DiscordEmbed[] { embed }),
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
