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
    public class KySoController : Controller
    {
        
        private readonly AppDbContext _context;

        public KySoController(AppDbContext context)
        {
            _context = context; 
        }



        [HttpPost]
        public async Task<IActionResult> Search(string makcb)
        {
            var result = await _context.KySo
                .AsNoTracking()
                .Where(b => b.makcb == makcb)
                .FirstOrDefaultAsync();

            if (result == null)
            {
                ViewBag.Message = "Không tìm thấy bệnh án với Mã KCB này !!!";
                ViewBag.TotalRecords = 0;
                return View("Index", new List<KySo>());
            }

            ViewBag.TotalRecords = 1;
            ViewBag.CurrentPage = 1;
            ViewBag.TotalPages = 1;
            ViewBag.PageSize = 1;
            return View("Index", new List<KySo> { result });
        }

 
        [HttpPost]
        public async Task<IActionResult> CancelSign(string[] makcb)
        {
            try
            {
                if (makcb == null || makcb.Length == 0)
                {
                    TempData["Error"] = "Vui lòng chọn ít nhất một bệnh án để hủy ký!";
                    return RedirectToAction("Index");
                }

                int successCount = 0;
                var updatedRecords = new List<string>();
                
                foreach (var id in makcb)
                {
                    var record = await _context.KySo.FirstOrDefaultAsync(b => b.makcb == id);
                    if (record != null)
                    {
                        // Debug: Log trước khi update
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
                    TempData["Debug"] = string.Join(", ", updatedRecords);
                }
                else
                {
                    TempData["Error"] = "❌ Không tìm thấy bệnh án nào để hủy ký!";
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
                var record = await _context.KySo.FirstOrDefaultAsync(k => k.makcb == makcb);
                if (record == null)
                {
                    TempData["Error"] = $"Không tìm thấy bệnh án {makcb}!";
                    return RedirectToAction("Index");
                }

                if (string.IsNullOrEmpty(record.daky))
                {
                    TempData["Error"] = "Bệnh án chưa có chữ ký nào!";
                    return RedirectToAction("Index");
                }

                // Parse danh sách chữ ký
                var dakyContent = record.daky.Trim('(', ')');
                var signatures = dakyContent.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                                             .Select(s => s.Trim())
                                             .ToList();

                if (signIndex < 0 || signIndex >= signatures.Count)
                {
                    TempData["Error"] = "Chữ ký không tồn tại!";
                    return RedirectToAction("Index");
                }

                // Lấy thông tin chữ ký sẽ xóa
                var signToRemove = signatures[signIndex];
                var signParts = signToRemove.Split('|');
                var signUser = signParts.Length > 1 ? signParts[1] : "Unknown";

                // Xóa chữ ký theo index
                signatures.RemoveAt(signIndex);

                // Cập nhật lại chuỗi daky
                if (signatures.Count == 0)
                {
                    record.daky = null;
                }
                else
                {
                    record.daky = "(" + string.Join(";", signatures) + ";)";
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = $"✅ Đã hủy chữ ký #{signIndex + 1} ({signUser}) của bệnh án {makcb}!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi hủy chữ ký: {ex.Message}";
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

                var query = _context.KySo
                    .AsNoTracking()
                    .Where(k => !string.IsNullOrEmpty(k.daky)); // Chỉ hiển thị những record đã ký

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


    }
}

