using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APP.Data;
using APP.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace APP.Controllers
{
    public class DangKyController : Controller
    {
        private readonly AppDbContext _context;

        public DangKyController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Index(string? search)
        {
            var query =
                from dk in _context.DangKy
                join p in _context.DmPhong on dk.maphong equals p.maphong into phongGroup
                from phong in phongGroup.DefaultIfEmpty()

                join k in _context.DmKhoa on dk.makk equals k.makk into khoaGroup
                from khoa in khoaGroup.DefaultIfEmpty()

                join cv in _context.DmChucvu on dk.machucvu equals cv.machucvu into chucvuGroup
                from chucvu in chucvuGroup.DefaultIfEmpty()

                join cb in _context.DmCapbac on dk.macapbac equals cb.macapbac into capbacGroup
                from capbac in capbacGroup.DefaultIfEmpty()

                join px in _context.DmPhuongxa on dk.mapx equals px.mapx into phuongGroup
                from phuong in phuongGroup.DefaultIfEmpty()

                join tt in _context.DmTt on dk.matt equals tt.matt into tinhGroup
                from tinh in tinhGroup.DefaultIfEmpty()

                join lh in _context.DmDangkyloaihinhkcb on dk.idloaihinhkcb equals lh.idloaihinhkcb into loaiHinhGroup
                from loaihinh in loaiHinhGroup.DefaultIfEmpty()

                join ht in _context.DmHinhthucdenkham on dk.mahtd equals ht.mahtd into hinhThucGroup
                from hinhthuc in hinhThucGroup.DefaultIfEmpty()

                select new DangKyViewModel
                {
                    makcb = dk.makcb,
                    hoten = dk.hoten,
                    ngaydk = dk.ngaydk,
                    ngaysinh = dk.ngaysinh,
                    maphai = dk.maphai,
                    socmnd = dk.socmnd,
                    dienthoai = dk.dienthoai,
                    sobhxh = dk.sobhxh,
                    lydovv130 = dk.lydovv130,
                    noilamviec = dk.noilamviec,
                    sonha = dk.sonha,
                    thonpho = dk.thonpho,
                    mahinhthucden = dk.mahtd,
                    idloaihinhkcb = dk.idloaihinhkcb,
                    tenphong = phong.tenphong,
                    tenkk = khoa.tenkk,
                    tenchucvu = chucvu.tenchucvu,
                    tencapbac = capbac.tencapbac,
                    viettat = phuong.viettat,
                    tentinh = tinh.tentinh,
                    diengiai = loaihinh.diengiai,
                    tenhtd = hinhthuc.tenhtd
                };

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x =>
                    x.makcb.Contains(search) ||
                    x.hoten.Contains(search));
            }

            var list = await query
                .OrderByDescending(x => x.ngaydk)
                .Take(500)
                .ToListAsync();

            ViewBag.Search = search;
            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string makcb)
        {
            if (string.IsNullOrEmpty(makcb))
                return NotFound();

            var record = await _context.DangKy.FirstOrDefaultAsync(x => x.makcb == makcb);
            if (record == null)
                return NotFound();

            ViewBag.PhongList = new SelectList(await _context.DmPhong.ToListAsync(), "maphong", "tenphong");
            ViewBag.KhoaList = new SelectList(await _context.DmKhoa.ToListAsync(), "makk", "tenkk");
            ViewBag.ChucVuList = new SelectList(await _context.DmChucvu.ToListAsync(), "machucvu", "tenchucvu");
            ViewBag.CapBacList = new SelectList(await _context.DmCapbac.ToListAsync(), "macapbac", "tencapbac");
            ViewBag.TinhList = new SelectList(await _context.DmTt.ToListAsync(), "matt", "tentinh");
            ViewBag.PhuongList = new SelectList(await _context.DmPhuongxa.ToListAsync(), "mapx", "viettat");
            ViewBag.LoaiHinhList = new SelectList(await _context.DmDangkyloaihinhkcb.ToListAsync(), "idloaihinhkcb", "diengiai");
            ViewBag.HinhThucList = new SelectList(await _context.DmHinhthucdenkham.ToListAsync(), "mahtd", "tenhtd");
            return View(record);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DangKy model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var existing = await _context.DangKy.FirstOrDefaultAsync(x => x.makcb == model.makcb);
            if (existing == null)
                return NotFound();

            existing.hoten = model.hoten;
            existing.ngaydk = model.ngaydk;
            existing.ngaysinh = model.ngaysinh;
            existing.maphai = model.maphai;
            existing.maphong = model.maphong;
            existing.dienthoai = model.dienthoai;
            existing.sobhxh = model.sobhxh;
            existing.mahtd = model.mahtd;
            existing.idloaihinhkcb = model.idloaihinhkcb;
            existing.lydovv130 = model.lydovv130;
            existing.idloaihinhkcb = model.idloaihinhkcb;
            existing.mahtd = model.mahtd;
            existing.noilamviec = model.noilamviec;
            existing.sonha = model.sonha;
            existing.thonpho = model.thonpho;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Cập nhật thông tin thành công!";
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteSelected(string[] selectedIds)
        {
            if (selectedIds != null && selectedIds.Any())
            {
                foreach (var id in selectedIds)
                {
                    await _context.Database.ExecuteSqlRawAsync("EXEC xoabenhnhan @p0", id);
                }

                TempData["Success"] = $"{selectedIds.Length} bệnh nhân đã được xóa.";
            }
            else
            {
                TempData["Error"] = "Không có bản ghi nào được chọn để xóa.";
            }

            return RedirectToAction("Index");
        }

    }
}