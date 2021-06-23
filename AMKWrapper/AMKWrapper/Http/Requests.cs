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

            public static DiscordRequest GetMember(string guild_id, string user_id, string token, TokenType tokenType) {
                return RawRequest(DiscordEndpoints.DiscordMember(guild_id, user_id), token, "GET", "text/html charset=utf-8;", "", tokenType);
            }
            public static DiscordRequest AckInteraction(string raw_content, string interaction_token, string interaction_id, string token, TokenType tokenType) {
                return RawRequest(DiscordEndpoints.Interaction(interaction_id, interaction_token), token, "POST", "application/json", raw_content, tokenType);
            }
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
        public static DiscordRequest RawRequest(string url, string auth, string method, string contentType, string jsonBody, TokenType tokenType, bool isEncoded = false) {
           if (!isEncoded)
                jsonBody = EncodeNonAsciiCharacters(jsonBody);

            if (isLocked) { Debug.Log("Http requests are locked.", ConsoleColor.Red); return new DiscordRequest() { ResponseBody = "", Succeeded = false }; }
            try {
                Debug.Log("[" + method + "] -> " + url, ConsoleColor.Gray);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = contentType;
                httpWebRequest.Method = method;
                if (tokenType == TokenType.Bot && !auth.StartsWith("Bot ")) {
                    auth = "Bot " + auth;
                } else {                   
                    //httpWebRequest.UserAgent = "Mozilla/5.0 (AMKWrapper)";
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
            catch (WebException ex) {
                string responsedata = "";
                
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream)) {
                    responsedata = reader.ReadToEnd();
                }
                int statusCode = (int)((HttpWebResponse)ex.Response).StatusCode;
                if (statusCode != 429) {
                    Debug.Log("Http Error: " + ex.Message, ConsoleColor.Yellow);
                    return new DiscordRequest() { ResponseBody = responsedata, Succeeded = false };
                }  else {
                    Ratelimit rl = Newtonsoft.Json.JsonConvert.DeserializeObject<Ratelimit>(responsedata);
                    Debug.Log("Ratelimited: Waiting for " + rl.retry_after + " seconds", ConsoleColor.DarkYellow);
                    System.Threading.Thread.Sleep((int)rl.retry_after + 2);
                    return RawRequest(url, auth, method, contentType, jsonBody, tokenType, true);
                }
            }
            catch (Exception ex) {
                Debug.Log("Critical error: " + ex.Message, ConsoleColor.Red);
                return new DiscordRequest() { ResponseBody = ex.Message, Succeeded = false };
            }
        }

        public static DiscordRequest RawUpload(string url, string auth, string[] files, TokenType tokenType, string filename = "default") {
            try {
                long length = new System.IO.FileInfo(files[0]).Length;

                string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "multipart/form-data; boundary=" +
                                        boundary;
                request.Method = "POST";
                request.Headers.Add("Authorization", (tokenType == TokenType.Bot ? "Bot " : "") + auth);
                request.KeepAlive = true;

                Stream memStream = new System.IO.MemoryStream();

                var boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" +
                                                                        boundary + "\r\n");
                var endBoundaryBytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" +
                                                                            boundary + "--");


                string formdataTemplate = "Content-Disposition: form-data; name=\"payload_json\"\r\nContent-Type: application/json\r\n{\"content\":\"test\"}\r\n";


                string headerTemplate =
                    "Content-Disposition: form-data; name=\"{0}\"; filename=\""+(filename == "default"?"file.":filename)+"\"\r\n" +
                    "Content-Type: application/octet-stream\r\n\r\n";

                for (int i = 0; i < files.Length; i++) {
                    memStream.Write(boundarybytes, 0, boundarybytes.Length);
                    var header = string.Format(headerTemplate, "uplTheFile", files[i]);
                    var headerbytes = System.Text.Encoding.UTF8.GetBytes(header);

                    memStream.Write(headerbytes, 0, headerbytes.Length);

                    using (var fileStream = new FileStream(files[i], FileMode.Open, FileAccess.Read)) {
                        var buffer = new byte[1024];
                        var bytesRead = 0;
                        while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0) {
                            memStream.Write(buffer, 0, bytesRead);
                        }
                    }
                }

                memStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);
                request.ContentLength = memStream.Length;

                using (Stream requestStream = request.GetRequestStream()) {
                    memStream.Position = 0;
                    byte[] tempBuffer = new byte[memStream.Length];
                    memStream.Read(tempBuffer, 0, tempBuffer.Length);
                    memStream.Close();
                    requestStream.Write(tempBuffer, 0, tempBuffer.Length);
                }

                using (var response = request.GetResponse()) {
                    Stream stream2 = response.GetResponseStream();
                    StreamReader reader2 = new StreamReader(stream2);
                    string serverResponse = "";
                    try { serverResponse = reader2.ReadToEnd(); } catch { }
                    return new DiscordRequest() {
                        ResponseBody = serverResponse,
                        Succeeded = true
                    };

                }
            }
            catch (WebException ex) {
                string responsedata = "";

                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream)) {
                    responsedata = reader.ReadToEnd();
                }
                int statusCode = (int)((HttpWebResponse)ex.Response).StatusCode;
                if (statusCode != 429) {
                    Debug.Log("Http Error while uploading file: " + ex.Message, ConsoleColor.Yellow);
                    return new DiscordRequest() { ResponseBody = responsedata, Succeeded = false };
                }
                else {
                    Ratelimit rl = Newtonsoft.Json.JsonConvert.DeserializeObject<Ratelimit>(responsedata);
                    Debug.Log("Ratelimited while uploading file: Waiting for " + rl.retry_after + " seconds", ConsoleColor.DarkYellow);
                    System.Threading.Thread.Sleep((int)rl.retry_after + 2);
                    return RawUpload(url, auth, files, tokenType);
                }
            }
            catch (Exception ex) {
                Debug.Log("Critical error: " + ex.Message, ConsoleColor.Red);
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
