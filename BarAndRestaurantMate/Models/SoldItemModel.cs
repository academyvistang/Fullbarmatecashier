using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BarAndRestaurantMate.Models
{

    public class GuestRoomModel
    {

        public List<HotelMateWeb.Dal.DataCore.GuestRoom> ItemList { get; set; }

        public string CheckingDate { get; set; }

        public decimal Balance { get; set; }
    }

    public class TerminalModel
    {

        public HotelMateWeb.Dal.DataCore.RoomPaymentType Terminal { get; set; }

        public List<HotelMateWeb.Dal.DataCore.GuestRoomAccount> ItemList { get; set; }
    }
    public class SoldItemModelAccomodation
    {
        public decimal TotalAmount { get; set; }

        public string DateSold { get; set; }

        public List<PersonAccomodationModel> ItemLst { get; set; }
    }

    public class GroupItemModel
    {
        public DateTime ActualDateSold { get; set; }

        public string Item { get; set; }

        public List<SoldItemModel> FinalList { get; set; }

        public string ItemDescription { get; set; }

        public List<GroupItemModel> DateList { get; set; }

        public int Quantity { get; set; }
    }
    public class SoldItemModel
    {

        public int Id { get; set; }
        
        public string Description { get; set; }
       
        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        public int Remainder { get; set; }
       
        public string CategoryName { get; set; }

        public decimal TotalPrice { get; set; }

        public DateTime DateSold { get; set; }

        public string PaymentTypeName { get; set; }
        
        public string PersonName { get; set; }
        
        public SoldItemModel Itemlst { get; set; }

        public List<SoldItemModel> ItemNewlst { get; set; }

        public List<HotelMateWeb.Dal.DataCore.GuestRoomAccount> RoomNewlst { get; set; }

        public List<HotelMateWeb.Dal.DataCore.Expense> Expenselst { get; set; }

        public List<HotelMateWeb.Dal.DataCore.GuestRoomAccount> RoomAccountList { get; set; }

        public string PaymentMethodNote { get; set; }

        public string PaymentMethodName { get; set; }

        public List<CombinedSalesModel> CombinedList { get; set; }

        public DateTime TimeOfSale { get; set; }

        public int? GuestId { get; set; }

        public bool IsActive { get; set; }

        public List<GroupItemModel> CompleteGrouping { get; set; }

        public int PaymentMethodId { get; set; }

        public int DistributionPointId { get; set; }

        public bool TillOpen { get; set; }
        public string RecieptNumber { get; internal set; }
        public decimal TotalTax { get; internal set; }
        public decimal TotalDiscount { get; internal set; }
        public decimal TotalServiceCharge { get; internal set; }
        public decimal TotalResident { get; internal set; }
        public decimal Total { get; internal set; }
        public decimal SubTotal { get; internal set; }
    }

    public class SoldItemIndexModel
    {
        public IEnumerable<SoldItemModel> ItemList { get; set; }
    }


    public class CombinedSalesModel
    {


        public DateTime DateSold { get; set; }

        public string RoomNumber { get; set; }

        public HotelMateWeb.Dal.DataCore.GuestRoom GuestRoom { get; set; }

       

        public string PaymentMethodNote { get; set; }

        public decimal Amount { get; set; }

        public string PaymentMethod { get; set; }

        public string StaffName { get; set; }

        public string Terminal { get; set; }
    }
}