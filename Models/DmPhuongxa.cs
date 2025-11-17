using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace APP.Models
{
    [Table("dmphuongxa")]
    public class DmPhuongxa
	{
        public int? mapx { get; set; }
        public string tenxa { set; get; } 
		
	}
}

