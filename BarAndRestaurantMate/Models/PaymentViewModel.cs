using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BarAndRestaurantMate.Models
{
    public class PaymentViewModel
    {
        public string OrderId { get; set; }

        public string CustomerEmail { get; set; }

        public string Amount { get; set; }

        public string AmountToPay { get; set; }

        public decimal amt { get; set; }

        public string MerchantId { get; set; }
    }
}