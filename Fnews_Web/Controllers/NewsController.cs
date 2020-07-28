using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Fnews_Web.Models;
using Fnews_Web.ViewModels;
using System.Security.Cryptography.X509Certificates;

namespace Fnews_Web.Controllers
{
    public class NewsController : Controller
    {
        private readonly FnewsContext _context;

        public NewsController(FnewsContext context)
        {
            _context = context;
        }

        // GET: News
        public async Task<IActionResult> Index(int? id)
        {
            var fnewsContext = _context.News.Where(n => n.IsActive == true).Include(x => x.NewsTag).ThenInclude(y => y.Tag)
                .Include(x => x.Channel).Where(x => x.ChannelId == id);
            return View(await fnewsContext.ToListAsync());
        }

        public async Task<IActionResult> Indexx(int? id)
        {
            var fnewsContext = _context.News.Include(n => n.Channel).Where(x => x.ChannelId == id);
            return View(await fnewsContext.ToListAsync());
        }

        // GET: News/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _context.News
                .Include(n => n.Channel)
                .Include(x => x.NewsTag).ThenInclude(x => x.Tag)
                .Include(x => x.Comment).ThenInclude(x => x.User)
                .FirstOrDefaultAsync(m => m.NewsId == id);
            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        // GET: News/Create
        public IActionResult Create()
        {
            ViewData["ChannelId"] = new SelectList(_context.Channel, "ChannelId", "ChannelId");
            //NewsTagData((News)_context.News.Include(x => x.NewsTag).ThenInclude(x => x.Tag));
            CreateNewsTagData();
            return View();
        }

        // POST: News/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NewsId,NewsTitle,NewsContent,DayOfPost,ChannelId,IsActive,LinkImage")] News news, string[] selected)
        {
            if (ModelState.IsValid)
            {
                _context.Add(news);
                await _context.SaveChangesAsync();
                var id = news.NewsId;

                var newsToUpdate = await _context.News
                    .Include(i => i.Channel)
                    .Include(i => i.NewsTag)
                        .ThenInclude(i => i.Tag)
                    .FirstOrDefaultAsync(m => m.NewsId == id);
                UpdateNewsTags(selected, newsToUpdate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ChannelId"] = new SelectList(_context.Channel, "ChannelId", "ChannelId", news.ChannelId);
            return View(news);
        }

        // GET: News/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _context.News
                .Include(x => x.NewsTag).ThenInclude(x => x.Tag)
                .Include(x => x.Comment).ThenInclude(x => x.User)
                .AsNoTracking().FirstOrDefaultAsync(x => x.NewsId == id);
            if (news == null)
            {
                return NotFound();
            }
            ViewData["ChannelId"] = new SelectList(_context.Channel, "ChannelId", "ChannelId", news.ChannelId);
            //ViewData["ChannelName"] = new SelectList(news.Channel.ChannelName, "ChannelName", "ChannelName", news.Channel.ChannelName);
            NewsTagData(news);
            return View(news);
        }

        // POST: News/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NewsId,NewsTitle,NewsContent,DayOfPost,ChannelId,IsActive,LinkImage")] News news, string[] selected)
        {
            if (id != news.NewsId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(news);


                    var newsToUpdate = await _context.News
                        .Include(i => i.Channel)
                        .Include(i => i.NewsTag)
                            .ThenInclude(i => i.Tag)
                        .FirstOrDefaultAsync(m => m.NewsId == id);
                    UpdateNewsTags(selected, newsToUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NewsExists(news.NewsId))
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
            ViewData["ChannelId"] = new SelectList(_context.Channel, "ChannelId", "ChannelId", news.ChannelId);
            return View(news);
        }

        // GET: News/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _context.News
                .Include(n => n.Channel)
                .FirstOrDefaultAsync(m => m.NewsId == id);
            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        // POST: News/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var news = await _context.News.FindAsync(id);
            news.IsActive = false;
            _context.News.Update(news);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NewsExists(int id)
        {
            return _context.News.Any(e => e.NewsId == id);
        }

        private void NewsTagData(News news)
        {
            var allTags = _context.Tag;
            var newsTagHS = new HashSet<int>(news.NewsTag.Select(c => c.TagId));
            var viewModel = new List<NewsTagView>();
            foreach (var tag in allTags)
            {
                viewModel.Add(new NewsTagView
                {
                    TagID = tag.TagId,
                    TagName = tag.TagName,
                    Checked = newsTagHS.Contains(tag.TagId)
                });
            }
            ViewData["Tags"] = viewModel;
        }

        private void CreateNewsTagData()
        {
            var allTags = _context.Tag;
            var newsTagHS = new HashSet<int>(_context.NewsTag.Select(c => c.TagId));
            var viewModel = new List<NewsTagView>();
            foreach (var tag in allTags)
            {
                viewModel.Add(new NewsTagView
                {
                    TagID = tag.TagId,
                    TagName = tag.TagName,
                    Checked = newsTagHS.Contains(tag.TagId)
                });
            }
            ViewData["Tags"] = viewModel;
        }

        //private void CommentData(News news)
        //{
        //    var allTags = _context.Tag;
        //    var instructorCourses = new HashSet<int>(news.Comment.Select(c => c.UserId));
        //    var viewModel = new List<NewsTagView>();
        //    foreach (var tag in allTags)
        //    {
        //        viewModel.Add(new NewsTagView
        //        {
        //            TagID = tag.TagId,
        //            TagName = tag.TagName,
        //            Checked = instructorCourses.Contains(tag.TagId)
        //        });
        //    }
        //    ViewData["Tags"] = viewModel;
        //}

        //private void ChannelsDropDownList(object selected = null)
        //{
        //    var query = from d in _context.Channel
        //                           select d;
        //    ViewBag.Channel = new SelectList(query.AsNoTracking(), "ChannelId", "ChannelName", selected);
        //}

        private void UpdateNewsTags(string[] selected, News newsToUpdate)
        {
            if (selected == null)
            {
                newsToUpdate.NewsTag = new List<NewsTag>();
                return;
            }

            var selectedHS = new HashSet<string>(selected);
            var newsTag = new HashSet<int>
                (newsToUpdate.NewsTag.Select(c => c.Tag.TagId));
            foreach (var tag in _context.Tag)
            {
                if (selectedHS.Contains(tag.TagId.ToString()))
                {
                    if (!newsTag.Contains(tag.TagId))
                    {
                        newsToUpdate.NewsTag.Add(new NewsTag { NewsId = newsToUpdate.NewsId, TagId = tag.TagId });
                    }
                }
                else
                {

                    if (newsTag.Contains(tag.TagId))
                    {
                        NewsTag tagToRemove = newsToUpdate.NewsTag.FirstOrDefault(i => i.TagId == tag.TagId);
                        _context.Remove(tagToRemove);
                    }
                }
            }
        }
    }
}
