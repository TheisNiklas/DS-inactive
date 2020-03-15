using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DS_InactiveWebApp.Data;
using DS_InactiveWebApp.Models;

namespace DS_InactiveWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerRestController : ControllerBase
    {
        private readonly DS_InactiveWebAppContext _context;

        public PlayerRestController(DS_InactiveWebAppContext context)
        {
            _context = context;
        }


        [Produces("application/json")]
        [HttpGet("search")]
        public async Task<IActionResult> Search()
        {
            try
            {
                string term = HttpContext.Request.Query["term"].ToString();
                var names = _context.Player.Where(p => p.name.ToLower().Contains(term.ToLower())).Select(p => p.name).ToList();
                return Ok(names);
            }
            catch
            {
                return BadRequest();
            }
        }


        // GET: api/PlayerRest
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Player>>> GetPlayer()
        {
            return await _context.Player.ToListAsync();
        }

        // GET: api/PlayerRest/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Player>> GetPlayer(long id)
        {
            var player = await _context.Player.FindAsync(id);

            if (player == null)
            {
                return NotFound();
            }

            return player;
        }

        // POST: api/PlayerRest
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Player>> PostPlayer(Player player)
        {
            _context.Player.Add(player);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPlayer", new { id = player.id }, player);
        }

        private bool PlayerExists(long id)
        {
            return _context.Player.Any(e => e.id == id);
        }
    }
}
