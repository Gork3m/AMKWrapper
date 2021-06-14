using System;
using System.Collections.Generic;
using System.Text;

namespace AMKWrapper.Http.Endpoints {
    
    public static class DiscordEndpoints {
        public static string endpointVersion = "v9";
        public static string gatewayVersion = "8";

        public static string Message(string channelid) => $@"https://discord.com/api/{endpointVersion}/channels/{channelid}/messages";
        public static string Message(string channelid, string messageid) => $@"https://discord.com/api/{endpointVersion}/channels/{channelid}/messages/{messageid}";
        public static string Gateway() => $@"wss://gateway.discord.gg/?v={gatewayVersion}&encoding=json";

        public static string Interaction(string interaction_id, string interaction_token) => $@"https://discord.com/api/{endpointVersion}/interactions/{interaction_id}/{interaction_token}/callback";
    }
}
