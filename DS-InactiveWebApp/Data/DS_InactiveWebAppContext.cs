using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DS_InactiveWebApp.Models;

namespace DS_InactiveWebApp.Data
{
    public class DS_InactiveWebAppContext : DbContext
    {
        public DS_InactiveWebAppContext (DbContextOptions<DS_InactiveWebAppContext> options)
            : base(options)
        {
        }

        public DbSet<DS_InactiveWebApp.Models.Player> Player { get; set; }
        public DbSet<DS_InactiveWebApp.Models.Village> Village { get; set; }
        public DbSet<DS_InactiveWebApp.Models.Kill> Kill { get; set; }
        public DbSet<DS_InactiveWebApp.Models.Ally> Ally { get; set; }

        
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source = L:/Pogrammier Projekte/Die Stämme/DS-inactive/bin/netcoreapp3.1/SqliteDB.db; Mode=ReadOnly");
            //=> options.UseSqlite("Data Source = ./SqliteDB.db; Mode=ReadOnly");
    }
}
