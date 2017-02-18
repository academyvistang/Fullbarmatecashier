using HotelMateWeb.Services.Core;
using HotelMateWeb.Services.ServiceApi;
using BarAndRestaurantMate.Helpers.Enums;
using BarAndRestaurantMate.Models;
using ReportManagement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DotNet.Highcharts;
using DotNet.Highcharts.Options;
using DotNet.Highcharts.Helpers;
using DotNet.Highcharts.Enums;
using BarAndRestaurantMate.Helpers;
using HotelMateWeb.Dal.DataCore;

namespace BarAndRestaurantMate.Controllers
{
    [HandleError(View = "CustomErrorView")]
    public class HotelReportController : PdfViewController
    {
        private readonly IGuestService _guestService;
        private readonly IRoomService _roomService;
        private readonly IGuestRoomService _guestRoomService;
        private readonly IGuestReservationService _guestReservationService;
        private readonly IGuestRoomAccountService _guestRoomAccountService;
        private readonly int _hotelAccountsTime = 14;
        private readonly IBusinessAccountService _businessAccountService;
        private readonly IBusinessAccountCorporateService _businessCorporateAccountService;
        private readonly IPersonService _personService = null;
        private readonly IPersonTypeService _personTypeService = null;
        private readonly IExpenseService _expenseService = null;
        private readonly IEmployeeShiftService _employeeShiftService;
        private readonly ISoldItemService _soldItemService;
        private readonly IPurchaseOrderService _purchaseOrderService;
        private readonly IFoodMatrixService _foodMatrixService;
        private readonly IUsedStockItemService _usedStockItemService;
        private readonly ISalesDiscountService _salesDiscountService;




        private int? _hotelId;
        private int HotelID
        {
            get { return _hotelId ?? GetHotelId(); }
            set { _hotelId = value; }
        }

        private int GetHotelId()
        {
            var username = User.Identity.Name;
            var user = _personService.GetAllForLogin().FirstOrDefault(x => x.Username.Equals(username, StringComparison.CurrentCultureIgnoreCase));
            return user.HotelId;
        }

        public HotelReportController()
        {
            int.TryParse(ConfigurationManager.AppSettings["HotelAccountsTime"].ToString(CultureInfo.InvariantCulture), out _hotelAccountsTime);

            _guestService = new GuestService();
            _businessAccountService = new BusinessAccountService();
            _roomService = new RoomService();
            _guestRoomService = new GuestRoomService();
            _guestReservationService = new GuestReservationService();
            _guestRoomAccountService = new GuestRoomAccountService();
            _personService = new PersonService();
            _personTypeService = new PersonTypeService();
            _expenseService = new ExpenseService();
            _employeeShiftService = new EmployeeShiftService();
            _soldItemService = new SoldItemService();
            _purchaseOrderService = new PurchaseOrderService();
            _businessCorporateAccountService = new BusinessCorporateAccountService();
            _foodMatrixService = new FoodMatrixService();
            _usedStockItemService = new UsedStockItemService();
            _salesDiscountService = new SalesDiscountService();

        }


        public ActionResult GuestCheckinPDF(DateTime? startDate, DateTime? endDate)
        {
            ReportViewModel model = new ReportViewModel();

            if (startDate.HasValue && endDate.HasValue)
            {
                if (startDate.Value == endDate.Value)
                {
                    var newDate = startDate.Value.AddDays(1);
                    endDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 0, 0, 1);
                }
            }

            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Now.AddMonths(1);

            //var lst = _guestService.GetByQuery(HotelID).Where(x => x.IsActive && x.GuestRooms.Where(y => y.CheckinDate > startDate.Value && y.IsActive).Count() > 0).OrderByDescending(x => x.CreatedDate).ToList();

            var lst = _guestRoomService.GetAll(HotelID).Where(x => x.IsActive && x.CheckinDate >= startDate && x.CheckinDate <= endDate).OrderBy(x => x.CheckinDate).ToList();

            var groupByList = lst.GroupBy(x => x.CheckinDate.ToShortDateString()).Select(x => new GuestRoomModel { CheckingDate = x.Key, ItemList = x.ToList(), Balance = decimal.Zero });

            model.GroupByList = groupByList.ToList();

            return this.ViewPdf("Guest CheckIn Report", "_GuestCheckinPdf", model);


        }

        public ActionResult GuestCheckin(DateTime? startDate, DateTime? endDate, string clickedButton)
        {
            if (!string.IsNullOrEmpty(clickedButton) && clickedButton.ToUpper().StartsWith("P"))
            {
                return RedirectToAction("GuestCheckinPDF", new { startDate, endDate });
            }

            if (startDate.HasValue && endDate.HasValue)
            {
                if (startDate.Value == endDate.Value)
                {
                    var newDate = startDate.Value.AddDays(1);
                    endDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 0, 0, 1);
                }
            }
            ReportViewModel model = new ReportViewModel();

            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Now.AddMonths(1);

            //var lst = _guestService.GetByQuery(HotelID).Where(x => x.IsActive && x.GuestRooms.Where(y => y.CheckinDate > startDate.Value && y.IsActive).Count() > 0).OrderByDescending(x => x.CreatedDate).ToList();

            var lst = _guestRoomService.GetAll(HotelID).Where(x => x.IsActive && x.CheckinDate >= startDate && x.CheckinDate <= endDate).OrderBy(x => x.CheckinDate).ToList();

            var groupByList = lst.GroupBy(x => x.CheckinDate.ToShortDateString()).Select(x => new GuestRoomModel { CheckingDate = x.Key, ItemList = x.ToList(), Balance = decimal.Zero });

            model.GroupByList = groupByList.ToList();

