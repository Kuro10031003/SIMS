using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIMS.Data;

namespace SIMS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly AppDbContext _db;
        public UsersController(AppDbContext db) { _db = db; }

        // 列出所有用户
        public IActionResult Index()
        {
            var users = _db.Users.ToList();
            return View(users);
        }

        // 切换激活/禁用状态
        public IActionResult ToggleActive(int id)
        {
            var user = _db.Users.Find(id);
            if (user != null)
            {
                user.IsActive = !user.IsActive;   // 反转：激活变禁用，禁用变激活
                _db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}