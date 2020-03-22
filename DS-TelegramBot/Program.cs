using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace DS_TelegramBot
{
    public static class Program
    {
        static ITelegramBotClient botClient;
        static SqlChats sqlChats;
        static SqlData sqlData;
        public static async Task Main()
        {
            botClient = new TelegramBotClient("965325023:AAEBQSxxB7VVZvhvYtvNLRoJSMZ78LS0VqU");
            sqlChats = new SqlChats("chats.db");
            sqlData = new SqlData("SqliteDB.db");
            

            var me = await botClient.GetMeAsync();

            botClient.OnMessage += BotOnMessage;
            botClient.OnCallbackQuery += BotOnCallbackQuerryReceived;

            int min = 65 - DateTime.Now.Minute;
            int mill = min * 60000;
            int period = 60 * 60 * 1000;
            System.Threading.Timer dt = new System.Threading.Timer(OnTimeElapsed, null, mill,  period);
            
            botClient.StartReceiving(Array.Empty<UpdateType>());
            Console.WriteLine($"Start listening for @{me.Username}");

            Console.ReadLine();
            botClient.StopReceiving();
        }

        private static async void OnTimeElapsed(object state)
        {
            Console.WriteLine("Sending out notifications");
            List<Chat> chats = sqlChats.getAllChats();
            foreach (Chat chat in chats)
            {
                if (!chat.flagSendMessages)
                {
                    List<Coord> oldCoords = new List<Coord>();
                    if (!string.IsNullOrWhiteSpace(chat.sentVillages))
                    {
                        var sendBBs = chat.sentVillages.Split(";");
                        sendBBs = sendBBs.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                        foreach (var sendBB in sendBBs)
                        {
                            oldCoords.Add(Coord.Parse(sendBB));
                        }
                    }

                    List<Village> BBs = sqlData.getBBInRadius(chat.playername, chat.radius);
                    List<Coord> newCoords = new List<Coord>();
                    foreach (var BB in BBs)
                    {
                        Coord newCoord = new Coord(BB);
                        bool t = oldCoords.Contains(newCoord);
                        if (!oldCoords.Contains(newCoord)) newCoords.Add(newCoord);
                    }

                    List<List<InlineKeyboardButton>> ikbList = new List<List<InlineKeyboardButton>>();

                    int counter = 0;
                    int row = 0;
                    foreach (var newCoord in newCoords)
                    {
                        if (counter == 0) ikbList.Add(new List<InlineKeyboardButton>());
                        if (counter < 4)
                        {
                            ikbList[row].Add(InlineKeyboardButton.WithUrl(newCoord.x + "|" + newCoord.y, "https://dep12.die-staemme.de/game.php?screen=map#" + newCoord.x + ";" + newCoord.y));
                            if (counter == 3)
                            {
                                counter = 0;
                                row++;
                            }
                            else counter++;
                        }
                    }
                    var inlineKeyboardMarkup = new InlineKeyboardMarkup(ikbList);

                    if (newCoords.Count > 0)
                    {
                        await botClient.SendTextMessageAsync(
                            chatId: chat.id,
                            text: "Hey, I found something for you:",
                            replyMarkup: inlineKeyboardMarkup
                        );
                    }

                    string newSentVillages = "";

                    foreach (var BB in BBs)
                    {
                        newSentVillages += BB.x + "," + BB.y + ";";
                    }
                    chat.sentVillages = newSentVillages;
                    sqlChats.setChat(chat);
                }
            }
        }

        private static async void BotOnMessage(object sender, MessageEventArgs e)
        {
            var message = e.Message;

            if (message.Text != null && message.Type == MessageType.Text)
            {
                if (MessageIsCommand(sender, e))
                {
                    Chat chat = sqlChats.getChat(message.Chat.Id);
                    switch (message.Text.Split(' ').First())
                    {
                        
                        case "/start":
                            if (sqlChats.getChat(message.Chat.Id) == null)
                            {
                                var inlineKeyboardMarkup = new InlineKeyboardMarkup(new[]
                                {
                                    new[]
                                    {
                                        InlineKeyboardButton.WithCallbackData("Ready", "firstInit,ready")
                                    }
                                });
                                await botClient.SendTextMessageAsync(
                                    chatId: message.Chat,
                                    text: "Hello!\n" +
                                          "It doesn't seem like we have seen each other yet.\n" +
                                          "I will start the initialisation then. Ready?",
                                    replyMarkup: inlineKeyboardMarkup
                                );
                                sqlChats.setChat(chat);
                            }
                            else if (!chat.isInitialised())
                            {
                                var inlineKeyboardMarkup = new InlineKeyboardMarkup(new[]
                                {
                                    new[]
                                    {
                                        InlineKeyboardButton.WithCallbackData("Ready", "firstInit,ready")
                                    }
                                });
                                await botClient.SendTextMessageAsync(
                                    chatId: message.Chat,
                                    text: "Welcome back.\n" +
                                          "It doesn't seem like we have finished the initialisation.\n" +
                                          "I will restart the initialisation then. Ready?",
                                    replyMarkup: inlineKeyboardMarkup
                                    );
                            }
                            else
                            {
                                var inlineKeyboardMarkup = new InlineKeyboardMarkup(new[]
                                {
                                    new[]
                                    {
                                        InlineKeyboardButton.WithCallbackData("Yes", "newInit,true"),
                                        InlineKeyboardButton.WithCallbackData("No", "newInit,false"),
                                    }
                                });

                                await botClient.SendTextMessageAsync(
                                    chatId: message.Chat,
                                    text: "Welcome again. Would you like to repeate the initialisation?",
                                    replyMarkup: inlineKeyboardMarkup
                                    );
                            }
                            break;
                        case "/help":
                            await botClient.SendTextMessageAsync(
                                  chatId: message.Chat,
                                  text: "These are all the available commands:\n" +
                                        "/start - Start the convesation or restart the initialisation.\n" +
                                        "/help - This help information.\n" +
                                        "/settings - Show your current settings.\n" +
                                        "/setplayername - Set yout _Die Stämme_ Playername.\n" +
                                        "/setradius - Set the radius around your villages in which new BBs are detected.\n" +
                                        "/stopnotify - Stops the Bot from sending you new messages.\n" +
                                        "/startnotify - Reenables the Bot to send you messages",
                                  parseMode: ParseMode.Markdown
                            );
                            break;
                        case "/settings":
                            await botClient.SendTextMessageAsync(
                                  chatId: message.Chat,
                                  text: "Your current settings are:\n\n" +
                                        "`Playername`: " + (chat.playername.Equals("") ? "*not set*" : chat.playername) + "\n" +
                                        "`Radius`: " + (chat.radius == 0 ? "*not set*" : chat.radius.ToString()),
                                  parseMode: ParseMode.Markdown
                            );
                            break;
                        case "/setplayername":
                            await botClient.SendTextMessageAsync(
                                  chatId: message.Chat,
                                  text: "Please send your *Die Stämme* playername, this input ist case-sensitive!",
                                  parseMode: ParseMode.Markdown
                            );
                            sqlChats.setPlayernameFlag(message.Chat.Id, 1);
                            break;
                        case "/setradius":
                            await botClient.SendTextMessageAsync(
                                  chatId: message.Chat,
                                  text: "Please send a value for the radius around your villages.\nYou will he notified, when a new BB apears within this area.",
                                  parseMode: ParseMode.Markdown
                            );
                            sqlChats.setRadiusFlag(message.Chat.Id, 1);
                            break;
                        case "/stopnotify":
                            sqlChats.setSendMessagesFlag(message.Chat.Id, false);
                            await botClient.SendTextMessageAsync(
                                  chatId: message.Chat,
                                  text: "Ok, the I will not send you any messages on my own anymore.",
                                  parseMode: ParseMode.Markdown
                            );
                            break;
                        case "/startnotify":
                            sqlChats.setSendMessagesFlag(message.Chat.Id, true);
                            await botClient.SendTextMessageAsync(
                                  chatId: message.Chat,
                                  text: "Ok, the I will start again with sending you messages with new BBs.",
                                  parseMode: ParseMode.Markdown
                            );
                            break;
                        default:
                            await botClient.SendTextMessageAsync(
                                chatId: message.Chat,
                                text: message.Text + " is not a valid command. Use /help for more information."
                            );
                            break;
                    }
                }
                else if (initIsActive(message.Chat.Id))
                {
                    InitChat(message);
                }
                else if (sqlChats.getPlayernameFlag(message.Chat.Id) != 0)
                {
                    sqlChats.setPlayername(message.Chat.Id, message.Text);
                    await botClient.SendTextMessageAsync(
                            chatId: message.Chat,
                            text: "Your playername has been set to " + message.Text,
                            parseMode: ParseMode.Markdown
                    );
                    sqlChats.setPlayernameFlag(message.Chat.Id, 0);
                }
                else if (sqlChats.getRadiusFlag(message.Chat.Id) != 0)
                {
                    if (int.TryParse(message.Text, out int i))
                    {
                        sqlChats.setRadius(message.Chat.Id, int.Parse(message.Text));
                        await botClient.SendTextMessageAsync(
                                chatId: message.Chat,
                                text: "The Radius has been set to " + message.Text,
                                parseMode: ParseMode.Markdown
                        );
                        sqlChats.setRadiusFlag(message.Chat.Id, 0);
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(
                            chatId: message.Chat,
                            text: "Please only send a number. Try again."
                        );
                    }
                }
            }
        }

        private static async void BotOnCallbackQuerryReceived(object sender, CallbackQueryEventArgs e)
        {
            var callbackQuery = e.CallbackQuery;
            string[] data = callbackQuery.Data.Split(',');

            switch (data[0])
            {
                case "newInit":
                    if (data[1] == "true")
                    {
                        InitChat(callbackQuery.Message);
                    }
                    else if (data[1] == "false")
                    {
                        await botClient.SendTextMessageAsync(
                            chatId: callbackQuery.Message.Chat,
                            text: "Ok, your configuration will remain as it is."
                        );
                    }
                    break;
                case "firstInit":
                    if (data[1] == "ready")
                    {
                        InitChat(callbackQuery.Message);
                    }
                    break;
                default:
                    break;
            }
        }

        private static bool initIsActive(long id)
        {
            if(sqlChats.getChat(id).flagInit != 0) return true;
            return false;
        }

        private static bool MessageIsCommand (object sender, MessageEventArgs e)
        {
            if (e.Message.Text != null)
            {
                if (e.Message.Text.StartsWith("/")) return true;
                return false;
            }
            return false;
        }

        private static async void InitChat(Message message)
        {
            var chat = sqlChats.getChat(message.Chat.Id);

            if (chat.flagInit == 0)
            {
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat,
                    text: "Please send your _Die Stämme_ playername, this input ist case-sensitive!",
                    parseMode: ParseMode.Markdown
                );
                sqlChats.setInitFlag(chat.id, 1);
                return;
            }
            else if (chat.flagInit == 1)
            {
                sqlChats.setPlayername(chat.id, message.Text);
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat,
                    text: "Your playername has been set to " + message.Text
                );
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat,
                    text: "Please send a value for the radius around your villages.\nYou will he notified, when a new BB apears within this area."
                );
                sqlChats.setInitFlag(chat.id, 2);
                return;
            }
            else if(chat.flagInit == 2)
            {
                if (int.TryParse(message.Text, out int i))
                {
                    sqlChats.setRadius(chat.id, int.Parse(message.Text));
                    await botClient.SendTextMessageAsync(
                            chatId: message.Chat,
                            text: "The Radius has been set to " + message.Text
                    );
                    await botClient.SendTextMessageAsync(
                            chatId: message.Chat,
                            text: "You will bei notified when new BBs appear, to stop that use /stopnotify"
                    );
                    sqlChats.setInitFlag(chat.id, 0);
                }
                else
                {
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat,
                        text: "Please only send a number. Try again."
                    );
                }
                return;
            }
        }
    }
    class Coord
    {
        public Coord() { }
        public Coord(Village vil)
        {
            this.x = int.Parse(vil.x.ToString());
            this.y = int.Parse(vil.y.ToString());
        }
        public static Coord Parse(string s)
        {
            var parts = s.Split(",");
            return new Coord() { x = int.Parse(parts[0]), y = int.Parse(parts[1]) };
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Coord objAsPart = obj as Coord;
            if (objAsPart == null) return false;
            else return Equals(objAsPart);
        }

        public bool Equals(Coord c)
        {
            if (c == null) return false;
            else if (c.x.Equals(x) && c.y.Equals(y)) return true;
            return false;
        }

        public override int GetHashCode()
        {
            int hashx = x == 0 ? 0 : y.GetHashCode();
            int hashy = y == 0 ? 0 : y.GetHashCode();

            return hashx ^ hashy;
        }

        public int x { get; set; }
        public int y { get; set; }
    }
}
