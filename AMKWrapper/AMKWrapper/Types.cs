using System;
using System.Collections.Generic;
using System.Text;

namespace AMKWrapper.Types
{
    public class PacketTypes {

    }
    public class Embed {

        public class DiscordEmbed {
            public string title { get; set; }
            public string description { get; set; }
            public string url { get; set; }
            public string timestamp { get; set; }
            public EmbedColor color { get; set; }
            public EmbedFooter footer { get; set; }
            public EmbedThumbnail thumbnail { get; set; }
            public EmbedImage image { get; set; }
            public EmbedAuthor author { get; set; }
            public EmbedField[] fields { get; set; }


        }
        public static string GetAvatarUrl(DiscordUser user) {
            return "https://cdn.discordapp.com/avatars/" + user.id + "/" + user.avatar + ".png?size=512";
        }
        public class EmbedThumbnail {
            public string url { get; set; }
        }
        public class EmbedImage {
            public string url { get; set; }
        }
        public class EmbedAuthor {
            public string name { get; set; }
            public string icon_url { get; set; }
            public string url { get; set; }

        }
        public class EmbedField {
            public string name { get; set; }
            public string value { get; set; }
            public bool inline { get; set; }
        }

        public class EmbedFooter {
            public string icon_url { get; set; }
            public string text { get; set; }

        }
        public enum EmbedColor {
            CuteGreen = 3586931,
            CuteCyan = 0x36BBAF,

            RedAsFuck = 0xff0000,
            WhiteAsFuck = 0xffffff,
            BlackAsFuck = 0x000000,
            BlueAsFuck = 0x0000ff,
            GreenAsFuck = 0x00ff00,
            
            Purple = 0xaa33aa
        }
    }
    public class DiscordUser {
        public string avatar { get; set; }
        public bool bot { get; set; }
        public string discriminator { get; set; }
        public string id { get; set; }
        public string username { get; set; }
    }
    public class MessageReference {
        public string message_id { get; set; }
        public string channel_id { get; set; }
        public string guild_id { get; set; }
    }
    public class DiscordMessage {
        public string guild_id { get; set; }
        public string content { get; set; }
        public MessageReference message_reference { get;set; }
        public DiscordUser author { get; set; }
        public string id { get; set; }
        public string channel_id { get; set; }
    }
    public class DiscordRequest {
        public string ResponseBody { get; set; }
        public bool Succeeded { get; set; }
    }
    public class Opcode {
        public int op { get; set; }
        public string t { get; set; }
    }


    
}
