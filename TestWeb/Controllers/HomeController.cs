using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TestWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //BU BİLGİLERİ PARATİKA PANELİNDEN ALACAKSINIZ
            string MERCHANT = "";
            string MERCHANTUSER = "";
            string MERCHANTPASSWORD = "";
            //BU BİLGİLERİ PARATİKA PANELİNDEN ALACAKSINIZ

            string hostHeader = Request.Headers["host"];
            string _3dUrl = string.Format("{0}://{1}/{2}",
               Request.Url.Scheme,
               hostHeader,
               "/Home/RETURN_3D").ToString();
            ParatikaNet.Paratika3D p = new ParatikaNet.Paratika3D(MERCHANT, MERCHANTUSER, MERCHANTPASSWORD, _3dUrl, Request.UserHostAddress);

            var tokenResponse = p.GetSessionToken(new ParatikaNet.PaymentModel
            {
                paymentId = Guid.NewGuid().ToString(),
                billingAddress = new ParatikaNet.PaymentAddress { address = "Fatih Sultan Mehmet Mah. Asude Sok. NO:20", city = "İstanbul", country = "Türkiye", phone = "+901234567890", postalCode = "" },
                shippingAddress = new ParatikaNet.PaymentAddress { address = "Fatih Sultan Mehmet Mah. Asude Sok. NO:20", city = "İstanbul", country = "Türkiye", phone = "+901234567890", postalCode = "" },
                customerEmail = "test@test.com",
                customerName = "Ersin Terzi",
                customerPhone = "+901234567890",
                totalPrice = 2000,
                carts = new List<ParatikaNet.PaymentCart>
                   {
                      new ParatikaNet.PaymentCart
                      {
                           description="Test Ürün Açıklama",
                            quantity=1,
                             stockCode="t001",
                              title="Test Ürün",
                               unitPrice=1000
                      },
                        new ParatikaNet.PaymentCart
                      {
                           description="Test Ürün Açıklama",
                            quantity=2,
                             stockCode="t002",
                              title="Test Ürün2",
                               unitPrice=500
                      }
                   }
            });
            if (tokenResponse.isSuccess)
            {
                TempData["token"] = tokenResponse.sessionToken;
            }
            return View();
        }

        public ActionResult RETURN_3D()
        {
            string getform = "";
            foreach (string item in Request.Form)
            {
                getform += item + " : " +HttpUtility.UrlDecode( Request.Form[item]) +"</br>";
            }
            TempData["data"] = getform;
            return View();
        }
    }
}