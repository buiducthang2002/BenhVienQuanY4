using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APP.Models
{
    [Table("thuchi")]
    public class ThuChi
    {
        [Key]
        [Column("mathuchi")]
        public int mathuchi { get; set; }

        [Column("makcb")]
        [StringLength(15)]
        public string makcb { get; set; }

        [Column("sophieu")]
        [StringLength(15)]
        public string sophieu { get; set; }

        [Column("ngay")]
        public DateTime ngay { get; set; }
    }
}
