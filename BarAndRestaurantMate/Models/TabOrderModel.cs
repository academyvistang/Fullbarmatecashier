using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BarAndRestaurantMate.Models
{
    public class TabOrderModel
    {
        public List<HotelMateWeb.Dal.DataCore.GuestOrderItem> Items { get; set; }

        public int Quantity { get; set; }

        public decimal FullPrice { get; set; }
    }
}