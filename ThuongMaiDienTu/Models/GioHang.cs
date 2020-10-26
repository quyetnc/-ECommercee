using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ThuongMaiDienTu.Models
{
    public class GioHang
    {
        THUONGMAIDIENTUEntities db  = new THUONGMAIDIENTUEntities();

        public int iID_Product { get; set; }

        public string sName { get; set; }

        public string sImage { get; set; }

        public double dPrice { get; set; }

        public int iQuantity { get; set; }
        public double dThanhTien
        {
            get { return iQuantity * dPrice; }
        }

        public GioHang(int IdProduct)
        {
            iID_Product = IdProduct;
            PRODUCT dt = db.PRODUCTs.Single(n => n.IdProduct == iID_Product);
            sName = dt.ProductName;
            sImage = dt.PRODUCT_IMG.FirstOrDefault().Filename;
            dPrice = double.Parse(dt.ProductPrice.ToString());
            iQuantity = 1;
        }
    }
}