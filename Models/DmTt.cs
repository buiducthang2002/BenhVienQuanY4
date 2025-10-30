using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace APP.Models
{
    [Table("dmtinhthanh")]
    public class DmTt
	{
        public int? matt { get; set; }
        public string tentinh { get; set; }
    }
}

