using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;
using Microsoft.Data.Sqlite;

namespace sqlite_app
{
    class Program
    {

        static string logString = "";
        static void Main(string[] args)
        {
            download("player");
            download("village");
            download("ally");
            download("kill_att");
            download("kill_def");
            download("kill_all");


            var connectionStringBuilder = new SqliteConnectionStringBuilder();

            connectionStringBuilder.DataSource = "./SqliteDB.db";

            using (var connection = new SqliteConnection(connectionStringBuilder.ConnectionString))
            {
                updatePlayer(connection);
                updateVillage(connection);
                updateAlly(connection);
                updateKill(connection);

                //outputVillages(connection, "Wichti1", 10, 10);
            }
            outputLog();
        }

        static void outputVillages(SqliteConnection connection, string playername, int radius, int days)
        {
            Player player = null;
            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = "SELECT id, name, ally, villages, points, rank, lastActive FROM player WHERE name = '" + playername + "'";

            using (var reader = selectCmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    player = new Player(reader);
                }
            }

            if (player == null)
            {
                // TODO Exception log
                return;
            }

            List<Village> villages = new List<Village>();

            selectCmd = connection.CreateCommand();
            selectCmd.CommandText = "SELECT id, name, x, y, player, points FROM village WHERE player = " + player.id + "";

            using (var reader = selectCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    villages.Add(new Village(reader));
                }
            }

