using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MiniF4Store.Models
{
    public class OTP
    {
        [Required]
        public string maOTP { get; set; }
    }
}