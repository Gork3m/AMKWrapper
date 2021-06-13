using System;
using System.Collections.Generic;
using System.Text;

namespace AMKWrapper.Types
{
    public class DiscordUser {
        public string avatar { get; set; }
        public bool bot { get; set; }
        public string discriminator { get; set; }
        public string id { get; set; }
        public string username { get; set; }
    }
    public class DiscordMessage {
        public string content { get; set; }
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
