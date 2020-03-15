using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace DS_InactiveWebApp.Models
{
    public class Ally
    {
        [DisplayName("ID")]
        public long id { get; set; }

        [DisplayName("Name")]
        public string name { get; set; }
        [DisplayName("Tag")]
        public string tag { get; set; }

        [DisplayName("Mitglieder")]
        public long members { get; set; }
        [DisplayName("Dörfer")]
        public long villages { get; set; }
        [DisplayName("Punkte")]
        public long points { get; set; }
        [DisplayName("Alle Punkte")]
        public long all_points { get; set; }
        [DisplayName("Rang")]
        public long rank { get; set; }

        public Ally() { }
        public Ally(SqliteDataReader reader, int offset = 0)
        {
            id = reader.GetInt64(0 + offset);
            name = reader.GetString(1 + offset);
            tag = reader.GetString(2 + offset);
            members = reader.GetInt64(3 + offset);
            villages = reader.GetInt64(4 + offset);
            points = reader.GetInt64(5 + offset);
            all_points = reader.GetInt64(6 + offset);
            rank = reader.GetInt64(7 + offset);
        }
    }
}
