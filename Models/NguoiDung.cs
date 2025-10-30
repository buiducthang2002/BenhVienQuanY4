using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace APP.Models
{
    [Table("nguoidung")]
    public class NguoiDung
    {
        [Key]
        public int nguoidungid { get; set; }

        [Required, MaxLength(100)]
        public string tendangnhap { get; set; }

        [Required]
        public string matkhau { get; set; }
    }
}