using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using HotelMateWeb.Dal.DataCore;
using HotelMateWeb.Services.Core;
using HotelMateWeb.Services.ServiceApi;
using BarAndRestaurantMate.Helpers;
using BarAndRestaurantMate.Helpers.Enums;
using BarAndRestaurantMate.Models;
using Microsoft.AspNet.Identity;
using DotNet.Highcharts;
using DotNet.Highcharts.Helpers;
using DotNet.Highcharts.Options;
using DotNet.Highcharts.Enums;
using System.Net.Mail;
using System.Net;

namespace BarAndRestaurantMate.Controllers
{
    [Authorize()]
    [HandleError(View = "CustomErrorView")]
    public class BarAdminController : Controller
    {
         private readonly IRoomService _roomService;
         private readonly IRoomTypeService _roomTypeService;
         private readonly IRoomStatuService _roomStatusService;
         private readonly IGuestService _guestService;
         private readonly IGuestRoomService _guestRoomService;
         private readonly IGuestReservationService _guestReservationService;
         private readonly IBusinessAccountService _businessAccountService;
         private readonly ITransactionService _transactionService;
         private readonly IGuestRoomAccountService _guestRoomAccountService;
         private readonly IPaymentService _soldItemService;

         private readonly IPersonService _personService = null;

         private int? _hotelId;
         private int HotelID
         {
             get { return _hotelId ?? GetHotelId(); }
             set { _hotelId = value; }
         }

         private int GetHotelId()
         {
             var username = User.Identity.Name;
             User.Identity.GetUserName();
             var user = _personService.GetAllForLogin().FirstOrDefault(x => x.Username.Equals(username, StringComparison.CurrentCultureIgnoreCase));
             return user.HotelId;
         }

         public void SendEmail()
         {
             MailMessage mail = new MailMessage("leboston@yahoo.com", "sendTo", "mailSubject", "mailBody");
             mail.From = new MailAddress("academyvistang@gmail.com", "HotelMate");
             mail.IsBodyHtml = true; // necessary if you're using html email
             NetworkCredential credential = new NetworkCredential("academyvistang@gmail.com", "Lauren280701");
             SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
             smtp.EnableSsl = true;
             smtp.UseDefaultCredentials = false;
             smtp.Credentials = credential;
             smtp.Send(mail);
         }


        public BarAdminController()
        {
            _personService = new PersonService();
            _roomService = new RoomService();
            _roomTypeService = new RoomTypeService();
            _guestService = new GuestService();
            _guestRoomService = new GuestRoomService();
            _businessAccountService = new BusinessAccountService();
            _guestReservationService = new GuestReservationService();
            _transactionService = new TransactionService();
            _guestRoomAccountService = new GuestRoomAccountService();
            _roomStatusService = new RoomStatuService();
            _soldItemService = new PaymentService();
        }

        public ActionResult GetGraph()
        {
            var lst = new List<string []>();
            int daysInThisMonth = DateTime.Now.Day;
            var startOfMonth = new DateTime(DateTime.Now.Year,DateTime.Now.Month,1);
            var guestSales = _guestRoomService.GetAll(HotelID).Where(x => x.CheckinDate.IsBetween(startOfMonth, DateTime.Now));

            for(int i = 1; i <= daysInThisMonth; i++)
            {
               lst.Add(GetTodaysSales(i,guestSales));
            }
            

            return Json(lst.ToArray(),JsonRequestBehavior.AllowGet);
        }

        private string[] GetTodaysSales(int i, IEnumerable<GuestRoom> guestSales)
        {
            var dateNow = new DateTime(DateTime.Now.Year,DateTime.Now.Month,i);
            var sales = guestSales.Where(x => x.CheckinDate.Date == dateNow.Date).CreditSummation();
            return new[] { i.ToString(),sales.ToString() };
        }

        public ActionResult CategoryCreate()
        {
            //var gr = _guestRoomService.GetAll(HotelID).ToList();
            return View(new AdminViewModel {  });
        }

        //
        public ActionResult Marketing()
        {            

            var adminViewModel = new AdminViewModel();
            var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var guestSales = _guestRoomService.GetAll(HotelID).Where(x => x.CheckinDate.IsBetween(startOfMonth, DateTime.Now));
            var creditSales = guestSales.CreditSummation();
            var debitSales = guestSales.DebitSummation();
            adminViewModel.ProfitSales = creditSales - debitSales;
            adminViewModel.MonthlyCreditSales = creditSales;
            adminViewModel.MonthlyDebitSales = debitSales;
            adminViewModel.NumberOfGuests = _guestService.GetAll(HotelID).Count(x => x.IsActive);
            adminViewModel.LongTermStay = guestSales.Count(x => x.GetNumberOfNights() > 7);
            adminViewModel.ShortTermStay = guestSales.Count(x => x.GetNumberOfNights() < 7);
            adminViewModel.ReservationCount = _guestReservationService.GetAll(HotelID).Count(x => x.StartDate > startOfMonth);
            adminViewModel.GuestSales = guestSales;
            var totalSales = guestSales.Sum(guestRoom => guestRoom.GuestRoomAccounts.Summation());
            var everything = guestSales.Sum(guestRoom => guestRoom.GuestRoomAccounts.Where(x => x.PaymentTypeId == (int)RoomPaymentTypeEnum.Laundry
                || x.PaymentTypeId == (int)RoomPaymentTypeEnum.Restuarant).Summation());
            adminViewModel.RoomOnlySales = totalSales - everything;

            return View(adminViewModel);
        }

