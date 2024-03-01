using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MiniF4Store.Models
{
    public class DangKyModel
    {
        [StringLength(50)]
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email bắt buộc nhập")]
        [DataType(DataType.EmailAddress, ErrorMessage = "E-mail sai định dạng")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu bắt buộc nhập")]
        [StringLength(maximumLength: 20, MinimumLength = 6, ErrorMessage = "Độ dài mật khẩu từ 6-20 kí tự")]
        [Display(Name = "Mật khẩu")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Mật khẩu bắt buộc nhập")]
        [Compare("Password", ErrorMessage = "Nhập lại mật khẩu không trùng với mật khẩu")]
        [Display(Name = "Xác nhận mật khẩu")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Họ tên bắt buộc nhập")]
        [StringLength(50)]
        [Display(Name = "Họ tên")]
        public string Name { get; set; }

        [StringLength(100)]
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Điện thoại bắt buộc nhập")]
        [StringLength(10,ErrorMessage ="Số điện thoại không hợp lệ")]
        [Display(Name = "Điện thoại")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Số điện thoại nhận hàng bắt buộc nhập")]
        [Display(Name="Số điện thoại")]
        public string SDT_NHANHANG { get; set; }

        [Required(ErrorMessage = "Địa chỉ nhận hàng bắt buộc nhập")]
        [Display(Name = "Địa chỉ nhận hàng")]
        public string DIACHI_NHANHANG { get; set; }

        [Required(ErrorMessage = "Họ tên người nhận hàng bắt buộc nhập")]
        [Display(Name = "Người nhận hàng")]
        public string TEN_NHANHANG { get; set; }
        public string To { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        //[Display(Name = "Mã OTP")]
        //public string OTP { get; set; }
    }
}