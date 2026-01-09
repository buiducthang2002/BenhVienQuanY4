using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APP.Models
{
    [Table("thanhtoan")]
    public class ThanhToan
    {
        [Key]
        public int ct { get; set; }
        public string? mathanhtoan { get; set; }
        public long makcb { get; set; }              // ĐỔI từ string → long
        public string? sophieu { get; set; }
        public DateTime? ngay { get; set; }
        public string? manv { get; set; }
        public string? madieutri { get; set; }
        public string? makhambenh { get; set; }
        public int? makk { get; set; }
        public int? maphong { get; set; }
        public decimal? thanhtoanke { get; set; }
        public decimal? chiphicu { get; set; }
        public string? ngoaigio { get; set; }
        public string? bosung { get; set; }
        public string? capcuu { get; set; }
        public string? ylenhcapcuu { get; set; }
        public string? ylenhkhac { get; set; }
        public string? mactpt { get; set; }
        public string? ailam { get; set; }
        public string? barcode { get; set; }
        public string? saudieutri { get; set; }
        public string? macu { get; set; }
        public string? ttchidinh { get; set; }
        public string? dangkyke { get; set; }
        public string? mattlq { get; set; }
        public string? khongindienbien { get; set; }
        public string? ghichu { get; set; }
    }
}

