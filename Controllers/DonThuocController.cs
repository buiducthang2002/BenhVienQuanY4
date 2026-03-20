using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using APP.Data;
using APP.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace APP.Controllers
{
    public class DonThuocController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DonThuocController> _logger;

        public DonThuocController(AppDbContext context, ILogger<DonThuocController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: DonThuoc
        public IActionResult Index()
        {
            if (TempData["Success"] != null)
                ViewBag.Success = TempData["Success"];
            if (TempData["Error"] != null)
                ViewBag.Error = TempData["Error"];

            return View(new List<DonThuocViewModel>());
        }

        // POST: DonThuoc/Search
        // Nhập makcb → lấy madonthuoc → hiển thị danh sách donthuocct
        [HttpPost]
        public async Task<IActionResult> Search(string makcb)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(makcb))
                {
                    ViewBag.Error = "Vui lòng nhập mã KCB!";
                    return View("Index", new List<DonThuocViewModel>());
                }

                // Lấy madonthuoc từ bảng donthuoc theo makcb
                var donthuoc = await _context.DonThuoc
                    .AsNoTracking()
                    .FirstOrDefaultAsync(d => d.makcb == makcb);

                if (donthuoc == null)
                {
                    ViewBag.Error = $"Không tìm thấy đơn thuốc cho mã KCB: <strong>{makcb}</strong>";
                    return View("Index", new List<DonThuocViewModel>());
                }

                // Lấy danh sách donthuocct theo madonthuoc
                var result = await (
                    from dtct in _context.DonThuocCT
                    where dtct.madonthuoc == donthuoc.madonthuoc
                    select new DonThuocViewModel
                    {
                        makcb = donthuoc.makcb,
                        madonthuoc = donthuoc.madonthuoc,
                        madonthuocct = dtct.madonthuocct,
                        soluong = dtct.soluong,
                        dongia = dtct.dongia,
                        thanhtien = dtct.thanhtien
                    }
                ).AsNoTracking().ToListAsync();

                if (result == null || result.Count == 0)
                {
                    ViewBag.Error = $"Không tìm thấy chi tiết đơn thuốc cho mã KCB: <strong>{makcb}</strong>";
                    return View("Index", new List<DonThuocViewModel>());
                }

                ViewBag.MaKCB = makcb;
                return View("Index", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tìm kiếm đơn thuốc");
                ViewBag.Error = $"Lỗi truy vấn: {ex.Message}";
                return View("Index", new List<DonThuocViewModel>());
            }
        }

        // POST: DonThuoc/Delete
        // Xóa các madonthuocc đã chọn trong bảng donthuocct
        [HttpPost]
        public async Task<IActionResult> Delete(int madonthuoc, List<long> selectedItems, string makcb)
        {
            if (selectedItems == null || selectedItems.Count == 0)
            {
                TempData["Error"] = "Vui lòng chọn ít nhất một bản ghi để xóa!";
                return RedirectToAction("Index");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _logger.LogInformation(
                    "Bắt đầu xóa donthuocct: madonthuoc={madonthuoc}, số lượng={count}",
                    madonthuoc, selectedItems.Count);

                int totalDeleted = 0;
                foreach (var madonthuocct in selectedItems)
                {
                    var rows = await _context.Database.ExecuteSqlRawAsync(@"
                        DELETE FROM donthuocct
                        WHERE madonthuoc = @madonthuoc
                          AND madonthuocct = @madonthuocct",
                        new SqlParameter("@madonthuoc", madonthuoc),
                        new SqlParameter("@madonthuocct", madonthuocct));
                    totalDeleted += rows;
                    _logger.LogInformation("Đã xóa madonthuocct={madonthuocct}: {rows} bản ghi", madonthuocct, rows);
                }

                await transaction.CommitAsync();

                TempData["Success"] = $"Xóa thành công {totalDeleted} bản ghi chi tiết đơn thuốc!";
                _logger.LogInformation("Xóa donthuocct thành công: {total} bản ghi", totalDeleted);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Lỗi khi xóa donthuocct");
                TempData["Error"] = $"Lỗi khi xóa: {ex.Message}";
                return RedirectToAction("Index");
            }
        }
    }
}
