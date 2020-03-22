using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace DS_TelegramBot
{
    public class SqlChats
    {
        private SQLiteConnection database { get; set; }
        public SqlChats(string connectionString)
        {
            database = new SQLiteConnection(connectionString);

            database.CreateTable<Chat>();
        }
        public Chat getChat(long id)
        {
            return database.Table<Chat>().Where(x => x.id == id).FirstOrDefault();
        }
        public List<Chat> getAllChats()
        {
            return database.Table<Chat>().ToList();
        }
        public void setChat(Chat chat)
        {
            if(database.Table<Chat>().Where(x => x.id == chat.id).FirstOrDefault() == null)
            {
                database.Insert(chat);
            }
            else
            {
                database.Update(chat);
            }
        }
        public void setPlayername(long id, string playername)
        {
            Chat chat = database.Table<Chat>().Where(x => x.id == id).FirstOrDefault();
            chat.playername = playername;
            database.Update(chat);
        }
        public void setRadius(long id, int radius)
        {
            Chat chat = database.Table<Chat>().Where(x => x.id == id).FirstOrDefault();
            chat.radius = radius;
            database.Update(chat);
        }
        public void setInitFlag(long id, int flag)
        {
            Chat chat = database.Table<Chat>().Where(x => x.id == id).FirstOrDefault();
            chat.flagInit = flag;
            database.Update(chat);
        }
        public int getInitFlag(long id)
        {
            Chat chat = database.Table<Chat>().Where(x => x.id == id).FirstOrDefault();
            return chat.flagInit;
        }
        public void setPlayernameFlag(long id, int flag)
        {
            Chat chat = database.Table<Chat>().Where(x => x.id == id).FirstOrDefault();
            chat.flagSetPlayername = flag;
            database.Update(chat);
        }
        public int getPlayernameFlag(long id)
        {
            Chat chat = database.Table<Chat>().Where(x => x.id == id).FirstOrDefault();
            return chat.flagSetPlayername;
        }
        public void setRadiusFlag(long id, int flag)
        {
            Chat chat = database.Table<Chat>().Where(x => x.id == id).FirstOrDefault();
            chat.flagSetRadius = flag;
            database.Update(chat);
        }
        public int getRadiusFlag(long id)
        {
            Chat chat = database.Table<Chat>().Where(x => x.id == id).FirstOrDefault();
            return chat.flagSetRadius;
        }
        public void setSendMessagesFlag(long id, bool flag)
        {
            Chat chat = database.Table<Chat>().Where(x => x.id == id).FirstOrDefault();
            chat.flagSendMessages = flag;
            database.Update(chat);
        }
        public bool getSendMessagesFlag(long id)
        {
            Chat chat = database.Table<Chat>().Where(x => x.id == id).FirstOrDefault();
            return chat.flagSendMessages;
        }
    }
    public class Chat
    {
        public Chat() {}
        public Chat(long id)
        {
            this.id = id;
        }
        [PrimaryKey]
        public long id { get; set; } = 0;
        public string playername { get; set; }
        public int radius { get; set; }
        public int flagInit { get; set; }
        public int flagSetPlayername { get; set; }
        public int flagSetRadius { get; set; }
        public bool flagSendMessages { get; set; } = true;
        public string sentVillages { get; set; }

        public bool isInitialised()
        {
            if (!string.IsNullOrWhiteSpace(playername) && radius != 0) return true;
            return false;
        }
    }
}
