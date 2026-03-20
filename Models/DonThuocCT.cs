using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APP.Models
{
    [Table("donthuocct")]
    public class DonThuocCT
    {
        [Key]
        public int madonthuoc { get; set; }
        public long madonthuocct { get; set; }
        public decimal soluong { get; set; }
        public decimal dongia { get; set; }
        public decimal thanhtien { get; set; }
    }
}
