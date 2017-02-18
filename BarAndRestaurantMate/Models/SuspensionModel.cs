using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BarAndRestaurantMate.Models
{
    public class SuspensionModel
    {
        public string itemsList { get; set; }

        public DateTime SuspentionTime { get; set; }

        public int Id { get; set; }
    }
}