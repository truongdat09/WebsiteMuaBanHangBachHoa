using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Net.Mail;
using MiniF4Store.Models;
using System.Web.Mail;

namespace MiniF4Store.Controllers
{
    public class TaiKhoanController : Controller
    {
        QL_BachHoaDataContext db = new QL_BachHoaDataContext();
        #region Đăng Nhập Tài Khoản
        [HttpGet]
        public ActionResult DangNhap()
        {            
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult DangNhap(FormCollection userlog)
        {
            string userMail = userlog["userMail"].ToString();
            string password = userlog["password"].ToString();
            if (String.IsNullOrEmpty(userMail))
            {
                ViewData["Loi1"] = "Tài khoản không được bỏ trống!";
            }
            if (String.IsNullOrEmpty(password))
            {
                ViewData["Loi2"] = "Mật khẩu không được bỏ trống!";
            }
            if (!String.IsNullOrEmpty(userMail) && !String.IsNullOrEmpty(password))
            {                
                var admin = db.NHANVIENs.SingleOrDefault(tk => tk.TAIKHOAN.Equals(userMail) && tk.MATKHAU.Equals(password));
                var check = db.KHACHHANGs.SingleOrDefault(t => t.EMAIL.Equals(userMail) && t.MATKHAU.Equals(password));

                if (check != null)
                {
                    Session["use"] = check;
                    return RedirectToAction("Index", "Home");
                }
                else if (admin != null)
                {
                    Session["admin"] = admin;
                    return RedirectToAction("Trang_chu", "Admin");
                }
                else
                {
                    ViewBag.Fail = "Tai khoan hoac mat khau khong chinh xac!";
                    return View("DangNhap");
                }
            }
            else
            {
                ViewBag.Fail = "Vui long nhap day du thong tin!";
                return View("DangNhap");
            }
        }
        #endregion

        #region Đăng Xuất
        public ActionResult DangXuat()
        {            
            Session["use"] = null;
            Session["GioHang"] = null;
            ViewBag.TongSoLuongLoaiMatHang = 0;
            return RedirectToAction("Index", "Home");            
        }
        #endregion

        #region Đăng Ký Tài Khoản
        //Kiểm tra độ mạnh yếu của mật khẩu khi dang ky
        public static bool IsStrongPassword(string password)
        {
            if (!string.IsNullOrEmpty(password))
            {
            // Kiểm tra độ dài
            if (password.Length < 8)
            {
                return false;
            }

            // Kiểm tra xem có ít nhất một chữ cái viết hoa
            if (!password.Any(char.IsUpper))
            {
                return false;
            }

            // Kiểm tra xem có ít nhất một chữ cái viết thường
            if (!password.Any(char.IsLower))
            {
                return false;
            }

            // Kiểm tra xem có ít nhất một chữ số
            if (!password.Any(char.IsDigit))
            {
                return false;
            }

            // Kiểm tra xem có ít nhất một ký tự đặc biệt
            if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
            {
                return false;
                }

            }

            return true;
        }
        // Hàm để tạo chuỗi ngẫu nhiên
        private string GenerateRandomString(int length)
        {
            string chars = "0123456789";
            Random random = new Random();
            char[] randomString = new char[length];

            for (int i = 0; i < length; i++)
            {
                randomString[i] = chars[random.Next(chars.Length)];
            }

            return new string(randomString);
        }
        [HttpGet]
        public ActionResult DangKy()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangKy(DangKyModel dkm)
        {
            if (db.KHACHHANGs.Any(x => x.SDT_KH == dkm.PhoneNumber))
            {
                ModelState.AddModelError("PhoneNumber", "Số điện thoại này đã được đăng ký!. Vui lòng chọn số điện thoại khác.");
            }
            if (!string.IsNullOrEmpty(dkm.PhoneNumber))
            {
                if (dkm.PhoneNumber.Length != 10)
                {
                    ModelState.AddModelError("PhoneNumber", "Số điện thoại không hợp lệ!");
                }
                else if (!dkm.PhoneNumber.All(char.IsDigit))
                {
                    ModelState.AddModelError("PhoneNumber", "Đây không phải là số điện thoại!");
                }
            }
            if (!string.IsNullOrEmpty(dkm.SDT_NHANHANG))
            {
                if (dkm.SDT_NHANHANG.Length != 10)
                {
                    ModelState.AddModelError("SDT_NHANHANG", "Số điện thoại không hợp lệ!");
                }
                else if (!dkm.SDT_NHANHANG.All(char.IsDigit))
                {
                    ModelState.AddModelError("SDT_NHANHANG", "Đây không phải là số điện thoại!");
                }
            }
            if (string.IsNullOrEmpty(dkm.Email))
            {
                ModelState.AddModelError("Email", "Email không được bỏ trống!.");
            }
            else
            {
                string email = dkm.Email.Trim();
                if (!string.IsNullOrEmpty(email))
                {
                    if (!Regex.IsMatch(email, @"^[a-zA-Z0-9._%-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$"))
                    {
                        ModelState.AddModelError("Email", "Định dạng Email không đúng.");
                    }
                }
            }
            if (!IsStrongPassword(dkm.Password))
            {
                ModelState.AddModelError("Password", "Mật khẩu không đủ mạnh. Yêu cầu ít nhất 8 ký tự, chứa chữ hoa, chữ thường, số và ký tự đặc biệt!.");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    KHACHHANG khachhang = new KHACHHANG();
                    khachhang.EMAIL = dkm.Email;
                    khachhang.MATKHAU = dkm.Password;
                    khachhang.TENKH = dkm.Name;
                    khachhang.SDT_KH = dkm.PhoneNumber;
                    khachhang.DIACHI = dkm.Address;
                    khachhang.SDT_NHANHANG = dkm.SDT_NHANHANG;
                    khachhang.TEN_NHANHANG = dkm.TEN_NHANHANG;
                    khachhang.DIACHI_NHANHANG = dkm.DIACHI_NHANHANG;
                    //khachhang.SDT_NHANHANG = dkm.PhoneNumber;
                    //khachhang.TEN_NHANHANG = dkm.Name;
                    //khachhang.DIACHI_NHANHANG = dkm.Address;
                    //----------------------------------------------------------
                    System.Net.Mail.MailMessage mm = new System.Net.Mail.MailMessage("truongdat20012002@gmail.com",dkm.Email);
                    mm.Subject = "MINIF4 Store - Xác nhận OTP đăng ký tài khoản";
                    // Tạo một chuỗi ngẫu nhiên
                    string randomString = GenerateRandomString(6);
                    mm.Body ="Mã OTP: " + randomString;
                    mm.IsBodyHtml = false;

                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;

                    NetworkCredential nc = new NetworkCredential("truongdat20012002@gmail.com", "owxpyxethkdfzkua");
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = nc;
                    smtp.Send(mm);
                    
                    // Insert vào cơ sở dữ liệu
                    khachhang.OTP = randomString;
                    db.KHACHHANGs.InsertOnSubmit(khachhang);
                    db.SubmitChanges();
                    //------------------------------------------------
                    ViewBag.guiOTP = "Đã gửi OTP đến hộp thư của bạn!";
                    ViewBag.kt = true;
                    ViewBag.maOTP = randomString;
                    return View("maOTP");
                }
                catch (Exception ex)
                {
                    ViewBag.kq = "Lỗi rồi.";
                    return View();
                }
            }
            else
            {
                ViewBag.kq = "Đăng ký tài khoản thất bại. Vui lòng thử lại!";
                return View();
            }
        }
        #endregion

