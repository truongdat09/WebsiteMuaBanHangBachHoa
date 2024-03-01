using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiniF4Store.Models
{
    public class GioHang
    {
        QL_BachHoaDataContext db = new QL_BachHoaDataContext();
        public int MASP { get; set; }
        public int MANH { get; set; }
        public string TENSP { get; set; }
        public string HINH { get; set; }
        public int SOLUONG { get; set; }
        public int GIABAN { get; set; }
        public int SOLUONG_ACTUALLY { get; set; }

        public KhachHang khachhang { get; set; }
        public double ThanhTien
        {
            get { return (SOLUONG * GIABAN); }
        }
        public KhachHang layTTKH(string sdt)
        {
            khachhang = new KhachHang(sdt);
            return khachhang;
        }
        public GioHang(int ma, string sdt)
        {
            MASP = ma;
            SANPHAM sp = db.SANPHAMs.Single(s => s.MASP == MASP);
            TENSP = sp.TENSP;
            GIABAN = int.Parse(sp.GIABAN.ToString());
            SOLUONG = 1;
            SOLUONG_ACTUALLY = int.Parse(sp.SOLUONG.ToString());
            layTTKH(sdt);

        }

    }
}