using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using APP.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace APP.Controllers
{
    public class HuyChuyenPhongController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<HuyChuyenPhongController> _logger;

        public HuyChuyenPhongController(AppDbContext context, ILogger<HuyChuyenPhongController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: HuyChuyenPhong
        public IActionResult Index()
        {
            return View();
        }

        // POST: HuyChuyenPhong/Delete
        [HttpPost]
        public async Task<IActionResult> Delete(string makcb)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _logger.LogInformation($"Bắt đầu hủy chuyển phòng cho Mã KCB: {makcb}");

                if (string.IsNullOrWhiteSpace(makcb))
                {
                    TempData["Error"] = "Vui lòng nhập Mã KCB!";
                    return RedirectToAction("Index");
                }

                // 1. Xóa dữ liệu từ bảng chuyenphong
                var rowsAffected1 = await _context.Database.ExecuteSqlRawAsync(@"
                    DELETE FROM chuyenphong
                    WHERE makcb = @makcb",
                    new SqlParameter("@makcb", makcb));

                _logger.LogInformation($"Đã xóa {rowsAffected1} bản ghi từ bảng chuyenphong");

                // 2. Xóa dữ liệu từ bảng khambenh với điều kiện nhokham = 1
                var rowsAffected2 = await _context.Database.ExecuteSqlRawAsync(@"
                    DELETE FROM khambenh
                    WHERE makcb = @makcb AND nhokham = 1",
                    new SqlParameter("@makcb", makcb));

                _logger.LogInformation($"Đã xóa {rowsAffected2} bản ghi từ bảng khambenh (nhokham=1)");

                await transaction.CommitAsync();

                TempData["Success"] = $"✅ Đã xóa thành công! Chuyển phòng: {rowsAffected1} bản ghi, Khám bệnh: {rowsAffected2} bản ghi";
                _logger.LogInformation($"Hủy chuyển phòng thành công cho Mã KCB: {makcb}");

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"Lỗi khi hủy chuyển phòng cho Mã KCB: {makcb}");
                _logger.LogError($"Chi tiết lỗi: {ex.Message}");

                if (ex.InnerException != null)
                {
                    _logger.LogError($"Inner Exception: {ex.InnerException.Message}");
                }

                TempData["Error"] = $"❌ Lỗi khi hủy chuyển phòng: {ex.Message}";
                return RedirectToAction("Index");
            }
        }
    }
}
