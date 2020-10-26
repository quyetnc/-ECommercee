using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ThuongMaiDienTu.Models
{
    public class OrderInfoModel
    {
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerComment { get; set; }
        public string Giftcode { get; set; }
        public string BankCode { get; set; }
        public int PaymentMethod { get; set; }
    }
}