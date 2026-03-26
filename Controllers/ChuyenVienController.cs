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
    public class ChuyenVienController : Controller
    {
        private readonly AppDbContext _context;

        public ChuyenVienController(AppDbContext context)
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

                var query = _context.ChuyenVien
                    .AsNoTracking()
                    .Where(k => !string.IsNullOrEmpty(k.daky));

                var totalRecords = await query.CountAsync();
                var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

                var list = await query
                    .OrderByDescending(k => k.makcb)
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
                return RedirectToAction("Index");

            var result = await _context.ChuyenVien
                .AsNoTracking()
                .Where(b => b.makcb == makcb || (b.makcb != null && b.makcb.EndsWith(makcb)))
                .FirstOrDefaultAsync();

            ViewBag.CurrentPage = 1;
            ViewBag.TotalPages = 1;
            ViewBag.PageSize = 1;

            if (result == null)
            {
                ViewBag.Message = "Không tìm thấy chuyển viện với Mã KCB này !!!";
                ViewBag.TotalRecords = 0;
                return View("Index", new List<ChuyenVien>());
            }

            ViewBag.TotalRecords = 1;
            return View("Index", new List<ChuyenVien> { result });
        }

        [HttpPost]
        public async Task<IActionResult> CancelSign(string[] makcb)
        {
            try
            {
                if (makcb == null || makcb.Length == 0)
                {
                    TempData["Error"] = "Vui lòng chọn ít nhất một chuyển viện để hủy ký!";
                    return RedirectToAction("Index");
                }

                int successCount = 0;
                var updatedRecords = new List<string>();

                foreach (var id in makcb)
                {
                    var record = string.IsNullOrEmpty(id)
                        ? null
                        : await _context.ChuyenVien.FirstOrDefaultAsync(b => b.makcb == id || (b.makcb != null && b.makcb.EndsWith(id)));
                    if (record != null)
                    {
                        var oldValue = record.daky;
                        record.daky = null!;
                        successCount++;
                        updatedRecords.Add($"{id} (từ '{oldValue}' → null)");
                    }
                }

                if (successCount > 0)
                {
                    await _context.SaveChangesAsync();
                    TempData["Success"] = $"✅ Đã hủy ký số thành công {successCount} !";
                }
                else
                {
                    TempData["Error"] = "❌ Không tìm thấy chuyển viện nào để hủy ký!";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi hủy ký: {ex.Message}";
            }

            return RedirectToAction("Index", new { t = DateTime.Now.Ticks });
        }

        [HttpPost]
        public async Task<IActionResult> CancelSingleSign(string makcb, int signIndex)
        {
            try
            {
                var record = string.IsNullOrEmpty(makcb)
                    ? null
                    : await _context.ChuyenVien.FirstOrDefaultAsync(k => k.makcb == makcb || (k.makcb != null && k.makcb.EndsWith(makcb)));
                if (record == null)
                {
                    TempData["Error"] = $"Không tìm thấy chuyển viện {makcb}!";
                    return RedirectToAction("Index");
                }

                if (string.IsNullOrEmpty(record.daky))
                {
                    TempData["Error"] = "Chuyển viện chưa có chữ ký nào!";
                    return RedirectToAction("Index");
                }

                var dakyContent = record.daky.Trim();
                var signatures = dakyContent.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                                             .Select(s => s.Trim())
                                             .ToList();

                if (signIndex < 0 || signIndex >= signatures.Count)
                {
                    TempData["Error"] = "Chữ ký không tồn tại!";
                    return RedirectToAction("Index");
                }

                var signToRemove = signatures[signIndex];
                var signParts = signToRemove.Split('|');
                var signUser = signParts.Length > 1 ? signParts[1] : "Unknown";

                signatures.RemoveAt(signIndex);

                record.daky = signatures.Count == 0 ? null : string.Join(";", signatures);

                await _context.SaveChangesAsync();
                TempData["Success"] = $"✅ Đã hủy chữ ký #{signIndex + 1} ({signUser}) của chuyển viện {makcb}!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi hủy chữ ký: {ex.Message}";
            }

            return RedirectToAction("Index", new { t = DateTime.Now.Ticks });
        }
    }
}
