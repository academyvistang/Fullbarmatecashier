using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BarAndRestaurantMate.Models
{
    public class MainInventoryViewModel
    {
        public string ItemName { get; internal set; }
        public int Remaining { get; internal set; }
        public int TotalRecieved { get; internal set; }
    }
}