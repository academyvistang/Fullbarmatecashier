

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
using ClosedXML.Excel;

namespace BarAndRestaurantMate.Controllers
{
    [HandleError(View = "CustomErrorView")]
    public class ItemController : Controller
    {

        private  IStorePointService _storePointService;
        private  IStorePointItemService _storePointItemService;
        private  IPersonService _personService;
        private  IStoreService _storeService;
        private  IStoreItemService _storeItemService;
        private  IStockItemService _stockItemService;
        private  IInvoiceService _invoiceService;
        private  IDistributionPointService _distributionPointService;
        private  IDistributionPointItemService _distributionPointItemService;
        private  IBatchService _batchService;
        private  IPOSItemService _pOSItemService;
        private  IDamagedBatchItemService _damagedBatchItemService;
        private  IUsedStockItemService _usedStockItemService;

        protected override void Dispose(bool disposing)
        {

            if (disposing && _usedStockItemService != null)
            {
                _usedStockItemService.Dispose();
                _usedStockItemService = null;
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

            if (disposing && _distributionPointService != null)
            {
                _distributionPointService.Dispose();
                _distributionPointService = null;
            }

            if (disposing && _distributionPointItemService != null)
            {
                _distributionPointItemService.Dispose();
                _distributionPointItemService = null;
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



        public ItemController()
        {
            _storePointItemService = new StorePointItemService();
            _storePointService = new StorePointService();
            _personService = new PersonService();
            _storeService = new StoreService();
            _stockItemService = new StockActualItemService();
            _invoiceService = new InvoiceService();
            _storeItemService = new StoreItemService();
            _distributionPointService = new DistributionPointService();
            _distributionPointItemService = new DistributionPointItemService();
            _batchService = new BatchService();
            _pOSItemService = new POSItemService();
            _damagedBatchItemService = new DamagedBatchItemService();
            _usedStockItemService = new UsedStockItemService();
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
                            decimal totalPrice  = dr.GetDecimal(1);
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

        private IEnumerable<ItemModel> GetAllItems()
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
                            yield return new ItemModel { Id = id, UnitPrice = unitPrice,  Description = description, PicturePath = picturePath, NotStatus = notStatus,
                                IsActive = isActive, StockItemName = name, Quantity = qty, NotNumber = notNumber, CategoryId = categoryId, Barcode = barcode,
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

        [HttpPost]
        public ActionResult DeleteRawMaterials(RawItemModel cm)
        {
            var existingItem = POSService.StockItemService.GetSpecificItemRaw(cm.Id).FirstOrDefault();

            var posItem = _pOSItemService.GetAll().FirstOrDefault(x => x.ItemId == existingItem.Id);

            if (existingItem != null)
            {
                existingItem.IsActive = false;
                existingItem.RawItem = false;
                var id = UpdateItem(existingItem);
            }

            if (posItem != null)
            {
                posItem.IsActive = false;
                _pOSItemService.Update(posItem);
            }

            return RedirectToAction("IndexMaterials");
        }



        [HttpPost]
        public ActionResult Delete(ItemModel cm)
        {
            var existingItem = POSService.StockItemService.GetSpecificItem(cm.Id).FirstOrDefault();

            var posItem = _pOSItemService.GetAll().FirstOrDefault(x => x.ItemId == existingItem.Id);

            if (existingItem != null)
            {
                existingItem.IsActive = false;
                var id = UpdateItem(existingItem);
            }

            if (posItem != null)
            {
                posItem.IsActive = false;
                _pOSItemService.Update(posItem);
            }

            return RedirectToAction("Index");
        }

        

        public void GetAllImages(string searchText)
        {
           
        }

        //[OutputCache(Duration = 3600, VaryByParam = "none")]
        public ActionResult Sales()
        {
            var items = GetAllSoldItems();
            SoldItemIndexModel siim = new SoldItemIndexModel { ItemList = items.OrderByDescending(x => x.DateSold).ToList()};
            return View(siim);

        }

        //
        //public ActionResult PORecieved(int? id, bool? saved)
        //{
        //    var po = _storePointService.GetById(id.Value);

        //    var pm = new PurchaseOrderModel {  Id = po.Id, Description = po.Description, Value = po.NetValue };

        //    var persons = _personService.GetAllForLogin().Where(x => x.IsActive && x.PersonTypeId != (int)PersonTypeEnum.Guest).ToList();

        //    var stores = _storeService.GetAll().ToList();

        //    persons.Insert(0, new Person { DisplayName = "-- Please Select--", PersonID = 0 });            

           
        //    pm.Items = null;
        //    var list = po.PurchaseOrderItems.ToList();
        //    var existingItemList = list.Select(x => x.ItemId).ToList();
        //    var allExistingStockItems = _stockItemService.GetAll().Where(x => !x.CookedFood).ToList();
        //    var newPoList = allExistingStockItems.Where(x => !existingItemList.Contains(x.Id)).Select(x => new PurchaseOrderItem { ItemId = x.Id, StockItem = x, Qty = 0 }).ToList();
        //    list.AddRange(newPoList);
        //    pm.Items = list;
        //    pm.Saved = saved;

        //    return View("CreateRecievable", pm);
        //}

        //public FileResult DownloadInvoice(int? id)
        //{
        //    var po = _storePointService.GetById(id.Value);
        //    var path = Path.Combine(Server.MapPath("~/Invoices"), po.InvoicePath);
        //    byte[] fileBytes = System.IO.File.ReadAllBytes(path);
        //    //string fileName = "myfile.ext";
        //    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, po.InvoicePath);
        //    //return null;
        //}


       

        //public ActionResult POView(int? id, bool? saved)
        //{
        //    var po = _storePointService.GetById(id.Value);

        //    var pm = new PurchaseOrderModel { Id = po.Id, Description = po.Description, Value = po.NetValue };

           

        //    var persons = new List<Person>();

        //    if (po.PurchaseOrderItems.Count > 0)
        //    {
        //        persons = _personService.GetAllForLogin().Where(x => x.IsActive && x.PersonTypeId != (int)PersonTypeEnum.Guest).ToList();
        //        persons.Insert(0, new Person { DisplayName = "-- Please Select--", PersonID = 0 });
        //    }
        //    else
        //    {
        //        persons = _personService.GetAllForLogin().ToList();
        //    }

            

        //    pm.Items = null;
        //    var list = po.PurchaseOrderItems.ToList();
        //    var existingItemList = list.Select(x => x.ItemId).ToList();
        //    var allExistingStockItems = _stockItemService.GetAll().Where(x => !x.CookedFood).ToList();
        //    var newPoList = allExistingStockItems.Where(x => !existingItemList.Contains(x.Id)).Select(x => new PurchaseOrderItem { ItemId = x.Id, StockItem = x, Qty = 0 }).ToList();
        //    list.AddRange(newPoList);
        //    pm.Items = list;
        //    pm.Saved = saved;
        //    return View("POView", pm);
        //}

        
        //public ActionResult PODelete(int? id, bool? saved)
        //{
        //    var pos = _storePointItemService.GetAll().Where(x => x.PurchaseOrderId == id.Value);
            
        //    foreach (var e in pos)
        //    {
        //        var poi = _storePointItemService.GetById(e.Id);
        //        _storePointItemService.Delete(poi);
        //    }

        //    var poDelete = _storePointService.GetById(id.Value);
          
        //    _storePointService.Delete(poDelete);

        //    return RedirectToAction("IndexPO");
        //}


        //public ActionResult ReassignToStoreManager(int? id, bool? saved)
        //{
        //    var po = _storePointService.GetById(id.Value);

        //    var pm = new PurchaseOrderModel { Id = po.Id, Description = po.Description, Value = po.NetValue };

           
        //    var persons = new List<Person>();

        //    if (po.PurchaseOrderItems.Count > 0)
        //    {
        //        persons = _personService.GetAllForLogin().Where(x => x.IsActive && x.PersonTypeId != (int)PersonTypeEnum.Guest).ToList();
        //        persons.Insert(0, new Person { DisplayName = "-- Please Select--", PersonID = 0 });
        //    }
        //    else
        //    {
        //        persons = _personService.GetAllForLogin().ToList();
        //    }

           

        //    pm.Items = null;
        //    var list = po.PurchaseOrderItems.ToList();
        //    var existingItemList = list.Select(x => x.ItemId).ToList();
        //    var allExistingStockItems = _stockItemService.GetAll().Where(x => !x.CookedFood).ToList();
        //    var newPoList = allExistingStockItems.Where(x => !existingItemList.Contains(x.Id)).Select(x => new PurchaseOrderItem { ItemId = x.Id, StockItem = x, Qty = 0 }).ToList();
        //    list.AddRange(newPoList);
        //    pm.Items = list;
        //    pm.Saved = saved;
        //    return View("ReassignToStoreManager", pm);
        //}

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

                    var lastBtch = _batchService.GetAll().LastOrDefault(x => x.DistributionPointId == id.Value);

                    if (lastBtch != null)
                    {
                        var existingdbi = _damagedBatchItemService.GetAll().FirstOrDefault(x =>  x.ItemId == itemId);

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

        
        //public ActionResult POEditReturn(int? id, bool? saved)
        //{
        //    var po = _storePointService.GetByIdInclude(id.Value, "PurchaseOrderItems,PurchaseOrderItems.StockItem");

        //    var pm = new PurchaseOrderModel { InvoicePath = po.InvoicePath, Id = po.Id, Description = po.Description, Value = po.NetValue, SupplierReference = po.SupplierInvoice, Recieved = po.GoodsRecieved };

        //    var persons = new List<Person>();

        //    if (po.PurchaseOrderItems.Count == 0)
        //    {
        //        persons.Insert(0, new Person { DisplayName = "-- Please Select--", PersonID = 0 });
        //    }
        //    else
        //    {
        //        persons = _personService.GetAllForLogin().ToList();
        //    }


        //    pm.Items = null;
        //    var list = po.PurchaseOrderItems.ToList();
        //    var existingItemList = list.Select(x => x.ItemId).ToList();
        //    var allExistingStockItems = _stockItemService.GetAll().Where(x => !x.CookedFood).ToList();
        //    var newPoList = allExistingStockItems.Where(x => !existingItemList.Contains(x.Id)).Select(x => new PurchaseOrderItem { ItemId = x.Id, StockItem = x, Qty = 0 }).ToList();
        //    list.AddRange(newPoList);
        //    pm.Items = list;
        //    pm.Saved = saved;
        //    return View(pm);
        //}
       

        //public ActionResult POEdit(int? id, bool? saved)
        //{
        //    var po = _storePointService.GetByIdInclude(id.Value, "PurchaseOrderItems, PurchaseOrderItems.StockItem");

        //    var pm = new PurchaseOrderModel { IsRawItem = po.IsRawItem, NetValue = po.NetValue, InvoicePath = po.InvoicePath,  Id = po.Id, Description = po.Description, Value = po.NetValue, SupplierReference = po.SupplierInvoice, Recieved = po.GoodsRecieved };

        //    var persons = new List<Person>();

        //    if(po.PurchaseOrderItems.Count == 0)
        //    {  
        //        persons.Insert(0, new Person { DisplayName = "-- Please Select--", PersonID = 0 });
        //    }
        //    else
        //    {
        //        persons = _personService.GetAllForLogin().ToList();
        //    }

            
        //    pm.Items = null;
        //    var list = po.PurchaseOrderItems.ToList();
        //    var existingItemList = list.Select(x => x.ItemId).ToList();
        //    var allExistingStockItems = _stockItemService.GetAll().Where(x => !x.CookedFood).ToList();

        //    if(po.IsRawItem)
        //    {
        //        allExistingStockItems = allExistingStockItems.Where(x => !x.IsActive.Value && x.RawItem).ToList();
        //    }

        //    var newPoList = allExistingStockItems.Where(x => !existingItemList.Contains(x.Id)).Select(x => new PurchaseOrderItem { ItemId = x.Id, StockItem = x, Qty = 0}).ToList();

        //    list.AddRange(newPoList);
        //    pm.Items = list;
        //    pm.Saved = saved;
        //    return View("CreatePO", pm);
        //}

       
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

            if(cm.RawItem)
                cm.RawItemQuantity = cm.UnitPrice;

            cm.selectList = selectList;
            cm.Saved = saved;
            cm.Id = id.Value;
            //cm.CookedFood = true;

            return View("Create",cm);
        }

        [HttpPost]
        public ActionResult Edit(ItemModel cm, string[] orderNumbers)
        {
            int id = 0;

            POSService.Entities.StockItem existingBarcode = null;

            if (!string.IsNullOrEmpty(cm.Barcode))
            {
                existingBarcode = POSService.StockItemService.GetStockItems(1).FirstOrDefault(x => x.Barcode == cm.Barcode);

                if (cm.Id == 0)
                {
                    if (existingBarcode != null)
                        ModelState.AddModelError("BarCode", "Please use a different barcode. This barcode exists for a different item");
                }
                else
                {
                    if (existingBarcode != null && existingBarcode.Id != cm.Id)
                        ModelState.AddModelError("BarCode", "Please use a different barcode. This barcode exists for a different item");
                }
            }

            if (cm.CategoryId == 0)
            {
                ModelState.AddModelError("CategoryId", "Please select a category");
            }

            var url = string.Empty;

            var extension = string.Empty;

            var imageName = RemoveWhitespace(cm.StockItemName);

            if (orderNumbers != null && orderNumbers.Count() > 0)
            {
                url = orderNumbers.FirstOrDefault();
                extension = Path.GetExtension(url);
                //bool vv = !extension.EndsWith(".JPG", StringComparison.InvariantCultureIgnoreCase);

                if (!extension.EndsWith(".PNG", StringComparison.InvariantCultureIgnoreCase) &&
                    !extension.EndsWith(".GIF", StringComparison.InvariantCultureIgnoreCase)
                    && !extension.EndsWith(".BMP", StringComparison.InvariantCultureIgnoreCase) &&
                    !extension.EndsWith(".JPG", StringComparison.InvariantCultureIgnoreCase)
                    && !extension.EndsWith(".BMP", StringComparison.InvariantCultureIgnoreCase))
                {
                    ModelState.AddModelError("_Form", "Please select a different image, image selected is corrupt.");
                }

                cm.PicturePath = imageName + extension;
            }


            if (ModelState.IsValid)
            {
                var existingId = GetExistingItem(cm.StockItemName);

                if (existingId.HasValue && cm.Id == 0)
                {
                    var existingItem = StockItemService.GetSpecificItem(cm.Id).FirstOrDefault();

                    cm.IsActive = true;
                    cm.NotStatus = "Live";
                    cm.Status = "Live";
                    cm.TotalQuantity = cm.Quantity;
                    cm.HotelId = 1;

                    if (string.IsNullOrEmpty(cm.Barcode))
                    {
                        cm.Barcode = "";
                    }

                    existingItem.IsActive = true;
                    existingItem.Status = "Live";
                    existingItem.TotalQuantity = cm.Quantity;
                    existingItem.HotelId = 1;
                    existingItem.Barcode = cm.Barcode;
                    existingItem.StockItemName = cm.StockItemName;
                    existingItem.Description = cm.Description;
                    existingItem.UnitPrice = cm.UnitPrice;
                    existingItem.OrigPrice = cm.OrigPrice;
                    existingItem.NotNumber = cm.NotNumber;
                    existingItem.CategoryId = cm.CategoryId;
                    existingItem.CookedFood = cm.CookedFood;
                    existingItem.KitchenOnly = cm.KitchenOnly;
                    existingItem.RawItem = cm.RawItem;



                    var file = Request.Files[0];

                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/Products"), fileName);
                        file.SaveAs(path);
                        existingItem.PicturePath = fileName;
                        //return path;
                    }


                    id = UpdateItem(existingItem);


                }
                else if (cm.Id > 0)
                {
                    var existingItem = StockItemService.GetSpecificItem(cm.Id).FirstOrDefault();

                    cm.IsActive = true;
                    cm.NotStatus = "Live";
                    cm.Status = "Live";
                    cm.TotalQuantity = cm.Quantity;
                    cm.HotelId = 1;

                    if (string.IsNullOrEmpty(cm.Barcode))
                    {
                        cm.Barcode = "";
                    }

                    existingItem.IsActive = true;
                    existingItem.Status = "Live";
                    existingItem.TotalQuantity = cm.Quantity;
                    existingItem.HotelId = 1;
                    existingItem.Barcode = cm.Barcode;
                    existingItem.StockItemName = cm.StockItemName;
                    existingItem.Description = cm.Description;
                    existingItem.UnitPrice = cm.UnitPrice;
                    existingItem.OrigPrice = cm.OrigPrice;
                    existingItem.NotNumber = cm.NotNumber;
                    existingItem.CategoryId = cm.CategoryId;
                    existingItem.CookedFood = cm.CookedFood;
                    existingItem.KitchenOnly = cm.KitchenOnly;
                    existingItem.RawItem = cm.RawItem;


                    var file = Request.Files[0];

                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/Products"), fileName);
                        file.SaveAs(path);
                        existingItem.PicturePath = fileName;
                        //return path;
                    }

                    id = UpdateItem(existingItem);

                   
                }
                else
                {
                    cm.IsActive = true;
                    cm.NotStatus = "Live";
                    cm.Status = "Live";
                    cm.TotalQuantity = cm.Quantity;
                    cm.HotelId = 1;

                    if (string.IsNullOrEmpty(cm.PicturePath))
                    {
                        cm.PicturePath = "default.png";
                    }

                    var file = Request.Files[0];

                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/Products"), fileName);
                        file.SaveAs(path);
                        cm.PicturePath = fileName;
                        //return path;
                    }

                    id = InsertItem(cm);

                    
                }

                bool saved = true;

                return RedirectToAction("Edit", new { id, saved });
            }

            int catId = cm.CategoryId;
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


            cm.selectList = selectList;

            return View(cm);
        }

        [HttpPost]
        public ActionResult EditB4(ItemModel cm, string[] orderNumbers)
        {
            int id = 0;

            POSService.Entities.StockItem existingBarcode = null;

            if (!string.IsNullOrEmpty(cm.Barcode))
            {
                existingBarcode = POSService.StockItemService.GetStockItems(1).FirstOrDefault(x => x.Barcode == cm.Barcode);

                if (cm.Id == 0)
                {
                    if (existingBarcode != null)
                        ModelState.AddModelError("BarCode", "Please use a different barcode. This barcode exists for a different item");
                }
                else
                {
                    if (existingBarcode != null && existingBarcode.Id != cm.Id)
                        ModelState.AddModelError("BarCode", "Please use a different barcode. This barcode exists for a different item");
                }
            }
            

            if (cm.CategoryId == 0)
            {
                ModelState.AddModelError("CategoryId", "Please select a category");
            }


            var url = string.Empty;

            var extension = string.Empty;

            var imageName = RemoveWhitespace(cm.StockItemName);

            if (orderNumbers != null && orderNumbers.Count() > 0)
            {
                url = orderNumbers.FirstOrDefault();
                extension = Path.GetExtension(url);
                //bool vv = !extension.EndsWith(".JPG", StringComparison.InvariantCultureIgnoreCase);

                if (!extension.EndsWith(".PNG", StringComparison.InvariantCultureIgnoreCase) &&
                    !extension.EndsWith(".GIF", StringComparison.InvariantCultureIgnoreCase)
                    && !extension.EndsWith(".BMP", StringComparison.InvariantCultureIgnoreCase) &&
                    !extension.EndsWith(".JPG", StringComparison.InvariantCultureIgnoreCase)
                    && !extension.EndsWith(".BMP", StringComparison.InvariantCultureIgnoreCase))
                {
                    ModelState.AddModelError("_Form", "Please select a different image, image selected is corrupt.");                   
                }

                cm.PicturePath = imageName + extension;
            }                    


            if (ModelState.IsValid)
            {
                if (cm.Id > 0)
                {
                    var existingItem = StockItemService.GetSpecificItem(cm.Id).FirstOrDefault();

                    cm.IsActive = true;
                    cm.NotStatus = "Live";
                    cm.Status = "Live";
                    cm.TotalQuantity = cm.Quantity;
                    cm.HotelId = 1;

                    if (string.IsNullOrEmpty(cm.Barcode))
                    {
                        cm.Barcode = "";
                    }

                    existingItem.IsActive = true;
                    existingItem.Status = "Live";
                    existingItem.TotalQuantity = cm.Quantity;
                    existingItem.HotelId = 1;

                    id = UpdateItem(existingItem);


                    if (id > 0 && !string.IsNullOrEmpty(url))
                    {
                        Stream imageStream = new WebClient().OpenRead(url);
                        Image img = Image.FromStream(imageStream);
                        var path = Path.Combine(Server.MapPath("~/Products"), imageName + extension);
                        try
                        {
                            img.Save(path);
                        }
                        catch
                        {

                        }

                    }
                }
                else
                {
                    cm.IsActive = true;
                    cm.NotStatus = "Live";
                    cm.Status = "Live";
                    cm.TotalQuantity = cm.Quantity;
                    cm.HotelId = 1;

                    if (string.IsNullOrEmpty(cm.PicturePath))
                    {
                        cm.PicturePath = "default.png";
                    }

                    if (string.IsNullOrEmpty(cm.Barcode))
                    {
                        cm.Barcode = "1111111111111";
                    }

                    cm.Description = cm.StockItemName;

                    id = InsertItem(cm);
                }

                bool saved = true;

                return RedirectToAction("Edit", new { id, saved });
            }

            int catId = cm.CategoryId;
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


            cm.selectList = selectList;

            return View(cm);
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


        public ActionResult DistributeOLd(int? storeId)
        {
            var store = _storeService.GetById(storeId.Value);

            var points = _distributionPointService.GetAll();

            ItemIndexModel cim1 = new ItemIndexModel();

            cim1.StoreId = storeId.Value;

            cim1.DistributionPoints = points.ToList();

            return View(cim1);
        }

        public ActionResult DistributeToPoint(int? id, int? poId)
        {
            var point = _distributionPointService.GetById(id.Value);

            ItemIndexModel cim1 = new ItemIndexModel();
            
            cim1.DistributionPoint = point;

            cim1.StorePointItem = _storePointService.GetById(poId.Value);

            cim1.StorePointId = poId.Value;

            return View(cim1);
        }

        [HttpPost]
        public ActionResult DistributeToPoint(int? id, int? poId, int? dummy)
        {
            //var allRealStock = _stockItemService.GetAll().Where(x => !x.CookedFood).ToList();

            

            var storePointItems = _storePointItemService.GetAll().Where(x => x.StorePointId == poId.Value).ToList();

            var allStoreItemIds = storePointItems.Select(x => x.Id).ToList();

            int totalNumberOfItems = 0;

            bool itemsTransfered = false;

            var thisUser = _personService.GetAllForLogin().Where(x => x.Username.ToUpper().Equals(User.Identity.Name.ToUpper())).FirstOrDefault();

            var batchDate = DateTime.Now;

            foreach (var itemId in allStoreItemIds)
            {
                var name = "StoreItem_" + itemId.ToString();

                //var realStock = allRealStock.FirstOrDefault(x => x.Id == itemId);

                if (Request.Form[name] != null)
                {
                    var qty = 0;

                    int.TryParse(Request.Form[name].ToString(), out qty);

                    var actualStoreItem = _storePointItemService.GetAll().FirstOrDefault(x => x.Id == itemId);

                    if (qty == 0 || actualStoreItem == null)
                        continue;

                    if (actualStoreItem != null)
                    {
                         actualStoreItem.Remaining = actualStoreItem.Remaining - qty;
                        _storePointItemService.Update(actualStoreItem);
                    }

                    totalNumberOfItems++;

                    var existingPOSItem = _pOSItemService.GetAllInclude("StockItem").FirstOrDefault(x => x.DistributionPointId == id.Value && x.ItemId == actualStoreItem.PurchaseOrderItem.ItemId);

                    var previousRemaining = 0;

                    if (existingPOSItem != null)
                    {
                        previousRemaining = existingPOSItem.Remaining;
                        existingPOSItem.Quantity += qty;
                        existingPOSItem.Remaining += qty;
                        existingPOSItem.LastTransfer = qty;

                        if (existingPOSItem.StockItem.RawItem)
                            existingPOSItem.IsActive = false;

                        _pOSItemService.Update(existingPOSItem);

                        itemsTransfered = true;
                    }
                    else
                    {

                        var originalItem = actualStoreItem.PurchaseOrderItem.StockItem;

                        POSItem posItem = new POSItem();
                        posItem.DistributionPointId = id.Value;
                        posItem.ItemId = originalItem.Id;
                        posItem.Quantity = qty;
                        posItem.Remaining = qty;
                        posItem.LastTransfer = qty;
                        posItem.IsActive = true;
                        //posItem.StorePointItemId = itemId;

                        if (originalItem.RawItem)
                            posItem.IsActive = false;

                        _pOSItemService.Create(posItem);

                        itemsTransfered = true;
                    }

                    Batch batch = new Batch {ItemId = actualStoreItem.PurchaseOrderItem.ItemId, BatchDate = batchDate, DistributionPointId = id.Value, QuantityTransferred = qty, StorePointId = poId, PreviousRemaining = previousRemaining };

                    _batchService.Create(batch);
                }
            }

            return RedirectToAction("TransferPO");
        }

        public ActionResult EditTransfer(int? id)
        {
            ItemIndexModel cim1 = new ItemIndexModel();

            var batch = _batchService.GetById(id.Value);

            cim1.PosItemList = batch.StockItem.POSItems.ToList();

            cim1.BatchId = id.Value;

            cim1.StoreId = batch.StorePointId.Value;

            return View(cim1);
        }


        public ActionResult IndexStore()
        {
            var stores = _storeService.GetAll().ToList();
            ItemIndexModel cim1 = new ItemIndexModel { StoreList = stores };           
            return View(cim1);
        }


        //public ActionResult IndexRecievables(int? id)
        //{
        //    var poItems = new List<PurchaseOrder>();
        //    ItemIndexModel cim1 = new ItemIndexModel();

        //    if (id.HasValue)
        //    {
        //        poItems = _storePointService.GetAll().ToList();
        //        cim1 = new ItemIndexModel { StorePointList = poItems };
        //        cim1.CanCreatePO = true;
        //        return View(cim1);
        //    }

        //    poItems = _storePointService.GetAll().OrderByDescending(x => x.OrderDate).ToList();
        //    cim1 = new ItemIndexModel { StorePointList = poItems };
        //    cim1.CanCreatePO = false;
        //    return View(cim1);
        //}


        //public ActionResult IndexForPurchase()
        //{
        //    var poItems = new List<PurchaseOrder>();

        //    ItemIndexModel cim1 = new ItemIndexModel();

        //    var thisUser = _personService.GetAllForLogin().ToList().Where(x => x.Username.ToUpper().Equals(User.Identity.Name)).FirstOrDefault();           

        //    poItems = _storePointService.GetAll().OrderByDescending(x => x.OrderDate).ToList();

        //    cim1 = new ItemIndexModel { StorePointList = poItems };

        //    cim1.CanCreatePO = false;

        //    cim1.ThisUserId = thisUser.PersonID;

        //    return View(cim1);
        //}

        
        public ActionResult IndexStockTaking()
        {

            ItemIndexModel cim1 = new ItemIndexModel();

            var thisUser = _personService.GetAllForLogin().ToList().Where(x => x.Username.ToUpper().Equals(User.Identity.Name)).FirstOrDefault();

            var allStores = _storeService.GetAll().ToList();

            return View(cim1);
        }

        
        public ActionResult OrderStock()
        {

            ItemIndexModel cim1 = new ItemIndexModel();

            var thisUser = _personService.GetAllForLogin().ToList().Where(x => x.Username.ToUpper().Equals(User.Identity.Name)).FirstOrDefault();

            var allStores = _storeService.GetAll().ToList();

            return View(cim1);
        }


        public ActionResult IndexStock(int? id)
        {
           
            ItemIndexModel cim1 = new ItemIndexModel();

            var thisUser = _personService.GetAllForLogin().ToList().Where(x => x.Username.ToUpper().Equals(User.Identity.Name)).FirstOrDefault();

            var allStores = _storeService.GetAll().ToList();

            cim1.StoreId = id.Value;

            if (id.HasValue)
            {
                allStores = allStores.Where(x => x.Id == id.Value).ToList();
                cim1 = new ItemIndexModel { allStores = allStores };
                cim1.CanCreatePO = true;
                cim1.ThisUserId = thisUser.PersonID;
                cim1.StoreId = id.Value;

                return View(cim1);
            }


            cim1 = new ItemIndexModel { allStores = allStores };
            cim1.CanCreatePO = false;
            cim1.ThisUserId = thisUser.PersonID;
            cim1.StoreId = id.Value;

            return View(cim1);
        }

        
        public ActionResult DamagesIndex()
        {

            var points = _distributionPointService.GetAll();

            ItemIndexModel cim1 = new ItemIndexModel();

            cim1.DistributionPoints = points.ToList();

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
                ItemIndexModel cim = new ItemIndexModel { ItemList = items };
                return View("ProductAlerts", cim);
            }

            var items1 = GetAllItemsRaw().Where(x => x.RawItem && !x.IsActive);
            ItemIndexModel cim1 = new ItemIndexModel { RawItemList = items1.ToList() };
            cim1.ReportName = "RawMaterialsItemList";
            cim1.FileToDownloadPath = GenerateExcelItemRawItems(cim1, cim1.ReportName);
            return View(cim1);
        }

        private string GenerateExcelItemRawItems(ItemIndexModel model, string reportName)
        {

            DataTable dt = new DataTable();

            dt.Columns.AddRange(new DataColumn[7] {
                                new DataColumn("No.", typeof(string)),
                                new DataColumn("Name", typeof(string)),
                                new DataColumn("Buying Price", typeof(string)),
                                new DataColumn("Selling Price", typeof(string)),
                                new DataColumn("Notification No.",typeof(string)),
                                new DataColumn("Cooked",typeof(string)),
                                new DataColumn("Kitchen",typeof(string))
            });

            int p = 1;

            foreach (var ru in model.RawItemList.OrderBy(x => x.StockItemName))
            {
                dt.Rows.Add(p, ru.StockItemName, ru.OrigPrice, ru.UnitPrice, ru.NotNumber, ru.CookedFood, ru.KitchenOnly);
                p++;
            }

            var fileName = "_" + reportName;

            var fileNameToUse = fileName + ".xlsx";

            var path = Path.Combine(Server.MapPath("~/Products"), fileNameToUse);

            //Codes for the Closed XML
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt, reportName);
                wb.SaveAs(path);
            }

            return fileName;
        }



