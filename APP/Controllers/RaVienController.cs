using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APP.Data;
using APP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace APP.Controllers
{
    public class RaVienController : Controller
    {
        private readonly AppDbContext _context;

        public RaVienController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string search)
        {
            var query = _context.RaVien.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(r => r.makcb.Contains(search));
            }

            var list = await query.OrderByDescending(r => r.ngay).Take(100).ToListAsync();
            ViewBag.Search = search;
            return View(list);
        }

        [HttpPost]
        public async Task<IActionResult> RaVienAction(string makcb)
        {
            var record = await _context.RaVien.FirstOrDefaultAsync(r => r.makcb == makcb);
            if (record != null)
            {
                record.soravien = "Đã ra viện";
                await _context.SaveChangesAsync();
                TempData["Success"] = $"✅ Đã ra viện cho mã {makcb}";
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> HuyRaVien(string makcb)
        {
            var record = await _context.RaVien.FirstOrDefaultAsync(r => r.makcb == makcb);
            if (record != null)
            {
                record.soravien = null;
                await _context.SaveChangesAsync();
                TempData["Warning"] = $"❌ Đã hủy ra viện cho mã {makcb}";
            }
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Edit(string makcb)
        {
            var record = await _context.RaVien.FirstOrDefaultAsync(r => r.makcb == makcb);
            if (record == null)
                return NotFound();
            return View(record);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RaVien model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _context.Update(model);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"📝 Cập nhật thông tin ra viện cho {model.makcb} thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}

