		
using System;
using System.ComponentModel.DataAnnotations;

namespace APP.Models
{
	public class LoginViewModel
    
	{
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        [Display(Name = "Tên đăng nhập")]
        public string tendangnhap { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string matkhau { get; set; }

        [Display(Name = "Giữ đăng nhập?")]
        public bool RememberMe { get; set; }
    }
}