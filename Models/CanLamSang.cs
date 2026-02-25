using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APP.Models
{
	public class CanLamSang
	{
        public int mahh { get; set; }
        public string? tendichvu { get; set; }
        public string? makcb { get; set; }
        public string? hoten { get; set; }
        public string? barcode { get; set; }
        public int? manvlam { get; set; }
        public string? ketluan { get; set; }
        public string? daky { get; set; }
        public int mathanhtoan { get; set; }
        public bool? daduyet { get; set; }
        public DateTime? ngaylam { get; set; }
        public DateTime? ngaykhopBC { get; set; }
        public DateTime? ngaytraKQ { get; set; }
        public string? ailam { get; set; }
        public bool datrakq { get; set; }
        public DateTime? ngayth { get; set; }
        public DateTime? ngayketquaLIS { get; set; }
    }
}

