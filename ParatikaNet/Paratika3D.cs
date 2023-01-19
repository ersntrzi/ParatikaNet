using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
namespace ParatikaNet
{
    public class Paratika3D
    {
        private string _merchant_id = "";
        private string _merchant_user = "";
        private string _merchant_password = "";
        private string _return_3d_url = "";
        private string _paratika_url = "";
        private string _clientIp = "";
        public Paratika3D(string merchantId, string merchantUser, string merchantPassword, string return_3d_url, string clientIp, string paratika_url = "https://entegrasyon.paratika.com.tr/paratika/api/v2")
        {
            _merchant_id = merchantId;
            _merchant_user = merchantUser;
            _merchant_password = merchantPassword;
            _return_3d_url = return_3d_url;
            if (string.IsNullOrEmpty(clientIp))
                _clientIp = "127.0.0.1";
            else
                _clientIp = clientIp;
            if (!string.IsNullOrEmpty(paratika_url))
                _paratika_url = paratika_url;
        }

        public ParatikaNetResponse GetSessionToken(PaymentModel paymentModel)
        {
            ParatikaNetResponse paratikaResponse = new ParatikaNetResponse();
            try
            {
                var response = prepareSessionTokenParameters(paymentModel);
                String requestData = convertToRequestData(response);
                var resp = getConnection(_paratika_url, requestData);
                if (resp.Item1)
                {
                    var pResponse = JObject.Parse(resp.Item2);
                    if (pResponse["sessionToken"] != null && pResponse["sessionToken"].ToString() != "")
                    {
                        paratikaResponse.isSuccess = true;
                        paratikaResponse.sessionToken = pResponse["sessionToken"].ToString();
                        return paratikaResponse;
                    }
                }
                else
                {
                    paratikaResponse.errorMessage = resp.Item2;
                    paratikaResponse.isSuccess = false;
                }

            }
            catch (Exception e)
            {
                paratikaResponse.errorMessage = e.Message;
                paratikaResponse.isSuccess = false;
            }
            return paratikaResponse;
        }
        private static Tuple<bool, string> getConnection(String url, String reqMsg)
        {
            String outputData = System.String.Empty;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                System.Net.ServicePointManager.Expect100Continue = false;
                byte[] data = Encoding.UTF8.GetBytes(reqMsg);
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
                request.KeepAlive = false;
                HttpWebResponse response =
                (HttpWebResponse)request.GetResponse();
                Stream streamResponse = response.GetResponseStream();
                StreamReader streamRead = new StreamReader(streamResponse);
                String read = streamRead.ReadToEnd();
                outputData = read;
                streamResponse.Close();
                streamRead.Close();
                response.Close();
                return new Tuple<bool, string>(true, read);

            }
            catch (WebException e)
            {
                return new Tuple<bool, string>(false, e.Message);
            }
            catch (Exception e)
            {
                return new Tuple<bool, string>(false, e.Message);
            }

        }
        private static String convertToRequestData(Dictionary<String, String> requestParameters)
        {
            StringBuilder requestData = new StringBuilder();
            foreach (KeyValuePair<string, string> entry in requestParameters)
            {

                var key = UrlEncodeExtended(entry.Key);
                var value = UrlEncodeExtended(entry.Value);
                requestData.Append(key + "=" + value + "&");

            }
            return requestData.ToString();
        }

