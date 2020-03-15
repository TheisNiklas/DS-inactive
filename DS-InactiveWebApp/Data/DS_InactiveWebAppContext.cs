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

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source = ./SqliteDB.db; Mode=ReadOnly");
    }
}
