using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HotelMateWeb.Dal.DataCore;
using POSService.Entities;

namespace BarAndRestaurantMate.Models
{
    public class BalanceSheetModel : BaseViewModel
    {
        public DateTime TransactionDate { get; set; }

        public decimal AmountPaidIn { get; set; }

        public decimal AmountPaidOut { get; set; }

        public int? Cashier { get; set; }

        public HotelMateWeb.Dal.DataCore.PaymentMethod PaymentMentMethod { get; set; }

        public HotelMateWeb.Dal.DataCore.RoomPaymentType PaymentType { get; set; }

        public int? PaymentTypeId { get; set; }
    }

    ////MainInventoryViewModel
    //public class MainInventoryViewModel : BaseViewModel
    //{
    //    public string ItemName { get;  set; }
    //    public int Remaining { get;  set; }
    //    public int TotalRecieved { get;  set; }
    //}

    public class InventoryViewModel : BaseViewModel
    {
        //public string StockItemName { get; set; }

        public int Acquired { get; set; }

        public int Returns { get; set; }

        public int NumberSold { get; set; }

        public int Remaining { get; set; }

        public int Damaged { get; set; }

        public bool ThereIsProblem { get; set; }

        public int NotifyNumber { get; set; }
        public int Distributed { get; internal set; }
        public POSService.Entities.StockItem StockItemName { get; internal set; }
        public string StockItemNameName { get; internal set; }
        public bool IsRawItem { get; internal set; }
    }

    public class FoodMatrixGroupByViewModel : BaseViewModel
    {
        public POSService.Entities.StockItem ActualRawMaterial { get; set; }

        public List<FinalAnalysisViewModel> ItemLst { get; set; }

        public decimal TotalPoRecieved { get; set; }

        public decimal UnitPerItem { get; set; }
    }

    public class FoodMatrixViewModel : BaseViewModel
    {

        public List<POSService.Entities.StockItem> CookedFoodList { get; set; }

        public POSService.Entities.StockItem RawMaterial { get; set; }
    }

    public class GrandFinalAnalysisViewModel : BaseViewModel
    {
        public decimal TotalPoRecieved { get; set; }

        public long Id { get; set; }

        public decimal UnitPerItem { get; set; }
    }

    public class FinalAnalysisViewModel : BaseViewModel
    {
        public POSService.Entities.StockItem RawMaterial { get; set; }

        public HotelMateWeb.Dal.DataCore.StockItem CookedFood { get; set; }

        public int TotalNumberSold { get; set; }

        public int NumberOfUnitsPerMeal { get; set; }

        public int TotalUnits { get; set; }
    }

    public class SoldItemViewModel : BaseViewModel
    {
        public HotelMateWeb.Dal.DataCore.StockItem Item { get; set; }

        public int Quantity { get; set; }
    }

    public class RawMaterialViewModel : BaseViewModel
    {
        public POSService.Entities.StockItem RawMaterial { get; set; }

        public int NumberOfSoldItems { get; set; }

        public double PerRawMaterial { get; set; }

        public string PossibilitiesList { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? StartDate { get; set; }

        public decimal UnitPrice { get; set; }

        public bool ThereIsProblem { get; set; }

        public double PerRawMaterialMoney { get; set; }

        public int NumberOfRawMaterialsUsed { get; set; }
    }

    public class ReportViewModel : BaseViewModel
    {
        public IQueryable<HotelMateWeb.Dal.DataCore.Guest> Guests { get; set; }

        public List<HotelMateWeb.Dal.DataCore.Room> Rooms { get; set; }

        public List<HotelMateWeb.Dal.DataCore.Guest> HotelGuests { get; set; }

        public bool RoomHistory { get; set; }

        public List<HotelMateWeb.Dal.DataCore.GuestRoom> RoomHistorys { get; set; }

        public List<IEnumerable<HotelMateWeb.Dal.DataCore.GuestRoom>> RoomOccupancy { get; set; }

        public DateTime? StartDate { get; set; }

        public List<ICollection<HotelMateWeb.Dal.DataCore.GuestRoom>> GuestRooms { get; set; }

        public List<HotelMateWeb.Dal.DataCore.GuestRoomAccount> Accounts { get; set; }

        public List<SoldItemModel> SalesModel { get; set; }

        public List<SoldItemModel> ModelGroupBy { get; set; }

        public List<SoldItemModelAccomodation> ModelGroupByAccomodation { get; set; }

        public List<GuestRoomModel> GroupByList { get; set; }

        public List<EmployeeGroupByModel> EmployeeGroupByList { get; set; }

        public List<BalanceSheetModel> BalanceSheet { get; set; }

        public decimal FullBalance { get; set; }

        public List<ConciseBalanceSheetModel> ConciseBalanceSheetSheet { get; set; }

        public List<HotelMateWeb.Dal.DataCore.PurchaseOrder> InventoryList { get; set; }

        public List<HotelMateWeb.Dal.DataCore.PurchaseOrder> PurchaseOrders { get; set; }

        public List<HotelMateWeb.Dal.DataCore.BusinessCorporateAccount> Bal { get; set; }

        public List<CompanyPaymentModel> CompanyAccountsList { get; set; }

        public System.Data.DataSet AllSoldItems { get; set; }

        public List<HotelMateWeb.Dal.DataCore.SoldItemsAll> AllSoldItemsNew { get; set; }

        public List<InventoryViewModel> CompleteInventoryList { get; set; }

        public List<POSService.Entities.StockItem> CookedFoodItems { get; set; }

        public List<POSService.Entities.StockItem> RawMaterialsItems { get; set; }

        public List<FoodMatrixViewModel> FoodMatrixModels { get; set; }

        public List<RawMaterialViewModel> PossibilityLst { get; set; }

        public List<HotelMateWeb.Dal.DataCore.GuestMessage> AllCashierMessages { get; set; }

        public List<HotelMateWeb.Dal.DataCore.SalesDiscount> AllDiscounts { get; set; }

        public decimal Cash { get; set; }

        public decimal TotalCashDiscount { get; set; }

        public decimal Tax { get; set; }

        public List<HotelMateWeb.Dal.DataCore.PurchaseOrder> Returns { get; set; }

        public List<SuspensionModel> AllSuspendedSales { get; set; }

        public List<HotelMateWeb.Dal.DataCore.GuestRoomAccount> AllCashPayments { get; set; }

        public POSService.Entities.StockItem ThisStock { get; set; }

        public List<HotelMateWeb.Dal.DataCore.FoodMatrix> Matrix { get; set; }

        public List<FoodMatrixGroupByViewModel> MatrixAnalysisList { get; set; }

        public List<PurchaseOrderItemModel> MainStoreInventoryList { get; set; }

        public IEnumerable<System.Web.Mvc.SelectListItem> selectList { get; set; }

        public string ReportName { get; set; }

        public int? Id { get; set; }

        public decimal SCValue { get; set; }

        public decimal TotalCashComplimentary { get; set; }
        public List<Payment> Payments { get; internal set; }
        public string FileToDownloadPath { get; internal set; }
        public int StorePointId { get; internal set; }
        public List<Payment> AllNewDiscounts { get; internal set; }
        public int? RawId { get; internal set; }
    }
}