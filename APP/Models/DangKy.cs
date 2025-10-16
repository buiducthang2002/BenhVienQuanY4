using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APP.Models
{
    [Table("dangky")]
    public class DangKy
	{
        [Key]
        [Column("makcb")]
        [StringLength(50)]
        public string makcb { get; set; } = string.Empty;

        [Column("ngaydk")]
        public DateTime? ngaydk { get; set; }

        [Column("hoten")]
        [StringLength(255)]
        public string? hoten { get; set; }

        [Column("ngaysinh")]
        public DateTime? ngaysinh { get; set; }

        [Column("maphai")]
        public int? maphai { get; set; }

        [Column("socmnd")]
        [StringLength(50)]
        public string? socmnd { get; set; }

        [Column("maphong")]
        public int? maphong { get; set; }

        [Column("makk")]
        public int? makk { get; set; }

        [Column("manv")]
        public int? manv { get; set; }

        [Column("machucvu")]
        public int? machucvu { get; set; }
        public int? macapbac { get; set; }                  
        public int? madonvi { get; set; }
        [StringLength(250)]
        public string noilamviec { get; set; }              
        [StringLength(100)]
        public string sonha { get; set; }                   

        [StringLength(150)]
        public string thonpho { get; set; }                 

        public int? mapx { get; set; }                      
        public int? maqh { get; set; }                      
        public int? matt { get; set; }
        public int? idloaihinhkcb { get; set; }             
        public int? mahinhthucden { get; set; }            

        [StringLength(500)]
        public string lydovv130 { get; set; }

        [Column("dienthoai")]
        [StringLength(20)]
        public string? dienthoai { get; set; }

        [Column("sobhxh")]
        [StringLength(50)]
        public string? sobhxh { get; set; }

    }
}

