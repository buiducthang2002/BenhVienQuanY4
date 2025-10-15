using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APP.Models
{
    [Table("tbl_benhan_benhantheobn")]
    public class BenhAn
	{
        [Key]
        public string makcb { get; set; }
        public string maubenhan { get; set; }
        public string sobenhan { get; set; }
        public DateTime ngaylam { get; set; }
        public int manv { get; set; }
        public string? daky { get; set; }
    }
}

