using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace APP.Models
    
{
    [Table("dmphong")]
	public class DmPhong
	{
        [Column("maphong")]
        public int? maphong { get; set; }
        public string? tenphong { get; set; }
    }
}

