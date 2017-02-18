using HotelMateWeb.Dal.DataCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BarAndRestaurantMate.Models
{
    public class PurchaseOrderItemModel
    {
        public StockItem ActualStockItem { get; set; }

        public int QuantityRecieved { get; set; }

        public int QuantityReturned { get; set; }

        public int QuantityRemaining { get; set; }
    }
    public class PurchaseOrderModel
    {
        [Display(Name="Description")]
        [Required(ErrorMessage = "Please enter a description")]
        public string Description { get; set; }

        [Display(Name = "Supplier Reference")]
        public string SupplierReference { get; set; }

        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }

        public IEnumerable<SelectListItem> selectList { get; set; }
        public int Id { get; set; }

        public bool? Saved { get; set; }

        public bool IsRawItem { get; set; }

        public IEnumerable<SelectListItem> selectListStore { get; set; }

        public List<HotelMateWeb.Dal.DataCore.PurchaseOrderItem> Items { get; set; }

        public IList<HotelMateWeb.Dal.DataCore.StockItem> StockItems { get; set; }

        [Display(Name = "Net Value")]
        public decimal Value { get; set; }

        public List<HotelMateWeb.Dal.DataCore.StockItem> DamagedGoods { get; set; }

        public bool Recieved { get; set; }

        [Display(Name = "Attach Invoice")]
        public string Invoice { get; set; }

        public string InvoicePath { get; set; }


        public decimal NetValue { get; set; }
    }

    public class RawItemModel
    {
        public int Id { get; set; }

        //[Required(ErrorMessage = "Please enter a description")]
        public string Description { get; set; }

        public bool IsActive { get; set; }

        [Display(Name = "Price Per KG")]
        [Required(ErrorMessage = "Please enter a selling price")]
        [Range(1, 99999999.99, ErrorMessage = "Value must be between 0 - 9,9999999.99")]
        public decimal UnitPrice { get; set; }

        public string PicturePath { get; set; }

        public string Status { get; set; }
        public int Quantity { get; set; }
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Please enter a buying price")]
        [Range(0, 99999999.99, ErrorMessage = "Value must be between 0 - 9,9999999.99")]
        public decimal OrigPrice { get; set; }

        [Required(ErrorMessage = "Please enter a Notification Number")]
        [Range(0, 999999.99, ErrorMessage = "Value must be between 0 - 9,9999999.99")]
        public int NotNumber { get; set; }

        public string NotStatus { get; set; }

        [Required(ErrorMessage = "Please enter a name")]
        public string StockItemName { get; set; }

        public int TotalQuantity { get; set; }

        public string Barcode { get; set; }

        public string Picture { get; set; }

        public int HotelId { get; set; }

        public IEnumerable<SelectListItem> selectList { get; set; }

        public bool? Saved { get; set; }


        [Display(Name = "Cooked Food")]
        public bool CookedFood { get; set; }

        public decimal? Price { get; set; }

        [Display(Name = "Kitchen Only")]
        public bool KitchenOnly { get; set; }

        [Display(Name = "Raw Item / Units ")]
        public bool RawItem { get; set; }

        public decimal ClubPrice { get; set; }

        public decimal RawItemQuantity { get; set; }
    }
    public class ItemModel
    {
        public int Id { get; set; }       

        //[Required(ErrorMessage = "Please enter a description")]
        public string Description { get; set; }

        public bool IsActive { get; set; }

        [Required(ErrorMessage = "Please enter a selling price")]
        [Range(1, 99999999.99, ErrorMessage = "Value must be between 0 - 9,9999999.99")]
        public decimal UnitPrice { get; set; }
        
        public string PicturePath { get; set; }

        public string Status { get; set; }

       // [Required(ErrorMessage = "Please enter a Quantity")]
       // [Range(1, 999999.99, ErrorMessage = "Value must be between 0 - 9,9999999.99")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Please enter a Category")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Please enter a buying price")]
        [Range(0, 99999999.99, ErrorMessage = "Value must be between 0 - 9,9999999.99")]
        public decimal OrigPrice { get; set; }

        [Required(ErrorMessage = "Please enter a Notification Number")]
        [Range(0, 999999.99, ErrorMessage = "Value must be between 0 - 9,9999999.99")]
        public int NotNumber { get; set; }

        public string NotStatus { get; set; }

        [Required(ErrorMessage = "Please enter a name")]
        public string StockItemName { get; set; }
       
        public int TotalQuantity { get; set; }

        public string Barcode { get; set; }

        public string Picture { get; set; }

        public int HotelId { get; set; }

        public IEnumerable<SelectListItem> selectList { get; set; }

        public bool? Saved { get; set; }


        [Display(Name="Cooked Food")]
        public bool CookedFood { get; set; }

        public decimal? Price { get; set; }

        [Display(Name = "Kitchen Only")]
        public bool KitchenOnly { get; set; }

        [Display(Name = "Raw Item / Units ")]
        public bool RawItem { get; set; }

        public decimal ClubPrice { get; set; }

        public decimal RawItemQuantity { get; set; }
    }

    public class ItemIndexModel : BaseViewModel
    {
        public IEnumerable<ItemModel> ItemList { get; set; }

        public IEnumerable<RawItemModel> RawItemList { get; set; }



        public HotelMateWeb.Dal.DataCore.PurchaseOrder POItem { get; set; }
        

        public bool CanCreatePO { get; set; }

        public List<HotelMateWeb.Dal.DataCore.Store> StoreList { get; set; }

        public HotelMateWeb.Dal.DataCore.Person ThisUser { get; set; }

        public int ThisUserId { get; set; }

        public int StoreId { get; set; }

        public List<HotelMateWeb.Dal.DataCore.Store> allStores { get; set; }

        public List<HotelMateWeb.Dal.DataCore.DistributionPoint> DistributionPoints { get; set; }

        public HotelMateWeb.Dal.DataCore.DistributionPoint DistributionPoint { get; set; }

        public HotelMateWeb.Dal.DataCore.Store Store { get; set; }

        public bool CanRecordDamages { get; set; }

        public int PoId { get; set; }

        public List<POSItem> PosItemList { get; set; }

        public List<StockItem> StockList { get; set; }

        public List<Batch> BatchPoItems { get; set; }
        public List<StorePoint> StorePointList { get; set; }
        public StorePoint StorePointItem { get; set; }
        public int StorePointId { get; set; }
        public int BatchId { get; set; }
        public string ReportName { get; internal set; }
        public string FileToDownloadPath { get; internal set; }
    }
    public class DamagedItemViewModel : BaseViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Item")]
        [Range(1,double.MaxValue,ErrorMessage = "Please enter a value greater than zero")]
        public int? ItemId { get; set; }
        public int NumberOfItems { get; set; }

        [Display(Name = "Terminal")]
        public int? DistributionPointId { get; set; }
        public IEnumerable<SelectListItem> Terminals { get; set; }
        public IEnumerable<SelectListItem> Items { get; set; }
        public IEnumerable<SelectListItem> Stores { get; set; }

        [Display(Name = "Store")]
        public int? StorePointId { get; set; }
        public bool? Saved { get; set; }
        [Required(ErrorMessage = "Please enter a description of the damage")]

        public string Description { get; set; }


    }

    public class ItemRawIndexModel : BaseViewModel
    {
        public IEnumerable<ItemModel> ItemList { get; set; }

        public IEnumerable<RawItemModel> RawItemList { get; set; }

        public HotelMateWeb.Dal.DataCore.PurchaseOrder POItem { get; set; }

        public List<HotelMateWeb.Dal.DataCore.PurchaseOrder> POItemList { get; set; }

        public bool CanCreatePO { get; set; }

        public List<HotelMateWeb.Dal.DataCore.Store> StoreList { get; set; }

        public HotelMateWeb.Dal.DataCore.Person ThisUser { get; set; }

        public int ThisUserId { get; set; }

        public int StoreId { get; set; }

        public List<HotelMateWeb.Dal.DataCore.Store> allStores { get; set; }

        public List<HotelMateWeb.Dal.DataCore.StorePoint> StorePoints { get; set; }

        public HotelMateWeb.Dal.DataCore.StorePoint StorePoint { get; set; }

        public HotelMateWeb.Dal.DataCore.Store Store { get; set; }

        public bool CanRecordDamages { get; set; }

        public int PoId { get; set; }

        public List<POSItem> PosItemList { get; set; }

        public List<StockItem> StockList { get; set; }

        public List<Batch> BatchPoItems { get; set; }
        public List<MainInventoryViewModel> POStockList { get; set; }
        public ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; }
        public List<StorePointItem> StoreTransferredItems { get; set; }
        public List<DamagedBatchItem> DamagesList { get; set; }
    }
}