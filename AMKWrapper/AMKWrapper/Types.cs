using System;
using System.Collections.Generic;
using System.Text;

namespace AMKWrapper.Types
{

    public class DiscordActivity {
        public string name { get; set; }
        public ActivityType type { get; set; }
        public enum ActivityType {
            Playing,
            Streaming,
            Listening,
            Watching
        }
    }
    public class DiscordStatus {
        public DiscordStatus(Status _status, DiscordActivity[] _activities = null, bool _afk = false) {
            activities = (_activities == null ? new DiscordActivity[] { } : _activities);
            afk = _afk;
            status = statnames[(int)_status];
        }
        private string[] statnames = { "dnd", "idle", "online", "invisible" };
        public enum Status {
            Dnd,
            Idle,
            Online,
            Invisible
        }
        public int since { get; set; }
        public DiscordActivity[] activities { get; set; }
        public string status { get; set; }
        public bool afk { get; set; }

    }
    public class Ratelimit {
        public string message { get; set; }
        public double retry_after { get; set; }
    }
    public class DiscordEmoji {
        public string id { get; set; }
        public string name { get; set; }
        public bool animated { get; set; }
    }
    public class Attachment {
        public string url { get; set; }
        public int size { get; set; }
        public string filename { get; set; }
    }
    public class InteractionData {
        public string custom_id { get; set; }
    }
    public class DiscordMember {
        public DiscordUser user { get; set; }
        public string[] roles { get; set; }
    }
    public class Interaction {

        public DiscordUser user { get; set; }
        public DiscordMember member { get; set; }
        public InteractionData data { get; set; }
        public DiscordMessage message { get; set; }
        public string id { get; set; }
        public string token { get; set; }
    }
    public class DiscordGuild {
        public string id { get; set; }
        public string name { get; set; }
        public string icon { get; set; }
        public bool owner { get; set; }
        public string permissions { get; set; }
        public string[] features { get; set; }

    }
    public partial class Button {

        
        public partial class ButtonContainer {
            public ButtonContainer(Button[] buttons) {
                if (buttons.Length > 5) {
                    throw new Exception("Too many buttons (5x5 max.)");
                }
                components = buttons;
                type = 1;
            }
            public Button[] components { get; set; }
            public int type { get; set; }
        }
        
        public Button(string text, ButtonStyle buttonStyle, string data, bool isDisabled = false, DiscordEmoji _emoji = null) {
            if (buttonStyle == ButtonStyle.Link) {
                url = data;
            }
            else {
                custom_id = data;
            }
            label = text;
            type = 2;
            style = (int)buttonStyle;
            disabled = isDisabled;
            emoji = _emoji;
        }
        public DiscordEmoji emoji { get; set; }
        public int type { get; set; }
        public string label { get; set; }
        public int style { get; set; }
        public bool disabled { get; set; }
        public string custom_id { get; set; }
        public string url { get; set; }
        public enum ButtonStyle {
            Blue = 1,
            Grey = 2,
            Green = 3,
            Red = 4,
            Link = 5
        }
    }
    
    public class SocketTypes {

        public class ChangeStatus {
            public int op { get; set; }
            public DiscordStatus d { get; set; }
        }
        public enum InteractionReplyType {
            /// <summary>
            /// Edit the message
            /// </summary>
            Edit = 7,
            /// <summary>
            /// Do absolutely nothing, but don't fail either.
            /// </summary>
            Pong = 6
        }
        public partial class InteractionReply {
            public class InteractionData_Send {
                public string content { get; set; }
                public Embed.DiscordEmbed[] embeds { get; set; }
                
                public Button.ButtonContainer[] components { get; set; }
            }
            public InteractionReply(InteractionReplyType replyType, Button.ButtonContainer[] replyComponents = null, string message = null, Embed.DiscordEmbed embed = null) {
                InteractionData_Send dat = new InteractionData_Send() {
                    content = message,
                    embeds = new Embed.DiscordEmbed[] { embed },
                    components = replyComponents
                };
                type = (int)replyType;
                data = dat;

            }
            public InteractionData_Send data { get; set; }
            public int type { get; set; }

        }
        public class DiscordMessage_Send {
            public string guild_id { get; set; }
            public string content { get; set; }
            public Embed.DiscordEmbed[] embeds { get; set; }
            public Button.ButtonContainer[] components { get; set; }
            public MessageReference message_reference { get; set; }
            public DiscordUser author { get; set; }
            public string id { get; set; }
            public string channel_id { get; set; }
        }
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
            Grayish = 0x757575,
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
        public Embed.DiscordEmbed[] embeds { get; set; }

        public Embed.DiscordEmbed embed { get; set; }
        public MessageReference message_reference { get;set; }
        public DiscordUser author { get; set; }
        public string id { get; set; }
        public string channel_id { get; set; }

        public Attachment[] attachments { get; set; }
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
