using System;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APP.Models
{
    [Table("phauthuat")]
    public class PhauThuatThuThuat
    {

        public string makcb { get; set; } = string.Empty;
 
        public long maphauthuat { get; set; }

        

        [Column("ngaybatdaumo")]
        public DateTime? ngaybatdaumo { get; set; }

        [Column("ngayketthucmo")]
        public DateTime? ngayketthucmo { get; set; }

        [Column("chandoantruocmo", TypeName = "nvarchar(1000)")]
        public string? chandoantruocmo { get; set; }

        [Column("chandoansaumo", TypeName = "nvarchar(1000)")]
        public string? chandoansaumo { get; set; }
        public string? ailam { get; set; }

       
        [Column("bac", TypeName = "nvarchar(100)")]
        public string? bac { get; set; }

      

        [Column("daky", TypeName = "nvarchar(max)")]
        public string? daky { get; set; }

        
    }
}

