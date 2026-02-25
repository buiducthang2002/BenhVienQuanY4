using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APP.Models
{
    [Table("thuchict")]
    public class ThuChiCT
    {
        [Key]
        [Column("mathuchict")]
        public long mathuchict { get; set; }

        [Column("mathuchi")]
        public int mathuchi { get; set; }

        [Column("mathanhtoan")]
        public int mathanhtoan { get; set; }

        [Column("mathanhtoanct")]
        public long mathanhtoanct { get; set; }

        [Column("dongia")]
        public decimal dongia { get; set; }
    }
}
