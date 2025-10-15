using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APP.Data;
using APP.Models;
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
        public async Task<IActionResult> DeleteSelected(long[] selectedIds)
        {
            if (selectedIds != null && selectedIds.Any())
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
            else
            {
                TempData["Error"] = "Chưa chọn bản ghi nào.";
            }

            return RedirectToAction("Index");
        }

      
        [HttpGet]
        public async Task<IActionResult> Index(string? search)
        {
            var query = _context.PhauThuatThuThuat.AsQueryable();

        
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p =>
                    (!string.IsNullOrEmpty(p.makcb) && p.makcb.Contains(search)) ||
                    p.maphauthuat.ToString().Contains(search)
                );
            }


            var rawList = await query
                .OrderByDescending(p => p.ngaybatdaumo)
                .Take(2000)
                .ToListAsync();

       
            var list = rawList
                .GroupBy(p => new { p.makcb, p.maphauthuat })
                .Select(g => g.First())
                .ToList();


            ViewBag.Search = search;
            return View("Index", list);
        }

        public IActionResult Default()
        {
            return RedirectToAction("Index");
        }
    }
}