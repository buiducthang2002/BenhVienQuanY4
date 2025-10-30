using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APP.Models
{
    [Table("ketquacls")]
    public class KetQuaCLS
	{
         [Key]
        public string? makcb { get; set; }
        public string? barcode { get; set; }
        public int mahh { get; set; } 
        public int? manvlam { get; set; }
        public string? ketluan { get; set; }
        public string? daky { get; set; }
    }
}

