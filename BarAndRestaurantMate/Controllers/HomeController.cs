using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using HotelMateWeb.Dal.DataCore;
using HotelMateWeb.Services.Core;
using HotelMateWeb.Services.ServiceApi;
using BarAndRestaurantMate.Helpers;
using BarAndRestaurantMate.Helpers.Enums;
using BarAndRestaurantMate.Models;
using BarAndRestaurantMate.Security;
using System.IO;
using Lib.Web.Mvc;
using System.Configuration;

using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Net.NetworkInformation;
using BarAndRestaurantMate.SignalrRepository;
using System.Net.Mail;
using System.Net;
using Microsoft.PointOfService;

namespace BarAndRestaurantMate.Controllers
{
    [HandleError(View = "CustomErrorView")]
    public class HomeController : Controller
    {
        private  IRoomService _roomService;
        private  IGuestService _guestService;
        private  IGuestReservationService _guestReservationService;
        private  IPersonService _personService = null;
        private  IGuestRoomService _guestRoomService;
        private  IGuestRequestItemService _guestRequestItemService;
        private  ITableItemService _tableItemService;
        private  IPOSItemService _pOSItemService;
        private IBarTableService _barTableService;
        private int _AdminID = 2;

        protected override void Dispose(bool disposing)
        {
            if (disposing && _barTableService != null)
            {
                _barTableService.Dispose();
                _barTableService = null;
            }

            if (disposing && _pOSItemService != null)
            {
                _pOSItemService.Dispose();
                _pOSItemService = null;
            }

            if (disposing && _roomService != null)
            {
                _roomService.Dispose();
                _roomService = null;
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

            if (disposing && _guestReservationService != null)
            {
                _guestReservationService.Dispose();
                _guestReservationService = null;
            }

            if (disposing && _guestRoomService != null)
            {
                _guestRoomService.Dispose();
                _guestRoomService = null;
            }

            if (disposing && _guestRequestItemService != null)
            {
                _guestRequestItemService.Dispose();
                _guestRequestItemService = null;
            }

            if (disposing && _tableItemService != null)
            {
                _tableItemService.Dispose();
                _tableItemService = null;
            }

            base.Dispose(disposing);
        }

        private int? _hotelId;
        private int HotelID
        {
            get { return _hotelId ?? 1; }
            set { _hotelId = value; }
        }

        //private int GetHotelId()
        //{
        //    var username = User.Identity.Name;
        //    var user = _personService.GetAllForLogin().FirstOrDefault(x => x.Username.Equals(username, StringComparison.CurrentCultureIgnoreCase));
        //    return user.HotelId;
        //}


        private Person _person;
        private Person Person
        {
            get { return _person ?? GetPerson(); }
            set { _person = value; }
        }

        private Person GetPerson()
        {
            var username = User.Identity.Name;
            var user = _personService.GetAllForLogin().FirstOrDefault(x => !string.IsNullOrEmpty(x.IdNumber) && x.Username.Equals(username, StringComparison.CurrentCultureIgnoreCase));
            return user;
        }

        public HomeController()
        {
            _personService = new PersonService();
            _roomService = new RoomService();
            _guestService = new GuestService();
            _guestReservationService = new GuestReservationService();
            _guestRoomService = new GuestRoomService();
            _guestRequestItemService = new GuestRequestItemService();
            _tableItemService = new TableItemService();
            _pOSItemService = new POSItemService();
            _barTableService = new BarTableService();
        }

        [ChildActionOnly]
        public ActionResult TopMenuMusic()
        {
            if (Request.IsAuthenticated)
            {
                var model = new RoomBookingViewModel { };
                return PartialView("_NavigationMusicNew", model);
            }
            else
            {
                var model = new RoomBookingViewModel { };
                return PartialView("_NavigationMusicNew", model);
            }
        }

        [ChildActionOnly]
        public ActionResult TopMenu()
        {
            
            if (Request.IsAuthenticated)
            {
                var model = new RoomBookingViewModel {};

                var personId = Person.PersonID;

                var guestGroupByTablesAll = _guestRequestItemService.GetAllInclude("GuestOrder").Where(x => x.IsActive && x.RequestBy == Person.Email).GroupBy(x => x.GuestOrder).Select(x => new GuestOrderModel { Id = x.Key.Id, ActualGuest = x.Key.Guest, Items = x.ToList(), ActualGuestOrder = x.Key }).ToList();

                model.PendingRequestOrders = guestGroupByTablesAll.Count;

                model.PendingCollections = _tableItemService.GetAll().Where(x => x.DateSold > GetStartDateTime() && x.Completed && !x.Collected && x.CompletedTime != null).GroupBy(x => x.TableId).Count();

            
                return PartialView("_NavigationNew", model);
            }
            else
            {
                var model = new RoomBookingViewModel { };
                return PartialView("_NavigationNew", model);
            }
        }

        private DateTime GetStartDateTime()
        {
            DateTime now = DateTime.Now;
            DateTime returnDate = DateTime.Today;
            DateTime yesterDay = DateTime.Today.AddDays(-1);

            if (now.Hour >= 0 && now.Hour < 8 && now.Second > 0)
            {
                var yesterdaysStartDate = new DateTime(yesterDay.Year, yesterDay.Month, yesterDay.Day, 8, 1, 1);
                returnDate = yesterdaysStartDate;
            }
            else
            {
                var todaysStartDate = new DateTime(now.Year, now.Month, now.Day, 8, 1, 1);
                returnDate = todaysStartDate;
            }

            return returnDate;
        }

        public ActionResult VideoJS()
        {
            return View();
        }

        public ActionResult OceansClip(int? id, string type)
        {

            FileInfo oceansClipInfo = null;
            string oceansClipMimeType = String.Format("videos/{0}", type);

            switch (type)
            {
                case "mp4":
                    oceansClipInfo = new FileInfo(Server.MapPath("~/Content/videos/The.Theory.of.Everything.2014.720p.BluRay.x264.YIFY.mp4"));
                    break;
                case "webm":
                    oceansClipInfo = new FileInfo(Server.MapPath("~/Content/video/oceans-clip.webm"));
                    break;
                case "ogg":
                    oceansClipInfo = new FileInfo(Server.MapPath("~/Content/video/oceans-clip.ogv"));
                    break;
            }

            return new RangeFilePathResult(oceansClipMimeType, oceansClipInfo.FullName, oceansClipInfo.LastWriteTimeUtc, oceansClipInfo.Length);
            //return new RangeFileStreamResult(oceansClipInfo.OpenRead(), oceansClipMimeType, oceansClipInfo.Name, oceansClipInfo.LastWriteTimeUtc);
        }

        [HttpPost]
        public ActionResult CheckAvailability(DateTime? arrived, DateTime? departed, int? room_select)
        {
            IEnumerable<GuestReservation> gr = _roomService.GetAll(HotelID).SelectMany(x => x.GuestReservations).ToList();

            if(!departed.HasValue)
            {
                departed = arrived.Value.AddDays(1);
            }

            var conflicts = gr.SelectAvailable(arrived.Value, departed.Value, room_select.Value).ToList();

                //RoomsMatrixList = allRooms.ToList()

            if (conflicts.Count > 0)
            {
                var ids = conflicts.Select(x => x.RoomId).ToList();
                var RoomsList = _roomService.GetAll(HotelID).Where(x => (x.StatusId == (int)RoomStatusEnum.Vacant || x.StatusId == (int)RoomStatusEnum.Dirty) && !ids.Contains(x.Id)).ToList();
                var model = new RoomBookingViewModel { CheckinDate = arrived, CheckoutDate = departed, RoomsList = RoomsList, RoomsMatrixList = RoomsList    };
                return View("Booking", model); 
            }
            else
            {
                var roomLst = _roomService.GetAll(HotelID).ToList();

                if(room_select.HasValue && room_select > 0)
                {
                    roomLst = roomLst.Where(x => x.RoomType == room_select).ToList();
                }

                var model = new RoomBookingViewModel { CheckinDate = arrived, CheckoutDate = departed, RoomsList = roomLst, RoomsMatrixList = roomLst };

                return View("Booking", model); 
            }
        }


        [HttpPost]
        public ActionResult CheckAvailabilityFuture(DateTime? arrived, DateTime? departed, int? room_select)
        {
            IEnumerable<GuestReservation> gr = _roomService.GetAll(HotelID).SelectMany(x => x.GuestReservations).ToList();

            var conflicts = gr.SelectAvailable(arrived.Value, departed.Value, room_select.Value).ToList();

            if (conflicts.Count > 0)
            {
                var ids = conflicts.Select(x => x.RoomId).ToList();
                var model = new RoomBookingViewModel { CheckinDate = arrived, CheckoutDate = departed, RoomsList = _roomService.GetAll(HotelID).Where(x => (x.StatusId == (int)RoomStatusEnum.Vacant || x.StatusId == (int)RoomStatusEnum.Dirty) && !ids.Contains(x.Id)).ToList() };
                return View("NewFutureBooking", model);
            }
            else
            {
                var roomLst = _roomService.GetAll(HotelID).ToList();

                if (room_select.HasValue && room_select > 0)
                {
                    roomLst = roomLst.Where(x => x.RoomType == room_select).ToList();
                }

                var model = new RoomBookingViewModel { CheckinDate = arrived, CheckoutDate = departed, RoomsList = roomLst };

                return View("NewFutureBooking", model);
            }
        }


        [HttpPost]
        public ActionResult CheckAvailabilityGroupBooking(DateTime? arrived, DateTime? departed, int? room_select)
        {
            IEnumerable<GuestReservation> gr = _roomService.GetAll(HotelID).SelectMany(x => x.GuestReservations).ToList();

            var conflicts = gr.SelectAvailable(arrived.Value, departed.Value, room_select.Value).ToList();

            if (conflicts.Count > 0)
            {
                var ids = conflicts.Select(x => x.RoomId).ToList();
                var model = new RoomBookingViewModel { CheckinDate = arrived, CheckoutDate = departed, RoomsList = _roomService.GetAll(HotelID).Where(x => (x.StatusId == (int)RoomStatusEnum.Vacant || x.StatusId == (int)RoomStatusEnum.Dirty) && !ids.Contains(x.Id)).ToList() };
                return View("NewFutureBooking", model);
            }
            else
            {
                var roomLst = _roomService.GetAll(HotelID).ToList();

                if (room_select.HasValue && room_select > 0)
                {
                    roomLst = roomLst.Where(x => x.RoomType == room_select).ToList();
                }

                var model = new RoomBookingViewModel { CheckinDate = arrived, CheckoutDate = departed, RoomsList = roomLst };

                return View("NewFutureBooking", model);
            }

        }


        //
        [HttpGet]
        //[OutputCache(Duration = 3600, VaryByParam = "id")]
        public ActionResult NoRemoveableRooms(int? id)
        {
            var groupBookingList = _guestService.GetAll(HotelID).Where(x => x.IsActive == true && x.GuestRooms.Any(y => y.GroupBooking) && x.Id == id.Value).ToList();

            var model = new RoomBookingViewModel
            {
                CheckinDate = DateTime.Now,
                CheckoutDate = DateTime.Now.AddDays(1),
                GuestList = groupBookingList,
                NoRemoveableRooms = true
            };

            return View("AmendGroupBooking", model);
        }


        [HttpGet]
        //[OutputCache(Duration = 3600, VaryByParam = "none")]
        public ActionResult AmendGroupBooking()
        {
            var groupBookingList = _guestService.GetAll(HotelID).Where(x => x.IsActive == true && x.GuestRooms.Any(y => y.GroupBooking)).ToList();
            var model = new RoomBookingViewModel { CheckinDate = DateTime.Now, CheckoutDate = DateTime.Now.AddDays(1),
                                                   GuestList = groupBookingList
            };

            return View(model); 
        }

        public ActionResult Index2()
        {
            return View();
        }

        public ActionResult Index1()
        {
            return View();
        }

        private string GetHotelsName()
        {
            //
            var hotelName = string.Empty;

            try
            {
                hotelName = ConfigurationManager.AppSettings["HotelName"].ToString();
            }
            catch
            {
                hotelName = "";
            }

            return hotelName;
        }


        public ActionResult IndexMusicVideo(bool? loginFailed)
        {
            var expiry = new DateTime(2016, 12, 31);
            var hotelName = GetHotelsName();

            return View(new BaseViewModel { HotelName = hotelName, ExpiryDate = expiry, FutureReservationCount = 0, LoginFailed = loginFailed, CheckinDate = DateTime.Today.Date, CheckoutDate = DateTime.Today.AddDays(1).Date });
         
        }
        
        //public string GetStringForSSID(Wlan.Dot11Ssid ssid)
        //{
        //    return Encoding.ASCII.GetString(ssid.SSID, 0, (int)ssid.SSIDLength);
        //}

       


        public ActionResult IndexStart()
        {
            var hotelName = "";
            var expiry = DateTime.Now;
            return View("Index", new BaseViewModel { HotelName = hotelName, LoginFailed = true, ExpiryDate = expiry, FutureReservationCount = 0, CheckinDate = DateTime.Today.Date, CheckoutDate = DateTime.Today.AddDays(1).Date });
        }

        private static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["Core"].ConnectionString;
        }

