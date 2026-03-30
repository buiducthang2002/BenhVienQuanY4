using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APP.Data;
using APP.Models;

namespace APP.Controllers
{
    public class HoiChanController : Controller
    {
        private readonly AppDbContext _context;

        public HoiChanController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Trang tìm kiếm
        public async Task<IActionResult> Index(string makcb)
        {
            if (string.IsNullOrEmpty(makcb))
                return View(new List<HoiChan>());

            var list = await _context.HoiChan
                .Where(x => x.makcb == makcb)
                .ToListAsync();

            ViewBag.makcb = makcb;
            return View(list);
        }

        // GET: Form sửa
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            var item = await _context.HoiChan.FindAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        // POST: Lưu sửa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(HoiChan model)
        {
            if (ModelState.IsValid)
            {
                _context.Entry(model).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { makcb = model.makcb });
            }
            return View(model);
        }

        // POST: Xóa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            var item = await _context.HoiChan.FindAsync(id);
            if (item == null) return NotFound();
            var makcb = item.makcb;
            _context.HoiChan.Remove(item);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new { makcb });
        }
    }
}
