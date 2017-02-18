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
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClosedXML.Excel;
using System.IO;

namespace BarAndRestaurantMate.Controllers
{
    [HandleError(View = "CustomErrorView")]
    public class BarReportController : PdfViewController
    {
        private IStorePointService _storePointService;
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
        private readonly IPaymentService _paymentService;

        private readonly IPurchaseOrderService _purchaseOrderService;
        private readonly IFoodMatrixService _foodMatrixService;
        private readonly IUsedStockItemService _usedStockItemService;
        private readonly ISalesDiscountService _salesDiscountService;
        private readonly IDistributionPointService _distributionPointService;
        private readonly IBatchService _batchService;
        private readonly ITableItemService _tableItemService;




        //private readonly IStockItemService _stockItemService;

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

        public BarReportController()
        {
            int.TryParse(ConfigurationManager.AppSettings["HotelAccountsTime"].ToString(CultureInfo.InvariantCulture), out _hotelAccountsTime);

            _guestService = new GuestService();
            _storePointService = new StorePointService();
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
            _distributionPointService = new DistributionPointService();
            _batchService = new BatchService();
            _tableItemService = new TableItemService();
            _paymentService = new PaymentService();
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
            if(!string.IsNullOrEmpty(clickedButton) && clickedButton.ToUpper().StartsWith("P"))
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

            var lst = _guestRoomService.GetAll(HotelID).Where(x => !x.IsActive &&  x.CheckoutDate >= startDate && x.CheckoutDate <= endDate).OrderBy(x => x.CheckoutDate).ToList();

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

        [HttpGet]
        public ActionResult SendEmail(bool? saved)
        {
            return View(new ReportViewModel { ItemSaved = saved });
        }


        [HttpPost]
        public ActionResult SendEmail(DateTime? startDate, DateTime? endDate)
        {
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

            var dest = GetOwnersTelephone();

            var now = DateTime.Today.AddDays(-1);

            var salesTotalRestaurant = GetRestaurantTotal(startDate, endDate);

            var salesTotalHotel = GetHotelTotal(startDate, endDate);

            var total = salesTotalHotel + salesTotalRestaurant;

            var emailTemplate = @"<p style='margin:0px;padding:0px;font-size:12px;font-family:Arial, Helvetica, sans-serif;color:#555;' id='yui_3_16_0_ym19_1_1463261898755_4224'>Warm Greetings,<br>
                                <br>This is to kindly inform you of your daily sales records, the sales details are listed below : <br>
                                <br>Start Date: XXDATEXX <br><br>
                                <br>End Date: XXENDDATEXX <br><br>
                                <br>Non Room Sales: XXNONROOMSALESXX <br><br>
                                <br>Room Sales: XXROOMSALESXX <br><br>
                                <br>Total Sales: XXTOTALSALESXX <br><br>
                                </p>";

            emailTemplate = emailTemplate.Replace("XXDATEXX", startDate.Value.ToString());
            emailTemplate = emailTemplate.Replace("XXENDDATEXX", endDate.Value.ToString());
            emailTemplate = emailTemplate.Replace("XXNONROOMSALESXX", salesTotalRestaurant.ToString());
            emailTemplate = emailTemplate.Replace("XXROOMSALESXX", salesTotalHotel.ToString());
            emailTemplate = emailTemplate.Replace("XXTOTALSALESXX", total.ToString());


            try
            {
                var emails = dest.Split(',').ToList();

                foreach (var email in emails)
                {

                    MailMessage mail = new MailMessage("academyvistang@gmail.com", email, "Your daily sales report", emailTemplate);
                    mail.From = new MailAddress("academyvistang@gmail.com", "HotelMate");
                    mail.IsBodyHtml = true; // necessary if you're using html email
                    NetworkCredential credential = new NetworkCredential("academyvistang@gmail.com", "Lauren280701");
                    SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                    smtp.EnableSsl = true;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = credential;
                    smtp.Send(mail);
                }
            }
            catch
            {

            }


            return RedirectToAction("SendEmail", new { saved = true });
        }

        private string GetOwnersEmail()
        {

            var ownersTelephone = string.Empty;

            try
            {
                ownersTelephone = ConfigurationManager.AppSettings["OwnersEmail"].ToString();
            }
            catch
            {
                ownersTelephone = "";
            }

            return ownersTelephone;
        }

        private string GetOwnersTelephone()
        {
            
            var ownersTelephone = string.Empty;

            try
            {
                ownersTelephone = ConfigurationManager.AppSettings["OwnersTelephone"].ToString();
            }
            catch
            {
                ownersTelephone = "";
            }

            return ownersTelephone;
        }

        private decimal GetHotelTotal(DateTime? startDate, DateTime? endDate)
        {
            DateTime start = startDate.Value;

            DateTime end = endDate.Value;

            var total = decimal.Zero;

            GuestRoomAccountService sis = new GuestRoomAccountService();

            total = sis.GetAll("").Where(x => x.RoomPaymentType.PaymentStatusId == 2 && (x.TransactionDate >= start && x.TransactionDate <= end)).Sum(x => x.Amount);

            return total;
        }

        private decimal GetRestaurantTotal(DateTime? startDate, DateTime? endDate)
        {
            DateTime start = startDate.Value;

            DateTime end = endDate.Value;

            var total = decimal.Zero;

            SoldItemService sis = new SoldItemService();

            total = sis.GetAll().Where(x => x.IsActive && (x.DateSold >= start && x.DateSold <= end)).Sum(x => x.TotalPrice);

            return total;
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

            foreach(var item in allStockCookedFood)
            {
                var passId = id.Value.ToString() + "_" + item.Id.ToString();

                var fm = allFoodMatrix.FirstOrDefault(x => x.RawItemId == id.Value && x.FoodItemId == item.Id);

                if(fm != null)
                {
                    _foodMatrixService.Delete(fm);
                }

                if(Request[passId] != null)
                {
                    
                    var torf = Request[passId].ToString();
                    if(torf != "false")
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
            ReportViewModel model = new ReportViewModel();
            model.CookedFoodItems = allStockCookedFood;

            model.CookedFoodItems.ForEach(x => x.RawMatrix = GetRawMatrix(x.Id));
            return View(model);
        }

        private List<string> GetRawMatrix(long? id)
        {
            var matrix = _foodMatrixService.GetAll().Where(x => x.FoodItemId == id.Value).ToList();
            List<string> strList = new List<string>();

            foreach( var m in matrix)
            {
                var str = m.StockItem1.StockItemName +  " - " + m.Qty +  " Units";
                strList.Add(str);
            }

            return strList;
        }

        public ActionResult SetUpMatrix(int? id)
        {
            var matrix = _foodMatrixService.GetAll().Where(x => x.FoodItemId == id.Value).ToList();

            var allRawStock = POSService.StockItemService.GetStockItemsRaw(1).ToList();
            var allStock = POSService.StockItemService.GetStockItems(1).ToList();

            var allRawFood = allRawStock.Where(x => x.RawItem).ToList();
            var thisStock = allStock.FirstOrDefault(x => x.Id == id.Value);
            ReportViewModel model = new ReportViewModel();
            model.ThisStock = thisStock;

            if(matrix.Count == 0)
            {
                model.RawMaterialsItems = allRawFood;
                return View(model);
            }
            else
            {
                model.Matrix = matrix;
                var matrixRawItemsId = matrix.Select(x => x.RawItemId).ToList();
                var matrixRange = allRawFood.Where(x => !matrixRawItemsId.Contains((int)x.Id)).Select(x => new FoodMatrix { Id = 0, Qty = 0, StockItem1 = new StockItem { Id = (int)x.Id, Description = x.Description, StockItemName = x.StockItemName  },
                    FoodItemId = (int)thisStock.Id, RawItemId = (int)x.Id }).ToList();
                model.Matrix.AddRange(matrixRange);
                return View("SetUpMatrixExisting", model);
            }
        }

       
        [HttpPost]
        public ActionResult SetUpMatrix(int? id, int? poId, int? dummy)
        {
            var allRealStock = POSService.StockItemService.GetStockItemsRaw(1).ToList();

            var allItems = POSService.StockItemService.GetStockItems(1).ToList();

            var realPosItem = allItems.FirstOrDefault(x => x.Id == id.Value);

            var allStockItemIds = allRealStock.Where(x => x.RawItem).Select(x => x.Id).ToList();

            var thisUser = _personService.GetAllForLogin().Where(x => x.Username.ToUpper().Equals(User.Identity.Name.ToUpper())).FirstOrDefault();

            var previousFoodMatrix = _foodMatrixService.GetAll().Where(x => x.FoodItemId == id.Value).ToList();

            foreach(var pfm in previousFoodMatrix)
            {
                _foodMatrixService.Delete(pfm);
            }

            foreach (var itemId in allStockItemIds)
            {
                var name = "RawItem_" + itemId.ToString();

                var realStock = allRealStock.FirstOrDefault(x => x.Id == itemId);

                if (Request.Form[name] != null)
                {
                    var qty = 0;
                    int.TryParse(Request.Form[name].ToString(), out qty);

                    if (qty == 0)
                        continue;

                    var newFM = new FoodMatrix();
                    newFM.RawItemId = (int)realStock.Id;
                    newFM.FoodItemId = (int)realPosItem.Id;
                    newFM.Qty = qty;
                    _foodMatrixService.Create(newFM);
                }
            }

            return RedirectToAction("SetUpMatrix", new  {id });
        }

        public ActionResult Analysis(DateTime? startDate, DateTime? endDate)
        {
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

            var allRawMaterialStock = POSService.StockItemService.GetStockItemsRaw(1).Where(x => x.RawItem).ToList();

            var allSold = _soldItemService.GetAllInclude("StockItem, StockItem.FoodMatrices,StockItem.FoodMatrices1").Where(x => x.DateSold >= startDate && x.DateSold <= endDate && x.StockItem.CookedFood).ToList();

            //var allSoldIds = allSold.Select(x => x.StockItem.Id).ToList();

            var groupByItem = allSold.GroupBy(x => x.StockItem).Select(x => new SoldItemViewModel { Item = x.Key, Quantity = x.ToList().Sum(y => y.Qty) }).ToList();

            List<FinalAnalysisViewModel> lst = new List<FinalAnalysisViewModel>();

            var allRecievedItems = _purchaseOrderService.GetAllByInclude("PurchaseOrderItems").Where(x => x.IsRawItem && x.OrderDate >= startDate && x.OrderDate <= endDate).SelectMany(x => x.PurchaseOrderItems).ToList();

            var matrix = _foodMatrixService.GetAll().ToList();

            var rawItemPoModelLst = new List<GrandFinalAnalysisViewModel>();

            foreach (var raw in allRawMaterialStock)
            {
                var lstMatrix = matrix.Where(x => x.RawItemId == raw.Id).ToList();
                var totalUnitsUsed = 0;

                var poReceived = allRecievedItems.Where(x => x.ItemId == raw.Id).Sum(x => x.QtyRecieved);

                var totaLunitsRecieved = poReceived;

                var grand = new GrandFinalAnalysisViewModel {Id = raw.Id, TotalPoRecieved = totaLunitsRecieved, UnitPerItem = raw.UnitPrice };

                rawItemPoModelLst.Add(grand);

                foreach (var m in lstMatrix)
                {
                    var actualItemId = m.FoodItemId;
                    var totalNumberSold = 0;
                    var actualSoldItem = groupByItem.FirstOrDefault(x => x.Item.Id == actualItemId);

                    if (actualSoldItem != null)
                    {
                        totalNumberSold = actualSoldItem.Quantity;
                        totalUnitsUsed += totalNumberSold * m.Qty;

                        lst.Add(new FinalAnalysisViewModel
                        {
                            RawMaterial = raw,
                            CookedFood = actualSoldItem.Item,
                            TotalNumberSold = actualSoldItem.Quantity,
                            NumberOfUnitsPerMeal = m.Qty,
                            TotalUnits = (m.Qty * actualSoldItem.Quantity)
                        });
                    }
                }
            }

            var groupByList = lst.GroupBy(x => x.RawMaterial).Select(x => new FoodMatrixGroupByViewModel { ActualRawMaterial = x.Key, ItemLst = x.ToList() }).ToList();

            groupByList.ForEach(x => x.TotalPoRecieved = GetPoForItem((int)x.ActualRawMaterial.Id, rawItemPoModelLst));

            groupByList.ForEach(x => x.UnitPerItem = GetUnitPerItemForItem((int)x.ActualRawMaterial.Id, rawItemPoModelLst));


            model.MatrixAnalysisList = groupByList;

            return View(model);

        }

        private decimal GetUnitPerItemForItem(int id, List<GrandFinalAnalysisViewModel> rawItemPoModelLst)
        {
            
            var item = rawItemPoModelLst.FirstOrDefault(x => x.Id == id);

            if (item != null)
                return item.UnitPerItem;

            return decimal.Zero;
        }

        private decimal GetPoForItem(int id, List<GrandFinalAnalysisViewModel> rawItemPoModelLst)
        {
            var item = rawItemPoModelLst.FirstOrDefault(x => x.Id == id);

            if (item != null)
                return item.TotalPoRecieved;

            return decimal.Zero;
        }

        


        public ActionResult AnalysisB4(DateTime? startDate, DateTime? endDate)
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


            foreach(var raw in allStock)
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
                lst.Add(new RawMaterialViewModel { ThereIsProblem = thereIsProblem, UnitPrice = raw.UnitPrice, StartDate = startDate, EndDate = endDate, RawMaterial = raw,
                    NumberOfRawMaterialsUsed = usedRawMaterials.Count, NumberOfSoldItems = soldItems.Count, PerRawMaterial = perRawMaterialCount, PerRawMaterialMoney = perRawMaterialMoney, PossibilitiesList = foodNames });
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



        public ActionResult MainInventory(DateTime? startDate, DateTime? endDate, int? id, int? rawId)
        {
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

            var allStorePoints = _storePointService.GetAll().ToList();

            var allpos = _purchaseOrderService.GetAllByInclude("PurchaseOrderItems,PurchaseOrderItems.StockItem").Where(x => x.OrderDate >= startDate && x.OrderDate <= endDate).ToList();

            if(rawId.HasValue)
            {
                allpos = allpos.Where(x => x.IsRawItem).ToList();
            }
            else
            {
                allpos = allpos.Where(x => !x.IsRawItem).ToList();
            }

            var poItems = allpos.SelectMany(x => x.PurchaseOrderItems).ToList();

            allStorePoints.Insert(0, new StorePoint { Id = 0, Name = "--Pls Select--" });

            IEnumerable<SelectListItem> selectList =
              from c in allStorePoints
              select new SelectListItem
              {
                  Selected = (c.Id == id),
                  Text = c.Name,
                  Value = c.Id.ToString()
              };

            model.selectList = selectList;


            if (id.HasValue)
            {
                var allGroupBy = poItems.SelectMany(x => x.StorePointItems).Where(x => x.StorePointId == id.Value).GroupBy(x => x.PurchaseOrderItem.StockItem)
                .Select(x => new PurchaseOrderItemModel
                {
                    ActualStockItem = x.Key,
                    QuantityRecieved = x.ToList().Sum(y => y.Quantity),
                    QuantityReturned = x.ToList().Sum(y => y.Damaged),
                    QuantityRemaining = x.ToList().Sum(y => y.Remaining)
                }).ToList();

                model.MainStoreInventoryList = allGroupBy;

                model.StorePointId = id.Value;

                model.ReportName = "MainInventory";

                model.FileToDownloadPath = GenerateExcelSheetMainInventory(model, model.ReportName);

                return View(model);
            }
            else
            {
                var allGroupBy = poItems.SelectMany(x => x.StorePointItems).GroupBy(x => x.PurchaseOrderItem.StockItem)
                .Select(x => new PurchaseOrderItemModel
                {
                    ActualStockItem = x.Key,
                    QuantityRecieved = x.ToList().Sum(y => y.Quantity),
                    QuantityReturned = x.ToList().Sum(y => y.Damaged),
                    QuantityRemaining = x.ToList().Sum(y => y.Remaining)
                }).ToList();

                model.MainStoreInventoryList = allGroupBy;

                model.StorePointId = 0;

                model.ReportName = "MainInventory";

                model.FileToDownloadPath = GenerateExcelSheetMainInventory(model, model.ReportName);

                return View(model);
            }
            
        }

        private string GenerateExcelSheetMainInventory(ReportViewModel model, string reportName)
        {

            DataTable dt = new DataTable();

            dt.Columns.AddRange(new DataColumn[5] {
                                new DataColumn("Item", typeof(string)),
                                new DataColumn("Qty Recieved", typeof(string)),
                                new DataColumn("Qty Distributed",typeof(string)),
                                new DataColumn("Qty Returned",typeof(string)),
                                new DataColumn("Qty Remaining",typeof(string))
            });

            int p = 1;

            foreach (var ru in model.MainStoreInventoryList)
            {
                dt.Rows.Add(ru.ActualStockItem.StockItemName, ru.QuantityRecieved, (ru.QuantityRecieved - (ru.QuantityReturned + ru.QuantityRemaining)), ru.QuantityReturned, ru.QuantityRemaining);
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

        public ActionResult InventoryByPointCashierXX(DateTime? startDate, DateTime? endDate, int? id)
        {
            var thisUser = _personService.GetAllForLogin().Where(x => x.Username.ToUpper().Equals(User.Identity.Name.ToUpper())).FirstOrDefault();

            var allDistributionPoints = _distributionPointService.GetAll().Where(x => x.Id == thisUser.DistributionPointId.Value).ToList();

            id = thisUser.DistributionPointId.Value;

            allDistributionPoints.Insert(0, new DistributionPoint { Id = 0, Name = "--Pls Select--" });

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

            var allBatches = _batchService.GetAll().Where(x => x.BatchDate >= startDate && x.BatchDate <= endDate && x.DistributionPointId == id.Value).ToList();
            var allPosItems = service.GetAllInclude("StockItem").ToList();

            if (id.HasValue)
            {
                allSold = allSold.Where(x => x.DistributionPointId == id.Value).ToList();
                allPosItems = allPosItems.Where(x => x.DistributionPointId == id.Value).ToList();
            }


            List<InventoryViewModel> lst = new List<InventoryViewModel>();

            foreach (var s in allStock)
            {
                var poi = allBatches;
                var allPoiForStock = poi.ToList();
                var quantity = allPoiForStock.Sum(x => x.QuantityTransferred);
                var numberSold = allSold.Where(x => x.StockItem.Id == s.Id).Sum(x => x.Qty);
                var remaining = 0;
                var returns = 0;

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


            IEnumerable<SelectListItem> selectList =
               from c in allDistributionPoints
               select new SelectListItem
               {
                   Selected = (c.Id == id),
                   Text = c.Name,
                   Value = c.Id.ToString()
               };

            model.selectList = selectList;

            model.ReportName = "InventoryByPointCashier";

            model.Id = id;

            model.FileToDownloadPath = GenerateExcelSheetInventory(model, "InventoryByPointCashier");

            return View(model);

        }

        

        public ActionResult InventoryByPointCashier(DateTime? startDate, DateTime? endDate, int? id, int? rawId)
        {

            var thisUser = _personService.GetAllForLogin().Where(x => x.Username.ToUpper().Equals(User.Identity.Name.ToUpper())).FirstOrDefault();

            id = thisUser.DistributionPointId;

            var allDistributionPoints = _distributionPointService.GetAll().Where(x => x.Id == id.Value).ToList();

            //allDistributionPoints.Insert(0, new DistributionPoint { Id = 0, Name = "--Pls Select--" });

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

            var allBatches = _batchService.GetAll().Where(x => x.BatchDate >= startDate && x.BatchDate <= endDate).ToList();

            var allPosItems = service.GetAllInclude("StockItem").ToList();

            if (id.HasValue && id.Value > 0)
            {
                allSold = allSold.Where(x => x.DistributionPointId == id.Value).ToList();
                allPosItems = allPosItems.Where(x => x.DistributionPointId == id.Value).ToList();
                allBatches = allBatches.Where(x => x.DistributionPointId == id.Value).ToList();
            }


            List<InventoryViewModel> lst = new List<InventoryViewModel>();

            foreach (var s in allStock)
            {
                var poi = allBatches.Where(x => x.ItemId == s.Id);

                var quantity = poi.Sum(x => x.QuantityTransferred);
                var previousRemaining = poi.Sum(x => x.PreviousRemaining);
                var numberSold = allSold.Where(x => x.StockItem.Id == s.Id).Sum(x => x.Qty);
                var remaining = (quantity - numberSold) + previousRemaining;

                var returns = 0;

                var damaged = 0;

                bool thereIsProblem = false;

                if (remaining < (quantity - (returns + numberSold + damaged)))
                {
                    thereIsProblem = true;
                }

                lst.Add(new InventoryViewModel { IsRawItem = s.RawItem, ThereIsProblem = thereIsProblem, StockItemName = s, Acquired = quantity, Returns = returns, NumberSold = numberSold, Remaining = remaining, Damaged = damaged });
            }

            if (rawId.HasValue)
            {
                lst = lst.Where(x => x.IsRawItem).ToList();
            }
            else
            {
                lst = lst.Where(x => !x.IsRawItem).ToList();
            }

            model.CompleteInventoryList = lst;

            IEnumerable<SelectListItem> selectList =
               from c in allDistributionPoints
               select new SelectListItem
               {
                   Selected = (c.Id == id),
                   Text = c.Name,
                   Value = c.Id.ToString()
               };

            model.selectList = selectList;

            model.ReportName = "InventoryByPointCashier";

            model.Id = id;

            model.FileToDownloadPath = GenerateExcelSheetInventory(model, "InventoryByPointCashier");

            return View(model);

        }

        public ActionResult InventoryByPoint(DateTime? startDate, DateTime? endDate, int? id, int? rawId)
        {
            var allDistributionPoints = _distributionPointService.GetAll().ToList();

            allDistributionPoints.Insert(0, new DistributionPoint { Id = 0, Name = "--Pls Select--" });

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

            var allBatches = _batchService.GetAll().Where(x => x.BatchDate >= startDate && x.BatchDate <= endDate).ToList();
            
            var allPosItems = service.GetAllInclude("StockItem").ToList();

            if (id.HasValue && id.Value > 0)
            {
                allSold = allSold.Where(x => x.DistributionPointId == id.Value).ToList();
                allPosItems = allPosItems.Where(x => x.DistributionPointId == id.Value).ToList();
                allBatches = allBatches.Where(x => x.DistributionPointId == id.Value).ToList();
            }
                

            List<InventoryViewModel> lst = new List<InventoryViewModel>();

            foreach (var s in allStock)
            {
                var poi = allBatches.Where(x => x.ItemId == s.Id);
               
                var quantity = poi.Sum(x => x.QuantityTransferred);
                var previousRemaining = poi.Sum(x => x.PreviousRemaining);
                var numberSold = allSold.Where(x => x.StockItem.Id == s.Id).Sum(x => x.Qty);
                var remaining = (quantity - numberSold) + previousRemaining;

                var returns = 0;

                var damaged = 0;

                bool thereIsProblem = false;

                if (remaining < (quantity - (returns + numberSold + damaged)))
                {
                    thereIsProblem = true;
                }

                lst.Add(new InventoryViewModel { IsRawItem = s.RawItem, ThereIsProblem = thereIsProblem, StockItemName = s, Acquired = quantity, Returns = returns, NumberSold = numberSold, Remaining = remaining, Damaged = damaged });
            }

            if(rawId.HasValue)
            {
                lst = lst.Where(x => x.IsRawItem).ToList();
            }
            else
            {
                lst = lst.Where(x => !x.IsRawItem).ToList();
            }

            model.CompleteInventoryList = lst;

            IEnumerable<SelectListItem> selectList =
               from c in allDistributionPoints
               select new SelectListItem
               {
                   Selected = (c.Id == id),
                   Text = c.Name,
                   Value = c.Id.ToString()
               };

            model.selectList = selectList;

            model.ReportName = "InventoryByPoint";

            model.Id = id;

            model.FileToDownloadPath = GenerateExcelSheetInventory(model, "InventoryByPoint");

            return View(model);

        }


        public ActionResult Inventory(DateTime? startDate, DateTime? endDate, int? rawId)
        {

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

            var all = _purchaseOrderService.GetAllByInclude("PurchaseOrderItems,PurchaseOrderItems.StockItem").Where(x => x.OrderDate >= startDate && x.OrderDate <= endDate).ToList();

            if(rawId.HasValue)
            {
                all = all.Where(x => x.IsRawItem).ToList();
            }
            else
            {
                all = all.Where(x => !x.IsRawItem).ToList();
            }

            List<InventoryViewModel> lst = new List<InventoryViewModel>();

            model.CompleteInventoryList = all.SelectMany(x => x.PurchaseOrderItems).GroupBy(x => x.StockItem).Select(x => 
            
            new InventoryViewModel { StockItemNameName = x.Key.StockItem1.StockItemName, Damaged = 0, Returns = 0, Acquired = x.Sum(y => y.QtyRecieved),
                Remaining = x.Sum(y => y.Qty), Distributed = x.Sum(y => y.StorePointItems.Sum(v => v.Quantity)) } ).ToList();

            model.FileToDownloadPath = GenerateExcelSheetInventory(model, "Inventory");

            model.RawId = rawId;

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

            var all = _purchaseOrderService.GetAllByInclude("PurchaseOrderItems").ToList();

            var accountsPaidin2 = all.Where(x => x.OrderDate >= startDate.Value && x.OrderDate <= endDate && x.PurchaseOrderItems.Any(y => y.Returns > 0)).ToList();

            var accountsPaidin = _soldItemService.GetAll().Where(x => x.IsActive && !x.BusinessAccountId.HasValue && x.DateSold >= startDate.Value && x.DateSold <= endDate && (x.PaymentMethodId != (int)PaymentMethodEnum.POSTBILL)).ToList();

            var accountsPaidin1 = _businessCorporateAccountService.GetAll(1).Where(x => x.TransactionDate >= startDate.Value && x.TransactionDate <= endDate).ToList();
   
            var balanceSheetModel = accountsPaidin.Select(x => new BalanceSheetModel { TransactionDate = DateTime.Parse(x.DateSold.Value.ToShortDateString()), AmountPaidIn = x.TotalPrice, AmountPaidOut = decimal.Zero, Cashier = x.TransactionId, PaymentMentMethod = x.PaymentMethod, PaymentType = null }).ToList();

            balanceSheetModel.AddRange(accountsPaidin1.Select(x => new BalanceSheetModel { TransactionDate = DateTime.Parse(x.TransactionDate.ToShortDateString()), AmountPaidIn = x.Amount, AmountPaidOut = decimal.Zero, Cashier = x.TransactionId, PaymentMentMethod = x.PaymentMethod, PaymentType = null }));
            
            //Returns
            balanceSheetModel.AddRange(accountsPaidin2.Select(x => new BalanceSheetModel { TransactionDate = DateTime.Parse(x.OrderDate.ToShortDateString()), AmountPaidIn = x.PurchaseOrderItems.Sum(y => y.ReturnValue), AmountPaidOut = decimal.Zero, Cashier = null, PaymentMentMethod = null, PaymentType = null }));

           
            var allStaff = _personService.GetAllForLogin().ToList();

            balanceSheetModel = balanceSheetModel.OrderByDescending(x => x.TransactionDate).ToList();

            balanceSheetModel.ForEach(x => x.UserName = GetUserNameOrNull(allStaff, x.Cashier));

            var totalDiscount = _salesDiscountService.GetAll().Where(x => x.DiscountDate >= startDate && x.DiscountDate <= endDate).Sum(x => x.Amount);

            model.TotalCashDiscount = totalDiscount;

            var tPaidIn = balanceSheetModel.Sum(x => x.AmountPaidIn);

            var overallTotal = tPaidIn;

            var taxValue = GetRestaurantTax();

            if (taxValue > 0)
            {
                model.Tax = taxValue * overallTotal;
            }

            var tPaidOut = balanceSheetModel.Sum(x => x.AmountPaidOut);

            tPaidIn = tPaidIn - totalDiscount;

            model.FullBalance = tPaidIn - tPaidOut;

            model.BalanceSheet = balanceSheetModel;

            model.ConciseBalanceSheetSheet = balanceSheetModel.GroupBy(x => x.TransactionDate).Select(x => new ConciseBalanceSheetModel { ActualDate = x.Key, TotalReceiveable = x.Sum(y => y.AmountPaidIn), TotalPayaeble = x.Sum(z => z.AmountPaidOut) }).ToList();

           

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

            var overallTotal = tPaidIn;

            var taxValue = GetRestaurantTax();

            if (taxValue > 0)
            {
                model.Tax = taxValue * overallTotal;
            }

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

            var accountsPaidin = _paymentService.GetAllInclude("").Where(x => x.IsActive && !x.BusinessAccountId.HasValue && x.PaymentDate >= startDate.Value && x.PaymentDate <= endDate && (x.PaymentMethodId != (int)PaymentMethodEnum.POSTBILL)).ToList();

            //_soldItemService.GetAllInclude("StockItem").Where(x => x.IsActive && !x.BusinessAccountId.HasValue && x.DateSold >= startDate.Value && x.DateSold <= endDate && (x.PaymentMethodId != (int)PaymentMethodEnum.POSTBILL)).ToList();

            var accountsPaidin1 = _businessCorporateAccountService.GetAll(1).Where(x => x.TransactionDate >= startDate.Value && x.TransactionDate <= endDate).ToList();

            //var all = _purchaseOrderService.GetAllByInclude("PurchaseOrderItems").ToList();

            //var accountsPaidin2 = all.Where(x => x.OrderDate >= startDate.Value && x.OrderDate <= endDate && x.PurchaseOrderItems.Any(y => y.Returns > 0)).ToList();

            //var accountsPaidOut = all.Where(x => x.OrderDate >= startDate.Value && x.OrderDate <= endDate).ToList();

           // accountsPaidOut.Clear();
           // accountsPaidin2.Clear();

            var expenses = GetAllItems().Where(x => x.ExpenseDate >= startDate && x.ExpenseDate <= endDate).ToList();

            var balanceSheetModel = accountsPaidin.Select(x => new BalanceSheetModel { TransactionDate = DateTime.Parse(x.PaymentDate.ToShortDateString()), AmountPaidIn = GetProfit(x), AmountPaidOut = decimal.Zero, Cashier = x.CashierId, PaymentMentMethod = x.PaymentMethod, PaymentType = null }).ToList();

            balanceSheetModel.AddRange(accountsPaidin1.Select(x => new BalanceSheetModel { TransactionDate = DateTime.Parse(x.TransactionDate.ToShortDateString()), AmountPaidIn = x.Amount, AmountPaidOut = decimal.Zero, Cashier = x.TransactionId, PaymentMentMethod = x.PaymentMethod, PaymentType = null }));

            //Returns
            //balanceSheetModel.AddRange(accountsPaidin2.Select(x => new BalanceSheetModel { TransactionDate = DateTime.Parse(x.OrderDate.ToShortDateString()), AmountPaidIn = x.PurchaseOrderItems.Sum(y => y.ReturnValue), AmountPaidOut = decimal.Zero, Cashier = null, PaymentMentMethod = null, PaymentType = null }));

            //balanceSheetModel.AddRange(accountsPaidOut.Select(x => new BalanceSheetModel { TransactionDate = DateTime.Parse(x.OrderDate.ToShortDateString()), AmountPaidIn = decimal.Zero, AmountPaidOut = x.NetValue, Cashier = x.RaisedBy, PaymentMentMethod = null, PaymentType = null }));

            balanceSheetModel.AddRange(expenses.Select(x => new BalanceSheetModel { TransactionDate = DateTime.Parse(x.ExpenseDate.ToShortDateString()), AmountPaidOut = x.Amount, AmountPaidIn = decimal.Zero, Cashier = x.StaffId, PaymentMentMethod = null, PaymentType = null }).ToList());

            var allStaff = _personService.GetAllForLogin().ToList();

            balanceSheetModel = balanceSheetModel.OrderByDescending(x => x.TransactionDate).ToList();

            balanceSheetModel.ForEach(x => x.UserName = GetUserNameOrNull(allStaff, x.Cashier));

            var totalDiscount = accountsPaidin.Sum(x => x.DiscountAmount);

            model.TotalCashDiscount = totalDiscount;

            var tPaidIn = balanceSheetModel.Sum(x => x.AmountPaidIn);

            var overallTotal = tPaidIn;

            model.Tax = accountsPaidin.Sum(x => x.TaxAmount);

            tPaidIn = tPaidIn - totalDiscount;

            var tPaidOut = balanceSheetModel.Sum(x => x.AmountPaidOut);

            model.FullBalance = tPaidIn - tPaidOut;

            model.BalanceSheet = balanceSheetModel;

            model.ConciseBalanceSheetSheet = balanceSheetModel.GroupBy(x => x.TransactionDate).Select(x => new ConciseBalanceSheetModel { ActualDate = x.Key, TotalReceiveable = x.Sum(y => y.AmountPaidIn), TotalPayaeble = x.Sum(z => z.AmountPaidOut) }).ToList();

            model.FileToDownloadPath = GenerateExcelSheetPANDL(model, "PandL");

            return View(model);
        }

        private string GenerateExcelSheetPANDL(ReportViewModel model, string reportName)
        {
            DataTable dt = new DataTable();

            dt.Columns.AddRange(new DataColumn[3] {
                                new DataColumn("Date", typeof(string)),
                                new DataColumn("Amount Paid In (Sales Profit)", typeof(string)),
                                new DataColumn("Amount Paid Out (Expenses)",typeof(string))
            });

            int p = 1;
          
            foreach (var ru in model.ConciseBalanceSheetSheet.OrderBy(x => x.ActualDate))
            {
                dt.Rows.Add(ru.ActualDate, ru.TotalReceiveable, ru.TotalPayaeble);
                p++;
            }

            dt.Rows.Add("Total", model.ConciseBalanceSheetSheet.Sum(x => x.TotalReceiveable), model.ConciseBalanceSheetSheet.Sum(x => x.TotalPayaeble));
            p++;

            dt.Rows.Add("Total Discount", model.TotalCashDiscount, "");
            p++;

            dt.Rows.Add("Balance", model.FullBalance, "");
            p++;

            dt.Rows.Add("Tax", model.Tax, "");
            p++;

            dt.Rows.Add("Balance With Tax", (model.FullBalance + model.Tax), "");
            p++;


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

        private decimal GetProfit(HotelMateWeb.Dal.DataCore.Payment x)
        {
           var items = _soldItemService.GetAll().Where(y => y.RecieptNumber == x.ReceiptNumber).ToList();

            var totalBoughtPriceBoughtFor = decimal.Zero;
            var totalBoughtPricePaidFor = decimal.Zero;


            foreach (var item in items)
            {
                totalBoughtPriceBoughtFor += item.StockItem.OrigPrice * item.Qty;
                totalBoughtPricePaidFor += item.StockItem.UnitPrice.Value * item.Qty;
            }

            return totalBoughtPricePaidFor - totalBoughtPriceBoughtFor;
        }

        private decimal GetProfit(HotelMateWeb.Dal.DataCore.SoldItemsAll x)
        {
            if(x.StockItem != null)
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

            var balanceSheetModel = accountsPaidin.Select(x => new BalanceSheetModel { TransactionDate = x.DateSold.Value, AmountPaidIn = x.TotalPrice, AmountPaidOut = decimal.Zero, 
                Cashier = x.TransactionId, PaymentMentMethod = x.PaymentMethod, PaymentTypeId = x.PaymentTypeId }).ToList();
           
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
            if(!id.HasValue)
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
            if(staff != null)
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

            var neWCombinedSalesModel = combinedSales.Select(x => new CombinedSalesModel {DateSold = x.TransactionDate, GuestRoom = x.GuestRoom, Amount = x.Amount, PaymentMethod = x.PaymentMethod.Name, 
                PaymentMethodNote = x.PaymentMethodNote, StaffName = x.Person.DisplayName, Terminal = x.RoomPaymentType.Name
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

            var realSales = simLst.GroupBy(x => x.TimeOfSale).Select(x => new SoldItemModel { TimeOfSale = x.Key, TotalPrice = x.Sum(y => y.TotalPrice),
                PaymentTypeName = x.FirstOrDefault().PaymentTypeName, PaymentMethodName = x.FirstOrDefault().PaymentMethodName,
                PersonName = x.FirstOrDefault().PersonName, DateSold = x.FirstOrDefault().DateSold, PaymentMethodNote = x.FirstOrDefault().PaymentMethodNote, 
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

            foreach(var gr in groupByGuestRoom)
            {
                var guestRoom = gr.GuestRoom;                

                var gbl = gr.ItemList.GroupBy(x => x.TransactionDate.ToShortDateString()).Select(x => new PersonDateModel { DateSoldString = x.Key, ItemLst = x.ToList() }).ToList();

                foreach(var modella in gbl)
                {
                    var groupByPerson = modella.ItemLst.GroupBy(x => x.Person).Select(x => new SalesPersonModel { Person = x.Key, ItemLst = x.ToList() }).ToList();

                    foreach(var p in groupByPerson)
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

                var allPaymentsMade = _businessCorporateAccountService.GetAll(HotelID).Where(x => x.BusinessAccountId == existingCompany.Id).ToList();

                lst.Add(new CompanyPaymentModel { Name = existingCompany.Name, Bill = allItemisedItems.Sum(x => x.TotalPrice), Payments = allPaymentsMade.Sum(x => x.Amount), Balance = allPaymentsMade.Sum(x => x.Amount) - allItemisedItems.Sum(x => x.TotalPrice) });

            }

            /* var existingCompany = _businessAccountService.GetById(id.Value);
                var allItemisedItems = existingCompany.SoldItemsAlls.Where(y => y.IsActive).ToList();
                var allPaymentsMade = _businessAccountCorporateService.GetAll(HotelID).Where(x => x.BusinessAccountId == id.Value).ToList();*/

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

            var allDiscounts = _paymentService.GetAllInclude("").Where(x => (x.DiscountAmount > 0 || x.PaymentTypeId == (int)PaymentMethodEnum.COMPLIMENTARY) && x.PaymentDate >= startDate && x.PaymentDate <= endDate).ToList();
            

            model.AllNewDiscounts = allDiscounts;

            model.ReportName = "Discounts";

            model.FileToDownloadPath = GenerateExcelSheetDiscounts(model, model.ReportName);

            return View(model);

        }

        private string GenerateExcelSheetDiscounts(ReportViewModel model, string reportName)
        {

            DataTable dt = new DataTable();

            dt.Columns.AddRange(new DataColumn[5] {
                                new DataColumn("Date", typeof(string)),
                                new DataColumn("Receipt Number", typeof(string)),
                                new DataColumn("Cashier", typeof(string)),
                                new DataColumn("Status",typeof(string)),
                                new DataColumn("Amount (NGN)",typeof(string))
            });

            int p = 1;

            foreach (var ru in model.AllNewDiscounts.OrderByDescending(x => x.PaymentDate))
            {
                dt.Rows.Add(ru.PaymentDate, ru.ReceiptNumber, ru.Person.DisplayName, ru.PaymentMethod.Name, ru.PaymentMethodId == (int)PaymentMethodEnum.COMPLIMENTARY ? ru.Total : ru.DiscountAmount);
                p++;
            }

            dt.Rows.Add("Total", "", "", "", model.AllNewDiscounts.Sum(x => x.PaymentMethodId == (int)PaymentMethodEnum.COMPLIMENTARY ? x.Total : x.DiscountAmount));

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

        public ActionResult IncomeStatementComplimentary(DateTime? startDate, DateTime? endDate, int? id)
        {
            ReportViewModel model = new ReportViewModel();

            var dateRecieved = false;

            if (startDate.HasValue && endDate.HasValue)
            {
                dateRecieved = true;

                if (startDate.Value == endDate.Value)
                {
                    var newDate = startDate.Value.AddDays(1);
                    endDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 0, 0, 1);
                }
                else
                {
                    startDate = new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day, 7, 0, 1);
                    var newDate = new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day, 7, 0, 1);
                    endDate = newDate;
                }

                if (IsNightClub())
                {
                    startDate = new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day, 7, 0, 1);
                    //var newDate = startDate.Value.AddDays(1);
                    var newDate = new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day, 7, 0, 1);
                    endDate = newDate;
                }
            }

            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Now.AddMonths(1);

            var conn = ConfigurationManager.ConnectionStrings[1].ConnectionString;

            var ds = _paymentService.GetAll();

            var salesModel = ConvertToList(ds.ToList());

            var allDistributionPoints = _distributionPointService.GetAll().ToList();

            allDistributionPoints.Insert(0, new DistributionPoint { Id = 0, Name = "--Pls Select--" });

            IEnumerable<SelectListItem> selectList =
                from c in allDistributionPoints
                select new SelectListItem
                {
                    Selected = (c.Id == id),
                    Text = c.Name,
                    Value = c.Id.ToString()
                };

            model.selectList = selectList;

            if (id.HasValue && id.Value > 0)
            {
                model.SalesModel = salesModel.Where(x => x.DateSold >= startDate && x.DateSold <= endDate && x.DistributionPointId == id.Value && x.PaymentMethodId == (int)PaymentMethodEnum.COMPLIMENTARY).OrderByDescending(x => x.DateSold).ToList();
            }
            else
            {
                model.SalesModel = salesModel.Where(x => x.DateSold >= startDate && x.DateSold <= endDate && x.PaymentMethodId == (int)PaymentMethodEnum.COMPLIMENTARY).OrderByDescending(x => x.DateSold).ToList();
            }

            model.ModelGroupBy = model.SalesModel.GroupBy(x => x.PaymentMethodName).
              Select(x => new SoldItemModel
              {
                  PaymentMethodName = x.Key,
                  SubTotal = x.Sum(y => y.SubTotal),
                  Total = x.Sum(y => y.Total),
                  TotalTax = x.Sum(y => y.TotalTax),
                  TotalDiscount = x.Sum(y => y.TotalDiscount),
                  TotalServiceCharge = x.Sum(y => y.TotalServiceCharge),
                  TotalResident = x.Sum(y => y.TotalResident),
                  ItemNewlst = x.ToList()
              }).ToList();

            model.ReportName = "IncomeStatementComplimentary";

            model.FileToDownloadPath = GenerateExcelSheet(model.ModelGroupBy, model.ReportName);

            model.Id = id;

            return View(model);
        }

        
        public ActionResult IncomeStatementCheque(DateTime? startDate, DateTime? endDate, int? id)
        {
            ReportViewModel model = new ReportViewModel();

            var dateRecieved = false;

            if (startDate.HasValue && endDate.HasValue)
            {
                dateRecieved = true;

                if (startDate.Value == endDate.Value)
                {
                    var newDate = startDate.Value.AddDays(1);
                    endDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 0, 0, 1);
                }
                else
                {
                    startDate = new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day, 7, 0, 1);
                    var newDate = new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day, 7, 0, 1);
                    endDate = newDate;
                }

                if (IsNightClub())
                {
                    startDate = new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day, 7, 0, 1);
                    //var newDate = startDate.Value.AddDays(1);
                    var newDate = new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day, 7, 0, 1);
                    endDate = newDate;
                }
            }

            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Now.AddMonths(1);

            var conn = ConfigurationManager.ConnectionStrings[1].ConnectionString;

            var ds = _paymentService.GetAll();

            var salesModel = ConvertToList(ds.ToList());

            var allDistributionPoints = _distributionPointService.GetAll().ToList();

            allDistributionPoints.Insert(0, new DistributionPoint { Id = 0, Name = "--Pls Select--" });

            IEnumerable<SelectListItem> selectList =
                from c in allDistributionPoints
                select new SelectListItem
                {
                    Selected = (c.Id == id),
                    Text = c.Name,
                    Value = c.Id.ToString()
                };

            model.selectList = selectList;

            if (id.HasValue && id.Value > 0)
            {
                model.SalesModel = salesModel.Where(x => x.DateSold >= startDate && x.DateSold <= endDate && x.DistributionPointId == id.Value && x.PaymentMethodId == (int)PaymentMethodEnum.Cheque).OrderByDescending(x => x.DateSold).ToList();
            }
            else
            {
                model.SalesModel = salesModel.Where(x => x.DateSold >= startDate && x.DateSold <= endDate && x.PaymentMethodId == (int)PaymentMethodEnum.Cheque).OrderByDescending(x => x.DateSold).ToList();
            }

            model.ModelGroupBy = model.SalesModel.GroupBy(x => x.PaymentMethodName).
               Select(x => new SoldItemModel
               {
                   PaymentMethodName = x.Key,
                   SubTotal = x.Sum(y => y.SubTotal),
                   Total = x.Sum(y => y.Total),
                   TotalTax = x.Sum(y => y.TotalTax),
                   TotalDiscount = x.Sum(y => y.TotalDiscount),
                   TotalServiceCharge = x.Sum(y => y.TotalServiceCharge),
                   TotalResident = x.Sum(y => y.TotalResident),
                   ItemNewlst = x.ToList()
               }).ToList();

            model.ReportName = "IncomeStatementCheque";

            model.FileToDownloadPath = GenerateExcelSheet(model.ModelGroupBy, model.ReportName);

            model.Id = id;

            return View(model);
        }

        public ActionResult IncomeStatementCreditCard(DateTime? startDate, DateTime? endDate, int? id)
        {
            ReportViewModel model = new ReportViewModel();

            var dateRecieved = false;

            if (startDate.HasValue && endDate.HasValue)
            {
                dateRecieved = true;

                if (startDate.Value == endDate.Value)
                {
                    var newDate = startDate.Value.AddDays(1);
                    endDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 0, 0, 1);
                }
                else
                {
                    startDate = new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day, 7, 0, 1);
                    var newDate = new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day, 7, 0, 1);
                    endDate = newDate;
                }

                if (IsNightClub())
                {
                    startDate = new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day, 7, 0, 1);
                    //var newDate = startDate.Value.AddDays(1);
                    var newDate = new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day, 7, 0, 1);
                    endDate = newDate;
                }
            }

            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Now.AddMonths(1);

