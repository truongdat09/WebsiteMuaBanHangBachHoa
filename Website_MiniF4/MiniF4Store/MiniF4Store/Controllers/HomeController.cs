using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MiniF4Store.Models;
using PagedList;
using System.Configuration;
using WebBanHangOnline.Models.Payments;
using System.Data.Linq;

namespace MiniF4Store.Controllers
{
    public class HomeController : Controller
    {
        QL_BachHoaDataContext db = new QL_BachHoaDataContext();
        int phiShip = 30000;  //Của Trung

        #region Kieu Mai
        public ActionResult Index()
        {
            //Số lượng giỏ hàng
            List<GioHang> lstGioHang = LayGioHang();
            List<YeuThich> lstYeuThich = LayYeuThich();
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongSoLuongLoaiMatHang = TongSoLuongLoaiMatHang();
            ViewBag.TongSoLuongYT = TongSoLuongYT();
            //Xuất sản phẩm
            var list = db.SANPHAMs.OrderByDescending(s => s.LUOTBAN).Skip(0).Take(10);
            return View(list);
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        public ActionResult Contact(string SDT_KH)
        {
            string kh = SDT_KH;
            List<YeuThich> lstYeuThich = LayYeuThich();
            ViewBag.TongSoLuongYT = TongSoLuongYT();
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongSoLuongLoaiMatHang = TongSoLuongLoaiMatHang();
            List<GioHang> lstGioHang = LayGioHang();
            List<HOADON> lst_hd = db.HOADONs.Where(s => s.SDT_KH == kh).ToList();
            if (lst_hd == null)
            {
                return HttpNotFound();
            }
            return View(lst_hd);
        }
        public ActionResult HuyDonHang(string ma, string sdt)
        {
            List<GioHang> lstGioHang = LayGioHang();
            List<YeuThich> lstYeuThich = LayYeuThich();
            ViewBag.TongSoLuongYT = TongSoLuongYT();
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongSoLuongLoaiMatHang = TongSoLuongLoaiMatHang();
            HOADON hd = db.HOADONs.SingleOrDefault(x => x.MAHD == ma);
            List<CHITIET_HD> cthd = db.CHITIET_HDs.ToList();
            List<SANPHAM> sanpham = db.SANPHAMs.ToList();
           

            if (hd != null)
            {
                hd.TRANGTHAI_DH = "Đã hủy";
                foreach (CHITIET_HD item in cthd)
                {
                    foreach (SANPHAM sp in sanpham)
                    {
                        if (hd.MAHD == item.MAHD && item.MASP==sp.MASP)
                        {
                            sp.SOLUONG += item.SL;
                        }
                    }

                }
                db.SubmitChanges();
                return RedirectToAction("Contact", "Home", new { SDT_KH = sdt });
            }
            return View();
        }
        public ActionResult DaGiao(string ma, string sdt)
        {
            List<GioHang> lstGioHang = LayGioHang();
            List<YeuThich> lstYeuThich = LayYeuThich();
            ViewBag.TongSoLuongYT = TongSoLuongYT();
            ViewBag.TongSoLuong = TongSoLuong();
            HOADON hd = db.HOADONs.SingleOrDefault(x => x.MAHD == ma);
            if (hd != null)
            {
                hd.TRANGTHAI_DH = "Đã giao";
                db.SubmitChanges();
                return RedirectToAction("Contact", "Home", new { SDT_KH = sdt });

            }
            return View();
        }
        public ActionResult DoiTra(string ma, string sdt, FormCollection f)
        {
            List<GioHang> lstGioHang = LayGioHang();
            List<YeuThich> lstYeuThich = LayYeuThich();
            ViewBag.TongSoLuongYT = TongSoLuongYT();
            ViewBag.TongSoLuong = TongSoLuong();
            HOADON hd = db.HOADONs.SingleOrDefault(x => x.MAHD == ma);
            if (hd != null)
            {
                hd.TRANGTHAI_DH = "Chờ đổi trả";
                hd.LIDO = f["lido"].ToString();
                db.SubmitChanges();
                return RedirectToAction("Contact", "Home", new { SDT_KH = sdt });

            }
            return View();

        }
        public ActionResult XemChiTietDT(string ma, string tinhtrang, string tongtien, string ngaylap)
        {
            List<GioHang> lstGioHang = LayGioHang();
            List<YeuThich> lstYeuThich = LayYeuThich();
            ViewBag.TongSoLuongYT = TongSoLuongYT();
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TinhTrang = tinhtrang;
            ViewBag.TongTien = tongtien;
            ViewBag.NgayLap = ngaylap;
            List<SANPHAM> sanpham = db.SANPHAMs.ToList();
            List<DOITRA> doitra = db.DOITRAs.ToList();
            List<HOADON> hoadon = db.HOADONs.ToList();


            var SP_Record = from hd in hoadon
                            join ct in doitra on hd.MAHD equals ct.MAHD into table1
                            from h in table1.ToList()
                            join sp in sanpham on h.MASP equals sp.MASP into table2
                            from i in table2.ToList()
                            select new ViewDoiTra
                            {
                                hoadon = hd,
                                doitra = h,
                                sanpham = i
                            };

            var hang = SP_Record.Where(s => s.hoadon.MAHD == ma);
            if (hang == null)
            {
                return HttpNotFound();
            }
            return View(hang);
        }
        public ActionResult XemChiTiet(string ma, string tinhtrang, string tongtien, string ngaylap)
        {
            List<GioHang> lstGioHang = LayGioHang();
            List<YeuThich> lstYeuThich = LayYeuThich();
            ViewBag.TongSoLuongYT = TongSoLuongYT();
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TinhTrang = tinhtrang;
            ViewBag.TongTien = tongtien;
            ViewBag.NgayLap = ngaylap;
            List<SANPHAM> sanpham = db.SANPHAMs.ToList();
            List<CHITIET_HD> chitiet_hd = db.CHITIET_HDs.ToList();
            List<HOADON> hoadon = db.HOADONs.ToList();


            var SP_Record = from hd in hoadon
                            join ct in chitiet_hd on hd.MAHD equals ct.MAHD into table1
                            from h in table1.ToList()
                            join sp in sanpham on h.MASP equals sp.MASP into table2
                            from i in table2.ToList()
                            select new View_DH
                            {
                                hoadon = hd,
                                chitiet_hd = h,
                                sanpham = i
                            };

            var hang = SP_Record.Where(s => s.hoadon.MAHD == ma);
            if (hang == null)
            {
                return HttpNotFound();
            }
            return View(hang);
        }
        public ActionResult shop(int? page, int? pageSize)
        {
            List<GioHang> lstGioHang = LayGioHang();
            List<YeuThich> lstYeuThich = LayYeuThich();
            ViewBag.TongSoLuongYT = TongSoLuongYT();
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongSoLuongLoaiMatHang = TongSoLuongLoaiMatHang();
            //Xuất sản phẩm
            if (page == null)
            {
                page = 1;
            }
            if (pageSize == null)
            {
                pageSize = 9;
            }

            var lstTang = db.SANPHAMs.OrderBy(s => s.GIABAN);
            return View(lstTang.ToPagedList((int)page, (int)pageSize));



        }
        public ActionResult LocTheoGia(int min, int max)
        {

            //Số lượng giỏ hàng
            List<GioHang> lstGioHang = LayGioHang();
            List<YeuThich> lstYeuThich = LayYeuThich();
            ViewBag.TongSoLuongYT = TongSoLuongYT();
            ViewBag.TongSoLuong = TongSoLuong();
            //Xuất sản phẩm
            List<SANPHAM> sanpham = db.SANPHAMs.ToList();
            var hang = sanpham.Where(s => s.GIABAN > min && s.GIABAN < max);
            if (hang == null)
            {
                return HttpNotFound();
            }
            return View(hang);
        }
        public ActionResult yeuthich()
        {
            List<YeuThich> lstYeuThich = LayYeuThich();
            ViewBag.TongSoLuongYT = TongSoLuongYT();
            if (Session["YeuThich"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            List<GioHang> lstGioHang = LayGioHang();
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongThanhTien = TongThanhTien();

            return View(lstYeuThich);
        }
        public int TongSoLuongYT()
        {
            var tsl = 0;
            List<YeuThich> lstGioHang = Session["YeuThich"] as List<YeuThich>;
            if (lstGioHang != null)
            {
                tsl = (from s in lstGioHang
                       select s.MASP).Count();
            }
            return tsl;
        }
        //Them yeu thich
        public ActionResult ThemYeuThich(int ma, string strURL)
        {
            List<YeuThich> lstYeuThich = LayYeuThich();
            List<GioHang> lstGioHang = LayGioHang();
            YeuThich SanPham = lstYeuThich.SingleOrDefault(sp => sp.MASP == ma);
            if (SanPham == null)
            {

                SanPham = new YeuThich(ma);
                lstYeuThich.Add(SanPham);
                return Redirect(strURL);
            }
            else
            {
                return Redirect(strURL);
            }
        }
        //Xoa yeu thich
        public ActionResult xoaYeuThich(int MaSP)
        {
            List<YeuThich> lstYeuThich = LayYeuThich();
            List<GioHang> lstGioHang = LayGioHang();

            YeuThich sp = lstYeuThich.Single(s => s.MASP == MaSP);
            if (sp != null)
            {
                lstYeuThich.RemoveAll(s => s.MASP == MaSP);
                return RedirectToAction("yeuthich", "Home");
            }
            if (lstYeuThich.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("yeuthich", "Home");
        }
        public List<YeuThich> LayYeuThich()
        {
            List<YeuThich> lstYeuThich = Session["YeuThich"] as List<YeuThich>;
            if (lstYeuThich == null)
            {
                lstYeuThich = new List<YeuThich>();
                Session["YeuThich"] = lstYeuThich;
            }
            return lstYeuThich;
        }
        public ActionResult search(FormCollection f) // Cái này cũng lạ
        {
            //Số lượng giỏ hàng
            List<GioHang> lstGioHang = LayGioHang();   // 
            //---------------------------------------------
            List<YeuThich> lstYeuThich = LayYeuThich();
            ViewBag.TongSoLuongYT = TongSoLuongYT();
            //---------------------------------------------
            ViewBag.TongSoLuong = TongSoLuong();
            //Xuất sản phẩm
            List<SANPHAM> list = null;
            if (!string.IsNullOrEmpty(f["search"].ToString()))
            {
                list = db.SANPHAMs.Where(x => x.TENSP.Contains(f["search"].ToString())).ToList();
            }
            return View(list);
        }
        public ViewResult LoaiSP(int masp)   // Này nữa  nè:)))))
        {

            List<GioHang> lstGioHang = LayGioHang();
            //---------------------------------------------------------
            List<YeuThich> lstYeuThich = LayYeuThich();
            ViewBag.TongSoLuongYT = TongSoLuongYT();
            //---------------------------------------------------------
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongSoLuongLoaiMatHang = TongSoLuongLoaiMatHang();
            //Xuất sản phẩm
            var Listsp = db.SANPHAMs.Where(s => s.MANH == masp).ToList();
            return View(Listsp);
        }
        public ActionResult details(int ma)   // :))) này nữa
        {
            //Số lượng giỏ hàng
            List<GioHang> lstGioHang = LayGioHang();
            //--------------------------------------------------------
            List<YeuThich> lstYeuThich = LayYeuThich();
            ViewBag.TongSoLuongYT = TongSoLuongYT();
            //--------------------------------------------------------
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongSoLuongLoaiMatHang = TongSoLuongLoaiMatHang();
            //Xuất sản phẩm
            SANPHAM hang = db.SANPHAMs.Single(s => s.MASP == ma);
            if (hang == null)
            {
                return HttpNotFound();
            }
            return View(hang);
        }
        #endregion

        #region  Huu Trung
        public ActionResult cart()   // Có cái gì yêu thích á,lạ lắm
        {
            //Cái này của mày nè Mai
            List<YeuThich> lstYeuThich = LayYeuThich();
            ViewBag.TongSoLuongYT = TongSoLuongYT();

            //Cái này của mày nè Mai - end
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            List<GioHang> lstGioHang = LayGioHang();
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongSoLuongLoaiMatHang = TongSoLuongLoaiMatHang();
            ViewBag.TongThanhTien = TongThanhTien();






            return View(lstGioHang);
        }

        public ActionResult LuuHoaDon()   // - thanh toan ship COD
        {
            List<GioHang> lstGioHang = LayGioHang();
            LuuHD(lstGioHang);
            SendEmail(1);


            //
            Session.Remove("GioHang");
            return RedirectToAction("Index", "Home");
        }
        public void SendEmail(int hinhthucthanhtoan)
        {
            List<GioHang> lstGioHang = LayGioHang();
            // send mail
            var str_SP = "";
            var Thanhtien = decimal.Zero;
            var Tongtien = decimal.Zero;

            var u = Session["use"] as MiniF4Store.Models.KHACHHANG;
            //model.SDT_KH = Session["u"] as string;
            var Sdt = u.SDT_NHANHANG;
            var Tenkhachhang = u.TEN_NHANHANG;
            var Email = u.EMAIL;
            var Trangthai = "";
            if (hinhthucthanhtoan == 1)
            {
                Trangthai = "Hình thức thanh toán:Ship COD";
            }
            else
            {
                Trangthai = "Hình thức thanh toán: thẻ tín dụng - Đã thanh toán";
            }
            var Diachinhanhang = u.DIACHI_NHANHANG;
            foreach (var sp in lstGioHang)
            {
                str_SP += "<tr>";
                str_SP += "<td>" + sp.TENSP + "</td>";
                str_SP += "<td>" + sp.SOLUONG + "</td>";
                str_SP += "<td>" + sp.GIABAN + "</td>";
                str_SP += "<td>" + sp.ThanhTien + "</td>";

                str_SP += "</tr>";
                Thanhtien = sp.SOLUONG * sp.GIABAN;
            }

            Tongtien = (int)TongThanhTien() + phiShip;
            if (Tongtien >= 299000)
            {
                Tongtien -= phiShip;
            }
            string contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/Content/Template/send2.html"));

            contentCustomer = contentCustomer.Replace("{{Madon}}", layMaHD());
            contentCustomer = contentCustomer.Replace("{{Sanpham}}", str_SP.ToString());
            contentCustomer = contentCustomer.Replace("{{Tenkhachhang}}", Tenkhachhang);
            contentCustomer = contentCustomer.Replace("{{Sdt}}", Sdt);
            contentCustomer = contentCustomer.Replace("{{Email}}", Email);
            contentCustomer = contentCustomer.Replace("{{Diachinhanhang}}", Diachinhanhang);
            contentCustomer = contentCustomer.Replace("{{Thanhtien}}", Thanhtien.ToString());
            contentCustomer = contentCustomer.Replace("{{Tongtien}}", Tongtien.ToString());
            contentCustomer = contentCustomer.Replace("{{Trangthai}}", Trangthai.ToString());
            MiniF4Store.Common.Common.sendEmail("MiniF4Store", "Đơn hàng #" + layMaHD(), contentCustomer.ToString(), Email.Trim());
        }

        public ActionResult CapNhatGioHang(int MaSP, FormCollection f)
        {
            var lstGioHang = LayGioHang();
            GioHang sp = lstGioHang.Single(s => s.MASP == MaSP);
            if (sp != null)
            {
                sp.SOLUONG = int.Parse(f["txtSoLuong"].ToString());
            }
            return RedirectToAction("cart", "Home");
        }
        public ActionResult ThemGioHang(int ma, string sdt, string strURL)   // Này nữa 
        {
            List<GioHang> lstGioHang = LayGioHang();
            //------------------------------------------------------------------
            List<YeuThich> lstYeuThich = LayYeuThich();
            ViewBag.TongSoLuongYT = TongSoLuongYT();
            ViewBag.TongSoLuongLoaiMatHang = TongSoLuongLoaiMatHang();


            //------------------------------------------------------------------
            GioHang SanPham = lstGioHang.SingleOrDefault(sp => sp.MASP == ma);
            if (SanPham == null)
            {

                SanPham = new GioHang(ma, sdt);
                lstGioHang.Add(SanPham);
                return Redirect(strURL);
            }
            else
            {
                //SanPham.SOLUONG++;
                return Redirect(strURL);
            }
        }
        public ActionResult xoaGioHang(int MaSP)  // này nữa
        {
            //----------------------------------------
            List<YeuThich> lstYeuThich = LayYeuThich();
            ViewBag.TongSoLuongYT = TongSoLuongYT();
            //----------------------------------------
            List<GioHang> lstGioHang = LayGioHang();
            GioHang sp = lstGioHang.Single(s => s.MASP == MaSP);
            if (sp != null)
            {
                lstGioHang.RemoveAll(s => s.MASP == MaSP);
                return RedirectToAction("cart", "Home");
            }
            if (lstGioHang.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("cart", "Home");
        }
        public List<GioHang> LayGioHang()
        {
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang == null)
            {
                lstGioHang = new List<GioHang>();
                Session["GioHang"] = lstGioHang;
            }
            return lstGioHang;
        }
        public int TongSoLuong()
        {
            int tsl = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                tsl = lstGioHang.Sum(sp => sp.SOLUONG);
            }
            return tsl;
        }
        public int TongSoLuongLoaiMatHang()
        {
            int tsl = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                //tsl = lstGioHang.Count();
                tsl = lstGioHang.Count();
            }
            return tsl;
        }

        public double TongThanhTien()
        {
            double ttt = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                ttt += lstGioHang.Sum(sp => sp.ThanhTien);
            }
            return ttt;
        }
        public ActionResult checkout(string SDT_KH)
        {
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            List<GioHang> lstGioHang = LayGioHang();
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongThanhTien = TongThanhTien();
            ViewBag.TongSoLuongLoaiMatHang = TongSoLuongLoaiMatHang();
            return View(lstGioHang);
        }
        public ActionResult DonHangCuaBan(string SDT_KH)
        {
            List<GioHang> lstGioHang = LayGioHang();
            if (lstGioHang.Count > 0)
            {
                LuuHD(lstGioHang);
                lstGioHang = null;
            }
            List<HOADON> hang = db.HOADONs.Where(s => s.SDT_KH == SDT_KH).ToList();
            if (hang == null)
            {
                return HttpNotFound();
            }
            return View(hang);
        }
        public void LuuHD(List<GioHang> lstGioHang)
        {
            //Lưu hóa đơn         
            HOADON model = new HOADON();
            model.MAHD = taoMaHD();
            model.NGAYLAP = DateTime.Now;
            var u = Session["use"] as MiniF4Store.Models.KHACHHANG;
            //model.SDT_KH = Session["u"] as string;
            model.SDT_KH = u.SDT_KH;
            model.TONGTIEN = (int)TongThanhTien() + phiShip;
            if (model.TONGTIEN >= 299000)
            {
                model.TONGTIEN -= phiShip;
            }
            model.TRANGTHAI_DH = "Chờ xác nhận";
            model.MANV = 1;
            model.HINHTHUC_TT = "Ship COD";

            db.HOADONs.InsertOnSubmit(model);
            db.SubmitChanges();
            //Lưu chi tiết hóa đơn

            List<CHITIET_HD> chiTietHoaDonList = new List<CHITIET_HD>();
            foreach (GioHang item in lstGioHang)
            {
                CHITIET_HD model_cthd = new CHITIET_HD();
                var capNhatSP = db.SANPHAMs.FirstOrDefault(p => p.MASP == item.MASP);
                if (capNhatSP != null)
                {
                    capNhatSP.LUOTBAN = capNhatSP.LUOTBAN + 1;
                }
                db.SubmitChanges();
                model_cthd.MAHD = model.MAHD;
                model_cthd.MASP = item.MASP;
                model_cthd.SL = item.SOLUONG;
                //- Thêm giá vào chi tiết hóa đơn
                model_cthd.GIABAN = item.GIABAN;
                //
                chiTietHoaDonList.Add(model_cthd);

            }
            db.CHITIET_HDs.InsertAllOnSubmit(chiTietHoaDonList);
            db.SubmitChanges();

        }
        public void LuuHD_VoiTheTinhDung(List<GioHang> lstGioHang)
        {
            //Lưu hóa đơn         
            HOADON model = new HOADON();
            model.MAHD = taoMaHD();
            model.NGAYLAP = DateTime.Now;
            var u = Session["use"] as MiniF4Store.Models.KHACHHANG;
            model.SDT_KH = u.SDT_KH;
            model.TONGTIEN = (int)TongThanhTien() + phiShip;
            if (model.TONGTIEN >= 299000)
            {
                model.TONGTIEN -= phiShip;
            }
            model.TRANGTHAI_DH = "Đã xác nhận";
            model.MANV = 1;
            model.HINHTHUC_TT = "Thẻ tín dụng";
          

            db.HOADONs.InsertOnSubmit(model);
            //Lưu chi tiết hóa đơn

            foreach (GioHang item in lstGioHang)
            {
                CHITIET_HD model_cthd = new CHITIET_HD();
                var capNhatSP = db.SANPHAMs.FirstOrDefault(p => p.MASP == item.MASP);
                if (capNhatSP != null)
                {
                    capNhatSP.LUOTBAN = capNhatSP.LUOTBAN + 1;
                }
                db.SubmitChanges();
                model_cthd.MAHD = model.MAHD;
                model_cthd.MASP = item.MASP;
                model_cthd.SL = item.SOLUONG;
                            
                model_cthd.GIABAN = item.GIABAN;

                db.CHITIET_HDs.InsertOnSubmit(model_cthd);
            }
            db.SubmitChanges();
            SendEmail(2);
        }
        public string taoMaHD()
        {
            int count = db.HOADONs.Count();
            count++;
            string t = count.ToString();
            return t;
        }
        public string layMaHD()
        {

            int count = db.HOADONs.Count();
            string t = count.ToString();
            return t;
        }

        //Xử lí vnpay             
        public ActionResult VnpayReturn()
        {
            ViewBag.TongSoLuongLoaiMatHang = TongSoLuongLoaiMatHang();
            if (Request.QueryString.Count > 0)
            {
                string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Chuoi bi mat
                var vnpayData = Request.QueryString;
                VnPayLibrary vnpay = new VnPayLibrary();

                foreach (string s in vnpayData)
                {
                    //get all querystring data
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(s, vnpayData[s]);
                    }
                }
                string orderCode = Convert.ToString(vnpay.GetResponseData("vnp_TxnRef"));
                long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                String vnp_SecureHash = Request.QueryString["vnp_SecureHash"];
                String TerminalID = Request.QueryString["vnp_TmnCode"];
                // long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
                String bankCode = Request.QueryString["vnp_BankCode"];
                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                    {
                        List<GioHang> lstGioHang = LayGioHang();
                        LuuHD_VoiTheTinhDung(lstGioHang);


                        //var itemOrder = db.Orders.FirstOrDefault(x => x.Code == orderCode);
                        //if (itemOrder != null)
                        //{
                        //    //itemOrder.Status = 2;//đã thanh toán
                        //    //db.Orders.Attach(itemOrder);
                        //    //db.Entry(itemOrder).State = System.Data.Entity.EntityState.Modified;
                        //    //db.SaveChanges();
                        //}
                        //Thanh toan thanh cong
                        string invoiceCode = taoMaHD();
                        ViewBag.InnerText = "Giao dịch được thực hiện thành công. Cảm ơn quý khách đã sử dụng dịch vụ";
                        // Thay thế bằng mã hóa đơn cụ thể bạn đang quan tâm
                        var vnp_Amount = db.HOADONs
                            .Where(hd => hd.MAHD == invoiceCode) // MaHoaDon là trường chứa mã hóa đơn
                            .Select(hd => hd.TONGTIEN)
                            .FirstOrDefault();
                        var tt = 0;
                        tt = (int)TongThanhTien() + phiShip;
                        if (tt >= 299000)
                        {
                            tt -= phiShip;
                        }
                        ViewBag.TongThanhTien = "Số tiền thanh toán (VND):" + tt;

                        //log.InfoFormat("Thanh toan thanh cong, OrderId={0}, VNPAY TranId={1}", orderId, vnpayTranId);
                        Session.Remove("GioHang");
                    }
                    else
                    {
                        //Thanh toan khong thanh cong. Ma loi: vnp_ResponseCode
                        ViewBag.InnerText = "Có lỗi xảy ra trong quá trình xử lý.Mã lỗi: " + vnp_ResponseCode;
                        Session.Remove("GioHang");
                        //log.InfoFormat("Thanh toan loi, OrderId={0}, VNPAY TranId={1},ResponseCode={2}", orderId, vnpayTranId, vnp_ResponseCode);
                    }
                    //displayTmnCode.InnerText = "Mã Website (Terminal ID):" + TerminalID;
                    //displayTxnRef.InnerText = "Mã giao dịch thanh toán:" + orderId.ToString();
                    //displayVnpayTranNo.InnerText = "Mã giao dịch tại VNPAY:" + vnpayTranId.ToString();

                    //displayBankCode.InnerText = "Ngân hàng thanh toán:" + bankCode;
                }
            }
            //var a = UrlPayment(0, "DH3574");

