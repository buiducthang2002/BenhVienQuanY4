using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace APP.Models
{
    [Table("dmhinhthucdenkham")]
    public class DmHinhthucdenkham
	{
		public int mahtd { set; get; }
		public string tenhtd { set; get; }
		
	}
}