        public ActionResult Distribute(int? id)
        {
            var points = _distributionPointService.GetAll();

            ItemIndexModel cim1 = new ItemIndexModel();

            cim1.DistributionPoints = points.ToList();

            cim1.StoreId = id.Value;

            return View(cim1);
        }

        public ActionResult TransferPOEdit()
        {
            //var poItems = new List<PurchaseOrder>();

            ItemIndexModel cim1 = new ItemIndexModel();

            var thisUser = _personService.GetAllForLogin().ToList().Where(x => x.Username.ToUpper().Equals(User.Identity.Name)).FirstOrDefault();

            cim1.BatchPoItems = _batchService.GetAll().Where(x => x.BatchDate > DateTime.Now.AddDays(-3) && x.StockItem.POSItems.Count > 0).ToList();

            cim1.CanCreatePO = false;

            cim1.ThisUserId = thisUser.PersonID;

            return View(cim1);
        }

        public ActionResult TransferPO()
        {
            //var poItems = new List<StorePoint>();

            ItemIndexModel cim1 = new ItemIndexModel();

            var thisUser = _personService.GetAllForLogin().ToList().Where(x => x.Username.ToUpper().Equals(User.Identity.Name)).FirstOrDefault();

            var poItems = _storePointService.GetAll().Where(x => x.StorePointItems.Sum(y => y.Remaining) > 0).OrderByDescending(x => x.Name).ToList();

            cim1 = new ItemIndexModel { StorePointList = poItems };

            cim1.CanCreatePO = false;

            cim1.ThisUserId = thisUser.PersonID;

            return View(cim1);
        }

        
        //public ActionResult IndexReturns()
        //{
        //    var poItems = new List<PurchaseOrder>();

