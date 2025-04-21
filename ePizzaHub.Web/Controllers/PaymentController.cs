﻿using ePizzaHub.Core.Database.Entities;
using ePizzaHub.Models;
using ePizzaHub.Services.Interfaces;
using ePizzaHub.Web.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace ePizzaHub.Web.Controllers
{
    public class PaymentController : BaseController
    {
        IConfiguration _configuration;
        IPaymentService _paymentService;
        public PaymentController(IConfiguration configuration, IPaymentService paymetService)
        {
            _configuration = configuration;
            _paymentService = paymetService;
        }

        public IActionResult Index()
        {
            PaymentModel payment = new PaymentModel();
            CartModel cart = TempData.Peek<CartModel>("Cart");
            if (cart != null)
            {
                payment.Cart = cart;
                payment.GrandTotal = Math.Round(cart.GrandTotal);
                payment.Currency = "INR";
                payment.Description = string.Join(",", cart.Items.Select(x => x.Name));
                payment.RazorpayKey = _configuration["Razorpay:Key"];
                payment.Receipt = Guid.NewGuid().ToString();

                payment.OrderId = _paymentService.CreateOrder(payment.GrandTotal * 100, payment.Currency, payment.Description);
            }
            return View(payment);
        }

        public IActionResult Status(IFormCollection form)
        {
            try
            {
                if (form.Keys.Count > 0 && !string.IsNullOrWhiteSpace(form["rzp_paymentid"]))
                {
                    string paymentId = form["rzp_paymentid"];
                    string orderId = form["rzp_orderid"];
                    string signature = form["rzp_signature"];
                    string transactionId = form["Receipt"];
                    string currency = form["Currency"];

                    var payment = _paymentService.GetPaymentDetails(paymentId);
                    bool IsSignVerified = _paymentService.VerifySignature(signature, orderId, paymentId);

                    if (IsSignVerified && payment != null)
                    {
                        CartModel cart = TempData.Get<CartModel>("Cart");
                        PaymentDetail model = new PaymentDetail();

                        model.CartId = cart.Id;
                        model.Total = cart.Total;
                        model.Tax = cart.Tax;
                        model.GrandTotal = cart.GrandTotal;
                        model.CreatedDate = DateTime.Now;

                        model.Status = payment.Attributes["status"]; //captured
                        model.TransactionId = transactionId;
                        model.Currency = payment.Attributes["currency"];
                        model.Email = payment.Attributes["email"];
                        model.Id = paymentId;
                        model.UserId = CurrentUser.Id;

                        int status = _paymentService.SavePaymentDetails(model);
                        if (status > 0)
                        {
                            Response.Cookies.Append("CId", ""); //resetting cartId in cookie

                            AddressModel address = TempData.Get<AddressModel>("Address");
                            //_orderService.PlaceOrder(CurrentUser.Id, orderId, paymentId, cart, address);

                            //TO DO: Send email
                            TempData.Set("PaymentDetails", model);
                            return RedirectToAction("Receipt");
                        }
                        else
                        {
                            ViewBag.Message = "Due to some technical issues we are not able to receive order details. We will contact you soon..";
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            ViewBag.Message = "Your payment has been faild, please contact us at support@epizzahub.com";
            return View();
        }
        public IActionResult Receipt()
        {
            PaymentDetail payment = TempData.Peek<PaymentDetail>("PaymentDetails");
            return View(payment);
        }
    }

}
