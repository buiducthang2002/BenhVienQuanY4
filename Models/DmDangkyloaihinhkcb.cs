using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace APP.Models
{
	[Table("dangky_loaihinhkcb")]
	public class DmDangkyloaihinhkcb
	{
		public int idloaihinhkcb { set; get; }
		public string diengiai { set; get; }
	}
}

