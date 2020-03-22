using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using Microsoft.Data.Sqlite;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace DS_InactiveWebApp.Models
{
    [Table("player")]
    public class Player
    {
        [Key]
        [DisplayName("ID")]
        public long id { get; set; }
        [DisplayName("Spielername")]
        public string name { get; set; }
        [Column("ally")]
        [DisplayName("Stamm-ID")]
        public long allyId { get; set; }
        [DisplayName("Dörfer")]
        public long villages { get; set; }
        [DisplayName("Punkte")]
        public long points { get; set; }
        [DisplayName("Rang")]
        public long rank { get; set; }
        [DisplayName("zuletzt aktiv")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime lastActive { get; set; }
        [NotMapped]
        public Kill kill { get; set; }
        [NotMapped]
        public Ally ally { get; set; }
        public Player() { }
        public Player(SqliteDataReader reader)
        {
            id = reader.GetInt64(0);
            name = reader.GetString(1);
            allyId = reader.GetInt64(2);
            villages = reader.GetInt64(3);
            points = reader.GetInt64(4);
            rank = reader.GetInt64(5);
            lastActive = reader.GetDateTime(6);
            ally = new Ally(reader, 7);
            kill = new Kill(reader, 15);
        }
    }
}
