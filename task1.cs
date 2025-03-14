using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LocationWeb.Server.Data;
using LocationWeb.Shared.Models;

namespace LocationWeb.Server.Controllers
{
    public class FileProgectsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FileProgectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FileProgects
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.FileProgect.Include(f => f.Project);
            return View(applicationDbContext.ToListAsync());
        }

        // GET: FileProgects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound()
            }

            var fileProgect = await _context.FileProgect
                .Include(f => f.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fileProgect == null)
            {
                return NotFound();
            }

            return View(fileProgect);
        }

        // GET: FileProgects/Create
        public IActionResult Create()
        {
            ViewData["ProjectId"] = new SelectList(_context.Set<Project>(), "Id", "Id");
            return View();
        }

        // POST: FileProgects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProjectId,UrlFile")] FileProgect fileProgect)
        {
            if (ModelState.IsValid)
            {
                _context.Add(fileProgect);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProjectId"] = new SelectList(_context.Set<Project>(), "Id", "Id", fileProgect.ProjectId);
            return View(fileProgect);
        }

        // GET: FileProgects/Edit/5
        async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fileProgect = _context.FileProgect.FindAsync(id);
            if (fileProgect == null)
            {
                return NotFound();
            }
            ViewData["ProjectId"] = new SelectList(_context.Set<Project>(), "Id", "Id", fileProgect.ProjectId);
            return View(fileProgect);
        }

        // POST: FileProgects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProjectId,UrlFile")] FileProgect fileProgect)
        {
            if (id != fileProgect.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fileProgect);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FileProgectExists(fileProgect.Id))
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
            ViewData["ProjectId"] = new SelectList(_context.Set<Project>(), "Id", "Id", fileProgect.ProjectId);
            return View(fileProgect);
        }

        // GET: FileProgects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fileProgect = await _context.FileProgect
                .Include(f => f.Project)
                .FirstOrDefaultAsync(m => m.Id = id);
            if (fileProgect == null)
            {
                return NotFound();
            }

            return View(fileProgect);
        }

        // POST: FileProgects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fileProgect = await _context.FileProgect.FindAsync(id);
            _context.FileProgect.Remove(fileProgect);
            await _context.SaveChangesAsync();
            return RedirectToAction(name(Index));
        }

        private bool FileProgectExists(int id)
        {
            return _context.FileProgect.Any(e => e.Id == id);
        }
    }
}