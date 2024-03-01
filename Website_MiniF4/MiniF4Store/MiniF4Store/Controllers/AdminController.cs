using MiniF4Store.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;

namespace MiniF4Store.Controllers
{
    public class AdminController : Controller
    {
        QL_BachHoaDataContext db = new QL_BachHoaDataContext();
        //
        // GET: /Admin/


        [ChildActionOnly]
        public ActionResult RenderHeader()
        {
            ViewBag.SLDonChuaDuyet = DonChuaXN();
            return PartialView("_HeaderNavBar");
        }


        public ActionResult Trang_chu()
        {
            ViewBag.TongDH = TongDH();
            ViewBag.TongDT = TongDoanhThu();
            ViewBag.TongSP = TongSoSP();
            ViewBag.TongNH = TongSoNH();

            var SP_Record = from s in db.SANPHAMs.OrderByDescending(x => x.LUOTBAN)
                            select s;
            return View(SP_Record);

        }

        public int? TongDoanhThu()
        {
            var tong = (from hd
                        in db.HOADONs
                        select hd.TONGTIEN).Sum();
            return tong;
        }

        public int? TongSoSP()
        {
            var tong = (from hd
                        in db.SANPHAMs
                        select hd).Count();
            return tong;
        }
        public int? TongSoNH()
        {
            var tong = (from hd
                        in db.PHIEUNHAPs
                        select hd).Count();
            return tong;
        }
        public ActionResult TimKiemSP(string searchString, int? page, int? pageSize)
        {
            var sp = from s in db.SANPHAMs
                     where s.TENSP.Contains(searchString)
                     join nh in db.NHOMHANGs on s.MANH equals nh.MANH
                     select s;

            if (page == null)
            {
                page = 1;
            }
            if (pageSize == null)
            {
                pageSize = 10;
            }
            return View(sp.ToPagedList((int)page, (int)pageSize));
        }
        public ActionResult SanPham(int? page, int? pageSize)
        {
            var SP_Record = from s in db.SANPHAMs
                            join nh in db.NHOMHANGs on s.MANH equals nh.MANH
                            select new View_SP
                            {
                                sanpham = s,
                                nhomhang = nh
                            };
            if (page == null)
            {
                page = 1;
            }
            if (pageSize == null)
            {
                pageSize = 10;
            }

            return View(SP_Record.ToPagedList((int)page, (int)pageSize));
        }

        public ActionResult View_SP()
        {
            var SP_Record = from s in db.SANPHAMs
                            select s;
            return View(SP_Record);

        }




        public ActionResult Don_hang(int? page, int? pageSize)
        {
            var SP_Record = from kh in db.KHACHHANGs
                            join ct in db.HOADONs on kh.SDT_KH equals ct.SDT_KH into table1
                            from h in table1.ToList()
                            join nv in db.NHANVIENs on h.MANV equals nv.MANV into table2
                            from i in table2.ToList()
                            select new View_DH_Linh
                            {
                                khachhang = kh,
                                hoadon = h,
                                nhanvien = i
                            };
            var sp = SP_Record.OrderByDescending(s => s.hoadon.NGAYLAP);
            if (page == null)
            {
                page = 1;
            }
            if (pageSize == null)
            {
                pageSize = 10;
            }

            return View(sp.ToPagedList((int)page, (int)pageSize));
        }


        //[HttpPost]
        public ActionResult ChiTiet(string ma)
        {
            var SP_Record = (from hd in db.HOADONs
                             where hd.MAHD == ma
                             join s in db.CHITIET_HDs on hd.MAHD equals s.MAHD
                             from ct in db.CHITIET_HDs
                             where ct.MAHD == ma
                             join sp in db.SANPHAMs on ct.MASP equals sp.MASP into table_1
                             from t in table_1.ToList()
                             select new View_ChiTietDH
                             {
                                 hoadon = hd,
                                 chitiet = ct,
                                 sanpham = t,

                             }).Distinct();

            return View(SP_Record);
        }



        public ActionResult Update_TTDH(string ma, int? page, int? pageSize)
        {
            HOADON hd = db.HOADONs.Where(x => x.MAHD == ma).FirstOrDefault();
            if (hd != null)
            {
                hd.TRANGTHAI_DH = "Đã xác nhận";
                hd.MANV = 1;
                db.SubmitChanges();
            }


            if (page == null)
            {
                page = 1;
            }
            if (pageSize == null)
            {
                pageSize = 10;
            }

            return View(db.HOADONs.ToPagedList((int)page, (int)pageSize));
        }