        private string ActualMacAdress()
        {
            string actualAddress = string.Empty;

            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("GetEventPics", myConnection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    myConnection.Open();

                    try
                    {
                        actualAddress = cmd.ExecuteScalar().ToString();
                    }
                    catch
                    {
                    }
                }
            }

            return actualAddress;
        }

        private bool GetMacAddress(out string strAuth)
        {
            var actualMacadress = ActualMacAdress().ToUpper().Trim();

            strAuth = actualMacadress;

            return actualMacadress == "YES";
        }

        public ActionResult MyHome(bool? iWantToLogin)
        {
            //SendEmail( new Guest());

            var expiry = new DateTime(2016, 12, 31);


            string strAuth = string.Empty;

            var authentic = GetMacAddress(out strAuth);

            var hotelName = string.Empty;

            if (!authentic)
            {
                return View("AuthenticationFailed", new BaseViewModel { StrAuth = strAuth });
            }


            if (Request.IsAuthenticated)
            {
                if (User.IsInRole("TELEVISION"))
                {
                    int p = 0;

                    var tableItems = _tableItemService.GetAll("StockItem,BarTable").Where(x => x.StockItem.StarBuy)
                        .OrderByDescending(x => x.DateSold).Select(x => new StarBuyModel { index = p++, PicturePath = x.StockItem.PicturePath, RealStockItemString = x.StockItem.StockItemName, RealTableString = x.BarTable.TableAlias }).ToList();

                    var starBuys = POSService.StockItemService.GetStockItems(1).Where(x => x.StarBuy).Select(x => new StarBuyModel { index = p++, PicturePath = x.PicturePath, RealStockItemString = x.StockItemName, RealTableString = "NGN " + x.UnitPrice.ToString() }).ToList();

                    tableItems.AddRange(starBuys);

                    tableItems = tableItems.OrderBy(x => x.index).ToList();

                    return View("IndexTelevisionNew", new BaseViewModel { StarBuysTableItems = starBuys, Kitchenlist = null, HotelName = hotelName, ExpiryDate = expiry, FutureReservationCount = 0, CheckinDate = DateTime.Today.Date, CheckoutDate = DateTime.Today.AddDays(1).Date });
                }
                else if (User.IsInRole("KITCHEN"))
                {
                    TableItemService tis = new TableItemService();
                    var kitchenlist = tis.GetAll("BarTable,StockItem,BarTable.Person,BarTable.Person.DistributionPoint")
                        .Where(x => x.BarTable.StaffId != _AdminID && x.DateSold > GetStartDateTime() && !x.Fulfilled && x.StockItem.CookedFood).OrderByDescending(x => x.DateSold).ToList().GroupBy(x => x.BarTable)
                        .Select(x => new KitchenModel { Note = x.ToList().FirstOrDefault().Note, List = x.ToList(), ValueIds = x.ToList().Select(y => y.Id.ToString()).ToDelimitedString(","), BarTab = x.Key }).ToList();
                    return View("IndexKitchenNew", new BaseViewModel { Kitchenlist = kitchenlist, HotelName = hotelName, ExpiryDate = expiry, FutureReservationCount = 0, CheckinDate = DateTime.Today.Date, CheckoutDate = DateTime.Today.AddDays(1).Date });
                }
                else if (User.IsInRole("MANAGER"))
                {
                    return View("IndexManager", new BaseViewModel { HotelName = hotelName, ExpiryDate = expiry, FutureReservationCount = 0, CheckinDate = DateTime.Today.Date, CheckoutDate = DateTime.Today.AddDays(1).Date });
                }
                else if (User.IsInRole("STORE"))
                {
                    TableItemService tis = new TableItemService();
                    var notify = _pOSItemService.GetAllInclude("StockItem").Where(x => !x.StockItem.CookedFood && x.Remaining <= x.StockItem.NotNumber).Any();
                    var kitchenlist = tis.GetAll("BarTable,StockItem,BarTable.Person,BarTable.Person.DistributionPoint")
                        .Where(x => x.BarTable.StaffId == _AdminID && x.DateSold > GetStartDateTime() && !x.StoreFulfilled && !x.StockItem.CookedFood).OrderByDescending(x => x.DateSold).ToList()
                        .GroupBy(x => x.BarTable).Select(x => new KitchenModel { Note = x.ToList().FirstOrDefault().Note, List = x.ToList(), ValueIds = x.ToList().Select(y => y.Id.ToString()).ToDelimitedString(","), BarTab = x.Key }).ToList();
                    return View("IndexStoreNew", new BaseViewModel { Notify = notify, Storelist = kitchenlist, HotelName = hotelName, ExpiryDate = expiry, FutureReservationCount = 0, CheckinDate = DateTime.Today.Date, CheckoutDate = DateTime.Today.AddDays(1).Date });
                }
                else if (User.IsInRole("SALES ASSISTANT"))
                {
                    return View("IndexWaitress", new BaseViewModel { HotelName = hotelName, ExpiryDate = expiry, FutureReservationCount = 0, CheckinDate = DateTime.Today.Date, CheckoutDate = DateTime.Today.AddDays(1).Date });
                }
                else if (User.IsInRole("ADMIN"))
                {
                    IGuestFeedBackService gfs = new GuestFeedBackService();
                    var allFeedback = gfs.GetAll().Where(x => x.DateCreated > DateTime.Now.AddMonths(-1)).ToList();
                    return View(new BaseViewModel { Feedbacks = allFeedback, HotelName = hotelName, ExpiryDate = expiry, FutureReservationCount = 0, CheckinDate = DateTime.Today.Date, CheckoutDate = DateTime.Today.AddDays(1).Date });
                }
                else if (User.IsInRole("CASHIER") || User.IsInRole("BARTENDER"))
                {
                    hotelName = GetHotelsName();
                    return View("IndexCashier", new BaseViewModel { HotelName = hotelName, ExpiryDate = expiry, FutureReservationCount = 0, CheckinDate = DateTime.Today.Date, CheckoutDate = DateTime.Today.AddDays(1).Date });
                }
                else if (User.IsInRole("ROOM SERVICE"))
                {
                    return View("IndexRoomService", new BaseViewModel { HotelName = hotelName, ExpiryDate = expiry, FutureReservationCount = 0, CheckinDate = DateTime.Today.Date, CheckoutDate = DateTime.Today.AddDays(1).Date });
                }
                else
                {
                    hotelName = GetHotelsName();

                    var isCheckedInGuest = false;
                    var isRestaturantGuest = false;

                    try
                    {
                        isCheckedInGuest = Person.Guests.FirstOrDefault().GuestRooms.Count > 0;
                    }
                    catch
                    {
                    }

                    try
                    {
                        isRestaturantGuest = Person.Guests.FirstOrDefault().GuestOrders.Where(x => x.IsActive).Count() > 0;
                    }
                    catch
                    {
                    }

                    return View(new BaseViewModel { IsRestaturantGuest = isRestaturantGuest, IsCheckedInGuest = isCheckedInGuest, HotelName = hotelName, ExpiryDate = expiry, FutureReservationCount = 0, CheckinDate = DateTime.Today.Date, CheckoutDate = DateTime.Today.AddDays(1).Date });
                }

            }
            else
            {
                if (IsSelfServiceCentre() && (!iWantToLogin.HasValue))
                {
                    return RedirectToAction("SelfService", "Account");
                }
                else
                {
                    return RedirectToAction("NewLogin", "Account");
                }

            }
        }

