using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DS_InactiveWebApp.Data;
using DS_InactiveWebApp.Models;

namespace DS_InactiveWebApp
{
    public class IndexModel : PageModel
    {
        private readonly DS_InactiveWebApp.Data.DS_InactiveWebAppContext _context;

        public IndexModel(DS_InactiveWebApp.Data.DS_InactiveWebAppContext context)
        {
            _context = context;
        }

        public IList<Player> Player { get;set; }
        public IList<Village> Village { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; }
        [BindProperty(SupportsGet = true)]
        public int SearchRadiusInt { get; set; } = 10;
        [BindProperty(SupportsGet = true)]
        public int SearchDurationInt { get; set; } = 10;
        public async Task OnGetAsync()
        {
            var locPlayer = new List<Player>();
            var locVillage = new List<Village>();

            TimeSpan start = new TimeSpan(3, 59, 0);
            TimeSpan end = new TimeSpan(4, 15, 0);
            TimeSpan now = DateTime.Now.TimeOfDay;

            if (!((now > start) && (now < end))) {
                if (!string.IsNullOrEmpty(SearchString))
                 {
                    string sql = "SELECT id, name, ally, villages, points, rank, lastActive FROM player WHERE name = '" + SearchString + "'";
                    var resultPlayer = _context.Player.FromSqlRaw(sql).ToList();

                    string insertCmd = "INSERT INTO player (id, name, ally, villages, points, rank, lastActive) VALUES(1,'Test',1,1,1,1, date('now'))";
                    var test = _context.Player.FromSqlRaw(insertCmd);


                    if (resultPlayer.Count == 1)
                    {
                        sql = "SELECT id, name, x, y, player, points FROM village WHERE player = " + resultPlayer[0].id;
                        var resultVillage = _context.Village.FromSqlRaw(sql).ToList();

                        foreach (var village in resultVillage)
                        {
                            sql = @"SELECT DISTINCT player.id, player.name, player.ally, player.villages, player.points, player.rank, player.lastActive FROM player LEFT JOIN village on player.id = village.player WHERE lastActive <= datetime('now', '-" + SearchDurationInt + @" day') 
                                            AND(village.x - " + village.x + ") * (village.x - " + village.x + ") + (village.y - " + village.y + ") * (village.y - " + village.y + ") <= " + SearchRadiusInt * SearchRadiusInt;
                            locPlayer.AddRange(_context.Player.FromSqlRaw(sql).ToList());

                            sql = @"SELECT DISTINCT village.id, village.name, village.x, village.y, village.player, village.points FROM village WHERE village.player = 0
                                            AND(village.x - " + village.x + ") * (village.x - " + village.x + ") + (village.y - " + village.y + ") * (village.y - " + village.y + ") <= " + SearchRadiusInt * SearchRadiusInt;
                            locVillage.AddRange(_context.Village.FromSqlRaw(sql).ToList());
                        }
                        locPlayer = locPlayer.Distinct().ToList();
                        locVillage = locVillage.Distinct().ToList();
                    }
                    else
                    {
                        var players = from p in _context.Player
                                      select p;
                        if (!string.IsNullOrEmpty(SearchString))
                        {
                            players = players.Where(s => s.name.Contains(SearchString));
                        }
                        locPlayer = await players.ToListAsync();
                    }
                }
            }
            else
            {
                locPlayer.Add(new Player
                {
                    name = "Service not avaiable, during database update. Please wait 15 Minutes."
                });
            }

            Player = locPlayer;
            Village = locVillage;
        }
    }
}