        private DotNet.Highcharts.Helpers.Data GetRandom(MonthSales[] courses, int termId, ref List<MonthSales> validCourses)
        {
            List<Point> p = new List<Point>();

            if (validCourses == null)
                validCourses = new List<MonthSales>();

            var actualList = courses;

            if (termId == 1)
            {
                actualList = courses.Where(x => x.Scale < 4).ToArray();
            }
            else if (termId == 2)
            {
                actualList = courses.Where(x => x.Scale >= 4 && x.Scale <= 6).ToArray();
            }
            else if (termId == 3)
            {
                actualList = courses.Where(x => x.Scale >= 7 && x.Scale <= 9).ToArray();
            }
            else
            {
                actualList = courses.Where(x => x.Scale > 9).ToArray();
            }

            foreach (var n in actualList)
            {
                var startDate = GetStartDate(n);

                var endDate = GetEndDate(n);

                decimal? grade = decimal.Zero;

                grade = _soldItemService.GetAll().Where(x => x.PaymentDate >= startDate && x.PaymentDate <= endDate).Sum(x => x.Total);

                var actualScore = decimal.Zero;

                if (grade != null)
                    actualScore = grade.Value;

                n.Value = actualScore;
                validCourses.Add(n);
                p.Add(new Point { Y = (Number)actualScore });

                //if (actualScore > 0)
                //{
                //    n.Value = actualScore;
                //    validCourses.Add(n);
                //    p.Add(new Point { Y = (Number)actualScore });
                //}
                //else
                //{
                //    validCourses.Add(n);
                //}
            }

            return new DotNet.Highcharts.Helpers.Data(p.ToArray());
        }

        private DateTime GetEndDate(MonthSales month)
        {
            if (month.Name == "January")
                return new DateTime(DateTime.Now.Year, 1, 31);
            else if (month.Name == "February")
                return new DateTime(DateTime.Now.Year, 2, 28);
            else if (month.Name == "March")
                return new DateTime(DateTime.Now.Year, 3, 31);
            else if (month.Name == "April")
                return new DateTime(DateTime.Now.Year, 4, 30);
            else if (month.Name == "May")
                return new DateTime(DateTime.Now.Year, 5, 31);
            else if (month.Name == "June")
                return new DateTime(DateTime.Now.Year, 6, 30);
            else if (month.Name == "July")
                return new DateTime(DateTime.Now.Year, 7, 31);
            else if (month.Name == "August")
                return new DateTime(DateTime.Now.Year, 8, 31);
            else if (month.Name == "September")
                return new DateTime(DateTime.Now.Year, 9, 30);
            else if (month.Name == "October")
                return new DateTime(DateTime.Now.Year, 10, 31);
            else if (month.Name == "November")
                return new DateTime(DateTime.Now.Year, 11, 30);
            else
                return new DateTime(DateTime.Now.Year, 12, 31);
        }

        private DateTime GetStartDate(MonthSales month)
        {
            if (month.Name == "January")
                return new DateTime(DateTime.Now.Year, 1, 1);
            else if (month.Name == "February")
                return new DateTime(DateTime.Now.Year, 2, 1);
            else if (month.Name == "March")
                return new DateTime(DateTime.Now.Year, 3, 1);
            else if (month.Name == "April")
                return new DateTime(DateTime.Now.Year, 4, 1);
            else if (month.Name == "May")
                return new DateTime(DateTime.Now.Year, 5, 1);
            else if (month.Name == "June")
                return new DateTime(DateTime.Now.Year, 6, 1);
            else if (month.Name == "July")
                return new DateTime(DateTime.Now.Year, 7, 1);
            else if (month.Name == "August")
                return new DateTime(DateTime.Now.Year, 8, 1);
            else if (month.Name == "September")
                return new DateTime(DateTime.Now.Year, 9, 1);
            else if (month.Name == "October")
                return new DateTime(DateTime.Now.Year, 10, 1);
            else if (month.Name == "November")
                return new DateTime(DateTime.Now.Year, 11, 1);
            else
                return new DateTime(DateTime.Now.Year, 12, 1);
        }

