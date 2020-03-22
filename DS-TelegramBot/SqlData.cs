using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DS_TelegramBot
{
    class SqlData
    {
        private SQLiteConnection database { get; set; }
        public SqlData(string connectionString)
        {
            database = new SQLiteConnection(connectionString);
        }
        public List<Village> getBBInRadius(string playername, int radius)
        {
            Player player = getPlayerByName(playername);
            var BBs = new List<Village>();
            var villages = database.Table<Village>().Where(x => x.player == player.id).ToList();

            foreach (var village in villages)
            {
                string sqlString = @"SELECT DISTINCT * FROM village WHERE village.player = 0 AND (village.x - " + village.x + ") * (village.x - " + village.x + ") + (village.y - " + village.y + ") * (village.y - " + village.y + ") <= " + radius * radius;
                BBs.AddRange(database.Query<Village>(sqlString));
                //BBs.AddRange(database.Table<Village>().Where(vil => vil.player == 0).Where(vil => ((village.x - vil.x) * (village.x - vil.x) + (village.y - vil.y) * (village.y - vil.y) <= radius * radius)).ToList());
            }
            var test = BBs.Distinct().ToList();
            return BBs.Distinct().ToList();
        }
        private Player getPlayerByName(string playername)
        {
            return database.Table<Player>().Where(x => x.name == playername).FirstOrDefault();
        }
    }

    [Table("village")]
    class Village
    {
        [PrimaryKey]
        public long id { get; set; }
        public string name { get; set; }
        public long x { get; set; }
        public long y { get; set; }
        public long player { get; set; }
        public long points { get; set; }
        public Village() { }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Village objAsPart = obj as Village;
            if (objAsPart == null) return false;
            else return Equals(objAsPart);
        }

        public bool Equals(Village v)
        {
            if (v == null) return false;
            else if (id == v.id) return true;
            return false;
        }

        public override int GetHashCode()
        {
            int hashId = id == 0 ? 0 : id.GetHashCode();

            return hashId;
        }
    }

    [Table("player")]
    public class Player
    {
        [PrimaryKey]
        public long id { get; set; }
        public string name { get; set; }
        public long ally { get; set; }
        public long villages { get; set; }
        public long points { get; set; }
        public long rank { get; set; }
        public DateTime lastActive { get; set; }
        public Player() { }
    }
}