            var conn = ConfigurationManager.ConnectionStrings[1].ConnectionString;

            var ds = _paymentService.GetAll();

            var salesModel = ConvertToList(ds.ToList());

            var allDistributionPoints = _distributionPointService.GetAll().ToList();

            allDistributionPoints.Insert(0, new DistributionPoint { Id = 0, Name = "--Pls Select--" });

            IEnumerable<SelectListItem> selectList =
                from c in allDistributionPoints
                select new SelectListItem
                {
                    Selected = (c.Id == id),
                    Text = c.Name,
                    Value = c.Id.ToString()
                };

            model.selectList = selectList;

            if (id.HasValue && id.Value > 0)
            {
                model.SalesModel = salesModel.Where(x => x.DateSold >= startDate && x.DateSold <= endDate && x.DistributionPointId == id.Value && x.PaymentMethodId == (int)PaymentMethodEnum.CreditCard).OrderByDescending(x => x.DateSold).ToList();
            }
            else
            {
                model.SalesModel = salesModel.Where(x => x.DateSold >= startDate && x.DateSold <= endDate && x.PaymentMethodId == (int)PaymentMethodEnum.CreditCard).OrderByDescending(x => x.DateSold).ToList();
            }

            model.ModelGroupBy = model.SalesModel.GroupBy(x => x.PaymentMethodName).
               Select(x => new SoldItemModel
               {
                   PaymentMethodName = x.Key,
                   SubTotal = x.Sum(y => y.SubTotal),
                   Total = x.Sum(y => y.Total),
                   TotalTax = x.Sum(y => y.TotalTax),
                   TotalDiscount = x.Sum(y => y.TotalDiscount),
                   TotalServiceCharge = x.Sum(y => y.TotalServiceCharge),
                   TotalResident = x.Sum(y => y.TotalResident),
                   ItemNewlst = x.ToList()
               }).ToList();

