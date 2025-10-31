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
            optionsBuilder.UseSqlServer("Server=171.254.95.46,11433;Database=TEST2025V6;User Id=sa;Password=123@vtt;TrustServerCertificate=True;");
        }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<DangKy>()
                .ToTable("dangky")
                .HasKey(d => d.makcb);

                

            modelBuilder.Entity<KySo>(entity =>
            {
                entity.ToTable("dangkyravien");
                entity.HasKey(k => k.makcb);
                entity.Property(k => k.makcb).HasMaxLength(50);
            });
                
            modelBuilder.Entity<DmCapbac>().HasNoKey();
            modelBuilder.Entity<DmChucvu>().HasNoKey();
            modelBuilder.Entity<DmKhoa>().HasNoKey();
            modelBuilder.Entity<DmPhong>().HasNoKey();
            modelBuilder.Entity<DmPhuongxa>().HasNoKey();
            modelBuilder.Entity<DmTt>().HasNoKey();
            modelBuilder.Entity<DmDangkyloaihinhkcb>().HasNoKey();
            modelBuilder.Entity<DmHinhthucdenkham>().HasNoKey();
            modelBuilder.Entity<DmKhoa>(entity =>
            {
                entity.ToTable("dmkk");
                entity.HasNoKey();
                entity.Property(e => e.makk).HasColumnName("makk");
                entity.Property(e => e.tenkk).HasColumnName("tenkk");
                entity.Ignore(e => e.maphong); 
            });


            modelBuilder.Entity<PhauThuatThuThuat>()
                .HasKey(p => new { p.makcb, p.maphauthuat });


            modelBuilder.Entity<KetQuaCLS>()
                .HasKey(k => new { k.makcb, k.mahh });
        }

        public DbSet<NguoiDung> NguoiDung { get; set; }
        public DbSet<BenhAn> BenhAn { get; set; }
        public DbSet<DMDichVu> DMDichVu { get; set; }
        public DbSet<KetQuaCLS> KetQuaCLS { get; set; }
        public DbSet<PhauThuatThuThuat> PhauThuatThuThuat { get; set; }
        public DbSet<DangKy> DangKy { get; set; }
        public DbSet<KySo> KySo { get; set; }
        
        public DbSet<DmKhoa> DmKhoa { get; set; }
        public DbSet<DmChucvu> DmChucvu { get; set; }
        public DbSet<DmCapbac> DmCapbac { get; set; }
        public DbSet<DmPhuongxa> DmPhuongxa { get; set; }
        public DbSet<DmTt> DmTt { get; set; }
        public DbSet<DmDangkyloaihinhkcb> DmDangkyloaihinhkcb { get; set; }
        public DbSet<DmHinhthucdenkham> DmHinhthucdenkham { get; set; }
        public DbSet<DmPhong> DmPhong { get; set; }

    }
}

