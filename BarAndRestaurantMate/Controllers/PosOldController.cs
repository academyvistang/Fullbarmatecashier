using HotelMateWeb.Dal.DataCore;
using HotelMateWeb.Services.Core;
using HotelMateWeb.Services.ServiceApi;
using BarAndRestaurantMate.Helpers;
using BarAndRestaurantMate.Helpers.Enums;
using BarAndRestaurantMate.Models;
using POSService;
using POSService.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.PointOfService;
using System.Web.Security;
using System.Data.SqlClient;


namespace BarAndRestaurantMate.Controllers
{
    [HandleError(View = "CustomErrorView")]
    public class PosOldController : Controller
    {
        private IEnumerable<POSService.Entities.StockItem> _products;
        private IEnumerable<Category> _categories;
        private IEnumerable<POSService.Entities.Guest> _guests;
        private IEnumerable<POSService.Entities.BusinessAccount> _businessAccount;

        private IEnumerable<Person> _cashiers;

        private  IGuestService _guestService;
        private  IPersonService _personService;
        private  IPOSItemService _posItemService;
        //private  int _distributionPointId = 5;
        private  ITableItemService _tableItemService;
        private  IGuestOrderItemService _guestOrderItemService;
        private  IGuestOrderService _guestOrderService;
        private  ISoldItemService _soldItemService;
        private  IBarTableService _barTableService;
        private  IGuestMessageService _guestMessageService;
        private  ISalesDiscountService _salesDiscountService;
        private  IBusinessAccountCorporateService _businessAccountCorporateService;
        private  ISuspendItemService _suspendItemService;
        private IPrinterTableService _printerTableService;



        public PosOldController()
        {
            _guestService = new GuestService();
            _personService = new PersonService();
            _posItemService = new POSItemService();
            _tableItemService = new TableItemService();
            _soldItemService = new SoldItemService();
            _guestOrderService = new GuestOrderService();
            _guestOrderItemService = new GuestOrderItemService();
            _barTableService = new BarTableService();
            _guestMessageService = new GuestMessageService();
            _salesDiscountService = new SalesDiscountService();
            _businessAccountCorporateService = new BusinessCorporateAccountService();
            _suspendItemService = new SuspendItemService();
            _printerTableService = new PrinterTableService();
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing && _printerTableService != null)
            {
                _printerTableService.Dispose();
                _printerTableService = null;
            }

            if (disposing && _businessAccountCorporateService != null)
            {
                _businessAccountCorporateService.Dispose();
                _businessAccountCorporateService = null;
            }

            if (disposing && _salesDiscountService != null)
            {
                _salesDiscountService.Dispose();
                _salesDiscountService = null;
            }

            if (disposing && _guestMessageService != null)
            {
                _guestMessageService.Dispose();
                _guestMessageService = null;
            }

            if (disposing && _guestService != null)
            {
                _guestService.Dispose();
                _guestService = null;
            }

            if (disposing && _personService != null)
            {
                _personService.Dispose();
                _personService = null;
            }

            if (disposing && _posItemService != null)
            {
                _posItemService.Dispose();
                _posItemService = null;
            }

            if (disposing && _guestOrderItemService != null)
            {
                _guestOrderItemService.Dispose();
                _guestOrderItemService = null;
            }

            if (disposing && _guestOrderService != null)
            {
                _guestOrderService.Dispose();
                _guestOrderService = null;
            }

            if (disposing && _barTableService != null)
            {
                _barTableService.Dispose();
                _barTableService = null;
            }

            

            base.Dispose(disposing);
        }

        private int? _person;
        private int? Person
        {
            get { return _person ?? GetPersonId(); }
            set { _person = value; }
        }

        private int? GetPersonId()
        {
            

            HttpCookie cookie = null;

            try
            {
                cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            }
            catch
            {

            }
            

            if (null == cookie)
            {
                if (_personService == null)
                    _personService = new PersonService();

                var who = User.Identity.Name;

                var user = _personService.GetAllForLogin().FirstOrDefault(x => x.Username.Equals(who, StringComparison.CurrentCultureIgnoreCase));
                return user.PersonID;
            }
               

            var decrypted = FormsAuthentication.Decrypt(cookie.Value);

            if (!string.IsNullOrEmpty(decrypted.UserData))
                return int.Parse(decrypted.UserData);

            return null;
        }

        private int? CashierDistributionPointId()
        {
            var user = _personService.GetById(GetPersonId().Value);
            return user.DistributionPointId;
        }

        private string CashierDistributionPointName()
        {
            var user = _personService.GetById(GetPersonId().Value);

            if(user.DistributionPoint != null)
            {
                return user.DistributionPoint.Name;
            }

            return string.Empty;
        }


        //private Person GetPerson()
        //{
        //    var username = User.Identity.Name;
        //    var user = _personService.GetAllForLogin().FirstOrDefault(x => x.Username.Equals(username, StringComparison.CurrentCultureIgnoreCase));
        //    return user;
        //}

        private int? _hotelId;
        private int HotelID
        {
            get { return _hotelId ?? 1; }
            set { _hotelId = value; }
        }


        public IEnumerable<POSService.Entities.StockItem> ProductsList
        {
            get
            {
                if (_products != null)
                    return _products;
                else
                {
                    _products = StockItemService.GetStockItems(1);
                    return _products;
                }
            }
            set
            {
                _products = StockItemService.GetStockItems(1);
            }
        }

        public IEnumerable<Person> CashiersList
        {
            get
            {
                if (_cashiers != null)
                    return _cashiers;
                else
                {
                    _cashiers = _personService.GetAllForLogin().Where(x => x.IsActive && (x.PersonTypeId == (int)PersonTypeEnum.SalesAssistant || x.PersonTypeId == (int)PersonTypeEnum.Cashier)).ToList(); //SAles Assistant
                    return _cashiers;
                }
            }
            set
            {
                //_cashiers = _personService.GetAllForLogin().Where(x => x.IsActive && x.PersonTypeId == (int)PersonTypeEnum.SalesAssistant).ToList(); //SAles Assistant
                _cashiers = _personService.GetAllForLogin().Where(x => x.IsActive && (x.PersonTypeId == (int)PersonTypeEnum.SalesAssistant || x.PersonTypeId == (int)PersonTypeEnum.Cashier)).ToList(); //SAles Assistant
                   
            }
        }

        public IEnumerable<POSService.Entities.BusinessAccount> BusinessAccountList
        {
            get
            {
                if (_businessAccount != null)
                    return _businessAccount;
                else
                {
                    _businessAccount = StockItemService.GetCurrentAccounts(1);
                    return _businessAccount;
                }
            }
            set
            {
                _businessAccount = StockItemService.GetCurrentAccounts(1);
            }
        }

        public IEnumerable<POSService.Entities.Guest> GuestsList
        {
            get
            {
                if (_guests != null)
                    return _guests;
                else
                {
                    _guests = StockItemService.GetCurrentGuests(1);
                    return _guests;
                }
            }
            set
            {
                _guests = StockItemService.GetCurrentGuests(1);
            }
        }

        public IEnumerable<Category> CategoriesList
        {
            get
            {
                if (_categories != null)
                {
                    if (GetHappyHour() == 1)
                        return _categories;
                    else
                        return _categories.Where(x => !x.Name.ToUpper().StartsWith("HAPPY")).ToList();
                } 
                else
                {
                    _categories = StockItemService.GetCategories(1);
                    if (GetHappyHour() == 1)
                        return _categories;
                    else
                        return _categories.Where(x => !x.Name.ToUpper().StartsWith("HAPPY")).ToList();
                }
            }
            set
            {
                _categories = StockItemService.GetCategories(1);
            }
        }

