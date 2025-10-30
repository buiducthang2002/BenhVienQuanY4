using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APP.Data;
using APP.Models;
using System.Threading.Tasks;
using System.Linq;

namespace APP.Controllers
{
    public class PhauThuatThuThuatController : Controller
    {
        private readonly AppDbContext _context;

        public PhauThuatThuThuatController(AppDbContext context)
        {
            _context = context;
        }

  
        [HttpPost]
        public async Task<IActionResult> DeleteSelected(long[] selectedIds)
        {
            if (selectedIds != null && selectedIds.Any())
            {
                var items = await _context.PhauThuatThuThuat
                    .Where(p => selectedIds.Contains(p.maphauthuat))
                    .ToListAsync();

                if (items.Any())
                {
                    _context.PhauThuatThuThuat.RemoveRange(items);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = $"{items.Count} bản ghi đã được xóa.";
                }
                else
                {
                    TempData["Error"] = "Không tìm thấy bản ghi nào để xóa.";
                }
            }
            else
            {
                TempData["Error"] = "Chưa chọn bản ghi nào.";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search)
        {
            var query = from pt in _context.PhauThuatThuThuat
                        join dk in _context.DangKy on pt.makcb equals dk.makcb
                        select new PhauThuatThuThuat
                        {
                            makcb = pt.makcb,
                            maphauthuat = pt.maphauthuat,
                            ngaybatdaumo = pt.ngaybatdaumo,
                            ngayketthucmo = pt.ngayketthucmo,
                            chandoantruocmo = pt.chandoantruocmo,
                            chandoansaumo = pt.chandoansaumo,
                            dienbien =pt.dienbien,
                            ailam = pt.ailam,
                            bac = pt.bac,
                            daky = pt.daky,
                            hoten = dk.hoten 
                        };

      
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p =>
                    (p.makcb != null && p.makcb.Contains(search)) ||
                    (p.hoten != null && p.hoten.Contains(search))
                );
            }

            var list = await query
                .OrderByDescending(p => p.ngaybatdaumo)
                .Take(2000)
                .ToListAsync();

            ViewBag.Search = search;
            return View("Index", list);
        }

        public IActionResult Default()
        {
            return RedirectToAction("Index");
        }
      




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PhauThuatThuThuat model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // ✅ Tìm theo cả 2 khóa chính
                    var originalItem = await _context.PhauThuatThuThuat
                        .FirstOrDefaultAsync(p => p.makcb == model.makcb && p.maphauthuat == model.maphauthuat);

                    if (originalItem == null)
                    {
                        TempData["Error"] = "Bản ghi không tồn tại.";
                        return RedirectToAction("Index");
                    }

                    // ✅ Cập nhật các trường cần thiết
                    originalItem.ngaybatdaumo = model.ngaybatdaumo;
                    originalItem.ngayketthucmo = model.ngayketthucmo;
                    originalItem.chandoantruocmo = model.chandoantruocmo;
                    originalItem.chandoansaumo = model.chandoansaumo;
                    originalItem.dienbien = model.dienbien;
                    originalItem.ailam = model.ailam;
                    originalItem.bac = model.bac;
                    originalItem.daky = model.daky;

                    _context.PhauThuatThuThuat.Update(originalItem);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Cập nhật phẫu thuật/thủ thuật thành công!";
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    TempData["Error"] = "Lỗi đồng thời khi cập nhật dữ liệu.";
                    return View(model);
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Lỗi khi lưu dữ liệu: {ex.Message}";
                    return View(model);
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string makcb, long maphauthuat)
        {
            if (string.IsNullOrEmpty(makcb))
            {
                return BadRequest("Thiếu mã KCB");
            }

            var item = await _context.PhauThuatThuThuat
                .FirstOrDefaultAsync(p => p.makcb == makcb && p.maphauthuat == maphauthuat);

            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

    }
}