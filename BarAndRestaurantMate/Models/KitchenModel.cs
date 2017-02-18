using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BarAndRestaurantMate.Models
{
    public class KitchenModel
    {
        public HotelMateWeb.Dal.DataCore.BarTable BarTab { get; set; }

        public List<HotelMateWeb.Dal.DataCore.TableItem> List { get; set; }

        public string ValueIds { get; set; }

        public IEnumerable<KitchenModelGroupByDateSold> DateSoldList { get; set; }

        public IEnumerable<KitchenModel> NewList { get; set; }

        public string Note { get; set; }
    }

    public class KitchenModelGroupByDateSold
    {
        public HotelMateWeb.Dal.DataCore.BarTable BarTab { get; set; }

        public List<HotelMateWeb.Dal.DataCore.TableItem> List { get; set; }

        public string ValueIds { get; set; }

        public DateTime ExactDateSold { get; set; }
    }
}