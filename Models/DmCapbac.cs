using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace APP.Models
{
    [Table("dmcapbac")]
    public class DmCapbac
	{
        public int? macapbac { get; set; }
        public string tencapbac { get; set; }
	}
}

