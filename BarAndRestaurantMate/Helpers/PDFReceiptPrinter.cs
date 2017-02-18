using BarAndRestaurantMate.Models;
using Invoicer.Models;
using Invoicer.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace BarAndRestaurantMate.Helpers
{
    public class PDFReceiptPrinter
    {
        public static string PrintInvoiceBar(string path, InvoicerModel gravm, string imagePath)
        {

            var guestItems = new List<ItemRow>();

            foreach (var rm in gravm.InvoicerItemModelList.OrderBy(x => x.ItemName))
            {
                guestItems.Add(new ItemRow { TransactionDate = rm.TimeOfSale, Quantity = rm.Quantity, Description = rm.ItemName, UnitPrice = rm.Price, TotalPrice = rm.TotalPrice });
            }

            string[] splitShopDetails = null;

            try
            {
                var shopDetails = ConfigurationManager.AppSettings["SHOPDETAILS"].ToString();

                splitShopDetails = shopDetails.Split('@').ToArray();

                if (splitShopDetails.Count() < 7)
                {
                    splitShopDetails = null;
                }

            }
            catch (Exception)
            {
            }

            if (splitShopDetails == null)
            {
                splitShopDetails = new string[] { "AcademyVista Ltd. @ B03 Eleganza, V.G.C, @ Lagos State @ Nigeria @ 08105387515 @ www.academyvista.com @ 6543210" };
            }

            string currency = "NGN", reference = gravm.ReceiptNumber + "_" + DateTime.Now.ToShortTimeString().Replace(":", ""), companyReg = splitShopDetails[6];
            string[] companyDetails = splitShopDetails, clientDetails = new string[] { "Sir/Madam" };
            List<ItemRow> itemsRows = new List<ItemRow>();
            List<TotalRow> totalRows = new List<TotalRow>();
            List<DetailRow> detailRows = new List<DetailRow>();
            string footerWebsite = splitShopDetails[5];
            

            string filename = new InvoicerApi(SizeOption.A4, OrientationOption.Landscape, currency)
                 .TextColor("#CC0000")
                 .BackColor("#FFD6CC")
                 .Image(imagePath, 125, 27)
                 .Reference(reference)
                 .Company(Address.Make("FROM", companyDetails, companyReg, ""))
                 .Client(Address.Make("BILLING TO", clientDetails))
                 .Items(guestItems)
                   .Totals(new List<TotalRow> {
                    TotalRow.Make("Sub Total", gravm.SubTotal),
                    TotalRow.Make(gravm.TaxDetails, gravm.Tax),
                    TotalRow.Make(gravm.DiscountDetails, gravm.Discount),
                    TotalRow.Make("Total", gravm.Total, true),
                 })
                 .Details(new List<DetailRow>
                  {
                    DetailRow.Make("PAYMENT INFORMATION", "A copy of this receipt will also be emailed to you.", "", "If you have any questions concerning this receipt, contact our front office or a duty manager.", "", "Thank you for your business."),
                   
                  })
                 .Footer(footerWebsite)
                 .Save(path, 0);

            return reference;
        }
    }
}