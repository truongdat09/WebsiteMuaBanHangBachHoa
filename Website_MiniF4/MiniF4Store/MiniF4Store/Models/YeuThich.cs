using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiniF4Store.Models
{
    public class YeuThich
    {
        QL_BachHoaDataContext db = new QL_BachHoaDataContext();
        public int MASP { get; set; }
        public string TENSP { get; set; }
        public int SOLUONG { get; set; }
        public int GIABAN { get; set; }
        public YeuThich(int ma)
        {
            MASP = ma;
            SANPHAM sp = db.SANPHAMs.Single(s => s.MASP == MASP);
            TENSP = sp.TENSP;
            GIABAN = int.Parse(sp.GIABAN.ToString());
            SOLUONG = int.Parse(sp.SOLUONG.ToString()); 
        }
    }

}