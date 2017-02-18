using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BarAndRestaurantMate.Models
{
    public class FlightModel
    {
    }

    public class StarBuyModel
    {
        public HotelMateWeb.Dal.DataCore.StockItem RealStockItem { get; set; }

        public HotelMateWeb.Dal.DataCore.BarTable RealTable { get; set; }

        public string RealTableString { get; set; }

        public string RealStockItemString { get; set; }

        public int index { get; set; }

        public string PicturePath { get; set; }
    }
}