using BarAndRestaurantMate.Helpers.Enums;
using BarAndRestaurantMate.Models;
using HotelMateWeb.Dal.DataCore;
using HotelMateWeb.Services.Core;
using HotelMateWeb.Services.ServiceApi;
using Microsoft.PointOfService;
using POSService;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using BarAndRestaurantMate.Helpers;
using BarAndRestaurantMate.Extensions;
using POSService.Entities;
using System.Net.Mail;
using System.Net;

namespace BarAndRestaurantMate.Controllers
{
    public class PosController : Controller
    {
        private  IGuestService _guestService;
        private  IPersonService _personService;
        private  IPOSItemService _posItemService;
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



        public PosController()
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


        public ActionResult DeleteEmptyTables()
        {
            var guestGroupByTablesAll = _guestOrderService.GetAll("Guest,BarTable,BarTable.Person").Where(x => x.IsActive).ToList();

            foreach (var t in guestGroupByTablesAll)
            {
                var actualGuestOrder = t;
                DeleteTable(actualGuestOrder);
            }

            var quickSale = _barTableService.GetAll().Where(x => x.TableId == 0 && x.TableItems.Count == 0 && x.StaffId == Person.Value).ToList();

            foreach(var q in quickSale)
            {
                try
                {
                    _barTableService.Delete(q);
                }
                catch
                {
                }
            }

            return RedirectToAction("IndexNew");
        }


        private void DeleteTable(GuestOrder actualGuestOrder)
        {
            if (actualGuestOrder.BarTable != null)
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


        public ActionResult ViewTables()
        {
            HotelMenuModel hmm = new HotelMenuModel();

            hmm.CashierCanOpenTable = true;

            hmm.ClubTime = false;

            var lst = new List<POSService.Entities.Guest>();

            if (System.Configuration.ConfigurationManager.AppSettings["UseHotelCustomers"].ToString() == "1")
            {
                lst = GuestsList.ToList();

                var ids = lst.Select(x => x.Id).ToList();

                var lstNew = _guestService.GetAll(1).Where(x => ids.Contains(x.Id) && !x.IsChild).ToList();

                var realIds = lstNew.Select(x => x.Id).ToList();

                lst.Insert(0, new POSService.Entities.Guest { Id = 0, FullName = "Outside Customer", RoomNumber = "Outside Customer", GuestRoomId = 0 });

                hmm.CurrentGuests = lst.Where(x => realIds.Contains((int)x.Id)).ToList();

                hmm.CurrentGuests.ToList().ForEach(x => x.RoomNumber = GetFullName(x, lstNew));
            }
            else
            {
                //BusinessAccountList
                var accountlst = BusinessAccountList.ToList();

                accountlst.Insert(0, new POSService.Entities.BusinessAccount { Id = 0, Name = "Quick Sale", });

                hmm.CurrentBusinessAccounts = accountlst;

                hmm.CurrentGuests = null;
            }

            var openTables = _barTableService.GetAll().Where(x => x.IsActive && x.Person.PersonID == Person.Value).OrderByDescending(x => x.CreatedDate).ToList();

            hmm.AllOpenTables = openTables;

            return View(hmm);
        }

        public ActionResult GetItemByBarCode(string code, int? tableId)
        {
            var allPosItems = _posItemService.GetAll();

            var _distributionPointId = CashierDistributionPointId().Value;

            var product = allPosItems.FirstOrDefault(x => x.Remaining > 0 && x.DistributionPointId == _distributionPointId && x.StockItem.Barcode.ToUpper() == code.ToUpper());

            int? scode = 0;

            if(product != null)
            {
                scode = product.ItemId;
            }

            bool? ignore = false;

            Tax_Rate t = new Tax_Rate { id = 1, rate = 10, type = 2 };

            var actualItemPrice = decimal.Zero;

            if (!tableId.HasValue)
            {
                tableId = 0;
                ignore = false;
            }
            else
            {
                var xeroTable = _barTableService.GetById(tableId.Value);

                if(xeroTable != null && xeroTable.TableId == 0)
                {
                    tableId = 0;
                    ignore = false;
                }
            }

            if (product != null && product.Remaining > 0 && (ignore.HasValue && !ignore.Value))
            {
                if (tableId.HasValue && tableId.Value > 0)
                {
                    var allEvery = _tableItemService.GetAllEvery("").ToList();

                    var existingTable = allEvery.FirstOrDefault(x => x.TableId == tableId && x.ItemId == scode.Value && x.Cashier == Person.Value && !x.IsActive);

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
                            goi.Price = product.StockItem.UnitPrice.Value;
                            goi.Quantity = 1;
                            goi.ItemId = scode.Value;
                            goi.WaitreesCanSee = true;
                            goi.GuestOrderId = go.Id;
                            goi = _guestOrderItemService.Create(goi);

                            TableItem ti = new TableItem();
                            ti.DateSold = DateTime.Now;
                            ti.Cashier = Person.Value;
                            ti.ItemId = scode.Value;
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
                        var allEvery = _tableItemService.GetAllEvery("").ToList();

                        var existingTableItem = allEvery.FirstOrDefault(x => x.BarTable.TableId == tableId && x.ItemId == scode.Value && x.Cashier == Person.Value && !x.IsActive);

                        if (existingTableItem != null)
                        {
                            existingTableItem.Qty = existingTableItem.Qty + 1;
                            _tableItemService.Update(existingTableItem);
                        }
                        else
                        {
                            TableItem ti = new TableItem();
                            ti.DateSold = DateTime.Now;
                            ti.Cashier = Person.Value;
                            ti.ItemId = scode.Value;
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
                                ti.ItemId = scode.Value;
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
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DoTransferTill()
        {
            HotelMenuModel hmm = new HotelMenuModel();

            hmm.CashierCanOpenTable = true;

            hmm.ClubTime = false;

            var lst = new List<POSService.Entities.Guest>();

            if (System.Configuration.ConfigurationManager.AppSettings["UseHotelCustomers"].ToString() == "1")
            {
                lst = GuestsList.ToList();

                var ids = lst.Select(x => x.Id).ToList();

                var lstNew = _guestService.GetAll(1).Where(x => ids.Contains(x.Id) && !x.IsChild).ToList();

                var realIds = lstNew.Select(x => x.Id).ToList();

                lst.Insert(0, new POSService.Entities.Guest { Id = 0, FullName = "Outside Customer", RoomNumber = "Outside Customer", GuestRoomId = 0 });

                hmm.CurrentGuests = lst.Where(x => realIds.Contains((int)x.Id)).ToList();

                hmm.CurrentGuests.ToList().ForEach(x => x.RoomNumber = GetFullName(x, lstNew));
            }
            else
            {
                //BusinessAccountList
                var accountlst = BusinessAccountList.ToList();

                accountlst.Insert(0, new POSService.Entities.BusinessAccount { Id = 0, Name = "Quick Sale", });

                hmm.CurrentBusinessAccounts = accountlst;

                hmm.CurrentGuests = null;
            }

            var personId = Person.Value;

            hmm.AvailableCashiers = CashiersList.Where(x => x.PersonID != personId).Select(x => new SelectListItem { Text = x.DisplayName, Value = x.PersonID.ToString(), Selected = x.PersonID ==  0 });

            return View(hmm);
        }

        //test

        public ActionResult OpenTable()
        {
            HotelMenuModel hmm = new HotelMenuModel();

            hmm.CashierCanOpenTable = true;

            hmm.ClubTime = false;

            var lst = new List<POSService.Entities.Guest>();

            if (System.Configuration.ConfigurationManager.AppSettings["UseHotelCustomers"].ToString() == "1")
            {
                lst = GuestsList.ToList();

                var ids = lst.Select(x => x.Id).ToList();

                var lstNew = _guestService.GetAll(1).Where(x => ids.Contains(x.Id) && !x.IsChild).ToList();

                var realIds = lstNew.Select(x => x.Id).ToList();

                lst.Insert(0, new POSService.Entities.Guest { Id = 0, FullName = "Outside Customer", RoomNumber = "Outside Customer", GuestRoomId = 0 });

                hmm.CurrentGuests = lst.Where(x => realIds.Contains((int)x.Id)).ToList();

                hmm.CurrentGuests.ToList().ForEach(x => x.RoomNumber = GetFullName(x, lstNew));
            }
            else
            {
                //BusinessAccountList
                var accountlst = BusinessAccountList.ToList();

                accountlst.Insert(0, new POSService.Entities.BusinessAccount { Id = 0, Name = "Quick Sale", });

                hmm.CurrentBusinessAccounts = accountlst;

                hmm.CurrentGuests = null;
            }

            var personId = Person.Value;

            var tableNumbers = "1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60,61,62,63,64,65,66,67,68,69,70";

            try
            {
                tableNumbers = System.Configuration.ConfigurationManager.AppSettings["TableNumbers"].ToString();
            }
            catch
            {
                tableNumbers = "1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60,61,62,63,64,65,66,67,68,69,70";

            }

            var tables = tableNumbers.Split(',').ToList();

            var guestTablesAll = _guestOrderService.GetAll("Guest,BarTable").Where(x => x.IsActive).ToList();

            var guestTables = guestTablesAll.Where(x => !x.PersonId.HasValue).ToList();

            var tablesWithCustomers = guestTables.Select(x => x.BarTable.TableId).ToList();

            var availableTables = guestTables.Select(x => x.BarTable.TableId.ToString()).ToList();

            

            hmm.AvailableTables = availableTables;

            hmm.MyTables = guestTablesAll.Where(x => x.PersonId == Person.Value).ToList();

            hmm.AvailableTablesSelectList = GetAvailableTables(null, availableTables);

            var noneTable = guestTablesAll.Distinct(new GuestOrderComparer()).Select(x => x.BarTable.TableId.ToString()).ToList();

            var openTables = tables.Except(noneTable).ToList();

            var personIds = guestTablesAll.Select(x => x.Guest.PersonId).ToList();

            hmm.CanSelectList = GetAvailableTables(null, openTables);

            hmm.CanSelectListExistingTable = GetAvailableTables(null, noneTable);

            hmm.CanSelectListPersons = GetAvailablePersons(null, personIds);

            hmm.CanSelectListPersonsExistingTable = hmm.CanSelectListPersons;

            return View(hmm);
        }

        private IEnumerable<SelectListItem> GetAvailablePersonsStaff(int? selectedId, List<Person> persons)
        {
            //var persons = _personService.GetAll(HotelID).Where(x => !guestIds.Contains(x.PersonID) && x.Email.ToUpper() != "GUEST" && x.PersonTypeId == (int)PersonTypeEnum.Guest).ToList();

            if (!selectedId.HasValue)
                selectedId = 0;

            var selectModels = new List<SelectListModel>();

            try
            {
                selectModels = persons.Select(x => new SelectListModel { Name = x.FirstName + " - " + x.Email, Id = x.PersonID }).ToList();
            }
            catch
            {
                selectModels = new List<SelectListModel>();
            }

            //selectModels.Insert(0, new SelectListModel { Name = "-- Anonymous --", Id = 0 });

            return selectModels.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString(), Selected = x.Id == selectedId });
        }

        private IEnumerable<SelectListItem> GetAvailablePersons(int? selectedId, List<int?> guestIds)
        {
            var persons = _personService.GetAll(1).Where(x => !guestIds.Contains(x.PersonID) && x.Email.ToUpper() != "GUEST" && x.PersonTypeId == (int)PersonTypeEnum.Guest).ToList();

            if (!selectedId.HasValue)
                selectedId = 0;

            var selectModels = new List<SelectListModel>();

            try
            {
                selectModels = persons.Select(x => new SelectListModel { Name = x.FirstName + " - " + x.Email, Id = x.PersonID }).ToList();
            }
            catch
            {
                selectModels = new List<SelectListModel>();
            }

            //selectModels.Insert(0, new SelectListModel { Name = "-- Anonymous --", Id = 0 });

            return selectModels.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString(), Selected = x.Id == selectedId });
        }

        private IEnumerable<SelectListItem> GetAvailableTablesNew(int? selectedId, List<BarTable> tables)
        {
            if (!selectedId.HasValue)
                selectedId = 0;

            var selectModels = new List<SelectListModel>();

            try
            {
                selectModels = tables.Select(x => new SelectListModel { Name = x.TableAlias, Id = x.Id }).ToList();
            }
            catch
            {
                selectModels = new List<SelectListModel>();
            }

            return selectModels.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString(), Selected = x.Id == selectedId });
        }