        #region Thông Tin Tài Khoản
        public ActionResult Profile(string id_kh)
        {
            if (string.IsNullOrEmpty(id_kh))
            {
                return RedirectToAction("Index","Home");
            }

            var khachhang = db.KHACHHANGs.SingleOrDefault(s => s.SDT_KH.Equals(id_kh));
            
            if (khachhang != null)
            {
                return View(khachhang);
            }

            return HttpNotFound();
        }
        #endregion

        #region Cập Nhật Thông Tin Tài Khoản
        [HttpGet]
        public ActionResult EditProfile(string id_kh)
        {
            if (string.IsNullOrEmpty(id_kh))
            {
                return RedirectToAction("Index", "Home");
            }

            var khachhang = db.KHACHHANGs.SingleOrDefault(s => s.SDT_KH.Equals(id_kh));

            if (khachhang != null)
            {
                return View(khachhang);
            }

            return HttpNotFound();
        }
        [HttpPost]
        public ActionResult EditProfile(KHACHHANG khachhang)
        {
            if (string.IsNullOrEmpty(khachhang.EMAIL) || string.IsNullOrEmpty(khachhang.DIACHI) ||
                string.IsNullOrEmpty(khachhang.TENKH) || string.IsNullOrEmpty(khachhang.MATKHAU)||
                string.IsNullOrEmpty(khachhang.TEN_NHANHANG)|| string.IsNullOrEmpty(khachhang.SDT_NHANHANG)||
                string.IsNullOrEmpty(khachhang.DIACHI_NHANHANG))
            {
                ModelState.AddModelError("", "Vui lòng nhập đầy đủ thông tin.");
            }
            if (!string.IsNullOrEmpty(khachhang.MATKHAU))
            {
                if (!IsStrongPassword(khachhang.MATKHAU))
                {
                    ModelState.AddModelError("MATKHAU", "Mật khẩu không đủ mạnh. Yêu cầu ít nhất 8 ký tự, chứa chữ hoa, chữ thường, số và ký tự đặc biệt.");
                }
            }
            string email=khachhang.EMAIL.Trim();
            if (!string.IsNullOrEmpty(email))
            { 
                if (!string.IsNullOrEmpty(email))
                {
                    if (!Regex.IsMatch(email, @"^[a-zA-Z0-9._%-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$"))
                    {
                        ModelState.AddModelError("Email", "Định dạng Email không đúng.");
                    }
                }
            }
            if (!string.IsNullOrEmpty(khachhang.SDT_NHANHANG))
            {
                if (khachhang.SDT_NHANHANG.Length != 10)
                {
                    ModelState.AddModelError("SDT_NHANHANG", "Số điện thoại nhận hàng không hợp lệ!");
                }
                else if (!khachhang.SDT_NHANHANG.All(char.IsDigit))
                {
                    ModelState.AddModelError("SDT_NHANHANG", "Số điện thoại nhận hàng không hợp lệ!");
                }
            }
            try
            {
                if (ModelState.IsValid)
                {                    
                    var existingKhachHang = db.KHACHHANGs.SingleOrDefault(s => s.SDT_KH.Equals(khachhang.SDT_KH));

                    if (existingKhachHang != null)
                    {
                        existingKhachHang.EMAIL = email;
                        existingKhachHang.MATKHAU = khachhang.MATKHAU;
                        existingKhachHang.TENKH = khachhang.TENKH;
                        existingKhachHang.DIACHI = khachhang.DIACHI;

                        existingKhachHang.DIACHI_NHANHANG = khachhang.DIACHI_NHANHANG;
                        existingKhachHang.TEN_NHANHANG = khachhang.TEN_NHANHANG;
                        existingKhachHang.SDT_NHANHANG = khachhang.SDT_NHANHANG;
                        // Lưu thay đổi vào cơ sở dữ liệu
                        db.SubmitChanges();

                        ViewBag.thongbao = "Chỉnh sửa thông tin thành công. Vui lòng đăng nhập lại tài khoản!";
                        ViewBag.thanhcong = true;
                        Session["use"] = null;
                        return View("DangNhap");
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Không tìm thấy khách hàng cần chỉnh sửa.";
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Đã có lỗi xảy ra. Vui lòng thử lại.";
            }
            return View(khachhang);
        }
        #endregion

        #region Xác Nhận OTP Khi Đăng Ký Tài Khoản
        public ActionResult maOTP()
        {
            return View();
        }
        [HttpPost]
        public ActionResult maOTP(OTP otp)
        {            
            if (string.IsNullOrEmpty(otp.maOTP))
            {
                ModelState.AddModelError("maOTP", "Vui lòng nhập mã OTP!.");
            }
            if (db.KHACHHANGs.Any(x => x.OTP == otp.maOTP))
            {
                ViewBag.thanhcongnhe = "Chúc mừng. Bạn đã đăng ký tài khoản thành công! Đăng nhập ngay nhé <33";
                ViewBag.ktra = true;
                return View("DangNhap");
            }
            else
            {
                ModelState.AddModelError("maOTP", "Mã OTP không chính xác!.");
                return View("maOTP");
            }
        }
        #endregion

        #region Mấy cái này để đăng xuất thì cho cut
        public int TongSoLuongLoaiMatHang()
        {
            int tsl = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                tsl = lstGioHang.Count();
            }
            return tsl;
        }
        #endregion
    }
}