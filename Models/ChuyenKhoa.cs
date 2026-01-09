using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APP.Models
{
    [Table("chuyenkhoa")]
    public class ChuyenKhoa
    {
        [Key]
        public int ct { get; set; }
        public int? machuyen { get; set; }            // mã chuyển (số)
        public long makcb { get; set; }               // mã KCB (bigint)
        public string? madieutri { get; set; }
        public int? madtchuyen { get; set; }          // mã điều trị chuyển (số)
        public int? makkc { get; set; }              // Phòng chuyển đến
        public int? matiepnhan { get; set; }          // mã tiếp nhận (số)
        public int? makk { get; set; }               // Phòng gốc
        public int? malydo { get; set; }              // lý do (số)
        public DateTime? ngay { get; set; }
        public int? tinhtrang { get; set; }           // trạng thái (số)
        public string? ailam { get; set; }
        public int? madonnguyen { get; set; }
        public int? manv { get; set; }
        public string? daky { get; set; }
    }
}

