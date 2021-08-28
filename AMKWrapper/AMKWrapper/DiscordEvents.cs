using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;


using AMKWrapper.Http;
using AMKWrapper.Debugging;
using AMKWrapper.Types;
using AMKWrapper.EventArgs;

namespace AMKWrapper.Events {
    public static class DiscordEvents {

        public static Random rnd = new Random();
        public static EventHook OnNewMessage(Action<MessageCreateEventArgs> callback) {
            EventHook eventHook = new EventHook();
            Gateway.messageCreatedCallbacks.Add(eventHook.HookId, callback);
            return eventHook;
        }
        public static EventHook OnInteraction(Action<InteractionCreateEventArgs> callback) {
            EventHook eventHook = new EventHook();
            Gateway.interactionCreatedCallbacks.Add(eventHook.HookId, callback);
            return eventHook;
        }
        public static EventHook OnMemberJoin(Action<MemberJoinEventArgs> callback) {
            EventHook eventHook = new EventHook();
            Gateway.memberJoinCallbacks.Add(eventHook.HookId, callback);
            return eventHook;
        }

        public partial class EventHook {
            private string GetGuid() {
                return GetRandomHex(8) + "-" + GetRandomHex(4) + "-" + GetRandomHex(6);
            }
            private string GetRandomHex(int len) {
                string hex = "0123456789abcdef";
                string ret = "";
                for (int i = 0; i < len; i++) {
                    ret += hex[rnd.Next(0, hex.Length)].ToString();
                }
                return ret;
            }
            public string HookId { get; }
            public EventHook() {
                HookId = GetGuid();
            }
            /// <summary>
            /// Make sure to delete it after you're done
            /// </summary>
            public void DeleteHook() {

                if (Gateway.messageCreatedCallbacks.ContainsKey(HookId)) {
                    Gateway.messageCreatedCallbacks.Remove(HookId);
                }

                Debug.Log("Removed Hook @ " + HookId);

            }
        }
    }
}
