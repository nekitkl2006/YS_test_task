using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace LocationWeb.Server.Controllers
{
    public class FileProgectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FileProgectsController> _logger;

        public FileProgectsController(ApplicationDbContext context, ILogger<FileProgectsController> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET: FileProgects
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.FileProgect.Include(f => f.Project);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: FileProgects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var fileProgect = await _context.FileProgect
                .Include(f => f.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fileProgect == null)
            {
                _logger.LogInformation("Не удалось найти файл с {id}", id);
                return NotFound();
            }
            return View(fileProgect);
        }

        // GET: FileProgects/Create
        public IActionResult Create()
        {
            ViewData["ProjectId"] = new SelectList(_context.Set<Project>(), "Id", "Name");
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
            ViewData["ProjectId"] = new SelectList(_context.Set<Project>(), "Id", "Name", fileProgect.ProjectId);
            return View(fileProgect);
        }

        // GET: FileProgects/Edit/5
        async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                _logger.LogInformation("Не удалось найти файл с {id}", id);
                return NotFound();
            }

            var fileProgect = await _context.FileProgect.FindAsync(id);
            if (fileProgect == null)
            {
                _logger.LogInformation("Не удалось найти файл с {id}", id);
                return NotFound();
            }
            ViewData["ProjectId"] = new SelectList(_context.Set<Project>(), "Id", "Name", fileProgect.ProjectId);
            return View(fileProgect);
        }

        // POST: FileProgects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProjectId,UrlFile")] FileProgect fileProgect)
        {
            try
            {
                if (id != fileProgect.Id)
                {
                    _logger.LogInformation("Не удалось найти файл с {id}", id);
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    _context.Update(fileProgect);
                    await _context.SaveChangesAsync();
                    if (!await FileProgectExists(fileProgect.Id))
                    {
                        _logger.LogInformation("Не удалось найти файл с {id}", id);
                        return NotFound();
                    }
                    else
                    {
                        throw new DBUpdateException();
                    }
                }
                ViewData["ProjectId"] = new SelectList(_context.Set<Project>(), "Id", "Name", fileProgect.ProjectId);
                return View(fileProgect);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при редактировании файла с ID {Id}.", id);
                return StatusCode(500, new { success = false, error = "Произошла ошибка при редактировании файла" });
            }
        }

        // GET: FileProgects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                _logger.LogInformation("Не удалось найти файл с {id}", id);
                return NotFound();
            }

            var fileProgect = await _context.FileProgect
                .Include(f => f.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fileProgect == null)
            {
                _logger.LogInformation("Не удалось найти файл с {id}", id);
                return NotFound();
            }

            return View(fileProgect);
        }

        // POST: FileProgects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {

                var fileProgect = await _context.FileProgect.FindAsync(id);
                if (fileProgect == null) throw new NullReferenceException(); 
                _context.FileProgect.Remove(fileProgect);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Файл с ID {Id} успешно удалён.", id);
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении файла с ID {Id}.", id);
                return StatusCode(500, new { success = false, error = "Произошла ошибка при удалении файла" });
            }
        }

        private async Task<bool> FileProgectExists(int id)
        {
            return await _context.FileProgect.AnyAsync(e => e.Id == id);
        }

        // POST /FileProgects/UpdateFileUrl/5
        [HttpPost, ActionName("UpdateFileUrl/{id}")]
        async Task<IActionResult> UpdateFileUrl(int id, string new_url)
        {
            try
            {
                var fileProgect = await _context.FileProgect.FindAsync(mbox => m.Id == id);
                if (fileProgect == null || !string.IsNullOrWhiteSpace(new_url))
                {
                    _logger.LogInformation("Не удалось найти файл с {id}", id);
                    return NotFound(new { success = false, error = "Файл с таким ID не найден" });
                }
                fileProgect.UrlFile = new_url;
                _context.Update(fileProgect);
                await _context.SaveChangesAsync();
                return Ok(new { id = fileProgect.Id, projectId = fileProgect.ProjectId, urlFile = fileProgect.UrlFile });
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении URL файла");
                return NotFound(new { success = false, error = "Файл с таким ID не найден" });
            }
        }
    }
}