        private IEnumerable<SelectListItem> GetAvailableTables(int? selectedId, List<string> tables)
        {
            if (!selectedId.HasValue)
                selectedId = 0;

            var selectModels = new List<SelectListModel>();

            try
            {
                selectModels = tables.Where(x => x != "0").Select(x => new SelectListModel { Name = x, Id = int.Parse(x) }).ToList();
            }
            catch
            {
                selectModels = new List<SelectListModel>();
            }

            //selectModels.Insert(0, new SelectListModel { Name = "-- Please Select --", Id = 0 });
            return selectModels.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString(), Selected = x.Id == selectedId });
        }

        public ActionResult IndexNew(int? tableId)
        {
            IndexViewModel vm = new IndexViewModel();

            if(tableId.HasValue)
            {
                vm.TableId = tableId.Value;
            }
            else
            {
                tableId = 0;
                vm.TableId = tableId.Value;
            }

            var lst = new List<POSService.Entities.Guest>();

            if (System.Configuration.ConfigurationManager.AppSettings["UseHotelCustomers"].ToString() == "1")
            {
                lst = GuestsList.ToList();

                var ids = lst.Select(x => x.Id).ToList();

                var lstNew = _guestService.GetAll(1).Where(x => ids.Contains(x.Id) && !x.IsChild).ToList();

                var realIds = lstNew.Select(x => x.Id).ToList();

                lst.Insert(0, new POSService.Entities.Guest { Id = 0, FullName = "Outside Customer", RoomNumber = "Outside Customer", GuestRoomId = 0 });

                vm.CurrentGuests = lst.Where(x => realIds.Contains((int)x.Id)).ToList();

                vm.CurrentGuests.ToList().ForEach(x => x.RoomNumber = GetFullName(x, lstNew));

                lst = vm.CurrentGuests.ToList();

                lst.Insert(0, new POSService.Entities.Guest { Id = 0, FullName = "Outside Customer", RoomNumber = "Outside Customer", GuestRoomId = 0 });

                vm.CurrentGuests = lst;

            }
            else
            {
                //BusinessAccountList
                var accountlst = BusinessAccountList.ToList();

                accountlst.Insert(0, new POSService.Entities.BusinessAccount { Id = 0, Name = "Quick Sale", });

                vm.CurrentBusinessAccounts = accountlst;

                vm.CurrentGuests = null;

            }
            

            //Path.Combine(Server.MapPath("~/ProductProfile/Initial")
            //var path = Path.Combine(Server.MapPath("~/Products/Small/"));
            //DirectoryInfo folder = new DirectoryInfo(path);
            //var files = folder.GetFiles("*.*", SearchOption.AllDirectories);
            //foreach(var f in files)
            //{
            //    ResizeImage(f.FullName, f.FullName, 120, 120, false);
            //}
            return View(vm);
        }

        public ActionResult Index(int? tableId)
        {
            IndexViewModel vm = new IndexViewModel();

            if (tableId.HasValue)
            {
                vm.TableId = tableId.Value;
            }
            else
            {
                tableId = 0;
                vm.TableId = tableId.Value;
            }

            var lst = new List<POSService.Entities.Guest>();

            if (System.Configuration.ConfigurationManager.AppSettings["UseHotelCustomers"].ToString() == "1")
            {
                lst = GuestsList.ToList();

                var ids = lst.Select(x => x.Id).ToList();

                var lstNew = _guestService.GetAll(1).Where(x => ids.Contains(x.Id) && !x.IsChild).ToList();

                var realIds = lstNew.Select(x => x.Id).ToList();

                lst.Insert(0, new POSService.Entities.Guest { Id = 0, FullName = "Outside Customer", RoomNumber = "Outside Customer", GuestRoomId = 0 });

                vm.CurrentGuests = lst.Where(x => realIds.Contains((int)x.Id)).ToList();

                vm.CurrentGuests.ToList().ForEach(x => x.RoomNumber = GetFullName(x, lstNew));

                lst = vm.CurrentGuests.ToList();

                lst.Insert(0, new POSService.Entities.Guest { Id = 0, FullName = "Outside Customer", RoomNumber = "Outside Customer", GuestRoomId = 0 });

                vm.CurrentGuests = lst;
            }
            else
            {
                //BusinessAccountList
                var accountlst = BusinessAccountList.ToList();

                accountlst.Insert(0, new POSService.Entities.BusinessAccount { Id = 0, Name = "Quick Sale", });

                vm.CurrentBusinessAccounts = accountlst;

                vm.CurrentGuests = null;

            }


            //Path.Combine(Server.MapPath("~/ProductProfile/Initial")
            //var path = Path.Combine(Server.MapPath("~/Products/Small/"));
            //DirectoryInfo folder = new DirectoryInfo(path);
            //var files = folder.GetFiles("*.*", SearchOption.AllDirectories);
            //foreach(var f in files)
            //{
            //    ResizeImage(f.FullName, f.FullName, 120, 120, false);
            //}
            return View("IndexNew", vm);
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

       

        private int? CashierDistributionPointId()
        {
            int hTax = 0;

            try
            {
                int.TryParse(ConfigurationManager.AppSettings["DistributionPointId"].ToString(), out hTax);
            }
            catch
            {
                hTax = 0;
            }

            return hTax;
        }

        private IEnumerable<POSService.Entities.StockItem> _products;
        private IEnumerable<POSService.Entities.Category> _categories;
        private IEnumerable<POSService.Entities.Guest> _guests;
        private IEnumerable<POSService.Entities.BusinessAccount> _businessAccount;
        private IEnumerable<HotelMateWeb.Dal.DataCore.Person> _cashiers;

        public IEnumerable<POSService.Entities.StockItem> ProductsList
        {
            get
            {
                if (_products != null)
                    return _products;
                else
                {
                    _products = StockItemService.GetStockItemsPOS();
                    return _products;
                }
            }
            set
            {
                _products = StockItemService.GetStockItemsPOS();
            }
        }

        private string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        [HttpPost]
        public ActionResult AddProduct(int? code, int? tableId, bool? ignore, string name, string price, int? product_id, string number )//name: name1, price: price1, product_id: id, number: number, registerid:
        {

            code = product_id;

            var _distributionPointId = CashierDistributionPointId().Value;

            Tax_Rate t = new Tax_Rate { id = 1, rate = 10, type = 2 };

            var actualItemPrice = decimal.Zero;

            if (!tableId.HasValue)
            {
                tableId = 0;
                ignore = false;
            }
            else
            {
                var xeroTable = _barTableService.GetById(tableId.Value);

                if (xeroTable != null && xeroTable.TableId == 0)
                {
                    tableId = 0;
                    ignore = false;
                }
            }

            var allPosItems = _posItemService.GetAll();

            var product = allPosItems.FirstOrDefault(x => x.Remaining > 0 && x.DistributionPointId == _distributionPointId && x.ItemId == code);

            if (product != null && product.Remaining > 0 && (ignore.HasValue && !ignore.Value))
            {
                if (tableId.HasValue && tableId.Value > 0)
                {
                    var allEvery = _tableItemService.GetAllEvery("").ToList();

                    var existingTable = allEvery.FirstOrDefault(x => x.TableId == tableId && x.ItemId == code.Value && x.Cashier == Person.Value && !x.IsActive);

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
                            goi.Price = product.StockItem.UnitPrice.Value;
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
                        var allEvery = _tableItemService.GetAllEvery("").ToList();

                        var existingTableItem = allEvery.FirstOrDefault(x => x.BarTable.TableId == tableId && x.ItemId == code.Value && x.Cashier == Person.Value && !x.IsActive);

                        if(existingTableItem != null)
                        {
                            existingTableItem.Qty = existingTableItem.Qty + 1;
                            _tableItemService.Update(existingTableItem);
                        }
                        else
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

            if(product != null)
            {
                var text = LeftAlign(product.StockItem.Description, product.StockItem.Description.Length + 5) + RightAlign(product.StockItem.UnitPrice.Value.ToString("0.00"), 12);
                ShowOnPoleDisplay(text);
            }
            

            return Json(new
            {
                //price = (product.Discounted && product.ClubPrice.HasValue && product.ClubPrice.Value > 0) ? product.ClubPrice.Value : product.UnitPrice,
                //name = product.StockItemName,
                //code = product.Id.ToString(),
                //tax_rate = t,
                //available = product.TotalQuantity
            }, JsonRequestBehavior.AllowGet);
        }

        private LineDisplay GetPoleDisplay()
        {
            PosExplorer posExplorer = null;

            try
            {
                posExplorer = new PosExplorer();
                DeviceInfo poleDisplay = posExplorer.GetDevice(DeviceType.LineDisplay, "LineDisplay"); //May need to change this if you don't use a logicial name or//my_device
                return (LineDisplay)posExplorer.CreateInstance(poleDisplay);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void DisconnectFromDisplay(LineDisplay lineDisplay)
        {
            try
            {
                lineDisplay.Release();
                lineDisplay.Close();
            }
            catch
            {

            }

        }

        private void ConnectToDisplay(LineDisplay lineDisplay)
        {
            try
            {
                lineDisplay.Open();
                lineDisplay.Claim(10000);
                lineDisplay.DeviceEnabled = true;
            }
            catch
            {
            }
        }

        public void ScrollText(LineDisplay lineDisplayDevice, string showText)
        {
            int wWindth = showText.Length + 20;
            lineDisplayDevice.CreateWindow(0, 0, 1, 20, 1, wWindth);
            lineDisplayDevice.DisplayText(showText);
            lineDisplayDevice.MarqueeType = DisplayMarqueeType.Left;
            lineDisplayDevice.MarqueeFormat = DisplayMarqueeFormat.Walk;
        }

        private void ShowOnPoleDisplay(string showText)
        {
            LineDisplay display = null;

            try
            {
                display = GetPoleDisplay();

                if(display == null)
                {
                    return;
                }

                ConnectToDisplay(display);

                display.ClearText();
                //string WelcomeMessage = "Welcome\r\n";
                //this.posLineDisplay.DisplayText(WelcomeMessage);
                ScrollText(display, showText);
                //display.DisplayTextAt(2, 1, LeftAlign(product.StockItem.Description, product.StockItem.Description.Length) + RightAlign(product.StockItem.UnitPrice.Value.ToString("0.00"), 12));

            }
            catch (Exception)
            {
                //throw ex;
            }
            finally
            {
                if (display != null)
                    DisconnectFromDisplay(display);
            }
        }

        private string RightAlign(string field, int len)
        {
            return field.PadLeft(len);
        }

        private string LeftAlign(string field, int len)
        {
            return field.PadRight(len);
        }

        [HttpGet]
        public ActionResult GetAllTables(int? id)
        {
            var vm = new IndexViewModel();

            var tables = _barTableService.GetAll().Where(x => x.IsActive && (x.Person.PersonID == Person.Value || x.Person.PersonTypeId == (int)PersonTypeEnum.SalesAssistant)).ToList();

            if(!id.HasValue)
            {
                id = 0;
            }

            vm.MyTables = tables;

            vm.TableId = id.Value;

            return PartialView("_LoadTables", vm);
        }

        public int? CreateNewUserByCashierPOS(string username, string telNumber)
        {
            var allPersons = _personService.GetAll(1).Select(x => x.Email).ToList();

            var existingPerson = allPersons.FirstOrDefault(x => x.ToUpper() == username.ToUpper());

            var canCreateNewUser = false;

            if (existingPerson != null)
            {
                for (int i = 1; i < 300; i++)
                {
                    var usernameNew = username + i.ToString();

                    existingPerson = allPersons.FirstOrDefault(x => x.ToUpper() == usernameNew.ToUpper());

                    if (existingPerson == null)
                    {
                        canCreateNewUser = true;
                        username = usernameNew;
                        break;
                    }
                }
            }
            else
            {
                canCreateNewUser = true;
            }

            if (!canCreateNewUser)
            {
                return null;
            }

            var person = CreateNewPersonGuest(username, telNumber);

            if (person != null)
            {
                return person.Id;
            }
            else
            {
                return null;
            }
        }
        public int OpenTableExistingByCashier(int? existingTableId, string guestName,out int tableNumber)
        {
            int? existingTableMemberId = null;

            existingTableMemberId = CreateNewUserByCashierPOS(guestName, "");

            var guestOrder = new GuestOrder();

            var barTable = _barTableService.GetAll().FirstOrDefault(x => x.GuestId == existingTableMemberId.Value);

            BarTable newBarTable = null;

            if (barTable == null)
            {
                var bt = new BarTable();
                bt.GuestId = existingTableMemberId.Value;
                bt.IsActive = true;
                bt.TableAlias = "Table " + existingTableId.Value.ToString() + "_" + guestName;
                bt.TableId = existingTableId.Value;
                bt.TableName = existingTableId.Value.ToString() + "_" + guestName;
                bt.StaffId = Person.Value;
                bt.CreatedDate = DateTime.Now;
                bt = _barTableService.Create(bt);
                newBarTable = bt;
                existingTableId = bt.Id;
                tableNumber = bt.TableId;
            }
            else
            {
                newBarTable = barTable;
                existingTableId = barTable.Id;
                tableNumber = barTable.TableId;
            }

            var waitressId = Person.Value;

            guestOrder.CreatedDate = DateTime.Now;
            guestOrder.GuestId = existingTableMemberId.Value;
            guestOrder.IsActive = true;
            guestOrder.Name = guestName + "_" + existingTableId.Value.ToString();
            guestOrder.Paid = false;
            guestOrder.PersonId = waitressId;
            guestOrder.TableId = existingTableId.Value;
            guestOrder.PreparedByWaitress = true;

            var allGuestOrders = _guestOrderService.GetAll("GuestRequestItems,GuestRequestItems.StockItem").ToList();

            GuestOrder guestTable = null;

            var exist = allGuestOrders.FirstOrDefault(x => x.IsActive && x.TableId == existingTableId.Value);

            if (exist == null)
            {
                if (existingTableMemberId.HasValue)
                {
                    var existingGuestTable = allGuestOrders.FirstOrDefault(x => x.IsActive && x.GuestId == existingTableMemberId.Value);

                    if (existingGuestTable != null)
                    {
                        existingGuestTable.TableId = existingTableId.Value;
                        existingGuestTable.PersonId = waitressId;
                        _guestOrderService.Update(existingGuestTable);
                        guestTable = existingGuestTable;
                    }
                    else
                    {
                        guestTable = _guestOrderService.Create(guestOrder);
                        //newlyCreatedTable = 1;
                    }
                }
                else
                {
                    guestTable = _guestOrderService.Create(guestOrder);
                    //newlyCreatedTable = 1;
                }
            }
            else
            {
                guestTable = exist;
            }

            return existingTableId.Value;
        }



        
        [HttpPost]
        public ActionResult SelectTable(int? id)
        {
            return Json(new { PV = 1 }, JsonRequestBehavior.AllowGet);
        }

        
        [HttpPost]
        public ActionResult RemoveTable(string id)
        {
            var table = _barTableService.GetAll().FirstOrDefault(x => x.IsActive && x.TableName == id);

            int success = 0;
            
            if(table != null && !table.TableItems.Any())
            {
                var conn = ConfigurationManager.ConnectionStrings[1].ConnectionString;
                StockItemService.DeleteTableItems(table.Id, conn, Person.Value);
                success = 1;
            }

            return Json(new { Failed = success }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddTable()
        {
            var username = DateTime.Now.ToShortTimeString();

             var table = _barTableService.GetAll().OrderBy(x => x.TableId).LastOrDefault(x => x.IsActive);

            int? realtableId = 1;

            if(table != null)
            {
                realtableId = table.TableId + 1;
            }

            var tableNumber = realtableId.Value;

            var openTableId = OpenTableExistingByCashier(realtableId, username, out tableNumber);

            return Json(new { OpenTableId = openTableId, TableNumber = tableNumber }, JsonRequestBehavior.AllowGet);
        }

        private HotelMateWeb.Dal.DataCore.Guest CreateNewPersonGuest(string username, string telephone)
        {
            var person = new Person();

            person.HotelId = 1;
            person.DisplayName = username;
            person.Email = username;
            person.FirstName = username;
            person.LastName = username;
            person.Password = username;
            person.Username = username;
            person.PersonTypeId = (int)PersonTypeEnum.Guest;
            person.IsActive = true;
            person.Address = username;
            person.Telephone = telephone;
            person.Mobile = telephone;

            person.Title = username;
            person.MiddleName = username;
            person.StartDate = DateTime.Now;
            person.EndDate = DateTime.Now;

            person.Guardian = username;
            person.GuardianAddress = username;
            person.PreviousEmployer = username;
            person.ReasonForLeaving = username;
            person.Notes = username;
            person.BirthDate = DateTime.Now;
            person.PreviousEmployerStartDate = DateTime.Now;
            person.PreviousEmployerEndDate = DateTime.Now;
            person.FullMember = false;
            person.IdNumber = "123456789";

            person = _personService.Create(person);

            var guest = new HotelMateWeb.Dal.DataCore.Guest();
            guest.FullName = person.DisplayName;
            guest.Address = person.Address;
            guest.Telephone = person.Telephone;
            guest.Mobile = person.Mobile;
            guest.CountryId = 0;
            guest.Status = "LIVE";
            guest.CarDetails = "";
            guest.Notes = person.Notes;
            guest.Email = person.Email;
            guest.IsActive = true;
            guest.CreatedDate = DateTime.Now;
            guest.HotelId = person.HotelId;
            guest.PersonId = person.PersonID;
            guest.IsChild = false;

            guest = _guestService.Create(guest);
            person.IdNumber = guest.Id.ToString();
            _personService.Update(person);

            //ActivateGameAccount(person);

            return guest;
        }

        private Person CreateNewPerson(string username, string telephone)
        {
            var person = new Person();

            person.HotelId = 1;
            person.DisplayName = username;
            person.Email = username;
            person.FirstName = username;
            person.LastName = username;
            person.Password = username;
            person.Username = username;
            person.PersonTypeId = (int)PersonTypeEnum.Guest;
            person.IsActive = true;
            person.Address = username;
            person.Telephone = telephone;
            person.Mobile = telephone;

            person.Title = username;
            person.MiddleName = username;
            person.StartDate = DateTime.Now;
            person.EndDate = DateTime.Now;

            person.Guardian = username;
            person.GuardianAddress = username;
            person.PreviousEmployer = username;
            person.ReasonForLeaving = username;
            person.Notes = username;
            person.BirthDate = DateTime.Now;
            person.PreviousEmployerStartDate = DateTime.Now;
            person.PreviousEmployerEndDate = DateTime.Now;
            person.FullMember = false;
            person.IdNumber = "123456789";

            person = _personService.Create(person);

            var guest = new HotelMateWeb.Dal.DataCore.Guest();
            guest.FullName = person.DisplayName;
            guest.Address = person.Address;
            guest.Telephone = person.Telephone;
            guest.Mobile = person.Mobile;
            guest.CountryId = 0;
            guest.Status = "LIVE";
            guest.CarDetails = "";
            guest.Notes = person.Notes;
            guest.Email = person.Email;
            guest.IsActive = true;
            guest.CreatedDate = DateTime.Now;
            guest.HotelId = person.HotelId;
            guest.PersonId = person.PersonID;
            guest.IsChild = false;

            guest = _guestService.Create(guest);
            person.IdNumber = guest.Id.ToString();
            _personService.Update(person);

            //ActivateGameAccount(person);

            return person;
        }

        private Person CreateNewPerson(string username)
        {
            var person = new Person();

            person.HotelId = 1;
            person.DisplayName = username;
            person.Email = username;
            person.FirstName = username;
            person.LastName = username;
            person.Password = username;
            person.Username = username;
            person.PersonTypeId = (int)PersonTypeEnum.Guest;
            person.IsActive = true;
            person.Address = username;

            person.Title = username;
            person.MiddleName = username;
            person.StartDate = DateTime.Now;
            person.EndDate = DateTime.Now;

            person.Guardian = username;
            person.GuardianAddress = username;
            person.PreviousEmployer = username;
            person.ReasonForLeaving = username;
            person.Notes = username;
            person.BirthDate = DateTime.Now;
            person.PreviousEmployerStartDate = DateTime.Now;
            person.PreviousEmployerEndDate = DateTime.Now;
            person.IdNumber = "123456789";


            person = _personService.Create(person);

            var guest = new HotelMateWeb.Dal.DataCore.Guest();
            guest.FullName = person.DisplayName;
            guest.Address = person.Address;
            guest.Telephone = person.Telephone;
            guest.Mobile = person.Mobile;
            guest.CountryId = 0;
            guest.Status = "LIVE";
            guest.CarDetails = "";
            guest.Notes = person.Notes;
            guest.Email = person.Email;
            guest.IsActive = true;
            guest.CreatedDate = DateTime.Now;
            guest.HotelId = person.HotelId;
            guest.PersonId = person.PersonID;
            guest.IsChild = false;

            guest = _guestService.Create(guest);
            person.IdNumber = guest.Id.ToString();
            _personService.Update(person);

            //ActivateGameAccount(person);

            return person;
        }

        //private IEnumerable<Person> _cashiers;
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

        [HttpPost]
        public ActionResult TransferTill(int? newCashierId)
        {
            //var thisUser = _personService.GetAllForLogin().Where(x => x.Username.ToUpper().Equals(User.Identity.Name.ToUpper())).FirstOrDefault();

            var person = CashiersList.FirstOrDefault(x => x.PersonID == newCashierId);
            var thisCashier = CashiersList.FirstOrDefault(x => x.PersonID == Person.Value);

            if (person != null && thisCashier != null && person.DistributionPointId == thisCashier.DistributionPointId)
            {
                var conn = ConfigurationManager.ConnectionStrings[1].ConnectionString;
                StockItemService.TransferTill(Person.Value, person.PersonID, conn);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult ViewTill()
        {
            var cashierId = Person.Value;

            var conn = ConfigurationManager.ConnectionStrings[1].ConnectionString;

            var startTime = DateTime.Now;

            var allReceipts = _soldItemService.GetAllInclude("StockItem,PaymentMethod").Where(x => x.PersonId == cashierId && x.TillOpen && x.IsActive).Select(x => x.RecieptNumber).ToList();

            var payments = new HotelMateWeb.Dal.UnitOfWork().PaymentRepository.Get().Where(x => allReceipts.Contains(x.ReceiptNumber)).ToList();

            ReportViewModel rvm = new ReportViewModel();

            rvm.Payments = payments;

            return View(rvm);
        }

        public ActionResult ViewTillOld()
        {
            var cashierId = Person.Value;

            var conn = ConfigurationManager.ConnectionStrings[1].ConnectionString;

            var startTime = DateTime.Now;

            var allSoldItems = _soldItemService.GetAllInclude("StockItem,PaymentMethod").Where(x => x.PersonId == cashierId && x.TillOpen && x.IsActive && (int)PaymentMethodEnum.POSTBILL != x.PaymentMethodId).OrderByDescending(x => x.DateSold).ToList();

            var lastItemSold = allSoldItems.OrderByDescending(x => x.DateSold).FirstOrDefault();
            var firstItemSold = allSoldItems.OrderByDescending(x => x.DateSold).LastOrDefault();


            if (!allSoldItems.Any())
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


                if (allSoldItems.Count == 1)
                {
                    var timeOfDiscount = lastItemSold.DateSold;
                    allDiscounts = _salesDiscountService.GetAll().Where(x => x.ActualCashierId == cashierId && x.DiscountDate >= timeOfDiscount).ToList();
                }
                else if (allSoldItems.Count > 1)
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
            if (!id.HasValue)
            {
                return RedirectToAction("ViewTill");
            }

            var cashierId = Person.Value;

            var unclearedItems = _tableItemService.GetAll().Where(x => x.Cashier == cashierId).Count();

            if (unclearedItems > 0)
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
                //PrintReceipt(allSoldItems, receiptNumber);
            }
            catch
            {

            }

            StockItemService.CloseTill(cashierId, conn);

            return RedirectToAction("Index");
        }

        

        public ActionResult Email(int? id, string email)
        {
            PaymentService ps = new PaymentService();

            var lastClient = ps.GetAll().Where(x => x.CashierId == Person.Value && x.DistributionPointId == CashierDistributionPointId()).OrderByDescending(x => x.PaymentDate).FirstOrDefault();

            if (lastClient != null)
            {

                var receiptNumber = lastClient.ReceiptNumber;

                var lst = _soldItemService.GetAll().Where(x => x.RecieptNumber == receiptNumber).Select(x => new InvoicerItemModel
                {
                    ItemName = x.StockItem.StockItemName,
                    Quantity = x.Qty,
                    Price = x.StockItem.UnitPrice.Value,
                    TotalPrice = x.TotalPrice,
                    TimeOfSale = x.DateSold.Value
                }).ToList();

                var gravm = new InvoicerModel
                {
                    InvoicerItemModelList = lst,
                    Discount = lastClient.DiscountAmount,
                    DiscountDetails = lastClient.Discount,
                    Tax = lastClient.TaxAmount,
                    TaxDetails = lastClient.Tax,
                    SubTotal = lastClient.SubTotal,
                    Total = lastClient.Total,
                    ReceiptNumber = receiptNumber
                };


                var path1 = Path.Combine(Server.MapPath("~/Products/Receipt/"));

                var filename = PDFReceiptPrinter.PrintInvoiceBar(path1, gravm, "");

                var path = Path.Combine(Server.MapPath("~/Products/Receipt/"), filename + ".pdf");

                var fileNameNew = filename + "_" + "Receipt.pdf";

                
                if (!string.IsNullOrEmpty(email))
                {
                    EmailAttachmentToGuest(email, path);
                }
            }

            return Json(new
            {

                success = true
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PrintPdf(int? id)
        {
            PaymentService ps = new PaymentService();

            var lastClient = ps.GetAll().Where(x => x.CashierId == Person.Value && x.DistributionPointId == CashierDistributionPointId()).OrderByDescending(x => x.PaymentDate).FirstOrDefault();

            if(lastClient != null)
            {
                var receiptNumber = lastClient.ReceiptNumber;

                var lst = _soldItemService.GetAll().Where(x => x.RecieptNumber == receiptNumber).Select(x => new InvoicerItemModel
                {
                    ItemName = x.StockItem.StockItemName,
                    Quantity = x.Qty,
                    Price = x.StockItem.UnitPrice.Value,
                    TotalPrice = x.TotalPrice,
                    TimeOfSale = x.DateSold.Value
                }).ToList();

                var gravm = new InvoicerModel
                {
                    InvoicerItemModelList = lst,
                    Discount = lastClient.DiscountAmount,
                    DiscountDetails = lastClient.Discount,
                    Tax = lastClient.TaxAmount,
                    TaxDetails = lastClient.Tax,
                    SubTotal = lastClient.SubTotal,
                    Total = lastClient.Total,
                    ReceiptNumber = receiptNumber
                };


                var path1 = Path.Combine(Server.MapPath("~/Products/Receipt/"));

                var filename = PDFReceiptPrinter.PrintInvoiceBar(path1, gravm, "");

                var path = Path.Combine(Server.MapPath("~/Products/Receipt/"), filename + ".pdf");

                var fileNameNew = filename + "_" + "Receipt.pdf";

                return File(path, "application/ms-excel", fileNameNew);

            }
            
            return Json(new
            {
                
                success = true
            }, JsonRequestBehavior.AllowGet);
        }

        private void EmailAttachmentToGuest(string customerEmail, string path, string strName = "Bar Receipt")
        {


            var emailTemplate = @"<p style='margin:0px;padding:0px;font-size:12px;font-family:Arial, Helvetica, sans-serif;color:#555;' id='yui_3_16_0_ym19_1_1463261898755_4224'>Warm Greetings #FULLNAME#,<br>
                                <br>Please find attached to this mail a copy of your receipt. We would like to thank you for your patronage.<br>
                                <br>Do not hesitate to contact us if you find any thing unusual about this receipt as your satisfaction is of utmost importance to us.<br>
                                <br>We look forward to seeing you again, Thank you.<br><br>
                                </p>";


            emailTemplate = emailTemplate.Replace("#FULLNAME#", "Sir/Madam");


            try
            {

                var dest = customerEmail;

                var storeName = ConfigurationManager.AppSettings["HotelName"].ToString();

                var emails = dest.Split(',').ToList();

                foreach (var email in emails)
                {

                    MailMessage mail = new MailMessage("academyvistang@gmail.com", email, "Your Bar Receipt At " + storeName, emailTemplate);
                    mail.From = new MailAddress("academyvistang@gmail.com", strName);
                    mail.IsBodyHtml = true; // necessary if you're using html email
                    NetworkCredential credential = new NetworkCredential("academyvistang@gmail.com", "Lauren280701");
                    SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                    smtp.EnableSsl = true;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = credential;
                    if (path != null)
                        mail.Attachments.Add(new Attachment(path));
                    smtp.Send(mail);
                }
            }
            catch
            {

            }
        }


        public ActionResult ReduceOrDeleteQuatityManager(int? tableId, int? id, int? qty, int? type, string password)
        {
            var itemIdString = id.ToString().Substring(1);

            int canDelete = 0;

            var itemId = id;

            var item = new TableItem();

            if (tableId.HasValue && tableId == 0)
            {
                item = _tableItemService.GetAllEvery("BarTable").FirstOrDefault(x => x.BarTable.TableId == tableId.Value && x.ItemId == itemId && x.Cashier == Person.Value && x.IsActive);
            }
            else
            {
                item = _tableItemService.GetAllEvery("").FirstOrDefault(x => x.TableId == tableId.Value && x.ItemId == id.Value && x.IsActive);
            }

            if (item != null)
            {
                var quantity = item.Qty;

                if (qty.HasValue && qty.Value > 0)
                {
                    quantity = qty.Value;
                }

                if (quantity <= 0 || !qty.HasValue)
                {
                    var guestOderItemId = item.GuestOrderItemId;

                    var guestId = item.BarTable.GuestId;

                    var managerUser = _personService.GetAll(1).FirstOrDefault(x => x.Password.Equals(password));

                    var isManager = false;

                    if(managerUser != null && managerUser.PersonTypeId == (int)PersonTypeEnum.Manager)
                    {
                        isManager = true;
                    }

                    if (isManager && item.IsActive)
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

                        if (isManager && item.IsActive)
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
                    var newQty = quantity;
                    var guestOderItemId = item.GuestOrderItemId;

                    if (guestOderItemId.HasValue)
                    {
                        var guestOrderItem = _guestOrderItemService.GetById(guestOderItemId);

                        if (guestOrderItem != null)
                        {
                            guestOrderItem.Quantity = newQty;
                            _guestOrderItemService.Update(guestOrderItem);
                        }
                    }

                    item.Qty = newQty;

                    _tableItemService.Update(item);

                    if (item.GuestOrderItemId.HasValue)
                    {
                        var existinggoi = _guestOrderItemService.GetById(item.GuestOrderItemId.Value);
                        if (existinggoi != null)
                        {
                            existinggoi.Quantity = newQty;
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

        public ActionResult ReduceOrDeleteQuatity(int? tableId, int? id, int? qty, int? type)
        {
            var itemIdString = id.ToString().Substring(1);


            int canDelete = 0;

            var itemId = id;

            var item = new TableItem();

            if (tableId.HasValue && tableId == 0)
            {
                item = _tableItemService.GetAllEvery("BarTable").FirstOrDefault(x => x.BarTable.TableId == tableId.Value && x.ItemId == itemId && x.Cashier == Person.Value && !x.IsActive);
            }
            else
            {
                item = _tableItemService.GetAllEvery("").FirstOrDefault(x => x.TableId == tableId.Value && x.ItemId == id.Value && !x.IsActive);
            }

            if (item != null)
            {
                var quantity = item.Qty;

                if (qty.HasValue && qty.Value > 0)
                {
                    quantity = qty.Value;
                }

                if (quantity <= 0 || !qty.HasValue)
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
                    var newQty = quantity;
                    var guestOderItemId = item.GuestOrderItemId;

                    if (guestOderItemId.HasValue)
                    {
                        var guestOrderItem = _guestOrderItemService.GetById(guestOderItemId);

                        if (guestOrderItem != null)
                        {
                            guestOrderItem.Quantity = newQty;
                            _guestOrderItemService.Update(guestOrderItem);
                        }
                    }

                    item.Qty = newQty;

                    _tableItemService.Update(item);

                    if (item.GuestOrderItemId.HasValue)
                    {
                        var existinggoi = _guestOrderItemService.GetById(item.GuestOrderItemId.Value);
                        if (existinggoi != null)
                        {
                            existinggoi.Quantity = newQty;
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

            if (actualItem != null)
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

        private static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["Core"].ConnectionString;
        }

        public ActionResult ReduceOrDeleteQuatityB4BIOYE(int? tableId, int? id, int? qty)
        {
            var itemIdString = id.ToString().Substring(1);

            var itemId = int.Parse(itemIdString);

            var item = new TableItem();

            if (tableId.HasValue && tableId == 0)
            {
                item = _tableItemService.GetAll("BarTable").FirstOrDefault(x => x.BarTable.TableId == tableId.Value && x.ItemId == itemId && x.Cashier == Person.Value);
            }
            else
            {
                item = _tableItemService.GetAll().FirstOrDefault(x => x.TableId == tableId.Value && x.ItemId == itemId);
            }

            if (item != null)
            {
                var quantity = item.Qty;

                if (quantity == qty.Value)
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

            if (id.Value == 0)
            {
                items = _tableItemService.GetAllEvery("BarTable").Where(x => x.BarTable.TableId == id.Value).ToList();
            }
            else
            {
                items = _tableItemService.GetAll().Where(x => x.TableId == id.Value).ToList();
            }

            foreach (var item in items)
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

                if (item.IsActive)
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

        public int GetGuestDetails(int? id)
        {
            if (System.Configuration.ConfigurationManager.AppSettings["UseHotelCustomers"].ToString() == "1")
            {
                var guest = GuestsList.FirstOrDefault(x => x.Id == id);

                if (guest == null)
                    guest = new POSService.Entities.Guest { Id = 0, FullName = "Outside Customer", RoomNumber = "Outside Customer", GuestRoomId = 0 };

                return guest.GuestRoomId;
            }
            else
            {
                var guest = BusinessAccountList.FirstOrDefault(x => x.Id == id);

                if (guest == null)
                    guest = new POSService.Entities.BusinessAccount { Id = 0, Name = "Quick Sale" };

                return 0;

            }
        }

        [HttpPost]
        public ActionResult Payment(string resident, decimal? residentAmount, decimal? serviceCharge, int? tableId, string type, string discount,
            decimal? discountamount, decimal? taxamount, string tax, decimal? subtotal, decimal? total,
            int? totalitems, decimal? paid, string status, int? paymentMethodId, string kitchenNote, string paymentMethodNote, int? guestId, int? guestRoomId)
        {
            var receiptNumber = string.Empty;

            if (!string.IsNullOrEmpty(type) && type == "0") //PrintOnly
            {
                double dTotal = 0;

                List<POSService.Entities.StockItem> lst = new List<POSService.Entities.StockItem>();

                var totalBill = total;

                double.TryParse(totalBill.ToString(), out dTotal);

                var guestTableNumber = string.Empty;

                receiptNumber = DateTime.Now.ToString().GetHashCode().ToString("x");

                if (tableId.HasValue && tableId.Value > 0)
                {
                    BarTable bt = _barTableService.GetById(tableId.Value);

                    if (bt == null)
                    {
                        return RedirectToAction("IndexNew");
                    }

                    guestTableNumber = bt.TableName;

                    var tableList = _tableItemService.GetAll().Where(x => x.TableId == tableId.Value).ToList();

                    lst = tableList.Select(x => new POSService.Entities.StockItem { Id = x.StockItem.Id, Quantity = x.Qty, UnitPrice = x.StockItem.UnitPrice.Value, Description = x.StockItem.Description }).ToList();
                }
                else
                {
                    var tableList = _tableItemService.GetAll("BarTable").Where(x => x.BarTable.TableId == tableId.Value && x.Cashier == Person.Value).ToList();

                    lst = tableList.Select(x => new POSService.Entities.StockItem { Id = x.StockItem.Id, Quantity = x.Qty, UnitPrice = x.StockItem.UnitPrice.Value, Description = x.StockItem.Description }).ToList();
                }

                try
                {
                    PrintReceiptRaw(lst, dTotal, tax, taxamount, discount, discountamount.Value, resident, residentAmount, serviceCharge, receiptNumber, guestTableNumber, paymentMethodId, subtotal, totalitems,true);
                }
                catch (Exception)
                {
                }

                return Json(new
                {
                    ErrorMessage = "",
                    Success = 1,
                }, JsonRequestBehavior.AllowGet);
            }

            receiptNumber = string.Empty;

            try
            {
                int? businessAccountId = null;

                if(!tableId.HasValue)
                {
                    tableId = 0;
                }

                if(guestId.HasValue && guestId.Value > 0)
                {
                    if (System.Configuration.ConfigurationManager.AppSettings["UseHotelCustomers"].ToString() == "1")
                    {
                        var guest = GuestsList.FirstOrDefault(x => x.Id == guestId.Value);

                        if(guest != null)
                        {
                            guestRoomId = guest.GuestRoomId;
                            businessAccountId = null;
                        }
                    }
                    else
                    {
                        var ba = BusinessAccountList.FirstOrDefault(x => x.Id == guestId.Value);

                        if(ba != null)
                        {
                            businessAccountId = (int)ba.Id;
                            guestRoomId = 0;
                        }
                    }
                }
                
                var guestTableNumber = string.Empty;
                var errorMessage = string.Empty;

                decimal amountPaidByCustomer = paid.Value;

                List<POSService.Entities.StockItem> lst = new List<POSService.Entities.StockItem>();

                if (System.Configuration.ConfigurationManager.AppSettings["UseHotelCustomers"].ToString() == "1")
                {
                    if (paymentMethodId == (int)PaymentMethodEnum.POSTBILL && guestId == 0)
                    {
                        errorMessage = "You cannot post a bill for a customer who is not staying in the hotel! Please go back and select a guest!";
                        return Json(new
                        {
                            ErrorMessage = errorMessage,
                            Success = 0,
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    if (paymentMethodId == (int)PaymentMethodEnum.POSTBILL && guestId == 0)
                    {
                        errorMessage = "You cannot post a bill for a customer who is not an account holder! Please go back and select an account holder!";

                        return Json(new
                        {
                            ErrorMessage = errorMessage,
                            Success = 0,
                        }, JsonRequestBehavior.AllowGet);
                    }
                }

                var totalBill = total;

                if (tableId.HasValue && tableId.Value > 0)
                {
                    BarTable bt = _barTableService.GetById(tableId.Value);

                    if (bt == null)
                    {
                        return RedirectToAction("IndexNew");
                    }

                    guestTableNumber = bt.TableName;

                    var tableList = _tableItemService.GetAll().Where(x => x.TableId == tableId.Value).ToList();

                    lst = tableList.Select(x => new POSService.Entities.StockItem { Id = x.StockItem.Id, Quantity = x.Qty, UnitPrice = x.StockItem.UnitPrice.Value, Description = x.StockItem.Description }).ToList();
                }
                else
                {
                    var tableList = _tableItemService.GetAll("BarTable").Where(x => x.BarTable.TableId == tableId.Value && x.Cashier == Person.Value).ToList();

                    lst = tableList.Select(x => new POSService.Entities.StockItem { Id = x.StockItem.Id, Quantity = x.Qty, UnitPrice = x.StockItem.UnitPrice.Value, Description = x.StockItem.Description }).ToList();
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

                receiptNumber = DateTime.Now.ToString().GetHashCode().ToString("x");

                var discountedSum = decimal.Zero;

                bool isHotel = false;

                if (System.Configuration.ConfigurationManager.AppSettings["UseHotelCustomers"].ToString() == "1")
                {
                    isHotel = true;
                }

                discountedSum = discountamount.Value;

                var outstanding = total.Value - paid.Value;

                if (guestId.HasValue && guestId.Value > 0 && outstanding > 0 && paymentMethodId != (int)PaymentMethodEnum.Cash)
                {
                    if (amountPaidByCustomer < totalBill && guestRoomId == 0)
                    {
                        //credit customer account
                        BusinessCorporateAccount bca = new BusinessCorporateAccount
                        {
                            Amount = outstanding,
                            TransactionDate = DateTime.Now,
                            TransactionId = Person.Value,
                            PaymentMethodId = (int)PaymentMethodEnum.POSTBILL,
                            PaymentMethodNote = paymentMethodNote,
                            BusinessAccountId = guestId.Value
                        };

                        _businessAccountCorporateService.Create(bca);
                    }
                    else
                    {
                        //GuestRoomAccount gra = new GuestRoomAccount
                        //{
                        //    Amount = outstanding,
                        //    GuestRoomId = guestRoomId.Value,
                        //    TransactionDate = DateTime.Now,
                        //    PaymentMethodId = (int)PaymentMethodEnum.POSTBILL,
                        //    PaymentMethodNote = "Paid To Restaurant, part payment/post bill",
                        //    PaymentTypeId = (int)RoomPaymentTypeEnum.Restuarant,
                        //    TransactionId = Person.Value
                        //};

                        //////Hotel Customer Part Payment
                        //GuestRoomAccountService gras = new GuestRoomAccountService();
                        //gras.Create(gra);
                    }


                    if (paymentMethodId == (int)PaymentMethodEnum.Cheque || paymentMethodId == (int)PaymentMethodEnum.CreditCard)
                    {
                        guestRoomId = 0;
                        guestId = 0;
                    }

                    if (paymentMethodId == (int)PaymentMethodEnum.Cash)
                    {
                        if (amountPaidByCustomer == decimal.Zero)
                        {
                            guestRoomId = 0;
                            guestId = 0;
                        }
                    }
                }

               
                var _distributionPointId = CashierDistributionPointId().Value;

                if (tableId.HasValue && tableId.Value > 0)
                {
                    transactionId = tableId.Value;
                }

                paymentMethodNote = string.Empty;

                if (string.IsNullOrEmpty(paymentMethodNote.Trim()))
                {
                    paymentMethodNote = guestTableNumber;
                }

                if (residentAmount.HasValue && residentAmount.Value > 0)
                {
                    paymentMethodNote = paymentMethodNote + " chargeSeperate";
                }

                int actualGuestId = 0;

                if (guestId.HasValue)
                {
                    actualGuestId = guestId.Value;
                }

                int actualguestRoomId = 0;

                if (guestRoomId.HasValue)
                {
                    actualguestRoomId = guestRoomId.Value;
                }

                if(!residentAmount.HasValue)
                {
                    residentAmount = decimal.Zero;
                    resident = "0%";
                }

                if (!taxamount.HasValue)
                {
                    taxamount = decimal.Zero;
                    tax = "0%";
                }

                if (!discountamount.HasValue)
                {
                    discountamount = decimal.Zero;
                    discount = "0%";
                }

                var serviceChargeString = "";

                if (!serviceCharge.HasValue)
                {
                    serviceCharge = decimal.Zero;
                    serviceChargeString = "0 NGN";
                }
                else
                {
                    serviceChargeString = serviceCharge.ToString() + " NGN";
                }

                

                StockItemService.UpdateSales(lst, transactionId, actualGuestId, businessAccountId, Person.Value, 1, actualguestRoomId, conn, paymentMethodId.Value, paymentMethodNote,
                    timeOfSale, _distributionPointId, isHotel, receiptNumber, total.Value, subtotal.Value, tax, taxamount.Value, discount, discountamount.Value, resident, residentAmount.Value,
                    serviceChargeString, serviceCharge.Value, paid.Value, outstanding, Person.Value);

                if (tableId.HasValue)
                {
                    if (tableId.Value > 0)
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

                double dTotal = 0;
                double.TryParse(totalBill.ToString(), out dTotal);

                try
                {
                    PrintReceiptRaw(lst, dTotal, tax, taxamount, discount, discountamount.Value, resident, residentAmount, serviceCharge, receiptNumber, guestTableNumber, paymentMethodId, subtotal, totalitems);
                }
                catch (Exception)
                {

                }
            }
            catch (Exception)
            {
            }

            if (total.HasValue)
            {
                var text = LeftAlign("Total", 12) + RightAlign(total.Value.ToString("0.00"), 12);
                ShowOnPoleDisplay(text);
            }

            return Json(new
            {
                ReceiptNumber = receiptNumber,
                ErrorMessage = "",
                Success = 1,
            }, JsonRequestBehavior.AllowGet);
        }

        private void PrintReceiptRaw(List<POSService.Entities.StockItem> lst, double dTotal, string tax, decimal? taxamount, string discount, decimal discountamount, 
            string resident, decimal? residentAmount, decimal? serviceCharge, string receiptNumber, string guestTableNumber, int? paymentMethodId, decimal? subtotal, int? totalitems, 
            bool printOnly = false)
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

                //PrintReceiptFooterRaw(printer, total, tax, discount, anyTaxDetails, "THANK YOU FOR YOUR PATRONAGE.", printOnly, paymentMethodId);

                PrintReceiptFooterRaw(printer, dTotal, tax,taxamount, discount, discountamount, resident, residentAmount, serviceCharge, "THANK YOU FOR YOUR PATRONAGE.", paymentMethodId, subtotal, totalitems, printOnly);

            }
            catch (Exception)
            {
                //throw ex;
            }
            finally
            {

            }

        }

        private void PrintReceiptFooterRaw(string printer, double dTotal, string tax, decimal? taxamount, string discount, decimal? discountamount, string resident,
            decimal? residentAmount, decimal? serviceCharge, string footerText, int? paymentMethodId, decimal? subtotal, int? totalitems, bool printOnly = false)
        {
            int RecLineChars = 42;

            string offSetString = new string(' ', ((RecLineChars / 2) - 4));

            var finalTotal = dTotal;

            PrintTextLineRaw(printer, new string('-', (RecLineChars / 3) * 2));
            PrintTextLineRaw(printer, offSetString + String.Format("SUB-TOTAL   {0}", subtotal.Value.ToString("#0.00")));
            PrintTextLineRaw(printer, offSetString + String.Format("TAX         {0}", taxamount.Value.ToString("#0.00")));
            PrintTextLineRaw(printer, offSetString + String.Format("DISCOUNT    {0}", discountamount.Value.ToString("#0.00")));
            PrintTextLineRaw(printer, offSetString + String.Format("S/CHARGE    {0}", serviceCharge.Value.ToString("#0.00")));
            

            if (residentAmount.HasValue)
            {
                PrintTextLineRaw(printer, offSetString + String.Format("RESIDENT    {0}", residentAmount.Value.ToString("#0.00")));
            }

            PrintTextLineRaw(printer, offSetString + new string('-', (RecLineChars / 3)));
            PrintTextLineRaw(printer, offSetString + String.Format("TOTAL        {0}", finalTotal));
            PrintTextLineRaw(printer, offSetString + new string('-', (RecLineChars / 3)));

            if (printOnly)
            {
                PrintTextLineRaw(printer, offSetString + String.Format("PRINTONLY -{0}", "BILL NOT CLOSED"));
            }
            else
            {
                if (paymentMethodId == (int)PaymentMethodEnum.COMPLIMENTARY)
                {
                    PrintTextLineRaw(printer, offSetString + String.Format("COMPLIMENT -{0}", "CLOSED"));
                }
                else
                {
                    PrintTextLineRaw(printer, offSetString + String.Format("CASHED -----{0}", "CLOSED"));
                }
            }

            PrintTextLineRaw(printer, offSetString + new string('-', (RecLineChars / 3)));

            PrintTextLineRaw(printer, String.Empty);

            //Embed 'center' alignment tag on front of string below to have it printed in the center of the receipt.
            var eCentre = Convert.ToChar(27) + Convert.ToChar(97) + "1";
            //PrintTextLineRaw(printer, System.Text.ASCIIEncoding.ASCII.GetString(new byte[] { 27, (byte)'|', (byte)'c', (byte)'A' }) + footerText);
            offSetString = new string(' ', RecLineChars / 4);
            PrintTextLineRaw(printer, offSetString + footerText);

            PrintTextLineRaw(printer, String.Empty);
            PrintTextLineRaw(printer, String.Empty);


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

        [HttpPost]
        //data: { 
        //type : type, client_id: clientID, clientname: clientName, discountamount: discountamount, taxamount: taxamount, tax: Tax, discount: Discount, subtotal: Subtotal, total: Total, 
        //created_by: createdBy, totalitems: totalItems, paid: Paid, status: Status, paidmethod: paidMethod, ccnum: ccnum, ccmonth: ccmonth, ccyear: ccyear, ccv: ccv },
        
        public ActionResult Paid(int? tableId,string type, int? client_id, string clientname,string discountamount, string taxamount, string tax, string discount, string subtotal, double? total,
        string totalitems, string Paid, string status, string paidmethod, bool? chargeSeperate)
        {
            var paymentMethodId = GetPaymentMethod(paidmethod);

            var cashierId = Person.Value;

            var errorMessage = "";

            if (System.Configuration.ConfigurationManager.AppSettings["UseHotelCustomers"].ToString() == "1")
            {
                if (paymentMethodId == (int)PaymentMethodEnum.POSTBILL && client_id == 0)
                {
                    errorMessage = "You cannot post a bill for a customer who is not staying in the hotel! Please go back and select a guest!";
                }
            }
            else
            {
                if (paymentMethodId == (int)PaymentMethodEnum.POSTBILL && client_id == 0)
                {
                    errorMessage = "You cannot post a bill for a customer who is not an account holder! Please go back and select an account holder!";
                }
            }

            var guestTableNumber = "";

                List<POSService.Entities.StockItem> lst = new List<POSService.Entities.StockItem>();


            if (tableId.HasValue && tableId.Value > 0)
            {
                BarTable bt = _barTableService.GetById(tableId.Value);

                if (bt == null)
                {
                    return RedirectToAction("Index");
                }

                guestTableNumber = bt.TableName;

                lst = _tableItemService.GetAll().Where(x => x.TableId == tableId.Value).ToList().Select(x => new POSService.Entities.StockItem { Id = x.Id, Quantity = x.Qty, UnitPrice = x.StockItem.UnitPrice.Value, Description = x.StockItem.Description }).ToList();

            }
            else
            {
                lst = _tableItemService.GetAll("BarTable").Where(x => x.BarTable.TableId == tableId.Value && x.Cashier == Person.Value).ToList().Select(x => new POSService.Entities.StockItem { Id = x.Id, Quantity = x.Qty, UnitPrice = x.StockItem.UnitPrice.Value, Description = x.StockItem.Description }).ToList();
            }

            if (lst.Count == 0)
            {
                ProcessTheOrderByTableNumberOne(tableId, "");
            }

            var guestRoomId = 0;
            var guestId = 0;
            var amountPaidByCustomer = 0;
            var transactionId = 0;
            var paymentMethodNote = "";

            if (guestId > 0 && amountPaidByCustomer > 0 && paidmethod != "cash")
            {
                if (amountPaidByCustomer < total && guestRoomId == 0)
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
                if (amountPaidByCustomer == decimal.Zero)
                {
                    guestRoomId = 0;
                    guestId = 0;
                }
            }

            var _distributionPointId = GetDistributionPointId();

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

            var conn = ConfigurationManager.ConnectionStrings[1].ConnectionString;

            var ticks = (int)DateTime.Now.Ticks;

            transactionId = Person.Value;

            int terminalId = _distributionPointId.Value;

            var timeOfSale = DateTime.Now;

            var receiptNumber = DateTime.Now.ToString().GetHashCode().ToString("x");

            decimal discountedSum = decimal.Zero;

            decimal.TryParse(discountamount, out discountedSum);

            StockItemService.UpdateSales(lst, transactionId, guestId, Person.Value, 1, guestRoomId, conn, paymentMethodId, paymentMethodNote, timeOfSale,
                _distributionPointId.Value, false, receiptNumber, terminalId, discountedSum, cashierId);

            if (tableId.HasValue)
            {
                if (tableId.Value > 0)
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


            double dTotal = 0;

            double.TryParse(total.ToString(), out dTotal);

            try
            {
                PrintReceiptRaw(lst, dTotal, 0, (double)discountedSum, receiptNumber, chargeSeperate, guestTableNumber, false, paymentMethodId);
            }
            catch (Exception)
            {
                //throw ex;
            }
                       


            return Json(new
            {
                ErrorMessage = errorMessage,
               success = true,
            }, JsonRequestBehavior.AllowGet);
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


        [HttpPost]
        public ActionResult ProcessTheOrderByTableNumberOne(int? tableId, string kitchenNote)
        {
            var itemsList = new List<TableItem>();

            int? realTableId = null;

            var tableAlias = string.Empty;

            if (tableId.Value > 0)
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

            return Json(new
            {
                success = true
            }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Print(int? tableId)
        {
            
            var guestTableNumber = string.Empty;
            var totalBill = decimal.Zero;
            var cashierId = 0;
            List<POSService.Entities.StockItem> lst = new List<POSService.Entities.StockItem>();

            if (tableId.HasValue && tableId.Value > 0)
            {

                BarTable bt = _barTableService.GetById(tableId.Value);

                if (bt == null)
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
            }


            double dTotal = 0;

            double.TryParse(totalBill.ToString(), out dTotal);

            try
            {
                var receiptNumber = DateTime.Now.ToString().GetHashCode().ToString("x");
                //PrintReceiptRaw(lst, dTotal, 0, (double)0, receiptNumber, false, guestTableNumber, true, (int)PaymentMethodEnum.Cash);
                decimal ddTotal = decimal.Zero;
                decimal.TryParse(dTotal.ToString(), out ddTotal);

                PrintReceiptRaw(lst, dTotal, "0%", decimal.Zero, "0%", decimal.Zero, "0%", decimal.Zero, decimal.Zero, receiptNumber, guestTableNumber, (int)PaymentMethodEnum.Cash, ddTotal, 0, true);

                //PrintReceipt(lst, dTotal, 0, 0, receiptNumber, chargeSeperate);
            }
            catch (Exception)
            {
                //throw ex;
            }

            return Json(new
            {
                success = true
            }, JsonRequestBehavior.AllowGet);
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



        private void SendListToNetworkPrinterAndPosPrinter(List<TableItem> itemsList, string tableAlias, string kitchenNote, string pointName)
        {
            PrintReceiptRaw(itemsList, tableAlias, kitchenNote, pointName);
        }

        private void PrintReceiptRaw(List<TableItem> lst, string tableAlias, string kitchenNote, string pointName)
        {

            //var printerName = ConfigurationManager.AppSettings["NetworkPrinterName"].ToString();
            var printerName = ConfigurationManager.AppSettings["KitchenPrinterName"].ToString();


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

        private static int? GetDistributionPointId()
        {
            DistributionPointService ds = new DistributionPointService();

            int id = ds.GetAll().FirstOrDefault().Id;

            try
            {
                if (System.Configuration.ConfigurationManager.AppSettings["DistributionPointId"] != null)
                {
                    int.TryParse(System.Configuration.ConfigurationManager.AppSettings["DistributionPointId"].ToString(), out id);
                }
            }
            catch
            {

            }

            return id;
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


        private void PrintReceipt(List<TableItem> lst, string tableAlias, string kitchenNote, string pointName)
        {
            PosPrinter printer = null;

            try
            {

                var grpList = lst.GroupBy(x => x.StockItem.Id).Select(x => new PrintStockItemModel
                {
                    Description = x.FirstOrDefault().StockItem.Description,
                    Quantity = x.Sum(z => z.Qty),
                    DateSold = x.FirstOrDefault().DateSold
                }).ToList();

                printer = GetReceiptPrinter();

                ConnectToPrinter(printer);

                var thisUserName = User.Identity.Name;

                PrintReceiptHeaderKitchen(printer, tableAlias, DateTime.Now, thisUserName, "", pointName);

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

            if (sc > decimal.Zero)
            {
                finalTotal = finalTotal + sc;
            }

            PrintTextLineRaw(printer, offSetString + new string('-', (RecLineChars / 3)));
            PrintTextLineRaw(printer, offSetString + String.Format("TOTAL      {0}", finalTotal));
            PrintTextLineRaw(printer, offSetString + new string('-', (RecLineChars / 3)));

            if (!printOnly)
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


        private void PrintReceiptFooter(PosPrinter printer, double subTotal, double tax, double discount, string anyTaxDetails, string footerText)
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

        private void PrintLineItemRaw(string printer, string itemCode, int quantity, double unitPrice)
        {
            PrintTestRawItemCode(printer, itemCode, quantity, unitPrice);
        }

        private void PrintTestRawItemCode(string printer, string itemCode, int quantity, double unitPrice)
        {
            if (!string.IsNullOrEmpty(itemCode))
            {
                if(itemCode.Length <= 11)
                {
                    PrintTextRaw(printer, TruncateAt(itemCode.PadRight(11), 11));
                    PrintTextRaw(printer, TruncateAt(quantity.ToString("#0.00").PadLeft(9), 9));
                    PrintTextRaw(printer, TruncateAt(unitPrice.ToString("#0.00").PadLeft(10), 10));
                    PrintTextLineRaw(printer, TruncateAt((quantity * unitPrice).ToString("#0.00").PadLeft(10), 10));

                }
                else
                {
                    var array = SplitByLength(itemCode, 35);

                    if (array.Length > 1)
                    {
                        PrintTextLineRaw(printer, TruncateAt(array[0].PadLeft(0), 35));
                        PrintTextRaw(printer, TruncateAt(array[1].PadRight(11), 11));
                        PrintTextRaw(printer, TruncateAt(quantity.ToString("#0.00").PadLeft(9), 9));
                        PrintTextRaw(printer, TruncateAt(unitPrice.ToString("#0.00").PadLeft(10), 10));
                        PrintTextLineRaw(printer, TruncateAt((quantity * unitPrice).ToString("#0.00").PadLeft(10), 10));
                    }
                    else
                    {
                        PrintTextLineRaw(printer, TruncateAt(array[0].PadLeft(0), 35));
                        PrintTextRaw(printer, TruncateAt(string.Empty.PadRight(11), 11));
                        PrintTextRaw(printer, TruncateAt(quantity.ToString("#0.00").PadLeft(9), 9));
                        PrintTextRaw(printer, TruncateAt(unitPrice.ToString("#0.00").PadLeft(10), 10));
                        PrintTextLineRaw(printer, TruncateAt((quantity * unitPrice).ToString("#0.00").PadLeft(10), 10));
                    }
                }
            }
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

        public IEnumerable<string> EnumerateByLength(string text, int length)
        {
            int index = 0;
            while (index < text.Length)
            {
                int charCount = Math.Min(length, text.Length - index);
                yield return text.Substring(index, charCount);
                index += length;
            }
        }

        public string[] SplitByLength(string text, int length)
        {
            return EnumerateByLength(text, length).ToArray();
        }

        private void PrintLineItemKitchenRaw(string printer, string itemCode, int quantity, string time)
        {
            if (!string.IsNullOrEmpty(itemCode))
            {
                if (itemCode.Length <= 31)
                {
                    PrintTextRaw(printer, TruncateAt(itemCode.PadRight(31), 31));
                    PrintTextLineRaw(printer, TruncateAt(quantity.ToString("#0.00").PadLeft(9), 9));
                }
                else
                {
                    var array = SplitByLength(itemCode, 35);

                    if (array.Length > 1)
                    {
                        PrintTextLineRaw(printer, TruncateAt(array[0].PadLeft(0), 35));
                        PrintTextRaw(printer, TruncateAt(array[1].PadRight(31), 31));
                        PrintTextLineRaw(printer, TruncateAt(quantity.ToString("#0.00").PadLeft(9), 9));
                    }
                    else
                    {
                        PrintTextLineRaw(printer, TruncateAt(array[0].PadLeft(0), 35));
                        PrintTextRaw(printer, TruncateAt(string.Empty.PadRight(31), 31));
                        PrintTextLineRaw(printer, TruncateAt(quantity.ToString("#0.00").PadLeft(9), 9));
                    }
                }
            }
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


        private string CashierDistributionPointName()
        {
            var user = _personService.GetById(GetPersonId().Value);

            if (user.DistributionPoint != null)
            {
                return user.DistributionPoint.Name;
            }

            return string.Empty;
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

        public ActionResult LoadBottomPos(int? id)
        {
            var vm = new IndexViewModel();

            //var every = _tableItemService.GetAllEvery("").Where(x => x.BarTable.Person.PersonID == Person.Value).ToList();

            var every = _tableItemService.GetAllEvery("").Where(x => x.BarTable.Person.PersonID == Person.Value || x.BarTable.Person.PersonTypeId == (int)PersonTypeEnum.SalesAssistant).ToList();


            if (!id.HasValue)
            {
                id = 0;
            }

            if (id.HasValue && id == 0)
            {
                vm.ExistingList = every.Where(x => x.BarTable.TableId == id.Value && x.Cashier == Person.Value).OrderByDescending(x => x.DateSold).ToList();
            }
            else
            {
                vm.ExistingList = every.Where(x => x.TableId == id.Value).OrderByDescending(x => x.DateSold).ToList();
            }

            var total = decimal.Zero;

            int totalCount = 0;

            foreach(var t in vm.ExistingList)
            {
                total += t.Qty * t.StockItem.UnitPrice.Value;
                totalCount += t.Qty;
            }

            vm.CanTakePayment = false;

            vm.CanCancelSale = true;

            vm.CanTakePayment = vm.ExistingList.Count > 0 && !vm.ExistingList.Any(x => !x.IsActive);
            vm.CanCancelSale = vm.ExistingList.Count > 0 && !vm.ExistingList.Any(x => x.IsActive);

            vm.TotalItems = totalCount;

            vm.Total = total;

            vm.TableId = id.Value;

            return PartialView("_LoadPOSBottom", vm);
        }
        [HttpGet]
        public ActionResult LoadPOS(int? id)
        {
            var vm = new IndexViewModel();

            //var tables = _barTableService.GetAll().Where(x => x.IsActive && (x.Person.PersonID == Person.Value || x.Person.PersonTypeId == (int)PersonTypeEnum.SalesAssistant)).ToList();

            var every = _tableItemService.GetAllEvery("").Where(x => x.BarTable.Person.PersonID == Person.Value || x.BarTable.Person.PersonTypeId == (int)PersonTypeEnum.SalesAssistant).ToList();

            if (!id.HasValue)
            {
                id = 0;
            }

            if (id.HasValue && id == 0)
            {
                vm.ExistingList = every.Where(x => x.BarTable.TableId == id.Value && x.Cashier == Person.Value).OrderByDescending(x => x.DateSold).ToList();
            }
            else
            {
                vm.ExistingList = every.Where(x => x.TableId == id.Value).OrderByDescending(x => x.DateSold).ToList();
            }

            return PartialView("_LoadPOS", vm);
        }

        [HttpGet]
        public ActionResult GetProducts(int? id)
        {
            var _distributionPointId = CashierDistributionPointId().Value;
           
            var products = ProductsList.Where(x => x.Remaining > 0 && x.DistributionPointId == _distributionPointId).ToList();

            var catList = products.Select(x => x.CategoryName).ToList();

            catList = catList.Distinct().ToList();

            if(id.HasValue && id.Value > 0)
            {
                products = products.Where(x => x.CategoryId == id.Value).ToList();
            }

            IndexViewModel vm = new IndexViewModel();

            vm.categoriesListString = catList;
            vm.CategoryId = id;

            vm.productsList = products.OrderBy(x => x.StockItemName);

            vm.ColorCodedProductsList = vm.productsList.ToList();

            vm.ColorCodedProductsList.ForEach(x => x.ColorCode = GetColorCodes(x));

            var cv = RenderRazorViewToString("_CategoryView", vm);

            var pv = RenderRazorViewToString("_ProductsView", vm);

            return Json(new { PV = pv, CV = cv }, JsonRequestBehavior.AllowGet);
        }

        private string GetColorCodes(POSService.Entities.StockItem x)
        {
            if(x.CategoryName.ToUpper().StartsWith("A") || x.CategoryName.ToUpper().StartsWith("B") || x.CategoryName.ToUpper().StartsWith("C") || x.CategoryName.ToUpper().StartsWith("D")
                || x.CategoryName.ToUpper().StartsWith("E"))
            {
                return "7";
            }
            else if (x.CategoryName.ToUpper().StartsWith("F") || x.CategoryName.ToUpper().StartsWith("G") || x.CategoryName.ToUpper().StartsWith("H") || x.CategoryName.ToUpper().StartsWith("I")
                || x.CategoryName.ToUpper().StartsWith("J"))
            {
                return "2";
            }

            else if (x.CategoryName.ToUpper().StartsWith("K") || x.CategoryName.ToUpper().StartsWith("L") || x.CategoryName.ToUpper().StartsWith("M") || x.CategoryName.ToUpper().StartsWith("N")
                || x.CategoryName.ToUpper().StartsWith("O"))
            {
                return "3";
            }
            else if (x.CategoryName.ToUpper().StartsWith("P") || x.CategoryName.ToUpper().StartsWith("Q") || x.CategoryName.ToUpper().StartsWith("R") || x.CategoryName.ToUpper().StartsWith("S"))
            {
                return "4";
            }
            else if (x.CategoryName.ToUpper().StartsWith("T") || x.CategoryName.ToUpper().StartsWith("U") || x.CategoryName.ToUpper().StartsWith("V") || x.CategoryName.ToUpper().StartsWith("W"))
            {
                return "5";
            }
            else 
            {
                return "6";
            }
        }
	}
}