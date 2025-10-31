using System;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APP.Models
{
    [Table("phauthuat")]
    public class PhauThuatThuThuat
    {
        [Key, Column(Order = 0)]
        public string makcb { get; set; } = string.Empty;

        [NotMapped]
        public string? hoten { get; set; }
        [NotMapped]
        public string? tenkk { get; set; }
        [NotMapped]
        public string? tenphong { get; set; }

        [Key, Column(Order = 1)]
        public long maphauthuat { get; set; }
        public string? bac { get; set; }
        public int makk{get;set;}
        public int maphong{get;set;}
        public int mathanhtoan{get;set;}
        

        

        [Column("ngaybatdaumo")]
        public DateTime? ngaybatdaumo { get; set; }

        [Column("ngayketthucmo")]
        public DateTime? ngayketthucmo { get; set; }

        [Column("chandoantruocmo", TypeName = "nvarchar(1000)")]
        public string? chandoantruocmo { get; set; }

        [Column("chandoansaumo", TypeName = "nvarchar(1000)")]
        public string? chandoansaumo { get; set; }
        public string? ailam { get; set; }
        [Column("dienbien", TypeName = "nvarchar(1000)")]
        public string? dienbien{get;set;}

      

        [Column("daky", TypeName = "nvarchar(max)")]
        public string? daky { get; set; }

        
    }
}

