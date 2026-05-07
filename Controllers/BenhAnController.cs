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
            if (string.IsNullOrWhiteSpace(makcb))
                return RedirectToAction("Index");

            var results = await _context.BenhAn
                .AsNoTracking()
                .Where(b => b.makcb == makcb || (b.makcb != null && b.makcb.EndsWith(makcb)))
                .ToListAsync();

            ViewBag.CurrentPage = 1;
            ViewBag.TotalPages = 1;
            ViewBag.PageSize = results.Count == 0 ? 1 : results.Count;

            if (results.Count == 0)
            {
                ViewBag.Message = "Không tìm thấy bệnh án với Mã KCB này.";
                ViewBag.TotalRecords = 0;
                return View("Index", new List<BenhAn>());
            }

            ViewBag.TotalRecords = results.Count;
            return View("Index", results);
        }



        [HttpPost]
        public async Task<IActionResult> CancelSign(string makcb, string? maubenhan)
        {
            try
            {
                if (string.IsNullOrEmpty(makcb))
                {
                    TempData["Error"] = "Mã KCB không hợp lệ!";
                }
                else
                {
                    var sql = "UPDATE tbl_benhan_benhantheobn SET daky = NULL WHERE (makcb = {0} OR makcb LIKE {1}) AND maubenhan = {2}";
                    var affected = await _context.Database.ExecuteSqlRawAsync(sql, makcb, "%" + makcb, maubenhan ?? string.Empty);

                    if (affected > 0)
                        TempData["Success"] = $"Đã hủy toàn bộ chữ ký bệnh án {makcb} ({maubenhan})! ({affected} bản ghi)";
                    else
                        TempData["Error"] = $"Không tìm thấy bệnh án {makcb} ({maubenhan})!";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi hủy ký: {ex.Message}";
            }

            return RedirectToAction("Index", new { t = DateTime.Now.Ticks });
        }

        [HttpPost]
        public async Task<IActionResult> CancelSingleSign(string makcb, int signIndex, string? maubenhan)
        {
            try
            {
                if (string.IsNullOrEmpty(makcb))
                {
                    TempData["Error"] = "Mã KCB không hợp lệ!";
                    return RedirectToAction("Index");
                }

                var records = await _context.BenhAn
                    .AsNoTracking()
                    .Where(b => (b.makcb == makcb || (b.makcb != null && b.makcb.EndsWith(makcb)))
                                && b.maubenhan == maubenhan)
                    .ToListAsync();

                if (records.Count == 0)
                {
                    TempData["Error"] = $"Không tìm thấy bệnh án {makcb} ({maubenhan})!";
                    return RedirectToAction("Index");
                }

                int totalUpdated = 0;
                string? signUser = null;

                foreach (var record in records)
                {
                    if (string.IsNullOrEmpty(record.daky)) continue;

                    var oldDaky = record.daky;
                    var signatures = oldDaky.Trim()
                        .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => s.Trim())
                        .ToList();

                    if (signIndex < 0 || signIndex >= signatures.Count) continue;

                    var signParts = signatures[signIndex].Split('|');
                    if (signUser == null && signParts.Length > 1) signUser = signParts[1];

                    signatures.RemoveAt(signIndex);
                    string? newDaky = signatures.Count == 0 ? null : string.Join(";", signatures);

                    var sql = "UPDATE tbl_benhan_benhantheobn SET daky = {0} WHERE makcb = {1} AND maubenhan = {2} AND daky = {3}";
                    var updated = await _context.Database.ExecuteSqlRawAsync(
                        sql,
                        (object?)newDaky ?? DBNull.Value,
                        record.makcb ?? (object)DBNull.Value,
                        record.maubenhan ?? (object)DBNull.Value,
                        oldDaky);
                    totalUpdated += updated;
                }

                if (totalUpdated > 0)
                    TempData["Success"] = $"Đã hủy chữ ký #{signIndex + 1} ({signUser ?? "Unknown"}) của bệnh án {makcb} ({maubenhan})!";
                else
                    TempData["Error"] = "Chữ ký không tồn tại hoặc bệnh án chưa có chữ ký!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi hủy chữ ký: {ex.Message}";
            }

            return RedirectToAction("Index", new { t = DateTime.Now.Ticks });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string makcb)
        {
            try
            {
                if (string.IsNullOrEmpty(makcb))
                {
                    TempData["Error"] = "Mã KCB không hợp lệ!";
                }
                else
                {
                    var sql = "DELETE FROM tbl_benhan_benhantheobn WHERE makcb = {0} OR makcb LIKE {1}";
                    var affected = await _context.Database.ExecuteSqlRawAsync(sql, makcb, "%" + makcb);

                    if (affected > 0)
                        TempData["Success"] = $"Đã xóa bệnh án {makcb} thành công! ({affected} bản ghi)";
                    else
                        TempData["Error"] = $"Không tìm thấy bệnh án {makcb}!";
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