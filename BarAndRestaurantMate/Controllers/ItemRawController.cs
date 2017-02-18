

using HotelMateWeb.Dal.DataCore;
using HotelMateWeb.Services.Core;
using HotelMateWeb.Services.ServiceApi;
using BarAndRestaurantMate.Helpers.Enums;
using BarAndRestaurantMate.Models;
using POSService;
using POSService.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using BarAndRestaurantMate.Importer;

namespace BarAndRestaurantMate.Controllers
{
    [HandleError(View = "CustomErrorView")]
    public class ItemRawController : Controller
    {

        private IPurchaseOrderService _purchaseOrderService;
        private IPurchaseOrderItemService _purchaseOrderItemService;
        private IPersonService _personService;
        private IStoreService _storeService;
        private IStoreItemService _storeItemService;
        private IStockItemService _stockItemService;
        private IInvoiceService _invoiceService;
        private IStorePointService _storePointService;
        private IStorePointItemService _storePointItemService;
        private IBatchService _batchService;
        private IPOSItemService _pOSItemService;
        private IDamagedBatchItemService _damagedBatchItemService;
        private IUsedStockItemService _usedStockItemService;
        private IDistributionPointService _distributionPointService;
        //private  ICategory _usedStockItemService;



        protected override void Dispose(bool disposing)
        {
            
            if (disposing && _distributionPointService != null)
            {
                _distributionPointService.Dispose();
                _distributionPointService = null;
            }

            if (disposing && _usedStockItemService != null)
            {
                _usedStockItemService.Dispose();
                _usedStockItemService = null;
            }

            if (disposing && _purchaseOrderService != null)
            {
                _purchaseOrderService.Dispose();
                _purchaseOrderService = null;
            }

            if (disposing && _purchaseOrderItemService != null)
            {
                _purchaseOrderItemService.Dispose();
                _purchaseOrderItemService = null;
            }

            if (disposing && _personService != null)
            {
                _personService.Dispose();
                _personService = null;
            }

            if (disposing && _storeService != null)
            {
                _storeService.Dispose();
                _storeService = null;
            }


            if (disposing && _storeItemService != null)
            {
                _storeItemService.Dispose();
                _storeItemService = null;
            }

            if (disposing && _stockItemService != null)
            {
                _stockItemService.Dispose();
                _stockItemService = null;
            }

            if (disposing && _invoiceService != null)
            {
                _invoiceService.Dispose();
                _invoiceService = null;
            }

            if (disposing && _storePointService != null)
            {
                _storePointService.Dispose();
                _storePointService = null;
            }

            if (disposing && _storePointItemService != null)
            {
                _storePointItemService.Dispose();
                _storePointItemService = null;
            }

            if (disposing && _batchService != null)
            {
                _batchService.Dispose();
                _batchService = null;
            }

            if (disposing && _pOSItemService != null)
            {
                _pOSItemService.Dispose();
                _pOSItemService = null;
            }

            if (disposing && _damagedBatchItemService != null)
            {
                _damagedBatchItemService.Dispose();
                _damagedBatchItemService = null;
            }


            base.Dispose(disposing);
        }



        public ItemRawController()
        {
            _purchaseOrderItemService = new PurchaseOrderItemService();
            _purchaseOrderService = new PurchaseOrderService();
            _personService = new PersonService();
            _storeService = new StoreService();
            _stockItemService = new StockActualItemService();
            _invoiceService = new InvoiceService();
            _storeItemService = new StoreItemService();
            _storePointService = new StorePointService();
            _storePointItemService = new StorePointItemService();
            _batchService = new BatchService();
            _pOSItemService = new POSItemService();
            _damagedBatchItemService = new DamagedBatchItemService();
            _usedStockItemService = new UsedStockItemService();
            _distributionPointService = new DistributionPointService();
        }

