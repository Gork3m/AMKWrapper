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
            Embed.DiscordEmbed embed = new Embed.DiscordEmbed() {
                color = Embed.EmbedColor.GreenAsFuck,
                description = "Type monkey if you're a monkey, and type clown if you're a clown",
                title = "Monkeymeter"
            };

            DiscordEvents.EventHook eventHook = DiscordEvents.OnNewMessage(delegate (MessageCreateEventArgs args) {
                if (args.message.content != "monkey") return;
                client.SendMessage("<@" + args.message.author.id + "> is a monkey :monkey:", args.message.channel_id);
                
            });

            DiscordEvents.EventHook eventHook2 = DiscordEvents.OnNewMessage(delegate (MessageCreateEventArgs args) {

                if (args.message.content != "clown") return;
                client.SendMessage("<@" + args.message.author.id + "> is a clown :clown:", args.message.channel_id);
            });

            var msg = client.SendMessage(embed, "853698668049072138");
            


            Console.ReadLine();
            eventHook2.DeleteHook();
            Console.ReadLine();
            client.Disconnect();
            Console.ReadLine();
        }
    }
}
