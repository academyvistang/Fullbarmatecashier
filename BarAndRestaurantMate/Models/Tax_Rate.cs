﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BarAndRestaurantMate.Models
{
    public class Tax_Rate
    {
        public int id { get; set; }
        public decimal rate { get; set; }
        public int type { get; set; } 

    }
}