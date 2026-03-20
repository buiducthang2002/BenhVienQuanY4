using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APP.Models
{
    [Table("donthuoc")]
    public class DonThuoc
    {
        [Key]
        public string? makcb { get; set; }
        public int madonthuoc { get; set; }
    }
}