        public Highcharts ColumnWithDrilldownSector()
        {


            var months = new MonthSales[] {
                new MonthSales { Name = "January", Scale = 1 },
                new MonthSales { Name = "February", Scale = 2 }, 
                new MonthSales { Name = "March", Scale = 3 },
                new MonthSales { Name = "April", Scale = 4 },
                new MonthSales { Name = "May", Scale = 5 }, 
                new MonthSales { Name = "June", Scale = 6 },
                new MonthSales { Name = "July", Scale = 7 }, 
                new MonthSales { Name = "August", Scale = 8 }, 
                new MonthSales { Name = "September", Scale = 9 },
                new MonthSales { Name = "October", Scale = 10 }, 
                new MonthSales { Name = "November", Scale = 11 }, 
                new MonthSales { Name = "December", Scale = 12 } };


            var coursesNames = months.Select(x => x.Name).ToArray();

            List<MonthSales> firststTermCourseList = new List<MonthSales>();
            List<MonthSales> secondTermCourseList = new List<MonthSales>();
            List<MonthSales> thirdTermCourseList = new List<MonthSales>();
            List<MonthSales> fourthTermCourseList = new List<MonthSales>();


            string[] firstTermSubjects = coursesNames;
            string[] secondTermSubjects = coursesNames;
            string[] thirdTermSubjects = coursesNames;
            string[] fourthTermSubjects = coursesNames;


            var firstTermData = GetRandom(months, 1, ref firststTermCourseList);
            var secondTermData = GetRandom(months, 2, ref secondTermCourseList);
            var thirdTermData = GetRandom(months, 3, ref thirdTermCourseList);
            var fourthTermData = GetRandom(months, 4, ref fourthTermCourseList);


            firstTermSubjects = firststTermCourseList.Select(x => x.Name).ToArray();
            secondTermSubjects = secondTermCourseList.Select(x => x.Name).ToArray();
            thirdTermSubjects = thirdTermCourseList.Select(x => x.Name).ToArray();
            fourthTermSubjects = fourthTermCourseList.Select(x => x.Name).ToArray();


            var p1 = (Number)GetActualData(1);
            var p2 = (Number)GetActualData(2);
            var p3 = (Number)GetActualData(3);
            var p4 = (Number)GetActualData(4);



            string[] categories = new[] { "1st Quarter", "2nd Quarter", "3rd Quarter", "4th Quarter" };
            string strName = " Year Sales";
            DotNet.Highcharts.Helpers.Data data = new DotNet.Highcharts.Helpers.Data(new[]
                                 {
                                     new Point
                                     {
                                         Y = p1,
                                         Color = System.Drawing.Color.FromName("colors[0]"),
                                         Drilldown = new Drilldown
                                                     {
                                                         Name = "1st Quarter",
                                                         Categories = firstTermSubjects,
                                                         Data = firstTermData,
                                                         Color = System.Drawing.Color.FromName("colors[0]")
                                                     }
                                     },
                                     new Point
                                     {
                                         Y = p2,
                                         Color = System.Drawing.Color.FromName("colors[1]"),
                                         Drilldown = new Drilldown
                                                     {
                                                         Name = "2nd Quarter",
                                                         Categories = secondTermSubjects,
                                                         Data = secondTermData,
                                                         Color = System.Drawing.Color.FromName("colors[1]")
                                                     }
                                     },
                                     new Point
                                     {
                                         Y = p3,
                                         Color = System.Drawing.Color.FromName("colors[2]"),
                                         Drilldown = new Drilldown
                                                     {
                                                         Name = "3rd Quarter", 
                                                         Categories = thirdTermSubjects,
                                                         Data = thirdTermData,
                                                         Color = System.Drawing.Color.FromName("colors[2]")
                                                     }
                                     } ,
                                     new Point
                                     {
                                         Y = p4,
                                         Color = System.Drawing.Color.FromName("colors[3]"),
                                         Drilldown = new Drilldown
                                                     {
                                                         Name = "4th Quarter", 
                                                         Categories = fourthTermSubjects,
                                                         Data = fourthTermData,
                                                         Color = System.Drawing.Color.FromName("colors[3]")
                                                     }
                                     }
                                 });

            Highcharts chart = new Highcharts("chart")
            .InitChart(new Chart { DefaultSeriesType = ChartTypes.Column })
            .SetTitle(new Title { Text = strName })
            .SetSubtitle(new Subtitle { Text = "Click the columns for Quarter View. Click again for Month View." })
            .SetXAxis(new XAxis { Categories = categories })
            .SetYAxis(new YAxis { Title = new YAxisTitle { Text = "Sales (NGN)" } })
            .SetLegend(new Legend { Enabled = false })
            .SetTooltip(new Tooltip { Formatter = "TooltipFormatter" })
            .SetPlotOptions(new PlotOptions
            {
                Column = new PlotOptionsColumn
                {
                    Cursor = Cursors.Pointer,
                    Point = new PlotOptionsColumnPoint { Events = new PlotOptionsColumnPointEvents { Click = "ColumnPointClick" } },
                    DataLabels = new PlotOptionsColumnDataLabels
                    {
                        Enabled = true,
                        Color = System.Drawing.Color.FromName("colors[0]"),
                        Formatter = "function() { return this.y +' '; }",
                        Style = "fontWeight: 'bold'"
                    }
                }
            })
            .SetSeries(new Series
            {
                Name = "Sales",
                Data = data,
                Color = System.Drawing.Color.White
            })
            .SetExporting(new Exporting { Enabled = false })
            .AddJavascripFunction(
                "TooltipFormatter",
                @"var point = this.point, s = this.x +':<b>'+ this.y +' </b><br/>';
                      if (point.drilldown) {
                        s += 'Click to view by '+ point.category +' results';
                      } else {
                        s += 'Click to return to Quarter View';
                      }
                      return s;"
            )
            .AddJavascripFunction(
                "ColumnPointClick",
                @"var drilldown = this.drilldown;
                      if (drilldown) { // drill down
                        setChart(drilldown.name, drilldown.categories, drilldown.data.data, drilldown.color);
                      } else { // restore
                        setChart(name, categories, data.data);
                      }"
            )
            .AddJavascripFunction(
                "setChart",
                @"chart.xAxis[0].setCategories(categories);
                      chart.series[0].remove();
                      chart.addSeries({
                         name: name,
                         data: data,
                         color: color || 'white'
                      });",
                "name", "categories", "data", "color"
            )
            .AddJavascripVariable("colors", "Highcharts.getOptions().colors")
            .AddJavascripVariable("name", "'{0}'".FormatWith(strName))
            .AddJavascripVariable("categories", JsonSerializer.Serialize(categories))
            .AddJavascripVariable("data", JsonSerializer.Serialize(data));
            return chart;
        }

        private Number GetActualData(int quarter)
        {

            if (quarter == 1)
            {
                DateTime startDate = new DateTime(DateTime.Now.Year, 1, 1);
                DateTime endDate = new DateTime(DateTime.Now.Year, 3, 31);
                return (Number)_soldItemService.GetAll().Where(x => x.PaymentDate >= startDate && x.PaymentDate <= endDate).Sum(x => x.Total);
            }
            else if (quarter == 2)
            {
                DateTime startDate = new DateTime(DateTime.Now.Year, 4, 1);
                DateTime endDate = new DateTime(DateTime.Now.Year, 6, 30);
                return (Number)_soldItemService.GetAll().Where(x => x.PaymentDate >= startDate && x.PaymentDate <= endDate).Sum(x => x.Total);
            }
            else if (quarter == 3)
            {
                DateTime startDate = new DateTime(DateTime.Now.Year, 7, 1);
                DateTime endDate = new DateTime(DateTime.Now.Year, 9, 30);
                return (Number)_soldItemService.GetAll().Where(x => x.PaymentDate >= startDate && x.PaymentDate <= endDate).Sum(x => x.Total);
            }
            else
            {
                DateTime startDate = new DateTime(DateTime.Now.Year, 10, 1);
                DateTime endDate = new DateTime(DateTime.Now.Year, 12, 31);
                return (Number)_soldItemService.GetAll().Where(x => x.PaymentDate >= startDate && x.PaymentDate <= endDate).Sum(x => x.Total);
            }
        }