        public static string UrlEncodeExtended(string value)
        {
            char[] chars = value.ToCharArray();
            StringBuilder encodedValue = new StringBuilder();
            foreach (char c in chars)
            {
                encodedValue.Append("%" + ((int)c).ToString("X2"));
            }
            return encodedValue.ToString();
        }
        private static String encodeParameter(String parameterValue)
        {
            String encodedValue = System.String.Empty;
            try
            {
                if (parameterValue != null)
                {
                    encodedValue = UrlEncodeExtended(parameterValue);
                }
            }
            catch (Exception ex)
            {
                // log.Info(" ParatikaUtil --> encodeParameter(parameterValue) --> UnsupportedEncodingException ");

            }
            return encodedValue;
        }
        private Dictionary<String, String> prepareSessionTokenParameters(PaymentModel paymentModel)
        {
            Dictionary<String, String> requestParameters = new Dictionary<String, String>();
            requestParameters.Add("ACTION", "SESSIONTOKEN");
            requestParameters.Add("MERCHANTPAYMENTID", paymentModel.paymentId);
            requestParameters.Add("AMOUNT", paymentModel.totalPrice.ToString("0"));
            requestParameters.Add("CURRENCY", "TRY");
            requestParameters.Add("SESSIONTYPE", "PAYMENTSESSION");
            requestParameters.Add("RETURNURL", _return_3d_url);
            JArray oItems = new JArray();
            foreach (var it in paymentModel.carts)
            {
                JObject item = new JObject();
                item.Add("code", it.stockCode);
                item.Add("name", it.title);
                item.Add("quantity", it.quantity);
                item.Add("description", it.description);
                item.Add("amount", it.unitPrice.ToString("0"));
                oItems.Add(item);
            }
            requestParameters.Add("ORDERITEMS", encodeParameter(oItems.ToString()));
            JObject extra = new JObject();
            extra.Add("IntegrationModel", "API");
            extra.Add("AlwaysSaveCard", "false");

            requestParameters.Add("MERCHANT", _merchant_id);
            requestParameters.Add("MERCHANTUSER", _merchant_user);
            requestParameters.Add("MERCHANTPASSWORD", _merchant_password);

            requestParameters.Add("CUSTOMER", Guid.NewGuid().ToString());
            requestParameters.Add("CUSTOMERNAME", paymentModel.customerName);
            requestParameters.Add("CUSTOMEREMAIL", paymentModel.customerEmail);
            requestParameters.Add("CUSTOMERPHONE", paymentModel.customerPhone);
            requestParameters.Add("CUSTOMERIP", _clientIp);
            if (!string.IsNullOrEmpty(paymentModel.billingAddress.address))
                requestParameters.Add("BILLTOADDRESSLINE", paymentModel.billingAddress.address);
            if (!string.IsNullOrEmpty(paymentModel.billingAddress.city))
                requestParameters.Add("BILLTOCITY", paymentModel.billingAddress.city);
            if (!string.IsNullOrEmpty(paymentModel.billingAddress.country))
                requestParameters.Add("BILLTOCOUNTRY", paymentModel.billingAddress.country);
            if (!string.IsNullOrEmpty(paymentModel.billingAddress.postalCode))
                requestParameters.Add("BILLTOPOSTALCODE", paymentModel.billingAddress.postalCode);
            if (!string.IsNullOrEmpty(paymentModel.billingAddress.phone))
                requestParameters.Add("BILLTOPHONE", paymentModel.billingAddress.phone);


            if (!string.IsNullOrEmpty(paymentModel.shippingAddress.address))
                requestParameters.Add("SHIPTOADDRESSLINE", paymentModel.shippingAddress.address);
            if (!string.IsNullOrEmpty(paymentModel.shippingAddress.city))
                requestParameters.Add("SHIPTOCITY", paymentModel.shippingAddress.city);
            if (!string.IsNullOrEmpty(paymentModel.shippingAddress.country))
                requestParameters.Add("SHIPTOCOUNTRY", paymentModel.shippingAddress.country);
            if (!string.IsNullOrEmpty(paymentModel.shippingAddress.postalCode))
                requestParameters.Add("SHIPTOPOSTALCODE", paymentModel.shippingAddress.postalCode);
            if (!string.IsNullOrEmpty(paymentModel.shippingAddress.phone))
                requestParameters.Add("SHIPTOPHONE", paymentModel.shippingAddress.phone);

            return requestParameters;
        }
    }
}