            return View();
        }
        public void UrlPayment(string SDT_KH)
        {
            string mahd = taoMaHD();
            var urlPayment = "";
            //var order = db.HOADONs.FirstOrDefault(x => x.MAHD == orderCode);
            //Get Config Info
            string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"]; //URL nhan ket qua tra ve 
            string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"]; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"]; //Ma định danh merchant kết nối (Terminal Id)
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Secret Key

            //Build URL for VNPAY
            VnPayLibrary vnpay = new VnPayLibrary();
            //
            int TONGTIEN = ((int)TongThanhTien() + phiShip);
            if (TONGTIEN >= 299000)
            {
                TONGTIEN = (TONGTIEN - phiShip);
            }
            Console.WriteLine("Tong  tien: " + TONGTIEN);

            TONGTIEN *= 100;
            //
            //var Price = (long)order.TotalAmount * 100;

            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", TONGTIEN.ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
            vnpay.AddRequestData("vnp_BankCode", "VNBANK");

            //if (TypePaymentVN == 1)
            //{
            //    vnpay.AddRequestData("vnp_BankCode", "VNPAYQR");
            //}
            //else if (TypePaymentVN == 2)
            //{
            //    vnpay.AddRequestData("vnp_BankCode", "VNBANK");
            //}
            //else if (TypePaymentVN == 3)
            //{
            //    vnpay.AddRequestData("vnp_BankCode", "INTCARD");
            //}

            DateTime CreatedDate = DateTime.Now;
            vnpay.AddRequestData("vnp_CreateDate", CreatedDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán đơn hàng :" + mahd);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other

            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);

            Random random = new Random();
            int randomNumber = random.Next(1, 10000);
            string _idTrans = mahd + randomNumber;
            vnpay.AddRequestData("vnp_TxnRef", _idTrans); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày
            //Add Params of 2.1.0 Version
            //Billing

            urlPayment = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            //log.InfoFormat("VNPAY URL: {0}", paymentUrl);
            Response.Redirect(urlPayment);
            //return urlPayment;

        }


        #endregion
    }
}