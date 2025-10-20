using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace APP.Models
{
	
    [Table("dmkk")]
    public class DmKhoa
	{
        [Column("maphong")]
        public int? maphong { get; set; }

        [Column("tenkk")]
        public string? tenkk { get; set; }
        [Column("makk")]           
        public int? makk { get; set; }
    }
}

