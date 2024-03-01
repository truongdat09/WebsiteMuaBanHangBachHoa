using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiniF4Store.Models
{
    public class KhachHang
    {
        QL_BachHoaDataContext db = new QL_BachHoaDataContext();
        public string SDT_KH { get; set; }
        public string TENKH { get; set; }
        public string DIACHI { get; set; }
        public string EMAIL { get; set; }
        public string MATKHAU { get; set; }

        public KhachHang(string sdt)
        {
            SDT_KH = sdt;
            KHACHHANG kh = db.KHACHHANGs.Single(s => s.SDT_KH == SDT_KH);
            TENKH = kh.TENKH;
            DIACHI = kh.DIACHI;
            EMAIL = kh.EMAIL;
            MATKHAU = kh.MATKHAU;
        }
    }
}