        public ActionResult MyHomeOld()
        {
            if (User.IsInRole("SALES ASSISTANT"))
            {
                var hotelName = "";
                var expiry = DateTime.Now.AddYears(5);
                return View("IndexWaitress", new BaseViewModel { HotelName = hotelName, ExpiryDate = expiry, FutureReservationCount = 0, CheckinDate = DateTime.Today.Date, CheckoutDate = DateTime.Today.AddDays(1).Date });
            }
            else if (User.IsInRole("TELEVISION"))
            {
                int p = 0;

                var expiry = new DateTime(2015, 11, 30);

                var hotelName = string.Empty;

                var tableItems = _tableItemService.GetAll("StockItem,BarTable").Where(x => x.StockItem.StarBuy)
                    .OrderByDescending(x => x.DateSold).Select(x => new StarBuyModel { index = p++, PicturePath = x.StockItem.PicturePath, RealStockItemString = x.StockItem.StockItemName, RealTableString = x.BarTable.TableAlias }).ToList();

                var starBuys = POSService.StockItemService.GetStockItems(1).Where(x => x.StarBuy).Select(x => new StarBuyModel { index = p++, PicturePath = x.PicturePath, RealStockItemString = x.StockItemName, RealTableString = x.UnitPrice.ToString() }).ToList();

                tableItems.AddRange(starBuys);

                return View("IndexTelevision", new BaseViewModel { StarBuysTableItems = starBuys, Kitchenlist = null, HotelName = hotelName, ExpiryDate = expiry, FutureReservationCount = 0, CheckinDate = DateTime.Today.Date, CheckoutDate = DateTime.Today.AddDays(1).Date });

            }
            else
            {
                var expiry = new DateTime(2016, 12, 31);

                var hotelName = GetHotelsName();

                IGuestFeedBackService gfs = new GuestFeedBackService();

                var allFeedback = gfs.GetAll().Where(x => x.DateCreated > DateTime.Now.AddMonths(-1)).ToList();

                var isCheckedInGuest = false;

                var isRestaturantGuest = false;

                try
                {
                    isCheckedInGuest = Person.Guests.FirstOrDefault().GuestRooms.Count > 0;
                }
                catch
                {
                }

                try
                {
                    isRestaturantGuest = Person.Guests.FirstOrDefault().GuestOrders.Where(x => x.IsActive).Count() > 0;
                }
                catch
                {
                }

                return View("Index", new BaseViewModel { Feedbacks = allFeedback, IsRestaturantGuest = isRestaturantGuest, IsCheckedInGuest = isCheckedInGuest, HotelName = hotelName, ExpiryDate = expiry, FutureReservationCount = 0, CheckinDate = DateTime.Today.Date, CheckoutDate = DateTime.Today.AddDays(1).Date });

            }
        }

        public ActionResult TelevisionIndex()
        {
           int p = 0;

           var expiry = new DateTime(2015, 11, 30);

           var hotelName = string.Empty;

            var tableItems = _tableItemService.GetAll("StockItem,BarTable").Where(x => x.StockItem.StarBuy)
                .OrderByDescending(x => x.DateSold).Select(x => new StarBuyModel { index = p++, PicturePath = x.StockItem.PicturePath, RealStockItemString = x.StockItem.StockItemName, RealTableString = x.BarTable.TableAlias }).ToList();

            var starBuys = POSService.StockItemService.GetStockItems(1).Where(x => x.StarBuy).Select(x => new StarBuyModel { index = p++, PicturePath = x.PicturePath, RealStockItemString = x.StockItemName, RealTableString = x.UnitPrice.ToString() }).ToList();

            tableItems.AddRange(starBuys);

            return View("IndexTelevision", new BaseViewModel { StarBuysTableItems = starBuys, Kitchenlist = null, HotelName = hotelName, ExpiryDate = expiry, FutureReservationCount = 0, CheckinDate = DateTime.Today.Date, CheckoutDate = DateTime.Today.AddDays(1).Date });
      
        }
       