        private int GetHappyHour()
        {
            string happyHour = string.Empty;

            try
            {
                happyHour = ConfigurationManager.AppSettings["HappyHour"].ToString();
            }
            catch
            {
                happyHour = string.Empty;
            }

            if (string.IsNullOrEmpty(happyHour))
            {
                return 0;
            }
            else
            {
                var h = happyHour.Split(',');
                if (h.Length > 0)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        try
                        {
                            var hHour = h[i].Split('@');

                            if (hHour.Length == 3)
                            {
                                int day = 0;
                                int start = 0;
                                int end = 0;

                                int.TryParse(hHour[0], out day);
                                int.TryParse(hHour[1], out start);
                                int.TryParse(hHour[2], out end);

                                var now = DateTime.Now;

                                var dateDay = now.Day;
                                var hour = now.Hour;

                                if (day == dateDay && hour >= start && hour <= end)
                                {
                                    return 1;
                                }
                            }
                            else
                            {
                                break;
                            }

                        }
                        catch
                        {
                            break;
                        }

                    }
                }
            }

            return 0;
        }

        ///[OutputCache(Duration = int.MaxValue, VaryByParam = "name")]
        public ActionResult GetAnonymous(string name)
        {
            var _distributionPointId = CashierDistributionPointId().Value;

            var availableItemsId = _posItemService.GetAllInclude("StockItem").Where(x => x.Remaining > 0 && x.DistributionPointId == _distributionPointId).Select(x => x.ItemId).ToList();

            name = name.ToUpper();

            var catListId = CategoriesList.Select(x => x.Id);

            var products = ProductsList.Where(x => x.StockItemName.ToUpper().Contains(name) && catListId.Contains(x.CategoryId) && availableItemsId.Contains((int)x.Id)).ToList();

            IndexViewModel vm = new IndexViewModel();

            vm.productsList = products.OrderBy(x => x.StockItemName);

            return PartialView("_Products", vm);
        }

        ///[OutputCache(Duration = int.MaxValue, VaryByParam = "category_id")]
        public ActionResult GetProducts(int? category_id, int? cat_id, int? per_page)
        {
            var _distributionPointId = CashierDistributionPointId().Value;

            var availableItemsId = _posItemService.GetAllInclude("StockItem").Where(x => x.Remaining > 0 && x.DistributionPointId == _distributionPointId).Select(x => x.ItemId).ToList();

            var products = ProductsList.Where(x => x.CategoryId == category_id && availableItemsId.Contains((int)x.Id)).ToList();

            IndexViewModel vm = new IndexViewModel();

            vm.productsList = products.OrderBy(x => x.StockItemName);

            return PartialView("_Products", vm);
        }

        private static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["Core"].ConnectionString;
        }
        
        public ActionResult SetClubPrice(int? product_id)
        {
            bool setClubPrice = false;

            if (product_id == 1)
                setClubPrice = true;

            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("SetClubPrice", myConnection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    myConnection.Open();

                    SqlParameter custId = cmd.Parameters.AddWithValue("@Discount", setClubPrice);

                    cmd.ExecuteNonQuery();

                    return Json(new
                    {
                        Remainder = setClubPrice
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            
        }

        public ActionResult GetRandekhiSeperateRate()
        {
            return Json(new
            {
                TaxRate = GetSeperateGuestTax()
            }, JsonRequestBehavior.AllowGet);
        }

        
        //[OutputCache(Duration = int.MaxValue, VaryByParam = "product_id,newQty,tableId")]
        public ActionResult GetProductCount(int? product_id, int? newQty, int? tableId)
        {
            var allPosItems = _posItemService.GetAll().ToList();

            var availableItemsId = allPosItems.Where(x => x.Remaining > 0).Select(x => x.ItemId).ToList();

            var product = ProductsList.FirstOrDefault(x => x.Id == product_id.Value && availableItemsId.Contains((int)x.Id));

            var actualPrice = decimal.Zero;

            if(product == null)
            {
                product = new POSService.Entities.StockItem();
                product.TotalQuantity = 0;
                product.UnitPrice = decimal.Zero;
            }
            else
            {
                var posItem = allPosItems.FirstOrDefault(x => x.ItemId == product_id.Value);
                actualPrice = product.UnitPrice;
                product.TotalQuantity = posItem.Remaining;
            }

            if (newQty.HasValue && (User.IsInRole("MANAGER") || User.IsInRole("BARTENDER") || User.IsInRole("CASHIER") ))
            {
                if(product.TotalQuantity >= newQty.Value)
                {
                    var allTableItems = _tableItemService.GetAllEvery("BarTable").ToList();

                    var item = allTableItems.FirstOrDefault(x => x.TableId == tableId.Value && x.ItemId == product_id.Value);

                    if(tableId.Value == 0)
                    {
                        item = allTableItems.FirstOrDefault(x => x.BarTable.TableId == tableId.Value && x.Cashier == Person.Value && x.ItemId == product_id.Value);
                    }

                    

                    if(item != null)
                    {
                        item.Qty = newQty.Value;
                        _tableItemService.Update(item);

                        if (item.GuestOrderItemId.HasValue)
                        {
                            var existinggoi = _guestOrderItemService.GetById(item.GuestOrderItemId.Value);

                            if (existinggoi != null)
                            {
                                existinggoi.Quantity = newQty.Value;
                                existinggoi.Price = existinggoi.Quantity * actualPrice;
                                _guestOrderItemService.Update(existinggoi);
                            }
                        }
                    }

                }
            }

            return Json(new
            {
                Remainder = product.TotalQuantity

            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SetTerminal(string[] terminal)
        {
            HttpContext.SetCourseListToNullCookie("PosTerminal");

            var pcs = (terminal ?? new string[0])
               .SelectMany(p => p.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)).ToArray();
            HttpContext.AmendCourseListCookie(ActionType.Add, pcs, "PosTerminal");

            return Json(new
            {                
                name = terminal,
                id = GetTerminalId(terminal.FirstOrDefault()),
            }, JsonRequestBehavior.AllowGet);
        }

        private int GetTerminalId(string terminal)
        {
            var str = terminal;

            if (terminal.ToUpper().StartsWith("B"))
                return (int)RoomPaymentTypeEnum.Bar;
            else if (terminal.ToUpper().StartsWith("R"))
                return (int)RoomPaymentTypeEnum.Restuarant;
            else if (terminal.ToUpper().StartsWith("L"))
                return (int)RoomPaymentTypeEnum.Laundry;
            else
                return (int)RoomPaymentTypeEnum.Restuarant;
        }

        

        public ActionResult DeleteEmptyTables()
        {
            var guestGroupByTablesAll = _guestOrderService.GetAll("Guest,BarTable,BarTable.Person").Where(x => x.IsActive).ToList();

            foreach (var t in guestGroupByTablesAll)
            {
                var actualGuestOrder = t;
                DeleteTable(actualGuestOrder);
            }

            return RedirectToAction("Index");
        }

        private void DeleteTable(GuestOrder actualGuestOrder)
        {
           if(actualGuestOrder.BarTable != null)
           {
               var bart = actualGuestOrder.BarTable;
               var itemCount = bart.TableItems.Where(x => x.IsActive).Count();
               if (bart.TableItems.Count == 0 || (itemCount == 0))
               {
                  //var fDelete = _guestOrderService.GetById(actualGuestOrder.Id);
                  //fDelete.IsActive = false;
                  //_guestOrderService.Update(fDelete);

                  //var tableForDelete = _barTableService.GetById(bart.Id);
                  //_barTableService.Delete(tableForDelete);
                   var conn = ConfigurationManager.ConnectionStrings[1].ConnectionString;

                   StockItemService.DeleteTableItems(bart.Id, conn, Person.Value);
               }
           }
        }
   

        public ActionResult GetItemByBarCode(string csrf_sma, string code)
        {
            Tax_Rate t = new Tax_Rate { id = 1, rate = 10, type = 2 };
            var product = ProductsList.FirstOrDefault(x => x.Barcode == code);

            return Json(new
            {
                item_price = product.UnitPrice,
                product_name = product.StockItemName,
                product_code = product.Id.ToString(),
                tax_rate = t
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReduceOrDeleteQuatity(int? tableId, int? id, int? qty)
        {
            var itemIdString = id.ToString().Substring(1);

            int canDelete = 0;

            var itemId = int.Parse(itemIdString);

            var item = new TableItem();

            if (tableId.HasValue && tableId == 0)
            {
                item = _tableItemService.GetAllEvery("BarTable").FirstOrDefault(x => x.BarTable.TableId == tableId.Value && x.ItemId == itemId && x.Cashier == Person.Value);
            }
            else
            {
                item = _tableItemService.GetAllEvery("").FirstOrDefault(x => x.TableId == tableId.Value && x.ItemId == itemId);
            }

            if (item != null)
            {
                var quantity = item.Qty;

                if (quantity == qty.Value)
                {
                    var guestOderItemId = item.GuestOrderItemId;
                    var guestId = item.BarTable.GuestId;

                    //if ((User.IsInRole("MANAGER") || User.IsInRole("BARTENDER") || User.IsInRole("CASHIER")))
                    if (User.IsInRole("MANAGER") || !item.IsActive)
                    {
                        if (guestOderItemId.HasValue)
                        {
                            var guestOrderItem = _guestOrderItemService.GetById(guestOderItemId);
                            
                            if (guestOrderItem != null)
                                _guestOrderItemService.Delete(guestOrderItem);
                        }

                        var personId = Person.Value;
                        var realItemId = item.ItemId;

                        _tableItemService.Delete(item);

                        canDelete = 1;

                        if (User.IsInRole("MANAGER") && item.IsActive)
                        {
                            var soldItemsAll = new SoldItemsAll();
                            soldItemsAll.DateSold = DateTime.Now;
                            soldItemsAll.PersonId = personId;
                            soldItemsAll.Qty = item.Qty;
                            soldItemsAll.GuestId = guestId;
                            soldItemsAll.ItemId = realItemId;
                            soldItemsAll.TotalPrice = decimal.Zero;
                            soldItemsAll.TransactionId = item.Cashier;
                            RecordManagersDelete(soldItemsAll);
                        }
                    }
                    else
                    {
                        canDelete = 0;
                    }
                }
                else
                {
                    var newQty = qty.Value;
                    var guestOderItemId = item.GuestOrderItemId;
                    if (guestOderItemId.HasValue)
                    {
                        var guestOrderItem = _guestOrderItemService.GetById(guestOderItemId);

                        if (guestOrderItem != null)
                        {
                            guestOrderItem.Quantity -= newQty;
                            _guestOrderItemService.Update(guestOrderItem);
                        }
                    }

                    item.Qty -= newQty;

                    _tableItemService.Update(item);

                    if (item.GuestOrderItemId.HasValue)
                    {
                        var existinggoi = _guestOrderItemService.GetById(item.GuestOrderItemId.Value);
                        if (existinggoi != null)
                        {
                            existinggoi.Quantity -= newQty;
                            _guestOrderItemService.Update(existinggoi);
                        }
                    }
                }
            }

            return Json(new
            {
                ReturnValue = canDelete,
                success = true
            }, JsonRequestBehavior.AllowGet);


        }

        private void RecordManagersDelete(SoldItemsAll soldItemsAll)
        {
            var actualItem = StockItemService.GetStockItems(1).FirstOrDefault(x => x.Id == soldItemsAll.ItemId);
            var totalPrice = decimal.Zero;

            if(actualItem != null)
            {
                totalPrice = soldItemsAll.Qty * actualItem.UnitPrice;
            }

            soldItemsAll.TotalPrice = totalPrice;

           var sqlString = @"INSERT INTO [dbo].[SOLDITEMS]
           ([ItemId]
           ,[Qty]
           ,[TotalPrice]
           ,[TransactionId]
           ,[DateSold]
		   ,[PersonId]
		   ,[GuestId]
           ,[IsActive]
		   )
           VALUES
           (@ItemId
           ,@Qty
           ,@TotalPrice
           ,@TransactionId
           ,@DateSold
		   ,@PersonId
		   ,@GuestId
           ,@IsActive
		   )";

            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sqlString, myConnection))
                {
                    myConnection.Open();
                    SqlParameter custId1 = cmd.Parameters.AddWithValue("@ItemId", soldItemsAll.ItemId);
                    SqlParameter custId2 = cmd.Parameters.AddWithValue("@Qty", soldItemsAll.Qty);
                    SqlParameter custId3 = cmd.Parameters.AddWithValue("@TotalPrice", soldItemsAll.TotalPrice);
                    SqlParameter custId4 = cmd.Parameters.AddWithValue("@TransactionId", soldItemsAll.TransactionId);
                    SqlParameter custId5 = cmd.Parameters.AddWithValue("@DateSold", soldItemsAll.DateSold);
                    SqlParameter custId6 = cmd.Parameters.AddWithValue("@PersonId", soldItemsAll.PersonId);
                    SqlParameter custId7 = cmd.Parameters.AddWithValue("@GuestId", soldItemsAll.GuestId);
                    SqlParameter custId8 = cmd.Parameters.AddWithValue("@IsActive", true);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public ActionResult ReduceOrDeleteQuatityB4BIOYE(int? tableId, int? id, int? qty)
        {
            var itemIdString = id.ToString().Substring(1);

            var itemId = int.Parse(itemIdString);

            var item = new TableItem();

            if (tableId.HasValue && tableId == 0)
            {
               item =  _tableItemService.GetAll("BarTable").FirstOrDefault(x => x.BarTable.TableId == tableId.Value && x.ItemId == itemId && x.Cashier == Person.Value);
            }
            else
            {
               item = _tableItemService.GetAll().FirstOrDefault(x => x.TableId == tableId.Value && x.ItemId == itemId);
            }

            if(item != null)
            {
                var quantity = item.Qty;

                if(quantity == qty.Value)
                {
                    var guestOderItemId = item.GuestOrderItemId;

                    //if ((User.IsInRole("MANAGER") || User.IsInRole("BARTENDER") || User.IsInRole("CASHIER") ))
                    if (User.IsInRole("MANAGER"))
                    {
                        if (guestOderItemId.HasValue)
                        {
                            var guestOrderItem = _guestOrderItemService.GetById(guestOderItemId);
                            if (guestOrderItem != null)
                                _guestOrderItemService.Delete(guestOrderItem);
                        }

                        _tableItemService.Delete(item);
                    }
                }
                else
                {
                    var newQty = qty.Value;
                    var guestOderItemId = item.GuestOrderItemId;
                    if (guestOderItemId.HasValue)
                    {
                        var guestOrderItem = _guestOrderItemService.GetById(guestOderItemId);

                        if (guestOrderItem != null)
                        {
                            guestOrderItem.Quantity -= newQty;
                            _guestOrderItemService.Update(guestOrderItem);
                        }
                    }

                    item.Qty -= newQty;

                    _tableItemService.Update(item);

                    if (item.GuestOrderItemId.HasValue)
                    {
                        var existinggoi = _guestOrderItemService.GetById(item.GuestOrderItemId.Value);
                        if (existinggoi != null)
                        {
                            existinggoi.Quantity -= newQty;
                            _guestOrderItemService.Update(existinggoi);
                        }
                    }
                }
            }           

            return Json(new
            {
                success = true
            }, JsonRequestBehavior.AllowGet);


        }


        public ActionResult DeleteFromTable(int? id)
        {
            var items = new List<TableItem>();

            if(id.Value == 0)
            {
                items = _tableItemService.GetAllEvery("BarTable").Where(x => x.BarTable.TableId == id.Value).ToList();
            }
            else
            {
                items = _tableItemService.GetAll().Where(x => x.TableId == id.Value).ToList();
            }

            foreach(var item in items)
            {
                var guestOderItemId = item.GuestOrderItemId;
                var guestId = item.BarTable.GuestId;

                if (guestOderItemId.HasValue)
                {
                    var guestOrderItem = _guestOrderItemService.GetById(guestOderItemId);

                    if (guestOrderItem != null)
                        _guestOrderItemService.Delete(guestOrderItem);
                }

                var personId = Person.Value;
                var realItemId = item.ItemId;

                _tableItemService.Delete(item);

                if(item.IsActive)
                {
                    var soldItemsAll = new SoldItemsAll();
                    soldItemsAll.DateSold = DateTime.Now;
                    soldItemsAll.PersonId = personId;
                    soldItemsAll.Qty = item.Qty;
                    soldItemsAll.GuestId = guestId;
                    soldItemsAll.ItemId = realItemId;
                    soldItemsAll.TotalPrice = decimal.Zero;
                    soldItemsAll.TransactionId = item.Cashier;
                    RecordManagersDelete(soldItemsAll);
                }

            }

            return Json(new
            {
                success = true
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPriceB4(int? code, string v)
        {
            Tax_Rate t = new Tax_Rate { id = 1, rate = 10, type = 2 };
            var product = ProductsList.FirstOrDefault(x => x.Id == code);

            return Json(new
            {
                price = product.UnitPrice,
                name = product.StockItemName,
                code = product.Id.ToString(),
                tax_rate = t,
                available = product.TotalQuantity
            }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetPriceExistingRows(int? code, int? tableId, bool? ignore)
        {
            var _distributionPointId = CashierDistributionPointId().Value;

            Tax_Rate t = new Tax_Rate { id = 1, rate = 10, type = 2 };

            //var thisUser = _personService.GetAllForLogin().FirstOrDefault(x => x.Username.ToUpper().Equals(User.Identity.Name.ToUpper()));

            var actualItemPrice = decimal.Zero;

            if (!tableId.HasValue)
            {
                tableId = 0;
                ignore = false;
            }
            //var product = ProductsList.FirstOrDefault(x => x.Id == code);

            var allPosItems = _posItemService.GetAll().ToList();

            var availableItemsId = allPosItems.Where(x => x.DistributionPointId == _distributionPointId).Select(x => x.ItemId).ToList();

            var product = ProductsList.FirstOrDefault(x => x.Id == code && availableItemsId.Contains((int)x.Id));

            if (product == null)
            {
                product = new POSService.Entities.StockItem();
                product.TotalQuantity = 0;
                product.StockItemName = "NOT FOUND";
                product.Id = 0;
                product.UnitPrice = decimal.Zero;
            }
            else
            {
                var posItem = allPosItems.FirstOrDefault(x => x.ItemId == code.Value);
                product.TotalQuantity = posItem.Remaining;
                //actualItemPrice = product.UnitPrice;
                actualItemPrice = (product.Discounted && product.ClubPrice.HasValue && product.ClubPrice.Value > 0) ? product.ClubPrice.Value : product.UnitPrice;
            }

            if (product.TotalQuantity > 0 && (ignore.HasValue && !ignore.Value))
            {

                if (tableId.HasValue && tableId.Value > 0)
                {
                    var existingTable = _tableItemService.GetAll().FirstOrDefault(x => x.TableId == tableId && x.ItemId == code.Value && x.Cashier == Person.Value);

                    var go = _guestOrderService.GetAll("").FirstOrDefault(x => x.TableId == tableId && x.IsActive);

                    if (go != null)
                    {
                        if (existingTable != null)
                        {
                            existingTable.Qty = existingTable.Qty + 1;
                            _tableItemService.Update(existingTable);

                            if (existingTable.GuestOrderItemId.HasValue)
                            {
                                var existinggoi = _guestOrderItemService.GetById(existingTable.GuestOrderItemId.Value);
                                if (existinggoi != null)
                                {
                                    existinggoi.Quantity = existinggoi.Quantity + 1;
                                    existinggoi.Price = existinggoi.Quantity * actualItemPrice;
                                    _guestOrderItemService.Update(existinggoi);
                                }
                            }
                        }
                        else
                        {
                            GuestOrderItem goi = new GuestOrderItem();
                            goi.IsActive = true;
                            goi.Confirmed = false;
                            goi.CreatedDate = DateTime.Now;
                            goi.Delivered = false;
                            goi.GuestCanSee = true;
                            goi.Paid = false;
                            goi.Price = product.UnitPrice;
                            goi.Quantity = 1;
                            goi.ItemId = code.Value;
                            goi.WaitreesCanSee = true;
                            goi.GuestOrderId = go.Id;
                            goi = _guestOrderItemService.Create(goi);

                            TableItem ti = new TableItem();
                            ti.DateSold = DateTime.Now;
                            ti.Cashier = Person.Value;
                            ti.ItemId = code.Value;
                            ti.Qty = 1;
                            ti.TableId = tableId.Value;
                            ti.GuestOrderItemId = goi.Id;
                            ti.CollectedTime = null;
                            ti.Completed = false;
                            ti.Collected = false;
                            ti.CompletedTime = null;
                            ti.Fulfilled = false;
                            ti.IsActive = false;
                            _tableItemService.Create(ti);
                        }

                    }
                }
                else
                {
                    var existingBarTable = _barTableService.GetAll().FirstOrDefault(x => x.TableId == 0 && x.StaffId == Person.Value);

                    if (existingBarTable != null)
                    {
                        TableItem ti = new TableItem();
                        ti.DateSold = DateTime.Now;
                        ti.Cashier = Person.Value;
                        ti.ItemId = code.Value;
                        ti.Qty = 1;
                        ti.TableId = existingBarTable.Id;
                        ti.CollectedTime = null;
                        ti.Completed = false;
                        ti.Collected = false;
                        ti.CompletedTime = null;
                        ti.Fulfilled = false;
                        ti.IsActive = false;
                        _tableItemService.Create(ti);
                    }
                    else
                    {
                        var anonymousGuest = _guestService.GetAll(1).FirstOrDefault(x => x.FullName == "GUEST");

                        if (anonymousGuest != null)
                        {
                            BarTable newBarTable = new BarTable { CreatedDate = DateTime.Now, GuestId = (int)anonymousGuest.Id, IsActive = false, TableAlias = "QuickSale", TableName = "QuickSale", StaffId = Person.Value, TableId = 0 };

                            _barTableService.Create(newBarTable);

                            if (newBarTable.Id > 0)
                            {
                                TableItem ti = new TableItem();
                                ti.DateSold = DateTime.Now;
                                ti.Cashier = Person.Value;
                                ti.ItemId = code.Value;
                                ti.Qty = 1;
                                ti.TableId = newBarTable.Id;
                                ti.CollectedTime = null;
                                ti.Completed = false;
                                ti.Collected = false;
                                ti.CompletedTime = null;
                                ti.Fulfilled = false;
                                ti.IsActive = false;
                                _tableItemService.Create(ti);
                            }
                        }
                    }
                }
            }

            return Json(new
            {
                price = (product.Discounted && product.ClubPrice.HasValue && product.ClubPrice.Value > 0) ? product.ClubPrice.Value : product.UnitPrice,
                name = product.StockItemName,
                code = product.Id.ToString(),
                tax_rate = t,
                available = product.TotalQuantity
            }, JsonRequestBehavior.AllowGet);
        }

        //[OutputCache(Duration = int.MaxValue, VaryByParam = "code,tableId,ignore")]        
        public ActionResult GetPrice(int? code, int? tableId, bool? ignore)
        {
            var _distributionPointId = CashierDistributionPointId().Value;

            Tax_Rate t = new Tax_Rate { id = 1, rate = 10, type = 2 };

            //var thisUser = _personService.GetAllForLogin().FirstOrDefault(x => x.Username.ToUpper().Equals(User.Identity.Name.ToUpper()));

            var actualItemPrice = decimal.Zero;

            if(!tableId.HasValue)
            {
                tableId = 0;
                ignore = false;
            }
            //var product = ProductsList.FirstOrDefault(x => x.Id == code);

            var allPosItems = _posItemService.GetAll().ToList();

            var availableItemsId = allPosItems.Where(x => x.Remaining > 0 && x.DistributionPointId == _distributionPointId).Select(x => x.ItemId).ToList();

            var product = ProductsList.FirstOrDefault(x => x.Id == code && availableItemsId.Contains((int)x.Id));

            if (product == null)
            {
                product = new POSService.Entities.StockItem();
                product.TotalQuantity = 0;
                product.StockItemName = "NOT FOUND";
                product.Id = 0;
                product.UnitPrice = decimal.Zero;
            }
            else
            {
                var posItem = allPosItems.FirstOrDefault(x => x.ItemId == code.Value);
                product.TotalQuantity = posItem.Remaining;
                //actualItemPrice = product.UnitPrice;
                actualItemPrice = (product.Discounted && product.ClubPrice.HasValue && product.ClubPrice.Value > 0) ? product.ClubPrice.Value : product.UnitPrice;
            }

            if(product.TotalQuantity > 0  && (ignore.HasValue && !ignore.Value))
            {

                if(tableId.HasValue && tableId.Value > 0)
                {
                    var existingTable = _tableItemService.GetAll().FirstOrDefault(x => x.TableId == tableId && x.ItemId == code.Value && x.Cashier == Person.Value);

                    var go = _guestOrderService.GetAll("").FirstOrDefault(x => x.TableId == tableId && x.IsActive);

                    if (go != null)
                    {
                        if (existingTable != null)
                        {
                            existingTable.Qty = existingTable.Qty + 1;
                            _tableItemService.Update(existingTable);

                            if (existingTable.GuestOrderItemId.HasValue)
                            {
                                var existinggoi = _guestOrderItemService.GetById(existingTable.GuestOrderItemId.Value);
                                if (existinggoi != null)
                                {
                                    existinggoi.Quantity = existinggoi.Quantity + 1;
                                    existinggoi.Price = existinggoi.Quantity * actualItemPrice;
                                    _guestOrderItemService.Update(existinggoi);
                                }
                            }
                        }
                        else
                        {
                            GuestOrderItem goi = new GuestOrderItem();
                            goi.IsActive = true;
                            goi.Confirmed = false;
                            goi.CreatedDate = DateTime.Now;
                            goi.Delivered = false;
                            goi.GuestCanSee = true;
                            goi.Paid = false;
                            goi.Price = product.UnitPrice;
                            goi.Quantity = 1;
                            goi.ItemId = code.Value;
                            goi.WaitreesCanSee = true;
                            goi.GuestOrderId = go.Id;
                            goi = _guestOrderItemService.Create(goi);

                            TableItem ti = new TableItem();
                            ti.DateSold = DateTime.Now;
                            ti.Cashier = Person.Value;
                            ti.ItemId = code.Value;
                            ti.Qty = 1;
                            ti.TableId = tableId.Value;
                            ti.GuestOrderItemId = goi.Id;
                            ti.CollectedTime = null;
                            ti.Completed = false;
                            ti.Collected = false;
                            ti.CompletedTime = null;
                            ti.Fulfilled = false;
                            ti.IsActive = false;
                            _tableItemService.Create(ti);
                        }

                    }
                }
                else
                {
                    var existingBarTable = _barTableService.GetAll().FirstOrDefault(x => x.TableId == 0 && x.StaffId == Person.Value);

                    if(existingBarTable != null)
                    {
                               TableItem ti = new TableItem();
                                ti.DateSold = DateTime.Now;
                                ti.Cashier = Person.Value;
                                ti.ItemId = code.Value;
                                ti.Qty = 1;
                                ti.TableId = existingBarTable.Id;
                                ti.CollectedTime = null;
                                ti.Completed = false;
                                ti.Collected = false;
                                ti.CompletedTime = null;
                                ti.Fulfilled = false;
                                ti.IsActive = false;
                                _tableItemService.Create(ti);
                    }
                    else
                    {
                        var anonymousGuest = _guestService.GetAll(1).FirstOrDefault(x => x.FullName == "GUEST");

                        if (anonymousGuest != null)
                        {
                            BarTable newBarTable = new BarTable { CreatedDate = DateTime.Now, GuestId = (int)anonymousGuest.Id, IsActive = false, TableAlias = "QuickSale", TableName = "QuickSale", StaffId = Person.Value, TableId = 0 };

                            _barTableService.Create(newBarTable);

                            if (newBarTable.Id > 0)
                            {
                                TableItem ti = new TableItem();
                                ti.DateSold = DateTime.Now;
                                ti.Cashier = Person.Value;
                                ti.ItemId = code.Value;
                                ti.Qty = 1;
                                ti.TableId = newBarTable.Id;
                                ti.CollectedTime = null;
                                ti.Completed = false;
                                ti.Collected = false;
                                ti.CompletedTime = null;
                                ti.Fulfilled = false;
                                ti.IsActive = false;
                                _tableItemService.Create(ti);
                            }
                        }
                    }
                }
            }

            return Json(new
            {
                price = (product.Discounted && product.ClubPrice.HasValue && product.ClubPrice.Value > 0) ? product.ClubPrice.Value : product.UnitPrice,
                name = product.StockItemName,
                code = product.Id.ToString(),
                tax_rate = t,
                available = product.TotalQuantity
            }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult TransferTill(int? id)
        {
            //var thisUser = _personService.GetAllForLogin().Where(x => x.Username.ToUpper().Equals(User.Identity.Name.ToUpper())).FirstOrDefault();

            var person = CashiersList.FirstOrDefault(x => x.PersonID == id);
            var thisCashier = CashiersList.FirstOrDefault(x => x.PersonID == Person.Value);

            if (person != null && thisCashier != null && person.DistributionPointId == thisCashier.DistributionPointId)
            {
                var conn = ConfigurationManager.ConnectionStrings[1].ConnectionString;
                StockItemService.TransferTill(Person.Value, person.PersonID, conn);
                return Json(new
                {
                    Details = person.DisplayName,
                    SuccessText = "Cashier Successfully Added"
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                person = new Person { PersonID = 0, DisplayName = "Cashier" };
                return Json(new
                {
                    Details = person.DisplayName,
                    SuccessText = "Till Transfer Unsuccessfully!!"
                }, JsonRequestBehavior.AllowGet);
            }
           
            
        }

        public ActionResult GetGuestDetails(int? id)
        {
            if (System.Configuration.ConfigurationManager.AppSettings["UseHotelCustomers"].ToString() == "1")
            {
                var guest = GuestsList.FirstOrDefault(x => x.Id == id);

                if (guest == null)
                    guest = new POSService.Entities.Guest { Id = 0, FullName = "Outside Customer", RoomNumber = "Outside Customer", GuestRoomId = 0 };

                return Json(new
                {
                    Details = guest.RoomNumber,
                    SuccessText = "Customer Successfully Added",
                    Id = guest.Id,
                    GuestRoomId = guest.GuestRoomId
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var guest = BusinessAccountList.FirstOrDefault(x => x.Id == id);

                if (guest == null)
                    guest = new POSService.Entities.BusinessAccount { Id = 0, Name = "Quick Sale" };

                return Json(new
                {
                    Details = guest.Name,
                    SuccessText = "Customer Successfully Added",
                    Id = guest.Id,
                    GuestRoomId = guest.Id
                }, JsonRequestBehavior.AllowGet);

            }
        }

        
        [HttpGet]
        public ActionResult GetCashierSuspendedSales(string searchText)
        {
            ReportViewModel vm = new ReportViewModel();

            var allMessages = _suspendItemService.GetAll().Where(x => x.StaffId == Person.Value && x.IsActive).GroupBy(x => x.SuspensionTime).Select(x => new SuspensionModel { Id = x.ToList().FirstOrDefault().Id, SuspentionTime = x.Key, itemsList = x.ToList().Select(y => y.ItemName).ToDelimitedString(",") }).ToList();

            vm.AllSuspendedSales = allMessages;

            return PartialView("_CashierTableSales", vm);
        }

        [HttpGet]
        public ActionResult GetCashierMessages(string searchText)
        {
            ReportViewModel vm = new ReportViewModel();

            var allMessages = _guestMessageService.GetAll().Where(x => x.MessageDate > DateTime.Now.AddHours(-12)).ToList();

            vm.AllCashierMessages = allMessages;

            return PartialView("_CashierTableMessages", vm);
        }

        public ActionResult ViewTill()
        {
            var cashierId = Person.Value;

            var conn = ConfigurationManager.ConnectionStrings[1].ConnectionString;

            var startTime = DateTime.Now;

            var allSoldItems = _soldItemService.GetAllInclude("StockItem,PaymentMethod").Where(x => x.PersonId == cashierId && x.TillOpen && x.IsActive && (int)PaymentMethodEnum.POSTBILL != x.PaymentMethodId).OrderByDescending(x => x.DateSold).ToList();

            var lastItemSold = allSoldItems.OrderByDescending(x => x.DateSold).FirstOrDefault();
            var firstItemSold = allSoldItems.OrderByDescending(x => x.DateSold).LastOrDefault();


            if(!allSoldItems.Any())
            {
                ReportViewModel rvm = new ReportViewModel();

                rvm.AllSoldItemsNew = allSoldItems;

                rvm.AllDiscounts = new List<SalesDiscount>();

                rvm.Cash = decimal.Zero;

                return View(rvm);

            }
            else
            {
                
                var allDiscounts = new List<SalesDiscount>();

                
                if(allSoldItems.Count == 1)
                {
                    var timeOfDiscount = lastItemSold.DateSold;
                    allDiscounts = _salesDiscountService.GetAll().Where(x => x.ActualCashierId == cashierId && x.DiscountDate >= timeOfDiscount).ToList();
                }
                else if(allSoldItems.Count > 1)
                {
                    var endtimeOfDiscount = lastItemSold.DateSold;
                    var starttimeOfDiscount = firstItemSold.DateSold;
                    allDiscounts = _salesDiscountService.GetAll().Where(x => x.ActualCashierId == cashierId && x.DiscountDate >= starttimeOfDiscount && x.DiscountDate <= endtimeOfDiscount).ToList();
                }

                ReportViewModel rvm = new ReportViewModel();

                rvm.AllSoldItemsNew = allSoldItems;

                rvm.AllDiscounts = allDiscounts;

                rvm.Cash = allSoldItems.Where(x => (int)PaymentMethodEnum.COMPLIMENTARY != x.PaymentMethodId).Sum(x => x.TotalPrice) - allDiscounts.Sum(x => x.Amount);

                var overallTotal = rvm.Cash;

                var numberOfBuys = allSoldItems.Where(x => (int)PaymentMethodEnum.COMPLIMENTARY != x.PaymentMethodId).GroupBy(x => x.RecieptNumber).Count();

                var taxValue = GetRestaurantTax();

                var scValue = GetServiceCharge();

                if (taxValue > 0)
                {
                    rvm.Tax = taxValue * overallTotal;
                }

                rvm.SCValue = scValue * numberOfBuys;

                rvm.Cash = rvm.Cash + rvm.Tax + rvm.SCValue;

                return View(rvm);
            }
        }

        public ActionResult CloseTill(int? id)
        {
            if(!id.HasValue)
            {
                return RedirectToAction("ViewTill");
            }

            var cashierId = Person.Value;

            var unclearedItems = _tableItemService.GetAll().Where(x => x.Cashier == cashierId).Count();

            if(unclearedItems > 0)
            {
                return Content(@"<script language='javascript' type='text/javascript'>
                alert('You cannot close your till as you have pending items, please transfer your till first!');
                location.href = '/POS/Index';
                </script>");
            }

            var conn = ConfigurationManager.ConnectionStrings[1].ConnectionString;
            
            var allSoldItems = StockItemService.GetSoldItems(cashierId, conn);

            var receiptNumber = DateTime.Now.ToString().GetHashCode().ToString("x");
            
            try
            {
                PrintReceipt(allSoldItems, receiptNumber);
            }
            catch
            {

            }
            
            StockItemService.CloseTill(cashierId, conn);
            return RedirectToAction("Index");
        }


        public ActionResult TablesIndex()
        {
            return RedirectToAction("Table", new {id = 1 });
        }

        public ActionResult GetAvailableTables(int? id)
        {
            var distributionPointId = CashierDistributionPointId();

            var guestOrders = _guestOrderService.GetAll("BarTable,BarTable.Person").Where(x => x.IsActive).ToList();

            if(distributionPointId.HasValue)
            {
                guestOrders = guestOrders.Where(x => x.BarTable.Person.DistributionPointId == distributionPointId).ToList();
            }

            guestOrders = guestOrders.OrderBy(x => x.BarTable.TableId).ToList();

            var tables = guestOrders.Select(x => new SelectTableModel { TableId = x.TableId, Name = x.BarTable.TableName }).ToList();

            if(User.IsInRole("BARTENDER"))
            {
                tables.Clear();
            }

            var dropDownList = new SelectList(tables, "TableId", "Name", 0);

            return Json(new { DropDownList = dropDownList }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetOccupiedTables(int? id)
        {
            var distributionPointId = CashierDistributionPointId();

            var guestOrders = _guestOrderService.GetAll("BarTable,BarTable.Person").Where(x => x.IsActive).ToList();

            if (distributionPointId.HasValue)
            {
                guestOrders = guestOrders.Where(x => x.BarTable.Person.DistributionPointId == distributionPointId).ToList();
            }

            //var tables = guestOrders.Select(x => new SelectTableModel { TableId = x.TableId, Name = x.BarTable.TableName }).ToList();

            if (User.IsInRole("BARTENDER"))
            {
                guestOrders.Clear();
            }

            IndexViewModel vm = new IndexViewModel();

            vm.OccupiedTables = guestOrders.OrderBy(x => x.BarTable.TableId).ToList();

            return PartialView("_NewLookTables", vm);

            //var dropDownList = new SelectList(tables, "TableId", "Name", 0);

            //return Json(new { DropDownList = dropDownList }, JsonRequestBehavior.AllowGet);
        }

        

        //[OutputCache(Duration = int.MaxValue, VaryByParam = "id")]
        public ActionResult Table(int? id, bool? chargeSeperate)
        {
            if (!id.HasValue)
                return RedirectToAction("Index", new { chargeSeperate });

            //var allPipo = _personService.GetAllForLogin().ToList();

            //var thisUser = allPipo.FirstOrDefault(x => x.Username.ToUpper().Equals(User.Identity.Name.ToString().ToUpper()));

            IndexViewModel vm = new IndexViewModel();

            //var guestOrders = _guestOrderService.GetAll("").Where(x => x.IsActive).ToList();

            //var tables = guestOrders.Select(x => x.TableId).ToList();

            //vm.Tables = tables;

            vm.TableId = id.Value;

            var cl = HttpContext.GetCourseListCookie("PosTerminal");

            var terminal = "Terminal";

            if (!string.IsNullOrEmpty(cl.FirstOrDefault()))
            {
                terminal = cl.FirstOrDefault();
            }

            var cashier = _personService.GetAllInclude("DistributionPoint").FirstOrDefault(x => x.PersonID == GetPersonId());

            var _distributionPointId = cashier.DistributionPointId;

            vm.DistributionPointName = cashier.DistributionPoint.Description;

            var bt = _barTableService.GetById(id.Value);

            //bt = null;

            if(bt == null)
            {
                return RedirectToAction("index", new { chargeSeperate });
            }

            vm.Table = "Table " + id.Value;

            if(id.HasValue && id.Value == 0)
            {
                vm.Table = "QuickSale";
            }


            if(bt != null)
            {
                vm.Table = bt.TableName;
            }

            vm.Terminal = terminal;

            var lst = new List<POSService.Entities.Guest>();

            if (System.Configuration.ConfigurationManager.AppSettings["UseHotelCustomers"].ToString() == "1")
            {

                lst = GuestsList.ToList();

                var ids = lst.Select(x => x.Id).ToList();

                var lstNew = _guestService.GetAll(HotelID).Where(x => ids.Contains(x.Id) && !x.IsChild).ToList();

                var realIds = lstNew.Select(x => x.Id).ToList();

                lst.Insert(0, new POSService.Entities.Guest { Id = 0, FullName = "Outside Customer", RoomNumber = "Outside Customer", GuestRoomId = 0 });

                vm.CurrentGuests = lst.Where(x => realIds.Contains((int)x.Id)).ToList();

                vm.CurrentGuests.ToList().ForEach(x => x.RoomNumber = GetFullName(x, lstNew));
            }
            else
            {
                //BusinessAccountList
                var accountlst = BusinessAccountList.ToList();

                accountlst.Insert(0, new POSService.Entities.BusinessAccount { Id = 0, Name = "Quick Sale", });

                vm.CurrentBusinessAccounts = accountlst;

                vm.CurrentGuests = null;

            }

            vm.CurrentCashiers = CashiersList;

            var allPosItems = _posItemService.GetAllInclude("StockItem").Where(x => x.DistributionPointId == _distributionPointId).ToList();

            if (allPosItems.Any())
            {
                var nt = allPosItems.FirstOrDefault().StockItem;
                vm.ClubTime = nt.Discounted && nt.ClubPrice.HasValue && nt.ClubPrice.Value > 0;
            }

            vm.CashierCanOpenTable = GetCashierOpeningStatus();

            var availableItemsId = allPosItems.Where(x => x.Remaining > 0).Select(x => x.ItemId).ToList();

            vm.productsList = ProductsList.Where(x => availableItemsId.Contains((int)x.Id)).ToList();

            var counter = allPosItems.Count(x => x.StockItem.NotNumber >= x.Quantity);

            vm.ProductsAlerts = counter;

            var catIds = vm.productsList.Select(x => x.CategoryId).ToList();

            vm.categoriesList = CategoriesList.Where(x => catIds.Contains(x.Id)).ToList();

            vm.productsList = vm.productsList.Where(x => x.CategoryId == 2).OrderBy(x => x.StockItemName).ToList();

            if (id.HasValue && id == 0)
            {
                vm.ExistingList = _tableItemService.GetAll("BarTable").Where(x => x.BarTable.TableId == id.Value && x.Cashier == Person.Value).OrderByDescending(x => x.DateSold).ToList();
            }
            else
            {
                vm.ExistingList = _tableItemService.GetAll().Where(x => x.TableId == id.Value).OrderByDescending(x => x.DateSold).ToList();
            }

            var isManager = 0;

            if (User.IsInRole("MANAGER") || User.IsInRole("CASHIER") || User.IsInRole("BARTENDER"))
                isManager = 1;

            vm.IsManager = isManager;

            vm.CanSuspend = false;

            if (vm.ExistingList != null && vm.ExistingList.Count > 0)
                vm.ShowAllButtons = true;

            vm.RealTaxValue = GetRestaurantTax();

            vm.ChargeSeperately = ChargeOutsideGuestSeperately();

            if (chargeSeperate.HasValue && chargeSeperate.Value)
            {
                vm.ChargeSeperatelyOn = true;
            }

            if (vm.RealTaxValue > 0)
                vm.Tax = vm.RealTaxValue/100;

            return View(vm);
        }


        private int GetSeperateGuestTax()
        {
            int hTax = 0;

            try
            {
                int.TryParse(ConfigurationManager.AppSettings["ChargeHotelGuestDifferentlyTax"].ToString(), out hTax);
            }
            catch
            {
                hTax = 0;
            }

            return hTax;
        }

        private bool ChargeOutsideGuestSeperately()
        {
            
            bool hDifferentCharge = false;

            try
            {
                hDifferentCharge = ConfigurationManager.AppSettings["ChargeHotelGuestDifferently"].ToString() == "1";
            }
            catch
            {
                hDifferentCharge = false;
            }

            return hDifferentCharge;
        }

        private int GetRestaurantTax()
        {
            int hTax = 0;

            try
            {
                int.TryParse(ConfigurationManager.AppSettings["RestaurantTax"].ToString(), out hTax);
            }
            catch
            {
                hTax = 0;
            }

            return hTax;
        }

        private string GetRestaurantTaxDetails()
        {
            string hTaxDetails = "";

            try
            {
                ConfigurationManager.AppSettings["RestaurantTaxDetails"].ToString();
            }
            catch
            {
                hTaxDetails = "";
            }

            return hTaxDetails;
        }

        private int GetRestaurantTaxDisplayOnly()
        {
            int hTax = 0;

            try
            {
                int.TryParse(ConfigurationManager.AppSettings["RestaurantTaxDisplayOnly"].ToString(), out hTax);
            }
            catch
            {
                hTax = 0;
            }

            return hTax;
        }

        private bool IsFullPos()
        {
            bool isFullPosService = true;

            try
            {
                var yOrn = ConfigurationManager.AppSettings["IsFullPosService"].ToString().ToUpper();

                if (yOrn == "NO")
                    isFullPosService = false;
            }
            catch (Exception)
            {
                isFullPosService = false;
            }

            return isFullPosService;
        }



        public ActionResult ClearSuspend()
        {
            var lstSS = _suspendItemService.GetAll().Where(x => x.IsActive && x.StaffId == Person.Value).ToList();

            foreach(var s in lstSS)
            {
                _suspendItemService.Delete(s);
            }

            return RedirectToAction("Index");
        }

        public ActionResult IndexNew(int? id, int? suspendId, int? tempSuspendId)
        {
            var allForDelete = _tableItemService.GetAllEvery("BarTable").Where(x => x.BarTable.TableId == 0 && x.Cashier == Person.Value).ToList();

            try
            {
                foreach(var del in allForDelete)
                {
                    _tableItemService.Delete(del);
                }
            }
            catch
            {

            }

            return RedirectToAction("Index", new { id, suspendId, tempSuspendId });


            var isFullPos = IsFullPos();

            if (!isFullPos)
                return RedirectToAction("IndexCashier", "Guest");

            var retrieveLastSale = false;

            if (id.HasValue && id.Value == 1)
                retrieveLastSale = true;

            IndexViewModel vm = new IndexViewModel();

            var cl = HttpContext.GetCourseListCookie("PosTerminal");

            var terminal = "Terminal";

            if (!string.IsNullOrEmpty(cl.FirstOrDefault()))
            {
                terminal = cl.FirstOrDefault();
            }

            vm.Terminal = terminal;

            vm.Retrieve = retrieveLastSale;

            var cashier = _personService.GetAllInclude("DistributionPoint").FirstOrDefault(x => x.PersonID == GetPersonId());

            var _distributionPointId = cashier.DistributionPointId;

            vm.DistributionPointName = cashier.DistributionPoint.Description;


            vm.Table = "Tables";

            var lst = new List<POSService.Entities.Guest>();

            if (System.Configuration.ConfigurationManager.AppSettings["UseHotelCustomers"].ToString() == "1")
            {
                lst = GuestsList.ToList();

                var ids = lst.Select(x => x.Id).ToList();

                var lstNew = _guestService.GetAll(HotelID).Where(x => ids.Contains(x.Id) && !x.IsChild).ToList();

                var realIds = lstNew.Select(x => x.Id).ToList();

                lst.Insert(0, new POSService.Entities.Guest { Id = 0, FullName = "Outside Customer", RoomNumber = "Outside Customer", GuestRoomId = 0 });

                vm.CurrentGuests = lst.Where(x => realIds.Contains((int)x.Id)).ToList();

                vm.CurrentGuests.ToList().ForEach(x => x.RoomNumber = GetFullName(x, lstNew));
            }
            else
            {
                //BusinessAccountList
                var accountlst = BusinessAccountList.ToList();

                accountlst.Insert(0, new POSService.Entities.BusinessAccount { Id = 0, Name = "Quick Sale", });

                vm.CurrentBusinessAccounts = accountlst;

                vm.CurrentGuests = null;

            }

            //vm.CashierCanOpenTable = GetCashierOpeningStatus();

            vm.CurrentCashiers = CashiersList;

            var allPosItems = _posItemService.GetAllInclude("StockItem").Where(x => x.DistributionPointId == _distributionPointId).ToList();

            if (allPosItems.Any())
            {
                var nt = allPosItems.FirstOrDefault().StockItem;
                vm.ClubTime = nt.Discounted && nt.ClubPrice.HasValue && nt.ClubPrice.Value > 0;
            }

            vm.CashierCanOpenTable = GetCashierOpeningStatus();

            var availableItemsId = allPosItems.Where(x => x.Remaining > 0).Select(x => x.ItemId).ToList();

            vm.productsList = ProductsList.Where(x => availableItemsId.Contains((int)x.Id)).ToList();

            var counter = allPosItems.Count(x => x.StockItem.NotNumber >= x.Quantity);

            vm.ProductsAlerts = counter;


            var catIds = vm.productsList.Select(x => x.CategoryId).ToList();

            vm.categoriesList = CategoriesList.Where(x => catIds.Contains(x.Id)).ToList();

            vm.productsList = vm.productsList.Where(x => x.CategoryId == 2).OrderBy(x => x.StockItemName).ToList();

            if (retrieveLastSale)
            {

                var itemsSold = _soldItemService.GetAll().Where(x => x.PersonId == Person.Value);

                var itemSold = itemsSold.LastOrDefault();

                if (itemSold != null)
                {
                    var entireList = _soldItemService.GetAll().Where(x => x.DateSold == itemSold.DateSold).ToList();

                    var completeList = entireList.Select(x => new TableItem { Cashier = x.PersonId, DateSold = x.DateSold.Value, ItemId = x.ItemId, Qty = x.Qty, TableId = 9999 }).ToList();

                    vm.ExistingList = completeList;

                    foreach (var tableItem in completeList)
                    {
                        _tableItemService.Create(tableItem);
                    }

                    foreach (var soldItem in entireList)
                    {
                        _soldItemService.Delete(soldItem);
                    }
                }
                else
                {
                    vm.ExistingList = _tableItemService.GetAll().Where(x => x.TableId == 0 && x.Cashier == Person.Value).ToList();
                }
            }
            else
            {
                vm.ExistingList = _tableItemService.GetAll("BarTable").Where(x => x.BarTable.TableId == 0 && x.Cashier == Person.Value).ToList();
            }

            if (suspendId.HasValue && suspendId.Value > 0)
            {
                var lstSS = _suspendItemService.GetAll().Where(x => x.IsActive && x.StaffId == Person.Value).ToList();

                var ss = lstSS.FirstOrDefault(x => suspendId.Value == x.Id);

                if (ss != null)
                {
                    var lstNew = lstSS.Where(x => x.StaffId == Person.Value && x.SuspensionTime == ss.SuspensionTime).GroupBy(x => x.PosItemId).Select(x => new RetrieveModel { ItemId = x.Key, Quantity = x.ToList().Sum(y => y.Quantity) }).ToList();

                    vm.ExistingList.Clear();

                    var lstTT = new List<TableItem>();

                    foreach (var newss in lstNew)
                    {
                        lstTT.Add(new TableItem { ItemId = newss.ItemId, Qty = newss.Quantity });
                    }

                    vm.ExistingList = lstTT;
                }
            }

            if (tempSuspendId.HasValue && tempSuspendId.Value > 0)
            {
                var lstSS = _suspendItemService.GetAll().Where(x => !x.IsActive && x.StaffId == Person.Value).ToList();

                var ss = lstSS.FirstOrDefault(x => tempSuspendId.Value == x.Id);

                if (ss != null)
                {
                    var lstNew = lstSS.Where(x => x.StaffId == Person.Value && x.SuspensionTime == ss.SuspensionTime).GroupBy(x => x.PosItemId).Select(x => new RetrieveModel { ItemId = x.Key, Quantity = x.ToList().Sum(y => y.Quantity) }).ToList();

                    vm.ExistingList.Clear();

                    var lstTT = new List<TableItem>();

                    foreach (var newss in lstNew)
                    {
                        lstTT.Add(new TableItem { ItemId = newss.ItemId, Qty = newss.Quantity });
                    }

                    vm.ExistingList = lstTT;
                }
            }

            var isManager = 0;

            if (User.IsInRole("MANAGER") || User.IsInRole("CASHIER") || User.IsInRole("BARTENDER"))
                isManager = 1;

            vm.IsManager = isManager;

            vm.CanSuspend = true;

            if (vm.ExistingList != null && vm.ExistingList.Count > 0)
                vm.ShowAllButtons = true;


            vm.RealTaxValue = GetRestaurantTax();

            if (vm.RealTaxValue > 0)
                vm.Tax = vm.RealTaxValue / 100;

            return View("Index", vm);
        }


        public ActionResult Index(int? id, int? suspendId, int? tempSuspendId, bool? chargeSeperate)
        {
            var isFullPos = IsFullPos();

            if (!isFullPos)
                return RedirectToAction("IndexCashier", "Guest");

            var retrieveLastSale = false;

            if (id.HasValue && id.Value == 1)
                retrieveLastSale = true;  

            IndexViewModel vm = new IndexViewModel();

            var cl = HttpContext.GetCourseListCookie("PosTerminal");

            var terminal = "Terminal";

            if(!string.IsNullOrEmpty(cl.FirstOrDefault()))
            {
                terminal = cl.FirstOrDefault();
            }

            vm.Terminal = terminal;

            vm.Retrieve = retrieveLastSale;

            var cashier = _personService.GetAllInclude("DistributionPoint").FirstOrDefault(x => x.PersonID == GetPersonId());

            var _distributionPointId = cashier.DistributionPointId;

            vm.DistributionPointName = cashier.DistributionPoint.Description;


            vm.Table = "Tables";

            var lst = new List<POSService.Entities.Guest>();

            if (System.Configuration.ConfigurationManager.AppSettings["UseHotelCustomers"].ToString() == "1")
            {

                lst = GuestsList.ToList();

                var ids = lst.Select(x => x.Id).ToList();

                var lstNew = _guestService.GetAll(HotelID).Where(x => ids.Contains(x.Id) && !x.IsChild).ToList();

                var realIds = lstNew.Select(x => x.Id).ToList();

                lst.Insert(0, new POSService.Entities.Guest { Id = 0, FullName = "Outside Customer", RoomNumber = "Outside Customer", GuestRoomId = 0 });

                vm.CurrentGuests = lst.Where(x => realIds.Contains((int)x.Id)).ToList();

                vm.CurrentGuests.ToList().ForEach(x => x.RoomNumber = GetFullName(x, lstNew));
            }
            else
            {
                //BusinessAccountList
                var accountlst = BusinessAccountList.ToList();

                accountlst.Insert(0, new POSService.Entities.BusinessAccount { Id = 0, Name = "Quick Sale", });

                vm.CurrentBusinessAccounts = accountlst;

                vm.CurrentGuests = null;

            }

            //vm.CashierCanOpenTable = GetCashierOpeningStatus();

            vm.CurrentCashiers = CashiersList;

            var allPosItems = _posItemService.GetAllInclude("StockItem").Where(x => x.DistributionPointId == _distributionPointId).ToList();

            if(allPosItems.Any())
            {
                var nt = allPosItems.FirstOrDefault().StockItem;
                vm.ClubTime = nt.Discounted && nt.ClubPrice.HasValue && nt.ClubPrice.Value > 0;
            }

            vm.CashierCanOpenTable = GetCashierOpeningStatus();

            var availableItemsId = allPosItems.Where(x => x.Remaining > 0).Select(x => x.ItemId).ToList();

            vm.productsList = ProductsList.Where(x => availableItemsId.Contains((int)x.Id)).ToList();

            var counter = allPosItems.Count(x => x.StockItem.NotNumber >= x.Quantity);

            vm.ProductsAlerts = counter;
            

            var catIds = vm.productsList.Select(x => x.CategoryId).ToList();

            vm.categoriesList = CategoriesList.Where(x => catIds.Contains(x.Id)).ToList();

            vm.productsList = vm.productsList.Where(x => x.CategoryId == 2).OrderBy(x => x.StockItemName).ToList();

            if (retrieveLastSale)
            {

                var itemsSold = _soldItemService.GetAll().Where(x => x.PersonId == Person.Value);

                var itemSold = itemsSold.LastOrDefault();

                if(itemSold != null)
                {
                   var entireList = _soldItemService.GetAll().Where(x => x.DateSold == itemSold.DateSold).ToList();

                   var completeList = entireList.Select(x => new TableItem { Cashier = x.PersonId, DateSold = x.DateSold.Value, ItemId = x.ItemId, Qty = x.Qty, TableId = 9999 }).ToList();

                   vm.ExistingList = completeList;

                    foreach(var tableItem in completeList)
                    {
                        _tableItemService.Create(tableItem);
                    }

                    foreach (var soldItem in entireList)
                    {
                        _soldItemService.Delete(soldItem);
                    }
                }
                else
                {
                    vm.ExistingList = _tableItemService.GetAll().Where(x => x.TableId == 0 && x.Cashier == Person.Value).ToList();
                }
            }
            else
            {
                vm.ExistingList = _tableItemService.GetAll("BarTable").Where(x => x.BarTable.TableId == 0 && x.Cashier == Person.Value).ToList();
            }

            if(suspendId.HasValue && suspendId.Value > 0)
            {
                var lstSS = _suspendItemService.GetAll().Where(x => x.IsActive && x.StaffId == Person.Value).ToList();

                var ss = lstSS.FirstOrDefault(x => suspendId.Value == x.Id);

                if(ss != null)
                {
                    var lstNew = lstSS.Where(x => x.StaffId == Person.Value && x.SuspensionTime == ss.SuspensionTime).GroupBy(x => x.PosItemId).Select(x => new RetrieveModel { ItemId = x.Key, Quantity = x.ToList().Sum(y => y.Quantity) }).ToList();

                    vm.ExistingList.Clear();

                    var lstTT = new List<TableItem>();

                    foreach(var newss in lstNew)
                    {
                        lstTT.Add(new TableItem { ItemId = newss.ItemId, Qty = newss.Quantity });
                    }

                    vm.ExistingList = lstTT;
                }
            }

            if (tempSuspendId.HasValue && tempSuspendId.Value > 0)
            {
                var lstSS = _suspendItemService.GetAll().Where(x => !x.IsActive && x.StaffId == Person.Value).ToList();

                var ss = lstSS.FirstOrDefault(x => tempSuspendId.Value == x.Id);

                if (ss != null)
                {
                    var lstNew = lstSS.Where(x => x.StaffId == Person.Value && x.SuspensionTime == ss.SuspensionTime).GroupBy(x => x.PosItemId).Select(x => new RetrieveModel { ItemId = x.Key, Quantity = x.ToList().Sum(y => y.Quantity) }).ToList();

                    vm.ExistingList.Clear();

                    var lstTT = new List<TableItem>();

                    foreach (var newss in lstNew)
                    {
                        lstTT.Add(new TableItem { ItemId = newss.ItemId, Qty = newss.Quantity });
                    }

                    vm.ExistingList = lstTT;
                }
            }

            var isManager = 0;

            if (User.IsInRole("MANAGER") || User.IsInRole("CASHIER") || User.IsInRole("BARTENDER"))
                isManager = 1;

            vm.IsManager = isManager;

            vm.CanSuspend = true;

            if (vm.ExistingList != null && vm.ExistingList.Count > 0)
                vm.ShowAllButtons = true;


            vm.RealTaxValue = GetRestaurantTax();

            vm.ChargeSeperately = ChargeOutsideGuestSeperately();

            if (chargeSeperate.HasValue && chargeSeperate.Value)
            {
                vm.ChargeSeperatelyOn = true;
            }

            if (vm.RealTaxValue > 0)
                vm.Tax = vm.RealTaxValue / 100;

            return View(vm);
        }

        private bool GetCashierOpeningStatus()
        {
            if(!User.IsInRole("CASHIER"))
            {
                return false;
            }

            try
            {
                if (ConfigurationManager.AppSettings["CashierCanOpenTable"].ToString() == "1")
                    return true;
            }
            catch
            {
                return false;
            }

            return false;

        }

        private string GetFullName(POSService.Entities.Guest g, List<HotelMateWeb.Dal.DataCore.Guest> lst)
        {

            var thisGuest = lst.FirstOrDefault(x => x.Id == g.Id);
            if (thisGuest != null)
            {
                var balance = thisGuest.GetGuestBalance();
                return g.RoomNumber + " Balance ( NGN " + balance.ToString() + " )";
            }
            else
            {
                return g.RoomNumber;
            }
        }

        [HttpPost]
        public ActionResult CheckIn(string suspend, int? tableId, string retrieve, string processorder, string kitchenNote, bool? chargeSeperate)
        {

            var printOnly = false;

            var suspendSale = false;

            var processOrderOnly = false;

            try
            {
                if (!string.IsNullOrEmpty(processorder))
                {
                    processOrderOnly = true;
                    printOnly = true;
                }

                if (processOrderOnly)
                {
                    ProcessTheOrderByTableNumber(tableId, kitchenNote);

                    if (tableId.HasValue && tableId.Value > 0)
                        return RedirectToAction("Table", new { id = tableId.Value, chargeSeperate });
                    else
                        return RedirectToAction("Table", new { id = 0, chargeSeperate });
                }

                if (!string.IsNullOrEmpty(suspend))
                {
                    printOnly = true;
                }

                if (!string.IsNullOrEmpty(retrieve))
                {
                    suspendSale = true;
                }

                int count = 0;
                int guestId = 0;
                int guestRoomId = 0;
                var guestTableNumber = string.Empty;

                int paymentMethodId = 1;

                int.TryParse(Request.Form["rpaidby"], out paymentMethodId);

                var rpaidby = Request.Form["rpaidby"].ToString();
                paymentMethodId = GetPaymentMethod(rpaidby.ToUpper());


                var cc_no_val = Request.Form["cc_no_val"].ToString();
                var cc_holder_val = Request.Form["cc_holder_val"].ToString();
                var cheque_no_val = Request.Form["cheque_no_val"].ToString();

                var paymentMethodNote = cc_no_val + " " + cc_holder_val + " " + cheque_no_val;

                int.TryParse(Request.Form["count"], out count);
                int.TryParse(Request.Form["HotelGuestId"], out guestId);
                int.TryParse(Request.Form["GuestRoomId"], out guestRoomId);

                decimal amountPaidByCustomer = 0;

                decimal.TryParse(Request.Form["paid_val"], out amountPaidByCustomer);
                

                List<POSService.Entities.StockItem> lst = new List<POSService.Entities.StockItem>();

                if (System.Configuration.ConfigurationManager.AppSettings["UseHotelCustomers"].ToString() == "1")
                {

                    if (paymentMethodId == (int)PaymentMethodEnum.POSTBILL && guestId == 0)
                    {
                        return Content(@"<script language='javascript' type='text/javascript'>
                        alert('You cannot post a bill for a customer who is not staying in the hotel! Please go back and select a guest!');
                        location.href = '/POS/Index';
                        </script>");
                    }
                }
                else
                {
                    if (paymentMethodId == (int)PaymentMethodEnum.POSTBILL && guestId == 0)
                    {
                        return Content(@"<script language='javascript' type='text/javascript'>
                        alert('You cannot post a bill for a customer who is not an account holder! Please go back and select an account holder!');
                        location.href = '/POS/Index';
                        </script>");
                    }
                }

                var totalBill = Decimal.Zero;
                var cashierId = 0;

                //var thisUser = _personService.GetAllForLogin().Where(x => x.Username.ToUpper().Equals(User.Identity.Name.ToUpper())).FirstOrDefault();

                if (tableId.HasValue && tableId.Value > 0)
                {

                    BarTable bt = _barTableService.GetById(tableId.Value);

                    if(bt == null)
                    {
                        return RedirectToAction("Index");
                    }

                    guestTableNumber = bt.TableName;

                    var tableList = _tableItemService.GetAll().Where(x => x.TableId == tableId.Value).ToList();

                    foreach (var ti in tableList)
                    {
                        var thisProduct = ProductsList.FirstOrDefault(x => x.Id == ti.ItemId);

                        var price = thisProduct.UnitPrice;

                        price = (thisProduct.Discounted && thisProduct.ClubPrice.HasValue && thisProduct.ClubPrice.Value > 0) ? thisProduct.ClubPrice.Value : thisProduct.UnitPrice;

                        var qty = ti.Qty;

                        totalBill += (price * qty);

                        var itemDescription = thisProduct.StockItemName;

                        lst.Add(new POSService.Entities.StockItem { Id = thisProduct.Id, Quantity = qty, UnitPrice = price, Description = itemDescription });

                        cashierId = ti.Cashier;
                    }
                }
                else
                {
                    var tableList = _tableItemService.GetAll("BarTable").Where(x => x.BarTable.TableId == tableId.Value && x.Cashier == Person.Value).ToList();


                    foreach (var ti in tableList)
                    {
                        var thisProduct = ProductsList.FirstOrDefault(x => x.Id == ti.ItemId);

                        var price = thisProduct.UnitPrice;

                        price = (thisProduct.Discounted && thisProduct.ClubPrice.HasValue && thisProduct.ClubPrice.Value > 0) ? thisProduct.ClubPrice.Value : thisProduct.UnitPrice;

                        var qty = ti.Qty;

                        totalBill += (price * qty);

                        var itemDescription = thisProduct.StockItemName;

                        lst.Add(new POSService.Entities.StockItem { Id = thisProduct.Id, Quantity = qty, UnitPrice = price, Description = itemDescription });

                        cashierId = ti.Cashier;
                    }



                    //for (int i = 1; i < count; i++)
                    //{
                    //    string p = "product" + i.ToString();
                    //    string q = "quantity" + i.ToString();
                    //    string pr = "price" + i.ToString();

                    //    int productId = 0;
                    //    int qty = 0;
                    //    decimal price = decimal.Zero;

                    //    int.TryParse(Request.Form[p], out productId);
                    //    int.TryParse(Request.Form[q], out qty);
                    //    decimal.TryParse(Request.Form[pr], out price);

                    //    if (productId == 0)
                    //        continue;

                    //    totalBill += (price * qty);

                    //    var itemDescription = ProductsList.FirstOrDefault(x => x.Id == productId).StockItemName;

                    //    lst.Add(new POSService.Entities.StockItem { Id = productId, Quantity = qty, UnitPrice = price, Description = itemDescription });
                    //}

                }

                if(lst.Count == 0)
                {
                    //lst = _tableItemService.GetAll("BarTable,StockItem").Where(x => x.BarTable.TableId == tableId.Value && x.Cashier == Person.Value)
                    //    .Select(x => new POSService.Entities.StockItem { Id = x.ItemId, Quantity = x.Qty, UnitPrice = x.StockItem.UnitPrice.Value, Description = x.StockItem.Description }).ToList();

                    //Check For Processing

                    ProcessTheOrderByTableNumber(tableId, kitchenNote);

                    if (tableId.HasValue && tableId.Value > 0)
                        return RedirectToAction("Table", new { id = tableId.Value });
                    else
                        return RedirectToAction("Table", new { id = 0 });
                }

                var conn = ConfigurationManager.ConnectionStrings[1].ConnectionString;

                var ticks = (int)DateTime.Now.Ticks;

                var transactionId = Person.Value;

                var cl = HttpContext.GetCourseListCookie("PosTerminal");

                var terminal = "Terminal";

                if (!string.IsNullOrEmpty(cl.FirstOrDefault()))
                {
                    terminal = cl.FirstOrDefault();
                }

                int terminalId = GetPosTerminalId(terminal);

                var timeOfSale = DateTime.Now;

                var receiptNumber = DateTime.Now.ToString().GetHashCode().ToString("x");

                var discountedSum = decimal.Zero;

                if (!printOnly && !suspendSale)
                {
                    bool isHotel = false;

                    if (System.Configuration.ConfigurationManager.AppSettings["UseHotelCustomers"].ToString() == "1")
                    {
                        isHotel = true;
                    }

                   

                    if (amountPaidByCustomer > decimal.Zero && amountPaidByCustomer < totalBill)
                    {
                        discountedSum = totalBill - amountPaidByCustomer;
                    }

                    if (guestId > 0 && amountPaidByCustomer > 0 && rpaidby != "cash")
                    {
                        if (amountPaidByCustomer < totalBill && guestRoomId == 0)
                        {
                                //credit customer account
                                BusinessCorporateAccount bca = new BusinessCorporateAccount
                                {
                                    Amount = amountPaidByCustomer,
                                    TransactionDate = DateTime.Now,
                                    TransactionId = Person.Value,
                                    PaymentMethodId = (int)PaymentMethodEnum.Cash,
                                    PaymentMethodNote = paymentMethodNote,
                                    BusinessAccountId = guestId
                                };

                                _businessAccountCorporateService.Create(bca);
                        }
                        else
                        {
                            GuestRoomAccount gra = new GuestRoomAccount
                            {
                                Amount = amountPaidByCustomer,
                                GuestRoomId = guestRoomId,
                                TransactionDate = DateTime.Now,
                                PaymentMethodId = (int)PaymentMethodEnum.Cash,
                                PaymentMethodNote = "Paid To Restaurant, part payment/post bill",
                                PaymentTypeId = (int)RoomPaymentTypeEnum.Restuarant,
                                TransactionId = Person.Value
                            };

                            ////Hotel Customer Part Payment
                            GuestRoomAccountService gras = new GuestRoomAccountService();
                            gras.Create(gra);
                            
                        }
                    }

                    if (paymentMethodId == (int)PaymentMethodEnum.Cheque || paymentMethodId == (int)PaymentMethodEnum.CreditCard)
                    {
                        guestRoomId = 0;
                        guestId = 0;
                    }
                    if (paymentMethodId == (int)PaymentMethodEnum.Cash)
                    {
                        if(amountPaidByCustomer == decimal.Zero)
                        {
                            guestRoomId = 0;
                            guestId = 0;
                        }
                    }

                    if (amountPaidByCustomer == decimal.Zero)
                        discountedSum = decimal.Zero;

                    if (paymentMethodId == (int)PaymentMethodEnum.POSTBILL || paymentMethodId == (int)PaymentMethodEnum.COMPLIMENTARY)
                    {
                        discountedSum = decimal.Zero;
                    }

                    var _distributionPointId = CashierDistributionPointId().Value;

                    if (tableId.HasValue && tableId.Value > 0)
                    {
                        transactionId = tableId.Value;
                    }

                    if (string.IsNullOrEmpty(paymentMethodNote.Trim()))
                    {
                        paymentMethodNote = guestTableNumber;
                    }

                    if (chargeSeperate.HasValue && chargeSeperate.Value)
                    {
                        paymentMethodNote = paymentMethodNote + " chargeSeperate";
                    }

                    //StockItemService.UpdateSales(lst, transactionId, guestId, Person.Value, 1, guestRoomId, conn, paymentMethodId, paymentMethodNote, timeOfSale, _distributionPointId, isHotel, receiptNumber, terminalId, discountedSum, cashierId);

                    if (tableId.HasValue)
                    {
                        if(tableId.Value > 0)
                        {
                            StockItemService.DeleteTableItems(tableId.Value, conn, Person.Value);
                        }
                        else
                        {
                            var thisTable = _barTableService.GetAll().FirstOrDefault(x => x.TableId == 0 && x.StaffId == Person.Value);

                            if (thisTable != null)
                            {
                                StockItemService.DeleteTableItemsNoDeleteTable(thisTable.Id, conn, Person.Value);
                            }

                        }
                    }
                       

                }

                if(suspendSale)
                {
                    var suspendTime = DateTime.Now;

                    foreach (var s in lst)
                    {
                        
                        SuspendItem ss = new SuspendItem();
                        ss.IsActive = true;
                        ss.ItemName = s.Description;
                        ss.PosItemId = (int)s.Id;
                        ss.Price = s.UnitPrice;
                        ss.Quantity = s.Quantity;
                        ss.StaffId = Person.Value;
                        ss.SuspensionTime = suspendTime;

                        _suspendItemService.Create(ss);
                        //AddtoSupendList
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    double dTotal = 0;

                    double.TryParse(totalBill.ToString(), out dTotal);

                    try
                    {

                        PrintReceiptRaw(lst, dTotal, 0, (double)discountedSum, receiptNumber, chargeSeperate, guestTableNumber, printOnly, paymentMethodId);
                        //PrintReceipt(lst, dTotal, 0, 0, receiptNumber, chargeSeperate);
                    }
                    catch (Exception)
                    {
                        //throw ex;
                    }
                }

               
            }
            catch(Exception ex)
            {
                //throw ex;
            }

            if (printOnly)
            {
                if (tableId.HasValue && tableId.Value > 0)
                    return RedirectToAction("Table", new { id = tableId.Value });
                else
                    return RedirectToAction("Table", new { id = 0 });
            }

            return RedirectToAction("Index");
        }

        private void ProcessTheOrderByTableNumber(int? tableId, string kitchenNote)
        {
            var itemsList = new List<TableItem>();

            int? realTableId = null;

            var tableAlias = string.Empty;

            if(tableId.Value > 0)
            {
                itemsList = _tableItemService.GetAllEvery("BarTable,StockItem").Where(x => !x.IsActive && x.Cashier == Person.Value && x.TableId == tableId.Value).ToList();

                foreach (var ti in itemsList)
                {
                    ti.IsActive = true;
                    tableAlias = ti.BarTable.TableAlias;
                    ti.Note = kitchenNote;
                    realTableId = ti.BarTable.Id;
                    _tableItemService.Update(ti);
                }
            }
            else
            {
                itemsList = _tableItemService.GetAllEvery("BarTable,StockItem").Where(x => !x.IsActive && x.Cashier == Person.Value && x.BarTable.TableId == tableId.Value).ToList();

                foreach (var ti in itemsList)
                {
                    tableAlias = ti.BarTable.TableAlias;
                    ti.IsActive = true;
                    ti.Note = kitchenNote;
                    realTableId = ti.BarTable.Id;
                    _tableItemService.Update(ti);
                }
            }

            if (realTableId.HasValue)
            {
                _printerTableService.Create(new PrinterTable { TableId = realTableId.Value, DateTime = DateTime.Now, IsActive = true });
            }

            itemsList = itemsList.Where(x => x.StockItem.CookedFood).ToList();

            var isFullKitchenScreen = UsingFullKitchenScreen();

            if (itemsList.Count > 0 && !isFullKitchenScreen)
            {
                var pointName = CashierDistributionPointName();
                var siList = itemsList.Select(x => new POSService.Entities.StockItem { Quantity = x.Qty, Id = x.ItemId, Description = x.StockItem.Description, UnitPrice = x.StockItem.UnitPrice.Value }).ToList();
                SendListToNetworkPrinterAndPosPrinter(itemsList, tableAlias, kitchenNote, pointName);
                //SendToPosPrinter(siList, tableAlias);
            }
                
        }

        private bool UsingFullKitchenScreen()
        {
            bool isUsingFullKitchenScreen = true;

            try
            {
                var yOrn = ConfigurationManager.AppSettings["UsingKitchenPC"].ToString().ToUpper();

                if (yOrn == "NO")
                    isUsingFullKitchenScreen = false;
            }
            catch (Exception)
            {
                isUsingFullKitchenScreen = false;
            }

            return isUsingFullKitchenScreen;
        }

        private void SendListToNetworkPrinterAndPosPrinter(List<TableItem> itemsList, string tableAlias, string kitchenNote, string pointName)
        {
            PrintReceiptRaw(itemsList, tableAlias, kitchenNote, pointName);
        }

        private void PrintReceiptRaw(List<TableItem> lst, string tableAlias, string kitchenNote, string pointName)
        {
           
            var printerName = ConfigurationManager.AppSettings["NetworkPrinterName"].ToString();

            try
            {

                var grpList = lst.GroupBy(x => x.StockItem.Id).Select(x => new PrintStockItemModel
                {
                    Description = x.FirstOrDefault().StockItem.Description,
                    Quantity = x.Sum(z => z.Qty),
                    DateSold = x.FirstOrDefault().DateSold
                }).ToList();

                

                var thisUserName = User.Identity.Name;

                PrintReceiptHeaderKitchenRaw(printerName, tableAlias, DateTime.Now, thisUserName, "", pointName);

                foreach (var item in grpList)
                {
                    PrintLineItemKitchenRaw(printerName, item.Description, item.Quantity, item.DateSold.ToShortTimeString());
                }

                PrintTextLineRaw(printerName, String.Empty);

                //Embed 'center' alignment tag on front of string below to have it printed in the center of the receipt.
                PrintTextLineRaw(printerName, kitchenNote);

                //Added in these blank lines because RecLinesToCut seems to be wrong on my printer and
                //these extra blank lines ensure the cut is after the footer ends.
                PrintTextLineRaw(printerName, String.Empty);
                PrintTextLineRaw(printerName, String.Empty);

               

                //Print 'advance and cut' escape command.
                RawPrinterHelper.FullCut(printerName);

            }
            catch (Exception)
            {
                //throw ex;
            }
            finally
            {
               
            }
        }


        private void PrintReceipt(List<TableItem> lst, string tableAlias, string kitchenNote, string pointName)
        {
            PosPrinter printer = null;

            try
            {

                var grpList = lst.GroupBy(x => x.StockItem.Id).Select(x => new PrintStockItemModel
                {
                    Description = x.FirstOrDefault().StockItem.Description, 
                    Quantity = x.Sum(z => z.Qty), DateSold = x.FirstOrDefault().DateSold }).ToList();

                printer = GetReceiptPrinter();

                ConnectToPrinter(printer);

                var thisUserName = User.Identity.Name;

                PrintReceiptHeaderKitchen(printer,tableAlias, DateTime.Now, thisUserName, "", pointName);

                foreach (var item in grpList)
                {
                    PrintLineItemKitchen(printer, item.Description, item.Quantity, item.DateSold.ToShortTimeString());
                }

                PrintTextLine(printer, String.Empty);

                //Embed 'center' alignment tag on front of string below to have it printed in the center of the receipt.
                PrintTextLine(printer, System.Text.ASCIIEncoding.ASCII.GetString(new byte[] { 27, (byte)'|', (byte)'c', (byte)'A' }) + kitchenNote);

                //Added in these blank lines because RecLinesToCut seems to be wrong on my printer and
                //these extra blank lines ensure the cut is after the footer ends.
                PrintTextLine(printer, String.Empty);
                PrintTextLine(printer, String.Empty);
                PrintTextLine(printer, String.Empty);
                PrintTextLine(printer, String.Empty);
                PrintTextLine(printer, String.Empty);

                //Print 'advance and cut' escape command.
                PrintTextLine(printer, System.Text.ASCIIEncoding.ASCII.GetString(new byte[] { 27, (byte)'|', (byte)'1', (byte)'0', (byte)'0', (byte)'P', (byte)'f', (byte)'P' }));

            }
            catch (Exception)
            {
                //throw ex;
            }
            finally
            {
                if (printer != null)
                    DisconnectFromPrinter(printer);
            }
        }

        private void PrintBillNew(List<GuestOrderItem> list, int? tableId, int personId)
        {
            PrintBillOnlyNew(list, tableId, personId);
        }


        private void PrintBillOnlyNew(List<GuestOrderItem> list, int? tableId, int personId)
        {
            BarTable bt = _barTableService.GetById(tableId.Value);

            var tableList = list.Where(x => x.IsActive).ToList();

            List<POSService.Entities.StockItem> lst = new List<POSService.Entities.StockItem>();

            var totalBill = decimal.Zero;

            foreach (var ti in tableList)
            {
                var thisProduct = ProductsList.FirstOrDefault(x => x.Id == ti.ItemId);

                var price = thisProduct.UnitPrice;

                price = (thisProduct.Discounted && thisProduct.ClubPrice.HasValue && thisProduct.ClubPrice.Value > 0) ? thisProduct.ClubPrice.Value : thisProduct.UnitPrice;


                var qty = ti.Quantity;

                totalBill += (price * qty);

                var itemDescription = thisProduct.StockItemName;

                lst.Add(new POSService.Entities.StockItem { Id = thisProduct.Id, Quantity = qty, UnitPrice = price, Description = itemDescription });

            }

            SendToPosPrinter(lst, bt.TableAlias);
        }

        private int GetPaymentMethod(string pm)
        {
            if (pm.ToUpper().StartsWith("POST"))
                return (int)PaymentMethodEnum.POSTBILL;
            if (pm.StartsWith("CC"))
                return (int)PaymentMethodEnum.CreditCard;                
            else if (pm.StartsWith("CASH"))
                return (int)PaymentMethodEnum.Cash;
            else if (pm.StartsWith("COM"))
                return (int)PaymentMethodEnum.COMPLIMENTARY;
            else 
                return (int)PaymentMethodEnum.Cheque;
                
        }

        private void PostToRoom(int guestId, int guestRoomId, decimal amount, int terminalId, int? paymentMethodId, string paymentMethodNote)
        {
            var guest = _guestService.GetById(guestId);

            if (ModelState.IsValid)
            {
                var guestRoom = guest.GuestRooms.FirstOrDefault(x => x.Id == guestRoomId);
                if (guestRoom.GuestRoomAccounts == null)
                    guestRoom.GuestRoomAccounts = new Collection<GuestRoomAccount>();
                var ticks = (int)DateTime.Now.Ticks;

                guestRoom.GuestRoomAccounts.Add(new GuestRoomAccount
                {
                    Amount = amount,
                    PaymentTypeId = terminalId,
                    TransactionDate = DateTime.Now,
                    TransactionId = Person.Value,
                    PaymentMethodId = paymentMethodId.HasValue ? paymentMethodId.Value : 1, PaymentMethodNote = paymentMethodNote
                });

                guest.GuestRooms.Add(guestRoom);

                _guestService.Update(guest);

                //return RedirectToAction("TopUpRestaurant", "GuestAccount", new { id = model.Guest.Id, paymentTypeId = model.PaymentTypeId, itemSaved = true });
            }
        }

        private void SendToPosPrinter(List<POSService.Entities.StockItem> lst, string tableName)
        {
            ArrayList ar = new ArrayList();
            ArrayList arSD = new ArrayList();
            ArrayList arVat = new ArrayList();


            var totalAmount = decimal.Zero;

            string strTableTime = MyPadright(tableName, 5) + MyPadright(DateTime.Now.ToShortTimeString(), 5);

            ar.Add(strTableTime);

            foreach (var si in lst)
            {
                var amount = si.UnitPrice * si.Quantity;
                totalAmount += amount;
                string str = MyPadright(si.Description, 5) + MyPadright(si.Quantity.ToString(), 5);
                ar.Add(str);
            }

            DoPrintJob(arSD, ar, arVat);
            
        }

        public static void DoPrintJob(ArrayList arShopDetails, ArrayList arItemList, ArrayList arVatChange)
        {
            var printerName = ConfigurationManager.AppSettings["NetworkPrinterName"].ToString();
            //printerName = "EPSON TM-T20II Receipt";

            try
            {
                byte[] DrawerOpen5 = { 0xA };

                char V = 'a';
                byte[] DrawerOpen = { 0x1B, Convert.ToByte(V), 1 };
                //RawPrinterHelper.DoSomeThing(printerName, DrawerOpen);

                V = '!';
                byte[] DrawerOpen1 = { 0x1B, Convert.ToByte(V), 0 };
                //RawPrinterHelper.DoSomeThing(printerName, DrawerOpen1);

                //for (int i = 0; i < arShopDetails.Count; i++)
                //{
                //    //RawPrinterHelper.SendStringToPrinter(printerName, arShopDetails[i].ToString());
                //    //RawPrinterHelper.DoSomeThing(printerName, DrawerOpen5); //LINE FEED
                //}


                V = 'd';
                byte[] DrawerOpen2 = { 0x1B, Convert.ToByte(V), 3 };
                //RawPrinterHelper.DoSomeThing(printerName, DrawerOpen2);

                V = 'a';
                byte[] DrawerOpen3 = { 0x1B, Convert.ToByte(V), 0 };
                //RawPrinterHelper.DoSomeThing(printerName, DrawerOpen3);

                V = '!';
                byte[] DrawerOpen4 = { 0x1B, Convert.ToByte(V), 1 };
                //RawPrinterHelper.DoSomeThing(printerName, DrawerOpen4);

                for (int i = 0; i < arItemList.Count; i++)
                {
                    //RawPrinterHelper.SendStringToPrinter(printerName, arItemList[i].ToString());
                    //RawPrinterHelper.DoSomeThing(printerName, DrawerOpen5); //LINE FEED
                }


                for (int i = 0; i < arVatChange.Count; i++)
                {
                    if (i == 0)
                    {
                        V = '!';
                        byte[] DrawerOpen6 = { 0x1B, Convert.ToByte(V), 17 };
                        //RawPrinterHelper.DoSomeThing(printerName, DrawerOpen6);
                    }

                    //RawPrinterHelper.SendStringToPrinter(printerName, arVatChange[i].ToString());
                    //RawPrinterHelper.DoSomeThing(printerName, DrawerOpen5); //LINE FEED


                    if (i == 0)
                    {
                        V = '!';
                        byte[] DrawerOpen7 = { 0x1B, Convert.ToByte(V), 0 };
                        //RawPrinterHelper.DoSomeThing(printerName, DrawerOpen7);
                    }
                }

                //RawPrinterHelper.DoSomeThing(printerName, DrawerOpen5); //LINE FEED
                //RawPrinterHelper.DoSomeThing(printerName, DrawerOpen5); //LINE FEED

                //RawPrinterHelper.FullCut(printerName);
                //RawPrinterHelper.OpenCashDrawer1(printerName);

            }
            catch (Exception)
            {
                // MessageBox.Show(ex.Message);
            }
        }

        private string MyPadright(string str, int len)
        {
            String str1 = new String(' ', len);
            return str + str1.ToString();
        }

        private int GetPosTerminalId(string terminal)
        {
            terminal = terminal.ToUpper();

            if (terminal == "RESTAURANT")
                return (int)RoomPaymentTypeEnum.Restuarant;

            else if (terminal == "BAR")
                return (int)RoomPaymentTypeEnum.Bar;

            else if (terminal == "LAUNDRY")
                return (int)RoomPaymentTypeEnum.Laundry;

            else if (terminal == "INTERNET")
                return (int)RoomPaymentTypeEnum.Laundry;


            return (int)RoomPaymentTypeEnum.Miscellenous;

        }

        private void PrintReceipt(System.Data.DataSet allSoldItems, string receiptNumber)
        {

            PosPrinter printer = GetReceiptPrinter();

            try
            {
                ConnectToPrinter(printer);

                string[] splitDetails = null;

                var thisUserName = User.Identity.Name;

                try
                {
                    var shopDetails = ConfigurationManager.AppSettings["SHOPDETAILS"].ToString();

                    splitDetails = shopDetails.Split('@');

                    if (splitDetails.Length != 4)
                    {
                        splitDetails = null;
                    }

                }
                catch
                {
                }

                #region PrintReceipt Twice

                for (int p = 0; p < 2; p++)
                {

                    if (splitDetails != null)
                    {
                        PrintReceiptHeader(printer, splitDetails[0].Trim(), splitDetails[1].Trim(), splitDetails[2].Trim(), splitDetails[3].Trim(), DateTime.Now, thisUserName, receiptNumber);
                    }
                    else
                    {
                        PrintReceiptHeader(printer, "ABCDEF Pte. Ltd.", "123 My Street, My City,", "My State, My Country", "012-3456789", DateTime.Now, thisUserName, receiptNumber);
                    }

                    int count = allSoldItems.Tables[0].Rows.Count;

                    double total = 0;

                    for (int i = 0; i < count; i++)
                    {
                        //SI.STOCKITEMNAME, SIA.Qty, SIA.TOTALPRICE
                        var description = allSoldItems.Tables[0].Rows[i][0].ToString();
                        var qty = int.Parse(allSoldItems.Tables[0].Rows[i][1].ToString());
                        var totalPrice = double.Parse(allSoldItems.Tables[0].Rows[i][2].ToString());
                        total += totalPrice;
                        var unitPrice = totalPrice / qty;
                        PrintLineItem(printer, description, qty, unitPrice);
                    }


                    try
                    {
                        PrintReceiptFooter(printer, total, 0, 0, "","YOUR SHIFT HAS NOW BEEN CLOSED.");
                    }
                    catch
                    {
                    }
                }

                #endregion

            }
            finally
            {
                DisconnectFromPrinter(printer);
            } 
        }

        private void PrintReceiptRawPrint(List<POSService.Entities.StockItem> lst, double total, double tax, int discount, string receiptNumber, bool? addRestaurantGuestExtraTax, int paymentMethodId)
        {
            var printer = ConfigurationManager.AppSettings["NetworkPrinterName"].ToString();

            try
            {
                var grpList = lst.GroupBy(x => x.Id).Select(x => new PrintStockItemModel { Description = x.FirstOrDefault().Description, Quantity = x.Sum(z => z.Quantity), UnitPrice = x.FirstOrDefault().UnitPrice }).ToList();



                string[] splitDetails = null;

                var thisUserName = User.Identity.Name;

                try
                {
                    var shopDetails = ConfigurationManager.AppSettings["SHOPDETAILS"].ToString();

                    splitDetails = shopDetails.Split('@');

                    if (splitDetails.Length != 4)
                    {
                        splitDetails = null;
                    }

                }
                catch (Exception)
                {
                    //throw ex;
                }

                if (splitDetails != null)
                {
                    //PrintReceiptHeaderRaw(printer, splitDetails[0].Trim(), splitDetails[1].Trim(), splitDetails[2].Trim(), splitDetails[3].Trim(), DateTime.Now, thisUserName, receiptNumber);
                    PrintReceiptHeaderRaw(printer, "THIS IS NOT A RECEIPT", "PLS ASK FOR RECEIPT", "THIS IS NOT A RECEIPT", "PLS ASK FOR RECEIPT", DateTime.Now, thisUserName, receiptNumber,"");
                }
                else
                {
                    PrintReceiptHeaderRaw(printer, "THIS IS NOT A RECEIPT", "PLS ASK FOR RECEIPT", "THIS IS NOT A RECEIPT", "PLS ASK FOR RECEIPT", DateTime.Now, thisUserName, receiptNumber,"");
                }


                foreach (var item in grpList)
                {
                    PrintLineItemRaw(printer, item.Description, item.Quantity, double.Parse(item.UnitPrice.ToString()));
                }



                var overallTotal = total;

                var taxValue = GetRestaurantTax();

                if (addRestaurantGuestExtraTax.HasValue && addRestaurantGuestExtraTax.Value)
                {
                    taxValue += GetSeperateGuestTax();
                }

                if (taxValue > 0)
                {
                    decimal newtaxValue = decimal.Divide((decimal)taxValue, 100M);
                    tax = (double)(newtaxValue * (decimal)overallTotal);
                }

                var displayTax = GetRestaurantTaxDisplayOnly();

                var anyTaxDetails = GetRestaurantTaxDetails();

                if (displayTax > 0)
                {
                    decimal newtaxValue = decimal.Divide((decimal)displayTax, 100M);
                    tax = (double)(newtaxValue * (decimal)total);
                    total = total - tax;
                }

                PrintReceiptFooterRaw(printer, total, tax, discount, anyTaxDetails, "THIS IS NOT A RECEIPT", false, paymentMethodId);

            }
            catch (Exception)
            {
                //throw ex;
            }
            finally
            {

            }
        }



        private void PrintReceiptRaw(List<POSService.Entities.StockItem> lst, double total, double tax, double discount, string receiptNumber, bool? addRestaurantGuestExtraTax, string guestTableNumber, bool printOnly, int paymentMethodId)
        {
            var printer = ConfigurationManager.AppSettings["NetworkPrinterName"].ToString();

            try
            {
                var grpList = lst.GroupBy(x => x.Id).Select(x => new PrintStockItemModel { Description = x.FirstOrDefault().Description, Quantity = x.Sum(z => z.Quantity), UnitPrice = x.FirstOrDefault().UnitPrice }).ToList();

                string[] splitDetails = null;

                var thisUserName = User.Identity.Name;

                try
                {
                    var shopDetails = ConfigurationManager.AppSettings["SHOPDETAILS"].ToString();

                    splitDetails = shopDetails.Split('@');

                    if (splitDetails.Length != 4)
                    {
                        splitDetails = null;
                    }

                }
                catch (Exception)
                {
                    //throw ex;
                }

                if (splitDetails != null)
                {
                    PrintReceiptHeaderRaw(printer, splitDetails[0].Trim(), splitDetails[1].Trim(), splitDetails[2].Trim(), splitDetails[3].Trim(), DateTime.Now, thisUserName, receiptNumber, guestTableNumber);
                }
                else
                {
                    PrintReceiptHeaderRaw(printer, "ABCDEF Pte. Ltd.", "123 My Street, My City,", "My State, My Country", "012-3456789", DateTime.Now, thisUserName, receiptNumber, guestTableNumber);
                }


                foreach (var item in grpList)
                {
                    PrintLineItemRaw(printer, item.Description, item.Quantity, double.Parse(item.UnitPrice.ToString()));
                }



                var overallTotal = total;

                var taxValue = GetRestaurantTax();

                if (addRestaurantGuestExtraTax.HasValue && addRestaurantGuestExtraTax.Value)
                {
                    taxValue = GetSeperateGuestTax();
                }

                if (taxValue > 0)
                {
                    decimal newtaxValue = decimal.Divide((decimal)taxValue, 100M);
                    tax = (double)(newtaxValue * (decimal)overallTotal);
                }

                var displayTax = GetRestaurantTaxDisplayOnly();

                var anyTaxDetails = GetRestaurantTaxDetails();

                if (displayTax > 0)
                {
                    decimal newtaxValue = decimal.Divide((decimal)displayTax, 100M);
                    tax = (double)(newtaxValue * (decimal)total);
                    total = total - tax;
                }

                PrintReceiptFooterRaw(printer, total, tax, discount, anyTaxDetails, "THANK YOU FOR YOUR PATRONAGE.", printOnly, paymentMethodId);

            }
            catch (Exception)
            {
                //throw ex;
            }
            finally
            {
                
            }
        }



        private void PrintReceipt(List<POSService.Entities.StockItem> lst, double total, double tax, int discount, string receiptNumber, bool? addRestaurantGuestExtraTax)
        {
            PosPrinter printer = null;

            try
            {
                var grpList = lst.GroupBy(x => x.Id).Select(x => new PrintStockItemModel { Description = x.FirstOrDefault().Description, Quantity = x.Sum(z => z.Quantity), UnitPrice = x.FirstOrDefault().UnitPrice }).ToList();

                printer = GetReceiptPrinter();

                ConnectToPrinter(printer);

                string[] splitDetails = null;

                var thisUserName = User.Identity.Name;

                try
                {
                    var shopDetails = ConfigurationManager.AppSettings["SHOPDETAILS"].ToString();

                    splitDetails = shopDetails.Split('@');

                    if(splitDetails.Length != 4)
                    {
                        splitDetails = null;
                    }

                }
                catch(Exception)
                {
                    //throw ex;
                }

                if(splitDetails != null)
                {
                    PrintReceiptHeader(printer, splitDetails[0].Trim(), splitDetails[1].Trim(), splitDetails[2].Trim(), splitDetails[3].Trim(), DateTime.Now, thisUserName, receiptNumber);
                }
                else
                {
                    PrintReceiptHeader(printer, "ABCDEF Pte. Ltd.", "123 My Street, My City,", "My State, My Country", "012-3456789", DateTime.Now, thisUserName, receiptNumber);
                }

                
                foreach (var item in grpList)
                {
                    PrintLineItem(printer, item.Description, item.Quantity, double.Parse(item.UnitPrice.ToString()));
                }

                

                var overallTotal = total;

                var taxValue = GetRestaurantTax();

                if (addRestaurantGuestExtraTax.HasValue && addRestaurantGuestExtraTax.Value)
                {
                    taxValue += GetSeperateGuestTax();
                }

                if (taxValue > 0)
                {
                    decimal newtaxValue = decimal.Divide((decimal)taxValue, 100M);
                    tax = (double)(newtaxValue * (decimal)overallTotal);
                }

                var displayTax = GetRestaurantTaxDisplayOnly();

                var anyTaxDetails = GetRestaurantTaxDetails();

                if(displayTax > 0)
                {
                    decimal newtaxValue = decimal.Divide((decimal)displayTax, 100M);
                    tax = (double)(newtaxValue * (decimal)total);
                    total = total - tax;
                }

                PrintReceiptFooter(printer, total, tax, discount, anyTaxDetails, "THANK YOU FOR YOUR PATRONAGE.");

            }
            catch (Exception)
            {
                //throw ex;
            }
            finally
            {
                if (printer != null)
                    DisconnectFromPrinter(printer);
            }
        }

        private void DisconnectFromPrinter(PosPrinter printer)
        {
            try
            {
                printer.Release();
                printer.Close();

            }
            catch
            {

            }
          
        }

        private void ConnectToPrinter(PosPrinter printer)
        {
            try
            {
                printer.Open();
                printer.Claim(10000);
                printer.DeviceEnabled = true;
            }
            catch
            {

            }
        }

        private PosPrinter GetReceiptPrinter()
        {
            //PosExplorer explorer = new PosExplorer();
            //return explorer.GetDevices(DeviceType.PosPrinter, DeviceCompatibilities.OposAndCompatibilityLevel1);

            PosExplorer posExplorer = null;

            try
            {

                posExplorer = new PosExplorer();
            }
            catch (Exception)
            {

                //posExplorer = new PosExplorer(this);
            }

            //var ppp = posExplorer.GetDevices(DeviceType.PosPrinter, DeviceCompatibilities.OposAndCompatibilityLevel1);
            // var pp = posExplorer.GetDevices();
            // DeviceInfo receiptPrinterDevice = posExplorer.GetDevice("EPSON TM-T20II Receipt", "EPSON TM-T20II Receipt"); //May need to change this if you don't use a logicial name or
            //use a different one.
            DeviceInfo receiptPrinterDevice = posExplorer.GetDevice(DeviceType.PosPrinter, "POSPrinter"); //May need to change this if you don't use a logicial name or//my_device
            //DeviceInfo receiptPrinterDevice = posExplorer.GetDevice(DeviceType.PosPrinter, "Microsoft PosPrinter Simulator"); //May need to change this if you don't use a logicial name or//my_device

            //DeviceInfo receiptPrinterDevice1 = posExplorer.GetDevice(DeviceType.LineDisplay, "my_device"); //May need to change this if you don't use a logicial name or//my_device
            // receiptPrinterDevice.

            return (PosPrinter)posExplorer.CreateInstance(receiptPrinterDevice);
        }

        
        private decimal GetServiceCharge()
        {
            decimal hTax = decimal.Zero;

            try
            {
                decimal.TryParse(ConfigurationManager.AppSettings["ServiceCharge"].ToString(), out hTax);
            }
            catch
            {
                hTax = decimal.Zero;
            }

            return hTax;
        }

        private void PrintReceiptFooterRaw(string printer, double subTotal, double tax, double discount, string anyTaxDetails, string footerText, bool printOnly, int paymentMethodId)
        {
            int RecLineChars = 42;

            string offSetString = new string(' ', ((RecLineChars / 2) - 4));

            var sc = GetServiceCharge();

           
            PrintTextLineRaw(printer, new string('-', (RecLineChars / 3) * 2));
            PrintTextLineRaw(printer, offSetString + String.Format("SUB-TOTAL  {0}", subTotal.ToString("#0.00")));
            PrintTextLineRaw(printer, offSetString + String.Format("TAX        {0}", tax.ToString("#0.00")));
            PrintTextLineRaw(printer, offSetString + String.Format("DISCOUNT   {0}", discount.ToString("#0.00")));

            if (sc > decimal.Zero)
            {
                PrintTextLineRaw(printer, offSetString + String.Format("SERVICE CHRG   {0}", sc.ToString("#0.00")));
            }
            
            var finalTotal = ((subTotal + tax) - (discount)).ToString("#0.00");

            if(sc > decimal.Zero)
            {
                finalTotal = finalTotal + sc;
            }

            PrintTextLineRaw(printer, offSetString + new string('-', (RecLineChars / 3)));
            PrintTextLineRaw(printer, offSetString + String.Format("TOTAL      {0}", finalTotal));
            PrintTextLineRaw(printer, offSetString + new string('-', (RecLineChars / 3)));

            if(!printOnly)
            {
                if (paymentMethodId == (int)PaymentMethodEnum.COMPLIMENTARY)
                {
                    PrintTextLineRaw(printer, offSetString + String.Format("COMPLIMENTARY -     {0}", "CLOSED"));
                }
                else
                {
                    PrintTextLineRaw(printer, offSetString + String.Format("CASHED ------     {0}", "CLOSED"));
                }
                
                PrintTextLineRaw(printer, offSetString + new string('-', (RecLineChars / 3)));
            }

            PrintTextLineRaw(printer, String.Empty);

            //Embed 'center' alignment tag on front of string below to have it printed in the center of the receipt.
            var eCentre = Convert.ToChar(27) + Convert.ToChar(97) + "1";
            //PrintTextLineRaw(printer, System.Text.ASCIIEncoding.ASCII.GetString(new byte[] { 27, (byte)'|', (byte)'c', (byte)'A' }) + footerText);
            offSetString = new string(' ', RecLineChars / 4);
            PrintTextLineRaw(printer, offSetString + footerText);

            PrintTextLineRaw(printer, String.Empty);
            PrintTextLineRaw(printer, String.Empty);

            if (!string.IsNullOrEmpty(anyTaxDetails))
            {
                PrintTextLineRaw(printer, offSetString + anyTaxDetails);
            }


            //Added in these blank lines because RecLinesToCut seems to be wrong on my printer and
            //these extra blank lines ensure the cut is after the footer ends.

            byte[] DrawerOpen5 = { 0xA };

            //PrintTextLineRaw(printer, String.Empty);
            //PrintTextLineRaw(printer, String.Empty);
            //PrintTextLineRaw(printer, String.Empty);
            //PrintTextLineRaw(printer, String.Empty);
            //PrintTextLineRaw(printer, String.Empty);

            RawPrinterHelper.DoSomeThing(printer, DrawerOpen5); //LINE FEED
            RawPrinterHelper.DoSomeThing(printer, DrawerOpen5); //LINE FEED
            RawPrinterHelper.DoSomeThing(printer, DrawerOpen5); //LINE FEED
            RawPrinterHelper.DoSomeThing(printer, DrawerOpen5); //LINE FEED
            RawPrinterHelper.DoSomeThing(printer, DrawerOpen5); //LINE FEED


            //Print 'advance and cut' escape command.
            //PrintTextLineRaw(printer, System.Text.ASCIIEncoding.ASCII.GetString(new byte[] { 27, (byte)'|', (byte)'1', (byte)'0', (byte)'0', (byte)'P', (byte)'f', (byte)'P' }));
            //PrintTextLineRaw(printer, String.Empty);
            //PrintTextLineRaw(printer, String.Empty);
            RawPrinterHelper.FullCut(printer);
            RawPrinterHelper.OpenCashDrawer1(printer);
        }


        private void PrintReceiptFooter(PosPrinter printer, double subTotal, double tax, double discount,string anyTaxDetails, string footerText)
        {
            string offSetString = new string(' ', printer.RecLineChars / 2);

            PrintTextLine(printer, new string('-', (printer.RecLineChars / 3) * 2));
            PrintTextLine(printer, offSetString + String.Format("SUB-TOTAL  {0}", subTotal.ToString("#0.00")));
            PrintTextLine(printer, offSetString + String.Format("TAX        {0}", tax.ToString("#0.00")));
            PrintTextLine(printer, offSetString + String.Format("DISCOUNT   {0}", discount.ToString("#0.00")));
            PrintTextLine(printer, offSetString + new string('-', (printer.RecLineChars / 3)));
            PrintTextLine(printer, offSetString + String.Format("TOTAL      {0}", ((subTotal + tax) - (discount)).ToString("#0.00")));
            PrintTextLine(printer, offSetString + new string('-', (printer.RecLineChars / 3)));
            PrintTextLine(printer, String.Empty);

            //Embed 'center' alignment tag on front of string below to have it printed in the center of the receipt.
            PrintTextLine(printer, System.Text.ASCIIEncoding.ASCII.GetString(new byte[] { 27, (byte)'|', (byte)'c', (byte)'A' }) + footerText);
            PrintTextLine(printer, String.Empty);
            PrintTextLine(printer, String.Empty);

            if (!string.IsNullOrEmpty(anyTaxDetails))
            {
                PrintTextLine(printer, offSetString + anyTaxDetails);
            }


            //Added in these blank lines because RecLinesToCut seems to be wrong on my printer and
            //these extra blank lines ensure the cut is after the footer ends.
            PrintTextLine(printer, String.Empty);
            PrintTextLine(printer, String.Empty);
            PrintTextLine(printer, String.Empty);
            PrintTextLine(printer, String.Empty);
            PrintTextLine(printer, String.Empty);

            //Print 'advance and cut' escape command.
            PrintTextLine(printer, System.Text.ASCIIEncoding.ASCII.GetString(new byte[] { 27, (byte)'|', (byte)'1', (byte)'0', (byte)'0', (byte)'P', (byte)'f', (byte)'P' }));
        }

        private void PrintLineItemRaw(string printer, string itemCode, int quantity, double unitPrice)
        {
            PrintTextRaw(printer, TruncateAt(itemCode.PadRight(11), 11));
            PrintTextRaw(printer, TruncateAt(quantity.ToString("#0.00").PadLeft(9), 9));
            PrintTextRaw(printer, TruncateAt(unitPrice.ToString("#0.00").PadLeft(10), 10));
            PrintTextLineRaw(printer, TruncateAt((quantity * unitPrice).ToString("#0.00").PadLeft(10), 10));
        }

        private void PrintLineItem(PosPrinter printer, string itemCode, int quantity, double unitPrice)
        {
            PrintText(printer, TruncateAt(itemCode.PadRight(11), 11));
            PrintText(printer, TruncateAt(quantity.ToString("#0.00").PadLeft(9), 9));
            PrintText(printer, TruncateAt(unitPrice.ToString("#0.00").PadLeft(10), 10));
            PrintTextLine(printer, TruncateAt((quantity * unitPrice).ToString("#0.00").PadLeft(10), 10));
        }

        private void PrintLineItemKitchen(PosPrinter printer, string itemCode, int quantity, string time)
        {
            PrintText(printer, TruncateAt(itemCode.PadRight(31), 31));
            PrintTextLine(printer, TruncateAt(quantity.ToString("#0.00").PadLeft(9), 9));
            //PrintText(printer, TruncateAt(time.PadLeft(10), 10));
            //PrintTextLine(printer, TruncateAt((quantity).ToString("#0.00").PadLeft(10), 10));
        }

        private void PrintLineItemKitchenRaw(string printer, string itemCode, int quantity, string time)
        {
            PrintTextRaw(printer, TruncateAt(itemCode.PadRight(31), 31));
            PrintTextLineRaw(printer, TruncateAt(quantity.ToString("#0.00").PadLeft(9), 9));
            //PrintText(printer, TruncateAt(time.PadLeft(10), 10));
            //PrintTextLine(printer, TruncateAt((quantity).ToString("#0.00").PadLeft(10), 10));
        }

        private void PrintReceiptHeaderKitchenRaw(string printer, string tableAlias, DateTime dateTime,
           string cashierName, string receiptNumber, string pointName)
        {
            int RecLineChars = 42;
            PrintTextLineRaw(printer, new string('-', RecLineChars / 2));
            PrintTextLineRaw(printer, String.Format("DATE : {0}", dateTime.ToShortDateString()));
            PrintTextLineRaw(printer, String.Format("CASHIER : {0}", cashierName));
            PrintTextLineRaw(printer, String.Format("TABLE NO. : {0}", tableAlias));
            PrintTextLineRaw(printer, String.Format("LOCATION : {0}", pointName));
            PrintTextLineRaw(printer, new string('=', RecLineChars));
            PrintTextLineRaw(printer, String.Empty);

        }

        private void PrintReceiptHeaderKitchen(PosPrinter printer, string tableAlias, DateTime dateTime,
            string cashierName, string receiptNumber, string pointName)
        {

            PrintTextLine(printer, new string('-', printer.RecLineChars / 2));
            PrintTextLine(printer, String.Format("DATE : {0}", dateTime.ToShortDateString()));
            PrintTextLine(printer, String.Format("CASHIER : {0}", cashierName));
            PrintTextLine(printer, String.Format("TABLE NO. : {0}", tableAlias));
            PrintTextLine(printer, String.Format("LOCATION : {0}", pointName));

            //PrintTextLine(printer, String.Empty);
            //PrintText(printer, "Item             ");
            //PrintText(printer, "Qty  ");
            //PrintText(printer, "Order Time ");
            //PrintTextLine(printer, "Total      ");
            PrintTextLine(printer, new string('=', printer.RecLineChars));
            PrintTextLine(printer, String.Empty);

        }

        private void PrintReceiptHeaderRaw(string printer, string companyName, string addressLine1, string addressLine2, string taxNumber, DateTime dateTime,
           string cashierName, string receiptNumber, string guestTableNumber)
        {
            int RecLineChars = 42;
            PrintTextLineRaw(printer, companyName);
            PrintTextLineRaw(printer, addressLine1);
            PrintTextLineRaw(printer, addressLine2);
            PrintTextLineRaw(printer, taxNumber);
            PrintTextLineRaw(printer, new string('-', RecLineChars));
            PrintTextLineRaw(printer, String.Empty);
            PrintTextLineRaw(printer, String.Format("DATE : {0}", dateTime.ToString()));
            PrintTextLineRaw(printer, String.Format("CASHIER : {0}", cashierName));
            PrintTextLineRaw(printer, String.Format("RECEIPT NO. : {0}", receiptNumber));
            PrintTextLineRaw(printer, String.Format("TABLE : {0}", guestTableNumber));

            PrintTextLineRaw(printer, String.Empty);
            PrintTextRaw(printer, "Item             ");
            PrintTextRaw(printer, "Qty  ");
            PrintTextRaw(printer, "Unit Price ");
            PrintTextRaw(printer, "Total      ");
            PrintTextLineRaw(printer, String.Empty);
            PrintTextLineRaw(printer, new string('=', RecLineChars));
            PrintTextLineRaw(printer, String.Empty);

        }

        private void PrintReceiptHeader(PosPrinter printer, string companyName, string addressLine1, string addressLine2, string taxNumber, DateTime dateTime,
            string cashierName, string receiptNumber)
        {
            PrintTextLine(printer, companyName);
            PrintTextLine(printer, addressLine1);
            PrintTextLine(printer, addressLine2);
            PrintTextLine(printer, taxNumber);
            PrintTextLine(printer, new string('-', printer.RecLineChars / 2));
            PrintTextLine(printer, String.Format("DATE : {0}", dateTime.ToShortDateString()));
            PrintTextLine(printer, String.Format("CASHIER : {0}", cashierName));
            PrintTextLine(printer, String.Format("RECEIPT NO. : {0}", receiptNumber));
            PrintTextLine(printer, String.Empty);
            PrintText(printer, "Item             ");
            PrintText(printer, "Qty  ");
            PrintText(printer, "Unit Price ");
            PrintTextLine(printer, "Total      ");
            PrintTextLine(printer, new string('=', printer.RecLineChars));
            PrintTextLine(printer, String.Empty);

        }

        private void PrintTextRaw(string printer, string text)
        {
            
            int RecLineChars = 42;
            string eNmlText = "";// = Convert.ToChar(27) + "!" + Convert.ToChar(0);
            text = eNmlText + text;

            if (text.Length <= RecLineChars)
                RawPrinterHelper.SendStringToPrinter(printer, text); //Print text
            else if (text.Length > RecLineChars)
                RawPrinterHelper.SendStringToPrinter(printer, TruncateAt(text, RecLineChars));//Print exactly as many characters as the printer allows, truncating the rest.
        }

        private void PrintTextLineRaw(string printer, string text)
        {
            
            string eNmlText = Convert.ToChar(27) + "!" + Convert.ToChar(0);
            text = eNmlText + text;
            int RecLineChars = 42;
            if (text.Length < RecLineChars)
                RawPrinterHelper.SendStringToPrinter(printer, text + Environment.NewLine); //Print text //Print text, then a new line character.
            else if (text.Length > RecLineChars)
                RawPrinterHelper.SendStringToPrinter(printer, TruncateAt(text, RecLineChars)); //Print exactly as many characters as the printer allows, truncating the rest, no new line character (printer will probably auto-feed for us)
            else if (text.Length == RecLineChars)
                RawPrinterHelper.SendStringToPrinter(printer, text + Environment.NewLine); //Print text, no new line character, printer will probably auto-feed for us.
        }

        private void allPosChar()
        {
            var eClear = Convert.ToChar(27) + "@";
            var eCentre = Convert.ToChar(27) + Convert.ToChar(97) + "1";
            var eLeft = Convert.ToChar(27) + Convert.ToChar(97) + "0";
            var eRight = Convert.ToChar(27) + Convert.ToChar(97) + "2";
            var eDrawer = eClear + Convert.ToChar(27) + "p" + Convert.ToChar(0) + ".}";
            var eCut = Convert.ToChar(27) + "i" + System.Environment.NewLine;
            var eSmlText = Convert.ToChar(27) + "!" + Convert.ToChar(1);
            var eNmlText = Convert.ToChar(27) + "!" + Convert.ToChar(0);
            var eInit = eNmlText + Convert.ToChar(13) + Convert.ToChar(27) + "c6" + Convert.ToChar(1) + Convert.ToChar(27) + "R3" + System.Environment.NewLine;
            var eBigCharOn = Convert.ToChar(27) + "!" + Convert.ToChar(56);
            var eBigCharOff = Convert.ToChar(27) + "!" + Convert.ToChar(0);
        }

        private void PrintText(PosPrinter printer, string text)
        {
            if (text.Length <= printer.RecLineChars)
                printer.PrintNormal(PrinterStation.Receipt, text); //Print text
            else if (text.Length > printer.RecLineChars)
                printer.PrintNormal(PrinterStation.Receipt, TruncateAt(text, printer.RecLineChars)); //Print exactly as many characters as the printer allows, truncating the rest.
        }

        private void PrintTextLine(PosPrinter printer, string text)
        {
            if (text.Length < printer.RecLineChars)
                printer.PrintNormal(PrinterStation.Receipt, text + Environment.NewLine); //Print text, then a new line character.
            else if (text.Length > printer.RecLineChars)
                printer.PrintNormal(PrinterStation.Receipt, TruncateAt(text, printer.RecLineChars)); //Print exactly as many characters as the printer allows, truncating the rest, no new line character (printer will probably auto-feed for us)
            else if (text.Length == printer.RecLineChars)
                printer.PrintNormal(PrinterStation.Receipt, text + Environment.NewLine); //Print text, no new line character, printer will probably auto-feed for us.
        }

        private string TruncateAt(string text, int maxWidth)
        {
            string retVal = text;
            if (text.Length > maxWidth)
                retVal = text.Substring(0, maxWidth);

            return retVal;
        }
    }

}