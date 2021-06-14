using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using System.Net;
using System.Web;

using AMKWrapper.Types;
using AMKWrapper.Debugging;
using AMKWrapper.Http.Endpoints;

namespace AMKWrapper.Http
{
    public static class Requests {
        /// <summary>
        /// Endpoints to interact with the raw API, you usually won't need to use any of these in your code.
        /// </summary>
        public static class API {
            public static DiscordRequest SendMessage(string channelid, string raw_content, string token, TokenType tokenType) {
                return RawRequest(DiscordEndpoints.Message(channelid), token, "POST", "application/json", raw_content, tokenType);
            }

            public static DiscordRequest EditMessage(string channelid, string messageid, string raw_content, string token, TokenType tokenType) {
                return RawRequest(DiscordEndpoints.Message(channelid, messageid), token, "PATCH", "application/json", raw_content, tokenType);
            }
        }
        /// <summary>
        /// Failsafe stuff, allows you to kill http requests instantly
        /// </summary>
        public static class Security {
            public static void LockHttpRequests() {
                
                Debug.Log("Locked all http requestst!", ConsoleColor.Yellow);
                isLocked = true;
            }
            public static void UnlockHttpRequests() {
                Debug.Log("Unlocked all http requests!", ConsoleColor.Yellow);
                isLocked = false;
            }
        }
        private static bool isLocked = false;
       /// <summary>
       /// Specifies token type.
       /// </summary>
        public enum TokenType {
            Bot,
            User // <-- bad boy
        }

        /// <summary>
        /// Used for interacting with the raw API, don't use this unless you know what you're doing :sunglasses:
        /// </summary>
        /// <param name="url"></param>
        /// <param name="auth"></param>
        /// <param name="method"></param>
        /// <param name="contentType"></param>
        /// <param name="jsonBody"></param>
        /// <param name="tokenType"></param>
        /// <returns></returns>
        public static DiscordRequest RawRequest(string url, string auth, string method, string contentType, string jsonBody, TokenType tokenType) {
            jsonBody = EncodeNonAsciiCharacters(jsonBody);
            if (isLocked) { Debug.Log("Http requests are locked.", ConsoleColor.Red); return new DiscordRequest() { ResponseBody = "", Succeeded = false }; }
            try {
                Debug.Log("[" + method + "] -> " + url, ConsoleColor.Gray);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = contentType;
                httpWebRequest.Method = method;
                if (tokenType == TokenType.Bot) {
                    auth = "Bot " + auth;
                } else {                   
                    httpWebRequest.UserAgent = "Mozilla/5.0 (AMKWrapper)";
                }
                httpWebRequest.Host = "discord.com";
                httpWebRequest.Headers.Add("Authorization", auth);
                if (!contentType.Contains("text/html")) {
                    httpWebRequest.ContentLength = jsonBody.Length;
                }

                httpWebRequest.Headers.Add("Accept-Language", "en-US");
                httpWebRequest.Headers.Add("Origin", "http://discord.com");
                httpWebRequest.Headers.Add("Sec-Fetch-Site", "same-origin");
                httpWebRequest.Headers.Add("Sec-Fetch-Mode", "cors");
                httpWebRequest.Headers.Add("Sec-Fetch-Dest", "empty");

                if (!contentType.Contains("text/html")) {
                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream())) {
                        streamWriter.Write(jsonBody);
                        streamWriter.Flush();
                        streamWriter.Close();
                    }
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream())) {
                    var result = streamReader.ReadToEnd();
                    return new DiscordRequest() { ResponseBody = result, Succeeded = true };
                }
            }
            catch (Exception ex) {
                Debug.Log("Http Error: " + ex.Message, ConsoleColor.Red);
                return new DiscordRequest() { ResponseBody = ex.Message, Succeeded = false };
            }
        }
        private static string EncodeNonAsciiCharacters(string value) {
            StringBuilder sb = new StringBuilder();
            foreach (char c in value) {
                if (c > 127) {
                    // This character is too big for ASCII
                    string encodedValue = "\\u" + ((int)c).ToString("x4");
                    sb.Append(encodedValue);
                } else {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
    }
}
