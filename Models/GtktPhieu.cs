using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APP.Models
{
    [Table("gtkt_danhsachphieutheobn")]
    public class GtktPhieu
    {
        [StringLength(50)]
        public string sophieu { get; set; } = string.Empty;

        [StringLength(15)]
        public string makcb { get; set; } = string.Empty;

        [StringLength(100)]
        public string mauphieu { get; set; } = string.Empty;

        public DateTime ngay { get; set; }

        public string? daky { get; set; }
    }
}
