using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MiniF4Store.Models
{
    public class DangNhapModel 
    {
        [Required]
        [Display(Name = "Tài khoản")]
        public string userMail { get; set; }

        [Required]
        [Display(Name = "Mật khẩu")]
        [StringLength(maximumLength: 20, MinimumLength = 6, ErrorMessage = "Độ dài mật khẩu từ 6-20 kí tự")]
        [DataType(DataType.Password)]
        public string password { get; set; }

        //[Display(Name = "Nhớ mật khẩu")]
        //public bool RememberMe { get; set; }
    }
}