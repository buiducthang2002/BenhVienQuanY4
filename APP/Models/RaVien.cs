using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APP.Models
{
    [Table("dangkyravien")]
    public class RaVien
    {
        [Key]
        [Column("makcb")]
        [StringLength(50)]
        public string? makcb { get; set; }

        public DateTime? ngay { get; set; }

        public int? makk { get; set; }
        public int? makq { get; set; }
        public int? mahtr { get; set; }
        public double? songaydieutri { get; set; }
       [Column(TypeName = "nvarchar(max)")]
        public string? phuongphapdieutri { get; set; }
        [Column(TypeName = "nvarchar(255)")]
        public string? ailam { get; set; }
        [Column(TypeName = "nvarchar(50)")]
        public string? soluutru { get; set; }
        [Column(TypeName = "nvarchar(50)")]
        public string? soluot { get; set; }
        public int? maphong { get; set; }
        public int? manv { get; set; }
        public int? ct { get; set; }
        public double? songaydieutrint { get; set; }
        public int? macachdieutri { get; set; }
        [Column(TypeName = "nvarchar(max)")]
        public string? nhanxet { get; set; }
        [Column(TypeName = "nvarchar(max)")]
        public string? ghichu { get; set; }
        [Column(TypeName = "nvarchar(max)")]
        public string? ykiendenghi { get; set; }
        public int? mabs { get; set; }
        [Column(TypeName = "nvarchar(20)")]
        public string? sodienthoai { get; set; }
        [Column(TypeName = "nvarchar(255)")]
        public string? chanbenhicd { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string? bhxh { get; set; }

        public int? thutruong { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? quatrinhbenhly { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? ketquacls { get; set; }

        public bool? ketthucdtnt { get; set; }

        public int? madieutridn { get; set; }
        public int? madonnguyen { get; set; }
        public int? maphongcuoi { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? daky { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? chuoinguoiky { get; set; }

        public double? songay285 { get; set; }
        public int? maailam { get; set; }
        public int? truongkhoa { get; set; }
        public int? manvkybangke { get; set; }
        public int? loaihinhkcb { get; set; }
        public int? songaynghi { get; set; }
        public int? iddoituongkcb { get; set; }
        

        [Column(TypeName = "nvarchar(max)")]
        public string? lydodanhgia { get; set; }

       

        [Column(TypeName = "nvarchar(50)")]
        public string? soravien { get; set; }
    }
}