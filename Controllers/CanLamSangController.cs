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
        public async Task<IActionResult> Index()
        {
            var list = await (
                from kq in _context.KetQuaCLS
                join dv in _context.DMDichVu on kq.mahh equals dv.mahh
                select new CanLamSang
                {
                    mahh = dv.mahh,
                    tendichvu = dv.tendichvu ?? "",
                    makcb = kq.makcb ?? "",
                    barcode = kq.barcode ?? "",
                    manvlam = kq.manvlam ?? 0,
                    ketluan = kq.ketluan ?? "",
                    daky = kq.daky ?? ""
                }
            ).Take(100).ToListAsync(); 
            return View(list);
        }

     
        [HttpPost]
        public async Task<IActionResult> Search(string makcb)
        {
            var list = await (
                from kq in _context.KetQuaCLS
                join dv in _context.DMDichVu on kq.mahh equals dv.mahh
                where kq.makcb == makcb
                select new CanLamSang
                {
                    mahh = dv.mahh,
                    tendichvu = dv.tendichvu ?? "",
                    makcb = kq.makcb ?? "",
                    barcode = kq.barcode ?? "",
                    manvlam = kq.manvlam ?? 0,
                    ketluan = kq.ketluan ?? "",
                    daky = kq.daky ?? ""
                }
            ).ToListAsync();

            return View("Index", list); 
        }


        [HttpPost]
        public async Task<IActionResult> DeleteSelected(int[] selectedIds)
        {
            if (selectedIds != null && selectedIds.Any())
            {
                var records = await _context.KetQuaCLS
                     .Where(k => selectedIds.Contains(k.mahh))
                    .ToListAsync();

                if (records.Any())
                {
                    _context.KetQuaCLS.RemoveRange(records);
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}