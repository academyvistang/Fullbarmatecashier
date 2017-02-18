using POSService.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BarAndRestaurantMate.Models
{
    public class EmailTemplateModel
    {
        public Guest Guest { get; set; }
        public DateTime ChekinDate { get; set; }
        public DateTime ChekoutDate { get; set; }
        public Decimal InitialDeposit { get; set; }

    }
}