        public ActionResult Index()
        {
            var adminViewModel = new AdminViewModel();
            adminViewModel.ColumnLineAndPie = ColumnWithDrilldownSector();
            return View(adminViewModel);
        }

        public ActionResult Tables()
        {
            var gr = _guestRoomService.GetAll(HotelID).ToList();
            return View(new AdminViewModel { GuestRooms = gr });            
        }

        //[HttpGet]
        //public ActionResult EditExpenses(int? id)
        //{
        //    var expenses = _expensesService.GetById(id.Value);
        //    var model = GetModelForNewExpenses(expenses);
        //    model.ExpensesTypeList = GetExpensesTypes(expenses.ExpensesType);
        //    return View(model);
        //}

        //[HttpGet]
        //public ActionResult NewExpenses()
        //{
        //    var model = GetModelForNewExpenses(null);
        //    model.ExpensesTypeList = GetExpensesTypes(null);
        //    return View("NewExpenses", model);
        //}

        //[HttpPost]
        //public ActionResult NewExpenses(ExpensesViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        Mapper.CreateMap<GuestViewModel, Guest>();
        //        var expenses = Mapper.Map<ExpensesViewModel, Expenses>(model);
        //        expenses.IsActive = true;

        //        if (expenses.Id > 0)
        //        {
        //            var existingExpenses = _expensesService.GetById(expenses.Id);
        //            existingExpenses.ExtNumber = expenses.ExtNumber;
        //            existingExpenses.NoOfBeds = expenses.NoOfBeds;
        //            existingExpenses.Notes = expenses.Notes;
        //            existingExpenses.Price = expenses.Price;
        //            existingExpenses.ExpensesNumber = expenses.ExpensesNumber;
        //            existingExpenses.ExpensesType = expenses.ExpensesType;
        //            _expensesService.Update(existingExpenses);
        //        }
        //        else
        //        {
        //            _expensesService.Create(expenses);
        //        }

        //        return RedirectToAction("Booking", "Home");
        //    }

        //    model.ExpensesTypeList = GetExpensesTypes(model.ExpensesType);
        //    return View(model);
        //}

        //private IEnumerable<SelectListItem> GetExpensesTypes(int? selectedId)
        //{
        //    if (!selectedId.HasValue)
        //        selectedId = 0;

        //    var bas =
        //        _expensesTypeService.GetAll(HotelID).ToList();
        //    bas.Insert(0, new ExpensesType { Name = "-- Please Select --", Id = 0 });
        //    return bas.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString(CultureInfo.InvariantCulture), Selected = x.Id == selectedId });
        //}

        //private ExpensesViewModel GetModelForNewExpenses(Expenses expenses)
        //{
        //    Mapper.CreateMap<Expenses, ExpensesViewModel>();
        //    expenses = expenses ?? new Expenses { IsActive = true };
        //    var rvm = Mapper.Map<Expenses, ExpensesViewModel>(expenses);
        //    return rvm;
        //}

       //Expenses

        [HttpGet]
        public ActionResult EditCreditAccount(int? id)
        {
            var creditAccount = _businessAccountService.GetById(id.Value);
            var model = GetModelForNewCreditAccount(creditAccount);
            return View("NewCreditAccount", model);
        }

