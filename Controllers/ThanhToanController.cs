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
    public class ThanhToanController : Controller
    {
        private readonly AppDbContext _context;

        public ThanhToanController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? sophieu = null)
        {
            if (string.IsNullOrWhiteSpace(sophieu))
            {
                ViewBag.Message = null;
                return View(new List<ThanhToanViewModel>());
            }
            return await DoSearch(sophieu);
        }

        [HttpPost]
        public async Task<IActionResult> Search(string sophieu)
        {
            return await DoSearch(sophieu);
        }

        private async Task<IActionResult> DoSearch(string? sophieu)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sophieu))
                {
                    ViewBag.Message = "⚠️ Vui lòng nhập số phiếu!";
                    ViewBag.MessageType = "warning";
                    return View("Index", new List<ThanhToanViewModel>());
                }

                var result = await (
                    from tt in _context.ThanhToan
                    join ttct in _context.ThanhToanCT on tt.mathanhtoan equals ttct.mathanhtoan
                    join dt in _context.DonThuoc on tt.makcb equals dt.makcb into dtGroup
                    from dt in dtGroup.DefaultIfEmpty()
                    where tt.sophieu == sophieu
                    select new ThanhToanViewModel
                    {
                        sophieu = tt.sophieu,
                        makcb = tt.makcb,
                        ngay = tt.ngay,
                        ngaythyl = tt.ngaythyl,
                        mathanhtoan = tt.mathanhtoan,
                        mathanhtoanct = ttct.mathanhtoanct,
                        thanhtien = ttct.thanhtien,
                        madonthuoc = dt != null ? dt.madonthuoc : (int?)null,
                        dt_ngay = dt != null ? dt.ngay : (DateTime?)null,
                        ngayduyet = dt != null ? dt.ngayduyet : (DateTime?)null
                    }
                ).AsNoTracking().ToListAsync();

                if (result == null || result.Count == 0)
                {
                    ViewBag.Message = $"❌ Không tìm thấy phiếu với số phiếu: <strong>{sophieu}</strong>";
                    ViewBag.MessageType = "danger";
                    return View("Index", new List<ThanhToanViewModel>());
                }

                ViewBag.Message = $"✅ Tìm thấy <strong>{result.Count}</strong> bản ghi";
                ViewBag.MessageType = "success";
                ViewBag.SoPhieuTimKiem = sophieu;

                return View("Index", result);
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"❌ Lỗi truy vấn: {ex.Message}";
                ViewBag.MessageType = "danger";
                return View("Index", new List<ThanhToanViewModel>());
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDates(string sophieu, DateTime? ttNgay, DateTime? ngayduyet)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sophieu))
                {
                    TempData["ErrorMessage"] = "Thiếu số phiếu!";
                    return RedirectToAction("Index");
                }

                if (!ttNgay.HasValue)
                {
                    TempData["ErrorMessage"] = "Vui lòng nhập Ngày!";
                    return RedirectToAction("Index", new { sophieu });
                }

                var ngay = ttNgay.Value;

                int ttRows = await _context.Database.ExecuteSqlInterpolatedAsync(
                    $"UPDATE thanhtoan SET ngay = {ngay}, ngaythyl = {ngay} WHERE sophieu = {sophieu}");

                int dtRows = await _context.Database.ExecuteSqlInterpolatedAsync(
                    $"UPDATE donthuoc SET ngay = {ngay}, ngayduyet = {ngayduyet} WHERE sophieu = {sophieu}");

                TempData["SuccessMessage"] =
                    $"Cập nhật xong — thanhtoan: {ttRows} dòng, donthuoc: {dtRows} dòng (sophieu={sophieu}, ngay={ngay:yyyy-MM-dd HH:mm}, ngayduyet={ngayduyet?.ToString("yyyy-MM-dd HH:mm") ?? "null"})";
            }
            catch (Exception ex)
            {
                var inner = ex.InnerException;
                var detail = inner != null ? $"{ex.Message} | Inner: {inner.Message}" : ex.Message;
                TempData["ErrorMessage"] = $"Lỗi cập nhật: {detail}";
            }

            return RedirectToAction("Index", new { sophieu });
        }
    }
}
