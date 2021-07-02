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
        public class UserSettings {
            int lastPlatformLock { get; set; }
        }
        static void Main(string[] args) {
            Debug.Log("Hello world lolol");
            try {
                Directory.CreateDirectory("./usercache");
            }
            catch { }
            Client client = new Client() {
                clientType = Requests.TokenType.Bot,
                token = File.ReadAllText("D:\\Allah.txt") // token
            };


            client.Initialize();
            client.UpdateStatus(new DiscordStatus(DiscordStatus.Status.Dnd, new DiscordActivity[] { new DiscordActivity() { name = "upload a file to obfuscate!", type = DiscordActivity.ActivityType.Watching } }));

            string loading = "<a:loading:838691283622166548>";
            string[] platforms = new string[] { "<:Other:839430519506862080> **`Other`** : Runs on almost all platforms *(5.1 to 5.4)*. If your script is made for a specific game, select corresponding platform instead.", "<:Roblox:839430176714129408> **`Roblox`** : Runs on roblox softwares only. *(For server scripts, use 'Other' platform).*", "<:CSGO:839430421183463454> **CS:GO** : Runs on gamesense only.", "<:FiveM:839430304019775488> **FiveM** : Runs on FiveM only *(Cfex/5.3)*." };

            DiscordEvents.EventHook main = DiscordEvents.OnNewMessage(delegate (MessageCreateEventArgs args) {
               
                if (args.message.author.bot) return;
                bool isObfuscationRequest = false;
                isObfuscationRequest = (args.message.attachments.Length == 1 && (args.message.attachments[0].url.ToLower().EndsWith(".lua") | args.message.attachments[0].url.ToLower().EndsWith(".txt")));

                if (args.message.guild_id == null && isObfuscationRequest) {
                    bool passed = false;
                    try {
                        if (Array.IndexOf(client.GetMemberFromGuild("838677105906417665", args.message.author.id).roles, "838679030013100042") != -1) {
                            passed = true;
                        }

                    } catch { }
                    if (!passed) {
                        args.Reply("You will be manually verified, please wait.");
                        return;
                    }

                }

                if (isObfuscationRequest) {

                    Embed.DiscordEmbed embed = new Embed.DiscordEmbed() {
                        author = new Embed.EmbedAuthor() {
                            name = args.message.author.username + "#" + args.message.author.discriminator,
                            icon_url = Embed.GetAvatarUrl(args.message.author)
                        },
                        color = Embed.EmbedColor.Grayish,
                        title = "MoonSec V3",
                        description = "Select features that you want to use, and press **obfuscate**.",
                        fields = new Embed.EmbedField[] {
                            new Embed.EmbedField() {
                                name = "Selected Features:",
                                value = platforms[0]
                            }
                        }
                    };
                    
                    bool antiTamperEnabled = false;
                    bool maxSecurityEnabled = false;
                    string maxsecurityDesc = "\n🔒 **`Max Security`** : Enables code optimization, uses more secure structures and increases security. Best when used with anti tamper. Not recommended on big scripts.";
                    string antitamperDesc = "\n🔒 **`Anti Tamper`** : This uses loadstring or load, so your platform must support one of them. Uses a cool loader-like protection to ensure the safety of the VM as well as your code. File size might be 80kb more than usual.";
                    int platform = 0;
                    Component antiTamperBtn = new Component("🔓 Anti Tamper", Component.ButtonStyle.Red, "at", false);
                    Component platformBtn = new Component("Other platforms", Component.ButtonStyle.Blue, "pl", false, new DiscordEmoji() { name = "Other", id = "839430519506862080", animated = false });
                    Component maxSecurityBtn = new Component("🔓 Max Security", Component.ButtonStyle.Red, "ms", false);
                    Component obfuscateBtn = new Component("Obfuscate", Component.ButtonStyle.Blue, "obf", false);

                    ComponentContainer line1 = new ComponentContainer(new Component[] { antiTamperBtn, maxSecurityBtn, obfuscateBtn });

                    DropdownOption _luaOpt = new DropdownOption("Other", "Runs on sandboxes, repl etc", "otheropt", true, new DiscordEmoji() { id = "839430519506862080", name = "Other" });
                    DropdownOption _robloxOpt = new DropdownOption("Roblox", "Runs on executors", "robloxopt", false, new DiscordEmoji() { id = "839430176714129408", name = "Roblox" });
                    DropdownOption _csgoOpt = new DropdownOption("CS:GO", "Runs on gamesense", "csgoopt", false, new DiscordEmoji() { id = "839430421183463454", name = "CSGO" });
                    DropdownOption _fivemOpt = new DropdownOption("FiveM", "Runs on FiveM", "fivemopt", false, new DiscordEmoji() { id = "839430304019775488", name = "FiveM" });

                    Component dropdown = new Component("dropdownaction", "Select options", new DropdownOption[] { _luaOpt, _robloxOpt, _csgoOpt, _fivemOpt }, 1, 1);

                    var msg = args.Reply(null, embed, new ComponentContainer[] { new ComponentContainer(new Component[] {dropdown}), line1 });
                    
                    DiscordEvents.EventHook act = new DiscordEvents.EventHook();
                    act = DiscordEvents.OnInteraction(delegate (InteractionCreateEventArgs args2) {
                        if (args2.interaction.message.id != msg.id) return;
                        if (args2.interaction.user == null) {
                            args2.interaction.user = args2.interaction.member.user;
                        }

                        if (args2.interaction.user.id == args.message.author.id) {
                            if (args2.interaction.data.custom_id == "dropdownaction") {
                                int oldpf = platform;
                                string[] opts2 = new string[] { "otheropt", "robloxopt", "csgoopt", "fivemopt" };
                                platform = Array.IndexOf(opts2, args2.interaction.data.values[0]);

                                
                                embed.fields[0].value = embed.fields[0].value.Replace(platforms[oldpf], platforms[platform]);
                                

                                if (platform == 0) {

                                    dropdown.options[0].@default = true;
                                    dropdown.options[1].@default = false;
                                    dropdown.options[2].@default = false;
                                    dropdown.options[3].@default = false;

                                }
                                if (platform == 1) {

                                    dropdown.options[0].@default = false;
                                    dropdown.options[1].@default = true;
                                    dropdown.options[2].@default = false;
                                    dropdown.options[3].@default = false;

                                }
                                if (platform == 2) {

                                    dropdown.options[0].@default = false;
                                    dropdown.options[1].@default = false;
                                    dropdown.options[2].@default = true;
                                    dropdown.options[3].@default = false;

                                }
                                if (platform == 3) {

                                    dropdown.options[0].@default = false;
                                    dropdown.options[1].@default = false;
                                    dropdown.options[2].@default = false;
                                    dropdown.options[3].@default = true;

                                }
                            }
                            if (args2.interaction.data.custom_id == "at") {
                                antiTamperEnabled = !antiTamperEnabled;
                                embed.fields[0].value = embed.fields[0].value.Replace(antitamperDesc, "");
                                if (antiTamperEnabled) {
                                    embed.fields[0].value += antitamperDesc;
                                }
                                antiTamperBtn.style = (int)(antiTamperEnabled ? Component.ButtonStyle.Green : Component.ButtonStyle.Red);
                                antiTamperBtn.label = antiTamperEnabled ? antiTamperBtn.label.Replace("Disabled", "Enabled").Replace("🔓","🔒") : antiTamperBtn.label.Replace("Enabled", "Disabled").Replace("🔒", "🔓");

                            }
                            if (args2.interaction.data.custom_id == "ms") {
                                maxSecurityEnabled = !maxSecurityEnabled;
                                maxSecurityBtn.style = (int)(maxSecurityEnabled ? Component.ButtonStyle.Green : Component.ButtonStyle.Red);
                                embed.fields[0].value = embed.fields[0].value.Replace(maxsecurityDesc, "");
                                if (maxSecurityEnabled) {
                                    embed.fields[0].value += maxsecurityDesc;
                                }
                                maxSecurityBtn.label = maxSecurityEnabled ? maxSecurityBtn.label.Replace("Disabled", "Enabled").Replace("🔓", "🔒") : maxSecurityBtn.label.Replace("Enabled", "Disabled").Replace("🔒", "🔓");

                            }
                            if (args2.interaction.data.custom_id == "obf") {
                                antiTamperBtn.disabled = true;
                                maxSecurityBtn.disabled = true;
                                platformBtn.disabled = true;
                                obfuscateBtn.disabled = true;
                                obfuscateBtn.label = "Obfuscating...";
                                obfuscateBtn.emoji = new DiscordEmoji() {
                                    id = "838691283622166548",
                                    name = "loading",
                                    animated = true
                                };
                                act.DeleteHook();
                                embed.fields = new Embed.EmbedField[] {
                                    embed.fields[0],
                                    new Embed.EmbedField() {
                                        name = "Progress:",
                                        value = "Getting file.. " + loading
                                    }
                                };



                            }
                            line1 = new ComponentContainer(new Component[] { antiTamperBtn, maxSecurityBtn, obfuscateBtn });



                            try {
                                args2.Ack(SocketTypes.InteractionReplyType.Edit, null, embed, new ComponentContainer[] { new ComponentContainer(new Component[] { dropdown }), line1 });
                            }
                            catch {
                                // 401
                                Debug.Log("Nigga trap", ConsoleColor.Cyan);
                               // client.EditMessage(embed, msg, new Button.ButtonContainer[] { line1 });
                            }

                            if (args2.interaction.data.custom_id == "obf") {
                                System.Net.WebClient wc = new System.Net.WebClient();
                                string data = wc.DownloadString(args.message.attachments[0].url);
                                Thread.Sleep(1000);
                                embed.fields[1].value = "**Received file :white_check_mark:**\nObfuscating.. " + loading;
                                client.EditMessage(embed, msg, new ComponentContainer[] { new ComponentContainer(new Component[] { dropdown }), line1 });
                                Thread.Sleep(1000);
                                embed.fields[1].value = "**Received file :white_check_mark:**\n**Obfuscated** :white_check_mark:\nUploading.. " + loading;
                                client.EditMessage(embed, msg, new ComponentContainer[] { new ComponentContainer(new Component[] { dropdown }), line1 });
                                Thread.Sleep(1000);
                                embed.fields[1].value = "**Received file :white_check_mark:**\n**Obfuscated** :white_check_mark:\n**Done!** :white_check_mark:";
                                embed.color = Embed.EmbedColor.CuteGreen;
                                embed.description = "**Enabled features:**\n<:" + platformBtn.emoji.name + ":" + platformBtn.emoji.id + "> `" + platformBtn.label + "` " + (antiTamperEnabled ? "\n`🔒 Anti Tamper` " : "") + (maxSecurityEnabled ? "\n`🔒 Max Security` " : "") + "\nGenerated **`nothing`**\nRandomized **`nothing`**";
                                
                                line1.components[2].label = "✅ Obfuscated";
                                line1.components[2].emoji = null;
                                var uploaded = client.UploadFiles(new string[] { "./usercache/foreskin.txt" }, "854386842391806012", "Protected.lua");
                                embed.fields = new Embed.EmbedField[] {

                                    new Embed.EmbedField() {
                                        name = "Download:",
                                        value = "**"+uploaded.attachments[0].url+"**"
                                    }
                                };
                                Debug.Log("Before");
                                client.EditMessage(embed, msg, new ComponentContainer[] {  });
                                Debug.Log("After");
                                


                            }

                        } else {
                            args2.Ack(SocketTypes.InteractionReplyType.Pong);
                        }

                    });

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

            client.Disconnect();
            Console.ReadKey();
        }
    }
}
