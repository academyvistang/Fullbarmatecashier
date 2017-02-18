using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BarAndRestaurantMate.Models
{
    public class ConciseBalanceSheetModel
    {
        public DateTime ActualDate { get; set; }

        public decimal TotalReceiveable { get; set; }

        public decimal TotalPayaeble { get; set; }
    }
}