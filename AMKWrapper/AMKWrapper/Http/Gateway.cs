using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.Net;
using System.Web;
using System.Net.WebSockets;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;

using AMKWrapper.Http;
using AMKWrapper.Http.Endpoints;
using AMKWrapper.Debugging;
using AMKWrapper.Types;
using AMKWrapper.EventArgs;

namespace AMKWrapper.Http {
    public partial class Gateway {
        public static Dictionary<string, Action<MessageCreateEventArgs>> messageCreatedCallbacks = new Dictionary<string, Action<MessageCreateEventArgs>>();
        public static Dictionary<string, Action<InteractionCreateEventArgs>> interactionCreatedCallbacks = new Dictionary<string, Action<InteractionCreateEventArgs>>();
        public Client _client { get; set; }
        public Dictionary<string, Action<string, Client>> eventPool = new Dictionary<string, Action<string, Client>>() {
            ["MESSAGE_CREATE"] = delegate(string raw, Client client) {
                JObject data = JObject.Parse(raw);
                
                MessageCreateEventArgs args = new MessageCreateEventArgs() {
                    message = JsonConvert.DeserializeObject<DiscordMessage>(data["d"].ToString()),
                    client = client
                };
                for (int i = 0; i < messageCreatedCallbacks.Count; i++) {

                    int index = i;
                    Task.Run(() => {
                        try {
                            messageCreatedCallbacks.ElementAt(index).Value(args);
                            }
                        catch(Exception ex) { Debug.Log("Error occured while running MESSAGE_CREATE hook @ ["+ messageCreatedCallbacks.ElementAt(index).Key +"] " + ex.Message, ConsoleColor.DarkRed); }
                        });
                    
                }
            },

            ["INTERACTION_CREATE"] = delegate (string raw, Client client) {
                JObject data = JObject.Parse(raw);

                InteractionCreateEventArgs args = new InteractionCreateEventArgs() {
                    interaction = JsonConvert.DeserializeObject<Interaction>(data["d"].ToString()),
                    client = client
                };
                for (int i = 0; i < interactionCreatedCallbacks.Count; i++) {

                    int index = i;
                    Task.Run(() => {
                        try {
                            interactionCreatedCallbacks.ElementAt(index).Value(args);
                        }
                        catch (Exception ex) { Debug.Log("Error occured while running INTERACTION_CREATE hook @ [" + interactionCreatedCallbacks.ElementAt(index).Key + "] " + ex.Message, ConsoleColor.DarkRed); }
                    });

                }
            }

        };
        public bool isActiveSocket { get; set; }
        public bool connected { get; set; }

        public DiscordStatus _cliStatus { get; set; }
        public Gateway() {
            isActiveSocket = true;
            connected = false;
        }
        public string token { get; set; }
        public ClientWebSocket socket { get; set; }
        public Requests.TokenType TokenType { get; set; }
        public bool Connect(int timeout = 30) {
            if (!isActiveSocket) return false;
            socket = new ClientWebSocket();
            connected = false;
            try {
               socket.ConnectAsync(new Uri(DiscordEndpoints.Gateway()), CancellationToken.None);
            }
            catch (Exception ex) { Debug.Log("Gateway Error: " + ex.Message, ConsoleColor.Red); return false; }

            Debug.Log("[2/6] Established websocket connection");
            GatewayMain();

            int t = 0;
            while (!connected && t < timeout) {
                Thread.Sleep(1000);
                t++;
            }
            if (t >= timeout) {
                Debug.Log("Timeout..", ConsoleColor.DarkRed);
                return false;
            }

            return connected;

        }
        public void Disconnect() {
            isActiveSocket = false;
            Debug.Log("Manually disconnected..", ConsoleColor.DarkYellow);
            try {
                socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "IntentionalDisconnection", CancellationToken.None);
            }
            catch {
                
            }
        }
        
        private async Task GatewayMain() {
            bool isValidSession = true;

            Task.Factory.StartNew(async () =>
            {
                try {

                    await Task.Delay(15000);
                    while (isValidSession) {
                        await Task.Delay(2000);
                        if (isValidSession) {
                            if (socket.State != WebSocketState.Open) {

                                try {
                                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "IntentionalDisconnection", CancellationToken.None);
                                }
                                catch { }
                                await Task.Delay(1000);
                                if (isValidSession) {
                                    isValidSession = false;
                                    Connect();
                                }

                            }
                        }
                    }

                }
                catch (Exception ex) {
                    
                }
            });


