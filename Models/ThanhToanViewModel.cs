using System;

namespace APP.Models
{
    /// <summary>
    /// ViewModel để hiển thị kết quả JOIN giữa thanhtoan và thanhtoanct
    /// </summary>
    public class ThanhToanViewModel
    {
        // Thông tin từ bảng thanhtoan (header)
        public string? sophieu { get; set; }
        public string? makcb { get; set; }
        public DateTime? ngay { get; set; }
        public int? mathanhtoan { get; set; }

        // Thông tin từ bảng thanhtoanct (detail)
        public long? mathanhtoanct { get; set; }
        public decimal? thanhtien { get; set; }
    }
}
