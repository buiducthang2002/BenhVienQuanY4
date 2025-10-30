
using System;
using APP.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace APP.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.25")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("APP.Models.BenhAn", b =>
                {
                    b.Property<int>("makcb")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("makcb"), 1L, 1);

                    b.Property<string>("bacsy")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("daky")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("manv")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("manvtongket")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("maubenhan")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ngaylam")
                        .HasColumnType("datetime2");

                    b.Property<string>("ngaytongket")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("sobenhan")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("makcb");

                    b.ToTable("BenhAn");
                });
#pragma warning restore 612, 618
        }
    }
}
