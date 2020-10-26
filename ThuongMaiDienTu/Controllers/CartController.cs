using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThuongMaiDienTu.Helper;
using ThuongMaiDienTu.Models;

namespace ThuongMaiDienTu.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(OrderInfoModel data)
        {
            try
            {
                if (String.IsNullOrEmpty(data.CustomerPhone) || String.IsNullOrEmpty(data.CustomerName) || String.IsNullOrEmpty(data.CustomerAddress)) throw new Exception("Vui lòng điền đầy đủ thông tin trước khi đặt hàng");
                var list = (Session["cart"] as List<PRODUCT>);
                if (list is null) throw new Exception("Đơn hàng rỗng không thể thanh toán");
                if (list.Count < 1) throw new Exception("Đơn hàng rỗng không thể thanh toán");

                using (THUONGMAIDIENTUEntities db = new THUONGMAIDIENTUEntities())
                {
                    // tìm và update khách nếu ko thì tạo
                    var customer = db.CUSTOMERs.Where(x => x.CustomerPhone.Equals(data.CustomerPhone)).FirstOrDefault();
                    if (customer is null) customer = new CUSTOMER();
                    customer.CustomerPhone = data.CustomerPhone;
                    customer.CustomerAddress = data.CustomerAddress;
                    customer.CustomerName = data.CustomerName;
                    customer.CustomerEmail = data.CustomerEmail;
                    if (customer.IdCustomer < 1) db.CUSTOMERs.Add(customer);
                    db.SaveChanges();

                    ORDER o = new ORDER();
                    o.IdCustomer = customer.IdCustomer;
                    o.DateOrder = DateTime.Now;
                    o.DateDelivery = DateTime.Now;
                    o.OrderComment = data.CustomerComment;
                    o.IdPayment = data.PaymentMethod;
                    o.IdStatus = 1;
                    int total = 0;

                    foreach (var p in list)
                    {
                        total += (int)p.ProductPrice;
                        PRODUCT_ORDER po = new PRODUCT_ORDER();
                        po.Count = 1;
                        po.Discount = 0;
                        po.IdProduct = p.IdProduct;
                        po.Price = p.ProductPrice;
                        o.PRODUCT_ORDER.Add(po);
                    }

                    o.Total = total;

                    db.ORDERs.Add(o);
                    db.SaveChanges();

                    if (data.PaymentMethod == 2)
                    {
                        VNPay vnpay = new VNPay();
                        string link = vnpay.GetBankingURL(total, data.BankCode, o.IdOrder.ToString(), DateTime.Now);
                        return Redirect(link);
                    }
                    TempData["id"] = o.IdOrder;
                    return RedirectToAction("Success");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return View();
        }


        public ActionResult Success()
        {
            if (TempData["id"] is null) return RedirectToAction("Index", "Home");
            ViewBag.Id = TempData["id"].ToString();
            Session["cart"] = null;
            return View();
        }
        public ActionResult Fail()
        {
            return View();
        }


        public ActionResult Callback()
        {
            if (Request.QueryString.Count > 0)
            {

                var vnpayData = Request.QueryString;
                VnPayLibrary vnpay = new VnPayLibrary();
                //if (vnpayData.Count > 0)
                //{
                foreach (string s in vnpayData)
                {
                    //get all querystring data
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(s, vnpayData[s]);
                    }
                }
                // }

                //vnp_TxnRef: Ma don hang merchant gui VNPAY tai command=pay    
                string orderId = vnpay.GetResponseData("vnp_TxnRef");

                // lấy ra idkiosk để lấy hashsecret trong kiosk
                string vnp_HashSecret = "NZWQLJRMIMACCBLXUUHZXWRDPRMJPIVQ";
                int IdTransaction = int.Parse(orderId);
                int IdKiosk = 0;
                DateTime payDate = DateTime.Now;

                //vnp_TransactionNo: Ma GD tai he thong VNPAY
                long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
                //vnp_ResponseCode:Response code from VNPAY: 00: Thanh cong, Khac 00: Xem tai lieu
                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                //vnp_SecureHash: MD5 cua du lieu tra ve
                String vnp_SecureHash = Request.QueryString["vnp_SecureHash"];


                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
                if (checkSignature)
                {

                    if (vnp_ResponseCode.Equals("00"))
                    {
                        //Thanh toan thanh cong

                        using (THUONGMAIDIENTUEntities db = new THUONGMAIDIENTUEntities())
                        {
                            var order = db.ORDERs.Where(x => x.IdOrder == IdTransaction).FirstOrDefault();
                            order.IdStatus = 2;
                            db.SaveChanges();
                            TempData["id"] = IdTransaction;
                            return RedirectToAction("Success");
                        }

                    }
                }

            }
            return RedirectToAction("Fail");

        }
    }
}