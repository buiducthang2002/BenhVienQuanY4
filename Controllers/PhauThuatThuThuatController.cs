using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APP.Data;
using APP.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSelected(long[] selectedIds)
        {
            if (selectedIds != null && selectedIds.Any())
            {
                try
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
                catch (Exception ex)
                {
                    TempData["Error"] = $"Lỗi khi xóa dữ liệu: {ex.Message}";
                }
            }
            else
            {
                TempData["Error"] = "Chưa chọn bản ghi nào.";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search, int page = 1, int pageSize = 50)
        {
            // Validate page parameters
            if (page < 1) page = 1;
            if (pageSize < 10) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var query = from pt in _context.PhauThuatThuThuat.AsNoTracking()
                        join dk in _context.DangKy on pt.makcb equals dk.makcb
                        join k in _context.DmKhoa on pt.makk equals k.makk into khoaGroup
                        from khoa in khoaGroup.DefaultIfEmpty()
                        join p in _context.DmPhong on pt.maphong equals p.maphong into phongGroup
                        from phong in phongGroup.DefaultIfEmpty()
                        select new PhauThuatThuThuat
                        {
                            makcb = pt.makcb,
                            maphauthuat = pt.maphauthuat,
                            makk = pt.makk,
                            maphong = pt.maphong,
                            mathanhtoan = pt.mathanhtoan,
                            ngaybatdaumo = pt.ngaybatdaumo,
                            ngayketthucmo = pt.ngayketthucmo,
                            chandoantruocmo = pt.chandoantruocmo,
                            chandoansaumo = pt.chandoansaumo,
                            dienbien =pt.dienbien,
                            ailam = pt.ailam,
                            bac = pt.bac,
                            daky = pt.daky,
                            hoten = dk.hoten,
                            tenkk = khoa.tenkk,
                            tenphong = phong.tenphong
                        };

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p =>
                    (p.makcb != null && p.makcb.Contains(search)) ||
                    (p.hoten != null && p.hoten.Contains(search))
                );
            }

            var totalRecords = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            var list = await query
                .OrderByDescending(p => p.ngaybatdaumo)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Search = search;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalRecords = totalRecords;

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
                    originalItem.makk = model.makk;
                    originalItem.maphong = model.maphong;
                    originalItem.mathanhtoan = model.mathanhtoan;
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
                    await LoadDropdownListsAsync();
                    return View(model);
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Lỗi khi lưu dữ liệu: {ex.Message}";
                    await LoadDropdownListsAsync();
                    return View(model);
                }
            }

            await LoadDropdownListsAsync();
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
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.makcb == makcb && p.maphauthuat == maphauthuat);

            if (item == null)
            {
                return NotFound();
            }

            await LoadDropdownListsAsync();
            return View(item);
        }

        private async Task LoadDropdownListsAsync()
        {
            var khoaTask = _context.DmKhoa.AsNoTracking().ToListAsync();
            var phongTask = _context.DmPhong.AsNoTracking().ToListAsync();
            await Task.WhenAll(khoaTask, phongTask);

            ViewBag.KhoaList = new SelectList(khoaTask.Result, "makk", "tenkk");
            ViewBag.PhongList = new SelectList(phongTask.Result, "maphong", "tenphong");
        }

    }
}