using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace DS_InactiveWebApp.Models
{
    public class Kill
    {
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
        public Kill(SqliteDataReader reader)
        {
            id = reader.GetInt64(0);
            if (!reader.IsDBNull(1))
            {
                att_rank = reader.GetInt64(1);
            }
            if (!reader.IsDBNull(2))
            {
                att_kill = reader.GetInt64(2);
            }
            if (!reader.IsDBNull(3))
            {
                lastAtt = reader.GetDateTime(3);
            }
            if (!reader.IsDBNull(4))
            {
                def_rank = reader.GetInt64(4);
            }
            if (!reader.IsDBNull(5))
            {
                def_kill = reader.GetInt64(5);
            }
            if (!reader.IsDBNull(6))
            {
                lastDef = reader.GetDateTime(6);
            }
            if (!reader.IsDBNull(7))
            {
                all_rank = reader.GetInt64(7);
            }
            if (!reader.IsDBNull(8))
            {
                all_kill = reader.GetInt64(8);
            }
            if (!reader.IsDBNull(9))
            {
                lastAll = reader.GetDateTime(9);
            }
        }
    }
}
