using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using APP.Data;
using APP.Models;
using Microsoft.EntityFrameworkCore;

namespace APP.Controllers
{
    public class GtktPhieuController : Controller
    {
        private readonly AppDbContext _context;

        public GtktPhieuController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 50)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 10) pageSize = 10;
                if (pageSize > 100) pageSize = 100;

                var query = _context.GtktPhieu.AsNoTracking();

                var totalRecords = await query.CountAsync();
                var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

                var list = await query
                    .OrderByDescending(g => g.ngay)
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

        [HttpPost]
        public async Task<IActionResult> Search(string makcb)
        {
            if (string.IsNullOrWhiteSpace(makcb))
            {
                ViewBag.Message = "Vui lòng nhập Mã KCB!";
                ViewBag.TotalRecords = 0;
                return View("Index", new List<GtktPhieu>());
            }

            var result = await _context.GtktPhieu
                .AsNoTracking()
                .Where(g => g.makcb == makcb || g.makcb.EndsWith(makcb))
                .OrderByDescending(g => g.ngay)
                .ToListAsync();

            if (result.Count == 0)
            {
                ViewBag.Message = "Không tìm thấy phiếu nào với Mã KCB này!";
                ViewBag.TotalRecords = 0;
                return View("Index", new List<GtktPhieu>());
            }

            ViewBag.TotalRecords = result.Count;
            ViewBag.CurrentPage = 1;
            ViewBag.TotalPages = 1;
            ViewBag.PageSize = result.Count;
            return View("Index", result);
        }

        [HttpPost]
        public async Task<IActionResult> CancelSign(string sophieu, string makcb, string mauphieu)
        {
            try
            {
                if (string.IsNullOrEmpty(sophieu) || string.IsNullOrEmpty(makcb) || string.IsNullOrEmpty(mauphieu))
                {
                    TempData["Error"] = "Thiếu thông tin để hủy ký!";
                    return RedirectToAction("Index");
                }

                var record = await _context.GtktPhieu.FirstOrDefaultAsync(g =>
                    g.sophieu == sophieu && g.makcb == makcb && g.mauphieu == mauphieu);

                if (record == null)
                {
                    TempData["Error"] = $"Không tìm thấy phiếu {sophieu}!";
                    return RedirectToAction("Index");
                }

                record.daky = null;
                await _context.SaveChangesAsync();
                TempData["Success"] = $"✅ Đã hủy ký phiếu {sophieu}!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi hủy ký: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> CancelSignMulti(string[] keys)
        {
            try
            {
                if (keys == null || keys.Length == 0)
                {
                    TempData["Error"] = "Vui lòng chọn ít nhất một phiếu để hủy ký!";
                    return RedirectToAction("Index");
                }

                int successCount = 0;
                foreach (var key in keys)
                {
                    var parts = key.Split('|');
                    if (parts.Length < 3) continue;

                    var sophieu = parts[0];
                    var makcb = parts[1];
                    var mauphieu = parts[2];

                    var record = await _context.GtktPhieu.FirstOrDefaultAsync(g =>
                        g.sophieu == sophieu && g.makcb == makcb && g.mauphieu == mauphieu);

                    if (record != null)
                    {
                        record.daky = null;
                        successCount++;
                    }
                }

                if (successCount > 0)
                {
                    await _context.SaveChangesAsync();
                    TempData["Success"] = $"✅ Đã hủy ký thành công {successCount} phiếu!";
                }
                else
                {
                    TempData["Error"] = "❌ Không tìm thấy phiếu nào để hủy ký!";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi hủy ký: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string sophieu, string makcb, string mauphieu)
        {
            try
            {
                if (string.IsNullOrEmpty(sophieu) || string.IsNullOrEmpty(makcb) || string.IsNullOrEmpty(mauphieu))
                {
                    TempData["Error"] = "Thiếu thông tin để xóa phiếu!";
                    return RedirectToAction("Index");
                }

                var record = await _context.GtktPhieu.FirstOrDefaultAsync(g =>
                    g.sophieu == sophieu && g.makcb == makcb && g.mauphieu == mauphieu);

                if (record == null)
                {
                    TempData["Error"] = $"Không tìm thấy phiếu {sophieu}!";
                    return RedirectToAction("Index");
                }

                _context.GtktPhieu.Remove(record);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"✅ Đã xóa phiếu {sophieu} (mẫu: {mauphieu})!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi xóa phiếu: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateNgay(string sophieu, string makcb, string mauphieu, DateTime ngay)
        {
            try
            {
                var record = await _context.GtktPhieu.FirstOrDefaultAsync(g =>
                    g.sophieu == sophieu && g.makcb == makcb && g.mauphieu == mauphieu);

                if (record == null)
                {
                    TempData["Error"] = $"Không tìm thấy phiếu {sophieu}!";
                    return RedirectToAction("Index");
                }

                record.ngay = ngay;
                await _context.SaveChangesAsync();
                TempData["Success"] = $"✅ Đã cập nhật ngày phiếu {sophieu}!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi cập nhật: {ex.Message}";
            }

            return RedirectToAction("Index");
        }
    }
}
