using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APP.Models
{
    [Table("donthuoc")]
    public class DonThuoc
    {
        public string? makcb { get; set; }
        [Key]
        public int madonthuoc { get; set; }
        public DateTime? ngay { get; set; }
        public DateTime? ngayduyet { get; set; }
        public string? sophieu { get; set; }
    }
}