        [HttpPost]
        public ActionResult NewCreditAccount(CreditAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                Mapper.CreateMap<CreditAccountViewModel, BusinessAccount>();
                var creditAccount = Mapper.Map<CreditAccountViewModel, BusinessAccount>(model);
                creditAccount.Status = "LIVE";

                if (creditAccount.Id > 0)
                {
                    var existingAccount = _businessAccountService.GetById(creditAccount.Id);
                    existingAccount.Address = creditAccount.Address;
                    existingAccount.CompanyAddress = creditAccount.CompanyAddress;
                    existingAccount.CompanyName = creditAccount.CompanyName;
                    existingAccount.CompanyTelephone = creditAccount.CompanyTelephone;
                    existingAccount.ContactName = creditAccount.ContactName;
                    existingAccount.ContactNumber = creditAccount.ContactNumber;
                    existingAccount.Email = creditAccount.Email;
                    existingAccount.Mobile = creditAccount.Mobile;
                    existingAccount.Name = creditAccount.Name;
                    existingAccount.NatureOfBusiness = creditAccount.NatureOfBusiness;
                    existingAccount.Telephone = creditAccount.Telephone;
                    _businessAccountService.Update(existingAccount);
                }
                else
                {
                    creditAccount.Debtor = false;
                    _businessAccountService.Create(creditAccount);
                }

                return RedirectToAction("Booking", "Home");
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult NewCreditAccount()
        {
            var model = GetModelForNewCreditAccount(null);
            return View("NewCreditAccount", model);
        }

        private CreditAccountViewModel GetModelForNewCreditAccount(BusinessAccount creditAccount)
        {
            Mapper.CreateMap<BusinessAccount, CreditAccountViewModel>();
            creditAccount = creditAccount ?? new BusinessAccount {Status = "LIVE" };
            var rvm = Mapper.Map<BusinessAccount, CreditAccountViewModel>(creditAccount);
            return rvm;
        }

        [HttpGet]
        public ActionResult EditRoom(int? id)
        {
            var room = _roomService.GetById(id.Value);
            var model = GetModelForNewRoom(room);
            model.RoomTypeList = GetRoomTypes(room.RoomType);
            model.RoomStatusList = GetRoomStatus(room.StatusId);
            return View("NewRoom", model);
        }

        [HttpGet]
        public ActionResult EditRoomChangeStatus(int? id)
        {
            var room = _roomService.GetById(id.Value);
            var model = GetModelForNewRoom(room);
            model.RoomTypeList = GetRoomTypes(room.RoomType);
            model.RoomStatusList = GetRoomStatusVacantOnly(room.StatusId);
            return View("NewRoomChangeStatus", model);
        }

        //

        [HttpGet]
        public ActionResult NewRoom()
        {
            var model = GetModelForNewRoom(null);
            model.RoomTypeList = GetRoomTypes(null);
            model.RoomStatusList = GetRoomStatus(null);
            return View("NewRoom", model);
        }

        [HttpPost]
        public ActionResult NewRoom(RoomViewModel model)
        {
            if (ModelState.IsValid)
            {
                Mapper.CreateMap<RoomViewModel, Room>();
                var room = Mapper.Map<RoomViewModel, Room>(model);
                room.IsActive = true;

                if (room.Id > 0)
                {
                    var existingRoom = _roomService.GetById(room.Id);
                    existingRoom.ExtNumber = room.ExtNumber;
                    existingRoom.NoOfBeds = room.NoOfBeds;
                    existingRoom.Notes = room.Notes;
                    existingRoom.Price = room.Price;
                    existingRoom.RoomNumber = room.RoomNumber;
                    existingRoom.RoomType = room.RoomType;
                    existingRoom.StatusId = room.StatusId;
                    _roomService.Update(existingRoom);
                }
                else
                {
                    _roomService.Create(room);
                }

                return RedirectToAction("Booking", "Home");
            }

            model.RoomTypeList = GetRoomTypes(model.RoomType);
            model.RoomStatusList = GetRoomStatus(model.StatusId);

            return View(model);
        }

        private IEnumerable<SelectListItem> GetRoomTypes(int? selectedId)
        {
            if (!selectedId.HasValue)
                selectedId = 0;

            var bas =
                _roomTypeService.GetAll(HotelID).ToList();
            bas.Insert(0, new RoomType { Name = "-- Please Select --", Id = 0 });
            return bas.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString(CultureInfo.InvariantCulture), Selected = x.Id == selectedId });
        }
        