        public ActionResult Huy_DH(string ma, int? page, int? pageSize)
        {
            HOADON hd = db.HOADONs.Where(x => x.MAHD == ma).FirstOrDefault();
            if (hd != null)
            {
                hd.TRANGTHAI_DH = "Đã hủy";
                hd.MANV = 1;
                db.SubmitChanges();
            }


            if (page == null)
            {
                page = 1;
            }
            if (pageSize == null)
            {
                pageSize = 10;
            }

            return View(db.HOADONs.ToPagedList((int)page, (int)pageSize));
        }

        public ActionResult Thong_Ke()
        {
            ViewBag.TongDH = TongDH();
            ViewBag.DHDaXN = DonDaXN();
            ViewBag.DonChuaXN = DonChuaXN();
            ViewBag.DHDaGiao = DHDaGiao();
            ViewBag.TongTienThang = TongTienThang(8);


            var query = (from product in db.SANPHAMs
                         join orderLine in db.CHITIET_HDs on product.MASP equals orderLine.MASP
                         join order in db.HOADONs on orderLine.MAHD equals order.MAHD
                         where order.NGAYLAP.Value.Month == 8 //&& order.OrderDate.Year == 2023  // Thay đổi tháng và năm cần kiểm tra
                         group orderLine by new { product.MASP, product.TENSP } into grouped
                         select new ThongKe
                         {

                             masp = grouped.Key.MASP,
                             tensp = grouped.Key.TENSP,
                             sl = grouped.Sum(ol => ol.SL)
                         }).OrderByDescending(x => x.sl).ToList();


            return View(query);
        }

        public ActionResult Update_DHDaGiao(string ma, int? page, int? pageSize)
        {
            HOADON hd = db.HOADONs.Where(x => x.MAHD == ma).FirstOrDefault();
            if (hd != null)
            {
                hd.TRANGTHAI_DH = "Đã giao";
                hd.MANV = 1;
                db.SubmitChanges();
            }


            if (page == null)
            {
                page = 1;
            }
            if (pageSize == null)
            {
                pageSize = 10;
            }

            return View(db.HOADONs.ToPagedList((int)page, (int)pageSize));
        }

        public ActionResult UD_ThongKe(int selectMonth, int selectYear)
        {
            ViewBag.TongDH = TongDH();
            ViewBag.DHDaXN = DonDaXN();
            ViewBag.DonChuaXN = DonChuaXN();
            ViewBag.DHDaGiao = DHDaGiao();
            ViewBag.TongTienThang = TongTienThang(selectMonth);
            ViewBag.Thang = selectMonth;
            ViewBag.Nam = selectYear;

            var query = (from product in db.SANPHAMs
                         join orderLine in db.CHITIET_HDs on product.MASP equals orderLine.MASP
                         join order in db.HOADONs on orderLine.MAHD equals order.MAHD
                         where order.NGAYLAP.Value.Month == selectMonth && order.NGAYLAP.Value.Year == selectYear  // Thay đổi tháng và năm cần kiểm tra
                         group orderLine by new { product.MASP, product.TENSP } into grouped
                         select new ThongKe
                         {

                             masp = grouped.Key.MASP,
                             tensp = grouped.Key.TENSP,
                             sl = grouped.Sum(ol => ol.SL)
                         }).OrderByDescending(x => x.sl).ToList();


            return View(query);
        }

        public int TongDH()
        {
            int sodh = (from dh in db.HOADONs select dh).Count();
            return sodh;
        }

        public int DonDaXN()
        {
            int dondxn = (from dh in db.HOADONs
                          where dh.TRANGTHAI_DH == "Đã xác nhận"
                          select dh).Count();
            return dondxn;

        }
        public int DHDaGiao()
        {
            int dondxn = (from dh in db.HOADONs
                          where dh.TRANGTHAI_DH == "Đã giao"
                          select dh).Count();
            return dondxn;

        }
        public int DonChuaXN()
        {
            int doncxn = (from dh in db.HOADONs
                          where dh.TRANGTHAI_DH == "Chờ xác nhận"
                          select dh).Count();
            return doncxn;
        }
        public int HuyDH()
        {
            int doncxn = (from dh in db.HOADONs
                          where dh.TRANGTHAI_DH == "Đã hủy"
                          select dh).Count();
            return doncxn;
        }

    

        public int? TongTienThang(int thang)
        {

            var tt = (from dh in db.HOADONs
                      where dh.NGAYLAP.Value.Month == thang
                      select dh.TONGTIEN).Sum();

            return tt;
        }

        public int? TongSP()
        {

            var tt = (from dh in db.SANPHAMs
                      select dh).Count();
                      
                     

            return tt;
        }


    }
}