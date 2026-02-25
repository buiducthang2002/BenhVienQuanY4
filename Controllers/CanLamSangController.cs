using System;
using System.Linq;
using System.Threading.Tasks;
using APP.Data;
using Microsoft.AspNetCore.Mvc;
using APP.Models;
using Microsoft.EntityFrameworkCore;

namespace APP.Controllers
{
    public class CanLamSangController : Controller
    {
        private readonly AppDbContext _context;

        public CanLamSangController(AppDbContext context)
        {
            _context = context;
        }

   
        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 50)
        {
            // Validate page parameters
            if (page < 1) page = 1;
            if (pageSize < 10) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var query = from kq in _context.KetQuaCLS.AsNoTracking()
                        join dv in _context.DMDichVu on kq.mahh equals dv.mahh
                        join dk in _context.DangKy on kq.makcb equals dk.makcb      
                        select new CanLamSang
                        {
                            mahh = kq.mahh,
                            tendichvu = dv.tendichvu ?? "",
                            makcb = kq.makcb ?? "",
                            hoten = dk.hoten?? "",
                            barcode = kq.barcode ?? "",
                            manvlam = kq.manvlam ?? 0,
                            ketluan = kq.ketluan ?? "",
                            daky = kq.daky ?? "",
                            mathanhtoan = kq.mathanhtoan,
                            daduyet = kq.daduyet,
                            ngaylam = kq.ngaylam,
                            ngaykhopBC = kq.ngaykhopBC,
                            ngaytraKQ = kq.ngaytraKQ,
                            ailam = kq.ailam,
                            datrakq = kq.datrakq,
                            ngayth = kq.ngayth,
                            ngayketquaLIS = kq.ngayketquaLIS
                        };

            var totalRecords = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            var list = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalRecords = totalRecords;

            return View(list);
        }

     
        [HttpPost]
        public async Task<IActionResult> Search(string makcb)
        {
            var list = await (
                from kq in _context.KetQuaCLS.AsNoTracking()
                join dv in _context.DMDichVu on kq.mahh equals dv.mahh
                join dk in _context.DangKy on kq.makcb equals dk.makcb
                where kq.makcb == makcb
                select new CanLamSang
                {
                    mahh = kq.mahh,
                    tendichvu = dv.tendichvu ?? "",
                    makcb = kq.makcb ?? "",
                    hoten = dk.hoten ?? "",
                    barcode = kq.barcode ?? "",
                    manvlam = kq.manvlam ?? 0,
                    ketluan = kq.ketluan ?? "",
                    daky = kq.daky ?? "",
                    mathanhtoan = kq.mathanhtoan,
                    daduyet = kq.daduyet,
                    ngaylam = kq.ngaylam,
                    ngaykhopBC = kq.ngaykhopBC,
                    ngaytraKQ = kq.ngaytraKQ,
                    ailam = kq.ailam,
                    datrakq = kq.datrakq,
                    ngayth = kq.ngayth,
                    ngayketquaLIS = kq.ngayketquaLIS
                }
            ).ToListAsync();

            ViewBag.TotalRecords = list.Count;
            ViewBag.CurrentPage = 1;
            ViewBag.TotalPages = 1;
            ViewBag.PageSize = list.Count;

            return View("Index", list); 
        }


        [HttpPost]
        public async Task<IActionResult> EditDetail(string makcb, int mahh,
            DateTime? ngaylam, DateTime? ngaykhopBC, DateTime? ngaytraKQ,
            DateTime? ngayth, DateTime? ngayketquaLIS)
        {
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $@"UPDATE ketquacls SET
                    ngaylam       = {ngaylam},
                    ngaykhopBC    = {ngaykhopBC},
                    ngaytraKQ     = {ngaytraKQ},
                    ngayth        = {ngayth},
                    ngayketquaLIS = {ngayketquaLIS}
                   WHERE makcb = {makcb} AND mahh = {mahh}");

            TempData["SuccessMessage"] = "Cập nhật chi tiết thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSelected(string[] selectedItems)
        {
            if (selectedItems != null && selectedItems.Any())
            {
                var totalDeleted = 0;
                foreach (var item in selectedItems)
                {
                    var parts = item.Split('|');
                    if (parts.Length < 2) continue;
                    var makcb = parts[0];
                    if (!int.TryParse(parts[1], out var mahh)) continue;
                    var barcode = parts.Length >= 3 ? parts[2] : null;

                    int affected;
                    if (string.IsNullOrWhiteSpace(barcode))
                    {
                        // Không có barcode: xóa đúng 1 dòng trùng theo makcb + mahh + barcode IS NULL
                        affected = await _context.Database.ExecuteSqlInterpolatedAsync($"DELETE TOP (1) FROM ketquacls WHERE makcb = {makcb} AND mahh = {mahh} AND barcode IS NULL");
                    }
                    else
                    {
                        // Có barcode: xóa đúng 1 dòng trùng khớp đủ 3 trường
                        affected = await _context.Database.ExecuteSqlInterpolatedAsync($"DELETE TOP (1) FROM ketquacls WHERE makcb = {makcb} AND mahh = {mahh} AND barcode = {barcode}");
                    }
                    totalDeleted += affected;
                }

                if (totalDeleted > 0)
                {
                    TempData["SuccessMessage"] = $"Đã xóa thành công {totalDeleted} bản ghi!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Không tìm thấy bản ghi nào để xóa!";
                }
            }
            else
            {
                TempData["WarningMessage"] = "Vui lòng chọn ít nhất một bản ghi để xóa!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}