            return View(model);
        }

        //RoomHistory

        public ActionResult GuestList(DateTime? startDate, DateTime? endDate)
        {
            ReportViewModel model = new ReportViewModel();

            if (startDate.HasValue && endDate.HasValue)
            {
                if (startDate.Value == endDate.Value)
                {
                    var newDate = startDate.Value.AddDays(1);
                    endDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 0, 0, 1);
                }
            }

            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Now.AddMonths(1);

            model.HotelGuests = _guestService.GetByQuery(HotelID).Where(x => x.IsActive && x.GuestRooms.Where(y => y.CheckinDate > startDate.Value && y.IsActive).Count() > 0).OrderByDescending(x => x.CreatedDate).ToList();

            return View(model);
        }

        public ActionResult GuestCheckout(DateTime? startDate, DateTime? endDate)
        {
            ReportViewModel model = new ReportViewModel();

            if (startDate.HasValue && endDate.HasValue)
            {
                if (startDate.Value == endDate.Value)
                {
                    var newDate = startDate.Value.AddDays(1);
                    endDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 0, 0, 1);
                }
            }

            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Now.AddMonths(1);

            //var lst = _guestService.GetByQuery(HotelID).Where(x => x.IsActive && x.GuestRooms.Where(y => y.CheckinDate > startDate.Value && y.IsActive).Count() > 0).OrderByDescending(x => x.CreatedDate).ToList();

            var lst = _guestRoomService.GetAll(HotelID).Where(x => !x.IsActive && x.CheckoutDate >= startDate && x.CheckoutDate <= endDate).OrderBy(x => x.CheckoutDate).ToList();

            var groupByList = lst.GroupBy(x => x.CheckinDate.ToShortDateString()).Select(x => new GuestRoomModel { CheckingDate = x.Key, ItemList = x.ToList(), Balance = decimal.Zero });

            model.GroupByList = groupByList.ToList();

            return View(model);
        }

        public ActionResult GuestReservation(DateTime? startDate, DateTime? endDate)
        {
            ReportViewModel model = new ReportViewModel();

            if (startDate.HasValue && endDate.HasValue)
            {
                if (startDate.Value == endDate.Value)
                {
                    var newDate = startDate.Value.AddDays(1);
                    endDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 0, 0, 1);
                }
            }

            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Now.AddMonths(1);

            model.HotelGuests = _guestService.GetByQuery(HotelID).Where(x => !x.IsActive && x.IsFutureReservation && x.GuestRooms.Where(y => y.CheckinDate > startDate.Value && !y.IsActive).Count() > 0).OrderByDescending(x => x.CreatedDate).ToList();

            return View(model);
        }

        public ActionResult RoomHistory(DateTime? startDate, DateTime? endDate)
        {
            ReportViewModel model = new ReportViewModel();

            if (startDate.HasValue && endDate.HasValue)
            {
                if (startDate.Value == endDate.Value)
                {
                    var newDate = startDate.Value.AddDays(1);
                    endDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 0, 0, 1);
                }
            }

            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Now.AddMonths(1);

            model.Rooms = _roomService.GetAll(HotelID).ToList();

            return View(model);
        }

        public ActionResult GuestGroupReservation(DateTime? startDate, DateTime? endDate)
        {
            ReportViewModel model = new ReportViewModel();

            if (startDate.HasValue && endDate.HasValue)
            {
                if (startDate.Value == endDate.Value)
                {
                    var newDate = startDate.Value.AddDays(1);
                    endDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 0, 0, 1);
                }
            }

            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Now.AddMonths(1);

            model.HotelGuests = _guestService.GetByQuery(HotelID).Where(x => x.IsActive && !x.IsFutureReservation && x.GuestRooms.Where(y => y.CheckinDate > startDate.Value && y.IsActive && y.GroupBooking).Count() > 0).OrderByDescending(x => x.CreatedDate).ToList();

            return View(model);
        }

        public ActionResult RoomOccupancy(DateTime? startDate, DateTime? endDate)
        {
            ReportViewModel model = new ReportViewModel();

            if (startDate.HasValue && endDate.HasValue)
            {
                if (startDate.Value == endDate.Value)
                {
                    var newDate = startDate.Value.AddDays(1);
                    endDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 0, 0, 1);
                }
            }

            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Now.AddMonths(1);

            model.Rooms = _roomService.GetAll(HotelID).ToList();

            model.StartDate = startDate;

            //   _guestService.GetByQuery(HotelID).Where(x => x.IsActive && !x.IsFutureReservation && x.GuestRooms.Where(y => y.CheckinDate > startDate.Value && y.IsActive && y.GroupBooking).Count() > 0).OrderByDescending(x => x.CreatedDate).ToList();

            return View(model);
        }


        public ActionResult AccountRecievable(DateTime? startDate, DateTime? endDate)
        {
            ReportViewModel model = new ReportViewModel();

            if (startDate.HasValue && endDate.HasValue)
            {
                if (startDate.Value == endDate.Value)
                {
                    var newDate = startDate.Value.AddDays(1);
                    endDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 0, 0, 1);
                }
            }

            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Now.AddMonths(1);

            model.Accounts = _guestRoomAccountService.GetAll(HotelID).Where(x => x.TransactionDate >= startDate && x.TransactionDate <= endDate && (x.PaymentMethodId != (int)PaymentMethodEnum.POSTBILL && x.PaymentTypeId != (int)RoomPaymentTypeEnum.Refund)).OrderByDescending(x => x.TransactionDate).ToList();

            return View(model);
        }

        public ActionResult AccountPayable(DateTime? startDate, DateTime? endDate)
        {
            ReportViewModel model = new ReportViewModel();

            if (startDate.HasValue && endDate.HasValue)
            {
                if (startDate.Value == endDate.Value)
                {
                    var newDate = startDate.Value.AddDays(1);
                    endDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 0, 0, 1);
                }
            }

            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Now.AddMonths(1);

            model.Accounts = _guestRoomAccountService.GetAll(HotelID).Where(x => x.TransactionDate >= startDate && x.TransactionDate <= endDate && x.PaymentTypeId == (int)RoomPaymentTypeEnum.Refund).OrderByDescending(x => x.TransactionDate).ToList();

            return View(model);
        }

        public ActionResult CreditGuestReport(DateTime? startDate, DateTime? endDate)
        {
            ReportViewModel model = new ReportViewModel();

            if (startDate.HasValue && endDate.HasValue)
            {
                if (startDate.Value == endDate.Value)
                {
                    var newDate = startDate.Value.AddDays(1);
                    endDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 0, 0, 1);
                }
            }

            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Now.AddMonths(1);

            model.HotelGuests = _guestService.GetByQuery(HotelID).Where(x => x.IsActive && !x.IsFutureReservation && x.GuestRooms.Where(y => y.CheckinDate > startDate.Value && y.IsActive && y.GroupBooking).Count() > 0).OrderByDescending(x => x.CreatedDate).ToList();

            return View(model);
        }


        public ActionResult CorporateGuestReport(DateTime? startDate, DateTime? endDate)
        {
            ReportViewModel model = new ReportViewModel();

            if (startDate.HasValue && endDate.HasValue)
            {
                if (startDate.Value == endDate.Value)
                {
                    var newDate = startDate.Value.AddDays(1);
                    endDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 0, 0, 1);
                }
            }

            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Now.AddMonths(1);

            model.HotelGuests = _guestService.GetByQuery(HotelID).Where(x => x.CompanyId.HasValue && !x.IsFutureReservation && x.GuestRooms.Where(y => y.CheckinDate > startDate.Value).Count() > 0).OrderByDescending(x => x.CreatedDate).ToList();

            return View(model);
        }


        public IEnumerable<ExpenseModel> GetAllItems()
        {
            List<ExpenseModel> lst = new List<ExpenseModel>();

            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM EXPENSE", myConnection))
                {
                    myConnection.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            int id = dr.GetInt32(0);    // Weight int
                            DateTime expenseDate = dr.GetDateTime(1);    // Weight int
                            bool isActive = dr.GetBoolean(2); // Breed string 
                            decimal amount = dr.GetDecimal(3);
                            int staffId = dr.GetInt32(4);
                            int expenseTypeId = dr.GetInt32(7);
                            string description = dr.GetString(8);


                            yield return new ExpenseModel
                            {
                                Id = id,
                                ExpenseDate = expenseDate,
                                IsActive = isActive,
                                Amount = amount,
                                Description = description,
                                ExpenseTypeId = expenseTypeId
                            };

                        }
                    }
                }
            }
        }

        private static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["Core"].ConnectionString;
        }

        [HttpPost]
        public ActionResult SaveMatrix(int? id)
        {
            var allStock = POSService.StockItemService.GetStockItems(1).ToList();
            var allStockCookedFood = allStock.Where(x => x.CookedFood).ToList();
            var allStockRawMaterials = allStock.Where(x => x.RawItem).ToList();

            var actualRawItem = allStockRawMaterials.FirstOrDefault(x => x.Id == id.Value);

            var allFoodMatrix = _foodMatrixService.GetAll().ToList();

            foreach (var item in allStockCookedFood)
            {
                var passId = id.Value.ToString() + "_" + item.Id.ToString();

                var fm = allFoodMatrix.FirstOrDefault(x => x.RawItemId == id.Value && x.FoodItemId == item.Id);

                if (fm != null)
                {
                    _foodMatrixService.Delete(fm);
                }

                if (Request[passId] != null)
                {

                    var torf = Request[passId].ToString();
                    if (torf != "false")
                    {
                        _foodMatrixService.Create(new HotelMateWeb.Dal.DataCore.FoodMatrix { FoodItemId = (int)item.Id, RawItemId = id.Value });
                    }
                }

            }

            return RedirectToAction("FoodMatrixSetup");
        }

        public ActionResult FoodMatrixSetup(DateTime? startDate, DateTime? endDate)
        {
            var allStock = POSService.StockItemService.GetStockItems(1).ToList();
            var allStockCookedFood = allStock.Where(x => x.CookedFood).ToList();
            var allStockRawMaterials = allStock.Where(x => x.RawItem).ToList();

            ReportViewModel model = new ReportViewModel();
            model.CookedFoodItems = allStockCookedFood;
            model.RawMaterialsItems = allStockRawMaterials;
            List<FoodMatrixViewModel> lst = new List<FoodMatrixViewModel>();

            foreach (var r in allStockRawMaterials)
            {
                lst.Add(new FoodMatrixViewModel { RawMaterial = r, CookedFoodList = allStockCookedFood });
            }


            model.FoodMatrixModels = lst;

            return View(model);
        }


        public ActionResult Analysis(DateTime? startDate, DateTime? endDate)
        {
            var allStock = POSService.StockItemService.GetStockItems(1).Where(x => x.RawItem).ToList();

            var service = new POSItemService();

            ReportViewModel model = new ReportViewModel();

            if (startDate.HasValue && endDate.HasValue)
            {
                if (startDate.Value == endDate.Value)
                {
                    var newDate = startDate.Value.AddDays(1);
                    endDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 0, 0, 1);
                }
            }

            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Now.AddMonths(1);

            var allSold = _soldItemService.GetAllInclude("StockItem").Where(x => x.DateSold >= startDate && x.DateSold <= endDate).ToList();
            var allUsedStock = _usedStockItemService.GetAll().Where(x => x.StartDate >= startDate && x.EndDate <= endDate).ToList();

            List<RawMaterialViewModel> lst = new List<RawMaterialViewModel>();


            var allIds = allSold.Select(x => x.ItemId).ToList();


            foreach (var raw in allStock)
            {
                var matrix = _foodMatrixService.GetAll().Where(x => x.RawItemId == raw.Id).ToList();
                var foodNames = matrix.Select(x => x.StockItem.StockItemName).ToDelimitedString(",");
                var allPossiblitiesFromThisRawMaterial = matrix.Select(x => x.FoodItemId).ToList();
                var soldItems = allSold.Where(x => allPossiblitiesFromThisRawMaterial.Contains(x.ItemId)).ToList();
                var usedRawMaterials = allUsedStock.Where(x => x.ItemId == raw.Id).ToList();
                double perRawMaterialCount = 0;
                double perRawMaterialMoney = 0;

                try
                {
                    perRawMaterialCount = (double)(soldItems.Count / usedRawMaterials.Count);
                }
                catch { }

                try
                {
                    perRawMaterialMoney = (double)(soldItems.Sum(XAxis => XAxis.TotalPrice) / usedRawMaterials.Count);
                }
                catch { }


                var thereIsProblem = false;
                lst.Add(new RawMaterialViewModel
                {
                    ThereIsProblem = thereIsProblem,
                    UnitPrice = raw.UnitPrice,
                    StartDate = startDate,
                    EndDate = endDate,
                    RawMaterial = raw,
                    NumberOfRawMaterialsUsed = usedRawMaterials.Count,
                    NumberOfSoldItems = soldItems.Count,
                    PerRawMaterial = perRawMaterialCount,
                    PerRawMaterialMoney = perRawMaterialMoney,
                    PossibilitiesList = foodNames
                });
            }

            model.PossibilityLst = lst;

            return View(model);
        }



        public ActionResult StockLevelIndex(DateTime? startDate, DateTime? endDate)
        {
            var allStock = POSService.StockItemService.GetStockItems(1).Where(x => !x.CookedFood).ToList();

            var service = new POSItemService();

            ReportViewModel model = new ReportViewModel();

            if (startDate.HasValue && endDate.HasValue)
            {
                if (startDate.Value == endDate.Value)
                {
                    var newDate = startDate.Value.AddDays(1);
                    endDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 0, 0, 1);
                }
            }

            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Now.AddMonths(1);

            var allSold = _soldItemService.GetAllInclude("StockItem").Where(x => x.DateSold >= startDate && x.DateSold <= endDate).ToList();

            var allPosItems = service.GetAllInclude("StockItem").ToList();

            var all = _purchaseOrderService.GetAllByInclude("PurchaseOrderItems,PurchaseOrderItems.StockItem").Where(x => x.OrderDate >= startDate && x.OrderDate <= endDate).ToList();

            List<InventoryViewModel> lst = new List<InventoryViewModel>();

            foreach (var s in allStock)
            {
                var poi = all.SelectMany(x => x.PurchaseOrderItems).ToList();
                var allPoiForStock = poi.Where(x => x.StockItem.Id == s.Id).ToList();
                var quantity = allPoiForStock.Sum(x => x.QtyRecieved);
                var returns = allPoiForStock.Sum(x => x.Returns);
                var numberSold = allSold.Where(x => x.StockItem.Id == s.Id).Sum(x => x.Qty);
                var remaining = 0;
                var damaged = 0;
                var notificationNumber = s.NotNumber;

                if (s.RawItem)
                {
                    remaining = s.Quantity;
                }
                else
                {
                    var poitem = allPosItems.FirstOrDefault(x => x.StockItem.Id == s.Id);

                    if (poitem != null)
                        remaining = poitem.Remaining;
                }

                bool thereIsProblem = false;

                if (remaining < notificationNumber)
                {
                    thereIsProblem = true;
                }

                lst.Add(new InventoryViewModel { NotifyNumber = notificationNumber, ThereIsProblem = thereIsProblem, StockItemName = s, Acquired = quantity, Returns = returns, NumberSold = numberSold, Remaining = remaining, Damaged = damaged });
            }

            model.CompleteInventoryList = lst;

            return View(model);

        }




        public ActionResult PurchaseOrderIndex(DateTime? startDate, DateTime? endDate)
        {

            ReportViewModel model = new ReportViewModel();

            if (startDate.HasValue && endDate.HasValue)
            {
                if (startDate.Value == endDate.Value)
                {
                    var newDate = startDate.Value.AddDays(1);
                    endDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 0, 0, 1);
                }
            }

            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Now.AddMonths(1);

            var poItems = new List<PurchaseOrder>();

            poItems = _purchaseOrderService.GetAllByInclude("PurchaseOrderItems").Where(x => x.OrderDate >= startDate && x.OrderDate <= endDate).OrderByDescending(x => x.OrderDate).ToList();

            model.PurchaseOrders = poItems;

            return View(model);
        }


        public ActionResult Inventory(DateTime? startDate, DateTime? endDate)
        {
            var allStock = POSService.StockItemService.GetStockItems(1).Where(x => !x.CookedFood).ToList();

            var service = new POSItemService();

            ReportViewModel model = new ReportViewModel();

            if (startDate.HasValue && endDate.HasValue)
            {
                if (startDate.Value == endDate.Value)
                {
                    var newDate = startDate.Value.AddDays(1);
                    endDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 0, 0, 1);
                }
            }

            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Now.AddMonths(1);

            var allSold = _soldItemService.GetAllInclude("StockItem").Where(x => x.DateSold >= startDate && x.DateSold <= endDate).ToList();

            var allPosItems = service.GetAllInclude("StockItem").ToList();

            var all = _purchaseOrderService.GetAllByInclude("PurchaseOrderItems,PurchaseOrderItems.StockItem").Where(x => x.OrderDate >= startDate && x.OrderDate <= endDate).ToList();

            List<InventoryViewModel> lst = new List<InventoryViewModel>();

            foreach (var s in allStock)
            {
                var poi = all.SelectMany(x => x.PurchaseOrderItems).ToList();
                var allPoiForStock = poi.Where(x => x.StockItem.Id == s.Id).ToList();
                var quantity = allPoiForStock.Sum(x => x.QtyRecieved);
                var returns = allPoiForStock.Sum(x => x.Returns);
                var numberSold = allSold.Where(x => x.StockItem.Id == s.Id).Sum(x => x.Qty);
                var remaining = 0;
                var damaged = 0;

                if (s.RawItem)
                {
                    remaining = s.Quantity;
                }
                else
                {
                    var poitem = allPosItems.FirstOrDefault(x => x.StockItem.Id == s.Id);

                    if (poitem != null)
                        remaining = poitem.Remaining;
                }

                bool thereIsProblem = false;

                if (remaining < (quantity - (returns + numberSold + damaged)))
                {
                    thereIsProblem = true;
                }

                lst.Add(new InventoryViewModel { ThereIsProblem = thereIsProblem, StockItemName = s, Acquired = quantity, Returns = returns, NumberSold = numberSold, Remaining = remaining, Damaged = damaged });
            }

            model.CompleteInventoryList = lst;

            return View(model);

        }


        public ActionResult StockReceiveable(DateTime? startDate, DateTime? endDate)
        {
            ReportViewModel model = new ReportViewModel();

            if (startDate.HasValue && endDate.HasValue)
            {
                if (startDate.Value == endDate.Value)
                {
                    var newDate = startDate.Value.AddDays(1);
                    endDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 0, 0, 1);
                }
            }

            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Now.AddMonths(1);

            var purchaseOrders = _purchaseOrderService.GetAll().Where(x => x.OrderDate >= startDate.Value && x.OrderDate <= endDate && !string.IsNullOrEmpty(x.InvoicePath) && x.PurchaseOrderItems.Where(y => y.Returns > 0).Any()).ToList();

            model.PurchaseOrders = purchaseOrders;

            return View(model);
        }


        public ActionResult StockPayable(DateTime? startDate, DateTime? endDate)
        {
            ReportViewModel model = new ReportViewModel();

            if (startDate.HasValue && endDate.HasValue)
            {
                if (startDate.Value == endDate.Value)
                {
                    var newDate = startDate.Value.AddDays(1);
                    endDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 0, 0, 1);
                }
            }

            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Now.AddMonths(1);

            var purchaseOrders = _purchaseOrderService.GetAll().Where(x => x.OrderDate >= startDate.Value && x.OrderDate <= endDate && !string.IsNullOrEmpty(x.InvoicePath)).ToList();

            model.PurchaseOrders = purchaseOrders;

            return View(model);
        }

        


        public ActionResult AllPayable(DateTime? startDate, DateTime? endDate)
        {
            ReportViewModel model = new ReportViewModel();

            if (startDate.HasValue && endDate.HasValue)
            {
                if (startDate.Value == endDate.Value)
                {
                    var newDate = startDate.Value.AddDays(1);
                    endDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 0, 0, 1);
                }
            }

            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Now.AddMonths(1);

            var accountsPaidin = _soldItemService.GetAll().Where(x => x.IsActive && !x.BusinessAccountId.HasValue && x.DateSold >= startDate.Value && x.DateSold <= endDate && (x.PaymentMethodId != (int)PaymentMethodEnum.POSTBILL)).ToList();

            var all = _purchaseOrderService.GetAll("").ToList();

            var accountsPaidOut = all.Where(x => x.OrderDate >= startDate.Value && x.OrderDate <= endDate).ToList();

            var expenses = GetAllItems().Where(x => x.ExpenseDate >= startDate && x.ExpenseDate <= endDate).ToList();

            var balanceSheetModel = accountsPaidOut.Select(x => new BalanceSheetModel { TransactionDate = DateTime.Parse(x.OrderDate.ToShortDateString()), AmountPaidIn = decimal.Zero, AmountPaidOut = x.NetValue, Cashier = x.RaisedBy, PaymentMentMethod = null, PaymentType = null }).ToList();

            balanceSheetModel.AddRange(expenses.Select(x => new BalanceSheetModel { TransactionDate = DateTime.Parse(x.ExpenseDate.ToShortDateString()), AmountPaidOut = x.Amount, AmountPaidIn = decimal.Zero, Cashier = x.StaffId, PaymentMentMethod = null, PaymentType = null }).ToList());

            var allStaff = _personService.GetAllForLogin().ToList();

            balanceSheetModel = balanceSheetModel.OrderByDescending(x => x.TransactionDate).ToList();

            balanceSheetModel.ForEach(x => x.UserName = GetUserName(allStaff, x.Cashier.Value));

            var tPaidIn = balanceSheetModel.Sum(x => x.AmountPaidIn);

            var tPaidOut = balanceSheetModel.Sum(x => x.AmountPaidOut);

            model.FullBalance = tPaidIn - tPaidOut;

            model.BalanceSheet = balanceSheetModel;

            model.ConciseBalanceSheetSheet = balanceSheetModel.GroupBy(x => x.TransactionDate).Select(x => new ConciseBalanceSheetModel { ActualDate = x.Key, TotalReceiveable = x.Sum(y => y.AmountPaidIn), TotalPayaeble = x.Sum(z => z.AmountPaidOut) }).ToList();


            return View(model);
        }


        public ActionResult PaymentReceipt(DateTime? startDate, DateTime? endDate)
        {
            ReportViewModel model = new ReportViewModel();

            if (startDate.HasValue && endDate.HasValue)
            {
                if (startDate.Value == endDate.Value)
                {
                    var newDate = startDate.Value.AddDays(1);
                    endDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 0, 0, 1);
                }
            }

            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Now.AddMonths(1);

            var accountsPaidin = _soldItemService.GetAllInclude("StockItem").Where(x => x.IsActive && !x.BusinessAccountId.HasValue && x.DateSold >= startDate.Value && x.DateSold <= endDate && (x.PaymentMethodId != (int)PaymentMethodEnum.POSTBILL)).ToList();

            var accountsPaidin1 = _businessCorporateAccountService.GetAll(1).Where(x => x.TransactionDate >= startDate.Value && x.TransactionDate <= endDate).ToList();

            var all = _purchaseOrderService.GetAllByInclude("PurchaseOrderItems").ToList();

            var accountsPaidin2 = all.Where(x => x.OrderDate >= startDate.Value && x.OrderDate <= endDate && x.PurchaseOrderItems.Any(y => y.Returns > 0)).ToList();

            var accountsPaidOut = all.Where(x => x.OrderDate >= startDate.Value && x.OrderDate <= endDate).ToList();

            var expenses = GetAllItems().Where(x => x.ExpenseDate >= startDate && x.ExpenseDate <= endDate).ToList();

            var balanceSheetModel = accountsPaidin.Select(x => new BalanceSheetModel { TransactionDate = DateTime.Parse(x.DateSold.Value.ToShortDateString()), AmountPaidIn = x.TotalPrice, AmountPaidOut = decimal.Zero, Cashier = x.TransactionId, PaymentMentMethod = x.PaymentMethod, PaymentType = null }).ToList();

            balanceSheetModel.AddRange(accountsPaidin1.Select(x => new BalanceSheetModel { TransactionDate = DateTime.Parse(x.TransactionDate.ToShortDateString()), AmountPaidIn = x.Amount, AmountPaidOut = decimal.Zero, Cashier = x.TransactionId, PaymentMentMethod = x.PaymentMethod, PaymentType = null }));

            balanceSheetModel.AddRange(accountsPaidin2.Select(x => new BalanceSheetModel { TransactionDate = DateTime.Parse(x.OrderDate.ToShortDateString()), AmountPaidIn = x.PurchaseOrderItems.Sum(y => y.ReturnValue), AmountPaidOut = decimal.Zero, Cashier = null, PaymentMentMethod = null, PaymentType = null }));

            balanceSheetModel.AddRange(accountsPaidOut.Select(x => new BalanceSheetModel { TransactionDate = DateTime.Parse(x.OrderDate.ToShortDateString()), AmountPaidIn = decimal.Zero, AmountPaidOut = x.NetValue, Cashier = x.RaisedBy, PaymentMentMethod = null, PaymentType = null }));

            balanceSheetModel.AddRange(expenses.Select(x => new BalanceSheetModel { TransactionDate = DateTime.Parse(x.ExpenseDate.ToShortDateString()), AmountPaidOut = x.Amount, AmountPaidIn = decimal.Zero, Cashier = x.StaffId, PaymentMentMethod = null, PaymentType = null }).ToList());

            var allStaff = _personService.GetAllForLogin().ToList();

            balanceSheetModel = balanceSheetModel.OrderByDescending(x => x.TransactionDate).ToList();

            balanceSheetModel.ForEach(x => x.UserName = GetUserNameOrNull(allStaff, x.Cashier));

            var tPaidIn = balanceSheetModel.Sum(x => x.AmountPaidIn);

            var totalDiscount = _salesDiscountService.GetAll().Where(x => x.DiscountDate >= startDate && x.DiscountDate <= endDate).Sum(x => x.Amount);

            model.TotalCashDiscount = totalDiscount;

            tPaidIn = tPaidIn - totalDiscount;

            var tPaidOut = balanceSheetModel.Sum(x => x.AmountPaidOut);

            model.FullBalance = tPaidIn - tPaidOut;

            model.BalanceSheet = balanceSheetModel;

            model.ConciseBalanceSheetSheet = balanceSheetModel.GroupBy(x => x.TransactionDate).Select(x => new ConciseBalanceSheetModel { ActualDate = x.Key, TotalReceiveable = x.Sum(y => y.AmountPaidIn), TotalPayaeble = x.Sum(z => z.AmountPaidOut) }).ToList();



            return View(model);
        }


        public ActionResult PandL(DateTime? startDate, DateTime? endDate)
        {
            ReportViewModel model = new ReportViewModel();

            if (startDate.HasValue && endDate.HasValue)
            {
                if (startDate.Value == endDate.Value)
                {
                    var newDate = startDate.Value.AddDays(1);
                    endDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 0, 0, 1);
                }
            }

            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Now.AddMonths(1);

            var accountsPaidin = _guestRoomAccountService.GetAll("").Where(x => x.TransactionDate >= startDate.Value && x.TransactionDate <= endDate && (x.PaymentMethodId != (int)PaymentMethodEnum.POSTBILL)).ToList();
               // _soldItemService.GetAllInclude("StockItem").Where(x => x.IsActive && !x.BusinessAccountId.HasValue && x.DateSold >= startDate.Value && x.DateSold <= endDate &&
               //(x.PaymentMethodId != (int)PaymentMethodEnum.POSTBILL)).ToList();

            var accountsPaidin1 = _businessCorporateAccountService.GetAll(1).Where(x => x.TransactionDate >= startDate.Value && x.TransactionDate <= endDate).ToList();

            var all = _purchaseOrderService.GetAllByInclude("PurchaseOrderItems").ToList();

            var accountsPaidin2 = all.Where(x => x.OrderDate >= startDate.Value && x.OrderDate <= endDate && x.PurchaseOrderItems.Any(y => y.Returns > 0)).ToList();

            var accountsPaidOut = all.Where(x => x.OrderDate >= startDate.Value && x.OrderDate <= endDate).ToList();

            accountsPaidOut.Clear();

            accountsPaidin2.Clear();

            var expenses = GetAllItems().Where(x => x.ExpenseDate >= startDate && x.ExpenseDate <= endDate).ToList();

            expenses.Clear();

            var balanceSheetModel = accountsPaidin.Select(x => new BalanceSheetModel { TransactionDate = DateTime.Parse(x.TransactionDate.ToShortDateString()), AmountPaidIn = GetProfitRooms(x), AmountPaidOut = decimal.Zero, Cashier = x.TransactionId, PaymentMentMethod = x.PaymentMethod, PaymentType = null }).ToList();

            balanceSheetModel.AddRange(accountsPaidin1.Select(x => new BalanceSheetModel { TransactionDate = DateTime.Parse(x.TransactionDate.ToShortDateString()), AmountPaidIn = x.Amount, AmountPaidOut = decimal.Zero, Cashier = x.TransactionId, PaymentMentMethod = x.PaymentMethod, PaymentType = null }));

            //Returns
            balanceSheetModel.AddRange(accountsPaidin2.Select(x => new BalanceSheetModel { TransactionDate = DateTime.Parse(x.OrderDate.ToShortDateString()), AmountPaidIn = x.PurchaseOrderItems.Sum(y => y.ReturnValue), AmountPaidOut = decimal.Zero, Cashier = null, PaymentMentMethod = null, PaymentType = null }));

            balanceSheetModel.AddRange(accountsPaidOut.Select(x => new BalanceSheetModel { TransactionDate = DateTime.Parse(x.OrderDate.ToShortDateString()), AmountPaidIn = decimal.Zero, AmountPaidOut = x.NetValue, Cashier = x.RaisedBy, PaymentMentMethod = null, PaymentType = null }));

            balanceSheetModel.AddRange(expenses.Select(x => new BalanceSheetModel { TransactionDate = DateTime.Parse(x.ExpenseDate.ToShortDateString()), AmountPaidOut = x.Amount, AmountPaidIn = decimal.Zero, Cashier = x.StaffId, PaymentMentMethod = null, PaymentType = null }).ToList());

            var allStaff = _personService.GetAllForLogin().ToList();

            balanceSheetModel = balanceSheetModel.OrderByDescending(x => x.TransactionDate).ToList();

            balanceSheetModel.ForEach(x => x.UserName = GetUserNameOrNull(allStaff, x.Cashier));

            var totalDiscount = _salesDiscountService.GetAll().Where(x => x.DiscountDate >= startDate && x.DiscountDate <= endDate).Sum(x => x.Amount);

            model.TotalCashDiscount = totalDiscount;

            var tPaidIn = balanceSheetModel.Sum(x => x.AmountPaidIn);

            tPaidIn = tPaidIn - totalDiscount;

            var tPaidOut = balanceSheetModel.Sum(x => x.AmountPaidOut);

            model.FullBalance = tPaidIn - tPaidOut;

            model.BalanceSheet = balanceSheetModel;

            model.ConciseBalanceSheetSheet = balanceSheetModel.GroupBy(x => x.TransactionDate).Select(x => new ConciseBalanceSheetModel { ActualDate = x.Key, TotalReceiveable = x.Sum(y => y.AmountPaidIn), TotalPayaeble = x.Sum(z => z.AmountPaidOut) }).ToList();


            return View(model);
        }

        private decimal GetProfitRooms(GuestRoomAccount x)
        {
            return x.Amount;
        }

        private decimal GetProfit(HotelMateWeb.Dal.DataCore.SoldItemsAll x)
        {
            if (x.StockItem != null)
            {
                var totalBoughtPrice = x.StockItem.OrigPrice * x.Qty;
                return x.TotalPrice - totalBoughtPrice;
            }

            return decimal.Zero;
        }

        public ActionResult BalanceSheetBar(DateTime? startDate, DateTime? endDate)
        {
            ReportViewModel model = new ReportViewModel();

            if (startDate.HasValue && endDate.HasValue)
            {
                if (startDate.Value == endDate.Value)
                {
                    var newDate = startDate.Value.AddDays(1);
                    endDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 0, 0, 1);
                }
            }

            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Now.AddMonths(1);

            var accountsPaidin = _soldItemService.GetAll().Where(x => x.DateSold >= startDate.Value && x.DateSold <= endDate && (x.PaymentMethodId != (int)PaymentMethodEnum.POSTBILL)).ToList();

            var balanceSheetModel = accountsPaidin.Select(x => new BalanceSheetModel
            {
                TransactionDate = x.DateSold.Value,
                AmountPaidIn = x.TotalPrice,
                AmountPaidOut = decimal.Zero,
                Cashier = x.TransactionId,
                PaymentMentMethod = x.PaymentMethod,
                PaymentTypeId = x.PaymentTypeId
            }).ToList();

            var allStaff = _personService.GetAllForLogin().ToList();

            balanceSheetModel = balanceSheetModel.OrderByDescending(x => x.TransactionDate).ToList();

            balanceSheetModel.ForEach(x => x.UserName = GetUserName(allStaff, x.Cashier.Value));

            var tPaidIn = balanceSheetModel.Sum(x => x.AmountPaidIn);

            var tPaidOut = balanceSheetModel.Sum(x => x.AmountPaidOut);

            model.FullBalance = tPaidIn - tPaidOut;

            model.BalanceSheet = balanceSheetModel;

            return View(model);
        }

        public ActionResult BalanceSheet(DateTime? startDate, DateTime? endDate)
        {
            ReportViewModel model = new ReportViewModel();

            if (startDate.HasValue && endDate.HasValue)
            {
                if (startDate.Value == endDate.Value)
                {
                    var newDate = startDate.Value.AddDays(1);
                    endDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 0, 0, 1);
                }
            }

            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Now.AddMonths(1);

            var accountsPaidIn = _guestRoomAccountService.GetAll(HotelID).Where(x => x.TransactionDate >= startDate.Value && x.TransactionDate <= endDate && (x.PaymentTypeId == (int)RoomPaymentTypeEnum.InitialDeposit || x.PaymentTypeId == (int)RoomPaymentTypeEnum.CashDeposit || x.PaymentTypeId == (int)RoomPaymentTypeEnum.ReservationDeposit)).ToList();

            var accountsPaidOut = _guestRoomAccountService.GetAll(HotelID).Where(x => x.TransactionDate >= startDate.Value && x.TransactionDate <= endDate && (x.PaymentTypeId == (int)RoomPaymentTypeEnum.Refund)).ToList();

            var expenses = GetAllItems().Where(x => x.ExpenseDate >= startDate && x.ExpenseDate <= endDate);

            var balanceSheetModel = accountsPaidIn.Select(x => new BalanceSheetModel { TransactionDate = x.TransactionDate, AmountPaidIn = x.Amount, AmountPaidOut = decimal.Zero, Cashier = x.TransactionId, PaymentMentMethod = x.PaymentMethod, PaymentType = x.RoomPaymentType }).ToList();

            balanceSheetModel.AddRange(accountsPaidOut.Select(x => new BalanceSheetModel { TransactionDate = x.TransactionDate, AmountPaidIn = decimal.Zero, AmountPaidOut = x.Amount, Cashier = x.TransactionId, PaymentMentMethod = x.PaymentMethod, PaymentType = x.RoomPaymentType }));

            balanceSheetModel.AddRange(expenses.Select(x => new BalanceSheetModel { TransactionDate = x.ExpenseDate, AmountPaidOut = x.Amount, AmountPaidIn = decimal.Zero, Cashier = x.StaffId, PaymentMentMethod = null, PaymentType = null }).ToList());

            var allStaff = _personService.GetAllForLogin().ToList();

            balanceSheetModel = balanceSheetModel.OrderByDescending(x => x.TransactionDate).ToList();

            balanceSheetModel.ForEach(x => x.UserName = GetUserNameOrNull(allStaff, x.Cashier));

            var tPaidIn = balanceSheetModel.Sum(x => x.AmountPaidIn);

            var tPaidOut = balanceSheetModel.Sum(x => x.AmountPaidOut);

            model.FullBalance = tPaidIn - tPaidOut;

            model.BalanceSheet = balanceSheetModel;

            return View(model);
        }

        private string GetUserNameOrNull(List<HotelMateWeb.Dal.DataCore.Person> allStaff, int? id)
        {
            if (!id.HasValue)
            {
                return "";
            }

            var staff = allStaff.FirstOrDefault(x => x.PersonID == id.Value);

            if (staff != null)
                return staff.Username;

            return "";
        }

        private string GetUserName(List<HotelMateWeb.Dal.DataCore.Person> allStaff, int p)
        {
            var staff = allStaff.FirstOrDefault(x => x.PersonID == p);
            if (staff != null)
                return staff.Username;

            return "";
        }


        public ActionResult CombinedSales(DateTime? startDate, DateTime? endDate)
        {
            ReportViewModel model = new ReportViewModel();

            if (startDate.HasValue && endDate.HasValue)
            {
                if (startDate.Value == endDate.Value)
                {
                    var newDate = startDate.Value.AddDays(1);
                    endDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 0, 0, 1);
                }
            }

            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Now.AddMonths(1);

            var combinedSales = _guestRoomAccountService.GetAll(HotelID).Where(x => x.TransactionDate >= startDate && x.TransactionDate <= endDate && (x.RoomPaymentType.PaymentStatusId == (int)RoomPaymentStatusEnum.Credit)).ToList();

            //model.ModelGroupBy = combinedSales.GroupBy(x => x.TransactionDate.ToShortDateString()).Select(x => new SoldItemModel { PaymentTypeName = x.Key, TotalPrice = x.Sum(y => y.Amount),
            //RoomAccountList = x.ToList() }).ToList();

            var neWCombinedSalesModel = combinedSales.Select(x => new CombinedSalesModel
            {
                DateSold = x.TransactionDate,
                GuestRoom = x.GuestRoom,
                Amount = x.Amount,
                PaymentMethod = x.PaymentMethod.Name,
                PaymentMethodNote = x.PaymentMethodNote,
                StaffName = x.Person.DisplayName,
                Terminal = x.RoomPaymentType.Name
            }).ToList();


            var conn = ConfigurationManager.ConnectionStrings[1].ConnectionString;

            var ds = POSService.StockItemService.GetSoldItems(conn);

            var salesModel = ConvertToList(ds);

            List<SoldItemModel> simLst = new List<SoldItemModel>();

            simLst = salesModel.Where(x => x.DateSold >= startDate && x.DateSold <= endDate).OrderByDescending(x => x.DateSold).ToList();

            if (!startDate.HasValue && !endDate.HasValue)
            {
                simLst = simLst.Where(x => x.DateSold.ToShortDateString().Equals(DateTime.Today.ToShortDateString(), StringComparison.InvariantCultureIgnoreCase)).OrderByDescending(x => x.DateSold).ToList();
            }

            var realSales = simLst.GroupBy(x => x.TimeOfSale).Select(x => new SoldItemModel
            {
                TimeOfSale = x.Key,
                TotalPrice = x.Sum(y => y.TotalPrice),
                PaymentTypeName = x.FirstOrDefault().PaymentTypeName,
                PaymentMethodName = x.FirstOrDefault().PaymentMethodName,
                PersonName = x.FirstOrDefault().PersonName,
                DateSold = x.FirstOrDefault().DateSold,
                PaymentMethodNote = x.FirstOrDefault().PaymentMethodNote,
            }).ToList();

            neWCombinedSalesModel.AddRange(realSales.Select(x => new CombinedSalesModel
            {
                DateSold = x.DateSold,
                GuestRoom = null,
                Amount = x.TotalPrice,
                PaymentMethod = x.PaymentMethodName,
                PaymentMethodNote = x.PaymentMethodNote,
                StaffName = x.PersonName,
                Terminal = x.PaymentTypeName
            }).ToList());

            model.ModelGroupBy = neWCombinedSalesModel.GroupBy(x => x.DateSold.ToShortDateString()).Select(x => new SoldItemModel { PaymentTypeName = x.Key, TotalPrice = x.Sum(y => y.Amount), CombinedList = x.ToList() }).ToList();

            return View(model);
        }

        public ActionResult TotalExpenditure(DateTime? startDate, DateTime? endDate)
        {
            ReportViewModel model = new ReportViewModel();

            if (startDate.HasValue && endDate.HasValue)
            {
                if (startDate.Value == endDate.Value)
                {
                    var newDate = startDate.Value.AddDays(1);
                    endDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 0, 0, 1);
                }
            }

            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Now.AddMonths(1);

            var expenditureList = _expenseService.GetAll().Where(x => x.ExpenseDate >= startDate && x.ExpenseDate <= endDate).ToList();

            model.ModelGroupBy = expenditureList.GroupBy(x => x.ExpenseDate.ToShortDateString()).Select(x => new SoldItemModel { PaymentTypeName = x.Key, TotalPrice = x.Sum(y => y.Amount), Expenselst = x.ToList() }).ToList();

            return View(model);
        }

        public ActionResult Sales(DateTime? startDate, DateTime? endDate)
        {
            ReportViewModel model = new ReportViewModel();

            if (startDate.HasValue && endDate.HasValue)
            {
                if (startDate.Value == endDate.Value)
                {
                    var newDate = startDate.Value.AddDays(1);
                    endDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 0, 0, 1);
                }
            }

            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Now.AddMonths(1);

            var items = GetAllSoldItems();

            items = items.Where(x => x.DateSold >= startDate && x.DateSold <= endDate).ToList();

            SoldItemIndexModel siim = new SoldItemIndexModel { ItemList = items };

            return View(siim);

        }

        //private static string GetConnectionString()
        //{
        //    return ConfigurationManager.ConnectionStrings["Core"].ConnectionString;
        //}


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

            //return lst;

        }

        public ActionResult SalesSummaryReport(DateTime? startDate, DateTime? endDate)
        {
            ReportViewModel model = new ReportViewModel();

            if (startDate.HasValue && endDate.HasValue)
            {
                if (startDate.Value == endDate.Value)
                {
                    var newDate = startDate.Value.AddDays(1);
                    endDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 0, 0, 1);
                }
            }

            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Now.AddMonths(1);

            var conn = ConfigurationManager.ConnectionStrings[1].ConnectionString;

            var ds = POSService.StockItemService.GetSoldItems(conn);

            var salesModel = ConvertToList(ds);

            model.SalesModel = salesModel.Where(x => x.DateSold >= startDate && x.DateSold <= endDate).OrderByDescending(x => x.DateSold).ToList();

            if (!startDate.HasValue && !endDate.HasValue)
            {
                model.SalesModel = salesModel.Where(x => x.DateSold.ToShortDateString().Equals(DateTime.Today.ToShortDateString(), StringComparison.InvariantCultureIgnoreCase)).OrderByDescending(x => x.DateSold).ToList();
            }

            model.ModelGroupBy = model.SalesModel.GroupBy(x => x.PaymentTypeName).Select(x => new SoldItemModel { PaymentTypeName = x.Key, ItemNewlst = x.ToList() }).ToList();

            return View(model);
        }

        //

        public ActionResult TotalSalesRoomsOnly(DateTime? startDate, DateTime? endDate)
        {
            ReportViewModel model = new ReportViewModel();

            if (startDate.HasValue && endDate.HasValue)
            {
                if (startDate.Value == endDate.Value)
                {
                    var newDate = startDate.Value.AddDays(1);
                    endDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 0, 0, 1);
                }
            }

            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Now.AddMonths(1);

            List<int> restAndBar = new List<int>();
            restAndBar.Add((int)RoomPaymentTypeEnum.Bar);
            restAndBar.Add((int)RoomPaymentTypeEnum.Laundry);
            restAndBar.Add((int)RoomPaymentTypeEnum.Restuarant);

            var allHotelPayments = _guestRoomAccountService.GetAll(HotelID).Where(x => x.PaymentMethodId != (int)PaymentMethodEnum.POSTBILL).ToList();

            var m = allHotelPayments.Where(x => x.TransactionDate >= startDate && x.TransactionDate <= endDate && (!restAndBar.Contains(x.PaymentTypeId))).ToList();

            var groupByGuestRoom = m.GroupBy(x => x.GuestRoom).Select(x => new AccomodationModel { ItemList = x.ToList(), GuestRoom = x.Key }).ToList();

            List<PersonAccomodationModel> pamList = new List<PersonAccomodationModel>();

            foreach (var gr in groupByGuestRoom)
            {
                var guestRoom = gr.GuestRoom;

                var gbl = gr.ItemList.GroupBy(x => x.TransactionDate.ToShortDateString()).Select(x => new PersonDateModel { DateSoldString = x.Key, ItemLst = x.ToList() }).ToList();

                foreach (var modella in gbl)
                {
                    var groupByPerson = modella.ItemLst.GroupBy(x => x.Person).Select(x => new SalesPersonModel { Person = x.Key, ItemLst = x.ToList() }).ToList();

                    foreach (var p in groupByPerson)
                    {
                        PersonAccomodationModel pamNew = new PersonAccomodationModel();

                        pamNew.GuestRoom = gr.GuestRoom;

                        pamNew.DateSold = modella.DateSoldString;

                        var totalPaidByGuest = p.ItemLst.Where(x => x.RoomPaymentType.PaymentStatusId == (int)RoomPaymentStatusEnum.Credit).Sum(x => x.Amount);
                        var totalPaidToGuest = p.ItemLst.Where(x => x.RoomPaymentType.PaymentStatusId == (int)RoomPaymentStatusEnum.Debit).Sum(x => x.Amount);
                        var guestTotal = totalPaidByGuest - totalPaidToGuest;

                        pamNew.Person = p.Person;
                        pamNew.TotalPaidByGuest = totalPaidByGuest;
                        pamNew.TotalPaidToGuest = totalPaidToGuest;
                        pamNew.GuestTotal = guestTotal;

                        //var pos = modella.ItemLst.Where(x => x.RoomPaymentType.PaymentStatusId == (int)RoomPaymentStatusEnum.Credit && x.PaymentMethodId == (int)PaymentMethodEnum.CreditCard).Sum(x => x.Amount);
                        //var cheque = modella.ItemLst.Where(x => x.RoomPaymentType.PaymentStatusId == (int)RoomPaymentStatusEnum.Credit && x.PaymentMethodId == (int)PaymentMethodEnum.Cheque).Sum(x => x.Amount);
                        //var cash = modella.ItemLst.Where(x => x.RoomPaymentType.PaymentStatusId == (int)RoomPaymentStatusEnum.Credit && x.PaymentMethodId == (int)PaymentMethodEnum.Cash).Sum(x => x.Amount);

                        var pos = p.ItemLst.Where(x => x.RoomPaymentType.PaymentStatusId == (int)RoomPaymentStatusEnum.Credit && x.PaymentMethodId == (int)PaymentMethodEnum.CreditCard).Sum(x => x.Amount);
                        var cheque = p.ItemLst.Where(x => x.RoomPaymentType.PaymentStatusId == (int)RoomPaymentStatusEnum.Credit && x.PaymentMethodId == (int)PaymentMethodEnum.Cheque).Sum(x => x.Amount);
                        var cash = p.ItemLst.Where(x => x.RoomPaymentType.PaymentStatusId == (int)RoomPaymentStatusEnum.Credit && x.PaymentMethodId == (int)PaymentMethodEnum.Cash).Sum(x => x.Amount);

                        pamNew.Cash = cash;
                        pamNew.Cheque = cheque;
                        pamNew.CreditCard = pos;
                        pamNew.Terminal = p.ItemLst.GroupBy(x => x.RoomPaymentType).Select(x => new TerminalModel { Terminal = x.Key, ItemList = x.ToList() }).ToList();


                        pamList.Add(pamNew);
                    }
                }
            }

            model.ModelGroupByAccomodation = pamList.GroupBy(x => x.DateSold).Select(x => new SoldItemModelAccomodation { DateSold = x.Key, ItemLst = x.ToList(), TotalAmount = x.Sum(y => y.GuestTotal) }).ToList();

            return View(model);
        }


        public ActionResult EmployeeAttendance(DateTime? startDate, DateTime? endDate)
        {
            ReportViewModel model = new ReportViewModel();

            if (startDate.HasValue && endDate.HasValue)
            {
                if (startDate.Value == endDate.Value)
                {
                    var newDate = startDate.Value.AddDays(1);
                    endDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 0, 0, 1);
                }
            }

            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Now.AddMonths(1);

            var listEmployee = _employeeShiftService.GetAll(HotelID).Where(x => x.ShiftDate >= startDate && x.ShiftDate <= endDate).ToList();

            var groupByList = listEmployee.GroupBy(x => x.Person).Select(x => new EmployeeGroupByModel { Person = x.Key, ItemList = x.ToList(), TotalAmount = x.Sum(y => y.TotalSales) }).ToList();

            model.EmployeeGroupByList = groupByList;

            return View(model);
        }

        public ActionResult TotalSalesBarRestaraurantOnly(DateTime? startDate, DateTime? endDate)
        {
            ReportViewModel model = new ReportViewModel();

            if (startDate.HasValue && endDate.HasValue)
            {
                if (startDate.Value == endDate.Value)
                {
                    var newDate = startDate.Value.AddDays(1);
                    endDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 0, 0, 1);
                }
            }

            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Now.AddMonths(1);

            List<int> restAndBar = new List<int>();

            restAndBar.Add((int)RoomPaymentTypeEnum.Bar);
            restAndBar.Add((int)RoomPaymentTypeEnum.Laundry);
            restAndBar.Add((int)RoomPaymentTypeEnum.Restuarant);

            var allHotelPayments = _guestRoomAccountService.GetAll(HotelID).Where(x => x.PaymentMethodId == (int)PaymentMethodEnum.POSTBILL).ToList();

            var pp = allHotelPayments.Where(x => x.TransactionDate >= startDate && x.TransactionDate <= endDate).ToList();

            var pp1 = pp.Select(x => x.PaymentTypeId).ToList();

            var m = allHotelPayments.Where(x => x.TransactionDate >= startDate && x.TransactionDate <= endDate && (restAndBar.Contains(x.PaymentTypeId))).ToList();

            var groupByGuestRoom = m.GroupBy(x => x.GuestRoom).Select(x => new AccomodationModel { ItemList = x.ToList(), GuestRoom = x.Key }).ToList();

            List<PersonAccomodationModel> pamList = new List<PersonAccomodationModel>();

            foreach (var gr in groupByGuestRoom)
            {
                var guestRoom = gr.GuestRoom;

                var gbl = gr.ItemList.GroupBy(x => x.TransactionDate.ToShortDateString()).Select(x => new PersonDateModel { DateSoldString = x.Key, ItemLst = x.ToList() }).ToList();

                foreach (var modella in gbl)
                {
                    var groupByPerson = modella.ItemLst.GroupBy(x => x.Person).Select(x => new SalesPersonModel { Person = x.Key, ItemLst = x.ToList() }).ToList();

                    foreach (var p in groupByPerson)
                    {
                        PersonAccomodationModel pamNew = new PersonAccomodationModel();

                        pamNew.GuestRoom = gr.GuestRoom;

                        pamNew.DateSold = modella.DateSoldString;

                        var totalPaidByGuest = p.ItemLst.Where(x => x.RoomPaymentType.PaymentStatusId == (int)RoomPaymentStatusEnum.Credit).Sum(x => x.Amount);
                        var totalPaidToGuest = p.ItemLst.Where(x => x.RoomPaymentType.PaymentStatusId == (int)RoomPaymentStatusEnum.Debit).Sum(x => x.Amount);
                        var guestTotal = totalPaidByGuest - totalPaidToGuest;

                        pamNew.Person = p.Person;
                        pamNew.TotalPaidByGuest = totalPaidByGuest;
                        pamNew.TotalPaidToGuest = totalPaidToGuest;
                        pamNew.GuestTotal = guestTotal;

                        //var pos = modella.ItemLst.Where(x => x.RoomPaymentType.PaymentStatusId == (int)RoomPaymentStatusEnum.Credit && x.PaymentMethodId == (int)PaymentMethodEnum.CreditCard).Sum(x => x.Amount);
                        //var cheque = modella.ItemLst.Where(x => x.RoomPaymentType.PaymentStatusId == (int)RoomPaymentStatusEnum.Credit && x.PaymentMethodId == (int)PaymentMethodEnum.Cheque).Sum(x => x.Amount);
                        //var cash = modella.ItemLst.Where(x => x.RoomPaymentType.PaymentStatusId == (int)RoomPaymentStatusEnum.Credit && x.PaymentMethodId == (int)PaymentMethodEnum.Cash).Sum(x => x.Amount);

                        var pos = p.ItemLst.Where(x => x.RoomPaymentType.PaymentStatusId == (int)RoomPaymentStatusEnum.Credit && x.PaymentMethodId == (int)PaymentMethodEnum.CreditCard).Sum(x => x.Amount);
                        var cheque = p.ItemLst.Where(x => x.RoomPaymentType.PaymentStatusId == (int)RoomPaymentStatusEnum.Credit && x.PaymentMethodId == (int)PaymentMethodEnum.Cheque).Sum(x => x.Amount);
                        var cash = p.ItemLst.Where(x => x.RoomPaymentType.PaymentStatusId == (int)RoomPaymentStatusEnum.Credit && x.PaymentMethodId == (int)PaymentMethodEnum.Cash).Sum(x => x.Amount);

                        pamNew.Cash = cash;
                        pamNew.Cheque = cheque;
                        pamNew.CreditCard = pos;
                        pamNew.Terminal = p.ItemLst.GroupBy(x => x.RoomPaymentType).Select(x => new TerminalModel { Terminal = x.Key, ItemList = x.ToList() }).ToList();


                        pamList.Add(pamNew);
                    }
                }
            }

            model.ModelGroupByAccomodation = pamList.GroupBy(x => x.DateSold).Select(x => new SoldItemModelAccomodation { DateSold = x.Key, ItemLst = x.ToList(), TotalAmount = x.Sum(y => y.GuestTotal) }).ToList();

            return View(model);
        }
        public ActionResult TotalSalesRooms(DateTime? startDate, DateTime? endDate)
        {
            ReportViewModel model = new ReportViewModel();

            if (startDate.HasValue && endDate.HasValue)
            {
                if (startDate.Value == endDate.Value)
                {
                    var newDate = startDate.Value.AddDays(1);
                    endDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 0, 0, 1);
                }
            }

            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Now.AddMonths(1);

            var allHotelPayments = _guestRoomAccountService.GetAll(HotelID).Where(x => x.PaymentMethodId != (int)PaymentMethodEnum.POSTBILL).ToList();

            var m = allHotelPayments.Where(x => x.TransactionDate >= startDate && x.TransactionDate <= endDate).ToList();

            var groupByGuestRoom = m.GroupBy(x => x.GuestRoom).Select(x => new AccomodationModel { ItemList = x.ToList(), GuestRoom = x.Key }).ToList();

            List<PersonAccomodationModel> pamList = new List<PersonAccomodationModel>();

            foreach (var gr in groupByGuestRoom)
            {
                var guestRoom = gr.GuestRoom;

                var gbl = gr.ItemList.GroupBy(x => x.TransactionDate.ToShortDateString()).Select(x => new PersonDateModel { DateSoldString = x.Key, ItemLst = x.ToList() }).ToList();

                foreach (var modella in gbl)
                {
                    var groupByPerson = modella.ItemLst.GroupBy(x => x.Person).Select(x => new SalesPersonModel { Person = x.Key, ItemLst = x.ToList() }).ToList();

                    foreach (var p in groupByPerson)
                    {
                        PersonAccomodationModel pamNew = new PersonAccomodationModel();

                        pamNew.GuestRoom = gr.GuestRoom;

                        pamNew.DateSold = modella.DateSoldString;

                        var totalPaidByGuest = p.ItemLst.Where(x => x.RoomPaymentType.PaymentStatusId == (int)RoomPaymentStatusEnum.Credit).Sum(x => x.Amount);
                        var totalPaidToGuest = p.ItemLst.Where(x => x.RoomPaymentType.PaymentStatusId == (int)RoomPaymentStatusEnum.Debit).Sum(x => x.Amount);
                        var guestTotal = totalPaidByGuest - totalPaidToGuest;

                        pamNew.Person = p.Person;
                        pamNew.TotalPaidByGuest = totalPaidByGuest;
                        pamNew.TotalPaidToGuest = totalPaidToGuest;
                        pamNew.GuestTotal = guestTotal;

                        //var pos = modella.ItemLst.Where(x => x.RoomPaymentType.PaymentStatusId == (int)RoomPaymentStatusEnum.Credit && x.PaymentMethodId == (int)PaymentMethodEnum.CreditCard).Sum(x => x.Amount);
                        //var cheque = modella.ItemLst.Where(x => x.RoomPaymentType.PaymentStatusId == (int)RoomPaymentStatusEnum.Credit && x.PaymentMethodId == (int)PaymentMethodEnum.Cheque).Sum(x => x.Amount);
                        //var cash = modella.ItemLst.Where(x => x.RoomPaymentType.PaymentStatusId == (int)RoomPaymentStatusEnum.Credit && x.PaymentMethodId == (int)PaymentMethodEnum.Cash).Sum(x => x.Amount);

                        var pos = p.ItemLst.Where(x => x.RoomPaymentType.PaymentStatusId == (int)RoomPaymentStatusEnum.Credit && x.PaymentMethodId == (int)PaymentMethodEnum.CreditCard).Sum(x => x.Amount);
                        var cheque = p.ItemLst.Where(x => x.RoomPaymentType.PaymentStatusId == (int)RoomPaymentStatusEnum.Credit && x.PaymentMethodId == (int)PaymentMethodEnum.Cheque).Sum(x => x.Amount);
                        var cash = p.ItemLst.Where(x => x.RoomPaymentType.PaymentStatusId == (int)RoomPaymentStatusEnum.Credit && x.PaymentMethodId == (int)PaymentMethodEnum.Cash).Sum(x => x.Amount);

                        pamNew.Cash = cash;
                        pamNew.Cheque = cheque;
                        pamNew.CreditCard = pos;
                        pamNew.Terminal = p.ItemLst.GroupBy(x => x.RoomPaymentType).Select(x => new TerminalModel { Terminal = x.Key, ItemList = x.ToList() }).ToList();


                        pamList.Add(pamNew);
                    }
                }
            }

            model.ModelGroupByAccomodation = pamList.GroupBy(x => x.DateSold).Select(x => new SoldItemModelAccomodation { DateSold = x.Key, ItemLst = x.ToList(), TotalAmount = x.Sum(y => y.GuestTotal) }).ToList();

            return View(model);
        }


        public ActionResult OtherIncome(DateTime? startDate, DateTime? endDate)
        {
            ReportViewModel model = new ReportViewModel();

            if (startDate.HasValue && endDate.HasValue)
            {
                if (startDate.Value == endDate.Value)
                {
                    var newDate = startDate.Value.AddDays(1);
                    endDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 0, 0, 1);
                }
            }

            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Now.AddMonths(1);

            var conn = ConfigurationManager.ConnectionStrings[1].ConnectionString;

            var ds = POSService.StockItemService.GetSoldItems(conn);

            var salesModel = ConvertToList(ds);

            model.SalesModel = salesModel.Where(x => x.DateSold >= startDate && x.DateSold <= endDate && x.GuestId == 0).OrderByDescending(x => x.DateSold).ToList();

            if (!startDate.HasValue && !endDate.HasValue)
            {
                model.SalesModel = salesModel.Where(x => x.DateSold.ToShortDateString().Equals(DateTime.Today.ToShortDateString(), StringComparison.InvariantCultureIgnoreCase)).OrderByDescending(x => x.DateSold).ToList();
            }

            model.ModelGroupBy = model.SalesModel.GroupBy(x => x.PaymentTypeName).Select(x => new SoldItemModel { PaymentTypeName = x.Key, TotalPrice = x.Sum(y => y.TotalPrice), ItemNewlst = x.ToList() }).ToList();

            return View(model);
        }

        public ActionResult Ledgers()
        {
            var companyList = _businessAccountService.GetAll(1).ToList();

            ReportViewModel model = new ReportViewModel();

            List<CompanyPaymentModel> lst = new List<CompanyPaymentModel>();

            foreach (var existingCompany in companyList)
            {
                var allItemisedItems = existingCompany.SoldItemsAlls.Where(y => y.IsActive).ToList();

                var allRoomsFound = existingCompany.GuestRooms.SelectMany(x => x.GuestRoomAccounts).ToList();

                var allPaymentsMade = _businessCorporateAccountService.GetAll(HotelID).Where(x => x.BusinessAccountId == existingCompany.Id).ToList();

                lst.Add(new CompanyPaymentModel { Name = existingCompany.Name, Bill = (allItemisedItems.Sum(x => x.TotalPrice) + allRoomsFound.Sum(y => y.Amount)), Payments = allPaymentsMade.Sum(x => x.Amount), Balance = allPaymentsMade.Sum(x => x.Amount) - allItemisedItems.Sum(x => x.TotalPrice) });

            }

            model.CompanyAccountsList = lst;

            return View(model);
        }


        public ActionResult Discounts(DateTime? startDate, DateTime? endDate)
        {
            ReportViewModel model = new ReportViewModel();

            if (startDate.HasValue && endDate.HasValue)
            {
                if (startDate.Value == endDate.Value)
                {
                    var newDate = startDate.Value.AddDays(1);
                    endDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 0, 0, 1);
                }
            }

            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Now.AddMonths(1);

            var allDiscounts = _salesDiscountService.GetAll().Where(x => x.DiscountDate >= startDate && x.DiscountDate <= endDate).ToList();

            model.AllDiscounts = allDiscounts;

            return View(model);

        }



        public ActionResult IncomeStatementCredit(DateTime? startDate, DateTime? endDate)
        {
            ReportViewModel model = new ReportViewModel();

            if (startDate.HasValue && endDate.HasValue)
            {
                if (startDate.Value == endDate.Value)
                {
                    var newDate = startDate.Value.AddDays(1);
                    endDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 0, 0, 1);
                }
            }

            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Now.AddMonths(1);


            var allCashPayments = _guestRoomAccountService.GetAll("GuestRoom,GuestRoom.Guest")
                .Where(x => x.TransactionDate >= startDate && x.TransactionDate <= endDate && x.PaymentMethodId == (int)PaymentMethodEnum.CreditCard).ToList();

            model.AllCashPayments = allCashPayments;

            return View(model);
        }


        public ActionResult AllRecievable(DateTime? startDate, DateTime? endDate)
        {
            ReportViewModel model = new ReportViewModel();

            if (startDate.HasValue && endDate.HasValue)
            {
                if (startDate.Value == endDate.Value)
                {
                    var newDate = startDate.Value.AddDays(1);
                    endDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 0, 0, 1);
                }
            }

            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Now.AddMonths(1);


            var allCashPayments = _guestRoomAccountService.GetAll("GuestRoom,GuestRoom.Guest")
                .Where(x => x.TransactionDate >= startDate && x.TransactionDate <= endDate).ToList();

            model.AllCashPayments = allCashPayments;

            return View(model);
        }

        public ActionResult IncomeStatementTotal(DateTime? startDate, DateTime? endDate)
        {
            ReportViewModel model = new ReportViewModel();

            if (startDate.HasValue && endDate.HasValue)
            {
                if (startDate.Value == endDate.Value)
                {
                    var newDate = startDate.Value.AddDays(1);
                    endDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 0, 0, 1);
                }
            }

            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Now.AddMonths(1);


            var allCashPayments = _guestRoomAccountService.GetAll("GuestRoom,GuestRoom.Guest")
                .Where(x => x.TransactionDate >= startDate && x.TransactionDate <= endDate).ToList();

            model.AllCashPayments = allCashPayments;

            return View(model);
        }

        public ActionResult IncomeStatement(DateTime? startDate, DateTime? endDate)
        {
            ReportViewModel model = new ReportViewModel();

            if (startDate.HasValue && endDate.HasValue)
            {
                if (startDate.Value == endDate.Value)
                {
                    var newDate = startDate.Value.AddDays(1);
                    endDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 0, 0, 1);
                }
            }

            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Now.AddMonths(1);


            var allCashPayments = _guestRoomAccountService.GetAll("GuestRoom,GuestRoom.Guest").Where(x => x.TransactionDate >= startDate && x.TransactionDate <= endDate && x.PaymentMethodId == (int)PaymentMethodEnum.Cash).ToList();

            model.AllCashPayments = allCashPayments;

            return View(model);
        }

        public ActionResult OtherIncomeStatement(DateTime? startDate, DateTime? endDate)
        {
            ReportViewModel model = new ReportViewModel();

            if (startDate.HasValue && endDate.HasValue)
            {
                if (startDate.Value == endDate.Value)
                {
                    var newDate = startDate.Value.AddDays(1);
                    endDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 0, 0, 1);
                }
            }

            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Now.AddMonths(1);

            var bal = _businessCorporateAccountService.GetAllInclde("BusinessAccount").Where(x => x.TransactionDate >= startDate && x.TransactionDate <= endDate).ToList();

            var all = _purchaseOrderService.GetAllByInclude("PurchaseOrderItems").ToList();

            var accountsPaidin2 = all.Where(x => x.OrderDate >= startDate.Value && x.OrderDate <= endDate && x.PurchaseOrderItems.Any(y => y.Returns > 0)).ToList();

            model.Bal = bal;

            model.Returns = accountsPaidin2;

            return View(model);
        }

        public ActionResult TotalSalesNonGuest(DateTime? startDate, DateTime? endDate)
        {
            ReportViewModel model = new ReportViewModel();

            if (startDate.HasValue && endDate.HasValue)
            {
                if (startDate.Value == endDate.Value)
                {
                    var newDate = startDate.Value.AddDays(1);
                    endDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 0, 0, 1);
                }
            }

            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Now.AddMonths(1);

            var conn = ConfigurationManager.ConnectionStrings[1].ConnectionString;

            var ds = POSService.StockItemService.GetSoldItems(conn);

            var salesModel = ConvertToList(ds);

            model.SalesModel = salesModel.Where(x => x.DateSold >= startDate && x.DateSold <= endDate && !x.GuestId.HasValue).OrderByDescending(x => x.DateSold).ToList();

            if (!startDate.HasValue && !endDate.HasValue)
            {
                model.SalesModel = salesModel.Where(x => x.DateSold.ToShortDateString().Equals(DateTime.Today.ToShortDateString(), StringComparison.InvariantCultureIgnoreCase)).OrderByDescending(x => x.DateSold).ToList();
            }

            model.ModelGroupBy = model.SalesModel.GroupBy(x => x.PaymentTypeName).Select(x => new SoldItemModel { PaymentTypeName = x.Key, TotalPrice = x.Sum(y => y.TotalPrice), ItemNewlst = x.ToList() }).ToList();

            return View(model);
        }

        public ActionResult TotalSales(DateTime? startDate, DateTime? endDate)
        {
            if (startDate.HasValue && endDate.HasValue)
            {
                if (startDate.Value == endDate.Value)
                {
                    var newDate = startDate.Value.AddDays(1);
                    endDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 0, 0, 1);
                }
            }

            ReportViewModel model = new ReportViewModel();

            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);


            if (!endDate.HasValue)
                endDate = DateTime.Now.AddMonths(1);

            var conn = ConfigurationManager.ConnectionStrings[1].ConnectionString;

            var ds = POSService.StockItemService.GetSoldItems(conn);

            var salesModel = ConvertToList(ds);

            model.SalesModel = salesModel.Where(x => x.DateSold >= startDate && x.DateSold <= endDate).OrderByDescending(x => x.DateSold).ToList();

            if (!startDate.HasValue && !endDate.HasValue)
            {
                model.SalesModel = salesModel.Where(x => x.DateSold.ToShortDateString().Equals(DateTime.Today.ToShortDateString(), StringComparison.InvariantCultureIgnoreCase)).OrderByDescending(x => x.DateSold).ToList();
            }

            model.ModelGroupBy = model.SalesModel.GroupBy(x => x.PaymentTypeName).Select(x => new SoldItemModel { PaymentTypeName = x.Key, TotalPrice = x.Sum(y => y.TotalPrice), ItemNewlst = x.ToList() }).ToList();

            return View(model);
        }

        private List<SoldItemModel> ConvertToList(System.Data.DataSet ds)
        {
            List<SoldItemModel> lst = new List<SoldItemModel>();

            int count = ds.Tables[0].Rows.Count;

            for (int i = 0; i < count; i++)
            {
                SoldItemModel sim = new SoldItemModel();
                sim.Description = ds.Tables[0].Rows[i][0].ToString();
                sim.Quantity = int.Parse(ds.Tables[0].Rows[i][1].ToString());
                sim.TotalPrice = decimal.Parse(ds.Tables[0].Rows[i][2].ToString());
                sim.DateSold = DateTime.Parse(ds.Tables[0].Rows[i][3].ToString());
                sim.PersonName = ds.Tables[0].Rows[i][4].ToString();
                sim.PaymentTypeName = ds.Tables[0].Rows[i][5].ToString();
                sim.PaymentMethodNote = ds.Tables[0].Rows[i][6].ToString();
                sim.PaymentMethodName = ds.Tables[0].Rows[i][7].ToString();
                sim.TimeOfSale = DateTime.Parse(ds.Tables[0].Rows[i][8].ToString());
                try
                {
                    sim.GuestId = int.Parse(ds.Tables[0].Rows[i][9].ToString());
                }
                catch
                {
                    sim.GuestId = null;
                }

                sim.PaymentMethodId = int.Parse(ds.Tables[0].Rows[i][10].ToString());

                lst.Add(sim);
            }

            return lst;
        }

        public ActionResult GuestDetails(DateTime? startDate, DateTime? endDate)
        {
            ReportViewModel model = new ReportViewModel();

            if (startDate.HasValue && endDate.HasValue)
            {
                if (startDate.Value == endDate.Value)
                {
                    var newDate = startDate.Value.AddDays(1);
                    endDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 0, 0, 1);
                }
            }

            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Now.AddMonths(1);


            model.HotelGuests = _guestService.GetByQuery(HotelID).Where(x => x.IsActive && !x.IsFutureReservation && x.GuestRooms.Where(y => y.CheckinDate > startDate.Value && y.IsActive && y.GroupBooking).Count() > 0).OrderByDescending(x => x.CreatedDate).ToList();

            return View(model);
        }
    }
}