using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APP.Models
{
    public class BanLamViecKhamBenh
    {
        [Key]
        [StringLength(15)]
        [Display(Name = "Mã KCB")]
        public string makcb { get; set; } = null!;

        [Display(Name = "Mã khu khám")]
        public int? makk { get; set; }

        [Display(Name = "Mã phòng")]
        public int? maphong { get; set; }

        [Display(Name = "Mã nhân viên")]
        public int? manv { get; set; }

        [Display(Name = "Ngày khám")]
        public DateTime? ngay { get; set; }

        [Display(Name = "Đã ký")]
        public string? daky { get; set; }
    }
}