using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BarAndRestaurantMate.Models
{
    public class ReceiptModel
    {
        public List<HotelMateWeb.Dal.DataCore.SoldItemsAll> Items { get; set; }

        public string RecieptNumber { get; set; }

        public string ValueIds { get; set; }

        public DateTime? SaleTime { get; set; }

        public bool BusinessAccount { get; set; }
    }
}