using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

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
                token = File.ReadAllText("config.txt") // token
            };


            client.Initialize();
            client.UpdateStatus(new DiscordStatus(DiscordStatus.Status.Dnd, new DiscordActivity[] { new DiscordActivity() { name = "for attackers", type = DiscordActivity.ActivityType.Watching } }));


            FBIBot.LoadStuff();
            FBIBot.RegisterCommands(client);
            FBIBot.RegisterOtherHandlers(client);

            Console.ReadKey();
            while (true) {
                Console.WriteLine("Type yes to shut down, anything else to cancel\n\n");
                string cc = Console.ReadLine();
                if (cc == "yes") {
                    break;
                }

            }

            client.Disconnect();
            Console.ReadKey();
        }
    }
}
