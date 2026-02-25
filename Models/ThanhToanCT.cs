using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APP.Models
{
    [Table("thanhtoanct")]
    public class ThanhToanCT
    {
        [Key]
        [Column("mathanhtoanct")]
        public long mathanhtoanct { get; set; }

        [Column("mathanhtoan")]
        public int? mathanhtoan { get; set; }

        [Column("thanhtien")]
        public decimal? thanhtien { get; set; }
    }
}
