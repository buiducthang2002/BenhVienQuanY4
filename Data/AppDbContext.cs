using APP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;

namespace APP.Data
{
    public class AppDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_httpContextAccessor.HttpContext != null)
            {
                var sessionConn = _httpContextAccessor.HttpContext.Session.GetString("DynamicConnection");
                if (!string.IsNullOrEmpty(sessionConn))
                {
                    optionsBuilder.UseSqlServer(sessionConn);
                    return;
                }
            }
            optionsBuilder.UseSqlServer("Server=192.168.0.26;Database=QY42026V6;User Id=sa;Password=123@vtt;TrustServerCertificate=True;");
        }
        public DbSet<NguoiDung> NguoiDung { get; set; }
        public DbSet<BenhAn> BenhAn { get; set; }
        public DbSet<RaVien> RaVien { get; set; }
        public DbSet<DMDichVu> DMDichVu { get; set; }
        public DbSet<KetQuaCLS> KetQuaCLS { get; set; }
    }
}