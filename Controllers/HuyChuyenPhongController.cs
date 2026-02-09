using APP.Data;
using APP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace APP.Controllers
{
    public class HuyChuyenPhongController : Controller
    {
        private readonly AppDbContext _db;
        private readonly ILogger<HuyChuyenPhongController> _logger;

        public HuyChuyenPhongController(AppDbContext db, ILogger<HuyChuyenPhongController> logger)
        {
            _db = db;
            _logger = logger;
        }

        // ====== INDEX ======
        public IActionResult Index(long? makcb)
        {
            try
            {
                _logger.LogInformation($"Bắt đầu lấy danh sách hủy chuyển phòng - Mã KCB: {makcb?.ToString() ?? "ALL"}");

                // Lưu mã KCB vào ViewBag để hiển thị lại trong form
                ViewBag.MaKCB = makcb;

                // Validate input
                if (!makcb.HasValue || makcb.Value <= 0)
                {
                    _logger.LogInformation("Chưa nhập mã KCB hợp lệ, trả về danh sách rỗng");
                    return View(new List<HuyChuyenPhongModel>());
                }

                // QUERY TỐI ƯU - Gộp tất cả vào 1 query duy nhất
                _logger.LogInformation($"Tìm kiếm trong bảng chuyenkhoa với makcb = {makcb.Value}");

                var data = (from ck in _db.chuyenkhoa.AsNoTracking()
                            join dk in _db.DangKy on ck.makcb.ToString() equals dk.makcb into dkLeft
                            from dk in dkLeft.DefaultIfEmpty()
                            join pg in _db.DmPhong on ck.makk equals pg.maphong into pgLeft
                            from pg in pgLeft.DefaultIfEmpty()
                            join pc in _db.DmPhong on ck.makkc equals pc.maphong into pcLeft
                            from pc in pcLeft.DefaultIfEmpty()
                            where ck.makcb == makcb.Value && ck.makkc != ck.makk
                            select new
                            {
                                ck.makcb,
                                ck.madieutri,
                                ck.makk,
                                ck.makkc,
                                ck.ngay,
                                ck.tinhtrang,
                                hoten = dk != null ? dk.hoten : "N/A",
                                tenphonggoc = pg != null ? pg.tenphong : ck.makk.ToString(),
                                tenphongchuyen = pc != null ? pc.tenphong : ck.makkc.ToString()
                            })
                            .ToList()
                            .Select(x => new HuyChuyenPhongModel
                            {
                                makcb = x.makcb,
                                hoten = x.hoten ?? "N/A",
                                madieutri = x.madieutri,
                                makk = x.makk,
                                tenphonggoc = x.tenphonggoc ?? "",
                                makkc = x.makkc,
                                tenphongchuyen = x.tenphongchuyen ?? "",
                                ngaychuyen = x.ngay,
                                tinhtrang = x.tinhtrang?.ToString()
                            })
                            .ToList();

                _logger.LogInformation($"Lấy thành công {data.Count} bản ghi cho mã KCB: {makcb.Value}");

                if (data.Count == 0)
                {
                    _logger.LogInformation($"Không tìm thấy bản ghi nào cho mã KCB: {makcb.Value}");
                }

                return View(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LỖI khi lấy danh sách hủy chuyển phòng");
                _logger.LogError($"Chi tiết lỗi: {ex.Message}");
                _logger.LogError($"StackTrace: {ex.StackTrace}");

                if (ex.InnerException != null)
                {
                    _logger.LogError($"Inner Exception: {ex.InnerException.Message}");
                }

                ViewBag.ErrorMessage = $"Lỗi: {ex.Message}";
                return View(new List<HuyChuyenPhongModel>());
            }
        }

        [HttpPost]
        public IActionResult HuyChuyenPhong(
            long makcb,
            string madieutri,
            int makk,       // phòng gốc
            int makkc       // phòng cần xoá
        )
        {
            using var tran = _db.Database.BeginTransaction();
            try
            {
                _logger.LogInformation($"Bắt đầu hủy chuyển phòng: makcb={makcb}, madieutri={madieutri}, makk={makk}, makkc={makkc}");

                // Validate input
                if (makcb <= 0 || string.IsNullOrEmpty(madieutri) || makk <= 0 || makkc <= 0)
                {
                    return BadRequest("Tham số không hợp lệ");
                }

                if (makk == makkc)
                {
                    return BadRequest("Phòng gốc và phòng chuyển không được trùng nhau");
                }

                // 1. Xoá lịch sử chuyển phòng
                var rowsAffected1 = _db.Database.ExecuteSqlRaw(@"
                    DELETE FROM chuyenkhoa
                    WHERE makcb = @makcb
                      AND madieutri = @madieutri
                      AND makkc = @makkc",
                    new SqlParameter("@makcb", makcb),
                    new SqlParameter("@madieutri", madieutri),
                    new SqlParameter("@makkc", makkc));
                _logger.LogInformation($"Đã xóa {rowsAffected1} dòng trong bảng chuyenkhoa");

                // 2. Xoá bảng chuyenphong
                var rowsAffected2 = _db.Database.ExecuteSqlRaw(@"
                    DELETE FROM chuyenphong
                    WHERE makcb = @makcb",
                    new SqlParameter("@makcb", makcb));
                _logger.LogInformation($"Đã xóa {rowsAffected2} dòng trong bảng chuyenphong");

                // 3. Xoá bảng khambenh với nhokham = 1
                var rowsAffected3 = _db.Database.ExecuteSqlRaw(@"
                    DELETE FROM khambenh
                    WHERE makcb = @makcb
                      AND nhokham = 1",
                    new SqlParameter("@makcb", makcb));
                _logger.LogInformation($"Đã xóa {rowsAffected3} dòng trong bảng khambenh (nhokham=1)");

                tran.Commit();
                _logger.LogInformation("Hủy chuyển phòng thành công");
                return Ok("Đã hủy chuyển phòng, bệnh nhân quay lại phòng gốc");
            }
            catch (Exception ex)
            {
                tran.Rollback();
                _logger.LogError(ex, "LỖI khi hủy chuyển phòng");
                _logger.LogError($"Chi tiết lỗi: {ex.Message}");
                _logger.LogError($"StackTrace: {ex.StackTrace}");

                if (ex.InnerException != null)
                {
                    _logger.LogError($"Inner Exception: {ex.InnerException.Message}");
                }

                return BadRequest($"Xử lý thất bại: {ex.Message}");
            }
        }
    }
}
