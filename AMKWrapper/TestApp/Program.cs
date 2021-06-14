using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;


using AMKWrapper;
using AMKWrapper.Http;
using AMKWrapper.Types;
using AMKWrapper.Debugging;
using AMKWrapper.EventArgs;
using AMKWrapper.Events;

namespace TestApp
{
    class Program {
        static void Main(string[] args) {
            Debug.Log("Hello world lolol");

            Client client = new Client() {
                clientType = Requests.TokenType.Bot,
                token = File.ReadAllText("D:\\Allah.txt") // token
            };

            client.Initialize();

            DiscordEvents.EventHook main = DiscordEvents.OnNewMessage(delegate (MessageCreateEventArgs args) {
                if (args.message.author.bot) return;
                bool isObfuscationRequest = false;
                isObfuscationRequest = (args.message.attachments.Length == 1 && (args.message.attachments[0].url.ToLower().EndsWith(".lua") | args.message.attachments[0].url.ToLower().EndsWith(".txt")));

                if (isObfuscationRequest) {

                    var msg = args.Reply("_ _", null);

                }
            });



            Console.ReadKey();
            while (true) {
                Console.WriteLine("Type yes to shut down, anything else to cancel\n\n");
                string cc = Console.ReadLine();
                if (cc == "yes") {
                    break;
                }

            }
        }
    }
}
