using System;

namespace APP.Models
{
    public class ThanhToanViewModel
    {
        // From thanhtoan
        public string? sophieu { get; set; }
        public string? makcb { get; set; }
        public DateTime? ngay { get; set; }
        public DateTime? ngaythyl { get; set; }
        public int? mathanhtoan { get; set; }

        // From thanhtoanct
        public long? mathanhtoanct { get; set; }
        public decimal? thanhtien { get; set; }

        // From donthuoc
        public int? madonthuoc { get; set; }
        public DateTime? dt_ngay { get; set; }
        public DateTime? ngayduyet { get; set; }
    }
}
