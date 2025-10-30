using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APP.Models
{
	public class CanLamSang
	{
        public int mahh { get; set; }
        public string? tendichvu { get; set; }
        public string? makcb { get; set; }
        public string? hoten { get; set; }
        public string? barcode { get; set; }
        public int? manvlam { get; set; }
        public string? ketluan { get; set; }
        public string? daky { get; set; }        
    }
}

