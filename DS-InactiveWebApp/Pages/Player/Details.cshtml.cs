﻿using System;
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
    public class DetailsModel : PageModel
    {
        private readonly DS_InactiveWebApp.Data.DS_InactiveWebAppContext _context;

        public DetailsModel(DS_InactiveWebApp.Data.DS_InactiveWebAppContext context)
        {
            _context = context;
        }

        public Player Player { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Player = await _context.Player.FirstOrDefaultAsync(m => m.id == id);

            if (Player == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