        public ActionResult Index(bool? loginFailed, string auth, bool? iWantToLogin)
        {
            var expiry = new DateTime(2017, 11, 30);

            if(DateTime.Now > expiry)
            {
                throw new Exception("License has expired");
            }

            string strAuth = string.Empty;

            var authentic = GetMacAddress(out strAuth);

            var hotelName = string.Empty;

            if (!authentic)
            {
                return View("AuthenticationFailed", new BaseViewModel { StrAuth = strAuth });
            }


            if (Request.IsAuthenticated)
            {
                if (User.IsInRole("TELEVISION"))
                {
                    int p = 0;

                    var tableItems = _tableItemService.GetAll("StockItem,BarTable").Where(x => x.StockItem.StarBuy)
                        .OrderByDescending(x => x.DateSold).Select(x => new StarBuyModel { index = p++, PicturePath = x.StockItem.PicturePath, RealStockItemString = x.StockItem.StockItemName, RealTableString = x.BarTable.TableAlias }).ToList();

                    var starBuys = POSService.StockItemService.GetStockItems(1).Where(x => x.StarBuy).Select(x => new StarBuyModel { index = p++, PicturePath = x.PicturePath, RealStockItemString = x.StockItemName, RealTableString = "NGN " +  x.UnitPrice.ToString() }).ToList();

                    tableItems.AddRange(starBuys);

                    tableItems = tableItems.OrderBy(x => x.index).ToList();

                    return View("IndexTelevisionNew", new BaseViewModel { StarBuysTableItems = starBuys, Kitchenlist = null, HotelName = hotelName, ExpiryDate = expiry, FutureReservationCount = 0, CheckinDate = DateTime.Today.Date, CheckoutDate = DateTime.Today.AddDays(1).Date });
                }
                else if (User.IsInRole("KITCHEN"))
                {
                    TableItemService tis = new TableItemService();
                    var kitchenlist = tis.GetAll("BarTable,StockItem,BarTable.Person,BarTable.Person.DistributionPoint")
                        .Where(x => x.BarTable.StaffId != _AdminID && x.DateSold > GetStartDateTime() && !x.Fulfilled && x.StockItem.CookedFood).OrderByDescending(x => x.DateSold).ToList().GroupBy(x => x.BarTable)
                        .Select(x => new KitchenModel { Note = x.ToList().FirstOrDefault().Note, List = x.ToList(), ValueIds = x.ToList().Select(y => y.Id.ToString()).ToDelimitedString(","), BarTab = x.Key }).ToList();
                    return View("IndexKitchenNew", new BaseViewModel { Kitchenlist = kitchenlist, HotelName = hotelName, ExpiryDate = expiry, FutureReservationCount = 0, CheckinDate = DateTime.Today.Date, CheckoutDate = DateTime.Today.AddDays(1).Date });
                }
                else if (User.IsInRole("MANAGER"))
                {
                    var openTables = _barTableService.GetAll().Where(x => x.IsActive).OrderByDescending(x => x.CreatedDate).ToList();
                    var allOpenTables = openTables;
                    return View("IndexManager", new BaseViewModel { AllOpenTables = allOpenTables, HotelName = hotelName, ExpiryDate = expiry, FutureReservationCount = 0, CheckinDate = DateTime.Today.Date, CheckoutDate = DateTime.Today.AddDays(1).Date });
                }
                else if (User.IsInRole("STORE"))
                {
                    TableItemService tis = new TableItemService();
                    var notify = _pOSItemService.GetAllInclude("StockItem").Where(x => !x.StockItem.CookedFood && x.Remaining <= x.StockItem.NotNumber).Any();
                    var kitchenlist = tis.GetAll("BarTable,StockItem,BarTable.Person,BarTable.Person.DistributionPoint")
                        .Where(x => x.BarTable.StaffId == _AdminID && x.DateSold > GetStartDateTime() && !x.StoreFulfilled && !x.StockItem.CookedFood).OrderByDescending(x => x.DateSold).ToList()
                        .GroupBy(x => x.BarTable).Select(x => new KitchenModel { Note = x.ToList().FirstOrDefault().Note, List = x.ToList(), ValueIds = x.ToList().Select(y => y.Id.ToString()).ToDelimitedString(","), BarTab = x.Key }).ToList();
                    return View("IndexStoreNew", new BaseViewModel { Notify = notify, Storelist = kitchenlist, HotelName = hotelName, ExpiryDate = expiry, FutureReservationCount = 0, CheckinDate = DateTime.Today.Date, CheckoutDate = DateTime.Today.AddDays(1).Date });
                }
                else if (User.IsInRole("SALES ASSISTANT"))
                {
                    var openTables = _barTableService.GetAll().Where(x => x.IsActive && x.StaffId == Person.PersonID).OrderByDescending(x => x.CreatedDate).ToList();
                    var allOpenTables = openTables;
                    return View("IndexWaitressNew", new BaseViewModel {AllOpenTables = allOpenTables, HotelName = hotelName, ExpiryDate = expiry, FutureReservationCount = 0, CheckinDate = DateTime.Today.Date, CheckoutDate = DateTime.Today.AddDays(1).Date });
                }
                else if (User.IsInRole("ADMIN"))
                {
                    IGuestFeedBackService gfs = new GuestFeedBackService();
                    var allFeedback = gfs.GetAll().Where(x => x.DateCreated > DateTime.Now.AddMonths(-1)).ToList();
                     return View(new BaseViewModel { Feedbacks = allFeedback, HotelName = hotelName, ExpiryDate = expiry, FutureReservationCount = 0, CheckinDate = DateTime.Today.Date, CheckoutDate = DateTime.Today.AddDays(1).Date });
                }
                else if (User.IsInRole("CASHIER") || User.IsInRole("BARTENDER"))
                {
                    return RedirectToAction("Index", "POS");
                    //hotelName = GetHotelsName();
                    //return View("IndexCashier", new BaseViewModel { HotelName = hotelName, ExpiryDate = expiry, FutureReservationCount = 0, CheckinDate = DateTime.Today.Date, CheckoutDate = DateTime.Today.AddDays(1).Date });
                }
                else if (User.IsInRole("ROOM SERVICE"))
                {
                    return View("IndexRoomService", new BaseViewModel { HotelName = hotelName, ExpiryDate = expiry, FutureReservationCount = 0, CheckinDate = DateTime.Today.Date, CheckoutDate = DateTime.Today.AddDays(1).Date });
                }
                else
                {
                    hotelName = GetHotelsName();

                    var isCheckedInGuest = false;

                    var isRestaturantGuest = false;

                    try
                    {
                        isCheckedInGuest = Person.Guests.FirstOrDefault().GuestRooms.Count > 0;
                    }
                    catch
                    {
                    }

                    try
                    {
                        isRestaturantGuest = Person.Guests.FirstOrDefault().GuestOrders.Where(x => x.IsActive).Count() > 0;
                    }
                    catch
                    {
                    }

                    

                    return View(new BaseViewModel { IsRestaturantGuest = isRestaturantGuest, IsCheckedInGuest = isCheckedInGuest, HotelName = hotelName, ExpiryDate = expiry, FutureReservationCount = 0, CheckinDate = DateTime.Today.Date, CheckoutDate = DateTime.Today.AddDays(1).Date });
                }

            }
            else
            {

                if (IsSelfServiceCentre() && (!iWantToLogin.HasValue) )
                {
                    return RedirectToAction("SelfService", "Account");
                }
                else
                {
                    return RedirectToAction("NewLogin", "Account");
                }
                
            }
        }


        public ActionResult GetWaitressAlerts()
        {
            KitchenInfoRepository objRepo = new KitchenInfoRepository();
            var info = objRepo.GetAlertData();
            var _guestMessageService = new GuestMessageService();
            var todaysMessages = _guestMessageService.GetAll().Where(x => DateTime.Today.ToShortDateString() == x.MessageDate.ToShortDateString()).OrderByDescending(x => x.MessageDate).ToList();
            return PartialView("_CollectionAlertsPartial", new BaseViewModel { TodaysMessages = todaysMessages });
        }


        public ActionResult GetWaitressOrders()
        {
            KitchenInfoRepository objRepo = new KitchenInfoRepository();
            var info = objRepo.GetData();
            TableItemService tis = new TableItemService();
            var kitchenlist = tis.GetAll("BarTable,StockItem,BarTable.Person,BarTable.Person.DistributionPoint").Where(x => x.DateSold > GetStartDateTime() && x.Completed && !x.Collected).OrderByDescending(x => x.DateSold).ToList().GroupBy(x => x.BarTable).Select(x => new KitchenModel { List = x.ToList(), ValueIds = x.ToList().Select(y => y.Id.ToString()).ToDelimitedString(","), BarTab = x.Key }).ToList();
            return PartialView("_CollectionPartial", new BaseViewModel { Kitchenlist = kitchenlist });
        }


        public ActionResult GetTelevisionOrders()
        {
            int p = 0;

            KitchenInfoRepository objRepo = new KitchenInfoRepository();

            var info = objRepo.GetData();

            var tableItems = _tableItemService.GetAll("StockItem,BarTable").Where(x => x.StockItem.StarBuy)
                        .OrderByDescending(x => x.DateSold).Select(x => new StarBuyModel { index = p++, PicturePath = x.StockItem.PicturePath, RealStockItemString = x.StockItem.StockItemName, RealTableString = x.BarTable.TableAlias }).ToList();

            var starBuys = POSService.StockItemService.GetStockItems(1).Where(x => x.StarBuy).Select(x => new StarBuyModel { index = p++, PicturePath = x.PicturePath, RealStockItemString = x.StockItemName, RealTableString = "NGN " + x.UnitPrice.ToString() }).ToList();

            tableItems.AddRange(starBuys);

            tableItems = tableItems.OrderBy(x => x.index).ToList();

            return PartialView("_TelevisionSlider", new BaseViewModel { StarBuysTableItems = tableItems });
        }



        public ActionResult GetStoreOrders()
        {
            KitchenInfoRepository objRepo = new KitchenInfoRepository();

            var info = objRepo.GetData();

            TableItemService tis = new TableItemService();
            var kitchenlist = tis.GetAll("BarTable,StockItem,BarTable.Person,BarTable.Person.DistributionPoint")
                .Where(x => x.BarTable.StaffId != _AdminID && x.DateSold > GetStartDateTime() && !x.Fulfilled && !x.StockItem.CookedFood).OrderByDescending(x => x.DateSold).ToList().GroupBy(x => x.BarTable)
                .Select(x => new KitchenModel { Note = x.ToList().FirstOrDefault().Note, List = x.ToList(), ValueIds = x.ToList().Select(y => y.Id.ToString()).ToDelimitedString(","), BarTab = x.Key }).ToList();
            return PartialView("_KitchenStorePartial", new BaseViewModel { Kitchenlist = kitchenlist });

        }