        private IEnumerable<SelectListItem> GetRoomStatusVacantOnly(int? selectedId)
        {
            if (!selectedId.HasValue)
                selectedId = 0;

            var bas =
                _roomStatusService.GetAll(HotelID).Where(x => x.Id == (int)RoomStatusEnum.Vacant).ToList();
            //bas.Insert(0, new RoomStatu { Name = "-- Please Select --", Id = 0 });
            return bas.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString(CultureInfo.InvariantCulture), Selected = x.Id == selectedId });
        }

        private IEnumerable<SelectListItem> GetRoomStatus(int? selectedId)
        {
            if (!selectedId.HasValue)
                selectedId = 0;

            var bas =
                _roomStatusService.GetAll(HotelID).Where(x => x.Id != (int)RoomStatusEnum.Occupied).ToList();

            bas.Insert(0, new RoomStatu { Name = "-- Please Select --", Id = 0 });

            return bas.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString(CultureInfo.InvariantCulture), Selected = x.Id == selectedId });
        }

        private RoomViewModel GetModelForNewRoom(Room room)
        {
            Mapper.CreateMap<Room, RoomViewModel>();
            room = room ?? new Room{IsActive = true};
            var rvm = Mapper.Map<Room, RoomViewModel>(room);
            return rvm;
        }

      

        [HttpGet]
        public ActionResult GuestCheckinReport(DateTime? startDate, DateTime? endDate, int? page)
        {
            startDate = startDate ?? DateTime.Now.AddMonths(-1);
            endDate = endDate ?? DateTime.Now.AddMonths(1);

            const int pageSize = 1;
            var entirelist = _guestRoomService.GetByQuery(HotelID).Where(x => x.CheckinDate > startDate.Value && x.CheckinDate < endDate.Value && x.IsActive && x.Room.RoomStatu.Id == (int) RoomStatusEnum.Occupied);
            var paginatedList = new PaginatedList<GuestRoom>(entirelist, page ?? 0, pageSize, startDate, endDate);

            var avm = new AdminViewModel
                {
                    PaginatedGuestRoomList = paginatedList
                };

            return View(avm);
        }

        [HttpGet]
        public ActionResult GuestCheckoutReport(DateTime? startDate, DateTime? endDate, int? page)
        {
            startDate = startDate ?? DateTime.Now.AddMonths(-1);
            endDate = endDate ?? DateTime.Now.AddMonths(1);

            const int pageSize = 1;
            var entirelist = _guestRoomService.GetByQuery(HotelID).Where(x => x.CheckoutDate > startDate.Value && x.CheckoutDate < endDate.Value && !x.Guest.IsActive);
            var paginatedList = new PaginatedList<GuestRoom>(entirelist, page ?? 0, pageSize, startDate, endDate);

            var avm = new AdminViewModel
            {
                PaginatedGuestRoomList = paginatedList
            };

            return View(avm);
        }

        [HttpGet]
        public ActionResult DueReservationReport(DateTime? startDate, DateTime? endDate, int? page)
        {
            startDate = startDate ?? DateTime.Now.AddDays(-1);
            endDate = endDate ?? DateTime.Now.AddDays(7);

            const int pageSize = 1;
            var entirelist = _guestReservationService.GetByQuery(HotelID).Where(x => x.StartDate > startDate.Value && x.StartDate < endDate.Value && x.IsActive);
            var paginatedList = new PaginatedList<GuestReservation>(entirelist, page ?? 0, pageSize, startDate, endDate);

            var avm = new AdminViewModel
            {
                PaginatedGuestReservationList = paginatedList
            };

            return View(avm);
        }

        [HttpGet]
        public ActionResult GuestFolioReport(DateTime? startDate, DateTime? endDate, int? page)
        {
            startDate = startDate ?? DateTime.Now.AddMonths(-1);
            endDate = endDate ?? DateTime.Now.AddMonths(1);

            const int pageSize = 1;
            var entirelist = _guestService.GetByQuery(HotelID).Where(x => x.IsActive);
            var paginatedList = new PaginatedList<Guest>(entirelist, page ?? 0, pageSize, startDate, endDate);

            var avm = new AdminViewModel
            {
                PaginatedGuestList = paginatedList
            };

            return View(avm);
        }

        [HttpGet]
        public ActionResult GuestListReport(DateTime? startDate, DateTime? endDate, int? page)
        {
            startDate = startDate ?? DateTime.Now.AddMonths(-1);
            endDate = endDate ?? DateTime.Now.AddMonths(1);

            const int pageSize = 1;
            var entirelist = _guestService.GetByQuery(HotelID).Where(x => x.IsActive && x.CreatedDate.ReportIsBetween(startDate.Value,endDate.Value));
            var paginatedList = new PaginatedList<Guest>(entirelist, page ?? 0, pageSize, startDate, endDate);

            var avm = new AdminViewModel
            {
                PaginatedGuestList = paginatedList
            };

            return View(avm);
        }

        [HttpGet]
        public ActionResult ReservationListReport(DateTime? startDate, DateTime? endDate, int? page)
        {
            startDate = startDate ?? DateTime.Now.AddMonths(-1);
            endDate = endDate ?? DateTime.Now.AddYears(3);

            const int pageSize = 1;
            var entirelist = _guestReservationService.GetByQuery(HotelID).Where(x => x.StartDate > startDate.Value && x.StartDate < endDate.Value && x.IsActive);
            var paginatedList = new PaginatedList<GuestReservation>(entirelist, page ?? 0, pageSize, startDate, endDate);

            var avm = new AdminViewModel
            {
                PaginatedGuestReservationList = paginatedList
            };

            return View(avm);
        }

        [HttpGet]
        public ActionResult RoomHistoryReport(DateTime? startDate, DateTime? endDate, int? page)
        {
            startDate = startDate ?? DateTime.Now.AddMonths(-1);
            endDate = endDate ?? DateTime.Now.AddYears(3);

            const int pageSize = 1;
            var entirelist = _guestRoomService.GetByQuery(HotelID).Where(x => x.CheckinDate.IsBetween(startDate.Value, endDate.Value));
            var paginatedList = new PaginatedList<GuestRoom>(entirelist, page ?? 0, pageSize, startDate, endDate);

            var avm = new AdminViewModel
            {
                PaginatedGuestRoomList = paginatedList
            };

            return View(avm);
        }

        [HttpGet]
        public ActionResult RoomHistoryReportSummary(DateTime? startDate, DateTime? endDate, int? page)
        {
            startDate = startDate ?? DateTime.Now.AddMonths(-1);
            endDate = endDate ?? DateTime.Now.AddYears(3);

            const int pageSize = 1;
            var entirelist = _guestRoomService.GetByQuery(HotelID).Where(x => x.CheckinDate.IsBetween(startDate.Value, endDate.Value));
            var paginatedList = new PaginatedList<GuestRoom>(entirelist, page ?? 0, pageSize, startDate, endDate);

            var avm = new AdminViewModel
            {
                PaginatedGuestRoomList = paginatedList
            };

            return View(avm);
        }

        [HttpGet]
        public ActionResult GroupReservationReport(DateTime? startDate, DateTime? endDate, int? page)
        {
            startDate = startDate ?? DateTime.Now.AddMonths(-1);
            endDate = endDate ?? DateTime.Now.AddYears(3);

            const int pageSize = 1;
            var entirelist = _guestRoomService.GetByQuery(HotelID).Where(x => x.CheckinDate.IsBetween(startDate.Value, endDate.Value) && x.GroupBooking);
            var paginatedList = new PaginatedList<GuestRoom>(entirelist, page ?? 0, pageSize, startDate, endDate);

            var avm = new AdminViewModel
            {
                PaginatedGuestRoomList = paginatedList
            };

            return View(avm);
        }

         
        [HttpGet]
        public ActionResult OtherChargesReport(DateTime? startDate, DateTime? endDate, int? page)
        {
            startDate = startDate ?? DateTime.Now.AddMonths(-1);
            endDate = endDate ?? DateTime.Now.AddYears(3);

            const int pageSize = 1;
            var entirelist = _guestRoomService.GetByQuery(HotelID).Where(x => x.CheckinDate.ReportIsBetween(startDate.Value, endDate.Value) && x.GroupBooking);
            var paginatedList = new PaginatedList<GuestRoom>(entirelist, page ?? 0, pageSize, startDate, endDate);

            var avm = new AdminViewModel
            {
                PaginatedGuestRoomList = paginatedList
            };

            return View(avm);
        }

        [HttpGet]
        public ActionResult AccountReceivableReport(DateTime? startDate, DateTime? endDate, int? page)
        {
            startDate = startDate ?? DateTime.Now.AddMonths(-1);
            endDate = endDate ?? DateTime.Now.AddYears(3);

            const int pageSize = 1;
            var entirelist = _guestRoomAccountService.GetByQuery(HotelID).Where(x => x.TransactionDate.ReportIsBetween(startDate.Value, endDate.Value) && x.RoomPaymentType.PaymentStatusId == (int) RoomPaymentStatusEnum.Credit);
            var paginatedList = new PaginatedList<GuestRoomAccount>(entirelist, page ?? 0, pageSize, startDate, endDate);

            var avm = new AdminViewModel
            {
                PaginatedGuestRoomAccountList = paginatedList
            };

            return View(avm);
        }

        [HttpGet]
        public ActionResult AccountPayableReport(DateTime? startDate, DateTime? endDate, int? page)
        {
            startDate = startDate ?? DateTime.Now.AddMonths(-1);
            endDate = endDate ?? DateTime.Now.AddYears(3);

            const int pageSize = 1;
            var entirelist = _guestRoomAccountService.GetByQuery(HotelID).Where(x => x.TransactionDate.ReportIsBetween(startDate.Value, endDate.Value) && x.RoomPaymentType.PaymentStatusId == (int)RoomPaymentStatusEnum.Debit);
            var paginatedList = new PaginatedList<GuestRoomAccount>(entirelist, page ?? 0, pageSize, startDate, endDate);

            var avm = new AdminViewModel
            {
                PaginatedGuestRoomAccountList = paginatedList
            };

            return View(avm);
        }

        [HttpGet]
        public ActionResult TaxesReport(DateTime? startDate, DateTime? endDate, int? page)
        {
            startDate = startDate ?? DateTime.Now.AddMonths(-1);
            endDate = endDate ?? DateTime.Now.AddYears(3);

            const int pageSize = 1;
            var entirelist = _guestRoomAccountService.GetByQuery(HotelID).Where(x => x.TransactionDate.ReportIsBetween(startDate.Value, endDate.Value) && x.RoomPaymentType.PaymentStatusId == (int)RoomPaymentStatusEnum.Debit);
            var paginatedList = new PaginatedList<GuestRoomAccount>(entirelist, page ?? 0, pageSize, startDate, endDate);

            var avm = new AdminViewModel
            {
                PaginatedGuestRoomAccountList = paginatedList
            };

            return View(avm);
        }

        [HttpGet]
        public ActionResult CreditGuestReport(DateTime? startDate, DateTime? endDate, int? page)
        {
            startDate = startDate ?? DateTime.Now.AddMonths(-1);
            endDate = endDate ?? DateTime.Now.AddYears(3);

            const int pageSize = 1;
            var entirelist = _guestRoomAccountService.GetByQuery(HotelID).Where(x => x.TransactionDate.ReportIsBetween(startDate.Value, endDate.Value) && x.RoomPaymentType.PaymentStatusId == (int)RoomPaymentStatusEnum.Debit);
            var paginatedList = new PaginatedList<GuestRoomAccount>(entirelist, page ?? 0, pageSize, startDate, endDate);

            var avm = new AdminViewModel
            {
                PaginatedGuestRoomAccountList = paginatedList
            };

            return View(avm);
        }

        [HttpGet]
        public ActionResult CompanyCoporateGuestReport(DateTime? startDate, DateTime? endDate, int? page)
        {
            startDate = startDate ?? DateTime.Now.AddMonths(-1);
            endDate = endDate ?? DateTime.Now.AddYears(3);

            const int pageSize = 1;
            var entirelist = _guestService.GetByQuery(HotelID).Where(x => x.CreatedDate.ReportIsBetween(startDate.Value, endDate.Value) && x.CompanyId > 0);
            var paginatedList = new PaginatedList<Guest>(entirelist, page ?? 0, pageSize, startDate, endDate);

            var avm = new AdminViewModel
            {
                PaginatedGuestList = paginatedList
            };

            return View(avm);
        }

        [HttpGet]
        public ActionResult ProfitReport(DateTime? startDate, DateTime? endDate, int? page)
        {
            startDate = startDate ?? DateTime.Now.AddMonths(-1);
            endDate = endDate ?? DateTime.Now.AddYears(3);

            const int pageSize = 1;
            var entirelist = _guestRoomAccountService.GetByQuery(HotelID).Where(x => x.TransactionDate.ReportIsBetween(startDate.Value, endDate.Value) && x.RoomPaymentType.PaymentStatusId == (int)RoomPaymentStatusEnum.Debit);
            var paginatedList = new PaginatedList<GuestRoomAccount>(entirelist, page ?? 0, pageSize, startDate, endDate);

            var avm = new AdminViewModel
            {
                PaginatedGuestRoomAccountList = paginatedList
            };

            return View(avm);
        }

        //Create an expenses table so that profit can be calculated

        [HttpGet]
        public ActionResult GuestDetailsReport(DateTime? startDate, DateTime? endDate, int? page)
        {
            startDate = startDate ?? DateTime.Now.AddMonths(-1);
            endDate = endDate ?? DateTime.Now.AddMonths(1);

            const int pageSize = 1;
            var entirelist = _guestService.GetByQuery(HotelID).Where(x => x.CreatedDate.ReportIsBetween(startDate.Value, endDate.Value));
            var paginatedList = new PaginatedList<Guest>(entirelist, page ?? 0, pageSize, startDate, endDate);

            var avm = new AdminViewModel
            {
                PaginatedGuestList = paginatedList
            };

            return View(avm);
        }

        [HttpGet]
        public ActionResult StateSecurityServiceGuestDetailsReport(DateTime? startDate, DateTime? endDate, int? page)
        {
            startDate = startDate ?? DateTime.Now.AddMonths(-1);
            endDate = endDate ?? DateTime.Now.AddMonths(1);

            const int pageSize = 1;
            var entirelist = _guestService.GetByQuery(HotelID).Where(x => x.IsActive && x.CreatedDate.ReportIsBetween(startDate.Value, endDate.Value));
            var paginatedList = new PaginatedList<Guest>(entirelist, page ?? 0, pageSize, startDate, endDate);

            var avm = new AdminViewModel
            {
                PaginatedGuestList = paginatedList
            };

            return View(avm);
        }


        [HttpGet]
        public ActionResult InitialDepositReport(DateTime? startDate, DateTime? endDate, int? page)
        {
            startDate = startDate ?? DateTime.Now.AddMonths(-1);
            endDate = endDate ?? DateTime.Now.AddYears(3);

            const int pageSize = 1;
            var entirelist = _guestRoomAccountService.GetByQuery(HotelID).Where(x => x.TransactionDate.ReportIsBetween(startDate.Value, endDate.Value) && x.RoomPaymentType.Id == (int)RoomPaymentTypeEnum.InitialDeposit);
            var paginatedList = new PaginatedList<GuestRoomAccount>(entirelist, page ?? 0, pageSize, startDate, endDate);

            var avm = new AdminViewModel
            {
                PaginatedGuestRoomAccountList = paginatedList
            };

            return View(avm);
        }

        [HttpGet]
        public ActionResult ReservationDepositReport(DateTime? startDate, DateTime? endDate, int? page)
        {
            startDate = startDate ?? DateTime.Now.AddMonths(-1);
            endDate = endDate ?? DateTime.Now.AddYears(3);

            const int pageSize = 1;
            var entirelist = _guestRoomAccountService.GetByQuery(HotelID).Where(x => x.TransactionDate.ReportIsBetween(startDate.Value, endDate.Value) && x.RoomPaymentType.Id == (int)RoomPaymentTypeEnum.ReservationDeposit);
            var paginatedList = new PaginatedList<GuestRoomAccount>(entirelist, page ?? 0, pageSize, startDate, endDate);

            var avm = new AdminViewModel
            {
                PaginatedGuestRoomAccountList = paginatedList
            };

            return View(avm);
        }

        [HttpGet]
        public ActionResult BarReport(DateTime? startDate, DateTime? endDate, int? page)
        {
            startDate = startDate ?? DateTime.Now.AddMonths(-1);
            endDate = endDate ?? DateTime.Now.AddYears(3);

            const int pageSize = 1;
            var entirelist = _guestRoomAccountService.GetByQuery(HotelID).Where(x => x.TransactionDate.ReportIsBetween(startDate.Value, endDate.Value) && x.RoomPaymentType.Id == (int)RoomPaymentTypeEnum.Bar);
            var paginatedList = new PaginatedList<GuestRoomAccount>(entirelist, page ?? 0, pageSize, startDate, endDate);

            var avm = new AdminViewModel
            {
                PaginatedGuestRoomAccountList = paginatedList
            };

            return View(avm);
        }

        [HttpGet]
        public ActionResult RestaurantReport(DateTime? startDate, DateTime? endDate, int? page)
        {
            startDate = startDate ?? DateTime.Now.AddMonths(-1);
            endDate = endDate ?? DateTime.Now.AddYears(3);

            const int pageSize = 1;
            var entirelist = _guestRoomAccountService.GetByQuery(HotelID).Where(x => x.TransactionDate.ReportIsBetween(startDate.Value, endDate.Value) && x.RoomPaymentType.Id == (int)RoomPaymentTypeEnum.Restuarant);
            var paginatedList = new PaginatedList<GuestRoomAccount>(entirelist, page ?? 0, pageSize, startDate, endDate);

            var avm = new AdminViewModel
            {
                PaginatedGuestRoomAccountList = paginatedList
            };

            return View(avm);
        }

        [HttpGet]
        public ActionResult MiscellenousReport(DateTime? startDate, DateTime? endDate, int? page)
        {
            startDate = startDate ?? DateTime.Now.AddMonths(-1);
            endDate = endDate ?? DateTime.Now.AddYears(3);

            const int pageSize = 1;
            var entirelist = _guestRoomAccountService.GetByQuery(HotelID).Where(x => x.TransactionDate.ReportIsBetween(startDate.Value, endDate.Value) && x.RoomPaymentType.Id == (int)RoomPaymentTypeEnum.Laundry);
            var paginatedList = new PaginatedList<GuestRoomAccount>(entirelist, page ?? 0, pageSize, startDate, endDate);

            var avm = new AdminViewModel
            {
                PaginatedGuestRoomAccountList = paginatedList
            };

            return View(avm);
        }

        [HttpGet]
        public ActionResult RefundReport(DateTime? startDate, DateTime? endDate, int? page)
        {
            startDate = startDate ?? DateTime.Now.AddMonths(-1);
            endDate = endDate ?? DateTime.Now.AddYears(3);

            const int pageSize = 1;
            var entirelist = _guestRoomAccountService.GetByQuery(HotelID).Where(x => x.TransactionDate.ReportIsBetween(startDate.Value, endDate.Value) && x.RoomPaymentType.Id == (int)RoomPaymentTypeEnum.Refund);
            var paginatedList = new PaginatedList<GuestRoomAccount>(entirelist, page ?? 0, pageSize, startDate, endDate);

            var avm = new AdminViewModel
            {
                PaginatedGuestRoomAccountList = paginatedList
            };

            return View(avm);
        }

        private string GetPaymentType(int paymentType)
        {
            if (paymentType == (int)RoomPaymentTypeEnum.Bar)
            {
                return "BAR";
            }

            if (paymentType == (int)RoomPaymentTypeEnum.Restuarant)
            {
                return "RESTAURANT";
            }

            if (paymentType == (int)RoomPaymentTypeEnum.Laundry)
            {
                return "LAUNDRY";
            }

            return "MISCELLANEOUS";
        }
    }
}