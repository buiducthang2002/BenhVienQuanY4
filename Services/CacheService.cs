using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using APP.Data;
using APP.Models;

namespace APP.Services
{
    public class CacheService
    {
        private readonly IMemoryCache _cache;
        private readonly AppDbContext _context;
        private readonly ILogger<CacheService> _logger;
        private const int CACHE_DURATION_MINUTES = 30;

        public CacheService(IMemoryCache cache, AppDbContext context, ILogger<CacheService> logger)
        {
            _cache = cache;
            _context = context;
            _logger = logger;
        }

        public async Task<List<DmPhong>> GetPhongListAsync()
        {
            return await _cache.GetOrCreateAsync("PhongList", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CACHE_DURATION_MINUTES);
                _logger.LogInformation("Loading PhongList from database...");
                return await _context.DmPhong.AsNoTracking().ToListAsync();
            }) ?? new List<DmPhong>();
        }

        public async Task<List<DmKhoa>> GetKhoaListAsync()
        {
            return await _cache.GetOrCreateAsync("KhoaList", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CACHE_DURATION_MINUTES);
                _logger.LogInformation("Loading KhoaList from database...");
                return await _context.DmKhoa.AsNoTracking().ToListAsync();
            }) ?? new List<DmKhoa>();
        }

        public async Task<List<DmChucvu>> GetChucVuListAsync()
        {
            return await _cache.GetOrCreateAsync("ChucVuList", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CACHE_DURATION_MINUTES);
                _logger.LogInformation("Loading ChucVuList from database...");
                return await _context.DmChucvu.AsNoTracking().ToListAsync();
            }) ?? new List<DmChucvu>();
        }

        public async Task<List<DmCapbac>> GetCapBacListAsync()
        {
            return await _cache.GetOrCreateAsync("CapBacList", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CACHE_DURATION_MINUTES);
                _logger.LogInformation("Loading CapBacList from database...");
                return await _context.DmCapbac.AsNoTracking().ToListAsync();
            }) ?? new List<DmCapbac>();
        }

        public async Task<List<DmTt>> GetTinhListAsync()
        {
            return await _cache.GetOrCreateAsync("TinhList", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CACHE_DURATION_MINUTES);
                _logger.LogInformation("Loading TinhList from database...");
                return await _context.DmTt.AsNoTracking().ToListAsync();
            }) ?? new List<DmTt>();
        }

        public async Task<List<DmPhuongxa>> GetPhuongListAsync()
        {
            return await _cache.GetOrCreateAsync("PhuongList", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CACHE_DURATION_MINUTES);
                _logger.LogInformation("Loading PhuongList from database...");
                return await _context.DmPhuongxa
                    .AsNoTracking()
                    .Where(p => p.tenxa != null && !string.IsNullOrWhiteSpace(p.tenxa))
                    .ToListAsync();
            }) ?? new List<DmPhuongxa>();
        }

        public async Task<List<DmDangkyloaihinhkcb>> GetLoaiHinhListAsync()
        {
            return await _cache.GetOrCreateAsync("LoaiHinhList", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CACHE_DURATION_MINUTES);
                _logger.LogInformation("Loading LoaiHinhList from database...");
                return await _context.DmDangkyloaihinhkcb.AsNoTracking().ToListAsync();
            }) ?? new List<DmDangkyloaihinhkcb>();
        }

        public async Task<List<DmHinhthucdenkham>> GetHinhThucListAsync()
        {
            return await _cache.GetOrCreateAsync("HinhThucList", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CACHE_DURATION_MINUTES);
                _logger.LogInformation("Loading HinhThucList from database...");
                return await _context.DmHinhthucdenkham.AsNoTracking().ToListAsync();
            }) ?? new List<DmHinhthucdenkham>();
        }

        public void ClearCache()
        {
            var keys = new[] { "PhongList", "KhoaList", "ChucVuList", "CapBacList", "TinhList", "PhuongList", "LoaiHinhList", "HinhThucList" };
            foreach (var key in keys)
            {
                _cache.Remove(key);
            }
            _logger.LogInformation("All lookup caches cleared.");
        }
    }
}

