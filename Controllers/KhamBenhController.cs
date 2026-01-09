using APP.Data;
using APP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

public class KhamBenhController : Controller
{
    private readonly AppDbContext _db;
    private readonly ILogger<KhamBenhController> _logger;

    public KhamBenhController(AppDbContext db, ILogger<KhamBenhController> logger)
    {
        _db = db;
        _logger = logger;
    }

    // ====== INDEX ======
    public IActionResult Index(long? makcb)      // ĐỔI từ string → long?
    {
        try
        {
            _logger.LogInformation($"Bắt đầu lấy danh sách hủy chuyển phòng - Mã KCB: {makcb?.ToString() ?? "ALL"}");

            // Lưu mã KCB vào ViewBag để hiển thị lại trong form
            ViewBag.MaKCB = makcb;

            // Nếu không nhập mã KCB, trả về danh sách rỗng
            if (!makcb.HasValue || makcb.Value == 0)
            {
                _logger.LogInformation("Chưa nhập mã KCB, trả về danh sách rỗng");
                return View(new List<HuyChuyenPhongModel>());
            }

            // QUERY ĐƠN GIẢN - chỉ lấy từ chuyenkhoa trước
            _logger.LogInformation($"Tìm kiếm trong bảng chuyenkhoa với makcb = {makcb.Value}");
            
            var dataSimple = _db.chuyenkhoa
                .Where(ck => ck.makcb == makcb.Value && ck.makkc != ck.makk)
                .ToList();
            
            _logger.LogInformation($"Tìm thấy {dataSimple.Count} bản ghi trong chuyenkhoa");
            
            if (dataSimple.Count == 0)
            {
                // Thử tìm tất cả để xem có data không
                var allData = _db.chuyenkhoa.Where(ck => ck.makkc != ck.makk).Take(5).ToList();
                _logger.LogInformation($"Tổng số bản ghi có makkc != makk: {allData.Count}");
                if (allData.Any())
                {
                    _logger.LogInformation($"Ví dụ makcb trong DB: {string.Join(", ", allData.Select(x => x.makcb))}");
                }
            }

            var query = from ck in _db.chuyenkhoa
                        join pg in _db.DmPhong on ck.makk equals pg.maphong into pgLeft
                        from pg in pgLeft.DefaultIfEmpty()
                        join pc in _db.DmPhong on ck.makkc equals pc.maphong into pcLeft
                        from pc in pcLeft.DefaultIfEmpty()
                        where ck.makcb == makcb.Value && ck.makkc != ck.makk
                        select new
                        {
                            ck,
                            pg,
                            pc
                        };

            var data = query.ToList().Select(x => new HuyChuyenPhongModel
            {
                makcb = x.ck.makcb,
                hoten = x.ck.matiepnhan?.ToString() ?? "N/A",  
                madieutri = x.ck.madieutri,

                makk = x.ck.makk,
                tenphonggoc = x.pg != null ? x.pg.tenphong : x.ck.makk.ToString(),

                makkc = x.ck.makkc,
                tenphongchuyen = x.pc != null ? x.pc.tenphong : x.ck.makkc.ToString(),

                ngaychuyen = x.ck.ngay,
                tinhtrang = x.ck.tinhtrang?.ToString(),

                // Tính tổng chi phí tại phòng chuyển
                tongchiphi = _db.thanhtoan
                    .Where(tt => tt.makcb == x.ck.makcb
                              && tt.madieutri == x.ck.madieutri
                              && tt.maphong == x.ck.makkc)
                    .Sum(tt => (decimal?)tt.thanhtoanke) ?? 0
            }).ToList();

            _logger.LogInformation($"Lấy thành công {data.Count} bản ghi cho mã KCB: {makcb.Value}");
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
    long makcb,     // ĐỔI từ string → long
    string madieutri,
    int makk,       // phòng gốc (1)
    int makkc       // phòng cần xoá (2,3)
)
{
    using var tran = _db.Database.BeginTransaction();
    try
    {
        _logger.LogInformation($"Bắt đầu hủy chuyển phòng: makcb={makcb}, madieutri={madieutri}, makk={makk}, makkc={makkc}");

        // 1. Đẩy chi phí về phòng gốc
        var rowsAffected1 = _db.Database.ExecuteSqlRaw(@"
            UPDATE thanhtoan
            SET maphong = @makk
            WHERE makcb = @makcb
              AND madieutri = @madieutri
              AND maphong = @makkc",
            new SqlParameter("@makk", makk),
            new SqlParameter("@makcb", makcb),
            new SqlParameter("@madieutri", madieutri),
            new SqlParameter("@makkc", makkc));
        _logger.LogInformation($"Đã cập nhật {rowsAffected1} dòng trong bảng thanhtoan");

        // 2. Xoá xếp phòng giường của phòng chuyển đến
        var rowsAffected2 = _db.Database.ExecuteSqlRaw(@"
            DELETE FROM xepphonggiuong
            WHERE makcb = @makcb
              AND madieutri = @madieutri
              AND maphong = @makkc",
            new SqlParameter("@makcb", makcb),
            new SqlParameter("@madieutri", madieutri),
            new SqlParameter("@makkc", makkc));
        _logger.LogInformation($"Đã xóa {rowsAffected2} dòng trong bảng xepphonggiuong");

        // 3. Xoá lịch sử chuyển phòng
        var rowsAffected3 = _db.Database.ExecuteSqlRaw(@"
            DELETE FROM chuyenkhoa
            WHERE makcb = @makcb
              AND madieutri = @madieutri
              AND makkc = @makkc",
            new SqlParameter("@makcb", makcb),
            new SqlParameter("@madieutri", madieutri),
            new SqlParameter("@makkc", makkc));
        _logger.LogInformation($"Đã xóa {rowsAffected3} dòng trong bảng chuyenkhoa");

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