            try {
                ArraySegment<Byte> bfr = new ArraySegment<byte>(new Byte[4096]);
                WebSocketReceiveResult result = null;
                
                while (isValidSession && isActiveSocket) {
                    bool isEmpty = false;
                    using (var ms = new MemoryStream()) {
                        do {
                            try {
                                result = socket.ReceiveAsync(bfr, CancellationToken.None).Result;               
                                ms.Write(bfr.Array, bfr.Offset, result.Count);
                            }
                            catch { isEmpty = true; break; }
                        }
                        while (!result.EndOfMessage);
                        if (isEmpty) continue;
                        ms.Seek(0, SeekOrigin.Begin);
                        if (result.MessageType == WebSocketMessageType.Close) {
                            using (var reader = new StreamReader(ms, Encoding.UTF8)) {
                                if (!reader.ReadToEnd().Contains("IntentionalDisconnection")) {
                                    ms.Dispose();
                                    isValidSession = false;                                    
                                    if (isActiveSocket) {
                                        Debug.Log("Received Socket.Close", ConsoleColor.Yellow);
                                        await Task.Delay(2000);
                                        Debug.Log("[1/6] Reconnecting..", ConsoleColor.DarkYellow);
                                        Connect();
                                    }
                                }
                            }
                        }

                        try {

                            if (result.MessageType == WebSocketMessageType.Text) {

                                using (var reader = new StreamReader(ms, Encoding.UTF8)) {
                                    string packet = reader.ReadToEnd();

                                    #region Heartbeat task
                                    Opcode opcode = JsonConvert.DeserializeObject<Opcode>(packet);
                                    switch (opcode.op) {

                                        case 10: // Op 10 Hello
                                            Debug.Log("[3/6] Received Hello");
                                            await SendString(socket, "{\"op\": 2, \"d\": { \"token\": \"" + token + "\", \"intents\": 4611, \"presence\": "+JsonConvert.SerializeObject(_cliStatus)+", \"properties\": { \"$os\": \"linux\", \"$browser\": \"AMKWrapper\", \"$device\": \"AMKWrapper\" } } }");
                                            Debug.Log("[4/6] Sent identify packet");
                                            break;

                                        case 9: // Op 9 Invalid session
                                            Debug.Log("Session invalidated by server", ConsoleColor.Yellow);
                                            isValidSession = false;
                                            ms.Dispose();
                                            try {
                                                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "IntentionalDisconnection", CancellationToken.None);
                                            }
                                            catch { }
                                            await Task.Delay(2000);
                                            if (isActiveSocket) {
                                                Debug.Log("[1/6] Reconnecting..", ConsoleColor.DarkYellow);
                                                Connect();
                                            }
                                            break;

                                        case 7: // Op 7 Reconnect
                                            Debug.Log("Reconnection requested by server", ConsoleColor.Yellow);
                                            isValidSession = false;
                                            ms.Dispose();
                                            try {
                                                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "IntentionalDisconnection", CancellationToken.None);
                                            }
                                            catch { }
                                            await Task.Delay(2000);
                                            if (isActiveSocket) {
                                                Debug.Log("[1/6] Reconnecting..", ConsoleColor.DarkYellow);
                                                Connect();
                                            }
                                            break;
                                        case 0:
                                            if (eventPool.ContainsKey(opcode.t)) {
                                                try {
                                                    
                                                    Task.Run(() => eventPool[opcode.t](packet, _client)); // asyncness :sunglasses:
                                                }
                                                catch(Exception ex) {
                                                    Debug.Log("Error while callback: " + opcode.t + ": " + ex.Message, ConsoleColor.DarkRed);
                                                }
                                            }
                                            break;
                                    }

                                   

                                 
                                    if (JsonConvert.DeserializeObject<Opcode>(packet).t == "READY") {

                                        Debug.Log("[5/6] Received ready!");                                        
                                        await Task.Factory.StartNew(async () =>
                                        {
                                            while (isValidSession && isActiveSocket) {
                                                await SendString(socket, "{\"op\": 1, \"d\": null}"); // heartbeating
                                                await Task.Delay(30000);
                                            }
                                        });
                                        connected = true;

                                    }
                                    #endregion
                                    

                                }
                            }
                        }
                        catch(Exception ex) { Debug.Log("Exception while reading gateway packet: " + ex.Message, ConsoleColor.DarkRed); }
                    }
                }
            }
            catch (Exception ex) {
                Debug.Log("Main Gateway Error: " + ex.Message, ConsoleColor.Red);
                
            }
        }

        public async Task SendString(ClientWebSocket wsc, String data) {
            var encoded = Encoding.UTF8.GetBytes(data);
            var buffer = new ArraySegment<Byte>(encoded, 0, encoded.Length);
            await wsc.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }

    }
}
