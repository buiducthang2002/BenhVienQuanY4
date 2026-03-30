using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APP.Data;
using APP.Models;

namespace APP.Controllers
{
    public class BanLamViecKhamBenhController : Controller
    {
        private readonly AppDbContext _context;

        public BanLamViecKhamBenhController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Trang tìm kiếm
        public async Task<IActionResult> Index(string makcb)
        {
            if (string.IsNullOrEmpty(makcb))
                return View(new List<BanLamViecKhamBenh>());

            var list = await _context.BanLamViecKhamBenhs
                .Where(x => x.makcb == makcb)
                .ToListAsync();

            ViewBag.makcb = makcb;
            return View(list);
        }

        // GET: Form sửa
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            var item = await _context.BanLamViecKhamBenhs.FindAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        // POST: Lưu sửa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BanLamViecKhamBenh model)
        {
            if (ModelState.IsValid)
            {
                _context.Entry(model).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { makcb = model.makcb });
            }
            return View(model);
        }
    }
}
