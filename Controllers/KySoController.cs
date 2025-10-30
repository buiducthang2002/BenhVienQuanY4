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
                .Where(b => b.makcb == makcb)
                .FirstOrDefaultAsync();

            if (result == null)
            {
                ViewBag.Message = "Không tìm thấy bệnh án với Mã KCB này !!!";
                return View("Index");
            }

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
                    TempData["Success"] = $"✅ Đã hủy ký thành công {successCount} bệnh án!";
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
        public async Task<IActionResult> Index()
        {
            try
            {
                var list = await _context.KySo
                    .Where(k => !string.IsNullOrEmpty(k.daky)) // Chỉ hiển thị những record đã ký
                    .ToListAsync();
                return View(list);
            }
            catch (Exception ex)
            {
                return Content($"❌ Lỗi truy vấn dữ liệu: {ex.Message}");
            }
        }


    }
}

