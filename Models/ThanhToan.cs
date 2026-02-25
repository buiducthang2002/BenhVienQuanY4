using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APP.Models
{
    [Table("thanhtoan")]
    public class ThanhToan
    {
        [Key]
        [Column("mathanhtoan")]
        public int mathanhtoan { get; set; }

        [Column("makcb")]
        [StringLength(15)]
        public string makcb { get; set; }

        [Column("sophieu")]
        [StringLength(50)]
        public string sophieu { get; set; }

        [Column("ngay")]
        public DateTime ngay { get; set; }

        [Column("manv")]
        public int? manv { get; set; }

        [Column("madieutri")]
        public long? madieutri { get; set; }

        [Column("makhambenh")]
        public long? makhambenh { get; set; }

        [Column("makk")]
        public int? makk { get; set; }

        [Column("maphong")]
        public int? maphong { get; set; }

        [Column("ailam")]
        [StringLength(500)]
        public string? ailam { get; set; }

        [Column("songaydungdon")]
        public int songaydungdon { get; set; }

        [Column("daky")]
        public string? daky { get; set; }

        [Column("maailam")]
        public int? maailam { get; set; }

        [Column("ngaythyl")]
        public DateTime? ngaythyl { get; set; }
    }
}
