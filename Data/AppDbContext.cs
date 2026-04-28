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




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<DangKy>(entity =>
            {
                entity.ToTable("dangky");
                entity.HasKey(d => d.makcb);
                // Ignore tenxa vì bảng dangky không có cột này, chỉ có mapx (foreign key)
                entity.Ignore(e => e.tenxa);


            });



            modelBuilder.Entity<KySo>(entity =>
            {
                entity.ToTable("dangkyravien");
                entity.HasKey(k => k.makcb);
                entity.Property(k => k.makcb).HasMaxLength(50);
            });

            modelBuilder.Entity<ChuyenVien>(entity =>
            {
                entity.ToTable("dangkychuyenvien");
                entity.HasKey(c => c.makcb);
                entity.Property(c => c.makcb).HasMaxLength(50);
            });

            modelBuilder.Entity<DmCapbac>().HasNoKey();
            modelBuilder.Entity<DmChucvu>().HasNoKey();
            modelBuilder.Entity<DmKhoa>().HasNoKey();
            // DmPhong có khóa chính là maphong
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

            modelBuilder.Entity<DmPhuongxa>(entity =>
            {
                entity.ToTable("dmphuongxa");
                entity.HasNoKey();
                entity.Property(e => e.mapx).HasColumnName("mapx");
                entity.Property(e => e.tenxa).HasColumnName("tenxa");
            });


            modelBuilder.Entity<PhauThuatThuThuat>()
                .HasKey(p => new { p.makcb, p.maphauthuat });


            modelBuilder.Entity<KetQuaCLS>()
                .HasKey(k => new { k.makcb, k.mahh });

            modelBuilder.Entity<ThanhToan>(entity =>
            {
                entity.ToTable("thanhtoan");
                entity.HasKey(t => t.mathanhtoan);
            });

            modelBuilder.Entity<ThanhToanCT>(entity =>
            {
                entity.ToTable("thanhtoanct");
                entity.HasKey(tc => tc.mathanhtoanct);
                entity.Property(tc => tc.thanhtien).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<ThuChi>(entity =>
            {
                entity.ToTable("thuchi");
                entity.HasKey(t => t.mathuchi);
            });

            modelBuilder.Entity<ThuChiCT>(entity =>
            {
                entity.ToTable("thuchict");
                entity.HasKey(tc => tc.mathuchict);
                entity.Property(tc => tc.dongia).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<DonThuoc>(entity =>
            {
                entity.ToTable("donthuoc");
                entity.HasKey(d => d.madonthuoc);
                entity.Property(d => d.madonthuoc).ValueGeneratedNever();
            });

            modelBuilder.Entity<DonThuocCT>(entity =>
            {
                entity.ToTable("donthuocct");
                entity.HasKey(d => new { d.madonthuoc, d.madonthuocct });
                entity.Property(d => d.dongia).HasColumnType("decimal(18,2)");
                entity.Property(d => d.thanhtien).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<BanLamViecKhamBenh>(entity =>
            {
                entity.ToTable("khambenh");
                entity.HasKey(b => b.makcb);
            });

            modelBuilder.Entity<HoiChan>(entity =>
            {
                entity.ToTable("hoichan");
                entity.HasKey(h => h.makcb);
            });
        }

        public DbSet<NguoiDung> NguoiDung { get; set; } = null!;
        public DbSet<BenhAn> BenhAn { get; set; } = null!;
        public DbSet<DMDichVu> DMDichVu { get; set; } = null!;
        public DbSet<KetQuaCLS> KetQuaCLS { get; set; } = null!;
        public DbSet<PhauThuatThuThuat> PhauThuatThuThuat { get; set; } = null!;
        public DbSet<DangKy> DangKy { get; set; } = null!;

        public DbSet<BanLamViecKhamBenh> BanLamViecKhamBenhs { get; set; } = null!;
        public DbSet<HoiChan> HoiChan { get; set; } = null!;
        public DbSet<KySo> KySo { get; set; } = null!;

        // Bảng nghiệp vụ
        public DbSet<ChuyenVien> ChuyenVien { get; set; } = null!;
        public DbSet<ChuyenKhoa> chuyenkhoa { get; set; } = null!;
        public DbSet<ThanhToan> ThanhToan { get; set; } = null!;
        public DbSet<ThanhToanCT> ThanhToanCT { get; set; } = null!;
        public DbSet<ThuChi> ThuChi { get; set; } = null!;
        public DbSet<ThuChiCT> ThuChiCT { get; set; } = null!;
        public DbSet<DonThuoc> DonThuoc { get; set; } = null!;
        public DbSet<DonThuocCT> DonThuocCT { get; set; } = null!;


        // Danh mục
        public DbSet<DmKhoa> DmKhoa { get; set; } = null!;
        public DbSet<DmChucvu> DmChucvu { get; set; } = null!;
        public DbSet<DmCapbac> DmCapbac { get; set; } = null!;
        public DbSet<DmPhuongxa> DmPhuongxa { get; set; } = null!;
        public DbSet<DmTt> DmTt { get; set; } = null!;
        public DbSet<DmDangkyloaihinhkcb> DmDangkyloaihinhkcb { get; set; } = null!;
        public DbSet<DmHinhthucdenkham> DmHinhthucdenkham { get; set; } = null!;
        public DbSet<DmPhong> DmPhong { get; set; } = null!;

    }
}

