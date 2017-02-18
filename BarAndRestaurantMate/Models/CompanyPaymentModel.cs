using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BarAndRestaurantMate.Models
{
    public class CompanyPaymentModel
    {
        public decimal Bill { get; set; }

        public decimal Payments { get; set; }

        public decimal Balance { get; set; }

        public string Name { get; set; }
    }
}