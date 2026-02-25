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

        /// <summary>
        /// Trang chính - hiển thị form tìm kiếm
        /// </summary>
        public IActionResult Index()
        {
            ViewBag.Message = null;
            return View(new List<ThanhToanViewModel>());
        }

        /// <summary>
        /// API tìm kiếm thanh toán theo số phiếu
        /// Chỉ nhập sophieu, hệ thống tự JOIN 2 bảng
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Search(string sophieu)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sophieu))
                {
                    ViewBag.Message = "⚠️ Vui lòng nhập số phiếu!";
                    ViewBag.MessageType = "warning";
                    return View("Index", new List<ThanhToanViewModel>());
                }

                // 🎯 TRUY VẤN CHÍNH: JOIN 2 bảng thanhtoan và thanhtoanct
                // Chỉ cần nhập sophieu, hệ thống tự lấy mathanhtoan và JOIN
                var result = await (
                    from tt in _context.ThanhToan
                    join ttct in _context.ThanhToanCT on tt.mathanhtoan equals ttct.mathanhtoan
                    where tt.sophieu == sophieu
                    select new ThanhToanViewModel
                    {
                        // Thông tin từ bảng thanhtoan (header)
                        sophieu = tt.sophieu,
                        makcb = tt.makcb,
                        ngay = tt.ngay,
                        mathanhtoan = tt.mathanhtoan,

                        // Thông tin từ bảng thanhtoanct (detail)
                        mathanhtoanct = ttct.mathanhtoanct
                    }
                ).AsNoTracking().ToListAsync();

                if (result == null || result.Count == 0)
                {
                    ViewBag.Message = $"❌ Không tìm thấy phiếu thanh toán với số phiếu: <strong>{sophieu}</strong>";
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
    }
}
