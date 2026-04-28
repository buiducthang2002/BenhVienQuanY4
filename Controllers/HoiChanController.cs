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
        public async Task<IActionResult> Edit(string makcb)
        {
            var item = await _context.HoiChan.FindAsync(makcb);
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
        public async Task<IActionResult> Delete(string makcb)
        {
            var item = await _context.HoiChan.FindAsync(makcb);
            if (item == null) return NotFound();
            _context.HoiChan.Remove(item);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new { makcb });
        }

        // POST: Huỷ ký
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HuyKy(string makcb)
        {
            var item = await _context.HoiChan.FindAsync(makcb);
            if (item == null) return NotFound();
            item.chutoa = null;
            item.thuky = null;
            item.daky = null;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new { makcb = item.makcb });
        }
    }
}