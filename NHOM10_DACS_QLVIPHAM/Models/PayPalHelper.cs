using PayPal.Api;
using System;
using System.Collections.Generic;

namespace NHOM10_DACS_QLVIPHAM.Models
{
    public class PayPalHelper
    {
        private readonly PayPalConfig _config;

        public PayPalHelper(PayPalConfig config)
        {
            _config = config;
        }

        public Dictionary<string, string> GetConfigDictionary()
        {
            return new Dictionary<string, string> 
            { 
                { "mode", _config.Mode },
                { "connectionTimeout", "360000" },
                { "requestRetries", "1" }
            };
        }

        public APIContext GetAPIContext()
        {
            try
            {
                var configDictionary = GetConfigDictionary();
                var accessToken = new OAuthTokenCredential(_config.ClientId, _config.ClientSecret, configDictionary).GetAccessToken();
                var apiContext = new APIContext(accessToken) { Config = configDictionary };
                return apiContext;
            }
            catch (Exception ex)
            {
                // Log lỗi
                Console.WriteLine($"Không thể tạo API Context: {ex.Message}");
                throw new Exception($"Không thể kết nối đến PayPal. Lỗi: {ex.Message}", ex);
            }
        }

        public string CreatePayment(decimal amount, string description, string returnUrl, string cancelUrl)
        {
            try
            {
                var apiContext = GetAPIContext();

                var payer = new Payer() { payment_method = "paypal" };
                var redirectUrls = new RedirectUrls()
                {
                    cancel_url = cancelUrl,
                    return_url = returnUrl
                };

                var transactionList = new List<Transaction>
                {
                    new Transaction()
                    {
                        amount = new Amount() { currency = "USD", total = amount.ToString("F2") },
                        description = description
                    }
                };

                var payment = new Payment()
                {
                    intent = "sale",
                    payer = payer,
                    transactions = transactionList,
                    redirect_urls = redirectUrls
                };

                var createdPayment = payment.Create(apiContext);
                var approvalUrl = createdPayment.links.Find(l => l.rel.ToLower() == "approval_url")?.href;

                if (string.IsNullOrEmpty(approvalUrl))
                    throw new Exception("Không thể tạo URL thanh toán PayPal");

                return approvalUrl;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi tạo thanh toán PayPal: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                throw;
            }
        }

        public Payment ExecutePayment(string paymentId, string payerId)
        {
            try
            {
                var apiContext = GetAPIContext();
                var payment = Payment.Get(apiContext, paymentId);
                var execution = new PaymentExecution() { payer_id = payerId };
                return payment.Execute(apiContext, execution);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi thực hiện thanh toán PayPal: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                throw;
            }
        }

        public bool VerifyPayment(string paymentId)
        {
            try
            {
                var apiContext = GetAPIContext();
                var payment = Payment.Get(apiContext, paymentId);
                return payment.state.ToLower() == "approved";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi xác minh thanh toán PayPal: {ex.Message}");
                return false;
            }
        }
    }
} 