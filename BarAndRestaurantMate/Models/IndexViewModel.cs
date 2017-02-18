using POSService.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BarAndRestaurantMate.Models
{

    public class LatestRoomViewModel
    {
        public string RoomTypeName { get; set; }

        public List<string> PicturesList { get; set; }

        public IEnumerable<decimal> Price { get; set; }

        public IEnumerable<string> Description { get; set; }
    }

    public class GroupByGroupByModel
    {
        public string ItemName { get; set; }

        public string ValueIds { get; set; }

        public int Qty { get; set; }

        public DateTime DateSold { get; set; }

        public int Id { get; set; }
    }
    public class LatestGroupByModel
    {

        public DateTime Datesold { get; set; }

        public List<HotelMateWeb.Dal.DataCore.TableItem> Items { get; set; }

        public string DatesoldStr { get; set; }

        public string ValueIds { get; set; }

    }

    public class IndexViewModel
    {
        public IEnumerable<StockItem> productsList { get; set; }

        public IEnumerable<Category> categoriesList { get; set; }

        public IEnumerable<Guest> CurrentGuests { get; set; }

        public IEnumerable<HotelMateWeb.Dal.DataCore.Person> CurrentCashiers { get; set; }

        public bool CashierCanOpenTable { get; set; }

        public int GuestId { get; set; }

        public int RealTaxValue { get; set; }

        public decimal Tax { get; set; }

        public int PersonId { get; set; }

        public string Terminal { get; set; }

        public string Table { get; set; } 

        public int ProductsAlerts { get; set; }

        public int TableId { get; set; }

        public List<HotelMateWeb.Dal.DataCore.TableItem> ExistingList { get; set; }

        public bool Retrieve { get; set; }

        public List<int> Tables { get; set; }

        public List<BusinessAccount> CurrentBusinessAccounts { get; set; }

        public int IsManager { get; set; }

        public string DistributionPointName { get; set; }

        public bool CanSuspend { get; set; }

        public bool ClubTime { get; set; }

        public List<HotelMateWeb.Dal.DataCore.GuestOrder> OccupiedTables { get; set; }

        public bool ShowAllButtons { get; set; }

        public bool ChargeSeperately { get; set; }

        public bool ChargeSeperatelyOn { get; set; }

        public List<string> categoriesListString { get; set; }

        public int? CategoryId { get; set; }

        public List<HotelMateWeb.Dal.DataCore.BarTable> MyTables { get; set; }

        public List<StockItem> ColorCodedProductsList { get; set; }

        public decimal Total { get; set; }

        public int TotalItems { get; set; }
        public bool CanTakePayment { get; set; }
        public bool CanCancelSale { get;set; }
    }
}