        //    ItemIndexModel cim1 = new ItemIndexModel();

        //    var thisUser = _personService.GetAllForLogin().ToList().Where(x => x.Username.ToUpper().Equals(User.Identity.Name)).FirstOrDefault();

        //    poItems = _storePointService.GetAllByInclude("PurchaseOrderItems,PurchaseOrderItems.StockItem").Where(x => x.GoodsRecieved).OrderByDescending(x => x.OrderDate).ToList();

        //    cim1 = new ItemIndexModel { StorePointList = poItems };

        //    cim1.CanCreatePO = false;

        //    cim1.ThisUserId = thisUser.PersonID;

        //    return View(cim1);
        //}


        //public ActionResult IndexPO()
        //{
        //    var poItems = new List<PurchaseOrder>();

        //    ItemIndexModel cim1 = new ItemIndexModel();

        //    var thisUser = _personService.GetAllForLogin().ToList().Where(x => x.Username.ToUpper().Equals(User.Identity.Name)).FirstOrDefault();

        //    poItems = _storePointService.GetAll().Where(x => string.IsNullOrEmpty(x.Notes)).OrderByDescending(x => x.OrderDate).ToList();

        //    cim1 = new ItemIndexModel { StorePointList = poItems };

        //    cim1.CanCreatePO = false;

        //    cim1.ThisUserId = thisUser.PersonID;

        //    return View(cim1);
        //}



        public ActionResult CompleteStockIndex(int? id)
        {
            if (id.HasValue)
            {
                var items = GetAllItems();
                items = items.ToList();
                ItemIndexModel cim = new ItemIndexModel { ItemList = items };
                return View("ProductAlerts", cim);
            }

            var allStock = _stockItemService.GetAll().Where(x => !x.CookedFood).ToList();
            ItemIndexModel cim1 = new ItemIndexModel { StockList = allStock };
            return View(cim1);
        }


        public ActionResult SalesStockIndex(int? id)
        {
            if (id.HasValue)
            {
                var items = GetAllItems();
                items = items.ToList();
                ItemIndexModel cim = new ItemIndexModel { ItemList = items };
                return View("ProductAlerts", cim);
            }

            var itemsPOS = _pOSItemService.GetAllInclude("StockItem").ToList();
            ItemIndexModel cim1 = new ItemIndexModel { PosItemList = itemsPOS };
            return View(cim1);
        }

        public ActionResult Index(int? id)
        {
            if (id.HasValue)
            {
                var items = GetAllItems();
                items = items.Where(x => x.IsActive).ToList();
                ItemIndexModel cim = new ItemIndexModel { ItemList = items };
                cim.ReportName = "ItemList";
                cim.FileToDownloadPath = GenerateExcelItemExpenses(cim, cim.ReportName);
                return View("ProductAlerts",cim);
            }

            var items1 = GetAllItems();
            ItemIndexModel cim1 = new ItemIndexModel { ItemList = items1.Where(x => x.IsActive).ToList() };
            cim1.ReportName = "ItemList";
            cim1.FileToDownloadPath = GenerateExcelItemExpenses(cim1, cim1.ReportName);
            return View(cim1);
        }

