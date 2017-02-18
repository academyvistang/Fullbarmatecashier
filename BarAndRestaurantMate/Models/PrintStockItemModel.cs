using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BarAndRestaurantMate.Models
{
    public class PrintStockItemModel
    {
        public int Quantity { get; set; }

        public string Description { get; set; }

        public decimal UnitPrice { get; set; }

        public DateTime DateSold { get; set; }
    }
}