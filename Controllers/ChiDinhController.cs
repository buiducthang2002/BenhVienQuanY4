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
    public class ChiDinhController : Controller
    {
        private readonly AppDbContext _context;
        private const int PageSize = 50; // Số bản ghi mỗi trang

        public ChiDinhController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Hiển thị dữ liệu từ bảng thanhtoan với phân trang và tìm kiếm
        /// </summary>
        public async Task<IActionResult> Index(int page = 1, string searchTerm = "", DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                page = page < 1 ? 1 : page;

                // Tạo query cơ bản
                var query = _context.ThanhToan.AsQueryable();

                // Tìm kiếm theo số phiếu hoặc mã KCB
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query = query.Where(x => x.sophieu.Contains(searchTerm) || x.makcb.Contains(searchTerm));
                }

                // Lọc theo khoảng thời gian
                if (fromDate.HasValue)
                {
                    query = query.Where(x => x.ngay >= fromDate.Value);
                }
                if (toDate.HasValue)
                {
                    var toDateEnd = toDate.Value.Date.AddDays(1).AddSeconds(-1);
                    query = query.Where(x => x.ngay <= toDateEnd);
                }

                // Đếm tổng số bản ghi (cho phân trang)
                var totalRecords = await query.CountAsync();
                var totalPages = (int)Math.Ceiling(totalRecords / (double)PageSize);

                // Lấy dữ liệu phân trang
                var danhSach = await query
                    .OrderByDescending(x => x.ngay)
                    .Skip((page - 1) * PageSize)
                    .Take(PageSize)
                    .AsNoTracking()
                    .ToListAsync();

                // Truyền thông tin phân trang vào ViewBag
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;
                ViewBag.TotalRecords = totalRecords;
                ViewBag.PageSize = PageSize;
                ViewBag.SearchTerm = searchTerm;
                ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
                ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");

                return View(danhSach);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Lỗi khi tải dữ liệu: {ex.Message}";
                return View(new List<ThanhToan>());
            }
        }

        /// <summary>
        /// Hiển thị form sửa thông tin chỉ định
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var item = await _context.ThanhToan
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.mathanhtoan == id);

                if (item == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy bản ghi!";
                    return RedirectToAction(nameof(Index));
                }

                return View(item);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Xử lý cập nhật thông tin chỉ định
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ThanhToan model)
        {
            try
            {
                if (id != model.mathanhtoan)
                {
                    TempData["ErrorMessage"] = "Dữ liệu không hợp lệ!";
                    return RedirectToAction(nameof(Index));
                }

                var item = await _context.ThanhToan.FindAsync(id);
                if (item == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy bản ghi!";
                    return RedirectToAction(nameof(Index));
                }

                // Chỉ cập nhật các trường được phép
                item.ngay = model.ngay;
                item.manv = model.manv;
                item.ailam = model.ailam;
                item.maailam = model.maailam;
                item.ngaythyl = model.ngaythyl;

                _context.Update(item);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Cập nhật thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi cập nhật: {ex.Message}";
                return View(model);
            }
        }

        /// <summary>
        /// Xóa bản ghi chỉ định
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var item = await _context.ThanhToan.FindAsync(id);
                if (item == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy bản ghi cần xóa!";
                    return RedirectToAction(nameof(Index));
                }

                _context.ThanhToan.Remove(item);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Xóa bản ghi thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi xóa: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
