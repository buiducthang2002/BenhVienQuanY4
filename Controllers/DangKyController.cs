using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APP.Data;
using APP.Models;
using APP.Services;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;

namespace APP.Controllers
{
    public class DangKyController : Controller
    {
        private readonly AppDbContext _context;
        private readonly CacheService _cache;

        public DangKyController(AppDbContext context, CacheService cache)
        {
            _context = context;
            _cache = cache;
        }
        [HttpGet]
        public async Task<IActionResult> Index(string? search, int page = 1, int pageSize = 50)
        {
            // Validate page parameters
            if (page < 1) page = 1;
            if (pageSize < 10) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var query =
                from dk in _context.DangKy.AsNoTracking()
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
                    makcb = dk.makcb ?? string.Empty,
                    hoten = dk.hoten ?? string.Empty,
                    ngaydk = dk.ngaydk,
                    ngaysinh = dk.ngaysinh,
                    maphai = dk.maphai,
                    socmnd = dk.socmnd ?? string.Empty,
                    manghenghiep = dk.manghenghiep,
                    madoituong = dk.madoituong,
                    dienthoai = dk.dienthoai ?? string.Empty,
                    sobhxh = dk.sobhxh ?? string.Empty,
                    lydovv130 = dk.lydovv130 ?? string.Empty,
                    noilamviec = dk.noilamviec ?? string.Empty,
                    sonha = dk.sonha ?? string.Empty,
                    thonpho = dk.thonpho ?? string.Empty,
                    mahinhthucden = dk.mahtd,
                    idloaihinhkcb = dk.idloaihinhkcb,
                    manv = dk.manv,
                    madonvi = dk.madonvi,
                    tenphong = phong.tenphong,
                    tenkk = khoa.tenkk,
                    tenchucvu = chucvu.tenchucvu,
                    tencapbac = capbac.tencapbac,
                    tenxa = phuong.tenxa,
                    tentinh = tinh.tentinh,
                    diengiai = loaihinh.diengiai,
                    tenhtd = hinhthuc.tenhtd
                };

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x =>
                    (x.makcb ?? string.Empty).Contains(search) ||
                    (x.hoten ?? string.Empty).Contains(search));
            }

            // Get total count for pagination
            var totalRecords = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            var list = await query
                .OrderByDescending(x => x.ngaydk)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Search = search;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalRecords = totalRecords;
            
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

            // Load all dropdown lists from cache for better performance
            var phuongTask = _cache.GetPhuongListAsync();
            var phongTask = _cache.GetPhongListAsync();
            var khoaTask = _cache.GetKhoaListAsync();
            var chucvuTask = _cache.GetChucVuListAsync();
            var capbacTask = _cache.GetCapBacListAsync();
            var tinhTask = _cache.GetTinhListAsync();
            var loaiHinhTask = _cache.GetLoaiHinhListAsync();
            var hinhThucTask = _cache.GetHinhThucListAsync();

            await Task.WhenAll(phuongTask, phongTask, khoaTask, chucvuTask, capbacTask, tinhTask, loaiHinhTask, hinhThucTask);

            ViewBag.PhongList = new SelectList(phongTask.Result, "maphong", "tenphong", record.maphong);
            ViewBag.KhoaList = new SelectList(khoaTask.Result, "makk", "tenkk", record.makk);
            ViewBag.ChucVuList = new SelectList(chucvuTask.Result, "machucvu", "tenchucvu", record.machucvu);
            ViewBag.CapBacList = new SelectList(capbacTask.Result, "macapbac", "tencapbac", record.macapbac);
            ViewBag.TinhList = new SelectList(tinhTask.Result, "matt", "tentinh", record.matt);
            ViewBag.PhuongList = new SelectList(phuongTask.Result, "mapx", "tenxa", record.mapx);
            ViewBag.LoaiHinhList = new SelectList(loaiHinhTask.Result, "idloaihinhkcb", "diengiai", record.idloaihinhkcb);
            ViewBag.HinhThucList = new SelectList(hinhThucTask.Result, "mahtd", "tenhtd", record.mahtd);
            return View(record);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DangKy model)
        {
            // Kiểm tra makcb có giá trị không
            if (string.IsNullOrEmpty(model.makcb))
            {
                TempData["Error"] = "Mã KCB không được để trống.";
                return RedirectToAction("Index");
            }

            await LoadDropdownListsAsync(model.maphong, model.makk, model.machucvu, model.macapbac, model.matt, model.mapx, model.idloaihinhkcb, model.mahtd);

            // Xóa lỗi validation cho tenxa vì nó không được lưu vào database (đã Ignore trong DbContext)
            ModelState.Remove("tenxa");

            if (!ModelState.IsValid)
            {
                // Hiển thị tất cả lỗi validation
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                if (errors.Any())
                {
                    TempData["Error"] = "Có lỗi validation: " + string.Join("; ", errors);
                }
                return View(model);
            }

            var existing = await _context.DangKy.FirstOrDefaultAsync(x => x.makcb == model.makcb);
            if (existing == null)
            {
                TempData["Error"] = "Không tìm thấy bản ghi cần cập nhật.";
                return View(model);
            }

            // Cập nhật tất cả các trường từ form
            existing.hoten = model.hoten;
            existing.ngaydk = model.ngaydk;
            existing.ngaysinh = model.ngaysinh;
            
            // Xử lý maphai - bắt buộc phải có giá trị (1 hoặc 2)
            if (!model.maphai.HasValue || (model.maphai.Value != 1 && model.maphai.Value != 2))
            {
                ModelState.AddModelError("maphai", "Vui lòng chọn giới tính.");
                return View(model);
            }
            existing.maphai = model.maphai.Value;
            
            existing.socmnd = model.socmnd;
            existing.manghenghiep = model.manghenghiep;
            existing.madoituong = model.madoituong;
            
            // Xử lý các trường int? - chuyển 0 hoặc null thành null và validate foreign key
            if (model.maphong.HasValue && model.maphong.Value > 0)
            {
                // Kiểm tra phòng có tồn tại không
                var phongExists = await _context.DmPhong.AnyAsync(p => p.maphong == model.maphong.Value);
                if (!phongExists)
                {
                    ModelState.AddModelError("maphong", "Phòng được chọn không tồn tại trong hệ thống.");
                    return View(model);
                }
                existing.maphong = model.maphong;
            }
            else
            {
                existing.maphong = null;
            }

            if (model.makk.HasValue && model.makk.Value > 0)
            {
                var khoaExists = await _context.DmKhoa.AnyAsync(k => k.makk == model.makk.Value);
                if (!khoaExists)
                {
                    ModelState.AddModelError("makk", "Khoa được chọn không tồn tại trong hệ thống.");
                    return View(model);
                }
                existing.makk = model.makk;
            }
            else
            {
                existing.makk = null;
            }

            if (model.machucvu.HasValue && model.machucvu.Value > 0)
            {
                var chucvuExists = await _context.DmChucvu.AnyAsync(cv => cv.machucvu == model.machucvu.Value);
                if (!chucvuExists)
                {
                    ModelState.AddModelError("machucvu", "Chức vụ được chọn không tồn tại trong hệ thống.");
                    return View(model);
                }
                existing.machucvu = model.machucvu;
            }
            else
            {
                existing.machucvu = null;
            }

            if (model.macapbac.HasValue && model.macapbac.Value > 0)
            {
                var capbacExists = await _context.DmCapbac.AnyAsync(cb => cb.macapbac == model.macapbac.Value);
                if (!capbacExists)
                {
                    ModelState.AddModelError("macapbac", "Cấp bậc được chọn không tồn tại trong hệ thống.");
                    return View(model);
                }
                existing.macapbac = model.macapbac;
            }
            else
            {
                existing.macapbac = null;
            }

            if (model.matt.HasValue && model.matt.Value > 0)
            {
                var tinhExists = await _context.DmTt.AnyAsync(t => t.matt == model.matt.Value);
                if (!tinhExists)
                {
                    ModelState.AddModelError("matt", "Tỉnh được chọn không tồn tại trong hệ thống.");
                    return View(model);
                }
                existing.matt = model.matt;
            }
            else
            {
                existing.matt = null;
            }

            if (model.mapx.HasValue && model.mapx.Value > 0)
            {
                var phuongExists = await _context.DmPhuongxa.AnyAsync(px => px.mapx == model.mapx.Value);
                if (!phuongExists)
                {
                    ModelState.AddModelError("mapx", "Xã/Phường được chọn không tồn tại trong hệ thống.");
                    return View(model);
                }
                existing.mapx = model.mapx;
            }
            else
            {
                existing.mapx = null;
            }

            if (model.mahtd.HasValue && model.mahtd.Value > 0)
            {
                var hinhthucExists = await _context.DmHinhthucdenkham.AnyAsync(ht => ht.mahtd == model.mahtd.Value);
                if (!hinhthucExists)
                {
                    ModelState.AddModelError("mahtd", "Hình thức đến khám được chọn không tồn tại trong hệ thống.");
                    return View(model);
                }
                existing.mahtd = model.mahtd;
            }
            else
            {
                existing.mahtd = null;
            }

            if (model.idloaihinhkcb.HasValue && model.idloaihinhkcb.Value > 0)
            {
                var loaihinhExists = await _context.DmDangkyloaihinhkcb.AnyAsync(lh => lh.idloaihinhkcb == model.idloaihinhkcb.Value);
                if (!loaihinhExists)
                {
                    ModelState.AddModelError("idloaihinhkcb", "Loại hình KCB được chọn không tồn tại trong hệ thống.");
                    return View(model);
                }
                existing.idloaihinhkcb = model.idloaihinhkcb;
            }
            else
            {
                existing.idloaihinhkcb = null;
            }
            
            existing.dienthoai = model.dienthoai;
            existing.sobhxh = model.sobhxh;
            existing.lydovv130 = model.lydovv130;
            existing.noilamviec = model.noilamviec;
            existing.sonha = model.sonha;
            existing.thonpho = model.thonpho;
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                // Lấy thông tin chi tiết từ inner exception
                string errorMessage = dbEx.Message;
                if (dbEx.InnerException != null)
                {
                    errorMessage += $" | Chi tiết: {dbEx.InnerException.Message}";
                    
                    // Kiểm tra lỗi SQL Server cụ thể
                    if (dbEx.InnerException is SqlException sqlEx)
                    {
                        errorMessage += $" | SQL Error: {sqlEx.Number}";
                        if (sqlEx.Number == 547) // Foreign key constraint
                        {
                            errorMessage += " - Lỗi ràng buộc khóa ngoại. Vui lòng kiểm tra giá trị phòng/khoa/chức vụ... có tồn tại trong hệ thống.";
                        }
                        else if (sqlEx.Number == 515) // Cannot insert NULL
                        {
                            errorMessage += " - Không thể chèn giá trị NULL vào cột không cho phép NULL.";
                        }
                    }
                }
                
                TempData["Error"] = $"Lỗi khi lưu dữ liệu: {errorMessage}";
                await LoadDropdownListsAsync(model.maphong, model.makk, model.machucvu, model.macapbac, model.matt, model.mapx, model.idloaihinhkcb, model.mahtd);
                return View(model);
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    errorMessage += $" | Chi tiết: {ex.InnerException.Message}";
                }
                TempData["Error"] = $"Lỗi khi lưu dữ liệu: {errorMessage}";
                await LoadDropdownListsAsync(model.maphong, model.makk, model.machucvu, model.macapbac, model.matt, model.mapx, model.idloaihinhkcb, model.mahtd);
                return View(model);
            }
            TempData["Success"] = "Cập nhật thông tin thành công!";
            return RedirectToAction("Index");
        }

        private async Task LoadDropdownListsAsync(int? maphong = null, int? makk = null, int? machucvu = null, int? macapbac = null, int? matt = null, int? mapx = null, int? idloaihinhkcb = null, int? mahtd = null)
        {
            var phuongTask = _cache.GetPhuongListAsync();
            var phongTask = _cache.GetPhongListAsync();
            var khoaTask = _cache.GetKhoaListAsync();
            var chucvuTask = _cache.GetChucVuListAsync();
            var capbacTask = _cache.GetCapBacListAsync();
            var tinhTask = _cache.GetTinhListAsync();
            var loaiHinhTask = _cache.GetLoaiHinhListAsync();
            var hinhThucTask = _cache.GetHinhThucListAsync();

            await Task.WhenAll(phuongTask, phongTask, khoaTask, chucvuTask, capbacTask, tinhTask, loaiHinhTask, hinhThucTask);

            ViewBag.PhongList = new SelectList(phongTask.Result, "maphong", "tenphong", maphong);
            ViewBag.KhoaList = new SelectList(khoaTask.Result, "makk", "tenkk", makk);
            ViewBag.ChucVuList = new SelectList(chucvuTask.Result, "machucvu", "tenchucvu", machucvu);
            ViewBag.CapBacList = new SelectList(capbacTask.Result, "macapbac", "tencapbac", macapbac);
            ViewBag.TinhList = new SelectList(tinhTask.Result, "matt", "tentinh", matt);
            ViewBag.PhuongList = new SelectList(phuongTask.Result, "mapx", "tenxa", mapx);
            ViewBag.LoaiHinhList = new SelectList(loaiHinhTask.Result, "idloaihinhkcb", "diengiai", idloaihinhkcb);
            ViewBag.HinhThucList = new SelectList(hinhThucTask.Result, "mahtd", "tenhtd", mahtd);
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