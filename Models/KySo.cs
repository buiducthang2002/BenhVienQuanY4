using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APP.Models
{
    [Table("dangkyravien")]
    public class KySo
	{
        [Key]
        [StringLength(50)]
        public string makcb { get; set; } = string.Empty;
       public DateTime? ngay { get; set; }
       public int? makk { get; set; }
      
       public double? songaydieutri { get; set; }
       public string? phuongphapdieutri { get; set; }
       public string? ailam { get; set; }
       public string? daky { get; set; }
    }
}
    
