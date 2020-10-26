using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace ThuongMaiDienTu.Helper
{
    public class VNPay
    {
        public string URL_VNPAY_INTERNET_BANKING = "http://sandbox.vnpayment.vn/paymentv2/vpcpay.html";

        public string VNPAY_TMNCODE = "CTEE0001";
        public string VNPAY_HASH_SECRECT = "NZWQLJRMIMACCBLXUUHZXWRDPRMJPIVQ";
        public string VNPAY_VERSION = "2.0.0";
        public string URL_VNPAY_REFUND;

        public string GetBankingURL(double amount, string bankcode, string idbill, DateTime createDate, string language = "vn")
        {

            //Build URL for VNPAY
            VnPayLibrary vnpay = new VnPayLibrary();
            string vnp_HashSecret = VNPAY_HASH_SECRECT;
            string vnp_TmnCode = VNPAY_TMNCODE;
            vnpay.AddRequestData("vnp_Locale", language);
            vnpay.AddRequestData("vnp_Version", VNPAY_VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_TxnRef", idbill);
            vnpay.AddRequestData("vnp_OrderInfo", "He thong yeu cau thanh toan cho hoa don " + idbill);
            vnpay.AddRequestData("vnp_OrderType", "250006"); //default value: other
            vnpay.AddRequestData("vnp_Amount", (amount * 100).ToString());
            vnpay.AddRequestData("vnp_ReturnUrl", HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "/Cart/Callback");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
            vnpay.AddRequestData("vnp_CreateDate", createDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_BankCode", bankcode);
            string paymentUrl = vnpay.CreateRequestUrl(URL_VNPAY_INTERNET_BANKING, vnp_HashSecret);

            return paymentUrl;

        }

        public void Refund(long IDVNPay, string OrderId, DateTime payDate, float Amount, string info)
        {

            var vnpay_api_url = URL_VNPAY_REFUND;
            var vnpHashSecret = VNPAY_HASH_SECRECT;
            string vnp_TmnCode = VNPAY_TMNCODE;
            var vnpay = new VnPayLibrary();
            var createDate = DateTime.Now;
            var strDatax = "";

            try

            {

                var amountrf = Convert.ToInt32(Amount) * 100;
                vnpay.AddRequestData("vnp_Version", VNPAY_VERSION);
                vnpay.AddRequestData("vnp_Command", "refund");
                vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);

                vnpay.AddRequestData("vnp_CreateBy", "kiosk");
                vnpay.AddRequestData("vnp_TransactionType", "02");
                vnpay.AddRequestData("vnp_TxnRef", OrderId);
                vnpay.AddRequestData("vnp_Amount", amountrf.ToString());
                vnpay.AddRequestData("vnp_OrderInfo", info);
                vnpay.AddRequestData("vnp_TransDate", payDate.ToString("yyyyMMddHHmmss"));

                vnpay.AddRequestData("vnp_CreateDate", createDate.ToString("yyyyMMddHHmmss"));
                vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());



                var paymentUrl = vnpay.CreateRequestUrl(vnpay_api_url, vnpHashSecret);

                var request = (HttpWebRequest)WebRequest.Create(paymentUrl);
                request.AutomaticDecompression = DecompressionMethods.GZip;
                using (var response = (HttpWebResponse)request.GetResponse())
                using (var stream = response.GetResponseStream())
                    if (stream != null)
                        using (var reader = new StreamReader(stream))
                        {
                            strDatax = reader.ReadToEnd();
                        }
                var data = strDatax.Split('&');

                //amount
                var vnpAmount = data[0].Split('=');
                var amount = int.Parse(vnpAmount[1]);

                // ResponseCode
                var vnpResponseCode = data[5].Split('=');
                var responsecode = vnpResponseCode[1];

                // Vnpay TransactionStatus
                var vnpTransactionStatus = data[8].Split('=');
                var transactionstatus = vnpTransactionStatus[1];

                // OrderID
                var vnpTxnRef = data[10].Split('=');
                var txnref = vnpTxnRef[1];
                if (responsecode.Equals("00")) return;
                throw new Exception(strDatax);
            }
            catch (Exception ex)
            {
                throw new Exception(strDatax);
            }
        }


        public bool Check(string OrderId, DateTime payDate)
        {

            var vnpay_api_url = URL_VNPAY_REFUND;
            var vnpHashSecret = VNPAY_HASH_SECRECT;
            string vnp_TmnCode = VNPAY_TMNCODE;
            var vnpay = new VnPayLibrary();
            var createDate = DateTime.Now;
            var strDatax = "";

            try

            {
                vnpay.AddRequestData("vnp_Version", VNPAY_VERSION);
                vnpay.AddRequestData("vnp_Command", "querydr");
                vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
                vnpay.AddRequestData("vnp_Merchant", "VNPAY");
                vnpay.AddRequestData("vnp_TxnRef", OrderId);
                vnpay.AddRequestData("vnp_OrderInfo", "queryDr ma GD:" + OrderId);
                vnpay.AddRequestData("vnp_TransDate", payDate.ToString("yyyyMMddHHmmss"));
                vnpay.AddRequestData("vnp_CreateDate", createDate.ToString("yyyyMMddHHmmss"));
                vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());

                var queryDr = vnpay.CreateRequestUrl(vnpay_api_url, vnpHashSecret);
                var request = (HttpWebRequest)WebRequest.Create(vnpay_api_url);
                request.AutomaticDecompression = DecompressionMethods.GZip;
                using (var response = (HttpWebResponse)request.GetResponse())
                using (var stream = response.GetResponseStream())
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            strDatax = reader.ReadToEnd();
                        }
                    }
                }


                var data = strDatax.Split('&');

                //amount
                var vnpAmount = data[0].Split('=');
                var amount = int.Parse(vnpAmount[1]) / 100;

                // ResponseCode
                var vnpResponseCode = data[5].Split('=');
                var responsecode = vnpResponseCode[1];

                // Vnpay TransactionStatus
                var vnpTransactionStatus = data[8].Split('=');
                var transactionstatus = vnpTransactionStatus[1];

                // OrderID
                var vnpTxnRef = data[10].Split('=');
                var txnref = vnpTxnRef[1];

                if (transactionstatus.Equals("00")) return true;
                return false; 
            }
            catch
            {
                return false;
            }
        }


        internal string GetBankingURL(double totalMoney, string paymentData, object idTicket, string v)
        {
            throw new NotImplementedException();
        }
    }

    public class VnPayLibrary
    {
        private SortedList<String, String> _requestData = new SortedList<String, String>(new VnPayCompare());
        private SortedList<String, String> _responseData = new SortedList<String, String>(new VnPayCompare());

        public void AddRequestData(string key, string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                _requestData.Add(key, value);
            }
        }

        public void AddResponseData(string key, string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                _responseData.Add(key, value);
            }
        }

        public string GetResponseData(string key)
        {
            string retValue;
            if (_responseData.TryGetValue(key, out retValue))
            {
                return retValue;
            }
            else
            {
                return string.Empty;
            }
        }

        #region Request

        public string CreateRequestUrl(string baseUrl, string vnp_HashSecret)
        {
            StringBuilder data = new StringBuilder();
            foreach (KeyValuePair<string, string> kv in _requestData)
            {
                if (!String.IsNullOrEmpty(kv.Value))
                {
                    data.Append(kv.Key + "=" + HttpUtility.UrlEncode(kv.Value) + "&");
                }
            }
            string queryString = data.ToString();
            string rawData = GetRequestRaw();
            baseUrl += "?" + queryString;
            string vnp_SecureHash = Utils.Md5(vnp_HashSecret + rawData);
            baseUrl += "vnp_SecureHash=" + vnp_SecureHash;
            return baseUrl;
        }

        private string GetRequestRaw()
        {
            StringBuilder data = new StringBuilder();
            foreach (KeyValuePair<string, string> kv in _requestData)
            {
                if (!String.IsNullOrEmpty(kv.Value))
                {
                    data.Append(kv.Key + "=" + kv.Value + "&");
                }
            }
            //remove last '&'
            if (data.Length > 0)
            {
                data.Remove(data.Length - 1, 1);
            }
            return data.ToString();
        }

        #endregion

        #region Response process

        public bool ValidateSignature(string inputHash, string secretKey)
        {
            string rspRaw = GetResponseRaw();
            string myChecksum = Utils.Md5(secretKey + rspRaw);
            return myChecksum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
        }
        private string GetResponseRaw()
        {

            StringBuilder data = new StringBuilder();
            if (_responseData.ContainsKey("vnp_SecureHashType"))
            {
                _responseData.Remove("vnp_SecureHashType");
            }
            if (_responseData.ContainsKey("vnp_SecureHash"))
            {
                _responseData.Remove("vnp_SecureHash");
            }
            foreach (KeyValuePair<string, string> kv in _responseData)
            {
                if (!String.IsNullOrEmpty(kv.Value))
                {
                    data.Append(kv.Key + "=" + kv.Value + "&");
                }
            }
            //remove last '&'
            if (data.Length > 0)
            {
                data.Remove(data.Length - 1, 1);
            }
            return data.ToString();
        }

        #endregion
    }

    public class Utils
    {
        public static string Md5(string sInput)
        {
            HashAlgorithm algorithmType = default(HashAlgorithm);
            ASCIIEncoding enCoder = new ASCIIEncoding();
            byte[] valueByteArr = enCoder.GetBytes(sInput);
            byte[] hashArray = null;
            // Encrypt Input string 
            algorithmType = new MD5CryptoServiceProvider();
            hashArray = algorithmType.ComputeHash(valueByteArr);
            //Convert byte hash to HEX
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hashArray)
            {
                sb.AppendFormat("{0:x2}", b);
            }
            return sb.ToString();
        }
        public static string GetIpAddress()
        {
            string ipAddress;
            try
            {
                ipAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                if (string.IsNullOrEmpty(ipAddress) || (ipAddress.ToLower() == "unknown"))
                    ipAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            catch (Exception ex)
            {
                ipAddress = "Invalid IP:" + ex.Message;
            }

            return ipAddress;
        }
    }

    public class VnPayCompare : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            if (x == y) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            var vnpCompare = CompareInfo.GetCompareInfo("en-US");
            return vnpCompare.Compare(x, y, CompareOptions.Ordinal);
        }
    }
}