        public ActionResult GetPrinterOrdersRoomService()
        {
            KitchenInfoRepository objRepo = new KitchenInfoRepository();

            var info = objRepo.GetPrinterData();

            TableItemService tis = new TableItemService();

            var kitchenlistAll = tis.GetAll("BarTable,StockItem,BarTable.Person,BarTable.Person.DistributionPoint").Where(x => !x.Fulfilled).OrderByDescending(x => x.DateSold).ToList();

            var kitchenlistPrintOnly = kitchenlistAll.Where(x => !x.SentToPrinter && x.BarTable.StaffId == _AdminID).OrderByDescending(x => x.DateSold).ToList().GroupBy(x => x.BarTable)
                .Select(x => new KitchenModel { Note = x.ToList().FirstOrDefault().Note, List = x.ToList(), ValueIds = x.ToList().Select(y => y.Id.ToString()).ToDelimitedString(","), BarTab = x.Key }).ToList();

            foreach (var tb in kitchenlistPrintOnly)
            {
                var tablename = tb.BarTab.TableAlias;
                var cashierName = tb.BarTab.Person.DisplayName;
                var pointName = (tb.BarTab.Person.DistributionPoint != null) ? tb.BarTab.Person.DistributionPoint.Description : "UNKNOWN";
                var mmmm1 = tb.List.GroupBy(x => x.DateSold.ToShortTimeString()).Select(y => new BarAndRestaurantMate.Models.LatestGroupByModel { DatesoldStr = y.Key, Items = y.ToList(), Datesold = y.ToList().FirstOrDefault().DateSold, ValueIds = y.ToList().Select(z => z.Id.ToString()).ToDelimitedString(",") }).ToList();
                foreach (var rmm in mmmm1)
                {
                    var ids = rmm.ValueIds;

                    try
                    {
                        SendToOPOSPrinter(rmm.Items, tablename, cashierName, pointName);
                    }
                    catch
                    {

                    }

                    SaveAsPrinted(ids);
                }
            }

            return new JsonResult();

        }


