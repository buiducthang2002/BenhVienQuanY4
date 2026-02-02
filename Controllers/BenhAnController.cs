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
    public class BenhAnController : Controller
    {
        private readonly AppDbContext _context;

        public BenhAnController(AppDbContext context)
        {
            _context = context;
        }



        [HttpPost]
        public async Task<IActionResult> Search(string makcb)
        {
            var result = await _context.BenhAn
                .AsNoTracking()
                .Where(b => b.makcb == makcb)
                .FirstOrDefaultAsync();

            if (result == null)
            {
                ViewBag.Message = "Không tìm thấy bệnh án với Mã KCB này.";
                ViewBag.TotalRecords = 0;
                return View("Index", new List<BenhAn>());
            }

            ViewBag.TotalRecords = 1;
            ViewBag.CurrentPage = 1;
            ViewBag.TotalPages = 1;
            ViewBag.PageSize = 1;
            return View("Index", new List<BenhAn> { result });
        }


        [HttpPost]
        public async Task<IActionResult> CancelSign(string makcb)
        {
            try
            {
                var record = await _context.BenhAn.FirstOrDefaultAsync(b => b.makcb == makcb);
                if (record != null)
                {
                    record.daky = null;
                    await _context.SaveChangesAsync();
                    TempData["Success"] = $"Đã hủy ký bệnh án {makcb} thành công!";
                }
                else
                {
                    TempData["Error"] = $"Không tìm thấy bệnh án {makcb}!";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi hủy ký: {ex.Message}";
            }

            // Force refresh với cache busting
            return RedirectToAction("Index", new { t = DateTime.Now.Ticks });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string makcb)
        {
            try
            {
                var record = await _context.BenhAn.FirstOrDefaultAsync(b => b.makcb == makcb);
                if (record == null)
                {
                    TempData["Error"] = $"Không tìm thấy bệnh án {makcb}!";
                }
                else
                {
                    _context.BenhAn.Remove(record);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = $"Đã xóa bệnh án {makcb} thành công!";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi xóa bệnh án: {ex.Message}";
            }

            return RedirectToAction("Index", new { t = DateTime.Now.Ticks });
        }
        public async Task<IActionResult> Index(int page = 1, int pageSize = 50)
        {
            try
            {
                // Validate page parameters
                if (page < 1) page = 1;
                if (pageSize < 10) pageSize = 10;
                if (pageSize > 100) pageSize = 100;

                var totalRecords = await _context.BenhAn.CountAsync();
                var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

                var list = await _context.BenhAn
                    .AsNoTracking()
                    .OrderByDescending(b => b.makcb)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                ViewBag.CurrentPage = page;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalPages = totalPages;
                ViewBag.TotalRecords = totalRecords;

                return View(list);
            }
            catch (Exception ex)
            {
                return Content($"❌ Lỗi truy vấn dữ liệu: {ex.Message}");
            }
        }
    }
}

