using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APP.Models
{
    [Table("dangkychuyenvien")]
    public class ChuyenVien
	{
        [Key]
        [StringLength(50)]
       public string makcb { get; set; } = string.Empty;
       public DateTime? ngay { get; set; }
       public string? ailam { get; set; }
       public string? daky { get; set; }
    }
}
    