        public ActionResult GetPrinterOrders()
        {
            KitchenInfoRepository objRepo = new KitchenInfoRepository();

            var info = objRepo.GetPrinterData();

            TableItemService tis = new TableItemService();

            var kitchenlistAll = tis.GetAll("BarTable,StockItem,BarTable.Person,BarTable.Person.DistributionPoint").Where(x => !x.Fulfilled).OrderByDescending(x => x.DateSold).ToList();

            var kitchenlistPrintOnly = kitchenlistAll.Where(x => !x.SentToPrinter && x.BarTable.StaffId != _AdminID && x.StockItem.CookedFood).OrderByDescending(x => x.DateSold).ToList().GroupBy(x => x.BarTable)
                .Select(x => new KitchenModel { Note = x.ToList().FirstOrDefault().Note, List = x.ToList(), ValueIds = x.ToList().Select(y => y.Id.ToString()).ToDelimitedString(","), BarTab = x.Key }).ToList();

            foreach (var tb in kitchenlistPrintOnly)
            {
                var tablename = tb.BarTab.TableAlias;
                var cashierName = tb.BarTab.Person.DisplayName;
                var pointName = (tb.BarTab.Person.DistributionPoint != null) ? tb.BarTab.Person.DistributionPoint.Description : "UNKNOWN";
                var mmmm1 = tb.List.GroupBy(x => x.DateSold.ToShortTimeString()).Select(y => new BarAndRestaurantMate.Models.LatestGroupByModel { DatesoldStr = y.Key, Items = y.ToList(), Datesold = y.ToList().FirstOrDefault().DateSold, ValueIds = y.ToList().Select(z => z.Id.ToString()).ToDelimitedString(",") }).ToList();
                foreach (var rmm in mmmm1)
                {
                    var ids = rmm.ValueIds;

                    try
                    {
                        SendToOPOSPrinter(rmm.Items, tablename, cashierName, pointName);
                    }
                    catch
                    {

                    }

                    SaveAsPrinted(ids);
                }
            }

            return new JsonResult();

        }

        
        public ActionResult GetRoomServiceOrdersPrevious()
        {
            KitchenInfoRepository objRepo = new KitchenInfoRepository();

            var info = objRepo.GetData();

            TableItemService tis = new TableItemService();

            var kitchenlistAll = tis.GetAllEvery("BarTable,StockItem,BarTable.Person,BarTable.Person.DistributionPoint").ToList();


            var countAllInActive = kitchenlistAll.OrderBy(x => x.Id).LastOrDefault();

            var ringAlarm = 0;

            if (countAllInActive != null && countAllInActive.IsActive)
                ringAlarm = 1;


            var kitchenlistPrintOnly = kitchenlistAll.Where(x => !x.SentToPrinter && x.BarTable.StaffId == _AdminID && x.DateSold > GetStartDateTime() && x.Fulfilled).OrderByDescending(x => x.DateSold).ToList().GroupBy(x => x.BarTable)
                .Select(x => new KitchenModel { Note = x.ToList().FirstOrDefault().Note, List = x.ToList(), ValueIds = x.ToList().Select(y => y.Id.ToString()).ToDelimitedString(","), BarTab = x.Key }).ToList();


            var kitchenlist = kitchenlistAll.Where(x => x.BarTable.StaffId == _AdminID && x.DateSold > GetStartDateTime() && x.Fulfilled).OrderByDescending(x => x.DateSold).ToList().GroupBy(x => x.BarTable)
                .Select(x => new KitchenModel { Note = x.ToList().FirstOrDefault().Note, List = x.ToList(), ValueIds = x.ToList().Select(y => y.Id.ToString()).ToDelimitedString(","), BarTab = x.Key }).ToList();


            string kitchenlistStr = RenderRazorViewToString("_KitchenRoomServicePartialFulfilled", new BaseViewModel { Kitchenlist = kitchenlist });

            return Json(new { RingAlarm = ringAlarm, kitchenlistStr }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetRoomServiceOrders()
        {
            KitchenInfoRepository objRepo = new KitchenInfoRepository();

            var info = objRepo.GetData();

            TableItemService tis = new TableItemService();

            var kitchenlistAll = tis.GetAllEvery("BarTable,StockItem,BarTable.Person,BarTable.Person.DistributionPoint").ToList();


            var countAllInActive = kitchenlistAll.OrderBy(x => x.Id).LastOrDefault();

            var ringAlarm = 0;

            if (countAllInActive != null && countAllInActive.IsActive)
                ringAlarm = 1;


            var kitchenlistPrintOnly = kitchenlistAll.Where(x => !x.SentToPrinter && x.BarTable.StaffId == _AdminID && x.DateSold > GetStartDateTime() && !x.Fulfilled).OrderByDescending(x => x.DateSold).ToList().GroupBy(x => x.BarTable)
                .Select(x => new KitchenModel { Note = x.ToList().FirstOrDefault().Note, List = x.ToList(), ValueIds = x.ToList().Select(y => y.Id.ToString()).ToDelimitedString(","), BarTab = x.Key }).ToList();


            var kitchenlist = kitchenlistAll.Where(x => x.BarTable.StaffId == _AdminID && x.DateSold > GetStartDateTime() && !x.Fulfilled).OrderByDescending(x => x.DateSold).ToList().GroupBy(x => x.BarTable)
                .Select(x => new KitchenModel { Note = x.ToList().FirstOrDefault().Note, List = x.ToList(), ValueIds = x.ToList().Select(y => y.Id.ToString()).ToDelimitedString(","), BarTab = x.Key }).ToList();


            string kitchenlistStr = RenderRazorViewToString("_KitchenRoomServicePartial", new BaseViewModel { Kitchenlist = kitchenlist });

            return Json(new { RingAlarm = ringAlarm, kitchenlistStr }, JsonRequestBehavior.AllowGet);

        }



        public ActionResult GetKitchenOrders()
        {
            KitchenInfoRepository objRepo = new KitchenInfoRepository();

            var info = objRepo.GetData();

            TableItemService tis = new TableItemService();

            var kitchenlistAll = tis.GetAllEvery("BarTable,StockItem,BarTable.Person,BarTable.Person.DistributionPoint").ToList();


            var countAllInActive = kitchenlistAll.OrderBy(x => x.Id).LastOrDefault();

            var ringAlarm = 0;

            if (countAllInActive != null && countAllInActive.IsActive)
                ringAlarm = 1;


            var kitchenlistPrintOnly = kitchenlistAll.Where(x => !x.SentToPrinter && x.BarTable.StaffId != _AdminID && x.DateSold > GetStartDateTime() && !x.Fulfilled && x.StockItem.CookedFood).OrderByDescending(x => x.DateSold).ToList().GroupBy(x => x.BarTable)
                .Select(x => new KitchenModel { Note = x.ToList().FirstOrDefault().Note, List = x.ToList(), ValueIds = x.ToList().Select(y => y.Id.ToString()).ToDelimitedString(","), BarTab = x.Key }).ToList();


            var kitchenlist = kitchenlistAll.Where(x => x.BarTable.StaffId != _AdminID && x.DateSold > GetStartDateTime() && !x.Fulfilled && x.StockItem.CookedFood).OrderByDescending(x => x.DateSold).ToList().GroupBy(x => x.BarTable)
                .Select(x => new KitchenModel { Note = x.ToList().FirstOrDefault().Note, List = x.ToList(), ValueIds = x.ToList().Select(y => y.Id.ToString()).ToDelimitedString(","), BarTab = x.Key }).ToList();


            string kitchenlistStr = RenderRazorViewToString("_KitchenPartial", new BaseViewModel { Kitchenlist = kitchenlist });

            //return PartialView("_KitchenPartial", new BaseViewModel { Kitchenlist = kitchenlist, RingAlarm = ringAlarm });

            return Json(new { RingAlarm = ringAlarm, kitchenlistStr }, JsonRequestBehavior.AllowGet);
       
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

        private void SaveAsPrinted(string valueIds)
        {
             if(string.IsNullOrEmpty(valueIds))
              return;
            

            var vals = valueIds.Split(',');

            foreach (var v in vals)
            {
                var ti = _tableItemService.GetById(int.Parse(v));

                if(ti != null)
                {
                    ti.SentToPrinter = true;
                    _tableItemService.Update(ti);
                }
            }

            return;
        }

        private void SendToOPOSPrinter(List<TableItem> list, string tablename, string cashierName, string pointName)
        {
            var note = string.Empty;

            if(list.Any() && !string.IsNullOrEmpty( list.FirstOrDefault().Note))
            {
                note = list.FirstOrDefault().Note;
            }

            List<POSService.Entities.StockItem> lst = new List<POSService.Entities.StockItem>();

            foreach(var ti in list)
            {
                lst.Add(new POSService.Entities.StockItem { Description = ti.StockItem.StockItemName, Quantity = ti.Qty });
            }

            PrintKitchenDocketRaw(lst, note, tablename, cashierName, pointName);

            //PrintKitchenDocket(lst, note, tablename, cashierName, pointName);
        }

        private void PrintTextRaw(string printer, string text)
        {

            int RecLineChars = 42;
            string eNmlText = Convert.ToChar(27) + "!" + Convert.ToChar(0);
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

        private void PrintKitchenDocketRaw(List<POSService.Entities.StockItem> lst, string note, string tableName, string cashierName, string pointName)
        {
            var printerName = ConfigurationManager.AppSettings["PrinterName"].ToString();
            var networkPrinterName = ConfigurationManager.AppSettings["NetworkPrinterName"].ToString();

            var grpList = lst.GroupBy(x => x.Id).Select(x => new PrintStockItemModel
            {
                Description = x.FirstOrDefault().Description,
                Quantity = x.Sum(z => z.Quantity)
            }).ToList();

            try
            {
                byte[] DrawerOpen5 = { 0xA };

                //char V = 'a';
                //byte[] DrawerOpen = { 0x1B, Convert.ToByte(V), 1 };
                //RawPrinterHelper.DoSomeThing(printerName, DrawerOpen);

                //V = '!';
                //byte[] DrawerOpen1 = { 0x1B, Convert.ToByte(V), 0 };
                //RawPrinterHelper.DoSomeThing(printerName, DrawerOpen1);


                //V = 'd';
                //byte[] DrawerOpen2 = { 0x1B, Convert.ToByte(V), 3 };
                //RawPrinterHelper.DoSomeThing(printerName, DrawerOpen2);

                //V = 'a';
                //byte[] DrawerOpen3 = { 0x1B, Convert.ToByte(V), 0 };
                //RawPrinterHelper.DoSomeThing(printerName, DrawerOpen3);

                //V = '!';
                //byte[] DrawerOpen4 = { 0x1B, Convert.ToByte(V), 1 };
                //RawPrinterHelper.DoSomeThing(printerName, DrawerOpen4);

                

                PrintTextLineRaw(printerName, "TIME ---" + DateTime.Now.ToString());
                //RawPrinterHelper.SendStringToPrinter(printerName, string.Empty);
                //RawPrinterHelper.DoSomeThing(printerName, DrawerOpen5); //LINE FEED


                PrintTextLineRaw(printerName, "TABLE ---" + tableName);
                //RawPrinterHelper.SendStringToPrinter(printerName, string.Empty);
                //RawPrinterHelper.DoSomeThing(printerName, DrawerOpen5); //LINE FEED

                PrintTextLineRaw(printerName, note);
                RawPrinterHelper.SendStringToPrinter(printerName, string.Empty);
                RawPrinterHelper.DoSomeThing(printerName, DrawerOpen5); //LINE FEED

                int RecLineChars = 42;

                PrintTextLineRaw(printerName, new string('=', RecLineChars));
                RawPrinterHelper.SendStringToPrinter(printerName, string.Empty);
                RawPrinterHelper.DoSomeThing(printerName, DrawerOpen5); //LINE FEED


                foreach (var item in grpList)
                {
                    string str = TruncateAt(item.Description.PadRight(31), 31) + TruncateAt(item.Quantity.ToString().PadLeft(3), 3);
                    PrintTextLineRaw(printerName, str);
                    RawPrinterHelper.DoSomeThing(printerName, DrawerOpen5); //LINE FEED
                }

                RawPrinterHelper.SendStringToPrinter(printerName, string.Empty);

                RawPrinterHelper.SendStringToPrinter(printerName, new string('=', RecLineChars));
                

                RawPrinterHelper.DoSomeThing(printerName, DrawerOpen5); //LINE FEED
                RawPrinterHelper.DoSomeThing(printerName, DrawerOpen5); //LINE FEED

                RawPrinterHelper.FullCut(printerName);

            }
            catch (Exception)
            {
                // MessageBox.Show(ex.Message);
            }

           
        }


        private void PrintKitchenDocket(List<POSService.Entities.StockItem> lst, string note, string tableName, string cashierName, string pointName)
        {
            PosPrinter printer = GetReceiptPrinter();

            try
            {
                ConnectToPrinter(printer);

                var thisUserName = User.Identity.Name;

                PrintReceiptHeaderKitchen(printer, tableName, DateTime.Now, cashierName, pointName);

                foreach (var item in lst)
                {
                    PrintLineItem(printer, item.Description, item.Quantity);
                }

                PrintReceiptFooter(printer, 0, 0, 0, note);
            }
            finally
            {
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
            PosExplorer posExplorer = null;

            try
            {

                posExplorer = new PosExplorer();
            }
            catch (Exception)
            {
            }

            DeviceInfo receiptPrinterDevice = posExplorer.GetDevice(DeviceType.PosPrinter, "POSPrinter"); //May need to change this if you don't use a logicial name or//my_device
            
            return (PosPrinter)posExplorer.CreateInstance(receiptPrinterDevice);
        }

        private void PrintReceiptHeaderKitchen(PosPrinter printer, string tableAlias, DateTime dateTime,
            string cashierName, string pointName)
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


        private void PrintReceiptFooter(PosPrinter printer, double subTotal, double tax, double discount, string footerText)
        {
            string offSetString = new string(' ', printer.RecLineChars / 2);

            PrintTextLine(printer, new string('-', (printer.RecLineChars / 3) * 2));
            //PrintTextLine(printer, offSetString + String.Format("SUB-TOTAL  {0}", subTotal.ToString("#0.00")));
            //PrintTextLine(printer, offSetString + String.Format("TAX        {0}", tax.ToString("#0.00")));
            //PrintTextLine(printer, offSetString + String.Format("DISCOUNT   {0}", discount.ToString("#0.00")));
            //PrintTextLine(printer, offSetString + new string('-', (printer.RecLineChars / 3)));
            //PrintTextLine(printer, offSetString + String.Format("TOTAL      {0}", (subTotal - (tax + discount)).ToString("#0.00")));
            //PrintTextLine(printer, offSetString + new string('-', (printer.RecLineChars / 3)));
            PrintTextLine(printer, String.Empty);

            //Embed 'center' alignment tag on front of string below to have it printed in the center of the receipt.
            PrintTextLine(printer, System.Text.ASCIIEncoding.ASCII.GetString(new byte[] { 27, (byte)'|', (byte)'c', (byte)'A' }) + footerText);

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

        private void PrintLineItem(PosPrinter printer, string strTableDetails)
        {

            PrintText(printer, TruncateAt(strTableDetails.PadRight(21), 21));
            PrintText(printer, TruncateAt(DateTime.Now.ToShortTimeString().PadLeft(9), 9));
            PrintTextLine(printer, String.Empty);
            PrintTextLine(printer, String.Empty);
            PrintTextLine(printer, new string('-', (printer.RecLineChars / 3) * 2));
        }

        private void PrintLineItem(PosPrinter printer, string itemCode, int quantity)
        {
            PrintText(printer, TruncateAt(itemCode.PadRight(31), 31));
            PrintTextLine(printer, TruncateAt(quantity.ToString("#0.00").PadLeft(9), 9));
            //PrintText(printer, TruncateAt(unitPrice.ToString("#0.00").PadLeft(10), 10));
            //PrintTextLine(printer, TruncateAt((quantity * unitPrice).ToString("#0.00").PadLeft(10), 10));
        }

        private void PrintReceiptHeader(PosPrinter printer, string companyName, string addressLine1, string addressLine2, string taxNumber, DateTime dateTime, string cashierName)
        {
            PrintTextLine(printer, companyName);
            PrintTextLine(printer, addressLine1);
            PrintTextLine(printer, addressLine2);
            PrintTextLine(printer, taxNumber);
            PrintTextLine(printer, new string('-', printer.RecLineChars / 2));
            PrintTextLine(printer, String.Format("DATE : {0}", dateTime.ToShortDateString()));
            PrintTextLine(printer, String.Format("CASHIER : {0}", cashierName));
            PrintTextLine(printer, String.Empty);
            PrintText(printer, "Item             ");
            PrintText(printer, "Qty  ");
            PrintText(printer, "Unit Price ");
            PrintTextLine(printer, "Total      ");
            PrintTextLine(printer, new string('=', printer.RecLineChars));
            PrintTextLine(printer, String.Empty);

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

        private bool IsSelfServiceCentre()
        { 
            var selfService = false;

            try
            {
                selfService = ConfigurationManager.AppSettings["IsSelfServiceCentre"].ToString() == "1";
            }
            catch
            {
                selfService = false;
            }

            return selfService;
        }

        //private bool ConnectToWifi()
        //{
        //    return true;

        //    WlanClient client = new WlanClient();

        //    bool connectedToWifi = false;

        //    foreach (WlanClient.WlanInterface wlanIface in client.Interfaces)
        //    {
        //        // Lists all networks with WEP security
        //        Wlan.WlanAvailableNetwork[] networks = wlanIface.GetAvailableNetworkList(0);

        //        //foreach (Wlan.WlanAvailableNetwork network in networks)
        //        //{

        //        //    var tt = GetStringForSSID(network.dot11Ssid);

        //        //    if(tt.ToUpper().StartsWith("IONITV"))
        //        //    {
        //        //        var strenght = network.wlanSignalQuality;
        //        //        var pp = 90;
        //        //    }


        //        //    //if (network.dot11DefaultCipherAlgorithm == Wlan.Dot11CipherAlgorithm.WEP)
        //        //    //{
        //        //    //    var tt = GetStringForSSID(network.dot11Ssid);
        //        //    //    //Console.WriteLine("Found WEP network with SSID {0}.", GetStringForSSID(network.dot11Ssid));
        //        //    //}


        //        //}

        //        // Retrieves XML configurations of existing profiles.
        //        // This can assist you in constructing your own XML configuration
        //        // (that is, it will give you an example to follow).
        //        var wifi = GetWifi();
        //        var actualXML = "";
        //        foreach (Wlan.WlanProfileInfo profileInfo in wlanIface.GetProfiles().Where(x => x.profileName.ToUpper() == wifi.ToUpper()))
        //        {
        //            string name = profileInfo.profileName; // this is typically the network's SSID
        //            string xml = wlanIface.GetProfileXml(profileInfo.profileName);
        //            actualXML = xml;
        //        }

        //        try
        //        {

        //            string profileName = GetWifi(); // this is also the SSID
        //            string profileXml = actualXML;
        //            wlanIface.SetProfile(Wlan.WlanProfileFlags.AllUser, profileXml, true);
        //            wlanIface.Connect(Wlan.WlanConnectionMode.Profile, Wlan.Dot11BssType.Any, profileName);
        //            connectedToWifi = true;
        //        }
        //        catch
        //        {
        //            connectedToWifi = false;
        //        }

        //    }

        //    return connectedToWifi;
        //}

        private string GetWifi()
        {
            //
            var wifi = string.Empty;

            try
            {
                wifi = ConfigurationManager.AppSettings["WIFI"].ToString();
            }
            catch
            {
                wifi = "";
            }

            return wifi;
        }

        //[OutputCache(Duration = 3600, VaryByParam = "arrive,depart,room_select")]
        public ActionResult FutureBooking(DateTime? arrive, DateTime? depart, int? room_select)
        {
            if (!arrive.HasValue) arrive = DateTime.Now;
            if (!depart.HasValue) depart = DateTime.Now.AddDays(1);
            if (!room_select.HasValue) room_select = 0;

            //var allFutureGuests = _guestService.GetAll(HotelID).Where(x => !x.IsActive && x.IsFutureReservation).SelectMany(x => x.GuestRooms);
            //allFutureGuests = allFutureGuests.Distinct();
            //var model = new RoomBookingViewModel { GuestsRoomsList = allFutureGuests.ToList()};
            var guestList = _guestService.GetAll(HotelID).Where(x => !x.IsActive && x.IsFutureReservation).ToList();
            var model = new RoomBookingViewModel { GuestList = guestList };
            return View(model);  
        }

        public ActionResult FutureBookingShow(int? id)
        {
            var room = _roomService.GetById(id.Value);
            var guestLists = room.GuestReservations.Where(x => x.IsActive).Select(x => x.Guest);
            var guestList = guestLists.Where(x => !x.IsActive && x.IsFutureReservation).ToList();
            var model = new RoomBookingViewModel { GuestList = guestList };
            return View("FutureBooking", model);
        }

        [HttpGet]
        //[OutputCache(Duration = 3600, VaryByParam = "arrive,depart,room_select")]
        public ActionResult PrintLandingForGuest(int? id, DateTime? arrive, DateTime? depart, int? room_select)
        {
            var model = new RoomBookingViewModel { GuestId = id.Value};
            return View(model);
        }

        [HttpGet]
        //[OutputCache(Duration = 3600, VaryByParam = "arrive,depart,room_select")]
        public ActionResult PrintLandingForGuestCheckin(int? id, DateTime? arrive, DateTime? depart, int? room_select)
        {
            var model = new RoomBookingViewModel { GuestId = id.Value };
            return View(model);
        }

        //[OutputCache(Duration = 3600, VaryByParam = "arrive,depart,room_select")]
        public ActionResult NewFutureBooking(DateTime? arrive, DateTime? depart, int? room_select)
        {
            if (!arrive.HasValue) arrive = DateTime.Now;
            if (!depart.HasValue) depart = DateTime.Now.AddDays(1);
            if (!room_select.HasValue) room_select = 0;

            IEnumerable<GuestReservation> gr = _roomService.GetAll(HotelID).SelectMany(x => x.GuestReservations).ToList();
            var conflicts = gr.SelectAvailable(arrive.Value, depart.Value, room_select.Value).ToList();

            if (conflicts.Count > 0)
            {
                var ids = conflicts.Select(x => x.RoomId).ToList();
                var model = new RoomBookingViewModel { CheckinDate = arrive, CheckoutDate = depart, RoomsList = _roomService.GetAll(HotelID).Where(x => !ids.Contains(x.Id)).ToList() };
                return View("NewFutureBooking", model);
            }
            else
            {
                var model = new RoomBookingViewModel { CheckinDate = arrive, CheckoutDate = depart, RoomsList = _roomService.GetAll(HotelID).ToList() };
                return View("NewFutureBooking", model);
            }
        }

        //[OutputCache(Duration = 3600, VaryByParam = "arrive,depart,room_select")]
        public ActionResult GroupBooking(DateTime? arrive, DateTime? depart, int? room_select)
        {
            if (!arrive.HasValue) arrive = DateTime.Now;
            if (!depart.HasValue) depart = DateTime.Now.AddDays(1);
            if (!room_select.HasValue) room_select = 0;

            IEnumerable<GuestReservation> gr = _roomService.GetAll(HotelID).SelectMany(x => x.GuestReservations).ToList();
            var conflicts = gr.SelectAvailable(arrive.Value, depart.Value, room_select.Value).ToList();

            if (conflicts.Count > 0)
            {
                var ids = conflicts.Select(x => x.RoomId).ToList();
                var model = new RoomBookingViewModel { CheckinDate = arrive, CheckoutDate = depart, RoomsList = _roomService.GetAll(HotelID).Where(x => !ids.Contains(x.Id)).ToList() };
                return View("GroupBooking", model);
            }
            else
            {
                var model = new RoomBookingViewModel { CheckinDate = arrive, CheckoutDate = depart, RoomsList = _roomService.GetAll(HotelID).ToList() };
                return View("GroupBooking", model);
            }
        }

        //NextMonth
        public ActionResult NextMonth(int? id, int? room_select)
        {
            id++;

            if (id.Value > 12)
                id = 1;

            var now = DateTime.Today;
            DateTime? arrive = new DateTime(now.Year, id.Value, 1);
            DateTime? depart = arrive.Value.AddMonths(1);



            if (!arrive.HasValue) arrive = DateTime.Now;
            if (!depart.HasValue) depart = DateTime.Now.AddDays(1);
            if (!room_select.HasValue) room_select = 0;

            IEnumerable<GuestReservation> gr = _roomService.GetAll(HotelID).SelectMany(x => x.GuestReservations).ToList();


            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var daysInMonth = System.DateTime.DaysInMonth(now.Year, now.Month);
            var endOfMonth = new DateTime(now.Year, now.Month, daysInMonth);

            var conflicts = gr.SelectAvailable(arrive.Value, depart.Value, room_select.Value).ToList();
            var allRooms = _roomService.GetAll(HotelID);

            if (conflicts.Count > 0)
            {
                var ids = conflicts.Select(x => x.RoomId).ToList();
                var model = new RoomBookingViewModel { RoomsList = allRooms.Where(x => !ids.Contains(x.Id)).ToList(), RoomsMatrixList = allRooms.ToList(), StartOfMonth = startOfMonth, EndOfMonth = endOfMonth };
                model.MonthId = arrive.Value.Month;
                model.ThisMonth = arrive.Value;
                return PartialView("_RoomMatrixDisplay", model);
            }
            else
            {
                var ids = _guestRoomService.GetAll(HotelID).Where(x => x.CheckoutDate >= arrive).Select(x => x.RoomId).ToList();
                var dontSelectRooms = allRooms.Where(x => ids.Contains(x.Id)).ToList();

                var model = new RoomBookingViewModel { RoomsList = dontSelectRooms, RoomsMatrixList = dontSelectRooms, StartOfMonth = startOfMonth, EndOfMonth = endOfMonth };
                model.MonthId = arrive.Value.Month;
                model.ThisMonth = arrive.Value;
                return PartialView("_RoomMatrixDisplay", model);
            }
        }

        public ActionResult PreviousMonth(int? id, int? room_select)
        {
            if (id.Value > 1)
                id--;

            var now = DateTime.Today;
            DateTime? arrive = new DateTime(now.Year, id.Value, 1);
            DateTime? depart = arrive.Value.AddMonths(1);



            if (!arrive.HasValue) arrive = DateTime.Now;
            if (!depart.HasValue) depart = DateTime.Now.AddDays(1);
            if (!room_select.HasValue) room_select = 0;

            IEnumerable<GuestReservation> gr = _roomService.GetAll(HotelID).SelectMany(x => x.GuestReservations).ToList();
            

            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var daysInMonth = System.DateTime.DaysInMonth(now.Year, now.Month);
            var endOfMonth = new DateTime(now.Year, now.Month, daysInMonth);

            var conflicts = gr.SelectAvailable(arrive.Value, depart.Value, room_select.Value).ToList();
            var allRooms = _roomService.GetAll(HotelID);

            if (conflicts.Count > 0)
            {
                var ids = conflicts.Select(x => x.RoomId).ToList();
                var model = new RoomBookingViewModel { RoomsList = allRooms.Where(x => !ids.Contains(x.Id)).ToList(), RoomsMatrixList = allRooms.ToList(), StartOfMonth = startOfMonth, EndOfMonth = endOfMonth };
                model.MonthId = arrive.Value.Month;
                model.ThisMonth = arrive.Value;
                return PartialView("_RoomMatrixDisplay", model);
            }
            else
            {
                var model = new RoomBookingViewModel { RoomsList = allRooms, RoomsMatrixList = allRooms.ToList(), StartOfMonth = startOfMonth, EndOfMonth = endOfMonth };
                model.MonthId = arrive.Value.Month;
                model.ThisMonth = arrive.Value;
                return PartialView("_RoomMatrixDisplay", model);
            }
            
        }


        public ActionResult ViewRooms(DateTime? arrive, DateTime? depart, int? room_select)
        {
            if (!arrive.HasValue) arrive = DateTime.Now;
            if (!depart.HasValue) depart = DateTime.Now.AddDays(1);
            if (!room_select.HasValue) room_select = 0;

            IEnumerable<GuestReservation> gr = _roomService.GetAll(HotelID).SelectMany(x => x.GuestReservations).ToList();
            var now = DateTime.Today;

            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var daysInMonth = System.DateTime.DaysInMonth(now.Year, now.Month);
            var endOfMonth = new DateTime(now.Year, now.Month, daysInMonth);

            var conflicts = gr.SelectAvailable(arrive.Value, depart.Value, room_select.Value).ToList();
            var allRooms = _roomService.GetAll(HotelID);

            if (conflicts.Count > 0)
            {
                var ids = conflicts.Select(x => x.RoomId).ToList();
                var model = new RoomBookingViewModel { RoomsList = allRooms.Where(x => !ids.Contains(x.Id)).ToList(), RoomsMatrixList = allRooms.ToList(), StartOfMonth = startOfMonth, EndOfMonth = endOfMonth };
                model.MonthId = now.Month;
                model.ThisMonth = now;
                return View("ViewRooms", model);
            }
            else
            {
                var model = new RoomBookingViewModel { RoomsList = allRooms, RoomsMatrixList = allRooms.ToList(), StartOfMonth = startOfMonth, EndOfMonth = endOfMonth };
                model.MonthId = now.Month;
                model.ThisMonth = now;

                return View("ViewRooms", model);
            }
        }

       

        //
        public ActionResult MatrixRooms(DateTime? arrive, DateTime? depart, int? room_select)
        {
            if (!arrive.HasValue) arrive = DateTime.Now;
            if (!depart.HasValue) depart = DateTime.Now.AddDays(1);
            if (!room_select.HasValue) room_select = 0;

            IEnumerable<GuestReservation> gr = _roomService.GetAll(HotelID).SelectMany(x => x.GuestReservations).ToList();
            var now = DateTime.Today;

            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var daysInMonth = System.DateTime.DaysInMonth(now.Year, now.Month);
            var endOfMonth = new DateTime(now.Year, now.Month, daysInMonth);

            var conflicts = gr.SelectAvailable(arrive.Value, depart.Value, room_select.Value).ToList();
            var allRooms = _roomService.GetAll(HotelID);

            if (conflicts.Count > 0)
            {
                var ids = conflicts.Select(x => x.RoomId).ToList();
                var model = new RoomBookingViewModel { RoomsList = allRooms.Where(x => !ids.Contains(x.Id)).ToList(), RoomsMatrixList = allRooms.ToList(), StartOfMonth = startOfMonth, EndOfMonth = endOfMonth };
                model.MonthId = now.Month;
                model.ThisMonth = now;
                return View("MatrixRooms", model);
            }
            else
            {
                var model = new RoomBookingViewModel { RoomsList = allRooms, RoomsMatrixList = allRooms.ToList(), StartOfMonth = startOfMonth, EndOfMonth = endOfMonth };
                model.MonthId = now.Month;
                model.ThisMonth = now;

                return View("MatrixRooms", model);
            }
        }

        ////[OutputCache(Duration = 3600, VaryByParam = "arrive,depart,room_select")]
        public ActionResult Booking(DateTime? arrive, DateTime? depart, int? room_select)
        {
            if (!arrive.HasValue) arrive = DateTime.Now;
            if (!depart.HasValue) depart = DateTime.Now.AddDays(1);
            if (!room_select.HasValue) room_select = 0;                      

            IEnumerable<GuestReservation> gr = _roomService.GetAll(HotelID).SelectMany(x => x.GuestReservations).ToList();
            var now = DateTime.Today;

            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var daysInMonth = System.DateTime.DaysInMonth(now.Year, now.Month);
            var endOfMonth = new DateTime(now.Year, now.Month, daysInMonth);

            var conflicts = gr.SelectAvailable(arrive.Value, depart.Value, room_select.Value).ToList();
            var allRooms = _roomService.GetAll(HotelID);

            if (conflicts.Count > 0)
            {
                var ids = conflicts.Select(x => x.RoomId).ToList();                
                var model = new RoomBookingViewModel { RoomsList = allRooms.Where(x => !ids.Contains(x.Id)).ToList(), RoomsMatrixList = allRooms.ToList(), StartOfMonth = startOfMonth, EndOfMonth = endOfMonth };
                model.MonthId = now.Month;
                model.ThisMonth = now;
                return View("Booking", model);
            }
            else
            {
                var model = new RoomBookingViewModel { RoomsList = allRooms, RoomsMatrixList = allRooms.ToList(), StartOfMonth = startOfMonth, EndOfMonth = endOfMonth };
                model.MonthId = now.Month;
                model.ThisMonth = now;

                return View("Booking", model);
            }
        }

        public ActionResult CleanFolder()
        {
            //Path.Combine(Server.MapPath("~/ProductProfile/Initial")
            var path = Path.Combine(Server.MapPath("~/Products/Small/"));
            DirectoryInfo folder = new DirectoryInfo(path);
            var files = folder.GetFiles("*.*", SearchOption.AllDirectories);
            foreach (var f in files)
            {
                ResizeImage(f.FullName, f.FullName, 120, 120, false);

            }
            return RedirectToAction("Index");
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