            foreach (var village in villages)
            {
                selectCmd.CommandText = @"SELECT DISTINCT player.id, player.name, player.ally, player.villages, player.points, player.rank, player.lastActive FROM player LEFT JOIN village on player.id = village.player WHERE lastActive <= datetime('now', '-" + days + @" day') 
                                            AND(village.x - " + village.x + ") * (village.x - " + village.x + ") + (village.y - " + village.y + ") * (village.y - " + village.y + ") <= " + radius * radius;

                using (var sw = new System.IO.StreamWriter("./inactive.txt", false))
                {
                    using (var reader = selectCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var playerinactive = new Player(reader);
                            sw.WriteLine(playerinactive.name);
                        }
                    }
                }
            }
        }

        static void download(string filename)
        {
            FileInfo fi = new FileInfo("./" + filename + ".txt");
            if (fi.LastWriteTime <= DateTime.Now.AddHours(-23))
            {
                WebClient Client = new WebClient();
                Client.DownloadFile("https://dep12.die-staemme.de/map/" + filename + ".txt", "./" + filename + ".txt");
                logString += filename + ".txt downloaded\n";
            }
            else
            {
                logString += filename + ".txt not downloaded, time < 23 hours\nInfos: fi.date=" + fi.LastWriteTime;
            }
        }

        static void outputLog()
        {
            using (var sw = new System.IO.StreamWriter("./log.txt", false))
            {
                sw.WriteLine(logString);
            }
        }

        static void updatePlayer(SqliteConnection connection)
        {

            connection.Open();

            //Create a table (drop if already exists first):

            var createTableCmd = connection.CreateCommand();
            createTableCmd.CommandText = "CREATE TABLE IF NOT EXISTS player(id int PRIMARY KEY, name TEXT NOT NULL, ally int, villages int, points int, rank int, lastActive DateTime)";
            createTableCmd.ExecuteNonQuery();

            //Seed some data:
            using (var transaction = connection.BeginTransaction())
            {
                string line;
                var insertCmd = connection.CreateCommand();
                var readCmd = connection.CreateCommand();
                System.IO.StreamReader file = new System.IO.StreamReader("./player.txt");
                while ((line = file.ReadLine()) != null)
                {
                    Player playerNew = new Player(line);
                    readCmd.CommandText = "SELECT id, name, ally, villages, points, rank, lastActive FROM player WHERE id = " + playerNew.id;
                    using (var reader = readCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var player = new Player(reader);

                            insertCmd.CommandText = "UPDATE player SET name = '" + playerNew.name + "', ally=" + playerNew.ally + ", villages=" + playerNew.villages + ", points = " + playerNew.points + ", rank= " + playerNew.rank;

                            if (player.points != playerNew.points)
                            {
                                insertCmd.CommandText += ", lastActive = date('now')";
                            }
                            insertCmd.CommandText += " WHERE id = " + playerNew.id;


                            insertCmd.ExecuteNonQuery();
                        }
                        else
                        {
                            insertCmd.CommandText = "INSERT INTO player (id, name, ally, villages, points, rank, lastActive) VALUES(" + playerNew.id + ",'" + playerNew.name + "'," + playerNew.ally + "," + playerNew.villages + "," + playerNew.points + ","
                                + playerNew.rank + ", date('now'))";
                            insertCmd.ExecuteNonQuery();
                        }
                    }

                }
                file.Close();

                transaction.Commit();
            }

            //Read the newly inserted data:
            //var selectCmd = connection.CreateCommand();
            //selectCmd.CommandText = "SELECT id, name, ally, villages, points, rank, lastActive FROM player WHERE lastActive < date('now', '-10 day')";

            //using (var sw = new System.IO.StreamWriter("./inactive.txt", false))
            //{
            //    using (var reader = selectCmd.ExecuteReader())
            //    {
            //        while (reader.Read())
            //        {
            //            var player = new Player(reader);
            //            sw.WriteLine(player.name);
            //        }
            //    }
            //}
        }
        static void updateVillage(SqliteConnection connection)
        {
            connection.Open();

            //Create a table (drop if already exists first):

            var createTableCmd = connection.CreateCommand();
            createTableCmd.CommandText = "CREATE TABLE IF NOT EXISTS village(id int PRIMARY KEY, name TEXT NOT NULL, x int, y int, player int, points int)";
            createTableCmd.ExecuteNonQuery();

            //Seed some data:
            using (var transaction = connection.BeginTransaction())
            {
                string line;
                var insertCmd = connection.CreateCommand();
                var readCmd = connection.CreateCommand();
                System.IO.StreamReader file = new System.IO.StreamReader("./village.txt");
                while ((line = file.ReadLine()) != null)
                {
                    Village village = new Village(line);
                    readCmd.CommandText = "SELECT id, name, x, y, player, points FROM village WHERE id = " + village.id;
                    using (var reader = readCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            //var village = new Village(reader);
                            insertCmd.CommandText = "UPDATE village SET name = '" + village.name + "', x=" + village.x + ", y=" + village.y + ", player = " + village.player + ", points= " + village.points + " WHERE id = " + village.id;
                            insertCmd.ExecuteNonQuery();
                        }
                        else
                        {
                            insertCmd.CommandText = "INSERT INTO village (id, name, x, y, player, points) VALUES(" + village.id + ",'" + village.name + "'," + village.x + "," + village.y + "," + village.player + ","
                                + village.points + ")";
                            insertCmd.ExecuteNonQuery();
                        }
                    }

                }
                file.Close();

                transaction.Commit();
            }
        }

        static void updateAlly(SqliteConnection connection)
        {
            connection.Open();

            //Create a table (drop if already exists first):

            var createTableCmd = connection.CreateCommand();
            createTableCmd.CommandText = "CREATE TABLE IF NOT EXISTS ally(id int PRIMARY KEY, name TEXT NOT NULL, tag TEXT NOT NULL, members int, villages int, points int, all_points int, rank int)";
            createTableCmd.ExecuteNonQuery();

            //Seed some data:
            using (var transaction = connection.BeginTransaction())
            {
                string line;
                var insertCmd = connection.CreateCommand();
                var readCmd = connection.CreateCommand();
                System.IO.StreamReader file = new System.IO.StreamReader("./ally.txt");
                while ((line = file.ReadLine()) != null)
                {
                    Ally ally = new Ally(line);
                    readCmd.CommandText = "SELECT id, name, tag, members, villages, points, all_points, rank FROM ally WHERE id = " + ally.id;
                    using (var reader = readCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            //var village = new Village(reader);
                            insertCmd.CommandText = "UPDATE ally SET name = '" + ally.name + "', tag='" + ally.tag + "', members=" + ally.members + ", villages=" + ally.villages + ", points= " + ally.points + ", all_points=" + ally.all_points + ", rank=" + ally.rank+ " WHERE id = " + ally.id;
                            insertCmd.ExecuteNonQuery();
                        }
                        else
                        {
                            insertCmd.CommandText = "INSERT INTO ally (id, name, tag, members, villages, points, all_points, rank) VALUES(" + ally.id + ",'" + ally.name + "','" + ally.tag + "'," + ally.members + "," + ally.villages + ","
                                + ally.points + "," + ally.all_points + "," + ally.rank + ")";
                            insertCmd.ExecuteNonQuery();
                        }
                    }

                }
                file.Close();

                transaction.Commit();
            }
        }

        static void updateKill(SqliteConnection connection)
        {

            connection.Open();

            //Create a table (drop if already exists first):

            var createTableCmd = connection.CreateCommand();
            createTableCmd.CommandText = "CREATE TABLE IF NOT EXISTS kill(id int PRIMARY KEY, att_rank int, att_kill int, lastAtt DateTime, def_rank int, def_kill int, lastDef DateTime, all_rank int, all_kill int, lastAll DateTime, sup_kill int, lastsup DateTime)";
            createTableCmd.ExecuteNonQuery();

            //Seed some data:
            using (var transaction = connection.BeginTransaction())
            {
                string line;
                var insertCmd = connection.CreateCommand();
                var readCmd = connection.CreateCommand();
                System.IO.StreamReader file = new System.IO.StreamReader("./kill_att.txt");
                while ((line = file.ReadLine()) != null)
                {
                    Kill_att killAtt = new Kill_att(line);
                    readCmd.CommandText = "SELECT id, att_rank, att_kill, lastAtt, def_rank, def_kill, lastDef, all_rank, all_kill, lastAll, sup_kill, lastsup FROM kill WHERE id = " + killAtt.id;
                    using (var reader = readCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Kill kill = new Kill(reader);

                            insertCmd.CommandText = "UPDATE kill SET att_rank = " + killAtt.rank + ", att_kill=" + killAtt.kills;

                            if (kill.att_kill != killAtt.kills)
                            {
                                insertCmd.CommandText += ", lastAtt = date('now')";
                            }
                            insertCmd.CommandText += " WHERE id = " + killAtt.id;


                            insertCmd.ExecuteNonQuery();
                        }
                        else
                        {
                            insertCmd.CommandText = "INSERT INTO kill (id, att_rank, att_kill, lastAtt) VALUES(" + killAtt.id + "," + killAtt.rank + "," + killAtt.kills + ", date('now'))";
                            insertCmd.ExecuteNonQuery();
                        }
                    }
                }
                file.Close();

                file = new System.IO.StreamReader("./kill_def.txt");
                while ((line = file.ReadLine()) != null)
                {
                    Kill_def killDef = new Kill_def(line);
                    readCmd.CommandText = "SELECT id, att_rank, att_kill, lastAtt, def_rank, def_kill, lastDef, all_rank, all_kill, lastAll, sup_kill, lastsup FROM kill WHERE id = " + killDef.id;
                    using (var reader = readCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Kill kill = new Kill(reader);

                            insertCmd.CommandText = "UPDATE kill SET def_rank = " + killDef.rank + ", def_kill=" + killDef.kills;

                            if (kill.def_kill != killDef.kills)
                            {
                                insertCmd.CommandText += ", lastDef = date('now')";
                            }
                            insertCmd.CommandText += " WHERE id = " + killDef.id;

                            insertCmd.ExecuteNonQuery();
                        }
                        else
                        {
                            insertCmd.CommandText = "INSERT INTO kill (id, def_rank, def_kill, lastDef) VALUES(" + killDef.id + "," + killDef.rank + "," + killDef.kills + ", date('now'))";
                            insertCmd.ExecuteNonQuery();
                        }
                    }
                }
                file.Close();

                file = new System.IO.StreamReader("./kill_all.txt");
                while ((line = file.ReadLine()) != null)
                {
                    Kill_all killAll = new Kill_all(line);
                    readCmd.CommandText = "SELECT id, att_rank, att_kill, lastAtt, def_rank, def_kill, lastDef, all_rank, all_kill, lastAll, sup_kill, lastsup FROM kill WHERE id = " + killAll.id;
                    using (var reader = readCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Kill kill = new Kill(reader);

                            long killSup = killAll.kills - kill.att_kill - kill.def_kill;

                            insertCmd.CommandText = "UPDATE kill SET all_rank = " + killAll.rank + ", all_kill=" + killAll.kills + ", sup_kill=" + killSup;

                            if (kill.att_kill != killAll.kills)
                            {
                                insertCmd.CommandText += ", lastAll = date('now')";
                            }
                            if (kill.sup_kill != killSup)
                            {
                                insertCmd.CommandText += ", lastSup = date('now')";
                            }


                            insertCmd.CommandText += " WHERE id = " + killAll.id;

                            insertCmd.ExecuteNonQuery();
                        }
                        else
                        {
                            insertCmd.CommandText = "INSERT INTO kill (id, all_rank, all_kill, lastAll, sup_kill, lastsup) VALUES(" + killAll.id + "," + killAll.rank + "," + killAll.kills + ", date('now')," + killAll.kills + ", date('now'))";
                            insertCmd.ExecuteNonQuery();
                        }
                    }
                }
                file.Close();



                transaction.Commit();
            }
        }
    }

    class Player
    {
        public long id { get; set; }
        public string name { get; set; }
        public long ally { get; set; }
        public long villages { get; set; }
        public long points { get; set; }
        public long rank { get; set; }
        public DateTime lastActive { get; set; }
        public Player() { }
        public Player(SqliteDataReader reader) {
            id = reader.GetInt64(0);
            name = reader.GetString(1);
            ally = reader.GetInt64(2);
            villages = reader.GetInt64(3);
            points = reader.GetInt64(4);
            rank = reader.GetInt64(5);
            lastActive = reader.GetDateTime(6);
        }
        public Player(string line)
        {
            var parts = line.Split(',');

            id = long.Parse(parts[0]);
            name = HttpUtility.UrlDecode(parts[1]);
            ally = long.Parse(parts[2]);
            villages = long.Parse(parts[3]);
            points = long.Parse(parts[4]);
            rank = long.Parse(parts[5]);
        }
    }

    class Village
    {
        public long id { get; set; }
        public string name { get; set; }
        public long x { get; set; }
        public long y { get; set; }
        public long player { get; set; }
        public long points { get; set; }
        public Village() { }
        public Village(SqliteDataReader reader)
        {
            id = reader.GetInt64(0);
            name = reader.GetString(1);
            x = reader.GetInt64(2);
            y = reader.GetInt64(3);
            player = reader.GetInt64(4);
            points = reader.GetInt64(5);
        }
        public Village(string line)
        {
            var parts = line.Split(',');

            id = long.Parse(parts[0]);
            name = HttpUtility.UrlDecode(parts[1]);
            x = long.Parse(parts[2]);
            y = long.Parse(parts[3]);
            player = long.Parse(parts[4]);
            points = long.Parse(parts[5]);
        }
    }

    class Ally
    {
        public long id { get; set; }
        public string name { get; set; }
        public string tag { get; set; }
        public long members { get; set; }
        public long villages { get; set; }
        public long points { get; set; }
        public long all_points { get; set; }
        public long rank { get; set; }
        public Ally () { }
        public Ally(SqliteDataReader reader)
        {
            id = reader.GetInt64(0);
            name = reader.GetString(1);
            tag = reader.GetString(2);
            members = reader.GetInt64(3);
            villages = reader.GetInt64(4);
            points = reader.GetInt64(5);
            all_points = reader.GetInt64(6);
            rank = reader.GetInt64(7);
        }
        public Ally(string line)
        {
            var parts = line.Split(',');

            id = long.Parse(parts[0]);
            name = HttpUtility.UrlDecode(parts[1]);
            tag = HttpUtility.UrlDecode(parts[2]);
            members = long.Parse(parts[3]);
            villages = long.Parse(parts[4]);
            points = long.Parse(parts[5]);
            all_points = long.Parse(parts[6]);
            rank = long.Parse(parts[7]);
        }
    }

    class Kill
    {
        public long id { get; set; }
        public long att_rank { get; set; }
        public long att_kill { get; set; }
        public DateTime lastAtt { get; set; }
        public long def_rank { get; set; }
        public long def_kill { get; set; }
        public DateTime lastDef { get; set; }
        public long all_rank { get; set; }
        public long all_kill { get; set; }
        public DateTime lastAll { get; set; }
        public long sup_kill { get; set; }
        public DateTime lastSup { get; set; }
        public Kill() { }
        public Kill(SqliteDataReader reader)
        {
            id = reader.GetInt64(0);
            if (!reader.IsDBNull(1)) att_rank = reader.GetInt64(1);
            if (!reader.IsDBNull(2)) att_kill = reader.GetInt64(2);
            if (!reader.IsDBNull(3)) lastAtt = reader.GetDateTime(3);
            if (!reader.IsDBNull(4)) def_rank = reader.GetInt64(4);
            if (!reader.IsDBNull(5)) def_kill = reader.GetInt64(5);
            if (!reader.IsDBNull(6)) lastDef = reader.GetDateTime(6);
            if (!reader.IsDBNull(7)) all_rank = reader.GetInt64(7);
            if (!reader.IsDBNull(8)) all_kill = reader.GetInt64(8);
            if (!reader.IsDBNull(9)) lastAll = reader.GetDateTime(9);
            if (!reader.IsDBNull(10)) sup_kill = reader.GetInt64(10);
            if (!reader.IsDBNull(11)) lastSup = reader.GetDateTime(11);
        }
    }

    class Kill_all
    {
        public long rank = 0;
        public long id = 0;
        public long kills = 0;

        public Kill_all(string line) 
        {
            var parts = line.Split(',');
            long.TryParse(parts[0], out rank);
            long.TryParse(parts[1], out id);
            long.TryParse(parts[2], out kills);
        }
    }

    class Kill_att
    {
        public long rank;
        public long id;
        public long kills;
        public Kill_att(string line)
        {
            var parts = line.Split(',');
            long.TryParse(parts[0], out rank);
            long.TryParse(parts[1], out id);
            long.TryParse(parts[2], out kills);
        }
    }

    class Kill_def
    {
        public long rank;
        public long id;
        public long kills;
        public Kill_def(string line)
        {
            var parts = line.Split(',');
            long.TryParse(parts[0], out rank);
            long.TryParse(parts[1], out id);
            long.TryParse(parts[2], out kills);
        }
    }
}
