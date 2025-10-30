using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APP.Models
{
    [Table("dmdichvu")]
    public class DMDichVu
	{
        [Key]
        public int mahh { get; set; }

        [Column(TypeName = "nvarchar(255)")]
        public string? tendichvu { get; set; }
    }
}

