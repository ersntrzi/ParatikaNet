using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParatikaNet
{
    public class PaymentModel
    {
        public string paymentId { get; set; }
        public double totalPrice { get; set; }
        public List<PaymentCart> carts { get; set; }
        public string customerName { get; set; }
        public string customerEmail { get; set; }
        public string customerPhone { get; set; }
        public PaymentAddress shippingAddress { get; set; }
        public PaymentAddress billingAddress { get; set; }
    }
    public class PaymentAddress
    {
        public string address { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string postalCode { get; set; }
        public string phone { get; set; }
    }
    public class PaymentCart
    {
        public string stockCode { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public int quantity { get; set; }
        public double unitPrice { get; set; }
    }
    public class ParatikaNetResponse
    {
        public bool isSuccess { get; set; }
        public string errorMessage { get; set; }
        public string sessionToken { get; set; }

    }
}
