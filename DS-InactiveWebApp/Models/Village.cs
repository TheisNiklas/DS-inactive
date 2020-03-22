using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace DS_InactiveWebApp.Models
{
    [Table("village")]
    public class Village
    {
        [Key]
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
    }
}
