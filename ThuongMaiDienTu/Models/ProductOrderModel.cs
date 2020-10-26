using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ThuongMaiDienTu.Models
{
    public class ProductOrderModel
    {
        public int? IdProduct { get; set; }
        public int? IdOrder { get; set; }
        public string IMEI { get; set; }
    }
}