using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DS_InactiveWebApp.Models
{
    [Table("kill")]
    public class Kill
    {
        [Key]
        [DisplayName("ID")]
        public long id { get; set; }

        [DisplayName("Rang")]
        public long att_rank { get; set; }
        [DisplayName("Bash-Punkte")]
        public long att_kill { get; set; }
        [DisplayName("Letzte Aktivität")]
        public DateTime lastAtt { get; set; }

        [DisplayName("Rang")]
        public long def_rank { get; set; }
        [DisplayName("Bash-Punkte")]
        public long def_kill { get; set; }
        [DisplayName("Letzte Aktivität")]
        public DateTime lastDef { get; set; }

        [DisplayName("Rang")]
        public long all_rank { get; set; }
        [DisplayName("Bash-Punkte")]
        public long all_kill { get; set; }
        [DisplayName("Letzte Aktivität")]
        public DateTime lastAll { get; set; }
        public Kill() { }
        public Kill(SqliteDataReader reader, int offset = 0)
        {
            if (!reader.IsDBNull(0 + offset)) id = reader.GetInt64(0 + offset);
            if (!reader.IsDBNull(1 + offset)) att_rank = reader.GetInt64(1 + offset);
            if (!reader.IsDBNull(2 + offset)) att_kill = reader.GetInt64(2 + offset);
            if (!reader.IsDBNull(3 + offset)) lastAtt = reader.GetDateTime(3 + offset);
            if (!reader.IsDBNull(4 + offset)) def_rank = reader.GetInt64(4 + offset);
            if (!reader.IsDBNull(5 + offset)) def_kill = reader.GetInt64(5 + offset);
            if (!reader.IsDBNull(6 + offset)) lastDef = reader.GetDateTime(6 + offset);
            if (!reader.IsDBNull(7 + offset)) all_rank = reader.GetInt64(7 + offset);
            if (!reader.IsDBNull(8 + offset)) all_kill = reader.GetInt64(8 + offset);
            if (!reader.IsDBNull(9 + offset)) lastAll = reader.GetDateTime(9 + offset);
        }
    }
}
