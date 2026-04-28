using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APP.Models
{
    [Table("hoichan")]
    public class HoiChan
    {
        [Key]
        [StringLength(15)]
        [Display(Name = "Mã KCB")]
        public string makcb { get; set; } = null!;

        [Display(Name = "Mã điều trị")]
        public long? madieutri { get; set; }

        [Required]
        [Display(Name = "Ngày hội chẩn")]
        public DateTime ngay { get; set; }

        [Display(Name = "Chủ tọa")]
        public int? chutoa { get; set; }

        [Display(Name = "Thư ký")]
        public int? thuky { get; set; }

        [Required]
        [StringLength(500)]
        [Display(Name = "Thành viên")]
        public string thanhvien { get; set; } = null!;

        [StringLength(15)]
        [Display(Name = "Số phiếu")]
        public string? sophieu { get; set; }

        [Display(Name = "Đã ký")]
        public string? daky { get; set; }
    }
}