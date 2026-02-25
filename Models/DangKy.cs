using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APP.Models
{
    [Table("dangky")]
    public class DangKy
    {
        [Key]
        public string? makcb { get; set; }
        public string? hoten { get; set; }
        public DateTime? ngaysinh { get; set; }
        public int? maphai { get; set; }
        public string? socmnd { get; set; }
        public int manghenghiep { get; set; }
        public int madoituong { get; set; }
        

        public int? maphong { get; set; }
        public int? makk { get; set; }
        public int? manv { get; set; }
        public int? machucvu { get; set; }
        public int? macapbac { get; set; }
        public int? madonvi { get; set; }

        public string? noilamviec { get; set; }
        public string? sonha { get; set; }
        public string? thonpho { get; set; }
        public int? mapx { get; set; }
        public int? maqh { get; set; }
        public int? matt { get; set; }
        public string? tenxa { get; set; }

        public int? idloaihinhkcb { get; set; }
     

        public string? lydovv130 { get; set; }
        public string? dienthoai { get; set; }
        public string? sobhxh { get; set; }

        [Column("mahinhthucden")] 
        public int? mahtd { get; set; }  

    }
}