        private string GenerateExcelItemExpenses(ItemIndexModel model, string reportName)
        {
           
            DataTable dt = new DataTable();

            dt.Columns.AddRange(new DataColumn[7] {
                                new DataColumn("No.", typeof(string)),
                                new DataColumn("Name", typeof(string)),
                                new DataColumn("Buying Price", typeof(string)),
                                new DataColumn("Selling Price", typeof(string)),
                                new DataColumn("Notification No.",typeof(string)),
                                new DataColumn("Cooked",typeof(string)),
                                new DataColumn("Kitchen",typeof(string))
            });

            int p = 1;

            foreach (var ru in model.ItemList.OrderBy(x => x.Description))
            {
                dt.Rows.Add(p,ru.Description, ru.OrigPrice, ru.UnitPrice, ru.NotNumber, ru.CookedFood, ru.KitchenOnly);
                p++;
            }

            var fileName = "_" + reportName;

            var fileNameToUse = fileName + ".xlsx";

            var path = Path.Combine(Server.MapPath("~/Products"), fileNameToUse);

            //Codes for the Closed XML
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt, reportName);
                wb.SaveAs(path);
            }

            return fileName;
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

            categories.Insert(0, new CategoryModel {Name = "-- Please Select--", Id = 0 });            

            IEnumerable<SelectListItem> selectList =
                from c in categories
                select new SelectListItem
                {
                    Selected = (c.Id == catId),
                    Text = c.Name,
                    Value = c.Id.ToString()
                };

            ItemModel cm = new ItemModel { RawItemQuantity = 1M, ClubPrice = decimal.Zero, CookedFood = false , Id = 0, selectList = selectList, NotNumber = 100, OrigPrice = 100, Barcode = "", Quantity = 0, TotalQuantity = 0, Description = "Description" };

