using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BarAndRestaurantMate.Models
{

    public class InvoicerItemModel
    {
        public string ItemName { get;  set; }
        public decimal Price { get;  set; }
        public int Quantity { get;  set; }
        public DateTime TimeOfSale { get;  set; }
        public decimal TotalPrice { get;  set; }
    }
    public class InvoicerModel
    {
        public decimal Total { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Discount { get; set; }

        public string DiscountDetails { get; set; }
        public string TaxDetails { get; set; }

        public List<InvoicerItemModel> InvoicerItemModelList { get; set; }
        public string ReceiptNumber { get;  set; }
    }
}