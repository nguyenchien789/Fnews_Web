using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Fnews_Web.Models;

namespace Fnews_Web.Controllers
{
    public class ChannelsController : Controller
    {
        private readonly FnewsContext _context;

        public ChannelsController(FnewsContext context)
        {
            _context = context;
        }

        // GET: Channels
        public async Task<IActionResult> Index()
        {
            var fnewsContext = _context.Channel.Include(c => c.Group);
            return View(await fnewsContext.ToListAsync());
        }

        // GET: Channels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var channel = await _context.Channel
                .Include(c => c.Group)
                .FirstOrDefaultAsync(m => m.ChannelId == id);
            if (channel == null)
            {
                return NotFound();
            }

            return View(channel);
        }

        // GET: Channels/Create
        public IActionResult Create()
        {
            ViewData["GroupId"] = new SelectList(_context.Group, "GroupId", "GroupId");
            return View();
        }

        // POST: Channels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ChannelId,ChannelName,GroupId,IsActive")] Channel channel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(channel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GroupId"] = new SelectList(_context.Group, "GroupId", "GroupId", channel.GroupId);
            return View(channel);
        }

        // GET: Channels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var channel = await _context.Channel.FindAsync(id);
            if (channel == null)
            {
                return NotFound();
            }
            ViewData["GroupId"] = new SelectList(_context.Group, "GroupId", "GroupId", channel.GroupId);
            return View(channel);
        }

        // POST: Channels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ChannelId,ChannelName,GroupId,IsActive")] Channel channel)
        {
            if (id != channel.ChannelId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(channel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChannelExists(channel.ChannelId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["GroupId"] = new SelectList(_context.Group, "GroupId", "GroupId", channel.GroupId);
            return View(channel);
        }

        // GET: Channels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var channel = await _context.Channel
                .Include(c => c.Group)
                .FirstOrDefaultAsync(m => m.ChannelId == id);
            if (channel == null)
            {
                return NotFound();
            }

            return View(channel);
        }

        // POST: Channels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var channel = await _context.Channel.FindAsync(id);
            _context.Channel.Remove(channel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChannelExists(int id)
        {
            return _context.Channel.Any(e => e.ChannelId == id);
        }
    }
}
