using System;
using AMKWrapper;
using AMKWrapper.Http;
using AMKWrapper.Debugging;
using System.IO;

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

            Console.WriteLine(client.SendMessage("Hello world!", "853698668049072138").content);

            Console.ReadLine();


            client.Disconnect();
            Console.ReadLine();
        }
    }
}