            return View(cm);
        }

        
        //[HttpPost]
        //public ActionResult CREATEPORECIEVED(PurchaseOrderModel pm, string submitButton)
        //{       
        //    int id = pm.Id;
        //    int storeId = 1;

        //    if (ModelState.IsValid)
        //    {
        //        if (pm.Id > 0)
        //        {
        //            var existingPo = _storePointService.GetById(pm.Id);
                   
        //            bool addToStock = false;

        //            if (submitButton.StartsWith("A"))
        //            {
        //                existingPo.Completed = true;
        //                existingPo.Notes = "ACCEPTED";
        //                addToStock = true;
        //            }                        
        //            else
        //            {
        //                existingPo.InvoiceRaised = false;
                    
        //                existingPo.Notes = "REJECTED, Incorrectc Items count";
        //            }

        //            _storePointService.Update(existingPo);

        //            //if (addToStock)
        //                //AddToStock(existingPo, existingPo.AssignedTo);
        //        }


        //        return RedirectToAction("IndexStore");
        //    }


        //    return RedirectToAction("IndexStore");
        //}

        private void AddToStock(PurchaseOrder existingPo, int thisUserId)
        {
            var today = DateTime.Now;

            foreach(var poi in existingPo.PurchaseOrderItems.Where(x => x.Qty > 0))
            {
                var storeItem = _storeItemService.GetAll().FirstOrDefault(x => x.ItemId == poi.ItemId);
                if(storeItem == null)
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

        
        //[HttpPost]
        //public ActionResult ReassignToStoreManager(PurchaseOrderModel pm)
        //{
        //    int id = pm.Id;

        //    var thisUser = _personService.GetAllForLogin().Where(x => x.Username.ToUpper().Equals(User.Identity.Name.ToUpper())).FirstOrDefault();

        //    if (ModelState.IsValid)
        //    {
        //        if (pm.Id > 0)
        //        {
        //            var existingPo = _storePointService.GetById(pm.Id);
        //            var allExistingPoItems = existingPo.PurchaseOrderItems.ToList();

                   
        //            existingPo.Description = pm.Description;

        //            var allRealStock = _stockItemService.GetAll().Where(x => !x.CookedFood).ToList();

        //            var allStockItemIds = allRealStock.Select(x => x.Id).ToList();

        //            int totalNumberOfItems = 0;

        //            var totalValue = decimal.Zero;

        //            foreach (var itemId in allStockItemIds)
        //            {
        //                var name = "POItem_" + itemId.ToString();

        //                var realStock = allRealStock.FirstOrDefault(x => x.Id == itemId);

        //                if (Request.Form[name] != null)
        //                {
        //                    var qty = 0;
        //                    int.TryParse(Request.Form[name].ToString(), out qty);

        //                    if (qty == 0)
        //                        continue;

        //                    totalNumberOfItems++;

        //                    var existingPoItem = _storePointItemService.GetAll().FirstOrDefault(x => x.ItemId == itemId && x.PurchaseOrderId == existingPo.Id);

        //                    if (existingPoItem != null)
        //                    {
        //                        existingPoItem.Qty = qty;
        //                        var thisValue = (decimal)(realStock.UnitPrice * qty);
        //                        totalValue += thisValue;
        //                        existingPoItem.TotalPrice = thisValue;
        //                        _storePointItemService.Update(existingPoItem);
        //                    }
        //                    else
        //                    {
        //                        var thisValue = (decimal)(realStock.UnitPrice * qty);
        //                        totalValue += thisValue;
        //                        var poItem = new PurchaseOrderItem { PurchaseOrderId = pm.Id, ItemId = itemId, Qty = qty, QtyRecieved = 0, TotalPrice = thisValue };
        //                        _storePointItemService.Create(poItem);
        //                    }
        //                }
        //            }

        //            existingPo.NetValue = totalValue;

        //            existingPo.BaseNetValue = totalValue;
        //            existingPo.InvoiceRaised = true;

        //            existingPo.GoodsBought = true;

        //            existingPo.GoodsRecieved = false;

        //            _storePointService.Update(existingPo);

        //            RaiseInvoice(existingPo, thisUser.PersonID);
                
        //        }


        //        return RedirectToAction("IndexForPurchase");
        //    }

        //    var persons = _personService.GetAllForLogin().Where(x => x.IsActive && x.PersonTypeId != (int)PersonTypeEnum.Guest).ToList();

        //    if (pm.Id == 0)
        //    {
        //        persons = new List<Person>() { thisUser };
        //    }
        //    else
        //    {
        //        persons.Insert(0, new Person { DisplayName = "-- Please Select--", PersonID = 0 });
        //    }

        //    return View(pm);
        //}

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


        //[HttpPost]
        //public ActionResult ReturnPO(int? id)
        //{
        //    int? poId = id;

        //    var allRealStock = _stockItemService.GetAll().Where(x => !x.CookedFood).ToList();

        //    var allStockItemIds = allRealStock.Select(x => x.Id).ToList();

        //    int totalNumberOfItems = 0;

        //    var thisUser = _personService.GetAllForLogin().Where(x => x.Username.ToUpper().Equals(User.Identity.Name.ToUpper())).FirstOrDefault();

        //    decimal returnAmount = decimal.Zero;

        //    foreach (var itemId in allStockItemIds)
        //    {
        //        var name = "StoreItem_" + itemId.ToString();

        //        var realStock = allRealStock.FirstOrDefault(x => x.Id == itemId);

        //        if (Request.Form[name] != null)
        //        {
        //            var qty = 0;
        //            int.TryParse(Request.Form[name].ToString(), out qty);

        //            if (qty == 0)
        //                continue;

        //            var actualPoItem = _storePointItemService.GetAll().FirstOrDefault(x => x.ItemId == itemId && x.PurchaseOrderId == poId.Value);

        //            if (actualPoItem != null)
        //            {
        //                actualPoItem.Qty = actualPoItem.Qty - qty;
        //                //actualPoItem.QtyRecieved = actualPoItem.Qty - qty;
        //                actualPoItem.QtyRecieved = actualPoItem.QtyRecieved - qty;
        //                actualPoItem.Returns = qty;
        //                actualPoItem.ReturnValue = qty * actualPoItem.StockItem.OrigPrice;
        //                _storePointItemService.Update(actualPoItem);
        //                returnAmount += (qty * actualPoItem.StockItem.OrigPrice); 
        //            }

        //            totalNumberOfItems++;
        //        }
        //    }

        //    if(returnAmount > decimal.Zero)
        //    {
        //        var thePo = _storePointService.GetById(poId.Value);

        //        if(thePo != null)
        //        {
        //            thePo.BaseNetValue = thePo.BaseNetValue - returnAmount;
        //            thePo.NetValue = thePo.NetValue - returnAmount;
        //            _storePointService.Update(thePo);
        //        }
        //    }

        //    return RedirectToAction("IndexReturns");
        //}


        [HttpPost]
        public ActionResult EditDistributeToPoint(int? batchId, int? storeId)
        {
            var batch = _batchService.GetById(batchId.Value);

            var allStoreItems = _storePointItemService.GetAll().ToList();

            var allPosItems = _pOSItemService.GetAll().ToList();

            var allItemIds = allPosItems.Select(x => x.Id).ToList();

            foreach (var itemId in allItemIds)
            {
                var newQtyName = "StoreItem_" + itemId.ToString();

                var originalQtyName = "POItem_" + itemId.ToString();

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

                var realPosItem = allPosItems.FirstOrDefault(x => x.Id == itemId && x.DistributionPointId == batch.DistributionPointId);

                var storePointItem = allStoreItems.FirstOrDefault(x => x.PurchaseOrderItem.ItemId  == realPosItem.ItemId);

                if(storePointItem == null || newQty == 0)
                {
                    continue;
                }

                if (realPosItem != null)
                {
                    realPosItem.Quantity -= originalQty;
                    realPosItem.Quantity += newQty;
                    realPosItem.Remaining -= originalQty;
                    realPosItem.Remaining += newQty;

                    realPosItem.LastTransfer = newQty;

                    _pOSItemService.Update(realPosItem);

                    if (storePointItem != null)
                    {
                        storePointItem.Quantity -= originalQty;
                        storePointItem.Quantity += newQty;
                        storePointItem.Remaining += originalQty;
                        storePointItem.Remaining -= newQty;
                        _storePointItemService.Update(storePointItem);
                    }
                }

                
            }

            return RedirectToAction("TransferPOEdit");
        }

        //[HttpPost]
        //public ActionResult DistributeToPoint(int? id, int? poId, int? dummy)
        //{
        //    var allRealStock = _stockItemService.GetAll().Where(x => !x.CookedFood).ToList();

        //    var allStockItemIds = allRealStock.Select(x => x.Id).ToList();

        //    int totalNumberOfItems = 0;

        //    bool itemsTransfered = false;

        //    var thisUser = _personService.GetAllForLogin().Where(x => x.Username.ToUpper().Equals(User.Identity.Name.ToUpper())).FirstOrDefault();

        //    var batchDate = DateTime.Now;

        //    foreach (var itemId in allStockItemIds)
        //    {
        //        var name = "StoreItem_" + itemId.ToString();

        //        var realStock = allRealStock.FirstOrDefault(x => x.Id == itemId);

        //        if (Request.Form[name] != null)
        //        {
        //            var qty = 0;
        //            int.TryParse(Request.Form[name].ToString(), out qty);

        //            if (qty == 0)
        //                continue;

        //            //var poItems = _purchaseOrderService.GetById(poId.Value).PurchaseOrderItems.ToList();
        //            //var actualPoItem = poItems.FirstOrDefault(x => x.ItemId == itemId);

        //            var actualPoItem = _storePointItemService.GetAll().FirstOrDefault(x => x.ItemId == itemId && x.PurchaseOrderId == poId.Value);

        //            if(actualPoItem != null)
        //            {

        //                actualPoItem.Qty = actualPoItem.Qty - qty;

        //                _storePointItemService.Update(actualPoItem);
        //            }

        //            totalNumberOfItems++;

        //            var existingPOSItem = _pOSItemService.GetAllInclude("StockItem").FirstOrDefault(x => x.DistributionPointId == id.Value && x.ItemId == itemId);



        //            if (existingPOSItem != null)
        //            {
        //                existingPOSItem.Quantity += qty;
        //                existingPOSItem.Remaining += qty;
        //                if (existingPOSItem.StockItem.RawItem)
        //                    existingPOSItem.IsActive = false;
        //                _pOSItemService.Update(existingPOSItem);
        //                itemsTransfered = true;

        //                Batch batch = new Batch { POSItemId = existingPOSItem.Id, BatchDate = batchDate, DistributionPointId = id.Value, PurchaseOrderItemId = actualPoItem.Id, QuantityTransferred = qty };
        //                _batchService.Create(batch);
        //            }
        //            else
        //            {
        //                var originalItem = _stockItemService.GetById(itemId);
        //                POSItem posItem = new POSItem();
        //                posItem.DistributionPointId = id.Value;
        //                posItem.ItemId = itemId;
        //                posItem.Quantity = qty;
        //                posItem.Remaining = qty;
        //                posItem.IsActive = true;

        //                if (originalItem.RawItem)
        //                    posItem.IsActive = false;

        //                _pOSItemService.Create(posItem);

        //                itemsTransfered = true;

        //                Batch batch = new Batch { POSItemId = posItem.Id, BatchDate = batchDate, DistributionPointId = id.Value, PurchaseOrderItemId = actualPoItem.Id, QuantityTransferred = qty };
        //                _batchService.Create(batch);
        //            }
        //        }
        //    }

        //    if (itemsTransfered)
        //    {
        //        var po = _storePointService.GetAllByInclude("PurchaseOrderItems").FirstOrDefault(x => x.Id == poId.Value);

        //        var someItemsAreRemaining = po.PurchaseOrderItems.Sum(x => x.Qty) > 0;

        //        if (po != null)
        //        {
        //            if (!someItemsAreRemaining)
        //            {
        //                po.Completed = true;
        //                po.Notes = "Full Transfer Done";
        //            }
        //            else
        //            {
        //                po.Notes = "Some Transfer Done";
        //            }

        //            _storePointService.Update(po);
        //        }
        //    }

        //    return RedirectToAction("IndexPO");
        //}

        //[HttpPost]
        //public ActionResult CreatePO(PurchaseOrderModel pm)
        //{
        //    int id = pm.Id;

        //    var thisUser = _personService.GetAllForLogin().Where(x => x.Username.ToUpper().Equals(User.Identity.Name.ToUpper())).FirstOrDefault();

        //    if (ModelState.IsValid)
        //    {
        //        if(pm.Id > 0)
        //        {
        //            var existingPo = _storePointService.GetById(pm.Id);
        //            var allExistingPoItems = existingPo.PurchaseOrderItems.ToList();

        //            existingPo.OrderDate = pm.OrderDate;
        //            existingPo.SupplierInvoice = pm.SupplierReference;
        //            existingPo.Description = pm.Description;
        //            existingPo.GoodsRecieved = pm.Recieved;
        //            existingPo.IsRawItem = pm.IsRawItem;


        //            var allRealStock = _stockItemService.GetAll().Where(x => !x.CookedFood).ToList();

        //            var allStockItemIds = allRealStock.Select(x => x.Id).ToList();

        //            int totalNumberOfItems = 0;
        //            var totalValue = decimal.Zero;

        //            foreach(var itemId in allStockItemIds)
        //            {
        //                var name = "POItem_" + itemId.ToString();

        //                var realStock = allRealStock.FirstOrDefault(x => x.Id == itemId);

        //                if(Request.Form[name] != null)
        //                {
        //                    var qty = 0;
        //                    int.TryParse(Request.Form[name].ToString(), out qty);

        //                    if(qty == 0)
        //                        continue;

        //                    totalNumberOfItems++;

        //                    var existingPoItem = _storePointItemService.GetAll().FirstOrDefault(x => x.ItemId == itemId && x.PurchaseOrderId == existingPo.Id);

        //                    if(existingPoItem != null)
        //                    {
        //                        existingPoItem.Qty = qty;
        //                        existingPoItem.QtyRecieved = qty;
        //                        var thisValue = (decimal)(realStock.OrigPrice * qty);
        //                        totalValue += thisValue;
        //                        existingPoItem.TotalPrice = thisValue;
        //                        _storePointItemService.Update(existingPoItem);

        //                        if (pm.Recieved)
        //                        {
        //                            realStock.TotalQuantity -= realStock.Quantity;//Remove PreviouslyAdded

        //                            realStock.Quantity = qty;
        //                            realStock.TotalQuantity += qty;
        //                            _stockItemService.Update(realStock);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        var thisValue = (decimal)(realStock.OrigPrice * qty);
        //                        totalValue += thisValue;
        //                        var poItem = new PurchaseOrderItem { PurchaseOrderId = pm.Id, ItemId = itemId, Qty = qty, QtyRecieved = qty, TotalPrice = thisValue };
        //                        _storePointItemService.Create(poItem);

        //                        if(pm.Recieved)
        //                        {
        //                            realStock.Quantity = qty;
        //                            realStock.TotalQuantity += qty;
        //                            _stockItemService.Update(realStock);
        //                        }

        //                    }
        //                }
        //            }

        //            existingPo.NetValue = totalValue;
        //            existingPo.BaseNetValue = totalValue;

        //            var file = Request.Files[0];

        //            if (file != null && file.ContentLength > 0)
        //            {
        //                var fileName = existingPo.Id.ToString() + "_" + Path.GetFileName(file.FileName);
        //                var path = Path.Combine(Server.MapPath("~/Invoices"), fileName);
        //                file.SaveAs(path);
        //                existingPo.InvoicePath = fileName;
        //            }

        //            _storePointService.Update(existingPo);
        //        }
        //        else
        //        {

        //            var po = new PurchaseOrder();
        //            po.Description = pm.Description;
        //            po.OrderDate = DateTime.Now;
        //            po.RaisedBy = thisUser.PersonID;
        //            po.CreatedDate = DateTime.Now;
        //            po.NetValue = decimal.Zero;
        //            po.BaseNetValue = decimal.Zero;
        //            po.IsActive = true;
        //            po.SupplierInvoice = pm.SupplierReference;
        //            po.IsRawItem = pm.IsRawItem;
        //            _storePointService.Create(po);

        //            id = po.Id;

        //        }

        //        return RedirectToAction("POEdit", new { id, saved = true });
        //    }



        //    var persons = _personService.GetAllForLogin().Where(x => x.IsActive && x.PersonTypeId != (int)PersonTypeEnum.Guest).ToList();

        //    if(pm.Id == 0)
        //    {
        //        persons = new List<Person>() { thisUser };
        //    }
        //    else
        //    {
        //        persons.Insert(0, new Person { DisplayName = "-- Please Select--", PersonID = 0 });
        //    }


        //    return View(pm);
        //}

        //CreateOpenTill
        [HttpPost]
        public ActionResult CreateOpenTill(string itemname, decimal? itemamount)
        {
            return RedirectToAction("Index", "POS");

            var existing = GetExistingItem(itemname);
            var cats = GetAllCategories();
            var categories = cats.ToList();

            if (!existing.HasValue && !string.IsNullOrEmpty(itemname) && itemamount.HasValue)
            {
                     ItemModel cm = new ItemModel();
                     cm.StockItemName = itemname;
                     cm.Description = itemname;
                     cm.OrigPrice = 100;
                     cm.UnitPrice = itemamount.Value;
                     cm.Quantity = 10000000;
                     cm.CategoryId = categories.LastOrDefault().Id;
                     cm.NotNumber = 10000;
                     cm.TotalQuantity = 10000000;

                    cm.IsActive = true;
                    cm.NotStatus = "Live";
                    cm.Status = "Live";
                    cm.TotalQuantity = cm.Quantity;
                    cm.HotelId = 1;

                    if(string.IsNullOrEmpty(cm.PicturePath))
                    {
                        cm.PicturePath = "default.png";
                    }

                    cm.Description = cm.StockItemName;
                    //cm.CookedFood = true;
                    
                    var id = InsertItem(cm);

                    if (cm.CookedFood)
                    {
                        var dPoints = _distributionPointService.GetAll().ToList();

                        foreach (var dPoint in dPoints)
                        {
                            if (dPoint != null)
                            {
                                var existingPointOfServiceItem = _pOSItemService.GetByQuery().FirstOrDefault(x => x.DistributionPointId == dPoint.Id && x.ItemId == id);

                                if (existingPointOfServiceItem == null)
                                {
                                    POSItem posItem = new POSItem();
                                    posItem.DistributionPointId = dPoint.Id;
                                    posItem.Invinsible = true;
                                    posItem.IsActive = true;
                                    posItem.ItemId = id;
                                    posItem.Quantity = 10000000;
                                    posItem.Remaining = 10000000;
                                    _pOSItemService.Create(posItem);
                                }
                                else
                                {
                                    existingPointOfServiceItem.Invinsible = true;
                                    existingPointOfServiceItem.Quantity = 10000000;
                                    existingPointOfServiceItem.Remaining = 10000000;
                                    _pOSItemService.Update(existingPointOfServiceItem);
                                }

                            }
                        }

                    }
                    else
                    {
                        var dPoint = _distributionPointService.GetAll().FirstOrDefault(x => x.Name.ToUpper().Contains("K"));

                        var existingPointOfServiceItem = _pOSItemService.GetByQuery().FirstOrDefault(x => x.DistributionPointId == dPoint.Id && x.ItemId == id);

                        if (existingPointOfServiceItem != null)
                        {
                            existingPointOfServiceItem.IsActive = false;
                            _pOSItemService.Update(existingPointOfServiceItem);
                        }
                    }
                }

            return RedirectToAction("Index","POS");
        }

        
        [HttpPost]
        public ActionResult CreateRawMaterials(RawItemModel cm, string[] orderNumbers, string strType)
        {
            int id = 0;

            try
            {
                cm.CategoryId = GetAllCategories().LastOrDefault().Id;
            }
            catch
            {

            }

            if (cm.CategoryId == 0)
            {
                ModelState.AddModelError("CategoryId", "Please select a category");
            }

            var url = string.Empty;

            var extension = string.Empty;

            if (ModelState.IsValid)
            {
                var existingId = GetExistingItem(cm.StockItemName);

                if (existingId.HasValue && cm.Id == 0)
                {
                    var existingItem = StockItemService.GetSpecificItemRaw(cm.Id).FirstOrDefault();

                    cm.IsActive = false;
                    cm.NotStatus = "Live";
                    cm.Status = "Live";
                    cm.TotalQuantity = cm.Quantity;
                    cm.HotelId = 1;

                    if (string.IsNullOrEmpty(cm.Barcode))
                    {
                        cm.Barcode = "";
                    }

                    existingItem.IsActive = false;
                    existingItem.Status = "Live";
                    existingItem.TotalQuantity = cm.Quantity;
                    existingItem.HotelId = 1;
                    existingItem.Barcode = cm.Barcode;
                    existingItem.StockItemName = cm.StockItemName;
                    existingItem.Description = cm.StockItemName;
                    existingItem.UnitPrice = cm.UnitPrice;
                    existingItem.OrigPrice = cm.UnitPrice;
                    existingItem.NotNumber = cm.NotNumber;
                    existingItem.CategoryId = cm.CategoryId;
                    existingItem.RawItem = true;

                    id = UpdateItem(existingItem);
                }
                else if (cm.Id > 0)
                {
                    var existingItem = StockItemService.GetSpecificItemRaw(cm.Id).FirstOrDefault();

                    cm.IsActive = false;
                    cm.NotStatus = "Live";
                    cm.Status = "Live";
                    cm.TotalQuantity = cm.Quantity;
                    cm.HotelId = 1;

                    if (string.IsNullOrEmpty(cm.Barcode))
                    {
                        cm.Barcode = "";
                    }

                    existingItem.IsActive = false;
                    existingItem.Status = "Live";
                    existingItem.TotalQuantity = cm.Quantity;
                    existingItem.HotelId = 1;
                    existingItem.StockItemName = cm.StockItemName;
                    existingItem.Description = cm.Description;
                    existingItem.OrigPrice = cm.UnitPrice;
                    existingItem.UnitPrice = cm.UnitPrice;
                    existingItem.NotNumber = cm.NotNumber;
                    existingItem.CategoryId = cm.CategoryId;
                    existingItem.RawItem = true;
                    id = UpdateItem(existingItem);
                }
                else
                {
                    cm.IsActive = false;
                    cm.NotStatus = "Live";
                    cm.Status = "Live";
                    cm.TotalQuantity = cm.Quantity;
                    cm.HotelId = 1;
                    cm.RawItem = true;
                    cm.OrigPrice = cm.UnitPrice;
                    cm.UnitPrice = cm.UnitPrice;

                    if (string.IsNullOrEmpty(cm.PicturePath))
                    {
                        cm.PicturePath = "default.png";
                    }

                    cm.Description = cm.StockItemName;

                    id = InsertItem(cm);
                }

                bool saved = true;

                return RedirectToAction("EditRawMaterials", new { id, saved });
            }

            int catId = cm.CategoryId;
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


            cm.selectList = selectList;

            return View(cm);
        }


        //C:\ImportMenu\
        [HttpGet]

        public ActionResult ImportItems()
        {
            return View();
        }

        //C:\ImportMenu\
        [HttpPost]
        public ActionResult ImportItems(int? id)
        {
            if (Request.Files[0] != null)
            {
                id = ImportRecords(Request.Files[0]);
            }

            return RedirectToAction("Index");
        }



        [HttpPost]
        public ActionResult Create(ItemModel cm, string[] orderNumbers, string strType)
        {
            int id = 0;


            POSService.Entities.StockItem existingBarcode = null;

            if(!string.IsNullOrEmpty(cm.Barcode))
            {
                existingBarcode = POSService.StockItemService.GetStockItems(1).FirstOrDefault(x => x.Barcode == cm.Barcode);

                if (cm.Id == 0)
                {
                    if (existingBarcode != null)
                        ModelState.AddModelError("BarCode", "Please use a different barcode. This barcode exists for a different item");
                }
                else
                {
                    if (existingBarcode != null && existingBarcode.Id != cm.Id)
                        ModelState.AddModelError("BarCode", "Please use a different barcode. This barcode exists for a different item");
                }
            }
            
            if (cm.CategoryId == 0)
            {
                ModelState.AddModelError("CategoryId", "Please select a category");
            }

            var url = string.Empty;

            var extension = string.Empty;

            var imageName = RemoveWhitespace(cm.StockItemName);

            if (orderNumbers != null && orderNumbers.Count() > 0)
            {
                url = orderNumbers.FirstOrDefault();
                extension = Path.GetExtension(url);
                //bool vv = !extension.EndsWith(".JPG", StringComparison.InvariantCultureIgnoreCase);

                if (!extension.EndsWith(".PNG", StringComparison.InvariantCultureIgnoreCase) &&
                    !extension.EndsWith(".GIF", StringComparison.InvariantCultureIgnoreCase)
                    && !extension.EndsWith(".BMP", StringComparison.InvariantCultureIgnoreCase) &&
                    !extension.EndsWith(".JPG", StringComparison.InvariantCultureIgnoreCase)
                    && !extension.EndsWith(".BMP", StringComparison.InvariantCultureIgnoreCase))
                {
                    ModelState.AddModelError("_Form", "Please select a different image, image selected is corrupt.");
                }

                cm.PicturePath = imageName + extension;
            }                    


            if (ModelState.IsValid)
            {
                var existingId = GetExistingItem(cm.StockItemName);

                if (existingId.HasValue && cm.Id == 0)
                {
                    var existingItem = StockItemService.GetSpecificItem(cm.Id).FirstOrDefault();

                    cm.IsActive = true;
                    cm.NotStatus = "Live";
                    cm.Status = "Live";
                    cm.TotalQuantity = cm.Quantity;
                    cm.HotelId = 1;

                    if (string.IsNullOrEmpty(cm.Barcode))
                    {
                        cm.Barcode = "";
                    }

                    existingItem.IsActive = true;
                    existingItem.Status = "Live";
                    existingItem.TotalQuantity = cm.Quantity;
                    existingItem.HotelId = 1;
                    existingItem.Barcode = cm.Barcode;
                    existingItem.StockItemName = cm.StockItemName;
                    existingItem.Description = cm.StockItemName;
                    existingItem.UnitPrice = cm.UnitPrice;
                    existingItem.OrigPrice = cm.OrigPrice;
                    existingItem.NotNumber = cm.NotNumber;
                    existingItem.CategoryId = cm.CategoryId;
                    existingItem.CookedFood = cm.CookedFood;
                    existingItem.KitchenOnly = cm.KitchenOnly;
                    existingItem.RawItem = cm.RawItem;
                    existingItem.ClubPrice = cm.ClubPrice;

                    if(existingItem.RawItem)
                    {
                        existingItem.UnitPrice = cm.RawItemQuantity;
                    }


                    var file = Request.Files[0];

                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/Products"), fileName);
                        file.SaveAs(path);
                        existingItem.PicturePath = fileName;
                        var newFileName = Path.Combine(Server.MapPath("~/Products/Small"), fileName);
                        ResizeImage(path, newFileName, 120, 120, false);
                        var newFileNameLarge = Path.Combine(Server.MapPath("~/Products/Large"), fileName);
                        ResizeImage(path, newFileNameLarge, 950, 550, true);
                    }

                    id = UpdateItem(existingItem);

                    if (cm.CookedFood)
                    {
                        var dPoints = _distributionPointService.GetAll().ToList();

                        foreach (var dPoint in dPoints)
                        {
                            if (dPoint != null)
                            {
                                var existingPointOfServiceItem = _pOSItemService.GetByQuery().FirstOrDefault(x => x.DistributionPointId == dPoint.Id && x.ItemId == id);

                                if (existingPointOfServiceItem == null)
                                {
                                    POSItem posItem = new POSItem();
                                    posItem.DistributionPointId = dPoint.Id;
                                    posItem.Invinsible = true;
                                    posItem.IsActive = true;
                                    posItem.ItemId = id;
                                    posItem.Quantity = 10000000;
                                    posItem.Remaining = 10000000;
                                    _pOSItemService.Create(posItem);
                                }
                                else
                                {
                                    existingPointOfServiceItem.Invinsible = true;
                                    existingPointOfServiceItem.Quantity = 10000000;
                                    existingPointOfServiceItem.Remaining = 10000000;
                                    _pOSItemService.Update(existingPointOfServiceItem);
                                }

                            }
                        }

                    }
                }
                else if (cm.Id > 0)
                {
                    var existingItem = StockItemService.GetSpecificItem(cm.Id).FirstOrDefault();

                    cm.IsActive = true;
                    cm.NotStatus = "Live";
                    cm.Status = "Live";
                    cm.TotalQuantity = cm.Quantity;
                    cm.HotelId = 1;

                    if (string.IsNullOrEmpty(cm.Barcode))
                    {
                        cm.Barcode = "";
                    }

                    existingItem.IsActive = true;
                    existingItem.Status = "Live";
                    existingItem.TotalQuantity = cm.Quantity;
                    existingItem.HotelId = 1;
                    existingItem.Barcode = cm.Barcode;
                    existingItem.StockItemName = cm.StockItemName;
                    existingItem.Description = cm.Description;
                    existingItem.UnitPrice = cm.UnitPrice;
                    existingItem.OrigPrice = cm.OrigPrice;
                    existingItem.NotNumber = cm.NotNumber;
                    existingItem.CategoryId = cm.CategoryId;
                    existingItem.CookedFood = cm.CookedFood;
                    existingItem.KitchenOnly = cm.KitchenOnly;
                    existingItem.RawItem = cm.RawItem;
                    existingItem.ClubPrice = cm.ClubPrice;

                    if (existingItem.RawItem)
                        existingItem.UnitPrice = cm.RawItemQuantity;


                    var file = Request.Files[0];

                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/Products"), fileName);
                        file.SaveAs(path);
                        existingItem.PicturePath = fileName;
                        var newFileName = Path.Combine(Server.MapPath("~/Products/Small"), fileName);
                        ResizeImage(path, newFileName, 120, 120, false);
                        var newFileNameLarge = Path.Combine(Server.MapPath("~/Products/Large"), fileName);
                        ResizeImage(path, newFileNameLarge, 950, 550, true);
                    }

                    id = UpdateItem(existingItem);

                    if (cm.CookedFood)
                    {
                        var dPoints = _distributionPointService.GetAll().ToList();

                        foreach (var dPoint in dPoints)
                        {
                            if (dPoint != null)
                            {
                                var existingPointOfServiceItem = _pOSItemService.GetByQuery().FirstOrDefault(x => x.DistributionPointId == dPoint.Id && x.ItemId == id);

                                if (existingPointOfServiceItem == null)
                                {
                                    POSItem posItem = new POSItem();
                                    posItem.DistributionPointId = dPoint.Id;
                                    posItem.Invinsible = true;
                                    posItem.IsActive = true;
                                    posItem.ItemId = id;
                                    posItem.Quantity = 10000000;
                                    posItem.Remaining = 10000000;
                                    _pOSItemService.Create(posItem);
                                }
                                else
                                {
                                    existingPointOfServiceItem.Invinsible = true;
                                    existingPointOfServiceItem.Quantity = 10000000;
                                    existingPointOfServiceItem.Remaining = 10000000;
                                    _pOSItemService.Update(existingPointOfServiceItem);
                                }

                            }
                        }

                    }
                    
                }
                else
                {
                    cm.IsActive = true;
                    cm.NotStatus = "Live";
                    cm.Status = "Live";
                    cm.TotalQuantity = cm.Quantity;
                    cm.HotelId = 1;

                    if(string.IsNullOrEmpty(cm.PicturePath))
                    {
                        cm.PicturePath = "default.png";
                    }

                    var file = Request.Files[0];

                    if (file != null && file.ContentLength > 0)
                    {
                        //var fileName = Path.GetFileName(file.FileName);
                        //var path = Path.Combine(Server.MapPath("~/Products"), fileName);
                        //file.SaveAs(path);
                        //cm.PicturePath = fileName;
                        //var newFileName = Path.Combine(Server.MapPath("~/Products/Small"), fileName);
                        //Image image = Image.FromFile(path);
                        //Image thumb = image.GetThumbnailImage(125, 125, () => false, IntPtr.Zero);
                        //thumb.Save(newFileName);


                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/Products"), fileName);
                        file.SaveAs(path);
                        cm.PicturePath = fileName;
                        var newFileName = Path.Combine(Server.MapPath("~/Products/Small"), fileName);
                        ResizeImage(path, newFileName, 120, 120, false);
                        var newFileNameLarge = Path.Combine(Server.MapPath("~/Products/Large"), fileName);
                        ResizeImage(path, newFileNameLarge, 950, 550, true);
                    }


                    cm.Description = cm.StockItemName;

                    if(cm.ClubPrice > decimal.Zero)
                    {
                        cm.ClubPrice = cm.ClubPrice;
                    }
                    else
                    {
                        cm.ClubPrice = cm.UnitPrice;
                    }

                    if (cm.CookedFood)
                    {
                        cm.Quantity = 10000000;
                        cm.TotalQuantity = cm.Quantity;
                    }

                    if (cm.RawItem)
                    {
                        cm.UnitPrice = cm.RawItemQuantity;
                    }
                    
                    id = InsertItem(cm);

                    if (cm.CookedFood)
                    {
                        var dPoints = _distributionPointService.GetAll().ToList();

                        foreach (var dPoint in dPoints)
                        {
                            if (dPoint != null)
                            {
                                var existingPointOfServiceItem = _pOSItemService.GetByQuery().FirstOrDefault(x => x.DistributionPointId == dPoint.Id && x.ItemId == id);

                                if (existingPointOfServiceItem == null)
                                {
                                    POSItem posItem = new POSItem();
                                    posItem.DistributionPointId = dPoint.Id;
                                    posItem.Invinsible = true;
                                    posItem.IsActive = true;
                                    posItem.ItemId = id;
                                    posItem.Quantity = 10000000;
                                    posItem.Remaining = 10000000;
                                    _pOSItemService.Create(posItem);
                                }
                                else
                                {
                                    existingPointOfServiceItem.Invinsible = true;
                                    existingPointOfServiceItem.Quantity = 10000000;
                                    existingPointOfServiceItem.Remaining = 10000000;
                                    _pOSItemService.Update(existingPointOfServiceItem);
                                }

                            }
                        }

                    }
                }

                bool saved = true;

                return RedirectToAction("Edit", new { id, saved });
            }

            int catId = cm.CategoryId;
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


            cm.selectList = selectList;

            return View(cm);            
        }

        public void ResizeImage(string OriginalFile, string NewFile, int NewWidth, int MaxHeight, bool OnlyResizeIfWider)
        {
            System.Drawing.Image FullsizeImage = System.Drawing.Image.FromFile(OriginalFile);

            // Prevent using images internal thumbnail
            FullsizeImage.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);
            FullsizeImage.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);

            if (OnlyResizeIfWider)
            {
                if (FullsizeImage.Width <= NewWidth)
                {
                    NewWidth = FullsizeImage.Width;
                }
            }

            int NewHeight = FullsizeImage.Height * NewWidth / FullsizeImage.Width;
            if (NewHeight > MaxHeight)
            {
                // Resize with height instead
                NewWidth = FullsizeImage.Width * MaxHeight / FullsizeImage.Height;
                NewHeight = MaxHeight;
            }

            System.Drawing.Image NewImage = FullsizeImage.GetThumbnailImage(NewWidth, NewHeight, null, IntPtr.Zero);

            // Clear handle to original file so that we can overwrite it if necessary
            FullsizeImage.Dispose();

            // Save resized picture
            NewImage.Save(NewFile);
        }

        private void ProcessDirectory(string targetDirectory)
        {
            string[] fileEntries = Directory.GetFiles(targetDirectory);

            foreach (string fileName in fileEntries)
            {
                string strFilename = fileName;
                StoreItem(strFilename);
                int p = 90;
            }
        }

        private void StoreItem(string strFilename)
        {
            string itemname = strFilename.ToUpper().Replace(".JPG", "").Replace(@"C:\AYOFOODITEMS\","");

            var stockItem = new ItemModel
            {
                Barcode = "",
                CategoryId = 120,
                CookedFood = true,
                Description = itemname,
                HotelId = 1,
                IsActive = true,
                KitchenOnly = true,
                RawItem = false,
                NotNumber = 100,
                OrigPrice = 190,
                Price = 2500,
                StockItemName = itemname,
                Quantity = 0,
                TotalQuantity = 0,
                UnitPrice = 250,
                PicturePath = itemname + ".JPG",
                Status = "Live"
            };

            PassStockModel(stockItem);
        }

        private int InsertCategory(CategoryModel cm)
        {
            int id = 0;

            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("categoryInsert", myConnection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    myConnection.Open();

                    cmd.Parameters.AddWithValue("@Name", cm.Name.Trim().ToUpper());
                    cmd.Parameters.AddWithValue("@Description", cm.Description.Trim().ToUpper());
                    cmd.Parameters.AddWithValue("@IsActive", cm.IsActive);

                    try
                    {
                        int.TryParse(cmd.ExecuteScalar().ToString(), out id);
                    }
                    catch
                    {

                    }
                }
            }

            return id;
        }

        private int UpdateCategory(CategoryModel cm)
        {
            int id = 0;

            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("categoryUpdate", myConnection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    myConnection.Open();

                    //SqlParameter custId = cmd.Parameters.AddWithValue("@CustomerId", 10);

                    cmd.Parameters.AddWithValue("@Name", cm.Name);
                    cmd.Parameters.AddWithValue("@Description", cm.Description);
                    cmd.Parameters.AddWithValue("@IsActive", cm.IsActive);
                    cmd.Parameters.AddWithValue("@Id", cm.Id);

                    try
                    {
                        int.TryParse(cmd.ExecuteScalar().ToString(), out id);
                    }
                    catch
                    {

                    }
                }
            }

            return id;
        }

        private int ImportRecords(HttpPostedFileBase file)
        {
            //C:\ImportMenu\
            var excelImporter = new ExcelImporter();

            int id = 0;


            var dataSet = excelImporter.GetDataSet(file.InputStream, file.FileName);
            //var course = courseService.GetCourseQuick(id.Value);

            if (dataSet != null)
            {

                var dt = dataSet.Tables[0];
                var totalColunmnCount = dt.Columns.Count;
                int totalrows = 3000;
                int i = 1;//start from 1st row
                bool noMoreLines = false;

                var catId = 1;

                for (int p = i; p < totalrows; p++)
                {
                    if (noMoreLines)
                        break;

                    try
                    {
                        var dataRow = dt.Rows[p]; //Get Data Columns
                        var itemname = dataRow[0].ToString();
                        var price = decimal.Zero;
                        var cookedFood = 0;
                        var rawItem = 0;
                        var kitchenOnly = 0;
                       
                        var glassPrice = decimal.Zero;

                        decimal.TryParse(dataRow[1].ToString(), out price);

                        if(string.IsNullOrEmpty(itemname))
                        {
                            break;
                        }

                        if (price == decimal.Zero)
                        {
                            var newCategory = new CategoryModel { Description = itemname, Name = itemname, IsActive = true };

                            var existingCategory = GetAllCategories().FirstOrDefault(x => x.Name.ToUpper().Trim().Equals(itemname.ToUpper().Trim()));

                            if(existingCategory == null)
                            {
                                catId = InsertCategory(newCategory);
                            }
                            
                        }
                        else
                        {
                            int.TryParse(dataRow[2].ToString(), out cookedFood);
                            decimal.TryParse(dataRow[3].ToString(), out glassPrice);

                            var originalPrice = 1000;
                            kitchenOnly = cookedFood;
                            var notNumber = 100;

                            var stockItem = new ItemModel
                            {
                                Barcode = "",
                                CategoryId = catId,
                                CookedFood = (cookedFood == 1 ? true : false),
                                Description = itemname.ToUpper().Trim(),
                                HotelId = 1,
                                IsActive = true,
                                KitchenOnly = (kitchenOnly == 1 ? true : false),
                                RawItem = (rawItem == 1 ? true : false),
                                NotNumber = notNumber,
                                OrigPrice = originalPrice,
                                Price = price,
                                StockItemName = itemname.ToUpper().Trim(),
                                Quantity = 0,
                                TotalQuantity = 0,
                                UnitPrice = price,
                                Status = "Live"
                            };

                            PassStockModel(stockItem);

                            if (glassPrice > 0)
                            {
                                stockItem.Description = stockItem.Description + " GLASS";
                                stockItem.StockItemName = stockItem.StockItemName + " GLASS";
                                stockItem.UnitPrice = glassPrice;
                                stockItem.Price = glassPrice;
                                PassStockModel(stockItem);
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        noMoreLines = true;
                    }
                }
            }

            return id;
        }

        

        private void PassStockModel(ItemModel cm)
        {   
                var existingId = GetExistingItem(cm.StockItemName);

                int id = 0;

                if (existingId.HasValue && cm.Id == 0)
                {
                    var existingItem = StockItemService.GetSpecificItem(cm.Id).FirstOrDefault();

                    cm.IsActive = true;
                    cm.NotStatus = "Live";
                    cm.Status = "Live";
                    cm.TotalQuantity = cm.Quantity;
                    cm.HotelId = 1;

                    if (string.IsNullOrEmpty(cm.Barcode))
                    {
                        cm.Barcode = "";
                    }

                    existingItem.IsActive = true;
                    existingItem.Status = "Live";
                    existingItem.TotalQuantity = cm.Quantity;
                    existingItem.HotelId = 1;
                    existingItem.Barcode = cm.Barcode;
                    existingItem.StockItemName = cm.StockItemName;
                    existingItem.Description = cm.StockItemName;
                    existingItem.UnitPrice = cm.UnitPrice;
                    existingItem.OrigPrice = cm.OrigPrice;
                    existingItem.NotNumber = cm.NotNumber;
                    existingItem.CategoryId = cm.CategoryId;
                    existingItem.CookedFood = cm.CookedFood;
                    existingItem.KitchenOnly = cm.KitchenOnly;
                    existingItem.RawItem = cm.RawItem;
                    existingItem.PicturePath = cm.PicturePath;

                    id = UpdateItem(existingItem);

                    if (cm.CookedFood)
                    {
                        var dPoints = _distributionPointService.GetAll().ToList();

                        foreach (var dPoint in dPoints)
                        {
                            if (dPoint != null)
                            {
                                var existingPointOfServiceItem = _pOSItemService.GetByQuery().FirstOrDefault(x => x.DistributionPointId == dPoint.Id && x.ItemId == id);

                                if (existingPointOfServiceItem == null)
                                {
                                    POSItem posItem = new POSItem();
                                    posItem.DistributionPointId = dPoint.Id;
                                    posItem.Invinsible = true;
                                    posItem.IsActive = true;
                                    posItem.ItemId = id;
                                    posItem.Quantity = 10000000;
                                    posItem.Remaining = 10000000;
                                    _pOSItemService.Create(posItem);
                                }
                                else
                                {
                                    existingPointOfServiceItem.Invinsible = true;
                                    existingPointOfServiceItem.Quantity = 10000000;
                                    existingPointOfServiceItem.Remaining = 10000000;
                                    _pOSItemService.Update(existingPointOfServiceItem);
                                }

                            }
                        }

                    }
                }
                else if (cm.Id > 0)
                {
                    var existingItem = StockItemService.GetSpecificItem(cm.Id).FirstOrDefault();

                    cm.IsActive = true;
                    cm.NotStatus = "Live";
                    cm.Status = "Live";
                    cm.TotalQuantity = cm.Quantity;
                    cm.HotelId = 1;

                    if (string.IsNullOrEmpty(cm.Barcode))
                    {
                        cm.Barcode = "";
                    }

                    existingItem.IsActive = true;
                    existingItem.Status = "Live";
                    existingItem.TotalQuantity = cm.Quantity;
                    existingItem.HotelId = 1;
                    existingItem.Barcode = cm.Barcode;
                    existingItem.StockItemName = cm.StockItemName;
                    existingItem.Description = cm.Description;
                    existingItem.UnitPrice = cm.UnitPrice;
                    existingItem.OrigPrice = cm.OrigPrice;
                    existingItem.NotNumber = cm.NotNumber;
                    existingItem.CategoryId = cm.CategoryId;
                    existingItem.CookedFood = cm.CookedFood;
                    existingItem.KitchenOnly = cm.KitchenOnly;
                    existingItem.RawItem = cm.RawItem;
                    existingItem.PicturePath = cm.PicturePath;

                    id = UpdateItem(existingItem);

                    if (cm.CookedFood)
                    {
                        var dPoints = _distributionPointService.GetAll().ToList();

                        foreach (var dPoint in dPoints)
                        {
                            if (dPoint != null)
                            {
                                var existingPointOfServiceItem = _pOSItemService.GetByQuery().FirstOrDefault(x => x.DistributionPointId == dPoint.Id && x.ItemId == id);

                                if (existingPointOfServiceItem == null)
                                {
                                    POSItem posItem = new POSItem();
                                    posItem.DistributionPointId = dPoint.Id;
                                    posItem.Invinsible = true;
                                    posItem.IsActive = true;
                                    posItem.ItemId = id;
                                    posItem.Quantity = 10000000;
                                    posItem.Remaining = 10000000;
                                    _pOSItemService.Create(posItem);
                                }
                                else
                                {
                                    existingPointOfServiceItem.Invinsible = true;
                                    existingPointOfServiceItem.Quantity = 10000000;
                                    existingPointOfServiceItem.Remaining = 10000000;
                                    _pOSItemService.Update(existingPointOfServiceItem);
                                }

                            }
                        }

                    }
                   
                }
                else
                {
                    cm.IsActive = true;
                    cm.NotStatus = "Live";
                    cm.Status = "Live";
                    cm.TotalQuantity = cm.Quantity;
                    cm.HotelId = 1;

                    if (string.IsNullOrEmpty(cm.PicturePath))
                    {
                        cm.PicturePath = "default.png";
                    }

                    cm.Description = cm.StockItemName;
                    cm.PicturePath = cm.PicturePath;
                    
                    //existingItem.PicturePath = cm.PicturePath;


                    if (cm.CookedFood)
                    {
                        cm.Quantity = 10000000;
                        cm.TotalQuantity = cm.Quantity;
                    }

                    id = InsertItem(cm);

                    if (cm.CookedFood)
                    {
                        var dPoints = _distributionPointService.GetAll().ToList();

                        foreach (var dPoint in dPoints)
                        {
                            if (dPoint != null)
                            {
                                var existingPointOfServiceItem = _pOSItemService.GetByQuery().FirstOrDefault(x => x.DistributionPointId == dPoint.Id && x.ItemId == id);

                                if (existingPointOfServiceItem == null)
                                {
                                    POSItem posItem = new POSItem();
                                    posItem.DistributionPointId = dPoint.Id;
                                    posItem.Invinsible = true;
                                    posItem.IsActive = true;
                                    posItem.ItemId = id;
                                    posItem.Quantity = 10000000;
                                    posItem.Remaining = 10000000;
                                    _pOSItemService.Create(posItem);
                                }
                                else
                                {
                                    existingPointOfServiceItem.Invinsible = true;
                                    existingPointOfServiceItem.Quantity = 10000000;
                                    existingPointOfServiceItem.Remaining = 10000000;
                                    _pOSItemService.Update(existingPointOfServiceItem);
                                }
                            }
                        }

                    }
                }
        }

        public static string RemoveWhitespace(string input)
        {
            StringBuilder output = new StringBuilder(input.Length);

            for (int index = 0; index < input.Length; index++)
            {
                if (!Char.IsWhiteSpace(input, index))
                {
                    output.Append(input[index]);
                }
            }

            return output.ToString();
        }
         

        public int? GetExistingItem(string name)
        {
            int itemId = 0;

            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Select Id FROM STOCKITEM WHERE StockItemName = '" + name + "'", myConnection))
                {
                    //cmd.CommandType = CommandType.StoredProcedure;
                    myConnection.Open();
                    try
                    {
                        int.TryParse(cmd.ExecuteScalar().ToString(), out itemId);
                    }
                    catch
                    {

                    }

                }
            }

            if (itemId > 0)
                return itemId;
            else
                return null;
        }

        private int InsertItem(RawItemModel cm)
        {
            int id = 0;

            if (string.IsNullOrEmpty(cm.Barcode))
            {
                var ticks = (int)DateTime.Now.Ticks;
                cm.Barcode = ticks.ToString();
            }


            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("stockItemInsert", myConnection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    myConnection.Open();

                    cmd.Parameters.AddWithValue("@UnitPrice", cm.UnitPrice);
                    cmd.Parameters.AddWithValue("@ClubPrice", cm.ClubPrice);
                    cmd.Parameters.AddWithValue("@Description", cm.Description);
                    cmd.Parameters.AddWithValue("@PicturePath", cm.PicturePath);
                    cmd.Parameters.AddWithValue("@IsActive", cm.IsActive);
                    cmd.Parameters.AddWithValue("@Status", cm.Status);
                    cmd.Parameters.AddWithValue("@Quantity", cm.Quantity);
                    cmd.Parameters.AddWithValue("@CategoryId", cm.CategoryId);
                    cmd.Parameters.AddWithValue("@OrigPrice", cm.OrigPrice);
                    cmd.Parameters.AddWithValue("@NotNumber", cm.NotNumber);
                    cmd.Parameters.AddWithValue("@NotStatus", cm.NotStatus);
                    cmd.Parameters.AddWithValue("@StockItemName", cm.StockItemName);
                    cmd.Parameters.AddWithValue("@TotalQuantity", cm.TotalQuantity);
                    cmd.Parameters.AddWithValue("@Barcode", cm.Barcode);
                    cmd.Parameters.AddWithValue("@DISCOUNT", "NONE");
                    cmd.Parameters.AddWithValue("@OrderStatus", "NONE");
                    cmd.Parameters.AddWithValue("@DISCOUNTSTARTDATE", DateTime.Now);
                    cmd.Parameters.AddWithValue("@DISCOUNTENDDATE", DateTime.Now);
                    cmd.Parameters.AddWithValue("@DISCOUNTEDPERCENTAGE", decimal.Zero);
                    cmd.Parameters.AddWithValue("@HotelId", 1);
                    cmd.Parameters.AddWithValue("@CookedFood", cm.CookedFood);
                    cmd.Parameters.AddWithValue("@KitchenOnly", cm.KitchenOnly);
                    cmd.Parameters.AddWithValue("@StarBuy", false);
                    cmd.Parameters.AddWithValue("@RawItem", cm.RawItem);
                    cmd.Parameters.AddWithValue("@Discounted", false);

                    try
                    {
                        //cmd.ExecuteScalar().ToString();
                        int.TryParse(cmd.ExecuteScalar().ToString(), out id);
                    }
                    catch (Exception ex)
                    {
                        int p = 90;
                    }
                }
            }

            return id;
        }

        private int InsertItem(ItemModel cm)
        {
            int id = 0;

            if(string.IsNullOrEmpty(cm.Barcode))
            {
                var ticks = (int)DateTime.Now.Ticks;
                cm.Barcode = ticks.ToString();
            }
            

            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("stockItemInsert", myConnection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    myConnection.Open();                    

                    cmd.Parameters.AddWithValue("@UnitPrice", cm.UnitPrice);
                    cmd.Parameters.AddWithValue("@ClubPrice", cm.ClubPrice);
                    cmd.Parameters.AddWithValue("@Description", cm.Description);
                    cmd.Parameters.AddWithValue("@PicturePath", cm.PicturePath);
                    cmd.Parameters.AddWithValue("@IsActive", cm.IsActive);
                    cmd.Parameters.AddWithValue("@Status", cm.Status);
                    cmd.Parameters.AddWithValue("@Quantity", cm.Quantity);
                    cmd.Parameters.AddWithValue("@CategoryId", cm.CategoryId);
                    cmd.Parameters.AddWithValue("@OrigPrice", cm.OrigPrice);
                    cmd.Parameters.AddWithValue("@NotNumber", cm.NotNumber);
                    cmd.Parameters.AddWithValue("@NotStatus", cm.NotStatus);
                    cmd.Parameters.AddWithValue("@StockItemName", cm.StockItemName);
                    cmd.Parameters.AddWithValue("@TotalQuantity", cm.TotalQuantity);
                    cmd.Parameters.AddWithValue("@Barcode", cm.Barcode);
                    cmd.Parameters.AddWithValue("@DISCOUNT", "NONE");
                    cmd.Parameters.AddWithValue("@OrderStatus", "NONE");
                    cmd.Parameters.AddWithValue("@DISCOUNTSTARTDATE", DateTime.Now);
                    cmd.Parameters.AddWithValue("@DISCOUNTENDDATE", DateTime.Now);
                    cmd.Parameters.AddWithValue("@DISCOUNTEDPERCENTAGE", decimal.Zero);
                    cmd.Parameters.AddWithValue("@HotelId",1);
                    cmd.Parameters.AddWithValue("@CookedFood", cm.CookedFood);
                    cmd.Parameters.AddWithValue("@KitchenOnly", cm.KitchenOnly);
                    cmd.Parameters.AddWithValue("@StarBuy", false);
                    cmd.Parameters.AddWithValue("@RawItem", cm.RawItem);
                    cmd.Parameters.AddWithValue("@Discounted", false);



                    try
                    {
                        //cmd.ExecuteScalar().ToString();
                        int.TryParse(cmd.ExecuteScalar().ToString(), out id);
                    }
                    catch(Exception ex)
                    {
                        int p = 90;
                    }
                }
            }

            return id;
        }

        private int UpdateItem(POSService.Entities.StockItem cm)
        {
            int id = 0;

            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("stockItemUpdate", myConnection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    myConnection.Open();

                    //SqlParameter custId = cmd.Parameters.AddWithValue("@CustomerId", 10);
                    cmd.Parameters.AddWithValue("@Id", cm.Id);
                    cmd.Parameters.AddWithValue("@UnitPrice", cm.UnitPrice);
                    cmd.Parameters.AddWithValue("@ClubPrice", cm.ClubPrice);
                    cmd.Parameters.AddWithValue("@Description", cm.Description);
                    cmd.Parameters.AddWithValue("@PicturePath", cm.PicturePath);
                    cmd.Parameters.AddWithValue("@IsActive", cm.IsActive);
                    cmd.Parameters.AddWithValue("@Status", cm.Status);
                    cmd.Parameters.AddWithValue("@Quantity", cm.Quantity);
                    cmd.Parameters.AddWithValue("@CategoryId", cm.CategoryId);
                    cmd.Parameters.AddWithValue("@OrigPrice", cm.OrigPrice);
                    cmd.Parameters.AddWithValue("@NotNumber", cm.NotNumber);
                    cmd.Parameters.AddWithValue("@NotStatus", "LIVE");
                    cmd.Parameters.AddWithValue("@StockItemName", cm.StockItemName);
                    cmd.Parameters.AddWithValue("@TotalQuantity", cm.TotalQuantity);
                    cmd.Parameters.AddWithValue("@Barcode", cm.Barcode);
                    cmd.Parameters.AddWithValue("@DISCOUNT", "NONE");
                    cmd.Parameters.AddWithValue("@OrderStatus", "NONE");
                    cmd.Parameters.AddWithValue("@DISCOUNTSTARTDATE", DateTime.Now);
                    cmd.Parameters.AddWithValue("@DISCOUNTENDDATE", DateTime.Now);
                    cmd.Parameters.AddWithValue("@DISCOUNTEDPERCENTAGE", decimal.Zero);
                    cmd.Parameters.AddWithValue("@HotelId", 1);
                    cmd.Parameters.AddWithValue("@CookedFood", cm.CookedFood);
                    cmd.Parameters.AddWithValue("@KitchenOnly", cm.KitchenOnly);
                    cmd.Parameters.AddWithValue("@RawItem", cm.RawItem);

                    try
                    {
                        int.TryParse(cmd.ExecuteScalar().ToString(), out id);
                    }
                    catch(Exception ex)
                    {
                        int p = 90;
                    }
                }
            }

            return id;
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}