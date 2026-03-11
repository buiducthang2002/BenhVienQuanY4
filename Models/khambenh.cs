using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APP.Models
{
    [Table("khambenh")]
    public class khambenh
	{
        [Key]
        public string? makcb { get; set; }
        public int? maphong { get; set; }
        public int? makk { get; set; }
    
    }
}
    
