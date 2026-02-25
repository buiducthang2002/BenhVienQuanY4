using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APP.Models
{
    [Table("chuyenkhoa")]
    public class ChuyenKhoa
    {
        [Key]
                   
        public string makcb { get; set; } = string.Empty;  // mã KCB (string)
       

        // Navigation Properties
       
        public virtual DmPhong? PhongGoc { get; set; }

     
        public virtual DmPhong? PhongChuyen { get; set; }
    }
}

