using HotelMateWeb.Dal.DataCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BarAndRestaurantMate.Models
{

    public class FeesPaymentModel 
    {

        public string OrderId { get; set; }

        public string CustomerEmail { get; set; }

        public string Amount { get; set; }

        public string AmountToPay { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Amount entered must be greater than zero")]
        public decimal amt { get; set; }

        public string MerchantId { get; set; }

        public PaymentOrder PurchaseOrder { get; set; }

        public List<PaymentOrder> POList { get; set; }

        public int StudentId { get; set; }
    }
}