        public IEnumerable<SoldItemModel> GetAllSoldItems()
        {
            List<SoldItemModel> lst = new List<SoldItemModel>();

            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("GetStockSoldItems", myConnection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    myConnection.Open();

                    //SqlParameter custId = cmd.Parameters.AddWithValue("@CustomerId", 10);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            int id = dr.GetInt32(0);    // Weight int
                            decimal totalPrice = dr.GetDecimal(1);
                            decimal unitPrice = dr.GetDecimal(2);
                            int qty = dr.GetInt32(3);    // Weight int
                            DateTime datesold = dr.GetDateTime(4);  // Name string
                            string itemName = dr.GetString(5);  // Name string
                            int remainder = dr.GetInt32(6); // Breed string                                                
                            string categoryName = dr.GetString(7);    // Weight int


                            yield return new SoldItemModel
                            {
                                Id = id,
                                CategoryName = categoryName,
                                DateSold = datesold,
                                Description = itemName,
                                Quantity = qty,
                                Remainder = remainder,
                                TotalPrice = totalPrice,
                                UnitPrice = unitPrice


                            };

                        }
                    }
                }
            }
        }

        private IEnumerable<RawItemModel> GetAllItemsRaw()
        {
            List<RawItemModel> lst = new List<RawItemModel>();

            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("GetStockItemsRaw", myConnection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    myConnection.Open();

                    //SqlParameter custId = cmd.Parameters.AddWithValue("@CustomerId", 10);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            int id = dr.GetInt32(0);    // Weight int
                            decimal unitPrice = dr.GetDecimal(1);
                            string description = dr.GetString(2);  // Name string
                            string picturePath = dr.GetString(3);  // Name string
                            bool isActive = dr.GetBoolean(4); // Breed string 
                            string status = dr.GetString(5);  // Name string
                            int qty = dr.GetInt32(6);    // Weight int
                            int categoryId = dr.GetInt32(7);    // Weight int
                            decimal origPrice = dr.GetDecimal(8);
                            int notNumber = dr.GetInt32(9);
                            string notStatus = dr.GetString(10);
                            string name = dr.GetString(11); // Breed string 
                            //object obj = dr.GetByte(12); // Breed string 
                            int totalQuantity = dr.GetInt32(13);    // Weight int
                            string barcode = dr.GetString(14); // Breed string 
                            string orderStatus = dr.GetString(15); // Breed string 
                            int hotelId = dr.GetInt32(20);    // Weight int  
                            bool cookedFood = dr.GetBoolean(25);
                            bool kitchenOnly = dr.GetBoolean(26);
                            bool rawItem = dr.GetBoolean(27);
                            decimal clubPrice = dr.GetDecimal(29);

                            //lst.Add(new ItemModel { Id = id, Description = description, IsActive = isActive, Name = name });
                            yield return new RawItemModel
                            {
                                Id = id,
                                UnitPrice = unitPrice,
                                Description = description,
                                PicturePath = picturePath,
                                NotStatus = notStatus,
                                IsActive = isActive,
                                StockItemName = name,
                                Quantity = qty,
                                NotNumber = notNumber,
                                CategoryId = categoryId,
                                Barcode = barcode,
                                OrigPrice = origPrice,
                                CookedFood = cookedFood,
                                KitchenOnly = kitchenOnly,
                                RawItem = rawItem,
                                ClubPrice = clubPrice
                            };

                        }
                    }
                }
            }
        }


        private IEnumerable<ItemModel> GetAllItemsNonRaw()
        {
            List<ItemModel> lst = new List<ItemModel>();

            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("GetStockItems", myConnection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    myConnection.Open();

                    //SqlParameter custId = cmd.Parameters.AddWithValue("@CustomerId", 10);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            int id = dr.GetInt32(0);    // Weight int
                            decimal unitPrice = dr.GetDecimal(1);
                            string description = dr.GetString(2);  // Name string
                            string picturePath = dr.GetString(3);  // Name string
                            bool isActive = dr.GetBoolean(4); // Breed string 
                            string status = dr.GetString(5);  // Name string
                            int qty = dr.GetInt32(6);    // Weight int
                            int categoryId = dr.GetInt32(7);    // Weight int
                            decimal origPrice = dr.GetDecimal(8);
                            int notNumber = dr.GetInt32(9);
                            string notStatus = dr.GetString(10);
                            string name = dr.GetString(11); // Breed string 
                            //object obj = dr.GetByte(12); // Breed string 
                            int totalQuantity = dr.GetInt32(13);    // Weight int
                            string barcode = dr.GetString(14); // Breed string 
                            string orderStatus = dr.GetString(15); // Breed string 
                            int hotelId = dr.GetInt32(20);    // Weight int  
                            bool cookedFood = dr.GetBoolean(25);
                            bool kitchenOnly = dr.GetBoolean(26);
                            bool rawItem = dr.GetBoolean(27);
                            decimal clubPrice = dr.GetDecimal(29);

                            //lst.Add(new ItemModel { Id = id, Description = description, IsActive = isActive, Name = name });
                            yield return new ItemModel
                            {
                                Id = id,
                                UnitPrice = unitPrice,
                                Description = description,
                                PicturePath = picturePath,
                                NotStatus = notStatus,
                                IsActive = isActive,
                                StockItemName = name,
                                Quantity = qty,
                                NotNumber = notNumber,
                                CategoryId = categoryId,
                                Barcode = barcode,
                                OrigPrice = origPrice,
                                CookedFood = cookedFood,
                                KitchenOnly = kitchenOnly,
                                RawItem = rawItem,
                                ClubPrice = clubPrice
                            };

                        }
                    }
                }
            }

            //return lst;

        }

        private IEnumerable<ItemModel> GetAllItems()
        {
            List<ItemModel> lst = new List<ItemModel>();

            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("GetStockItemsRaw", myConnection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    myConnection.Open();

                    //SqlParameter custId = cmd.Parameters.AddWithValue("@CustomerId", 10);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            int id = dr.GetInt32(0);    // Weight int
                            decimal unitPrice = dr.GetDecimal(1);
                            string description = dr.GetString(2);  // Name string
                            string picturePath = dr.GetString(3);  // Name string
                            bool isActive = dr.GetBoolean(4); // Breed string 
                            string status = dr.GetString(5);  // Name string
                            int qty = dr.GetInt32(6);    // Weight int
                            int categoryId = dr.GetInt32(7);    // Weight int
                            decimal origPrice = dr.GetDecimal(8);
                            int notNumber = dr.GetInt32(9);
                            string notStatus = dr.GetString(10);
                            string name = dr.GetString(11); // Breed string 
                            //object obj = dr.GetByte(12); // Breed string 
                            int totalQuantity = dr.GetInt32(13);    // Weight int
                            string barcode = dr.GetString(14); // Breed string 
                            string orderStatus = dr.GetString(15); // Breed string 
                            int hotelId = dr.GetInt32(20);    // Weight int  
                            bool cookedFood = dr.GetBoolean(25);
                            bool kitchenOnly = dr.GetBoolean(26);
                            bool rawItem = dr.GetBoolean(27);
                            decimal clubPrice = dr.GetDecimal(29);

                            //lst.Add(new ItemModel { Id = id, Description = description, IsActive = isActive, Name = name });
                            yield return new ItemModel
                            {
                                Id = id,
                                UnitPrice = unitPrice,
                                Description = description,
                                PicturePath = picturePath,
                                NotStatus = notStatus,
                                IsActive = isActive,
                                StockItemName = name,
                                Quantity = qty,
                                NotNumber = notNumber,
                                CategoryId = categoryId,
                                Barcode = barcode,
                                OrigPrice = origPrice,
                                CookedFood = cookedFood,
                                KitchenOnly = kitchenOnly,
                                RawItem = rawItem,
                                ClubPrice = clubPrice
                            };

                        }
                    }
                }
            }

            //return lst;

        }

        private static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["Core"].ConnectionString;
        }

        //[OutputCache(Duration = 3600, VaryByParam = "id")]
        public ActionResult Delete(int? id)
        {
            var cats = GetAllItems();
            ItemModel cm = cats.FirstOrDefault(x => x.Id == id.Value);
            return View(cm);
        }

        public ActionResult DeleteRawMaterials(int? id)
        {
            var cats = GetAllItemsRaw();
            RawItemModel cm = cats.FirstOrDefault(x => x.Id == id.Value);
            return View(cm);
        }



        //[OutputCache(Duration = 3600, VaryByParam = "none")]
        public ActionResult Sales()
        {
            var items = GetAllSoldItems();
            SoldItemIndexModel siim = new SoldItemIndexModel { ItemList = items.OrderByDescending(x => x.DateSold).ToList() };
            return View(siim);

        }

        //
        public ActionResult PORecieved(int? id, bool? saved)
        {
            var po = _purchaseOrderService.GetById(id.Value);

            var pm = new PurchaseOrderModel { Id = po.Id, Description = po.Description, Value = po.NetValue };

            var persons = _personService.GetAllForLogin().Where(x => x.IsActive && x.PersonTypeId != (int)PersonTypeEnum.Guest).ToList();

            var stores = _storeService.GetAll().ToList();

            persons.Insert(0, new Person { DisplayName = "-- Please Select--", PersonID = 0 });


            pm.Items = null;
            var list = po.PurchaseOrderItems.ToList();
            var existingItemList = list.Select(x => x.ItemId).ToList();
            var allExistingStockItems = _stockItemService.GetAll().Where(x => !x.CookedFood).ToList();
            var newPoList = allExistingStockItems.Where(x => !existingItemList.Contains(x.Id)).Select(x => new PurchaseOrderItem { ItemId = x.Id, StockItem = x, Qty = 0 }).ToList();
            list.AddRange(newPoList);
            pm.Items = list;
            pm.Saved = saved;

            return View("CreateRecievable", pm);
        }

        public FileResult DownloadInvoice(int? id)
        {
            var po = _purchaseOrderService.GetById(id.Value);
            var path = Path.Combine(Server.MapPath("~/Invoices"), po.InvoicePath);
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);
            //string fileName = "myfile.ext";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, po.InvoicePath);
            //return null;
        }




        public ActionResult POView(int? id, bool? saved)
        {
            var po = _purchaseOrderService.GetById(id.Value);

            var pm = new PurchaseOrderModel { Id = po.Id, Description = po.Description, Value = po.NetValue };



            var persons = new List<Person>();

            if (po.PurchaseOrderItems.Count > 0)
            {
                persons = _personService.GetAllForLogin().Where(x => x.IsActive && x.PersonTypeId != (int)PersonTypeEnum.Guest).ToList();
                persons.Insert(0, new Person { DisplayName = "-- Please Select--", PersonID = 0 });
            }
            else
            {
                persons = _personService.GetAllForLogin().ToList();
            }



            pm.Items = null;
            var list = po.PurchaseOrderItems.ToList();
            var existingItemList = list.Select(x => x.ItemId).ToList();
            var allExistingStockItems = _stockItemService.GetAll().Where(x => !x.CookedFood).ToList();
            var newPoList = allExistingStockItems.Where(x => !existingItemList.Contains(x.Id)).Select(x => new PurchaseOrderItem { ItemId = x.Id, StockItem = x, Qty = 0 }).ToList();
            list.AddRange(newPoList);
            pm.Items = list;
            pm.Saved = saved;
            return View("POView", pm);
        }


        public ActionResult PODelete(int? id, bool? saved)
        {
            var pos = _purchaseOrderItemService.GetAll().Where(x => x.PurchaseOrderId == id.Value);

            foreach (var e in pos)
            {
                var poi = _purchaseOrderItemService.GetById(e.Id);
                _purchaseOrderItemService.Delete(poi);
            }

            var poDelete = _purchaseOrderService.GetById(id.Value);

            _purchaseOrderService.Delete(poDelete);

            return RedirectToAction("IndexPO");
        }


        public ActionResult ReassignToStoreManager(int? id, bool? saved)
        {
            var po = _purchaseOrderService.GetById(id.Value);

            var pm = new PurchaseOrderModel { Id = po.Id, Description = po.Description, Value = po.NetValue };


            var persons = new List<Person>();

            if (po.PurchaseOrderItems.Count > 0)
            {
                persons = _personService.GetAllForLogin().Where(x => x.IsActive && x.PersonTypeId != (int)PersonTypeEnum.Guest).ToList();
                persons.Insert(0, new Person { DisplayName = "-- Please Select--", PersonID = 0 });
            }
            else
            {
                persons = _personService.GetAllForLogin().ToList();
            }



            pm.Items = null;
            var list = po.PurchaseOrderItems.ToList();
            var existingItemList = list.Select(x => x.ItemId).ToList();
            var allExistingStockItems = _stockItemService.GetAll().Where(x => !x.CookedFood).ToList();
            var newPoList = allExistingStockItems.Where(x => !existingItemList.Contains(x.Id)).Select(x => new PurchaseOrderItem { ItemId = x.Id, StockItem = x, Qty = 0 }).ToList();
            list.AddRange(newPoList);
            pm.Items = list;
            pm.Saved = saved;
            return View("ReassignToStoreManager", pm);
        }

        public ActionResult Damages(int? id)
        {
            var allExistingStockItems = _stockItemService.GetAll().ToList();
            var pm = new PurchaseOrderModel { DamagedGoods = allExistingStockItems };
            pm.Id = id.Value;
            return View(pm);
        }

        [HttpPost]
        public ActionResult Damages(int? id, int[] dummy)
        {
            var allRealStock = _stockItemService.GetAll().ToList();

            var allStockItemIds = allRealStock.Select(x => x.Id).ToList();

            int totalNumberOfItems = 0;

            var thisUser = _personService.GetAllForLogin().Where(x => x.Username.ToUpper().Equals(User.Identity.Name.ToUpper())).FirstOrDefault();

            foreach (var itemId in allStockItemIds)
            {
                var name = "DamagedItem_" + itemId.ToString();

                var realStock = allRealStock.FirstOrDefault(x => x.Id == itemId);

                if (Request.Form[name] != null)
                {
                    var qty = 0;
                    int.TryParse(Request.Form[name].ToString(), out qty);

                    if (qty == 0)
                        continue;

                    totalNumberOfItems++;

                    var lastBtch = _batchService.GetAll().LastOrDefault(x => x.StorePointId == id.Value);

                    if (lastBtch != null)
                    {
                        var existingdbi = _damagedBatchItemService.GetAll().FirstOrDefault(x => x.ItemId == itemId);

                        if (existingdbi != null)
                        {
                            existingdbi.NumberDamaged = qty;
                            _damagedBatchItemService.Update(existingdbi);
                        }
                        else
                        {
                            DamagedBatchItem dbi = new DamagedBatchItem();
                            dbi.NumberDamaged = qty;
                            dbi.ItemId = itemId;
                            _damagedBatchItemService.Create(dbi);
                        }

                    }
                    else
                    {
                        return RedirectToAction("DamagesIndex");
                    }
                }
            }

            return RedirectToAction("Damages", new { id, saved = true });
        }


        public ActionResult POEditReturn(int? id, bool? saved)
        {
            var po = _purchaseOrderService.GetByIdInclude(id.Value, "PurchaseOrderItems,PurchaseOrderItems.StockItem");

            var pm = new PurchaseOrderModel { InvoicePath = po.InvoicePath, Id = po.Id, Description = po.Description, Value = po.NetValue, SupplierReference = po.SupplierInvoice, Recieved = po.GoodsRecieved };

            var persons = new List<Person>();

            if (po.PurchaseOrderItems.Count == 0)
            {
                persons.Insert(0, new Person { DisplayName = "-- Please Select--", PersonID = 0 });
            }
            else
            {
                persons = _personService.GetAllForLogin().ToList();
            }


            pm.Items = null;
            var list = po.PurchaseOrderItems.ToList();
            var existingItemList = list.Select(x => x.ItemId).ToList();
            var allExistingStockItems = _stockItemService.GetAll().Where(x => !x.CookedFood).ToList();
            var newPoList = allExistingStockItems.Where(x => !existingItemList.Contains(x.Id)).Select(x => new PurchaseOrderItem { ItemId = x.Id, StockItem = x, Qty = 0 }).ToList();
            list.AddRange(newPoList);
            pm.Items = list;
            pm.Saved = saved;
            return View(pm);
        }


        public ActionResult POEdit(int? id, bool? saved)
        {
            var po = _purchaseOrderService.GetByIdInclude(id.Value, "PurchaseOrderItems, PurchaseOrderItems.StockItem");

            var pm = new PurchaseOrderModel { IsRawItem = po.IsRawItem, NetValue = po.NetValue, InvoicePath = po.InvoicePath, Id = po.Id, Description = po.Description, Value = po.NetValue, SupplierReference = po.SupplierInvoice, Recieved = po.GoodsRecieved };

            var persons = new List<Person>();

            if (po.PurchaseOrderItems.Count == 0)
            {
                persons.Insert(0, new Person { DisplayName = "-- Please Select--", PersonID = 0 });
            }
            else
            {
                persons = _personService.GetAllForLogin().ToList();
            }


            pm.Items = null;
            var list = po.PurchaseOrderItems.ToList();
            var existingItemList = list.Select(x => x.ItemId).ToList();
            var allExistingStockItems = _stockItemService.GetAll().Where(x => !x.CookedFood).ToList();

            if (po.IsRawItem)
            {
                allExistingStockItems = allExistingStockItems.Where(x => !x.IsActive.Value && x.RawItem).ToList();
            }
            else
            {
                allExistingStockItems = allExistingStockItems.Where(x => x.IsActive.Value && !x.RawItem).ToList();
            }

            var newPoList = allExistingStockItems.Where(x => !existingItemList.Contains(x.Id)).Select(x => new PurchaseOrderItem { ItemId = x.Id, StockItem = x, Qty = 0 }).ToList();

            list.AddRange(newPoList);
            pm.Items = list;
            pm.Saved = saved;
            return View("CreatePO", pm);
        }


        [HttpGet]
        public ActionResult EditRawMaterials(int? id, bool? saved)
        {
            var items = GetAllItemsRaw().Where(x => !x.IsActive && x.RawItem);

            var item = items.FirstOrDefault(x => x.Id == id.Value);

            int catId = item.CategoryId;

            var cats = GetAllCategories();

            var categories = cats.ToList();

            categories.Insert(0, new CategoryModel { Name = "-- Please Select--", Id = 0 });

            IEnumerable<SelectListItem> selectList =
                from c in categories
                select new SelectListItem
                {
                    Selected = (c.Id == catId),
                    Text = c.Name,
                    Value = c.Id.ToString()
                };


            RawItemModel cm = item;

            if (cm.RawItem)
                cm.RawItemQuantity = cm.UnitPrice;

            cm.selectList = selectList;
            cm.Saved = saved;
            cm.Id = id.Value;
            //cm.CookedFood = true;

            return View("CreateRawMaterials", cm);
        }

        [HttpGet]
        public ActionResult Edit(int? id, bool? saved)
        {
            var items = GetAllItems();

            var item = items.FirstOrDefault(x => x.Id == id.Value);

            int catId = item.CategoryId;

            var cats = GetAllCategories();

            var categories = cats.ToList();

            categories.Insert(0, new CategoryModel { Name = "-- Please Select--", Id = 0 });

            IEnumerable<SelectListItem> selectList =
                from c in categories
                select new SelectListItem
                {
                    Selected = (c.Id == catId),
                    Text = c.Name,
                    Value = c.Id.ToString()
                };


            ItemModel cm = item;

            if (cm.RawItem)
                cm.RawItemQuantity = cm.UnitPrice;

            cm.selectList = selectList;
            cm.Saved = saved;
            cm.Id = id.Value;
            //cm.CookedFood = true;

            return View("Create", cm);
        }

      

        public IEnumerable<CategoryModel> GetAllCategories()
        {
            List<CategoryModel> lst = new List<CategoryModel>();

            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("GetCategories", myConnection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    myConnection.Open();

                    //SqlParameter custId = cmd.Parameters.AddWithValue("@CustomerId", 10);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            int id = dr.GetInt32(0);    // Weight int
                            string description = dr.GetString(1);  // Name string
                            string name = dr.GetString(2); // Breed string 
                            bool isActive = dr.GetBoolean(3); // Breed string 
                            //lst.Add(new CategoryModel { Id = id, Description = description, IsActive = isActive, Name = name });
                            yield return new CategoryModel { Id = id, Description = description, IsActive = isActive, Name = name };

                        }
                    }
                }
            }
        }


        //public ActionResult DistributeOLd(int? storeId)
        //{
        //    var store = _storeService.GetById(storeId.Value);

        //    var points = _StorePointService.GetAll();

        //    ItemRawIndexModel cim1 = new ItemRawIndexModel();

        //    cim1.StoreId = storeId.Value;

        //    cim1.StorePoints = points.ToList();

        //    return View(cim1);
        //}

        public ActionResult DistributeToPoint(int? id, int? poId)
        {
            var point = _storePointService.GetById(id.Value);

            ItemRawIndexModel cim1 = new ItemRawIndexModel();

            cim1.StorePoint = point;

            cim1.POItem = _purchaseOrderService.GetByIdInclude(poId.Value, "PurchaseOrderItems, PurchaseOrderItems.StockItem");

            return View(cim1);
        }

        public ActionResult EditTransfer(int? id)
        {
            var purchaseOder = _purchaseOrderService.GetById(id.Value);

            var poItems = new List<PurchaseOrder>();

            var batchPoItems = _batchService.GetAll().ToList();

            ItemRawIndexModel cim1 = new ItemRawIndexModel();

            cim1.BatchPoItems = batchPoItems;

            cim1.PoId = purchaseOder.Id;

            cim1.PurchaseOrderItems = purchaseOder.PurchaseOrderItems;

            cim1.StoreTransferredItems = purchaseOder.PurchaseOrderItems.SelectMany(x => x.StorePointItems).ToList();

            return View(cim1);
        }


        public ActionResult IndexStore()
        {
            var stores = _storeService.GetAll().ToList();
            ItemRawIndexModel cim1 = new ItemRawIndexModel { StoreList = stores };
            return View(cim1);
        }


        public ActionResult IndexRecievables(int? id)
        {
            var poItems = new List<PurchaseOrder>();
            ItemRawIndexModel cim1 = new ItemRawIndexModel();

            if (id.HasValue)
            {
                poItems = _purchaseOrderService.GetAll().ToList();
                cim1 = new ItemRawIndexModel { POItemList = poItems };
                cim1.CanCreatePO = true;
                return View(cim1);
            }

            poItems = _purchaseOrderService.GetAll().OrderByDescending(x => x.OrderDate).ToList();
            cim1 = new ItemRawIndexModel { POItemList = poItems };
            cim1.CanCreatePO = false;
            return View(cim1);
        }


        public ActionResult IndexForPurchase()
        {
            var poItems = new List<PurchaseOrder>();

            ItemRawIndexModel cim1 = new ItemRawIndexModel();

            var thisUser = _personService.GetAllForLogin().ToList().Where(x => x.Username.ToUpper().Equals(User.Identity.Name)).FirstOrDefault();

            poItems = _purchaseOrderService.GetAll().OrderByDescending(x => x.OrderDate).ToList();

            cim1 = new ItemRawIndexModel { POItemList = poItems };

            cim1.CanCreatePO = false;

            cim1.ThisUserId = thisUser.PersonID;

            return View(cim1);
        }


        public ActionResult IndexStockTaking()
        {

            ItemRawIndexModel cim1 = new ItemRawIndexModel();

            var thisUser = _personService.GetAllForLogin().ToList().Where(x => x.Username.ToUpper().Equals(User.Identity.Name)).FirstOrDefault();

            var allStores = _storeService.GetAll().ToList();

            return View(cim1);
        }


        public ActionResult OrderStock()
        {

            ItemRawIndexModel cim1 = new ItemRawIndexModel();

            var thisUser = _personService.GetAllForLogin().ToList().Where(x => x.Username.ToUpper().Equals(User.Identity.Name)).FirstOrDefault();

            var allStores = _storeService.GetAll().ToList();

            return View(cim1);
        }


        public ActionResult IndexStock(int? id)
        {

            ItemRawIndexModel cim1 = new ItemRawIndexModel();

            var thisUser = _personService.GetAllForLogin().ToList().Where(x => x.Username.ToUpper().Equals(User.Identity.Name)).FirstOrDefault();

            var allStores = _storeService.GetAll().ToList();

            cim1.StoreId = id.Value;

            if (id.HasValue)
            {
                allStores = allStores.Where(x => x.Id == id.Value).ToList();
                cim1 = new ItemRawIndexModel { allStores = allStores };
                cim1.CanCreatePO = true;
                cim1.ThisUserId = thisUser.PersonID;
                cim1.StoreId = id.Value;

                return View(cim1);
            }


            cim1 = new ItemRawIndexModel { allStores = allStores };
            cim1.CanCreatePO = false;
            cim1.ThisUserId = thisUser.PersonID;
            cim1.StoreId = id.Value;

            return View(cim1);
        }

        public ActionResult DamagesIndexNew()
        {
            ItemRawIndexModel cim1 = new ItemRawIndexModel();
            cim1.DamagesList = _damagedBatchItemService.GetAll().ToList();
            return View(cim1);
        }

        [HttpGet]
        public ActionResult RecordDamage(int? id)
        {
            DamagedItemViewModel divm = new DamagedItemViewModel();

            if (id.HasValue && id.Value > 0)
            {
                var d = _damagedBatchItemService.GetById(id.Value);

                divm.NumberOfItems = d.NumberDamaged;
                divm.ItemId = d.ItemId;
                divm.DistributionPointId = d.DistributionPointId;
                divm.StorePointId = d.StorePointId;
                divm.Id = d.Id;
                divm.Description = d.Description;
            }
            else
            {
                divm.NumberOfItems = 0;
                divm.ItemId = 0;
                divm.DistributionPointId = 0;
                divm.StorePointId = 0;
                divm.Id = 0;
                divm.Description = "";
            }
            

            var items = GetAllItems().ToList();
            var items1 = GetAllItemsNonRaw().ToList();

            items.AddRange(items1);

            var points = _storePointService.GetAll();
            var distributionPoints = _distributionPointService.GetAll();

            items.Insert(0, new ItemModel { StockItemName = "-- Please Select--", Id = 0 });
            points.Insert(0, new StorePoint { Name = "-- Please Select--", Id = 0 });
            distributionPoints.Insert(0, new DistributionPoint { Name = "-- Please Select--", Id = 0 });


            IEnumerable<SelectListItem> selectListItems =
               from c in items
               select new SelectListItem
               {
                   Selected = (c.Id == divm.ItemId),
                   Text = c.StockItemName,
                   Value = c.Id.ToString()
               };

            IEnumerable<SelectListItem> selectListStores =
              from c in points
              select new SelectListItem
              {
                  Selected = (c.Id == divm.StorePointId),
                  Text = c.Name,
                  Value = c.Id.ToString()
              };

            IEnumerable<SelectListItem> selectListTerminals =
              from c in distributionPoints
              select new SelectListItem
              {
                  Selected = (c.Id == divm.DistributionPointId),
                  Text = c.Name,
                  Value = c.Id.ToString()
              };

            divm.Terminals = selectListTerminals;
            divm.Stores = selectListStores;
            divm.Items = selectListItems;

            return View(divm);
        }

        [HttpPost]
        public ActionResult RecordDamage(DamagedItemViewModel model)
        {
            DamagedItemViewModel divm = model;

            int? id = model.StorePointId;

            if(model.StorePointId == 0 && model.DistributionPointId == 0)
            {
                ModelState.AddModelError("_Form", "Please you must select either a Terminal or a Store");
            }

            if(ModelState.IsValid)
            {
                var existingId = 0;

                if (model.Id > 0)
                {
                    var existingDamage = _damagedBatchItemService.GetById(model.Id);
                    existingDamage.Description = model.Description;
                    existingDamage.NumberDamaged = model.NumberOfItems;
                    existingDamage.ItemId = model.ItemId.Value;
                    existingDamage.StorePointId = null;
                    existingDamage.DistributionPointId = null;

                    if (model.StorePointId.HasValue && model.StorePointId.Value > 0)
                    {
                        existingDamage.StorePointId = model.StorePointId.Value;
                    }

                    if (model.DistributionPointId.HasValue && model.DistributionPointId.Value > 0)
                    {
                        existingDamage.DistributionPointId = model.DistributionPointId.Value;
                    }

                    _damagedBatchItemService.Update(existingDamage);

                    existingId = existingDamage.Id;


                }
                else
                {
                    var existingDamage = new DamagedBatchItem();

                    existingDamage.Description = model.Description;
                    existingDamage.NumberDamaged = model.NumberOfItems;
                    existingDamage.ItemId = model.ItemId.Value;
                    existingDamage.StorePointId = null;
                    existingDamage.DistributionPointId = null;

                    if (model.StorePointId.HasValue && model.StorePointId.Value > 0)
                    {
                        existingDamage.StorePointId = model.StorePointId.Value;
                    }


                    if (model.DistributionPointId.HasValue && model.DistributionPointId.Value > 0)
                    {
                        existingDamage.DistributionPointId = model.DistributionPointId.Value;
                    }

                    existingDamage.DateReported = DateTime.Now;

                    existingDamage = _damagedBatchItemService.Create(existingDamage);

                    existingId = existingDamage.Id;

                }

                return RedirectToAction("DamagesIndexNew", new { id = existingId, saved = true });
            }

            


            var items = GetAllItems().ToList();
            var items2 = GetAllItemsNonRaw().ToList();
            items.AddRange(items2);

            var points = _storePointService.GetAll();
            var distributionPoints = _distributionPointService.GetAll();

            items.Insert(0, new ItemModel { StockItemName = "-- Please Select--", Id = 0 });
            points.Insert(0, new StorePoint { Name = "-- Please Select--", Id = 0 });
            distributionPoints.Insert(0, new DistributionPoint { Name = "-- Please Select--", Id = 0 });

            IEnumerable<SelectListItem> selectListItems =
               from c in items
               select new SelectListItem
               {
                   Selected = (c.Id == model.ItemId),
                   Text = c.StockItemName,
                   Value = c.Id.ToString()
               };

            IEnumerable<SelectListItem> selectListStores =
              from c in points
              select new SelectListItem
              {
                  Selected = (c.Id == model.StorePointId),
                  Text = c.Name,
                  Value = c.Id.ToString()
              };

            IEnumerable<SelectListItem> selectListTerminals =
              from c in distributionPoints
              select new SelectListItem
              {
                  Selected = (c.Id == model.DistributionPointId),
                  Text = c.Name,
                  Value = c.Id.ToString()
              };

            divm.Terminals = selectListTerminals;
            divm.Stores = selectListStores;
            divm.Items = selectListItems;

            return View(divm);
        }
        public ActionResult DamagesIndex()
        {

            var points = _storePointService.GetAll();

            ItemRawIndexModel cim1 = new ItemRawIndexModel();

            cim1.StorePoints = points.ToList();

            cim1.CanRecordDamages = _batchService.GetAll().LastOrDefault() != null;

            return View(cim1);
        }


        [HttpGet]
        public ActionResult IndexMaterials(int? id)
        {

            if (id.HasValue)
            {
                var items = GetAllItems();
                items = items.Where(x => x.IsActive).ToList();
                ItemRawIndexModel cim = new ItemRawIndexModel { ItemList = items };
                return View("ProductAlerts", cim);
            }

            var items1 = GetAllItemsRaw().Where(x => x.RawItem && !x.IsActive);
            ItemRawIndexModel cim1 = new ItemRawIndexModel { RawItemList = items1.ToList() };
            return View(cim1);
        }



        public ActionResult Distribute(int? id)
        {
            var points = _storePointService.GetAll();

            ItemRawIndexModel cim1 = new ItemRawIndexModel();

            cim1.StorePoints = points.ToList();

            cim1.PoId = id.Value;

            return View(cim1);
        }

        public ActionResult TransferPOEdit()
        {
            var poItems = new List<PurchaseOrder>();

            ItemRawIndexModel cim1 = new ItemRawIndexModel();

            var thisUser = _personService.GetAllForLogin().ToList().Where(x => x.Username.ToUpper().Equals(User.Identity.Name)).FirstOrDefault();

            poItems = _purchaseOrderService.GetAllByInclude("PurchaseOrderItems,PurchaseOrderItems.StockItem").Where(x => x.TransferDone).OrderByDescending(x => x.OrderDate).ToList();

            cim1 = new ItemRawIndexModel { POItemList = poItems };

            cim1.CanCreatePO = false;

            cim1.ThisUserId = thisUser.PersonID;

            return View(cim1);
        }

        public ActionResult TransferPO()
        {
            var poItems = new List<PurchaseOrder>();

            ItemRawIndexModel cim1 = new ItemRawIndexModel();

            var thisUser = _personService.GetAllForLogin().ToList().Where(x => x.Username.ToUpper().Equals(User.Identity.Name)).FirstOrDefault();

            poItems = _purchaseOrderService.GetAllByInclude("PurchaseOrderItems,PurchaseOrderItems.StockItem").Where(x => x.GoodsRecieved && !x.Completed && x.PurchaseOrderItems.Count > 0).OrderByDescending(x => x.OrderDate).ToList();

            cim1 = new ItemRawIndexModel { POItemList = poItems };

            cim1.CanCreatePO = false;

            cim1.ThisUserId = thisUser.PersonID;

            return View(cim1);
        }


        public ActionResult IndexReturns()
        {
            var poItems = new List<PurchaseOrder>();

            ItemRawIndexModel cim1 = new ItemRawIndexModel();

            var thisUser = _personService.GetAllForLogin().ToList().Where(x => x.Username.ToUpper().Equals(User.Identity.Name)).FirstOrDefault();

            poItems = _purchaseOrderService.GetAllByInclude("PurchaseOrderItems,PurchaseOrderItems.StockItem").Where(x => x.GoodsRecieved && !x.TransferDone).OrderByDescending(x => x.OrderDate).ToList();

            cim1 = new ItemRawIndexModel { POItemList = poItems };

            cim1.CanCreatePO = false;

            cim1.ThisUserId = thisUser.PersonID;

            return View(cim1);
        }


        public ActionResult IndexPO()
        {
            var poItems = new List<PurchaseOrder>();

            ItemRawIndexModel cim1 = new ItemRawIndexModel();

            var thisUser = _personService.GetAllForLogin().ToList().Where(x => x.Username.ToUpper().Equals(User.Identity.Name)).FirstOrDefault();

            poItems = _purchaseOrderService.GetAll().Where(x => string.IsNullOrEmpty(x.Notes)).OrderByDescending(x => x.OrderDate).ToList();

            cim1 = new ItemRawIndexModel { POItemList = poItems };

            cim1.CanCreatePO = false;

            cim1.ThisUserId = thisUser.PersonID;

            return View(cim1);
        }



        public ActionResult CompleteStockIndex(int? id)
        {
            var stockList = _purchaseOrderItemService.GetAll().ToList().GroupBy(x => x.StockItem).Select(x => new MainInventoryViewModel { ItemName = x.Key.StockItemName, Remaining = x.ToList().Sum(y => y.Qty), TotalRecieved = x.ToList().Sum(y => y.QtyRecieved) }).ToList();

            ItemRawIndexModel cim1 = new ItemRawIndexModel { POStockList = stockList };

            return View(cim1);
        }


        public ActionResult SalesStockIndex(int? id)
        {
            if (id.HasValue)
            {
                var items = GetAllItems();
                items = items.ToList();
                ItemRawIndexModel cim = new ItemRawIndexModel { ItemList = items };
                return View("ProductAlerts", cim);
            }

            var itemsPOS = _pOSItemService.GetAllInclude("StockItem").ToList();
            ItemRawIndexModel cim1 = new ItemRawIndexModel { PosItemList = itemsPOS };
            return View(cim1);
        }

        public ActionResult Index(int? id)
        {
            if (id.HasValue)
            {
                var items = GetAllItems();
                items = items.Where(x => x.IsActive).ToList();
                ItemRawIndexModel cim = new ItemRawIndexModel { ItemList = items };
                return View("ProductAlerts", cim);
            }

            var items1 = GetAllItems();
            ItemRawIndexModel cim1 = new ItemRawIndexModel { ItemList = items1.Where(x => x.IsActive).ToList() };
            return View(cim1);
        }



        [HttpGet]
        public ActionResult CreatePO()
        {

            PurchaseOrderModel pm = new PurchaseOrderModel { Id = 0, Description = "", IsRawItem = false };

            return View(pm);
        }

        [HttpGet]
        public ActionResult CreateRawMaterials()
        {
            int catId = 0;
            var cats = GetAllCategories();
            var categories = cats.ToList();

            categories.Insert(0, new CategoryModel { Name = "-- Please Select--", Id = 0 });

            IEnumerable<SelectListItem> selectList =
                from c in categories
                select new SelectListItem
                {
                    Selected = (c.Id == catId),
                    Text = c.Name,
                    Value = c.Id.ToString()
                };

            RawItemModel cm = new RawItemModel { RawItemQuantity = 1M, ClubPrice = decimal.Zero, CookedFood = false, Id = 0, selectList = selectList, NotNumber = 100, OrigPrice = 100, Barcode = "", Quantity = 0, TotalQuantity = 0, Description = "Description" };

            return View(cm);
        }


        [HttpGet]
        public ActionResult Create()
        {
            int catId = 0;
            var cats = GetAllCategories();
            var categories = cats.ToList();

            categories.Insert(0, new CategoryModel { Name = "-- Please Select--", Id = 0 });

            IEnumerable<SelectListItem> selectList =
                from c in categories
                select new SelectListItem
                {
                    Selected = (c.Id == catId),
                    Text = c.Name,
                    Value = c.Id.ToString()
                };

            ItemModel cm = new ItemModel { RawItemQuantity = 1M, ClubPrice = decimal.Zero, CookedFood = false, Id = 0, selectList = selectList, NotNumber = 100, OrigPrice = 100, Barcode = "", Quantity = 0, TotalQuantity = 0, Description = "Description" };

            return View(cm);
        }


        [HttpPost]
        public ActionResult CREATEPORECIEVED(PurchaseOrderModel pm, string submitButton)
        {
            int id = pm.Id;
            int storeId = 1;

            if (ModelState.IsValid)
            {
                if (pm.Id > 0)
                {
                    var existingPo = _purchaseOrderService.GetById(pm.Id);

                    bool addToStock = false;

                    if (submitButton.StartsWith("A"))
                    {
                        existingPo.Completed = true;
                        existingPo.Notes = "ACCEPTED";
                        addToStock = true;
                    }
                    else
                    {
                        existingPo.InvoiceRaised = false;

                        existingPo.Notes = "REJECTED, Incorrectc Items count";
                    }

                    _purchaseOrderService.Update(existingPo);

                    //if (addToStock)
                    //AddToStock(existingPo, existingPo.AssignedTo);
                }


                return RedirectToAction("IndexStore");
            }


            return RedirectToAction("IndexStore");
        }

        private void AddToStock(PurchaseOrder existingPo, int thisUserId)
        {
            var today = DateTime.Now;

            foreach (var poi in existingPo.PurchaseOrderItems.Where(x => x.Qty > 0))
            {
                var storeItem = _storeItemService.GetAll().FirstOrDefault(x => x.ItemId == poi.ItemId);
                if (storeItem == null)
                {
                    var si = new StoreItem();
                    si.DateAdded = today;
                    si.ItemId = poi.ItemId;
                    si.Quantity = poi.Qty;
                    si.RecievedBy = thisUserId;

                    si.Remaining = si.Quantity;
                    _storeItemService.Create(si);

                }
                else
                {
                    storeItem.DateAdded = today;
                    storeItem.Quantity = poi.Qty;
                    storeItem.Remaining = storeItem.Remaining + poi.Qty;
                    _storeItemService.Update(storeItem);

                }

            }
        }


        [HttpPost]
        public ActionResult ReassignToStoreManager(PurchaseOrderModel pm)
        {
            int id = pm.Id;

            var thisUser = _personService.GetAllForLogin().Where(x => x.Username.ToUpper().Equals(User.Identity.Name.ToUpper())).FirstOrDefault();

            if (ModelState.IsValid)
            {
                if (pm.Id > 0)
                {
                    var existingPo = _purchaseOrderService.GetById(pm.Id);
                    var allExistingPoItems = existingPo.PurchaseOrderItems.ToList();


                    existingPo.Description = pm.Description;

                    var allRealStock = _stockItemService.GetAll().Where(x => !x.CookedFood).ToList();

                    var allStockItemIds = allRealStock.Select(x => x.Id).ToList();

                    int totalNumberOfItems = 0;

                    var totalValue = decimal.Zero;

                    foreach (var itemId in allStockItemIds)
                    {
                        var name = "POItem_" + itemId.ToString();

                        var realStock = allRealStock.FirstOrDefault(x => x.Id == itemId);

                        if (Request.Form[name] != null)
                        {
                            var qty = 0;
                            int.TryParse(Request.Form[name].ToString(), out qty);

                            if (qty == 0)
                                continue;

                            totalNumberOfItems++;

                            var existingPoItem = _purchaseOrderItemService.GetAll().FirstOrDefault(x => x.ItemId == itemId && x.PurchaseOrderId == existingPo.Id);

                            if (existingPoItem != null)
                            {
                                existingPoItem.Qty = qty;
                                var thisValue = (decimal)(realStock.UnitPrice * qty);
                                totalValue += thisValue;
                                existingPoItem.TotalPrice = thisValue;
                                _purchaseOrderItemService.Update(existingPoItem);
                            }
                            else
                            {
                                var thisValue = (decimal)(realStock.UnitPrice * qty);
                                totalValue += thisValue;
                                var poItem = new PurchaseOrderItem { PurchaseOrderId = pm.Id, ItemId = itemId, Qty = qty, QtyRecieved = 0, TotalPrice = thisValue };
                                _purchaseOrderItemService.Create(poItem);
                            }
                        }
                    }

                    existingPo.NetValue = totalValue;

                    existingPo.BaseNetValue = totalValue;
                    existingPo.InvoiceRaised = true;

                    existingPo.GoodsBought = true;

                    existingPo.GoodsRecieved = false;

                    _purchaseOrderService.Update(existingPo);

                    RaiseInvoice(existingPo, thisUser.PersonID);

                }


                return RedirectToAction("IndexForPurchase");
            }

            var persons = _personService.GetAllForLogin().Where(x => x.IsActive && x.PersonTypeId != (int)PersonTypeEnum.Guest).ToList();

            if (pm.Id == 0)
            {
                persons = new List<Person>() { thisUser };
            }
            else
            {
                persons.Insert(0, new Person { DisplayName = "-- Please Select--", PersonID = 0 });
            }

            return View(pm);
        }

        private void RaiseInvoice(PurchaseOrder existingPo, int creatorId)
        {
            Invoice invoice = _invoiceService.GetAll().FirstOrDefault(x => x.PurchaseOrderId == existingPo.Id);

            if (invoice == null)
            {
                invoice = new Invoice();
                invoice.AssignedTo = creatorId;
                invoice.CreatedDate = DateTime.Now;
                invoice.InvoiceDate = DateTime.Now;
                invoice.StatusType = (int)InvoiceStatusEnum.Paid;
                invoice.IsActive = true;
                invoice.RaisedBy = creatorId;
                invoice.PurchaseOrderId = existingPo.Id;
                invoice.TotalAmount = existingPo.NetValue;
                _invoiceService.Create(invoice);

            }
            else
            {
                invoice.TotalAmount = existingPo.NetValue;
                _invoiceService.Update(invoice);
            }
        }


        [HttpPost]
        public ActionResult StockLevel()
        {
            var allRealStock = _stockItemService.GetAll().Where(x => x.RawItem).ToList();

            var allStockItemIds = allRealStock.Select(x => x.Id).ToList();

            int totalNumberOfItems = 0;

            foreach (var itemId in allStockItemIds)
            {
                var name = "StoreItem_" + itemId.ToString();
                var strStartDate = "StartDate_" + itemId.ToString();
                var strEndDate = "EndDate_" + itemId.ToString();


                var realStock = allRealStock.FirstOrDefault(x => x.Id == itemId);

                if (Request.Form[name] != null)
                {
                    var qty = 0;

                    int.TryParse(Request.Form[name].ToString(), out qty);

                    if (qty == 0)
                        continue;

                    var startDate = DateTime.Now;
                    var endDate = DateTime.Now;

                    DateTime.TryParse(Request.Form[strStartDate].ToString(), out startDate);
                    DateTime.TryParse(Request.Form[strEndDate].ToString(), out endDate);


                    realStock.Quantity = realStock.Quantity - qty;
                    //realStock.TotalQuantity = realStock.TotalQuantity - qty;

                    _stockItemService.Update(realStock);

                    _usedStockItemService.Create(new UsedStockItem { EndDate = endDate, StartDate = startDate, ItemId = realStock.Id, UsedQuantity = qty });

                    totalNumberOfItems++;
                }
            }



            return RedirectToAction("IndexMaterials");
        }


        [HttpPost]
        public ActionResult ReturnPO(int? id)
        {
            int? poId = id;

            var allRealStock = _stockItemService.GetAll().Where(x => !x.CookedFood).ToList();

            var allStockItemIds = allRealStock.Select(x => x.Id).ToList();

            int totalNumberOfItems = 0;

            var thisUser = _personService.GetAllForLogin().Where(x => x.Username.ToUpper().Equals(User.Identity.Name.ToUpper())).FirstOrDefault();

            decimal returnAmount = decimal.Zero;

            foreach (var itemId in allStockItemIds)
            {
                var name = "StoreItem_" + itemId.ToString();

                var realStock = allRealStock.FirstOrDefault(x => x.Id == itemId);

                if (Request.Form[name] != null)
                {
                    var qty = 0;
                    int.TryParse(Request.Form[name].ToString(), out qty);

                    if (qty == 0)
                        continue;

                    var actualPoItem = _purchaseOrderItemService.GetAll().FirstOrDefault(x => x.ItemId == itemId && x.PurchaseOrderId == poId.Value);

                    if (actualPoItem != null)
                    {
                        actualPoItem.Qty = actualPoItem.Qty - qty;
                        //actualPoItem.QtyRecieved = actualPoItem.Qty - qty;
                        actualPoItem.QtyRecieved = actualPoItem.QtyRecieved - qty;
                        actualPoItem.Returns = qty;
                        actualPoItem.ReturnValue = qty * actualPoItem.StockItem.OrigPrice;
                        _purchaseOrderItemService.Update(actualPoItem);
                        returnAmount += (qty * actualPoItem.StockItem.OrigPrice);
                    }

                    totalNumberOfItems++;
                }
            }

            if (returnAmount > decimal.Zero)
            {
                var thePo = _purchaseOrderService.GetById(poId.Value);

                if (thePo != null)
                {
                    thePo.BaseNetValue = thePo.BaseNetValue - returnAmount;
                    thePo.NetValue = thePo.NetValue - returnAmount;
                    _purchaseOrderService.Update(thePo);
                }
            }

            return RedirectToAction("IndexReturns");
        }


        [HttpPost]
        public ActionResult EditDistributeToPoint(int? poId, int? dummy)
        {
            //var all = _batchService.GetAll().Where(x => x.PurchaseOrderItem.PurchaseOrderId == poId).ToList();
            var all = _batchService.GetAll().ToList();

            var allPurchaseOrderItems = _purchaseOrderItemService.GetAll().ToList();
            var allStoreItems = _storePointItemService.GetAll().ToList();
            var allBatchIds = all.Select(x => x.Id).ToList();
            var itemsTransfered = false;

            foreach (var storeItemId in allStoreItems.Select(x => x.Id))
            {
                var newQtyName = "StoreItem_" + storeItemId.ToString();
                var originalQtyName = "POItem_" + storeItemId.ToString();

                int newQty = 0;
                int originalQty = 0;

                try
                {
                    newQty = int.Parse(Request.Form[newQtyName]);
                }
                catch
                {

                }

                try
                {
                    originalQty = int.Parse(Request.Form[originalQtyName]);
                }
                catch
                {

                }

                var storeItem = allStoreItems.FirstOrDefault(x => x.Id == storeItemId);

                var realPoisItem = allPurchaseOrderItems.FirstOrDefault(x => x.Id == storeItem.PurchaseOrderItemId);

                if (storeItem != null)
                {
                    var pQty = newQty - originalQty;

                    if (pQty < 0)
                    {
                        storeItem.Quantity += pQty;
                    }
                    else
                    {
                        storeItem.Quantity += pQty;
                    }

                    storeItem.Remaining -= originalQty;
                    storeItem.Remaining += newQty;
                    _storePointItemService.Update(storeItem);
                }

                if (realPoisItem != null)
                {
                    var pQty = newQty - originalQty;

                    if (pQty < 0)
                    {
                        pQty = pQty * (-1);
                        realPoisItem.Qty += pQty;
                    }
                    else
                    {
                        realPoisItem.Qty -= pQty;
                    }

                    realPoisItem.QtyRecieved -= originalQty;
                    realPoisItem.QtyRecieved += newQty;
                    _purchaseOrderItemService.Update(realPoisItem);
                }

                itemsTransfered = true;
            }

            if (itemsTransfered)
            {
                var po = _purchaseOrderService.GetAllByInclude("PurchaseOrderItems").FirstOrDefault(x => x.Id == poId.Value);

                var count = po.PurchaseOrderItems.Sum(x => x.QtyRecieved);

                var countOrig = po.TotalNumberOfItems;

                var someItemsAreRemaining = count < countOrig;

                if (po != null)
                {
                    if (!someItemsAreRemaining)
                    {
                        po.Completed = true;
                        po.Notes = "Full Transfer Done";
                        po.TransferDone = true;
                    }
                    else
                    {
                        po.Notes = "Some Transfer Done";
                        po.Completed = false;
                    }
                    _purchaseOrderService.Update(po);
                }
            }

            return RedirectToAction("TransferPOEdit");
        }

        [HttpPost]
        public ActionResult DistributeToPoint(int? id, int? poId, int? dummy)
        {
            var allRealStock = _stockItemService.GetAll().Where(x => !x.CookedFood).ToList();

            var allStockItemIds = allRealStock.Select(x => x.Id).ToList();

            int totalNumberOfItems = 0;

            bool itemsTransfered = false;

            var thisUser = _personService.GetAllForLogin().Where(x => x.Username.ToUpper().Equals(User.Identity.Name.ToUpper())).FirstOrDefault();

            var batchDate = DateTime.Now;

            foreach (var itemId in allStockItemIds)
            {
                var name = "StoreItem_" + itemId.ToString();

                var realStock = allRealStock.FirstOrDefault(x => x.Id == itemId);

                if (Request.Form[name] != null)
                {
                    var qty = 0;
                    int.TryParse(Request.Form[name].ToString(), out qty);

                    var actualPoItem = _purchaseOrderItemService.GetAll().FirstOrDefault(x => x.ItemId == itemId && x.PurchaseOrderId == poId.Value);

                    if (qty == 0 || actualPoItem == null)
                        continue;

                    if (actualPoItem != null)
                    {
                        actualPoItem.Qty = actualPoItem.Qty - qty;
                        _purchaseOrderItemService.Update(actualPoItem);
                    }

                    totalNumberOfItems++;

                    var existingStorePointItem = _storePointItemService.GetAll().FirstOrDefault(x => x.StorePointId == id.Value && x.PurchaseOrderItemId == actualPoItem.Id);

                    if (existingStorePointItem != null)
                    {
                        existingStorePointItem.Quantity += qty;
                        existingStorePointItem.Remaining += qty;
                       
                        _storePointItemService.Update(existingStorePointItem);

                        itemsTransfered = true;
                    }
                    else
                    {
                        
                        StorePointItem storePointItem = new StorePointItem();
                        storePointItem.StorePointId = id.Value;
                        storePointItem.PurchaseOrderItemId = actualPoItem.Id;
                        storePointItem.Quantity = qty;
                        storePointItem.Remaining = qty;
                        storePointItem.IsActive = true;
                        _storePointItemService.Create(storePointItem);
                        itemsTransfered = true;
                    }
                }
            }

            if (itemsTransfered)
            {
                var po = _purchaseOrderService.GetAllByInclude("PurchaseOrderItems").FirstOrDefault(x => x.Id == poId.Value);

                var someItemsAreRemaining = po.PurchaseOrderItems.Sum(x => x.Qty) > 0;

                if (po != null)
                {
                    if (!someItemsAreRemaining)
                    {
                        po.Completed = true;
                        po.Notes = "Full Transfer Done";
                        po.TransferDone = true;
                    }
                    else
                    {
                        po.Notes = "Some Transfer Done";
                        po.TransferDone = true;
                    }

                    _purchaseOrderService.Update(po);
                }
            }

            return RedirectToAction("TransferPO");
        }

        [HttpPost]
        public ActionResult CreatePO(PurchaseOrderModel pm)
        {
            int id = pm.Id;

            var thisUser = _personService.GetAllForLogin().Where(x => x.Username.ToUpper().Equals(User.Identity.Name.ToUpper())).FirstOrDefault();

            if (ModelState.IsValid)
            {
                if (pm.Id > 0)
                {
                    var existingPo = _purchaseOrderService.GetById(pm.Id);
                    var allExistingPoItems = existingPo.PurchaseOrderItems.ToList();

                    existingPo.OrderDate = pm.OrderDate;
                    existingPo.SupplierInvoice = pm.SupplierReference;
                    existingPo.Description = pm.Description;
                    existingPo.GoodsRecieved = pm.Recieved;
                    existingPo.IsRawItem = pm.IsRawItem;


                    var allRealStock = _stockItemService.GetAll().Where(x => !x.CookedFood).ToList();

                    var allStockItemIds = allRealStock.Select(x => x.Id).ToList();

                    int totalNumberOfItems = 0;
                    var totalValue = decimal.Zero;
                    

                    foreach (var itemId in allStockItemIds)
                    {
                        var name = "POItem_" + itemId.ToString();

                        var realStock = allRealStock.FirstOrDefault(x => x.Id == itemId);

                        if (Request.Form[name] != null)
                        {
                            var qty = 0;

                            int.TryParse(Request.Form[name].ToString(), out qty);

                            if (qty == 0)
                                continue;

                            totalNumberOfItems += qty;

                            var existingPoItem = _purchaseOrderItemService.GetAll().FirstOrDefault(x => x.ItemId == itemId && x.PurchaseOrderId == existingPo.Id);

                            if (existingPoItem != null)
                            {
                                existingPoItem.Qty = qty;
                                existingPoItem.QtyRecieved = qty;
                                var thisValue = (decimal)(realStock.OrigPrice * qty);
                                totalValue += thisValue;
                                existingPoItem.TotalPrice = thisValue;
                                _purchaseOrderItemService.Update(existingPoItem);

                                if (pm.Recieved)
                                {
                                    realStock.TotalQuantity -= realStock.Quantity;//Remove PreviouslyAdded
                                    realStock.Quantity = qty;
                                    realStock.TotalQuantity += qty;
                                    _stockItemService.Update(realStock);
                                }
                            }
                            else
                            {
                                var thisValue = (decimal)(realStock.OrigPrice * qty);
                                totalValue += thisValue;
                                var poItem = new PurchaseOrderItem { PurchaseOrderId = pm.Id, ItemId = itemId, Qty = qty, QtyRecieved = qty, TotalPrice = thisValue };
                                _purchaseOrderItemService.Create(poItem);

                                if (pm.Recieved)
                                {
                                    realStock.Quantity = qty;
                                    realStock.TotalQuantity += qty;
                                    _stockItemService.Update(realStock);
                                }

                            }
                        }
                    }

                    existingPo.NetValue = totalValue;
                    existingPo.BaseNetValue = totalValue;
                    existingPo.TotalNumberOfItems = totalNumberOfItems;

                    var file = Request.Files[0];

                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = existingPo.Id.ToString() + "_" + Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/Invoices"), fileName);
                        file.SaveAs(path);
                        existingPo.InvoicePath = fileName;
                    }

                    _purchaseOrderService.Update(existingPo);
                }
                else
                {

                    var po = new PurchaseOrder();
                    po.Description = pm.Description;
                    po.OrderDate = DateTime.Now;
                    po.RaisedBy = thisUser.PersonID;
                    po.CreatedDate = DateTime.Now;
                    po.NetValue = decimal.Zero;
                    po.BaseNetValue = decimal.Zero;
                    po.IsActive = true;
                    po.SupplierInvoice = pm.SupplierReference;
                    po.IsRawItem = pm.IsRawItem;
                    _purchaseOrderService.Create(po);

                    id = po.Id;

                }

                return RedirectToAction("POEdit", new { id, saved = true });
            }



            var persons = _personService.GetAllForLogin().Where(x => x.IsActive && x.PersonTypeId != (int)PersonTypeEnum.Guest).ToList();

            if (pm.Id == 0)
            {
                persons = new List<Person>() { thisUser };
            }
            else
            {
                persons.Insert(0, new Person { DisplayName = "-- Please Select--", PersonID = 0 });
            }


            return View(pm);
        }
    }
}