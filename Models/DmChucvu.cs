using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace APP.Models
{
    [Table("dmchucvu")]
    public class DmChucvu
	{
        [Column("machucvu")]
        public int? machucvu { get; set; }
        public string tenchucvu { get; set; }
	}
}

