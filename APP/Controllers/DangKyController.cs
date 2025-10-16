using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APP.Data;
using APP.Models;
using System.Linq;
using System.Threading.Tasks;

namespace APP.Controllers
{
    public class DangKyController : Controller
    {
        private readonly AppDbContext _context;

        public DangKyController(AppDbContext context)
        {
            _context = context;
        }

        // 📝 GET: Hiển thị form sửa
        [HttpGet]
        public async Task<IActionResult> Edit(string makcb)
        {
            if (string.IsNullOrEmpty(makcb))
                return NotFound();

            var record = await _context.DangKy.FirstOrDefaultAsync(x => x.makcb == makcb);
            if (record == null)
                return NotFound();

            return View(record);
        }

        // 💾 POST: Lưu thay đổi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DangKy model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var existing = await _context.DangKy.FirstOrDefaultAsync(x => x.makcb == model.makcb);
            if (existing == null)
                return NotFound();

            // Cập nhật các trường cần thiết
            existing.hoten = model.hoten;
            existing.ngaydk = model.ngaydk;
            existing.ngaysinh = model.ngaysinh;
            existing.maphai = model.maphai;
            existing.maphong = model.maphong;
            existing.dienthoai = model.dienthoai;
            existing.sobhxh = model.sobhxh;
            

            await _context.SaveChangesAsync();
            TempData["Success"] = "Cập nhật thông tin thành công!";
            return RedirectToAction("Index");
        }

        // 📋 Danh sách đăng ký
        [HttpGet]
        public async Task<IActionResult> Index(string? search)
        {
            var query = _context.DangKy.AsQueryable();

            // 🔍 Tìm kiếm theo mã KCB hoặc họ tên
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p =>
                    (!string.IsNullOrEmpty(p.makcb) && p.makcb.Contains(search)) ||
                    (!string.IsNullOrEmpty(p.hoten) && p.hoten.Contains(search))
                );
            }

            var list = await query
                .OrderByDescending(p => p.ngaydk)
                .Take(500)
                .ToListAsync();

            ViewBag.Search = search;
            return View(list);
        }

        // 🗑️ Xóa các dòng đã chọn
        [HttpPost]
        public async Task<IActionResult> DeleteSelected(string[] selectedIds)
        {
            if (selectedIds != null && selectedIds.Any())
            {
                var items = await _context.DangKy
                    .Where(p => selectedIds.Contains(p.makcb))
                    .ToListAsync();

                _context.DangKy.RemoveRange(items);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"{items.Count} bản ghi đã được xóa.";
            }
            else
            {
                TempData["Error"] = "Không có bản ghi nào được chọn để xóa.";
            }

            return RedirectToAction("Index");
        }
    }
}