            model.ReportName = "IncomeStatementCreditCard";

            model.FileToDownloadPath = GenerateExcelSheet(model.ModelGroupBy, model.ReportName);


            model.Id = id;

            return View(model);
        }


        public ActionResult IncomeStatementCredit(DateTime? startDate, DateTime? endDate, int? id)
        {
            ReportViewModel model = new ReportViewModel();

            var dateRecieved = false;

            if (startDate.HasValue && endDate.HasValue)
            {
                dateRecieved = true;

                if (startDate.Value == endDate.Value)
                {
                    var newDate = startDate.Value.AddDays(1);
                    endDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 0, 0, 1);
                }
                else
                {
                    startDate = new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day, 7, 0, 1);
                    var newDate = new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day, 7, 0, 1);
                    endDate = newDate;
                }

                if (IsNightClub())
                {
                    startDate = new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day, 7, 0, 1);
                    //var newDate = startDate.Value.AddDays(1);
                    var newDate = new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day, 7, 0, 1);
                    endDate = newDate;
                }
            }

            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Now.AddMonths(1);

            var conn = ConfigurationManager.ConnectionStrings[1].ConnectionString;

            var ds = _paymentService.GetAll();

            var salesModel = ConvertToList(ds.ToList());

            var allDistributionPoints = _distributionPointService.GetAll().ToList();

            allDistributionPoints.Insert(0, new DistributionPoint { Id = 0, Name = "--Pls Select--" });

            IEnumerable<SelectListItem> selectList =
                from c in allDistributionPoints
                select new SelectListItem
                {
                    Selected = (c.Id == id),
                    Text = c.Name,
                    Value = c.Id.ToString()
                };

            model.selectList = selectList;

            if (id.HasValue && id.Value > 0)
            {
                model.SalesModel = salesModel.Where(x => x.DateSold >= startDate && x.DateSold <= endDate && x.DistributionPointId == id.Value && x.PaymentMethodId == (int)PaymentMethodEnum.POSTBILL).OrderByDescending(x => x.DateSold).ToList();
            }
            else
            {
                model.SalesModel = salesModel.Where(x => x.DateSold >= startDate && x.DateSold <= endDate && x.PaymentMethodId == (int)PaymentMethodEnum.POSTBILL).OrderByDescending(x => x.DateSold).ToList();
            }

            model.ModelGroupBy = model.SalesModel.GroupBy(x => x.PaymentMethodName).
               Select(x => new SoldItemModel
               {
                   PaymentMethodName = x.Key,
                   SubTotal = x.Sum(y => y.SubTotal),
                   Total = x.Sum(y => y.Total),
                   TotalTax = x.Sum(y => y.TotalTax),
                   TotalDiscount = x.Sum(y => y.TotalDiscount),
                   TotalServiceCharge = x.Sum(y => y.TotalServiceCharge),
                   TotalResident = x.Sum(y => y.TotalResident),
                   ItemNewlst = x.ToList()
               }).ToList();

            model.ReportName = "IncomeStatementCredit";

            model.FileToDownloadPath = GenerateExcelSheet(model.ModelGroupBy, model.ReportName);


            model.Id = id;

            return View(model);
        }

        public ActionResult ManagerialDelete(DateTime? startDate, DateTime? endDate, int? id, string clickedButton)
        {
            ReportViewModel model = new ReportViewModel();

            var dateRecieved = false;

            if (startDate.HasValue && endDate.HasValue)
            {
                dateRecieved = true;

                if (startDate.Value == endDate.Value)
                {
                    var newDate = startDate.Value.AddDays(1);
                    endDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 0, 0, 1);
                }
                else
                {
                    startDate = new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day, 7, 0, 1);
                    var newDate = new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day, 7, 0, 1);
                    endDate = newDate;
                }

                if (IsNightClub())
                {
                    startDate = new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day, 7, 0, 1);
                    //var newDate = startDate.Value.AddDays(1);
                    var newDate = new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day, 7, 0, 1);
                    endDate = newDate;
                }
            }

            if (!startDate.HasValue && !endDate.HasValue)
            {
                var yesterday = DateTime.Now.AddDays(-1);
                startDate = new DateTime(yesterday.Year, yesterday.Month, yesterday.Day, 7, 0, 1);
                var newDate = startDate.Value.AddDays(1);
                endDate = newDate;
            }

            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Now.AddMonths(1);

            var allDistributionPoints = _distributionPointService.GetAll().ToList();

            allDistributionPoints.Insert(0, new DistributionPoint { Id = 0, Name = "--Pls Select--" });

            IEnumerable<SelectListItem> selectList =
                from c in allDistributionPoints
                select new SelectListItem
                {
                    Selected = (c.Id == id),
                    Text = c.Name,
                    Value = c.Id.ToString()
                };

            model.selectList = selectList;

            var conn = ConfigurationManager.ConnectionStrings[1].ConnectionString;

            var ds = POSService.StockItemService.GetManagerialDeleteItems(conn);

            var deleteModel = ConvertToListDeletedItems(ds);

            if (id.HasValue && id.Value > 0)
            {
                model.SalesModel = deleteModel.Where(x => x.DateSold >= startDate && x.DateSold <= endDate).OrderByDescending(x => x.DateSold).ToList();
            }
            else
            {
                model.SalesModel = deleteModel.Where(x => x.DateSold >= startDate && x.DateSold <= endDate).OrderByDescending(x => x.DateSold).ToList();
            }

            //if (!startDate.HasValue && !endDate.HasValue)
            //{
            //    model.SalesModel = deleteModel.Where(x => x.DateSold.ToShortDateString().Equals(DateTime.Today.ToShortDateString(), StringComparison.InvariantCultureIgnoreCase)).OrderByDescending(x => x.DateSold).ToList();
            //}

            model.ReportName = "ManagerialDelete";
           
            return View(model);
        }



        public ActionResult IncomeStatementTotalPrint(DateTime? startDate, DateTime? endDate, int? id, string clickedButton)
        {
            ReportViewModel model = new ReportViewModel();

            var dateRecieved = false;

            if (startDate.HasValue && endDate.HasValue)
            {
                dateRecieved = true;

                if (startDate.Value == endDate.Value)
                {
                    var newDate = startDate.Value.AddDays(1);
                    endDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 0, 0, 1);
                }
                else
                {
                    startDate = new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day, 7, 0, 1);
                    var newDate = new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day, 7, 0, 1);
                    endDate = newDate;
                }

                if (IsNightClub())
                {
                    startDate = new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day, 7, 0, 1);
                    //var newDate = startDate.Value.AddDays(1);
                    var newDate = new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day, 7, 0, 1);
                    endDate = newDate;
                }
            }

            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Now.AddMonths(1);

            var allDistributionPoints = _distributionPointService.GetAll().ToList();

            allDistributionPoints.Insert(0, new DistributionPoint { Id = 0, Name = "--Pls Select--" });

            IEnumerable<SelectListItem> selectList =
                from c in allDistributionPoints
                select new SelectListItem
                {
                    Selected = (c.Id == id),
                    Text = c.Name,
                    Value = c.Id.ToString()
                };

            model.selectList = selectList;

            var conn = ConfigurationManager.ConnectionStrings[1].ConnectionString;

            var ds = POSService.StockItemService.GetSoldItemsPrint(conn);

            var salesModel = ConvertToList(ds);

            if (id.HasValue && id.Value > 0)
            {
                model.SalesModel = salesModel.Where(x => x.DateSold >= startDate && x.DateSold <= endDate && x.DistributionPointId == id.Value).OrderByDescending(x => x.DateSold).ToList();
            }
            else
            {
                model.SalesModel = salesModel.Where(x => x.DateSold >= startDate && x.DateSold <= endDate).OrderByDescending(x => x.DateSold).ToList();
            }

            var allReceipts = model.SalesModel.Select(x => x.RecieptNumber).ToList();

            var payments = _paymentService.GetAll().Where(x => allReceipts.Contains(x.ReceiptNumber)).ToList();

            model.ModelGroupBy = model.SalesModel.GroupBy(x => x.PaymentMethodName).Select(x => new SoldItemModel { PaymentTypeName = x.Key, TotalPrice = x.Sum(y => y.TotalPrice), ItemNewlst = x.ToList() }).ToList();

            model.Id = id;

            model.Payments = payments;

            model.ReportName = "IncomeStatementTotalPrint";

            model.Id = id;

            PrintPOSSales(model);

            return View("IncomeStatementTotalPrint", model);
        }

        
        public ActionResult SendEmailSalesReport(DateTime? startDate, DateTime? endDate, int? id)
        {
            ReportViewModel model = new ReportViewModel();

            var dateRecieved = false;

            if (startDate.HasValue && endDate.HasValue)
            {
                dateRecieved = true;

                if (startDate.Value == endDate.Value)
                {
                    var newDate = startDate.Value.AddDays(1);
                    endDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 0, 0, 1);
                }
                else
                {
                    startDate = new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day, 7, 0, 1);
                    var newDate = new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day, 7, 0, 1);
                    endDate = newDate;
                }

                if (IsNightClub())
                {
                    startDate = new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day, 7, 0, 1);
                    //var newDate = startDate.Value.AddDays(1);
                    var newDate = new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day, 7, 0, 1);
                    endDate = newDate;
                }
            }

            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Now.AddMonths(1);

            var conn = ConfigurationManager.ConnectionStrings[1].ConnectionString;

            var ds = _paymentService.GetAll();

            var salesModel = ConvertToList(ds.ToList());

            var allDistributionPoints = _distributionPointService.GetAll().ToList();

            allDistributionPoints.Insert(0, new DistributionPoint { Id = 0, Name = "--Pls Select--" });

            IEnumerable<SelectListItem> selectList =
                from c in allDistributionPoints
                select new SelectListItem
                {
                    Selected = (c.Id == id),
                    Text = c.Name,
                    Value = c.Id.ToString()
                };

            model.selectList = selectList;

            if (id.HasValue && id.Value > 0)
            {
                model.SalesModel = salesModel.Where(x => x.DateSold >= startDate && x.DateSold <= endDate && x.DistributionPointId == id.Value).OrderByDescending(x => x.DateSold).ToList();
            }
            else
            {
                model.SalesModel = salesModel.Where(x => x.DateSold >= startDate && x.DateSold <= endDate).OrderByDescending(x => x.DateSold).ToList();
            }

            model.ModelGroupBy = model.SalesModel.GroupBy(x => x.PaymentMethodName).
              Select(x => new SoldItemModel
              {
                  PaymentMethodName = x.Key,
                  SubTotal = x.Sum(y => y.SubTotal),
                  Total = x.Sum(y => y.Total),
                  TotalTax = x.Sum(y => y.TotalTax),
                  TotalDiscount = x.Sum(y => y.TotalDiscount),
                  TotalServiceCharge = x.Sum(y => y.TotalServiceCharge),
                  TotalResident = x.Sum(y => y.TotalResident),
                  ItemNewlst = x.ToList()
              }).ToList();

            model.ReportName = "SendEmailSalesReport";

            model.FileToDownloadPath = GenerateExcelSheet(model.ModelGroupBy, model.ReportName);

            model.Id = id;

            return View(model);
        }

        public ActionResult IncomeStatementTotal(DateTime? startDate, DateTime? endDate, int? id, string clickedButton)
        {
            if (!string.IsNullOrEmpty(clickedButton) && clickedButton == "POSPrint")
            {
                return RedirectToAction("IncomeStatementTotalPrint", new { startDate, endDate });
            }

            ReportViewModel model = new ReportViewModel();

            var dateRecieved = false;

            if (startDate.HasValue && endDate.HasValue)
            {
                dateRecieved = true;

                if (startDate.Value == endDate.Value)
                {
                    var newDate = startDate.Value.AddDays(1);
                    endDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 0, 0, 1);
                }
                else
                {
                    startDate = new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day, 7, 0, 1);
                    var newDate = new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day, 7, 0, 1);
                    endDate = newDate;
                }

                if (IsNightClub())
                {
                    startDate = new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day, 7, 0, 1);
                    //var newDate = startDate.Value.AddDays(1);
                    var newDate = new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day, 7, 0, 1);
                    endDate = newDate;
                }
            }

            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Now.AddMonths(1);

            var conn = ConfigurationManager.ConnectionStrings[1].ConnectionString;

            var ds = _paymentService.GetAll();

            var salesModel = ConvertToList(ds.ToList());

            var allDistributionPoints = _distributionPointService.GetAll().ToList();

            allDistributionPoints.Insert(0, new DistributionPoint { Id = 0, Name = "--Pls Select--" });

            IEnumerable<SelectListItem> selectList =
                from c in allDistributionPoints
                select new SelectListItem
                {
                    Selected = (c.Id == id),
                    Text = c.Name,
                    Value = c.Id.ToString()
                };

            model.selectList = selectList;

            if (id.HasValue && id.Value > 0)
            {
                model.SalesModel = salesModel.Where(x => x.DateSold >= startDate && x.DateSold <= endDate && x.DistributionPointId == id.Value).OrderByDescending(x => x.DateSold).ToList();
            }
            else
            {
                model.SalesModel = salesModel.Where(x => x.DateSold >= startDate && x.DateSold <= endDate).OrderByDescending(x => x.DateSold).ToList();
            }

            model.ModelGroupBy = model.SalesModel.GroupBy(x => x.PaymentMethodName).
              Select(x => new SoldItemModel
              {
                  PaymentMethodName = x.Key,
                  SubTotal = x.Sum(y => y.SubTotal),
                  Total = x.Sum(y => y.Total),
                  TotalTax = x.Sum(y => y.TotalTax),
                  TotalDiscount = x.Sum(y => y.TotalDiscount),
                  TotalServiceCharge = x.Sum(y => y.TotalServiceCharge),
                  TotalResident = x.Sum(y => y.TotalResident),
                  ItemNewlst = x.ToList()
              }).ToList();

            model.ReportName = "IncomeStatementTotal";

            model.FileToDownloadPath = GenerateExcelSheet(model.ModelGroupBy, model.ReportName);

            model.Id = id;

            return View(model);
        }

        public ActionResult CloseTill(int? id)
        {

            var thisUser = _personService.GetAllForLogin().Where(x => x.Username.ToUpper().Equals(User.Identity.Name.ToUpper())).FirstOrDefault();

            var cashierId = thisUser.PersonID;

            var unclearedItems = _tableItemService.GetAll().Where(x => x.Cashier == cashierId).Count();

            if (unclearedItems > 0)
            {
                return Content(@"<script language='javascript' type='text/javascript'>
                alert('You cannot close your till as you have pending items, please transfer your till first!');
                location.href = '/POS/Index';
                </script>");
            }

            var conn = ConfigurationManager.ConnectionStrings[1].ConnectionString;

            var receiptNumber = DateTime.Now.ToString().GetHashCode().ToString("x");

            try
            {
                var ds = POSService.StockItemService.GetSoldItemsForCashierOpenTill(cashierId, conn);

                var salesModel = ConvertToList(ds).ToList();

                salesModel = salesModel.Where(x => x.TillOpen).OrderByDescending(x => x.DateSold).ToList();

                ReportViewModel model = new ReportViewModel();

                if (id.HasValue && id.Value > 0)
                {
                    model.SalesModel = salesModel.OrderByDescending(x => x.DateSold).ToList();
                }
                else
                {
                    model.SalesModel = salesModel.OrderByDescending(x => x.DateSold).ToList();
                }

                var allReceipts = model.SalesModel.Select(x => x.RecieptNumber).ToList();

                var payments = _paymentService.GetAll().Where(x => allReceipts.Contains(x.ReceiptNumber)).ToList();

                model.ModelGroupBy = model.SalesModel.GroupBy(x => x.PaymentMethodName).Select(x => new SoldItemModel { PaymentTypeName = x.Key, TotalPrice = x.Sum(y => y.TotalPrice), ItemNewlst = x.ToList() }).ToList();
                
                model.ReportName = "Close Till";

                model.Id = id;

                model.Payments = payments;

                PrintPOSSales(model);

                POSService.StockItemService.CloseTill(cashierId, conn);
            }
            catch(Exception)
            {
            }

           
            return RedirectToAction("Index","Pos");
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

        private void PrintPOSSales(ReportViewModel model)
        {
            foreach (var item in model.ModelGroupBy.ToList())
            {
                PrinterHeaderLine(item.PaymentTypeName);

                PrinterHeaderLine("==================");

                foreach (var lst in item.ItemNewlst)
                {
                    PrinterHeaderLine(lst.Description, lst.Quantity, (double)lst.TotalPrice);
                }

                PrinterHeaderLine("Total ",(double)model.Payments.Where(x => x.PaymentMethod.Name == item.PaymentTypeName).Sum(x => x.Total));
            }

            var salestotal = "Sales Total ";

            //PrinterHeaderLine(salestotal, (double)model.ModelGroupBy.Where(x => x.PaymentMethodId != (int)PaymentMethodEnum.COMPLIMENTARY).Sum(x => x.TotalPrice));
            PrinterHeaderLine(salestotal, (double)model.Payments.Sum(x => x.SubTotal));

            var discounttotal = "Discount Total ";
            PrinterHeaderLine(discounttotal, (double)model.Payments.Sum(x => x.DiscountAmount));

            var totalLessDiscount = "Total Less Discount ";
            PrinterHeaderLine(totalLessDiscount, (double)(model.Payments.Sum(x => x.Total) - (model.Payments.Sum(x => x.DiscountAmount))));

            var totalTax = "Total Tax ";
            PrinterHeaderLine(totalTax, (double)model.Payments.Sum(x => x.TaxAmount));

            var totalSC = "Total Service Charge ";
            PrinterHeaderLine(totalSC, (double)model.Payments.Sum(x => x.ServiceChargeAmount));

            var totalRC = "Total Resident Charge ";
            PrinterHeaderLine(totalRC, (double)model.Payments.Sum(x => x.ResidentAmount));

            var totalEvery = "Total Cash";
            PrinterHeaderLine(totalEvery, (double)model.Payments.Where(x => x.PaymentMethodId == 1).Sum(x => x.Total));


            byte[] DrawerOpen5 = { 0xA };

            var printer = ConfigurationManager.AppSettings["NetworkPrinterName"].ToString();

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

        private string TruncateAt(string text, int maxWidth)
        {
            string retVal = text;
            if (text.Length > maxWidth)
                retVal = text.Substring(0, maxWidth);

            return retVal;
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

        private void PrintLineItemRaw(string printer, string itemCode, int quantity, double unitPrice)
        {
            PrintTextRaw(printer, TruncateAt(itemCode.PadRight(11), 11));
            PrintTextRaw(printer, TruncateAt(quantity.ToString("#0.00").PadLeft(9), 9));
            PrintTextRaw(printer, TruncateAt(unitPrice.ToString("#0.00").PadLeft(10), 10));
            PrintTextLineRaw(printer, TruncateAt((quantity * unitPrice).ToString("#0.00").PadLeft(10), 10));
        }

        private void PrintLineItemRaw(string printer, string itemCode)
        {
            PrintTextRaw(printer, TruncateAt(itemCode.PadRight(31), 31));
        }

        private void PrintLineItemRaw(string printer, string itemCode,double unitPrice)
        {
            PrintTextRaw(printer, TruncateAt(itemCode.PadRight(21), 21));
            PrintTextRaw(printer, TruncateAt(unitPrice.ToString("#0.00").PadLeft(10), 10));
            PrintTextLineRaw(printer, TruncateAt((unitPrice).ToString("#0.00").PadLeft(10), 10));
        }

        private void PrinterHeaderLine(string code,int quantity, double unitPrice)
        {
            var printer = ConfigurationManager.AppSettings["NetworkPrinterName"].ToString();
            PrintLineItemRaw(printer,code,quantity,unitPrice);
        }

        private void PrinterHeaderLine(string code,double unitPrice)
        {
            var printer = ConfigurationManager.AppSettings["NetworkPrinterName"].ToString();
            PrintLineItemRaw(printer, code, unitPrice);
        }

        private void PrinterHeaderLine(string code)
        {
            var printer = ConfigurationManager.AppSettings["NetworkPrinterName"].ToString();
            PrintLineItemRaw(printer, code);
        }

        private bool IsNightClub()
        {
            bool isNC = false;

            try
            {
                var yOrn = ConfigurationManager.AppSettings["IsNightClub"].ToString().ToUpper();

                if (yOrn == "1")
                    isNC = true;
            }
            catch (Exception)
            {
                isNC = false;
            }

            return isNC;
        }

        public ActionResult IncomeStatement(DateTime? startDate, DateTime? endDate, int? id)
        {
            ReportViewModel model = new ReportViewModel();

            var dateRecieved = false;

            if (startDate.HasValue && endDate.HasValue)
            {
                dateRecieved = true;

                if (startDate.Value == endDate.Value)
                {
                    var newDate = startDate.Value.AddDays(1);
                    endDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 0, 0, 1);
                }
                else
                {
                    startDate = new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day, 7, 0, 1);
                    var newDate = new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day, 7, 0, 1);
                    endDate = newDate;
                }

                if (IsNightClub())
                {
                    startDate = new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day, 7, 0, 1);
                    //var newDate = startDate.Value.AddDays(1);
                    var newDate = new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day, 7, 0, 1);
                    endDate = newDate;
                }
            }

            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Now.AddMonths(1);

            var conn = ConfigurationManager.ConnectionStrings[1].ConnectionString;

            var ds = _paymentService.GetAll();

            var salesModel = ConvertToList(ds.ToList());

            var allDistributionPoints = _distributionPointService.GetAll().ToList();

            allDistributionPoints.Insert(0, new DistributionPoint { Id = 0, Name = "--Pls Select--" });

            IEnumerable<SelectListItem> selectList =
                from c in allDistributionPoints
                select new SelectListItem
                {
                    Selected = (c.Id == id),
                    Text = c.Name,
                    Value = c.Id.ToString()
                };

            model.selectList = selectList;

            if (id.HasValue && id.Value > 0)
            {
                model.SalesModel = salesModel.Where(x => x.DateSold >= startDate && x.DateSold <= endDate && x.DistributionPointId == id.Value && x.PaymentMethodId == (int)PaymentMethodEnum.Cash).OrderByDescending(x => x.DateSold).ToList();
            }
            else
            {
                model.SalesModel = salesModel.Where(x => x.DateSold >= startDate && x.DateSold <= endDate && x.PaymentMethodId == (int)PaymentMethodEnum.Cash).OrderByDescending(x => x.DateSold).ToList();
            }

           
            model.ModelGroupBy = model.SalesModel.GroupBy(x => x.PaymentMethodName).
                Select(x => new SoldItemModel { PaymentMethodName = x.Key,
                    SubTotal = x.Sum(y => y.SubTotal),
                    Total = x.Sum(y => y.Total),
                    TotalTax = x.Sum(y => y.TotalTax),
                    TotalDiscount = x.Sum(y => y.TotalDiscount),
                    TotalServiceCharge = x.Sum(y => y.TotalServiceCharge),
                    TotalResident = x.Sum(y => y.TotalResident),
                    ItemNewlst = x.ToList() }).ToList();

            model.ReportName = "IncomeStatement";

            model.Id = id;

            model.FileToDownloadPath = GenerateExcelSheet(model.ModelGroupBy, model.ReportName);

            return View(model);
        }

        private string GenerateExcelSheetInventory(ReportViewModel model, string reportName)
        {
            DataTable dt = new DataTable();

            dt.Columns.AddRange(new DataColumn[6] {
                                new DataColumn("Item", typeof(string)),
                                new DataColumn("Received", typeof(string)),
                                new DataColumn("Qty Distributed",typeof(string)),
                                new DataColumn("Qty Returned",typeof(string)),
                                new DataColumn("Qty Damaged",typeof(string)),
                                new DataColumn("Qty Remaining",typeof(string))
            });

            int p = 1;

            foreach (var ru in model.CompleteInventoryList)
            {
                dt.Rows.Add(ru.StockItemNameName, ru.Acquired, ru.Distributed, ru.Returns, ru.Damaged, ru.Remaining);
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


        public string GenerateExcelSheet(List<SoldItemModel> lst, string reportName)
        {
            DataTable dt = new DataTable();

            dt.Columns.AddRange(new DataColumn[9] {
                                new DataColumn("Receipt No.", typeof(string)),
                                new DataColumn("Date", typeof(string)),
                                new DataColumn("Staff",typeof(string)),
                                new DataColumn("SubTotal",typeof(string)),
                                new DataColumn("Tax",typeof(string)),
                                new DataColumn("Discount",typeof(string)),
                                new DataColumn("S/Charge",typeof(string)),
                                new DataColumn("Resident",typeof(string)),
                                new DataColumn("Total",typeof(string))
            });

            int p = 1;

            foreach (var ru0 in lst)
            {
                dt.Rows.Add(ru0.PaymentMethodName, "", "", "", "", "", "", "", "");

                p++;

                foreach (var ru in ru0.ItemNewlst)
                {
                    dt.Rows.Add(ru.RecieptNumber, ru.DateSold.ToShortDateString(), ru.PersonName, ru.SubTotal, ru.TotalTax, ru.TotalDiscount, ru.TotalServiceCharge, ru.TotalResident, ru.Total);
                    p++;
                }
            }

            p++;

            dt.Rows.Add("", "", "", lst.Sum(x => x.SubTotal), lst.Sum(x => x.TotalTax), lst.Sum(x => x.TotalDiscount), lst.Sum(x => x.TotalServiceCharge), lst.Sum(x => x.TotalResident), lst.Sum(x => x.Total));


            var fileName = "_" + reportName;

            var fileNameToUse = fileName + ".xlsx";

            var path = Path.Combine(Server.MapPath("~/Products"), fileNameToUse);

            //Codes for the Closed XML
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt, reportName);
                wb.SaveAs(path);
                //string myName = Server.UrlEncode("Test" + "_" +
                //DateTime.Now.ToShortDateString() + ".xlsx");
                //MemoryStream stream = GetStream(wb);// The method is defined below
                //Response.Clear();
                //Response.Buffer = true;
                //Response.AddHeader("content-disposition",
                //"attachment; filename=" + myName);
                //Response.ContentType = "application/vnd.ms-excel";
                //Response.BinaryWrite(stream.ToArray());
                //Response.End();
            }

            return fileName;
        }

        public MemoryStream GetStream(XLWorkbook excelWorkbook)
        {
            MemoryStream fs = new MemoryStream();
            excelWorkbook.SaveAs(fs);
            fs.Position = 0;
            return fs;
        }

        

        [ValidateInput(false)]
        public FileResult DownloadStatementEmail(string id)
        {
            var path = Path.Combine(Server.MapPath("~/Products"), id + ".xlsx");
            var fileName = DateTime.Now.ToShortDateString() + "_" + "Excel.xlsx";

            var emailTemplate = @"<p style='margin:0px;padding:0px;font-size:12px;font-family:Arial, Helvetica, sans-serif;color:#555;' id='yui_3_16_0_ym19_1_1463261898755_4224'>Warm Greetings,<br>
                                <br>This is to kindly inform you of your daily sales records, the sales details are listed below : <br>
                                <br>Please see attached file for your sales statement<br><br>
                                </p>";


            try
            {

                var dest = GetOwnersEmail();

                var emails = dest.Split(',').ToList();

                foreach (var email in emails)
                {

                    MailMessage mail = new MailMessage("academyvistang@gmail.com", email, "Your daily sales report", emailTemplate);
                    mail.From = new MailAddress("academyvistang@gmail.com", "BarRestaurantMate");
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

            return File(path, "application/ms-excel", fileName);
        }

        [ValidateInput(false)]
        public FileResult DownloadStatement(string id)
        {
            var path = Path.Combine(Server.MapPath("~/Products"), id + ".xlsx");
            var fileName = DateTime.Now.ToShortDateString() + "_" + "Excel.xlsx";
            return File(path, "application/ms-excel", fileName);
        }


        public string ExportToExcel(List<SoldItemModel> lst, string reportName)
        {
            var gv = new GridView();

            var l = lst.ToList();

            lst.Add(new SoldItemModel { Total = lst.Sum(x => x.Total), SubTotal = lst.Sum(x => x.SubTotal), TotalDiscount = lst.Sum(x => x.TotalDiscount),
                TotalPrice = lst.Sum(x => x.TotalPrice), TotalTax = lst.Sum(x => x.TotalTax), TotalServiceCharge = lst.Sum(x => x.TotalServiceCharge),
                TotalResident = lst.Sum(x => x.TotalResident)
            });

            gv.DataSource = lst.ToList();

            gv.DataBind();

            //Response.ClearContent();
            //Response.Buffer = true;
            //Response.AddHeader("content-disposition", "attachment; filename=DemoExcel.xls");
            //Response.ContentType = "application/ms-excel";

            //Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);

            gv.RenderControl(objHtmlTextWriter);

            //Response.Output.Write(objStringWriter.ToString());
            //Response.Flush();
            //Response.End();

            var fileName = "_" + reportName;

            var fileNameToUse = fileName + ".xls";

            var path = Path.Combine(Server.MapPath("~/Products"), fileNameToUse);
            //file.SaveAs(path);

            StringWriter sw = objStringWriter;
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            //convert string from stringwriter to byte array and do the same as in the example above
            byte[] byteArray = encoding.GetBytes(sw.ToString());

            var f = File(byteArray, "application/ms-excel", "DemoExcel.xls");

            System.IO.File.WriteAllBytes(path, f.FileContents);


            //StringWriter sw = objStringWriter;
            //System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            // convert string from stringwriter to byte array and do the same as in the example above
            //byte[] byteArray = encoding.GetBytes(sw.ToString());
            //MemoryStream memStream = new MemoryStream();
            // memStream.Write(byteArray, 0, byteArray.GetLength(0));
            //memStream.Seek(0, SeekOrigin.Begin);

            //byte[] fileBytes = System.IO.File.ReadAllBytes(path);
            //string fileName = "myfile.ext";
            //return File(byteArray, "application/ms-excel", "DemoExcel.xls");

            //return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, po.InvoicePath);

            // return RedirectToAction("ImportVourcherCodes");

            return fileName;
        }

        

        private decimal GetRestaurantTax()
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

            if(hTax > 0)
                return Decimal.Divide((decimal)hTax,100M);

            return decimal.Zero;
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
           if(startDate.HasValue && endDate.HasValue)
           {
               if(startDate.Value == endDate.Value)
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

            if(!startDate.HasValue && !endDate.HasValue)
            {
                model.SalesModel = salesModel.Where(x => x.DateSold.ToShortDateString().Equals(DateTime.Today.ToShortDateString(),StringComparison.InvariantCultureIgnoreCase)).OrderByDescending(x => x.DateSold).ToList();
            }

            model.ModelGroupBy = model.SalesModel.GroupBy(x => x.PaymentTypeName).Select(x => new SoldItemModel { PaymentTypeName = x.Key, TotalPrice = x.Sum(y => y.TotalPrice), ItemNewlst = x.ToList()}).ToList();
            
            return View(model);
        }

        private List<SoldItemModel> ConvertToListDeletedItems(System.Data.DataSet ds)
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
              
                try
                {
                    sim.GuestId = int.Parse(ds.Tables[0].Rows[i][5].ToString());
                }
                catch
                {
                    sim.GuestId = null;
                }

                lst.Add(sim);
            }

            return lst;
        }

        private List<SoldItemModel> ConvertToListB4(System.Data.DataSet ds)
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
                sim.DistributionPointId = int.Parse(ds.Tables[0].Rows[i][11].ToString());
                lst.Add(sim);
            }

            return lst;
        }

        private IEnumerable<SoldItemModel> ConvertToList(List<Payment> paymentList)
        {
            List<SoldItemModel> lst = new List<SoldItemModel>();

            int count = paymentList.Count;

            for (int i = 0; i < count; i++)
            {
                SoldItemModel sim = new SoldItemModel();
                sim.Description = paymentList[i].ReceiptNumber.ToString();
                sim.Quantity = 0;
                sim.Total = decimal.Parse(paymentList[i].Total.ToString());
                sim.SubTotal = decimal.Parse(paymentList[i].SubTotal.ToString());
                sim.TotalTax = decimal.Parse(paymentList[i].TaxAmount.ToString());
                sim.TotalDiscount = decimal.Parse(paymentList[i].DiscountAmount.ToString());
                sim.TotalServiceCharge = decimal.Parse(paymentList[i].ServiceChargeAmount.ToString());
                sim.TotalResident = decimal.Parse(paymentList[i].ResidentAmount.ToString());

                sim.DateSold = DateTime.Parse(paymentList[i].PaymentDate.ToString());
                sim.PersonName = paymentList[i].Person.DisplayName.ToString();
                sim.PaymentTypeName = "1";
                sim.PaymentMethodNote = "";
                sim.PaymentMethodName = paymentList[i].PaymentMethod.Name.ToString();
                sim.TimeOfSale = sim.DateSold;

                try
                {
                    sim.GuestId = int.Parse(paymentList[i].GuestId.ToString());
                }
                catch
                {
                    sim.GuestId = null;
                }

                sim.PaymentMethodId = int.Parse(paymentList[i].PaymentMethodId.ToString());
                sim.DistributionPointId = int.Parse(paymentList[i].DistributionPointId.ToString());

                try
                {
                    sim.TillOpen = false;
                }
                catch
                {
                    sim.TillOpen = false;
                }

                try
                {
                    sim.RecieptNumber = paymentList[i].ReceiptNumber.ToString();
                }
                catch
                {
                    sim.RecieptNumber = "";
                }

                yield return sim;

            }
        }

        private IEnumerable<SoldItemModel> ConvertToList(System.Data.DataSet ds)
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
                sim.DistributionPointId = int.Parse(ds.Tables[0].Rows[i][11].ToString());

                try
                {
                    sim.TillOpen = bool.Parse(ds.Tables[0].Rows[i][12].ToString());
                }
                catch
                {
                    sim.TillOpen = false;
                }

                try
                {
                    sim.RecieptNumber = ds.Tables[0].Rows[i][13].ToString();
                }
                catch
                {
                    sim.RecieptNumber = "";
                }

                yield return sim;

            }

            //return lst;
        }

        private IEnumerable<SoldItemModel> ConvertToListB4Now(System.Data.DataSet ds)
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
                sim.DistributionPointId = int.Parse(ds.Tables[0].Rows[i][11].ToString());

                //lst.Add(sim);

                yield return sim;
                
            }

            //return lst;
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