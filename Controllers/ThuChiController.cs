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
    public class ThuChiController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ThuChiController> _logger;

        public ThuChiController(AppDbContext context, ILogger<ThuChiController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: ThuChi
        public IActionResult Index()
        {
            if (TempData["Success"] != null)
                ViewBag.Success = TempData["Success"];
            if (TempData["Error"] != null)
                ViewBag.Error = TempData["Error"];

            return View(new List<ThanhToanViewModel>());
        }

        // POST: ThuChi/Search
        // Bước 1+2: Nhập sophieu → lấy mathanhtoan → hiển thị danh sách thanhtoanct
        [HttpPost]
        public async Task<IActionResult> Search(string sophieu)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sophieu))
                {
                    ViewBag.Error = "Vui lòng nhập số phiếu!";
                    return View("Index", new List<ThanhToanViewModel>());
                }

                // Bước 1: Query thanhtoan by sophieu → lấy mathanhtoan
                var thanhtoan = await _context.ThanhToan
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.sophieu == sophieu);

                if (thanhtoan == null)
                {
                    ViewBag.Error = $"Không tìm thấy phiếu thanh toán với số phiếu: <strong>{sophieu}</strong>";
                    return View("Index", new List<ThanhToanViewModel>());
                }

                // Bước 2: Query thanhtoanct where mathanhtoan = value
                var result = await (
                    from ttct in _context.ThanhToanCT
                    where ttct.mathanhtoan == thanhtoan.mathanhtoan
                    select new ThanhToanViewModel
                    {
                        sophieu       = thanhtoan.sophieu,
                        makcb         = thanhtoan.makcb,
                        ngay          = thanhtoan.ngay,
                        mathanhtoan   = thanhtoan.mathanhtoan,
                        mathanhtoanct = ttct.mathanhtoanct,
                        thanhtien     = ttct.thanhtien
                    }
                ).AsNoTracking().ToListAsync();

                if (result == null || result.Count == 0)
                {
                    ViewBag.Error = $"Không tìm thấy chi tiết thanh toán cho số phiếu: <strong>{sophieu}</strong>";
                    return View("Index", new List<ThanhToanViewModel>());
                }

                ViewBag.Success = $"Tìm thấy <strong>{result.Count}</strong> bản ghi chi tiết";
                ViewBag.SoPhieu = sophieu;
                return View("Index", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tìm kiếm thu chi");
                ViewBag.Error = $"Lỗi truy vấn: {ex.Message}";
                return View("Index", new List<ThanhToanViewModel>());
            }
        }

        // POST: ThuChi/Delete
        // Bước 3→8: Người dùng chọn 1 mathanhtoanct → xóa trong transaction
        [HttpPost]
        public async Task<IActionResult> Delete(int mathanhtoan, long mathanhtoanct, string sophieu)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _logger.LogInformation(
                    "Bắt đầu xóa thu chi: mathanhtoan={mathanhtoan}, mathanhtoanct={mathanhtoanct}",
                    mathanhtoan, mathanhtoanct);

                // Bước 5: DELETE FROM thuchict
                var rows1 = await _context.Database.ExecuteSqlRawAsync(@"
                    DELETE FROM thuchict
                    WHERE mathanhtoan = @mathanhtoan
                      AND mathanhtoanct = @mathanhtoanct",
                    new SqlParameter("@mathanhtoan", mathanhtoan),
                    new SqlParameter("@mathanhtoanct", mathanhtoanct));

                _logger.LogInformation("Đã xóa {rows} bản ghi từ thuchict", rows1);

                // Bước 6: DELETE FROM thanhtoanct
                var rows2 = await _context.Database.ExecuteSqlRawAsync(@"
                    DELETE FROM thanhtoanct
                    WHERE mathanhtoan = @mathanhtoan
                      AND mathanhtoanct = @mathanhtoanct",
                    new SqlParameter("@mathanhtoan", mathanhtoan),
                    new SqlParameter("@mathanhtoanct", mathanhtoanct));

                _logger.LogInformation("Đã xóa {rows} bản ghi từ thanhtoanct", rows2);

                // Bước 7: COMMIT
                await transaction.CommitAsync();

                TempData["Success"] = $"✅ Xóa thành công! thuchict: {rows1} bản ghi, thanhtoanct: {rows2} bản ghi";
                _logger.LogInformation("Xóa thu chi thành công");

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Bước 8: ROLLBACK
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Lỗi khi xóa thu chi");

                TempData["Error"] = $"❌ Lỗi khi xóa: {ex.Message}";
                return RedirectToAction("Index");
            }
        }
    }
}
