using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIMS.Data;
using SIMS.Models;

namespace SIMS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProgrammesController : Controller
    {
        private readonly AppDbContext _db;
        public ProgrammesController(AppDbContext db) { _db = db; }

        public IActionResult Index()
        {
            var programmes = _db.Programmes.ToList();
            return View(programmes);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Programme programme)
        {
            _db.Programmes.Add(programme);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var programme = _db.Programmes.Find(id);
            if (programme == null) return NotFound();
            return View(programme);
        }

        // EDIT - 处理修改提交（POST）
        [HttpPost]
        public IActionResult Edit(Programme programme)
        {
            _db.Programmes.Update(programme);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        // DELETE - 显示确认页（GET）
        public IActionResult Delete(int id)
        {
            var programme = _db.Programmes.Find(id);
            if (programme == null) return NotFound();
            return View(programme);
        }

        // DELETE - 确认删除（POST）
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var programme = _db.Programmes.Find(id);
            if (programme != null)
            {
                _db.Programmes.Remove(programme);
                _db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}