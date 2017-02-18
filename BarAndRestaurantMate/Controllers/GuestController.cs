using HotelMateWeb.Dal.DataCore;
using HotelMateWeb.Services.Core;
using HotelMateWeb.Services.ServiceApi;
using BarAndRestaurantMate.Helpers;
using BarAndRestaurantMate.Helpers.Enums;
using BarAndRestaurantMate.Models;
using BarAndRestaurantMate.Security;
using Lib.Web.Mvc;
using POSService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Freestyle.Helpers;
//using Agbo21.Dal;
using Microsoft.PointOfService;
using System.Web.Security;
using BarAndRestaurantMate.Extensions;
using System.Collections;
using AutoMapper;
using HotelMateWeb.Dal;
using BarAndRestaurantMate.SignalrRepository;

namespace BarAndRestaurantMate.Controllers
{
    [HandleError(View = "CustomErrorView")]
    public class GuestController : Controller
    {
        private string _merchantID = "00027";
        private int _startCategoryID = 124;
        private int _AdminID = 2;
        private int HotelId = 1;



        //private string _responseCodesuccess = "00";

        private  IGuestService _guestService;
        private  IGuestPlaylistService _guestPlaylistService;
        private  IPersonService _personService;
        private  IMovieService _movieService;
        private  IPOSItemService _posItemService;
        private  ITaxiService _taxiService;
        private  IAdventureService _adventureService;
        private  IMovieCategoryService _movieCategoryService;
        private  IEscortService _escortService;
        private  IStockItemService _stockItemService;
        private  IGuestOrderItemService _guestOrderItemService;
        private  IGuestOrderService _guestOrderService;
        private  IGuestRequestItemService _guestRequestItemService;
        private  IGuestChatMessageService _guestChatMessageService;
        private  IGuestChatService _guestChatService;
        private  ITableItemService _tableItemService;
        private  IGuestMessageService _guestMessageService;
        private  IBarTableService _barTableService;
        private  ISoldItemService _soldItemService;
        private  IPaymentOrderService _paymentOrderService;
        private  ISchoolPictureService _schoolPictureService;
        private  IRoomService _roomService;
        private  IRoomTypeService _roomTypeService;
        private  IBusinessAccountService _businessAccountService;
        private IGuestRoomService _guestRoomService;
        private IGuestRoomAccountService _guestRoomAccountService;
        private IGuestReservationService _guestReservationService;
        private IPrinterTableService _printerTableService;

        public GuestController()
        {
            _businessAccountService = new BusinessAccountService();
            _schoolPictureService = new SchoolPictureService();
            _paymentOrderService = new PaymentOrderService();
            _guestMessageService = new GuestMessageService();
            _guestService = new GuestService();
            _personService = new PersonService();
            _movieService = new MovieService();
            _posItemService = new POSItemService();
            _taxiService = new TaxiService();
            _adventureService = new AdventureService();
            _movieCategoryService = new MovieCategoryService();
            _guestPlaylistService = new GuestPlaylistService();
            _escortService = new EscortService();
            _stockItemService = new StockActualItemService();
            _guestOrderItemService = new GuestOrderItemService();
            _guestOrderService = new GuestOrderService();
            _guestRequestItemService = new GuestRequestItemService();
            _guestChatMessageService = new GuestChatMessageService();
            _guestChatService = new GuestChatService();
            _tableItemService = new TableItemService();
            _barTableService = new BarTableService();
            _soldItemService = new SoldItemService();
            _roomService = new RoomService();
            _guestRoomService = new GuestRoomService();
            _guestRoomAccountService = new GuestRoomAccountService();
            _guestReservationService = new GuestReservationService();
            _roomTypeService = new RoomTypeService();
            _printerTableService = new PrinterTableService();
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing && _printerTableService != null)
            {
                _printerTableService.Dispose();
                _printerTableService = null;
            }

            if (disposing && _roomTypeService != null)
            {
                _roomTypeService.Dispose();
                _roomTypeService = null;
            }

            if (disposing && _guestRoomService != null)
            {
                _guestRoomService.Dispose();
                _guestRoomService = null;
            }

            if (disposing && _guestRoomAccountService != null)
            {
                _guestRoomAccountService.Dispose();
                _guestRoomAccountService = null;
            }
            if (disposing && _guestReservationService != null)
            {
                _guestReservationService.Dispose();
                _guestReservationService = null;
            }








            if (disposing && _roomService != null)
            {
                _roomService.Dispose();
                _roomService = null;
            }

            if (disposing && _businessAccountService != null)
            {
                _businessAccountService.Dispose();
                _businessAccountService = null;
            }


            if (disposing && _schoolPictureService != null)
            {
                _schoolPictureService.Dispose();
                _schoolPictureService = null;
            }

            if (disposing && _paymentOrderService != null)
            {
                _paymentOrderService.Dispose();
                _paymentOrderService = null;
            }
            if (disposing && _guestService != null)
            {
                _guestService.Dispose();
                _guestService = null;
            }

            if (disposing && _guestPlaylistService != null)
            {
                _guestPlaylistService.Dispose();
                _guestPlaylistService = null;
            }

            if (disposing && _personService != null)
            {
                _personService.Dispose();
                _personService = null;
            }

            if (disposing && _movieService != null)
            {
                _movieService.Dispose();
                _movieService = null;
            }

            if (disposing && _posItemService != null)
            {
                _posItemService.Dispose();
                _posItemService = null;
            }

            if (disposing && _taxiService != null)
            {
                _taxiService.Dispose();
                _taxiService = null;
            }

            if (disposing && _adventureService != null)
            {
                _adventureService.Dispose();
                _adventureService = null;
            }

            if (disposing && _movieCategoryService != null)
            {
                _movieCategoryService.Dispose();
                _movieCategoryService = null;
            }

            if (disposing && _escortService != null)
            {
                _escortService.Dispose();
                _escortService = null;
            }

            if (disposing && _stockItemService != null)
            {
                _stockItemService.Dispose();
                _stockItemService = null;
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

            if (disposing && _guestRequestItemService != null)
            {
                _guestRequestItemService.Dispose();
                _guestRequestItemService = null;
            }

            if (disposing && _guestChatMessageService != null)
            {
                _guestChatMessageService.Dispose();
                _guestChatMessageService = null;
            }

            if (disposing && _guestChatService != null)
            {
                _guestChatService.Dispose();
                _guestChatService = null;
            }

            if (disposing && _tableItemService != null)
            {
                _tableItemService.Dispose();
                _tableItemService = null;
            }

            if (disposing && _guestMessageService != null)
            {
                _guestMessageService.Dispose();
                _guestMessageService = null;
            }

            if (disposing && _barTableService != null)
            {
                _barTableService.Dispose();
                _barTableService = null;
            }

            if (disposing && _soldItemService != null)
            {
                _soldItemService.Dispose();
                _soldItemService = null;
            }

            base.Dispose(disposing);
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

            if(string.IsNullOrEmpty(happyHour))
            {
                 return 0;
            }
            else
            {
                var h = happyHour.Split(',');
                if(h.Length > 0)
                {
                    for(int i = 0; i < 6; i++)
                    {
                        try
                        {
                            var hHour = h[i].Split('@');

                            if(hHour.Length == 3)
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

                                if(day == dateDay && hour >= start && hour <= end)
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

        private IList<CategoryModel> _categories;
        public IList<CategoryModel> CategoryList
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
                    _categories = GetAllCategories().ToList();

                    if (GetHappyHour() == 1)
                        return _categories;
                    else
                        return _categories.Where(x => !x.Name.ToUpper().StartsWith("HAPPY")).ToList();

                }
            }
            set
            {
                _categories = GetAllCategories().ToList();
            }
        }

        private IList<StockItem> _realProducts;

        public IList<StockItem> RealProductsList
        {
            get
            {
                if (_realProducts != null)
                    return _realProducts;
                else
                {
                    _realProducts = _stockItemService.GetAll(); //StockItemService.GetStockItems(1).ToList();
                    return _realProducts;
                }
            }
            set
            {
                _products = StockItemService.GetStockItems(1).ToList();
            }
        }

        private IList<POSService.Entities.StockItem> _products;

        public IList<POSService.Entities.StockItem> ProductsList
        {
            get
            {
                if (_products != null)
                    return _products;
                else
                {
                    _products = StockItemService.GetStockItems(1).ToList();
                    return _products;
                }
            }
            set
            {
                _products = StockItemService.GetStockItems(1).ToList();
            }
        }


        private int? _hotelId;
        private int HotelID
        {
            get { return _hotelId ?? GetHotelId(); }
            set { _hotelId = value; }
        }

        private int GetHotelId()
        {
              return 1;
        }


        private int? _personId;
        private int? PersonId
        {
            get { return _personId ?? GetPersonId(); }
            set { _personId = value; }
        }

        //private Person GetPerson()
        //{
        //    var username = User.Identity.Name;
        //    var user = _personService.GetAllForLogin().FirstOrDefault(x => !string.IsNullOrEmpty(x.IdNumber) && x.Username.Equals(username, StringComparison.CurrentCultureIgnoreCase));
        //    return user;
        //}

        private int? GetPersonId()
        {
            var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];

            if (null == cookie)
                return null;

            var decrypted = FormsAuthentication.Decrypt(cookie.Value);

            if (!string.IsNullOrEmpty(decrypted.UserData))
                return int.Parse(decrypted.UserData);

            return null;
        }

        [HttpGet]
        public ActionResult AddPhotos()
        {
            HotelMenuModel hmm = new HotelMenuModel();
            hmm.Pictures = _schoolPictureService.GetAll().ToList();
            return View(hmm);
        }


        //[HttpGet]
        //public ActionResult GalleryIndex()
        //{
        //    HotelMenuModel hmm = new HotelMenuModel();
        //    hmm.Pictures = _schoolPictureService.GetAll().ToList();
        //    return View("GalleryIndexNew", hmm);
        //}

        [OutputCache(Duration = int.MaxValue)]
        public ActionResult Puzzle1(int? id)
        {
            return View();
        }

        [OutputCache(Duration = int.MaxValue)]
        public ActionResult Puzzle2(int? id)
        {
            return View();
        }

        [OutputCache(Duration = int.MaxValue)]
        public ActionResult Puzzle3(int? id)
        {
            return View();
        }

        [OutputCache(Duration = int.MaxValue)]
        public ActionResult Puzzle4(int? id)
        {
            return View();
        }

        [OutputCache(Duration = int.MaxValue)]
        public ActionResult Puzzle6(int? id)
        {
            return View();
        }

        [OutputCache(Duration = int.MaxValue)]
        public ActionResult Puzzle7(int? id)
        {
            return View();
        }

        [OutputCache(Duration = int.MaxValue)]
        public ActionResult Puzzle8(int? id)
        {
            return View();
        }
        

        [OutputCache(Duration = int.MaxValue)]
        public ActionResult Puzzle(int? id)
        {
            
            return View();
        }

        [OutputCache(Duration = int.MaxValue)]
        public ActionResult GamesIndex(int? id)
        {
            return View();
        }

        public ActionResult VideoPlayer(int? id)
        {
            return new VideoDataResult(id);
        }


       

        [HttpPost]
        public ActionResult EditTableItem(HotelMenuModel model, string send_booking)
        {
           if(ModelState.IsValid)
           {
               var ti = _tableItemService.GetById(model.Id);

               if(ti != null)
               {
                   if(send_booking.StartsWith("Save"))
                   {
                       ti.Qty = model.TableItem.Qty;
                       _tableItemService.Update(ti);
                   }
                   else
                   {
                       _tableItemService.Delete(ti);
                   }
               }
           }

            return RedirectToAction("IndexRoomService");
        }

        
        [HttpGet]
        public ActionResult EditTableitem(int? id)
        {
            var personId = PersonId.Value;
            var ti = _tableItemService.GetById(id.Value);
            HotelMenuModel hmm = new HotelMenuModel();
            hmm.TableItem = ti;
            hmm.GuestName = "GUEST";
            return View(hmm);
        }

        [HttpGet]
        [OutputCache(Duration = int.MaxValue, VaryByParam = "id")]
        public ActionResult BookEscort(int? id)
        {
            var personId = PersonId.Value;

            var guest = _guestService.GetAll(1).FirstOrDefault(x => x.PersonId == personId);
            var item = _escortService.GetAll(HotelID).FirstOrDefault(x => x.Id == id.Value);
            var drivername = item.Name;
            var guestName = guest.FullName;
            var guestPhone = guest.Mobile;
            var driversNumber = item.Telephone;
            var msg = "Your services are required at " + GetHotelsName() + ", Room No-" + guest.GuestRooms.FirstOrDefault().Room.RoomNumber + ", The Telephone number of the guest is : " + driversNumber + ", Guest name is " + guest.FullName;
           // SendSMS(driversNumber, "", msg);

            HotelMenuModel hmm = new HotelMenuModel();
            hmm.EscortItem = item;
            return View(hmm);
        }

        //[HttpGet]
        //public ActionResult AddPhotos()
        //{
        //    HotelMenuModel hmm = new HotelMenuModel();
        //    return View(hmm);
        //}

        [HttpGet]
        //[OutputCache(Duration = int.MaxValue)]
        public ActionResult GalleryIndex()
        {
            HotelMenuModel hmm = new HotelMenuModel();

            try
            {
                //hmm.Pictures = _schoolPictureService.GetAll().Where(x => !string.IsNullOrEmpty(x.ModifiedBy) && x.ModifiedBy.ToUpper() == "MANAGER").ToList();
                hmm.Pictures = _schoolPictureService.GetAll().OrderByDescending(x => x.CreatedDate).ToList();

            }
            catch
            {
                hmm.Pictures = new List<SchoolPicture>();
            }

            return View("GalleryIndexNew", hmm);
        }

        
        [HttpPost]
        public ActionResult DeletePictures(string caption)
        {
            HotelMenuModel hmm = new HotelMenuModel();

            var ids = _schoolPictureService.GetAll().Where(x => string.IsNullOrEmpty(x.ModifiedBy) && x.CreatedDate > DateTime.Now.AddMonths(-1)).Select(x => x.SchoolPicturesId).ToList();
            foreach(var id in ids)
            {
                string requestStr = "Approved_" + id.ToString();

                if(Request[requestStr] != null)
                {
                    var existingSchoolPicture = _schoolPictureService.GetById(id);
                    if(existingSchoolPicture != null)
                    {
                        var torf = Request[requestStr].ToString();
                        if(torf != "false")
                        {
                            existingSchoolPicture.ModifiedBy = "MANAGER";
                            existingSchoolPicture.ModifiedDate = DateTime.Now;

                            if(!string.IsNullOrEmpty(caption))
                            {
                                existingSchoolPicture.Caption = caption;
                            }

                        }
                        else
                        {
                            existingSchoolPicture.ModifiedBy = string.Empty;
                            existingSchoolPicture.ModifiedDate = DateTime.Now;
                        }

                        _schoolPictureService.Update(existingSchoolPicture);
                    }
                    
                }
            }

            return RedirectToAction("GalleryIndex");
        }
        [HttpGet]
        //[OutputCache(Duration = int.MaxValue)]
        public ActionResult ApproveGalleryIndex()
        {
            HotelMenuModel hmm = new HotelMenuModel();
            hmm.Pictures = _schoolPictureService.GetAll().Where(x => string.IsNullOrEmpty(x.ModifiedBy) && x.CreatedDate > DateTime.Now.AddMonths(-1)).ToList();
            return View(hmm);
        }

        [HttpGet]
        [OutputCache(Duration = int.MaxValue, VaryByParam = "id")]
        public ActionResult BookTaxi(int? id)
        {
            var personId = PersonId.Value;

            var guest = _guestService.GetAll(1).FirstOrDefault(x => x.PersonId == personId);
            var item = _taxiService.GetAll(HotelID).FirstOrDefault(x => x.Id == id.Value);
            var drivername = item.Name;
            var guestName = guest.FullName;
            var guestPhone = guest.Mobile;
            var driversNumber = item.Telephone;
            var msg = "Your services are required at " + GetHotelsName() + ", Room No-" + guest.GuestRooms.FirstOrDefault().Room.RoomNumber + ", The Telephone number of the guest is : " + driversNumber + ", Guest name is " + guest.FullName;
            SendSMS(driversNumber, "", msg);

            HotelMenuModel hmm = new HotelMenuModel();
            hmm.CarItem = item;
            return View(hmm);
        }

        [HttpGet]
        [OutputCache(Duration = int.MaxValue, VaryByParam = "id")]
        public ActionResult ContactAgent(int? id)
        {
            var personId = PersonId.Value;

            var guest = _guestService.GetAll(1).FirstOrDefault(x => x.PersonId == personId);

            var agentsNumber = GetBookingAgent();
            var msg = "A Guest at " + GetHotelsName() + " needs to book a flight, Room No-" + guest.GuestRooms.FirstOrDefault().Room.RoomNumber + ", The Telephone number of the guest is : " + agentsNumber + ", Guest name is " + guest.FullName;
            SendSMS(agentsNumber, "", msg);

            HotelMenuModel hmm = new HotelMenuModel();
            
            hmm.BookingAgentNumber = agentsNumber;

            return View(hmm);
        }

        private string GetHotelsName()
        {
            
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

        private string GetBookingAgent()
        {
            
            var bookingAgent = string.Empty;

            try
            {
                bookingAgent = ConfigurationManager.AppSettings["BookingAgent"].ToString();
            }
            catch
            {
                bookingAgent = "";
            }

            return bookingAgent;
        }


        private bool SendSMS(string dest, string source, string msg)
        {


            if (!dest.StartsWith("234") || string.IsNullOrEmpty(dest))
                return false;

            var telephones = dest.Split(',').ToList();

            foreach (var tel in telephones)
            {

                string username = "academyvist1";
                string password = "k9Md0uzK";
                source = "447958631557";

                var canSendSms = IsSMSEnabled();

                HTTPSMS.SendSMS sms = new HTTPSMS.SendSMS();

                sms.initialise(username, password);

                try
                {
                    if (canSendSms)
                        sms.sendSMS(tel, source, msg);
                }
                catch (HTTPSMS.SMSClientException ex)
                {
                    string msg23 = ex.Message();
                    return false;
                }
            }

            return true;
        }


        private bool IsSMSEnabled()
        {

            var sendSMS = false;

            try
            {
                string smsplus = ConfigurationManager.AppSettings["SMSMesagingEnabled"].ToString();

                if (smsplus == "1")
                    sendSMS = true;

            }
            catch
            {
                sendSMS = false;
            }

            return sendSMS;
        }


        [HttpGet]
        [OutputCache(Duration = int.MaxValue, VaryByParam = "id")]
        public ActionResult ViewCarImage(int? id)
        {
            var item = _taxiService.GetAll(HotelID).FirstOrDefault(x => x.Id == id.Value);
            HotelMenuModel hmm = new HotelMenuModel();
            hmm.CarItem = item;
            return View(hmm);
        }



        [HttpGet]
        [OutputCache(Duration = int.MaxValue, VaryByParam = "id")]
        public ActionResult ViewImage(int? id)
        {
            var item = _posItemService.GetAll().FirstOrDefault(x => x.Id == id.Value);
            HotelMenuModel hmm = new HotelMenuModel();
            hmm.MenuItem = item;
            return View(hmm);
        }


        [HttpGet]
        [OutputCache(Duration = int.MaxValue, VaryByParam = "id")]
        public ActionResult CompleteViewAdventure(int? id)
        {
            var Adventureitem = _adventureService.GetAll(HotelID).FirstOrDefault(x => x.Id == id.Value);
            HotelMenuModel hmm = new HotelMenuModel();
            hmm.Adventure = Adventureitem;
            return View(hmm);
        }

        private IEnumerable<POSService.Entities.BusinessAccount> _businessAccount;

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


        //VoidTransaction
        [HttpPost]
        public ActionResult VoidTransaction(string recieptNumber, string reciept, string reasonForVoid)
        {
            if (reciept.StartsWith("TRANSFER"))
            {
                return RedirectToAction("TranferToCompany", new { recieptNumber });
            }

            var allItems = _soldItemService.GetAll().Where(x => x.IsActive && x.RecieptNumber.ToUpper().Equals(recieptNumber.ToUpper())).ToList();

            foreach(var item in allItems)
            {
                item.IsActive = false;
                item.ReasonForVoid = reasonForVoid;
                _soldItemService.Update(item);
            }

            return RedirectToAction("IndexRetrieval");

        }

        [HttpPost]
        public ActionResult TransferToCompany(string recieptNumber, int? companyId)
        {
            var allItems = _soldItemService.GetAll().Where(x => x.IsActive && x.RecieptNumber.ToUpper().Equals(recieptNumber.ToUpper())).ToList();

            foreach (var item in allItems)
            {
                item.BusinessAccountId = companyId.Value;
                _soldItemService.Update(item);
            }

            return RedirectToAction("IndexRetrieval");
        }
        
        [HttpGet]
        public ActionResult TranferToCompany(string recieptNumber)
        {
            var accountlst = BusinessAccountList.ToList();
            var twentyfourhrsAgo = DateTime.Now.AddHours(-24);
            var items = _soldItemService.GetAllInclude("StockItem").Where(x => x.RecieptNumber == recieptNumber && x.IsActive).OrderByDescending(x => x.DateSold).GroupBy(x => x.RecieptNumber).Select(x => new ReceiptModel { SaleTime = x.Select(w => w.DateSold).FirstOrDefault(), RecieptNumber = x.Key, Items = x.ToList(), ValueIds = x.ToList().Select(y => y.Id.ToString()).ToDelimitedString(",") }).ToList();
            HotelMenuModel hmm = new HotelMenuModel();
            hmm.Receipts = items;
            hmm.CurrentBusinessAccounts = accountlst;
            hmm.ReceiptNumber = recieptNumber;
            return View(hmm);
        }

        [HttpGet]
        public ActionResult IndexRetrieval()
        {
            var twentyfourhrsAgo = DateTime.Now.AddHours(-24);
            var items = _soldItemService.GetAllInclude("StockItem").Where(x => x.DateSold > twentyfourhrsAgo && x.IsActive).OrderByDescending(x => x.DateSold).GroupBy(x => x.RecieptNumber).Select(x => new ReceiptModel { BusinessAccount = x.Select(w => w.BusinessAccountId.HasValue).FirstOrDefault(), SaleTime = x.Select(w => w.DateSold).FirstOrDefault(), RecieptNumber = x.Key, Items = x.ToList(), ValueIds = x.ToList().Select(y => y.Id.ToString()).ToDelimitedString(",") }).ToList();
            HotelMenuModel hmm = new HotelMenuModel();
            hmm.Receipts = items;
            return View(hmm);
        }

        
        [HttpGet]
        [OutputCache(Duration = int.MaxValue)]
        public ActionResult LocalServices()
        {
            var items = _adventureService.GetAll(HotelID).Where(x => x.IsPlaceOfInterest).ToList();
            items.Reverse();
            HotelMenuModel hmm = new HotelMenuModel();
            hmm.Adventures = items;
            return View(hmm);
        }


        [HttpGet]
        [OutputCache(Duration = int.MaxValue)]
        public ActionResult TaxiServices()
        {
            var items = _taxiService.GetAll(HotelID).ToList();
            HotelMenuModel hmm = new HotelMenuModel();
            hmm.Taxis = items;
            return View(hmm);
        }

        [HttpGet]
        [OutputCache(Duration = int.MaxValue)]
        public ActionResult EscortServices()
        {
            var items = _escortService.GetAll(HotelID).ToList();
            HotelMenuModel hmm = new HotelMenuModel();
            hmm.Escorts = items;
            return View(hmm);
        }

        
        [HttpGet]
        ////[OutputCache(Duration = int.MaxValue)]
        public ActionResult CreateProfile()
        {
            EscortModel hmm = new EscortModel();
            hmm = new EscortModel { };
            return View(hmm);
        }

        [HttpPost]
        public ActionResult CreateProfile(EscortModel model, HttpPostedFileBase[] files)
        {
            if(ModelState.IsValid)
            {
                Escort escort = new Escort { Description = model.Description, Name = model.Name, Mobile = model.Mobile, Telephone = model.Telephone };

                List<EscortPicture> list = new List<EscortPicture>();

                foreach (var file in files)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/Products/Dating"), fileName);
                        file.SaveAs(path);
                        list.Add(new EscortPicture { PicturePath = fileName });
                        escort.PicturePath = fileName;
                    }
                }

                escort.EscortPictures = list;
                _escortService.Create(escort);

            }

            model.Saved = true;

            return View(model);
        }

        [HttpGet]
        [OutputCache(Duration = int.MaxValue)]
        public ActionResult EscortLanding()
        {
            var items = _escortService.GetAll(HotelID).ToList();
            HotelMenuModel hmm = new HotelMenuModel();
            hmm.Escorts = items;
            return View(hmm);
        }


        

        [OutputCache(Duration = int.MaxValue, VaryByParam="id")]
        [HttpGet]
        public ActionResult BuyCard(int? id)
        {
            var item = _adventureService.GetAll(HotelID).FirstOrDefault(x => x.Id == id.Value);
            HotelMenuModel hmm = new HotelMenuModel();
            hmm.Adventure = item;
            return View(hmm);
        }


        
        [HttpGet]
        //[OutputCache(Duration = int.MaxValue)]
        public ActionResult BuyRechargeCard()
        {
            var items = _adventureService.GetAll(HotelID).Where(x => !x.IsPlaceOfInterest && string.IsNullOrEmpty(x.Address)).ToList();
            HotelMenuModel hmm = new HotelMenuModel();
            hmm.Adventures = items;
            return View(hmm);
        }


        
        [HttpGet]
        [OutputCache(Duration = int.MaxValue)]
        public ActionResult FlightServices()
        {
            var items = _adventureService.GetAll(HotelID).Where(x => !x.IsPlaceOfInterest).ToList();
            HotelMenuModel hmm = new HotelMenuModel();
            hmm.Adventures = items;
            return View(hmm);
        }


        
        [HttpPost]
        public ActionResult FilterByCategorySelfByName(string name, int? canaddItemInt, int? guestOrderId)
        {
            canaddItemInt = 0;

            var catyList = CategoryList.ToList();

            var catLstId = catyList.Select(x => x.Id);

            var lstRequest = _guestOrderService.GetAll("GuestRequestItems").FirstOrDefault(x => x.Id == guestOrderId.Value && x.IsActive).GuestRequestItems.ToList();

            HotelMenuModel hmm = new HotelMenuModel();

            hmm.RequestedItems = lstRequest.Where(x => x.IsActive).ToList();

            if (hmm.RequestedItems.All(x => x.RequestBy == User.Identity.Name))
            {
                canaddItemInt = 1;
            }

            if (string.IsNullOrEmpty(name))
            {
                int? distributionPointId = CashierDistributionPointId();

                var items = new List<MenuModel>();

                

                if (distributionPointId.HasValue)
                {
                    items = _posItemService.GetAllInclude("StockItem").Where(x => x.DistributionPointId == distributionPointId.Value && x.Remaining > 0).GroupBy(x => x.StockItem.CategoryId).Select(x => new MenuModel { Id = x.Key, CategoryName = GetCategoryName(x.Key, catyList), Items = x.ToList() }).ToList();
                }
                else
                {
                    items = _posItemService.GetAllInclude("StockItem").Distinct(new PosItemComparer()).Where(x => x.Remaining > 0).GroupBy(x => x.StockItem.CategoryId).Select(x => new MenuModel { Id = x.Key, CategoryName = GetCategoryName(x.Key, catyList), Items = x.ToList() }).ToList();
                }

                items = items.Where(x => catLstId.Contains(x.Id)).ToList();

                hmm.Menu = items;
                hmm.ItemsOrdered = new List<ItemModel>();
                hmm.CanaddItemInt = canaddItemInt.Value;
                hmm.GuestOrderId = guestOrderId.Value;

                if (canaddItemInt.HasValue && canaddItemInt.Value == 1)
                {
                    return PartialView("_MenuItemsRebindSelf", hmm);
                }
                else
                {
                    return PartialView("_MenuItemsRebindSelfNoAdd", hmm);
                }


            }
            else
            {
                int? distributionPointId = CashierDistributionPointId();

                var items = new List<MenuModel>();

                if (distributionPointId.HasValue)
                {
                    items = _posItemService.GetAllInclude("StockItem").Where(x => x.DistributionPointId == distributionPointId.Value && x.Remaining > 0 && x.StockItem.StockItemName.ToUpper().Contains(name.ToUpper())).GroupBy(x => x.StockItem.CategoryId).Select(x => new MenuModel { Id = x.Key, CategoryName = GetCategoryName(x.Key, catyList), Items = x.ToList() }).ToList();
                }
                else
                {
                    items = _posItemService.GetAllInclude("StockItem").Distinct(new PosItemComparer()).Where(x => x.Remaining > 0 && x.StockItem.StockItemName.ToUpper().Contains(name.ToUpper())).GroupBy(x => x.StockItem.CategoryId).Select(x => new MenuModel { Id = x.Key, CategoryName = GetCategoryName(x.Key, catyList), Items = x.ToList() }).ToList();
                }

                items = items.Where(x => catLstId.Contains(x.Id)).ToList();

                hmm.Menu = items;

                hmm.ItemsOrdered = new List<ItemModel>();

                hmm.CanaddItemInt = canaddItemInt.Value;
                hmm.GuestOrderId = guestOrderId.Value;


                if (canaddItemInt.HasValue && canaddItemInt.Value == 1)
                {
                    return PartialView("_MenuItemsRebindSelf", hmm);
                }
                else
                {
                    return PartialView("_MenuItemsRebindSelfNoAdd", hmm);
                }
            }
        }

        [HttpPost]
        public ActionResult GroupReservationExistingBooking(string selectedRoomIds, RoomBookingViewModel model, int? GuestId, int? paymentMethodId, string paymentMethodNote)
        {
            var roomIds = selectedRoomIds.Split(',');

            var guest = _guestService.GetById(GuestId.Value);

            if (guest.GuestRooms.Any())
            {
                var guestRooms = new List<GuestRoom>();

                var i = 0;

                foreach (var gr in guest.GuestRooms)
                {
                    var rm = _roomService.GetById(gr.RoomId);
                    DateTime dtIn;
                    DateTime dtOut;
                    DateTime.TryParse(Request.Form["arrive_" + gr.Id], out dtIn);
                    DateTime.TryParse(Request.Form["depart_" + gr.Id], out dtOut);
                    var groupBookingMainRoom = i == 0;

                    i++;

                    var existingGuestRoom = _guestRoomService.GetById(gr.Id);
                    existingGuestRoom.GroupBookingMainRoom = groupBookingMainRoom;
                    existingGuestRoom.CheckinDate = DateTime.Now;
                    existingGuestRoom.CheckoutDate = DateTime.Now.AddDays(model.NumberOfNights);
                    existingGuestRoom.IsActive = true;
                    existingGuestRoom.GroupBooking = guestRooms.Count > 1;

                    var conflicts = rm.RoomAvailability(existingGuestRoom.CheckinDate, existingGuestRoom.CheckoutDate, GuestId.Value);

                    if (conflicts.Count > 0)
                    {
                        ModelState.AddModelError("CheckOutDate",
                            "There is a reservation clash with your proposed checkin/checkout date(s)");
                    }
                    else
                    {
                        guestRooms.Add(existingGuestRoom);
                    }
                }

                if (ModelState.IsValid)
                {

                    var ticks = (int)DateTime.Now.Ticks;

                    foreach (var gr in guestRooms)
                    {
                        _guestRoomService.Update(gr);
                    }

                    if (model.InitialDeposit > 0)
                    {
                        var grId = guestRooms.FirstOrDefault(x => x.GroupBookingMainRoom).Id;

                        var gra = new GuestRoomAccount
                        {
                            Amount = model.InitialDeposit,
                            PaymentTypeId = (int)RoomPaymentTypeEnum.InitialDeposit,
                            TransactionDate = DateTime.Now,
                            TransactionId = _personService.GetAllForLogin().Where(x => x.Username.ToUpper().Equals(User.Identity.Name)).FirstOrDefault().PersonID,
                            GuestRoomId = grId,
                            PaymentMethodId = paymentMethodId.HasValue ? paymentMethodId.Value : 1,
                            PaymentMethodNote = paymentMethodNote
                        };

                        _guestRoomAccountService.Create(gra);
                    }

                    foreach (var gr in guestRooms)
                    {
                        var guestReservation = _guestReservationService.GetAll(HotelId).FirstOrDefault(x => x.GuestId == GuestId && x.RoomId == gr.RoomId);
                        guestReservation.StartDate = gr.CheckinDate;
                        guestReservation.EndDate = gr.CheckoutDate;
                        guestReservation.IsActive = true;
                        _guestReservationService.Update(guestReservation);
                    }

                    if (model.Guest.CompanyId > 0)
                    {
                        guest.CompanyId = model.Guest.CompanyId;
                    }
                    else
                    {
                        guest.BusinessAccount = null;
                        guest.CompanyId = null;
                    }

                    guest.IsFutureReservation = false;
                    guest.IsActive = true;
                    guest.Status = "LIVE";
                   

                    _guestService.Update(guest);

                    var allRoomIds = guest.GuestRooms.Select(x => x.RoomId).ToList();

                    var strRoomNumber = string.Empty;

                    foreach (var id in allRoomIds)
                    {
                        var room = _roomService.GetById(id);
                        room.StatusId = (int)RoomStatusEnum.Occupied;
                        _roomService.Update(room);
                        strRoomNumber = room.RoomNumber;
                    }

                    if(!string.IsNullOrEmpty(strRoomNumber))
                    {
                        try
                        {
                            guest = _guestService.GetById(guest.Id);
                            Person person = new Person();
                            person.HotelId = guest.HotelId;
                            person.Address = guest.Address;
                            person.BirthDate = DateTime.Now;
                            person.DisplayName = guest.FullName;
                            person.FirstName = guest.FullName;
                            person.LastName = guest.FullName;
                            person.MiddleName = guest.FullName;
                            person.Password = GetLastName(guest.FullName);
                            person.Email = person.Password;
                            person.Username = strRoomNumber;
                            person.Title = "Mr";
                            person.PersonTypeId = (int)PersonTypeEnum.Guest;
                            person.IsActive = true;
                            person.EndDate = DateTime.Now.AddYears(1);
                            person.IdNumber = guest.Id.ToString();
                            person.PreviousEmployerStartDate = DateTime.Now;
                            person.PreviousEmployerEndDate = DateTime.Now;
                            person.StartDate = DateTime.Now;
                            guest.Person = person;
                            _guestService.Update(guest);
                        }
                        catch
                        {

                        }
                    }
                    return RedirectToAction("Index", "Home");

                }

                var guestRoomsP = _roomService.GetAll(HotelId).Where(x => roomIds.Contains(x.Id.ToString())).ToList();
                var firstRoomId = guestRoomsP.FirstOrDefault().Id;

                var gbvm = new GroupBookingViewModel
                {
                    GuestRooms = guestRoomsP,
                    selectedRoomIds = selectedRoomIds,
                    CheckinDate = model.CheckinDate,
                    CheckoutDate = model.CheckoutDate,
                    RoomBookingViewModel = GetModelForNewBooking(firstRoomId),
                    PageTitle = "GROUP BOOKING"
                };

                return View("EnterGuestDatesGroup", gbvm);

            }

            return RedirectToAction("Index", "Home");
        }

        private string GetLastName(string fullname)
        {
            var splitter = fullname.Split(' ');

            var count = splitter.Length;

            if (count > 1)
            {
                return splitter[count - 1];
            }

            return fullname;
        }


        private RoomBookingViewModel GetModelForNewBooking(int id)
        {
            var rmm = new Room();
            rmm.BookRoom();
            Mapper.CreateMap<Room, RoomViewModel>();
            Mapper.CreateMap<Guest, GuestViewModel>();
            var room = _roomService.GetById(id);
            var rvm = Mapper.Map<Room, RoomViewModel>(room);
            var gvm = Mapper.Map<Guest, GuestViewModel>(new Guest { IsActive = true, Status = "LIVE" });
            var model = new RoomBookingViewModel
            {
                Room = rvm,
                Guest = gvm,
                GuestRoom =
                    new GuestRoom
                    {
                        Occupants = 1,
                        RoomId = id,
                        CheckinDate = DateTime.Now,
                        CheckoutDate = DateTime.Now,
                        RoomRate = room.Price.Value
                    },

                BusinessAccounts = GetBusinessAccounts(null)
            };

            return model;
        }


        [HttpGet]
        public ActionResult NewBookingFromFutureReservation(int? id)
        {
            var guest = _guestService.GetById(id.Value);
            var guestRooms = guest.GuestRooms.ToList();
            var roomIdValues = guestRooms.Select(x => x.Room.Id.ToString()).ToDelimitedString(",");
            var firstRoomId = guestRooms.FirstOrDefault().RoomId;
            var roomIds = guestRooms.Select(x => x.Id.ToString()).ToList();

            var gbvm = new GroupBookingViewModel
            {
               
                GuestRooms = null,
                GuestId = guest.Id,
                GuestRoomsCheckedIn = guestRooms,
                selectedRoomIds = roomIdValues,
                CheckinDate = DateTime.Now.AddDays(1),
                CheckoutDate = DateTime.Now.AddDays(2),
                RoomBookingViewModel = GetModelForNewBooking(firstRoomId, guest),
                PageTitle = "NEW GUEST RESERVATIONS"
            };

            gbvm.RoomBookingViewModel.BusinessAccounts = GetBusinessAccounts(null);
            gbvm.RoomBookingViewModel.RoomBookingRooms = GetMultiSelectRooms(roomIds);
            gbvm.RoomBookingViewModel.ReservationDeposit = guest.GetGuestReservationBalance();
            gbvm.RoomBookingViewModel.NumberOfNights = 1;

            return View("EnterGuestDatesBookNow", gbvm);
        }

        private RoomBookingViewModel GetModelForNewBooking(int id, Guest guest)
        {
            var rmm = new Room();
            rmm.BookRoom();
            Mapper.CreateMap<Room, RoomViewModel>();
            Mapper.CreateMap<Guest, GuestViewModel>();
            var room = _roomService.GetById(id);
            var rvm = Mapper.Map<Room, RoomViewModel>(room);
            var gvm = Mapper.Map<Guest, GuestViewModel>(guest);
            var model = new RoomBookingViewModel
            {
                Room = rvm,
                Guest = gvm,
                GuestRefund = decimal.Zero,
                GuestRoom =
                    new GuestRoom
                    {
                        Occupants = 1,
                        RoomId = id,
                        CheckinDate = DateTime.Now,
                        CheckoutDate = DateTime.Now,
                        RoomRate = room.Price.Value
                    },

                BusinessAccounts = GetBusinessAccounts(null)
            };
            return model;
        }

        private IEnumerable<SelectListItem> GetMultiSelectRooms(IEnumerable<string> ids)
        {
            return _roomService.GetAll(HotelId)
                    .Where(x => (x.StatusId == (int)RoomStatusEnum.Vacant || x.StatusId == (int)RoomStatusEnum.Dirty) && ids.Contains(x.Id.ToString(CultureInfo.InvariantCulture)))
                    .Select(x => new SelectListItem { Text = x.RoomNumber, Value = x.Id.ToString(CultureInfo.InvariantCulture), Selected = true });
        }

        private IEnumerable<SelectListItem> GetBusinessAccounts(int? selectedId)
        {
            if (!selectedId.HasValue)
                selectedId = 0;

            var bas =
                _businessAccountService.GetAll(HotelId).Where(x => !x.Debtor).ToList();
            bas.Insert(0, new BusinessAccount { CompanyName = "-- Please Select --", Id = 0 });
            //return bas.Select(x => new SelectListItem { Text = x.CompanyName, Value = x.Id.ToString(), Selected = true });
            return bas.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString(), Selected = x.Id == selectedId });
        }

        
        [HttpPost]
        public ActionResult FilterByCategoryWaitressByName(string name, int? canaddItemInt, int? guestOrderId)
        {
            canaddItemInt = 0;

            var catyList = CategoryList.ToList();

            var catLstId = catyList.Select(x => x.Id);

            var lstRequest = _guestOrderService.GetAll("GuestRequestItems").FirstOrDefault(x => x.Id == guestOrderId.Value && x.IsActive).GuestRequestItems.ToList();

            HotelMenuModel hmm = new HotelMenuModel();

            hmm.RequestedItems = lstRequest.Where(x => x.IsActive).ToList();

            if (hmm.RequestedItems.All(x => x.RequestBy == User.Identity.Name))
            {
                canaddItemInt = 1;
            }

            if (string.IsNullOrEmpty(name))
            {
                int? distributionPointId = CashierDistributionPointId();

                var items = new List<MenuModel>();

                if (distributionPointId.HasValue)
                {
                    items = _posItemService.GetAllInclude("StockItem").Where(x => x.DistributionPointId == distributionPointId.Value && x.Remaining > 0).GroupBy(x => x.StockItem.CategoryId).Select(x => new MenuModel { Id = x.Key, CategoryName = GetCategoryName(x.Key, catyList), Items = x.ToList() }).ToList();
                }
                else
                {
                    items = _posItemService.GetAllInclude("StockItem").Distinct(new PosItemComparer()).Where(x => x.Remaining > 0).GroupBy(x => x.StockItem.CategoryId).Select(x => new MenuModel { Id = x.Key, CategoryName = GetCategoryName(x.Key, catyList), Items = x.ToList() }).ToList();
                }

                items = items.Where(x => catLstId.Contains(x.Id)).ToList();

                hmm.Menu = items;
                hmm.ItemsOrdered = new List<ItemModel>();
                hmm.CanaddItemInt = canaddItemInt.Value;
                hmm.GuestOrderId = guestOrderId.Value;

                if (canaddItemInt.HasValue && canaddItemInt.Value == 1)
                {
                    return PartialView("_MenuItemsRebindWaitress", hmm);
                }
                else
                {
                    return PartialView("_MenuItemsRebindWaitressNoAdd", hmm);
                }


            }
            else
            {
                int? distributionPointId = CashierDistributionPointId();

                var items = new List<MenuModel>();

                if (distributionPointId.HasValue)
                {
                    items = _posItemService.GetAllInclude("StockItem").Where(x => x.DistributionPointId == distributionPointId.Value && x.Remaining > 0 && x.StockItem.StockItemName.ToUpper().Contains(name.ToUpper())).GroupBy(x => x.StockItem.CategoryId).Select(x => new MenuModel { Id = x.Key, CategoryName = GetCategoryName(x.Key, catyList), Items = x.ToList() }).ToList();
                }
                else
                {
                    items = _posItemService.GetAllInclude("StockItem").Distinct(new PosItemComparer()).Where(x => x.Remaining > 0 && x.StockItem.StockItemName.ToUpper().Contains(name.ToUpper())).GroupBy(x => x.StockItem.CategoryId).Select(x => new MenuModel { Id = x.Key, CategoryName = GetCategoryName(x.Key, catyList), Items = x.ToList() }).ToList();
                }

                items = items.Where(x => catLstId.Contains(x.Id)).ToList();
             
                hmm.Menu = items;

                hmm.ItemsOrdered = new List<ItemModel>();

                hmm.CanaddItemInt = canaddItemInt.Value;
                hmm.GuestOrderId = guestOrderId.Value;


                if (canaddItemInt.HasValue && canaddItemInt.Value == 1)
                {
                    return PartialView("_MenuItemsRebindWaitress", hmm);
                }
                else
                {
                    return PartialView("_MenuItemsRebindWaitressNoAdd", hmm);
                }
            }

        }

        
        [HttpPost]
        public ActionResult FilterByCategorySelf(int? id, int? canaddItemInt, int? guestOrderId)
        {
            canaddItemInt = 0;

            var catyList = CategoryList.ToList();

            var catLstId = catyList.Select(x => x.Id);

            var lstRequest = _guestOrderService.GetAll("GuestRequestItems").FirstOrDefault(x => x.Id == guestOrderId.Value && x.IsActive).GuestRequestItems.ToList();

            HotelMenuModel hmm = new HotelMenuModel();

            hmm.RequestedItems = lstRequest.Where(x => x.IsActive).ToList();

            if (hmm.RequestedItems.All(x => x.RequestBy == User.Identity.Name))
            {
                canaddItemInt = 1;
            }

            if (id.Value == 0)
            {
                int? distributionPointId = CashierDistributionPointId();

                var items = new List<MenuModel>();

                if (distributionPointId.HasValue)
                {
                    items = _posItemService.GetAllInclude("StockItem").Where(x => x.DistributionPointId == distributionPointId.Value && x.Remaining > 0).GroupBy(x => x.StockItem.CategoryId).Select(x => new MenuModel { Id = x.Key, CategoryName = GetCategoryName(x.Key, catyList), Items = x.ToList() }).ToList();
                }
                else
                {
                    items = _posItemService.GetAllInclude("StockItem").Distinct(new PosItemComparer()).Where(x => x.Remaining > 0).GroupBy(x => x.StockItem.CategoryId).Select(x => new MenuModel { Id = x.Key, CategoryName = GetCategoryName(x.Key, catyList), Items = x.ToList() }).ToList();
                }

                items = items.Where(x => catLstId.Contains(x.Id)).ToList();

                hmm.Menu = items;
                hmm.ItemsOrdered = new List<ItemModel>();
                hmm.CanaddItemInt = canaddItemInt.Value;
                hmm.GuestOrderId = guestOrderId.Value;

                if (canaddItemInt.HasValue && canaddItemInt.Value == 1)
                {
                    return PartialView("_MenuItemsRebindSelf", hmm);
                }
                else
                {
                    return PartialView("_MenuItemsRebindSelfNoAdd", hmm);
                }


            }
            else
            {
                int? distributionPointId = CashierDistributionPointId();

                var items = new List<MenuModel>();

                if (distributionPointId.HasValue)
                {
                    items = _posItemService.GetAllInclude("StockItem").Where(x => x.DistributionPointId == distributionPointId.Value && x.Remaining > 0 && x.StockItem.CategoryId == id.Value).GroupBy(x => x.StockItem.CategoryId).Select(x => new MenuModel { Id = x.Key, CategoryName = GetCategoryName(x.Key, catyList), Items = x.ToList() }).ToList();
                }
                else
                {
                    items = _posItemService.GetAllInclude("StockItem").Distinct(new PosItemComparer()).Where(x => x.Remaining > 0 && x.StockItem.CategoryId == id.Value).GroupBy(x => x.StockItem.CategoryId).Select(x => new MenuModel { Id = x.Key, CategoryName = GetCategoryName(x.Key, catyList), Items = x.ToList() }).ToList();
                }

                items = items.Where(x => catLstId.Contains(x.Id)).ToList();

                hmm.Menu = items;
                hmm.ItemsOrdered = new List<ItemModel>();

                hmm.CanaddItemInt = canaddItemInt.Value;
                hmm.GuestOrderId = guestOrderId.Value;


                if (canaddItemInt.HasValue && canaddItemInt.Value == 1)
                {
                    return PartialView("_MenuItemsRebindSelf", hmm);
                }
                else
                {
                    return PartialView("_MenuItemsRebindSelfNoAdd", hmm);
                }
            }

        }
        [HttpPost]
        public ActionResult FilterByCategoryWaitress(int? id, int? canaddItemInt, int? guestOrderId)
        {
            canaddItemInt = 0;

            var catyList = CategoryList.ToList();

            var catLstId = catyList.Select(x => x.Id);

            var lstRequest = _guestOrderService.GetAll("GuestRequestItems").FirstOrDefault(x => x.Id == guestOrderId.Value && x.IsActive).GuestRequestItems.ToList();

            HotelMenuModel hmm = new HotelMenuModel();

            hmm.RequestedItems = lstRequest.Where(x => x.IsActive).ToList();

            if (hmm.RequestedItems.All(x => x.RequestBy == User.Identity.Name))
            {
                canaddItemInt = 1;
            }

            if (id.Value == 0)
            {
                int? distributionPointId = CashierDistributionPointId();

                var items = new List<MenuModel>();

                if (distributionPointId.HasValue)
                {
                    items = _posItemService.GetAllInclude("StockItem").Where(x => x.DistributionPointId == distributionPointId.Value && x.Remaining > 0).GroupBy(x => x.StockItem.CategoryId).Select(x => new MenuModel { Id = x.Key, CategoryName = GetCategoryName(x.Key, catyList), Items = x.ToList() }).ToList();
                }
                else
                {
                    items = _posItemService.GetAllInclude("StockItem").Distinct(new PosItemComparer()).Where(x => x.Remaining > 0).GroupBy(x => x.StockItem.CategoryId).Select(x => new MenuModel { Id = x.Key, CategoryName = GetCategoryName(x.Key, catyList), Items = x.ToList() }).ToList();
                }

                items = items.Where(x => catLstId.Contains(x.Id)).ToList();

                hmm.Menu = items;
                hmm.ItemsOrdered = new List<ItemModel>();
                hmm.CanaddItemInt = canaddItemInt.Value;
                hmm.GuestOrderId = guestOrderId.Value;

                if (canaddItemInt.HasValue && canaddItemInt.Value == 1)
                {
                    return PartialView("_MenuItemsRebindWaitress", hmm);
                }
                else
                {
                    return PartialView("_MenuItemsRebindWaitressNoAdd", hmm);
                }


            }
            else
            {
                int? distributionPointId = CashierDistributionPointId();

                var items = new List<MenuModel>();

                if (distributionPointId.HasValue)
                {
                    items = _posItemService.GetAllInclude("StockItem").Where(x => x.DistributionPointId == distributionPointId.Value && x.Remaining > 0 && x.StockItem.CategoryId == id.Value).GroupBy(x => x.StockItem.CategoryId).Select(x => new MenuModel { Id = x.Key, CategoryName = GetCategoryName(x.Key, catyList), Items = x.ToList() }).ToList();
                }
                else
                {
                    items = _posItemService.GetAllInclude("StockItem").Distinct(new PosItemComparer()).Where(x => x.Remaining > 0 && x.StockItem.CategoryId == id.Value).GroupBy(x => x.StockItem.CategoryId).Select(x => new MenuModel { Id = x.Key, CategoryName = GetCategoryName(x.Key, catyList), Items = x.ToList() }).ToList();
                }

                items = items.Where(x => catLstId.Contains(x.Id)).ToList();

                hmm.Menu = items;
                hmm.ItemsOrdered = new List<ItemModel>();

                hmm.CanaddItemInt = canaddItemInt.Value;
                hmm.GuestOrderId = guestOrderId.Value;


                if (canaddItemInt.HasValue && canaddItemInt.Value == 1)
                {
                    return PartialView("_MenuItemsRebindWaitress", hmm);
                }
                else
                {
                    return PartialView("_MenuItemsRebindWaitressNoAdd", hmm);
                }
            }

        }

         
        [HttpPost]
        public ActionResult FilterByCategoryMenuDealOnly(int? id)
        {
            var catyList = CategoryList.ToList();

            if (id.Value == 0)
            {
                var items = _posItemService.GetAllInclude("StockItem").Distinct(new PosItemComparer()).Where(x => x.StockItem.Discounted).GroupBy(x => x.StockItem.CategoryId).Select(x => new MenuModel { Id = x.Key, CategoryName = GetCategoryName(x.Key, catyList), Items = x.ToList() }).ToList();
                HotelMenuModel hmm = new HotelMenuModel();
                hmm.Menu = items;
                hmm.ItemsOrdered = new List<ItemModel>();
                return PartialView("_MenuItemsMenuDeal", hmm);

            }
            else
            {
                var items = _posItemService.GetAllInclude("StockItem").Distinct(new PosItemComparer()).Where(x => x.StockItem.CategoryId == id.Value && x.StockItem.Discounted).GroupBy(x => x.StockItem.CategoryId).Select(x => new MenuModel { Id = x.Key, CategoryName = GetCategoryName(x.Key, catyList), Items = x.ToList() }).ToList();
                HotelMenuModel hmm = new HotelMenuModel();
                hmm.Menu = items;
                hmm.ItemsOrdered = new List<ItemModel>();
                return PartialView("_MenuItemsMenuDeal", hmm);
            }
        }
        
        [HttpPost]
        public ActionResult FilterByCategoryMenuOnly(int? id)
        {
            var catyList = CategoryList.ToList();

            if (id.Value == 0)
            {
                var items = _posItemService.GetAllInclude("StockItem").Distinct(new PosItemComparer()).GroupBy(x => x.StockItem.CategoryId).Select(x => new MenuModel { Id = x.Key, CategoryName = GetCategoryName(x.Key, catyList), Items = x.ToList() }).ToList();
                HotelMenuModel hmm = new HotelMenuModel();
                hmm.Menu = items;
                hmm.ItemsOrdered = new List<ItemModel>();
                return PartialView("_MenuItemsMenu", hmm);

            }
            else
            {
                var items = _posItemService.GetAllInclude("StockItem").Distinct(new PosItemComparer()).Where(x => x.StockItem.CategoryId == id.Value).GroupBy(x => x.StockItem.CategoryId).Select(x => new MenuModel { Id = x.Key, CategoryName = GetCategoryName(x.Key, catyList), Items = x.ToList() }).ToList();
                HotelMenuModel hmm = new HotelMenuModel();
                hmm.Menu = items;
                hmm.ItemsOrdered = new List<ItemModel>();
                return PartialView("_MenuItemsMenu", hmm);
            }

        }
       
        [HttpPost]
        //[OutputCache(Duration = int.MaxValue, VaryByParam = "id")]
        public ActionResult FilterByCategoryNoAdd(int? id)
        {
            var catyList = CategoryList.ToList();
            if (id.Value == 0)
            {
                var items = _posItemService.GetAllInclude("StockItem").Distinct(new PosItemComparer()).GroupBy(x => x.StockItem.CategoryId).Select(x => new MenuModel { Id = x.Key, CategoryName = GetCategoryName(x.Key, catyList), Items = x.ToList() }).ToList();
                HotelMenuModel hmm = new HotelMenuModel();
                hmm.Menu = items;
                hmm.ItemsOrdered = new List<ItemModel>();
                return PartialView("_MenuItemsRebindNoAdd", hmm);

            }
            else
            {
                var items = _posItemService.GetAllInclude("StockItem").Distinct(new PosItemComparer()).Where(x => x.StockItem.CategoryId == id.Value).GroupBy(x => x.StockItem.CategoryId).Select(x => new MenuModel { Id = x.Key, CategoryName = GetCategoryName(x.Key, catyList), Items = x.ToList() }).ToList();
                HotelMenuModel hmm = new HotelMenuModel();
                hmm.Menu = items;
                hmm.ItemsOrdered = new List<ItemModel>();
                return PartialView("_MenuItemsRebindNoAdd", hmm);
            }

        }


        
        [HttpPost]
        //[OutputCache(Duration = int.MaxValue, VaryByParam = "id,canaddItemInt,guestOrderId")]
        public ActionResult FilterByCategoryCustomers(int? id, int? canaddItemInt, int? guestOrderId)
        {
            canaddItemInt = 0;

            var catyList = CategoryList.ToList();

            var lstRequest = _guestOrderService.GetAll("GuestRequestItems").FirstOrDefault(x => x.Id == guestOrderId.Value && x.IsActive).GuestRequestItems.ToList();
            HotelMenuModel hmm = new HotelMenuModel();
            hmm.RequestedItems = lstRequest.Where(x => x.IsActive).ToList();

            if (hmm.RequestedItems.All(x => x.RequestBy == User.Identity.Name))
            {
                canaddItemInt = 1;
            }


            if (id.Value == 0)
            {
                var items = _posItemService.GetAllInclude("StockItem").Distinct(new PosItemComparer()).GroupBy(x => x.StockItem.CategoryId).Select(x => new MenuModel { Id = x.Key, CategoryName = GetCategoryName(x.Key, catyList), Items = x.ToList() }).ToList();
                
                hmm.Menu = items;
                hmm.ItemsOrdered = new List<ItemModel>();

                hmm.CanaddItemInt = canaddItemInt.Value;
                hmm.GuestOrderId = guestOrderId.Value;

                if (canaddItemInt.HasValue && canaddItemInt.Value == 1)
                {
                    return PartialView("_MenuItemsCustomerRebind", hmm);
                }
                else
                {
                    return PartialView("_MenuItemsCustomerRebindNoAdd", hmm);
                }

            }
            else
            {
                var items = _posItemService.GetAllInclude("StockItem").Distinct(new PosItemComparer()).Where(x => x.StockItem.CategoryId == id.Value).GroupBy(x => x.StockItem.CategoryId).Select(x => new MenuModel { Id = x.Key, CategoryName = GetCategoryName(x.Key, catyList), Items = x.ToList() }).ToList();
             
                hmm.Menu = items;
                hmm.ItemsOrdered = new List<ItemModel>();

                hmm.CanaddItemInt = canaddItemInt.Value;
                hmm.GuestOrderId = guestOrderId.Value;

                if (canaddItemInt.HasValue && canaddItemInt.Value == 1)
                {

                    return PartialView("_MenuItemsCustomerRebind", hmm);
                }
                else
                {
                    return PartialView("_MenuItemsCustomerRebindNoAdd", hmm);
                }
            }
        }

        [HttpPost]
        //[OutputCache(Duration = int.MaxValue, VaryByParam = "id,canaddItemInt")]
        public ActionResult FilterByCategory(int? id, int? canaddItemInt)
        {
            var catyList = CategoryList.ToList();

            if(id.Value == 0)
            {
                var items = _posItemService.GetAllInclude("StockItem").Distinct(new PosItemComparer()).GroupBy(x => x.StockItem.CategoryId).Select(x => new MenuModel { Id = x.Key, CategoryName = GetCategoryName(x.Key, catyList), Items = x.ToList() }).ToList();
                HotelMenuModel hmm = new HotelMenuModel();
                hmm.Menu = items;
                hmm.ItemsOrdered = new List<ItemModel>();

                hmm.CanaddItemInt = canaddItemInt.Value;

                if (canaddItemInt.HasValue && canaddItemInt.Value == 1)
                {
                    return PartialView("_MenuItemsRebind", hmm);
                }
                else
                {
                    return PartialView("_MenuItemsRebindNoAdd", hmm);
                }

            }
            else
            {
                var items = _posItemService.GetAllInclude("StockItem").Distinct(new PosItemComparer()).Where(x => x.StockItem.CategoryId == id.Value).GroupBy(x => x.StockItem.CategoryId).Select(x => new MenuModel { Id = x.Key, CategoryName = GetCategoryName(x.Key, catyList), Items = x.ToList() }).ToList();
                HotelMenuModel hmm = new HotelMenuModel();
                hmm.Menu = items;
                hmm.ItemsOrdered = new List<ItemModel>(); 

                hmm.CanaddItemInt = canaddItemInt.Value;

                if (canaddItemInt.HasValue && canaddItemInt.Value == 1)
                {
                
                    return PartialView("_MenuItemsRebind", hmm);
                }
                else
                {
                    return PartialView("_MenuItemsRebindNoAdd", hmm);
                }
            }
        }

        
        [HttpPost]
        public ActionResult AlertWaiter(string id)
        {
            if(!string.IsNullOrEmpty(id))
            {
                GuestMessage gm = new GuestMessage();
                gm.IsActive = true;
                gm.Message = "Your attention is needed on table : " + id;
                gm.MessageDate = DateTime.Now;
                gm.TableName = id;
                _guestMessageService.Create(gm);
            }
            

            return Json(new { total = decimal.Zero }, JsonRequestBehavior.AllowGet);
        }

        
        [HttpPost]
        public ActionResult RemoveFromSelfRequest(int? id)
        {
            var item = _guestRequestItemService.GetById(id);

            var guestOrderId = 0;

            var canRemoveItem = 0;

            if (item != null && item.RequestBy == User.Identity.Name)
            {
                guestOrderId = item.GuestOrderId;
                _guestRequestItemService.Delete(item);
                canRemoveItem = 1;
            }
            else
            {
                return Json(new { total = decimal.Zero, CanRemoveItem = canRemoveItem }, JsonRequestBehavior.AllowGet);
            }

            var total = decimal.Zero;

            if (guestOrderId > 0)
            {
                var order = _guestOrderService.GetByIdWithItems(guestOrderId, "GuestRequestItems,GuestRequestItems.StockItem");

                if (order != null)
                {
                    total = (order.GuestRequestItems.Where(x => x.IsActive).Any() && order.GuestRequestItems.Where(x => x.IsActive).FirstOrDefault().StockItem.Discounted && order.GuestRequestItems.Where(x => x.IsActive).FirstOrDefault().StockItem.ClubPrice.HasValue && order.GuestRequestItems.Where(x => x.IsActive).FirstOrDefault().StockItem.ClubPrice.Value > 0) ? Decimal.Round(order.GuestRequestItems.Where(x => x.IsActive).Sum(x => x.StockItem.ClubPrice.Value), 2) : Decimal.Round(order.GuestRequestItems.Where(x => x.IsActive).Sum(x => x.Price), 2);

                }
            }

            return Json(new { total = total, CanRemoveItem = canRemoveItem }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult RemoveFromWaitressRequest(int? id)
        {
            var item = _guestRequestItemService.GetById(id);

            var guestOrderId = 0;

            var canRemoveItem = 0;

            if (item != null && item.RequestBy == User.Identity.Name)
            {
                guestOrderId = item.GuestOrderId;
                _guestRequestItemService.Delete(item);
                canRemoveItem = 1;
            }
            else
            {
                return Json(new { total = decimal.Zero, CanRemoveItem = canRemoveItem }, JsonRequestBehavior.AllowGet);
            }

            var total = decimal.Zero;

            if (guestOrderId > 0)
            {
                var order = _guestOrderService.GetByIdWithItems(guestOrderId, "GuestRequestItems,GuestRequestItems.StockItem");

                if (order != null)
                {
                    total = (order.GuestRequestItems.Where(x => x.IsActive).Any() && order.GuestRequestItems.Where(x => x.IsActive).FirstOrDefault().StockItem.Discounted && order.GuestRequestItems.Where(x => x.IsActive).FirstOrDefault().StockItem.ClubPrice.HasValue && order.GuestRequestItems.Where(x => x.IsActive).FirstOrDefault().StockItem.ClubPrice.Value > 0) ? Decimal.Round(order.GuestRequestItems.Where(x => x.IsActive).Sum(x => x.StockItem.ClubPrice.Value), 2) : Decimal.Round(order.GuestRequestItems.Where(x => x.IsActive).Sum(x => x.Price), 2);

                }
            }

            return Json(new { total = total, CanRemoveItem = canRemoveItem }, JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        public ActionResult RemoveFromCustomerRequest(int? id)
        {
            var item = _guestRequestItemService.GetById(id);

            var guestOrderId = 0;

            var canRemoveItem = 0;

            if (item != null)
            {
                guestOrderId = item.GuestOrderId;

                if (item.RequestBy == User.Identity.Name)
                {
                    _guestRequestItemService.Delete(item);
                    canRemoveItem = 1;
                }
                 
            }
            else
            {
                return Json(new { total = decimal.Zero, CanRemoveItem = canRemoveItem }, JsonRequestBehavior.AllowGet);
            }

            var total = decimal.Zero;

            if (guestOrderId > 0)
            {
                var order = _guestOrderService.GetByIdWithItems(guestOrderId, "GuestRequestItems");

                if (order != null)
                {
                    total = (order.GuestRequestItems.Where(x => x.IsActive).Any() && order.GuestRequestItems.Where(x => x.IsActive).FirstOrDefault().StockItem.Discounted && order.GuestRequestItems.Where(x => x.IsActive).FirstOrDefault().StockItem.ClubPrice.HasValue && order.GuestRequestItems.Where(x => x.IsActive).FirstOrDefault().StockItem.ClubPrice.Value > 0) ? Decimal.Round(order.GuestRequestItems.Where(x => x.IsActive).Sum(x => x.StockItem.ClubPrice.Value), 2) : Decimal.Round(order.GuestRequestItems.Where(x => x.IsActive).Sum(x => x.Price), 2);
                }
            }

            return Json(new { total = total, CanRemoveItem = canRemoveItem }, JsonRequestBehavior.AllowGet);
        }

        
        [HttpPost]
        public ActionResult RemoveFromGuestRequestBill(int? id)
        {
            var item = _guestOrderItemService.GetById(id);

            var guestOrderId = 0;

            if (item != null)
            {
                guestOrderId = item.GuestOrderId;
                _guestOrderItemService.Delete(item);
            }

            var total = decimal.Zero;

            if (guestOrderId > 0)
            {
                var order = _guestOrderService.GetByIdWithItems(guestOrderId, "GuestOrderItems,GuestOrderItems.StockItem");

                if (order != null)
                {
                    total = (order.GuestOrderItems.Where(x => x.IsActive).Any() && order.GuestOrderItems.Where(x => x.IsActive).FirstOrDefault().StockItem.Discounted && order.GuestOrderItems.Where(x => x.IsActive).FirstOrDefault().StockItem.ClubPrice.HasValue && order.GuestOrderItems.Where(x => x.IsActive).FirstOrDefault().StockItem.ClubPrice.Value > 0) ? Decimal.Round(order.GuestOrderItems.Where(x => x.IsActive).Sum(x => x.StockItem.ClubPrice.Value), 2) : Decimal.Round(order.GuestOrderItems.Where(x => x.IsActive).Sum(x => x.Price), 2);
                }

            }

            return Json(new { total = total }, JsonRequestBehavior.AllowGet);
        }

        
        [HttpPost]
        public ActionResult RemoveFromGuestOrder(int? id)
        {
            var item = _guestOrderItemService.GetById(id);

            var guestOrderId = 0;
            var canRemoveItem = 0;

            if (item != null)
            {
                guestOrderId = item.GuestOrderId;

                _guestOrderItemService.Delete(item);

                canRemoveItem = 1;
            }
            else
            {
                return Json(new { total = decimal.Zero, CanRemoveItem = canRemoveItem }, JsonRequestBehavior.AllowGet);
            }

            var total = decimal.Zero;

            if (guestOrderId > 0)
            {
                var order = _guestOrderService.GetByIdWithItems(guestOrderId, "GuestOrderItems,GuestOrderItems.StockItem");

                if (order != null)
                {
                    total = (order.GuestOrderItems.Where(x => x.IsActive).Any() && order.GuestOrderItems.Where(x => x.IsActive).FirstOrDefault().StockItem.Discounted && order.GuestOrderItems.Where(x => x.IsActive).FirstOrDefault().StockItem.ClubPrice.HasValue && order.GuestOrderItems.Where(x => x.IsActive).FirstOrDefault().StockItem.ClubPrice.Value > 0) ? Decimal.Round(order.GuestOrderItems.Where(x => x.IsActive).Sum(x => x.StockItem.ClubPrice.Value), 2) : Decimal.Round(order.GuestOrderItems.Where(x => x.IsActive).Sum(x => x.Price), 2);
                }
            }

            return Json(new { total = total, CanRemoveItem = canRemoveItem, guestOrderId = guestOrderId }, JsonRequestBehavior.AllowGet);
        }

        
        [HttpPost]
        public ActionResult RemoveFromGuestRequest(int? id)
        {
            var item = _guestRequestItemService.GetById(id);

            var guestOrderId = 0;
            var canRemoveItem = 0;

            if (item != null)
            {
                guestOrderId = item.GuestOrderId;

                if (item.RequestBy == User.Identity.Name)
                {
                    _guestRequestItemService.Delete(item);
                    canRemoveItem = 1;
                }

            }
            else
            {
                return Json(new { total = decimal.Zero, CanRemoveItem = canRemoveItem }, JsonRequestBehavior.AllowGet);
            }

            var total = decimal.Zero;

            if (guestOrderId > 0)
            {
                var order = _guestOrderService.GetByIdWithItems(guestOrderId, "GuestRequestItems,GuestRequestItems.StockItem");

                if (order != null)
                {
                    total = (order.GuestRequestItems.Where(x => x.IsActive).Any() && order.GuestRequestItems.Where(x => x.IsActive).FirstOrDefault().StockItem.Discounted && order.GuestRequestItems.Where(x => x.IsActive).FirstOrDefault().StockItem.ClubPrice.HasValue && order.GuestRequestItems.Where(x => x.IsActive).FirstOrDefault().StockItem.ClubPrice.Value > 0) ? Decimal.Round(order.GuestRequestItems.Where(x => x.IsActive).Sum(x => x.StockItem.ClubPrice.Value), 2) : Decimal.Round(order.GuestRequestItems.Where(x => x.IsActive).Sum(x => x.Price), 2);
                }
            }

            return Json(new { total = total, CanRemoveItem = canRemoveItem, guestOrderId = guestOrderId }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult PlaceOrderCustomerAjax(int? itemId, int? quantity, int? guestId, int? tableId, int? guestOrderId)
        {
            var personId = PersonId.Value;

            var guest = _guestService.GetAll(1).FirstOrDefault(x => x.PersonId == personId);

            var item = _stockItemService.GetById(itemId.Value);
            HotelMenuModel hmm = new HotelMenuModel();
            hmm.ItemModel = new ItemModel { Id = itemId.Value, Quantity = quantity.Value, StockItemName = item.StockItemName, Price = (item.UnitPrice * quantity.Value) };
            hmm.ItemsOrdered = new List<ItemModel> { hmm.ItemModel };

            var price = (decimal)(item.UnitPrice * quantity.Value);

            var lstRequest = _guestOrderService.GetAll("GuestRequestItems, GuestRequestItems.StockItem").FirstOrDefault(x => x.Id == guestOrderId && x.IsActive).GuestRequestItems.ToList();

            hmm.RequestedItems = lstRequest.Where(x => x.IsActive).ToList();

            if (hmm.RequestedItems.All(x => x.RequestBy == User.Identity.Name))
            {
                hmm.CanAddItems = true;
                hmm.CanaddItemInt = 1;
            }

            if (hmm.CanAddItems)
            {

                try
                {
                    var ri = _guestRequestItemService.Create(new GuestRequestItem
                    {
                        ItemId = itemId.Value,
                        Quantity = quantity.Value,
                        Price = price,
                        GuestOrderId = guestOrderId.Value,
                        CreatedDate = DateTime.Now,
                        Paid = false,
                        Confirmed = false,
                        Delivered = false,
                        IsActive = true,
                        StockItem = item,
                        RequestBy = User.Identity.Name
                    });

                    hmm.RequestedItems.Add(ri);
             
                }
                catch (Exception)
                {
                }
            }

            hmm.RequestedItems.ForEach(x => x.StockItem = GetStockItem(x.StockItem, x.ItemId));
            hmm.GuestId = guestId.Value;
            hmm.TableId = tableId.Value;
            hmm.GuestOrderId = guestOrderId.Value;


            return PartialView("_TableOrderByCustomer", hmm);
        }

        private StockItem GetStockItem(StockItem stockItem, int itemId)
        {
            if(stockItem == null)
            {
                var newItem = RealProductsList.FirstOrDefault(x => x.Id == itemId);
                return newItem;
            }

            return stockItem;
        }

        
        [HttpPost]
        public ActionResult CanThisPersonAddItem(int? guestOrderId)
        {
            var lstRequest = _guestOrderService.GetAll("GuestRequestItems").FirstOrDefault(x => x.Id == guestOrderId && x.IsActive).GuestRequestItems.ToList();
            HotelMenuModel hmm = new HotelMenuModel();
            hmm.RequestedItems = lstRequest.Where(x => x.IsActive).ToList();

            if (hmm.RequestedItems.All(x => x.RequestBy == User.Identity.Name))
            {
                hmm.CanAddItems = true;
                hmm.CanaddItemInt = 1;
            }


            return Json(new { CanaddItemInt = hmm.CanaddItemInt }, JsonRequestBehavior.AllowGet);
        }

        
        [HttpPost]
        public ActionResult PlaceOrderBySelfAjax(int? itemId, int? quantity, int? guestId, int? tableId, int? guestOrderId)
        {
            var item = _stockItemService.GetById(itemId.Value);

            HotelMenuModel hmm = new HotelMenuModel();

            hmm.ItemModel = new ItemModel { Id = itemId.Value, Quantity = quantity.Value, StockItemName = item.StockItemName, Price = (item.UnitPrice * quantity.Value) };

            hmm.ItemsOrdered = new List<ItemModel> { hmm.ItemModel };

            var allOrder = _guestOrderService.GetAll("GuestRequestItems,GuestRequestItems.StockItem");

            var price = (decimal)(item.UnitPrice * quantity.Value);

            var lstRequest = allOrder.FirstOrDefault(x => x.Id == guestOrderId && x.IsActive).GuestRequestItems.ToList();

            hmm.RequestedItems = lstRequest.Where(x => x.IsActive).ToList();

            var oldList = hmm.RequestedItems;

            if (hmm.RequestedItems.All(x => x.RequestBy == User.Identity.Name))
            {
                hmm.CanAddItems = true;
                hmm.CanaddItemInt = 1;
            }

            var newId = 0;

            if (hmm.CanAddItems)
            {

                try
                {
                    var ri = _guestRequestItemService.Create(new GuestRequestItem
                    {
                        ItemId = itemId.Value,
                        Quantity = quantity.Value,
                        Price = price,
                        GuestOrderId = guestOrderId.Value,
                        CreatedDate = DateTime.Now,
                        Paid = false,
                        Confirmed = false,
                        Delivered = false,
                        //StockItem = item,
                        IsActive = true,
                        RequestBy = User.Identity.Name
                    });

                    hmm.RequestedItems.Add(ri);
                    newId = ri.Id;

                }
                catch (Exception)
                {

                }
            }

            hmm.RequestedItems.ForEach(x => x.StockItem = GetStockItem(x.StockItem, x.ItemId));

            hmm.GuestId = guestId.Value;
            hmm.TableId = tableId.Value;
            hmm.GuestOrderId = guestOrderId.Value;

            return PartialView("_TableOrderBySelf", hmm);
        }


        [HttpPost]
        public ActionResult PlaceOrderByWaitressAjax(int? itemId, int? quantity, int? guestId, int? tableId, int? guestOrderId)
        {
            //var personId = PersonId.Value;

            //var guest = _guestService.GetAll(1).FirstOrDefault(x => x.PersonId == personId);

            var item = _stockItemService.GetById(itemId.Value);

            HotelMenuModel hmm = new HotelMenuModel();

            hmm.ItemModel = new ItemModel { Id = itemId.Value, Quantity = quantity.Value, StockItemName = item.StockItemName, Price  = (item.UnitPrice * quantity.Value) };

            hmm.ItemsOrdered = new List<ItemModel> { hmm.ItemModel };

            var allOrder = _guestOrderService.GetAll("GuestRequestItems,GuestRequestItems.StockItem");

            //var actualCustomerEmail = allOrder.FirstOrDefault(x => x.Id == guestOrderId.Value).Guest.Email;

            var price = (decimal)(item.UnitPrice * quantity.Value);

            var lstRequest = allOrder.FirstOrDefault(x => x.Id == guestOrderId && x.IsActive).GuestRequestItems.ToList();

            hmm.RequestedItems = lstRequest.Where(x => x.IsActive).ToList();

            var oldList = hmm.RequestedItems;

            if (hmm.RequestedItems.All(x => x.RequestBy == User.Identity.Name))
            {
                hmm.CanAddItems = true;
                hmm.CanaddItemInt = 1;
            }

            var newId = 0;

            if (hmm.CanAddItems)
            {

                try 
                {
                  var ri =  _guestRequestItemService.Create(new GuestRequestItem
                    {
                        ItemId = itemId.Value,
                        Quantity = quantity.Value,
                        Price = price,
                        GuestOrderId = guestOrderId.Value,
                        CreatedDate = DateTime.Now,
                        Paid = false,
                        Confirmed = false,
                        Delivered = false,
                        //StockItem = item,
                        IsActive = true,
                        RequestBy = User.Identity.Name
                    });

                  hmm.RequestedItems.Add(ri);
                  newId = ri.Id;

                }
                catch(Exception)
                {

                }
            }

            hmm.RequestedItems.ForEach(x => x.StockItem = GetStockItem(x.StockItem, x.ItemId));

            hmm.GuestId = guestId.Value;
            hmm.TableId = tableId.Value;
            hmm.GuestOrderId = guestOrderId.Value;

            return PartialView("_TableOrderByWaitress", hmm);
        }

        [OutputCache(Duration = int.MaxValue)]
        [HttpGet]
        public ActionResult MenuDeals()
        {
            var catyList = CategoryList.ToList();

            var items = _posItemService.GetAllInclude("StockItem").Distinct(new PosItemComparer()).Where(x => x.StockItem.Discounted).GroupBy(x => x.StockItem.CategoryId).Select(x => new MenuModel { Id = x.Key, CategoryName = GetCategoryName(x.Key, catyList), Items = x.ToList() }).ToList();

            HotelMenuModel hmm = new HotelMenuModel();
            hmm.Menu = items;
            hmm.ItemsOrdered = new List<ItemModel>();


            int catId = 0;

            var cats = catyList;

            var catNames = items.Select(x => x.CategoryName).Distinct();

            var categories = cats.Where(x => catNames.Contains(x.Name)).ToList();

            categories.Insert(0, new CategoryModel { Name = "-- All --", Id = 0 });

            IEnumerable<SelectListItem> selectList =
                from c in categories
                select new SelectListItem
                {
                    Selected = (c.Id == catId),
                    Text = c.Name,
                    Value = c.Id.ToString()
                };

            hmm.selectList = selectList;

            return View(hmm);
        }

        [HttpPost]
        [OutputCache(Duration = int.MaxValue, VaryByParam = "name")]
        public ActionResult FilterByCategoryInteractiveMenuOnlyByName(string name)
        {
            name = name.ToUpper();

            var catyList = CategoryList.ToList();

            if (string.IsNullOrEmpty(name))
            {
                var items = _posItemService.GetAllInclude("StockItem").Distinct(new PosItemComparer()).OrderBy(x => x.StockItem.StockItemName).GroupBy(x => x.StockItem.CategoryId).Select(x => new MenuModel { Id = x.Key, CategoryName = GetCategoryName(x.Key, catyList), Items = x.ToList() }).ToList();
                HotelMenuModel hmm = new HotelMenuModel();
                hmm.Menu = items;
                hmm.ItemsOrdered = new List<ItemModel>();
                return PartialView("_ViewInteractiveMenuNew", hmm);

            }
            else
            {
                var items = _posItemService.GetAllInclude("StockItem").Distinct(new PosItemComparer()).Where(x => x.StockItem.StockItemName.ToUpper().Contains(name)).OrderBy(x => x.StockItem.StockItemName).GroupBy(x => x.StockItem.CategoryId).Select(x => new MenuModel { Id = x.Key, CategoryName = GetCategoryName(x.Key, catyList), Items = x.ToList() }).ToList();
                HotelMenuModel hmm = new HotelMenuModel();
                hmm.Menu = items;
                hmm.ItemsOrdered = new List<ItemModel>();
                return PartialView("_ViewInteractiveMenuNew", hmm);
            }
        }

        [HttpPost]
        [OutputCache(Duration = int.MaxValue, VaryByParam = "name")]
        public ActionResult FilterByCategoryInteractiveRoomOnlyByName(string name)
        {
            name = name.ToUpper();

            if (string.IsNullOrEmpty(name))
            {
                var allRooms = _roomService.GetAll(1);

                HotelMenuModel hmm = new HotelMenuModel();

                var categories = allRooms.Select(x => x.RoomType1).ToList();

                categories.Insert(0, new RoomType { Name = "-- All --", Id = 0 });

                int catId = 0;

                IEnumerable<SelectListItem> selectList =
                    from c in categories
                    select new SelectListItem
                    {
                        Selected = (c.Id == catId),
                        Text = c.Name,
                        Value = c.Id.ToString()
                    };

                hmm.selectList = selectList;

                hmm.RoomsTypes = allRooms.GroupBy(x => x.RoomType1).Select(x => new LatestRoomViewModel { Description = x.ToList().Select(y => y.Description), Price = x.ToList().Select(y => y.Price.Value), RoomTypeName = x.Key.Name, PicturesList = GetPictures(x.Key.Name) }).ToList();


                return PartialView("_ViewInteractiveRoomNew", hmm);

            }
            else
            {
                var allRooms = _roomService.GetAll(1);

                HotelMenuModel hmm = new HotelMenuModel();

                var categories = allRooms.Select(x => x.RoomType1).ToList();

                categories.Insert(0, new RoomType { Name = "-- All --", Id = 0 });

                int catId = 0;

                IEnumerable<SelectListItem> selectList =
                    from c in categories
                    select new SelectListItem
                    {
                        Selected = (c.Id == catId),
                        Text = c.Name,
                        Value = c.Id.ToString()
                    };

                hmm.selectList = selectList;

                hmm.RoomsTypes = allRooms.Where(x => x.RoomType1.Name.ToUpper() == name).GroupBy(x => x.RoomType1).Select(x => new LatestRoomViewModel { Description = x.ToList().Select(y => y.Description), Price = x.ToList().Select(y => y.Price.Value), RoomTypeName = x.Key.Name, PicturesList = GetPictures(x.Key.Name) }).ToList();

                return PartialView("_ViewInteractiveRoomNew", hmm);
            }
        }

        

        [HttpPost]
        [OutputCache(Duration = int.MaxValue, VaryByParam = "id")]
        public ActionResult FilterByCategoryInteractiveRoomOnly(int? id)
        {
            if (id.Value == 0)
            {
                var allRooms = _roomService.GetAll(1);

                HotelMenuModel hmm = new HotelMenuModel();

                var categories = allRooms.Select(x => x.RoomType1).ToList();

                categories.Insert(0, new RoomType { Name = "-- All --", Id = 0 });

                int catId = 0;

                IEnumerable<SelectListItem> selectList =
                    from c in categories
                    select new SelectListItem
                    {
                        Selected = (c.Id == catId),
                        Text = c.Name,
                        Value = c.Id.ToString()
                    };

                hmm.selectList = selectList;

                hmm.RoomsTypes = allRooms.GroupBy(x => x.RoomType1).Select(x => new LatestRoomViewModel { Description = x.ToList().Select(y => y.Description), Price = x.ToList().Select(y => y.Price.Value), RoomTypeName = x.Key.Name, PicturesList = GetPictures(x.Key.Name) }).ToList();

               
                return PartialView("_ViewInteractiveRoomNew", hmm);

            }
            else
            {
                var allRooms = _roomService.GetAll(1);

                HotelMenuModel hmm = new HotelMenuModel();

                var categories = allRooms.Select(x => x.RoomType1).ToList();

                categories.Insert(0, new RoomType { Name = "-- All --", Id = 0 });

                int catId = 0;

                IEnumerable<SelectListItem> selectList =
                    from c in categories
                    select new SelectListItem
                    {
                        Selected = (c.Id == catId),
                        Text = c.Name,
                        Value = c.Id.ToString()
                    };

                hmm.selectList = selectList;

                hmm.RoomsTypes = allRooms.Where(x => x.RoomType1.Id == id.Value).GroupBy(x => x.RoomType1).Select(x => new LatestRoomViewModel { Description = x.ToList().Select(y => y.Description), Price = x.ToList().Select(y => y.Price.Value), RoomTypeName = x.Key.Name, PicturesList = GetPictures(x.Key.Name) }).ToList();

                return PartialView("_ViewInteractiveRoomNew", hmm);
            }
        }

        [HttpPost]
        [OutputCache(Duration = int.MaxValue, VaryByParam = "id")]
        public ActionResult FilterByCategoryInteractiveMenuOnly(int? id)
        {
            var catyList = CategoryList.ToList();

            if (id.Value == 0)
            {
                var items = _posItemService.GetAllInclude("StockItem").Distinct(new PosItemComparer()).OrderBy(x => x.StockItem.StockItemName).GroupBy(x => x.StockItem.CategoryId).Select(x => new MenuModel { Id = x.Key, CategoryName = GetCategoryName(x.Key, catyList), Items = x.ToList() }).ToList();
                HotelMenuModel hmm = new HotelMenuModel();
                hmm.Menu = items;
                hmm.ItemsOrdered = new List<ItemModel>();
                return PartialView("_ViewInteractiveMenuNew", hmm);

            }
            else
            {
                var items = _posItemService.GetAllInclude("StockItem").Distinct(new PosItemComparer()).Where(x => x.StockItem.CategoryId == id.Value).OrderBy(x => x.StockItem.StockItemName).GroupBy(x => x.StockItem.CategoryId).Select(x => new MenuModel { Id = x.Key, CategoryName = GetCategoryName(x.Key, catyList), Items = x.ToList() }).ToList();
                HotelMenuModel hmm = new HotelMenuModel();
                hmm.Menu = items;
                hmm.ItemsOrdered = new List<ItemModel>();
                return PartialView("_ViewInteractiveMenuNew", hmm);
            }
        }

        [OutputCache(Duration = int.MaxValue)]
        [HttpGet]
        public ActionResult PaperMenu()
        {
            HotelMenuModel hmm = new HotelMenuModel();
            return View(hmm);
        }

        public ActionResult IndexTelevision()
        {
            var tiService = new TableItemService();

            int p = 0;

            var tableItems = _tableItemService.GetAll("StockItem,BarTable").Where(x => x.StockItem.StarBuy)
                .OrderByDescending(x => x.DateSold).Select(x => new StarBuyModel { index = p++, PicturePath = x.StockItem.PicturePath, RealStockItemString = x.StockItem.StockItemName, RealTableString = x.BarTable.TableAlias }).ToList();

            var starBuys = POSService.StockItemService.GetStockItems(1).Where(x => x.StarBuy).Select(x => new StarBuyModel { index = p++,PicturePath = x.PicturePath, RealStockItemString = x.StockItemName, RealTableString = x.UnitPrice.ToString() }).ToList();

            tableItems.AddRange(starBuys);

            return View("IndexTelevision", new BaseViewModel
            {
                StarBuysTableItems = tableItems,
                Kitchenlist = null,
                HotelName = "", 
                   ExpiryDate = DateTime.Now, FutureReservationCount = 0, CheckinDate = DateTime.Today.Date, CheckoutDate = DateTime.Today.AddDays(1).Date });
        }


        
        [OutputCache(Duration = int.MaxValue)]
        [HttpGet]
        public ActionResult SelfServiceRooms()
        {
            var allRooms = _roomService.GetAll(1);

            HotelMenuModel hmm = new HotelMenuModel();

            var categories = allRooms.Select(x => x.RoomType1).ToList();

            categories = categories.Distinct(new RoomTypeComparer()).ToList();

            categories.Insert(0, new RoomType { Name = "-- All --", Id = 0 });

            int catId = 0;

            IEnumerable<SelectListItem> selectList =
                from c in categories
                select new SelectListItem
                {
                    Selected = (c.Id == catId),
                    Text = c.Name,
                    Value = c.Id.ToString()
                };

            hmm.selectList = selectList;

            hmm.RoomsTypes = allRooms.GroupBy(x => x.RoomType1).Select(x => new LatestRoomViewModel { Description = x.ToList().Select(y => y.Description), Price = x.ToList().Select(y => y.Price.Value), RoomTypeName = x.Key.Name, PicturesList = GetPictures(x.Key.Name) }).ToList();

            return View(hmm);
        }

        private List<string> GetPictures(string catName)
        {
            var path = Path.Combine(Server.MapPath("~/Products/Rooms/" + catName ));

            DirectoryInfo d = new DirectoryInfo(path);//Assuming Test is your Folder

            FileInfo[] Files = d.GetFiles("*.*"); //Getting Text files

            List<string> strList = new List<string>();

            foreach (FileInfo file in Files)
            {
                strList.Add(file.Name);
            }

            return strList;
        }

        [OutputCache(Duration = int.MaxValue)]
        [HttpGet]
        public ActionResult InteractiveMenu()
        {
            var catyList = CategoryList.ToList();

            var every = _posItemService.GetAllInclude("StockItem").ToList();

            var items = every.Distinct(new PosItemComparer()).OrderBy(x => x.StockItem.StockItemName).GroupBy(x => x.StockItem.CategoryId).Select(x => new MenuModel { Id = x.Key, CategoryName = GetCategoryName(x.Key, catyList), Items = x.ToList() }).ToList();

            HotelMenuModel hmm = new HotelMenuModel();

            hmm.ItemsOrdered = new List<ItemModel>();

            int catId = 0;

            var cats = catyList;

            var catNames = items.Select(x => x.CategoryName).Distinct();

            var categories = cats.Where(x => catNames.Contains(x.Name)).ToList();

            if (cats.Any())
                _startCategoryID = cats.LastOrDefault().Id;

            categories.Insert(0, new CategoryModel { Name = "-- All --", Id = 0 });

            IEnumerable<SelectListItem> selectList =
                from c in categories
                select new SelectListItem
                {
                    Selected = (c.Id == catId),
                    Text = c.Name,
                    Value = c.Id.ToString()
                };

            hmm.selectList = selectList;

            var itemsToOnlyDisplayFoodInitially = every.Where(x => x.StockItem.CookedFood).Distinct(new PosItemComparer()).OrderBy(x => x.StockItem.StockItemName).GroupBy(x => x.StockItem.CategoryId).Select(x => new MenuModel { Id = x.Key, CategoryName = GetCategoryName(x.Key, catyList), Items = x.ToList() }).ToList();

            hmm.Menu = itemsToOnlyDisplayFoodInitially.ToList();

            return View(hmm);
        }

        [OutputCache(Duration = int.MaxValue)]
        [HttpGet]
        public ActionResult Menu()
        {
            var catyList = CategoryList.ToList();

            var items = _posItemService.GetAllInclude("StockItem").Distinct(new PosItemComparer()).GroupBy(x => x.StockItem.CategoryId).Select(x => new MenuModel { Id = x.Key, CategoryName = GetCategoryName(x.Key, catyList), Items = x.ToList() }).ToList();
           
            HotelMenuModel hmm = new HotelMenuModel();
            hmm.Menu = items;
            hmm.ItemsOrdered = new List<ItemModel>();


            int catId = 0;

            var cats = catyList;

            var catNames = items.Select(x => x.CategoryName).Distinct();

            var categories = cats.Where(x => catNames.Contains(x.Name)).ToList();

            categories.Insert(0, new CategoryModel { Name = "-- All --", Id = 0 });

            IEnumerable<SelectListItem> selectList =
                from c in categories
                select new SelectListItem
                {
                    Selected = (c.Id == catId),
                    Text = c.Name,
                    Value = c.Id.ToString()
                };

            hmm.selectList = selectList;
            return View(hmm);
        }


        [HttpPost]
        public ActionResult GoToOrder(int? id)
        {
            var personId = PersonId.Value;

            int? tableId = null;

            int? guestId = null;

            var guestOrder = _guestOrderService.GetAll("GuestOrderItems").FirstOrDefault(x => x.Id == id && x.IsActive);

            tableId = guestOrder.TableId;

            guestId = guestOrder.GuestId;


            if (guestOrder != null)
            {
                guestOrder.GuestOrderItems.ForEach(x => x.Delivered = true);
                guestOrder.GuestOrderItems.ForEach(x => x.Confirmed = true);
                guestOrder.GuestOrderItems.ForEach(x => x.IsActive = false);
                guestOrder.GuestOrderItems.ForEach(x => x.Paid = false);
                guestOrder.GuestOrderItems.ForEach(x => x.WaitreesCanSee = true);



                foreach (var item in guestOrder.GuestOrderItems)
                {
                    guestOrder.GuestBillItems.Add(new GuestBillItem { CreatedDate = DateTime.Now, IsActive = true, ItemId = item.ItemId, Price = item.Price, Quantity = item.Quantity, Paid = false, Confirmed = false, Delivered = true });
                }

                _guestOrderService.Update(guestOrder);
            }

            return RedirectToAction("ViewWaitressTableProcessed", new { id = tableId, guestId = guestId });
        }

        
        [HttpGet]
        public ActionResult WaitressBills()
        {
            var personId = PersonId.Value;

            var guestGroupByTablesAllBill = _guestOrderItemService.GetAllInclude("GuestOrder,GuestOrder.Guest,StockItem").Where(x => x.IsActive && x.GuestOrder.PersonId == personId).GroupBy(x => x.GuestOrder)
                .Select(x => new GuestOrderModel { Id = x.Key.Id, ActualGuest = x.Key.Guest, BillItems = x.ToList(), ActualGuestBill = x.Key }).ToList();

            HotelMenuModel hmm = new HotelMenuModel();

            hmm.AllOpenBillsGroupBy = guestGroupByTablesAllBill;

            return View(hmm);
        }


        [HttpGet]
        public ActionResult UnusedTables()
        {
            var personId = PersonId.Value;

            var guestGroupByTablesAll = _guestOrderService.GetAll("GuestRequestItems,GuestOrderItems,BarTable,Guest").Where(x => x.GuestOrderItems.Count == 0 && x.GuestRequestItems.Count == 0).ToList();
           
            HotelMenuModel hmm = new HotelMenuModel();

            hmm.UnusedTables = guestGroupByTablesAll;

            return View(hmm);
        }


        //
        [HttpGet]
        public ActionResult AllOrdersEmpty()
        {
            var guestGroupByTablesAll = _guestOrderService.GetAll("Guest,BarTable,BarTable.Person").Where(x => x.IsActive && x.GuestOrderItems.Count == 0).GroupBy(x => x.BarTable).Select(x => new GuestOrderModel { Id = x.Key.GuestOrders.FirstOrDefault().Id, ActualGuest = x.Key.Guest, RealTable = x.Key }).ToList();

            HotelMenuModel hmm = new HotelMenuModel();

            hmm.AllOpenOrdersGroupBy = guestGroupByTablesAll;

            return View(hmm);
        }

        
        [HttpGet]
        public ActionResult AllOrders(int? tableId)
        {
            if (tableId.HasValue)
            {
                var guestGroupByTablesAll = _guestOrderItemService.GetAllInclude("GuestOrder,GuestOrder.Guest,StockItem,GuestOrder.BarTable,GuestOrder.BarTable.Person")
               .Where(x => x.IsActive && x.GuestOrder.TableId == tableId.Value).GroupBy(x => x.GuestOrder).Select(x => new GuestOrderModel { Id = x.Key.Id, ActualGuest = x.Key.Guest, OrderItems = x.ToList(), ActualGuestOrder = x.Key }).ToList();

                HotelMenuModel hmm = new HotelMenuModel();

                hmm.AllOpenOrdersGroupBy = guestGroupByTablesAll;

                return View(hmm);
            }
            else
            {
                var guestGroupByTablesAll = _guestOrderItemService.GetAllInclude("GuestOrder,GuestOrder.Guest,StockItem,GuestOrder.BarTable,GuestOrder.BarTable.Person")
                .Where(x => x.IsActive).GroupBy(x => x.GuestOrder).Select(x => new GuestOrderModel { Id = x.Key.Id, ActualGuest = x.Key.Guest, OrderItems = x.ToList(), ActualGuestOrder = x.Key }).ToList();

                HotelMenuModel hmm = new HotelMenuModel();

                hmm.AllOpenOrdersGroupBy = guestGroupByTablesAll;

                return View(hmm);
            }
            

           
        }

        
        [HttpGet]
        public ActionResult RequestBill()
        {
            var personId = PersonId.Value;

            var guestGroupByTablesAll = _guestOrderItemService.GetAllInclude("GuestOrder,GuestOrder.Guest,StockItem,GuestOrder.BarTable").Where(x => x.IsActive && x.GuestOrder.PersonId == personId).GroupBy(x => x.GuestOrder).Select(x => new GuestOrderModel { Id = x.Key.Id, ActualGuest = x.Key.Guest,  OrderItems = x.ToList(), ActualGuestOrder = x.Key }).ToList();

            HotelMenuModel hmm = new HotelMenuModel();

            hmm.AllOpenOrdersGroupBy = guestGroupByTablesAll;

            return View(hmm);
        }

        [HttpGet]
        public ActionResult WaitressOrders()
        {
            var personId = PersonId.Value;

            var guestGroupByTablesAll = _guestRequestItemService.GetAllInclude("GuestOrder,GuestOrder.Guest,StockItem,GuestOrder.BarTable").Where(x => x.IsActive && x.RequestBy == User.Identity.Name).GroupBy(x => x.GuestOrder).Select(x => new GuestOrderModel { Id = x.Key.Id, ActualGuest = x.Key.Guest, Items = x.ToList(), ActualGuestOrder = x.Key }).ToList();
          
            HotelMenuModel hmm = new HotelMenuModel();

            hmm.AllOpenOrdersGroupBy = guestGroupByTablesAll;

            return View(hmm);
        }

        
        [HttpGet]
        public ActionResult CreateNewUserSendText(string telNumber)
        {
            var msg = "To see your bill, please connect to IONITV on your Wifi hotspots, then go to ionitv.mshome.net on your browser, Enter your user name and password. Click on Bill.";

            string realTelephone = "234" + telNumber.Substring(1);

            int count = msg.Length;

            SendSMS(realTelephone, "", msg);

            return Json(new { CreatedNewUser = 1 }, JsonRequestBehavior.AllowGet);

        }

        
        public int? CreateNewUserByCashier(string username, string telNumber)
        {
            var allPersons = _personService.GetAll(HotelID).Select(x => x.Email).ToList();

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

            var person = CreateNewPerson(username, telNumber);

            if (person != null)
            {
                var openTablePersonIds = _barTableService.GetAllInclude("Guest").Select(x => x.Guest.PersonId).ToList();

                var persons = _personService.GetAll(HotelID).Where(x => !openTablePersonIds.Contains(x.PersonID) && x.PersonTypeId == (int)PersonTypeEnum.Guest && x.Email.ToUpper() != "GUEST").ToList();

                var guestIds = persons.SelectMany(x => x.Guests.Select(y => y.Id)).ToList();

                persons.Reverse();

                var dropDownList = new SelectList(persons, "PersonId", "Email", person.PersonID);

               return person.PersonID;
            }
            else
            {
                return null;
            }

        }

        [HttpGet]
        public ActionResult CreateNewUser(string username, string telNumber)
        {
            var allPersons = _personService.GetAll(HotelID).Select(x => x.Email).ToList();

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

           if(!canCreateNewUser)
           {
               return Json(new { CreatedNewUser = 0, Username =  username }, JsonRequestBehavior.AllowGet);
           }

           var person = CreateNewPerson(username, telNumber);

            if(person != null)
            {
                var openTablePersonIds = _barTableService.GetAllInclude("Guest").Select(x => x.Guest.PersonId).ToList();

                var persons = _personService.GetAll(HotelID).Where(x => !openTablePersonIds.Contains(x.PersonID) && x.PersonTypeId == (int)PersonTypeEnum.Guest && x.Email.ToUpper() != "GUEST").ToList();

                var guestIds = persons.SelectMany(x => x.Guests.Select(y => y.Id)).ToList();

                persons.Reverse();

                var dropDownList = new SelectList(persons, "PersonId", "Email", person.PersonID);

                //var msg = "To see your bill pls connect to IONITV on your Wifi, then go to ionitv.mshome.net on your browser. Your username is :" + person.Username + " , Password is : " + person.Password + " , Click on Bill.";
                
                //string realTelephone = "234" + telNumber.Substring(1);

                //int count = msg.Length;

                //SendSMS(realTelephone, "", msg);

                return Json(new { DropDownList = dropDownList, Username = username, CreatedNewUser = 1 }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { CreatedNewUser = 0, Username = username }, JsonRequestBehavior.AllowGet);
            }
            
        }


        private Person CreateNewPerson(string username, string telephone)
        {
            var person = new Person();

            person.HotelId = HotelID;
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

            var guest = new Guest();
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

            person.HotelId = HotelID;
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

            var guest = new Guest();
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

        private void ActivateGameAccount(Person person)
        {
            UnitOfWork unitOfWork = new UnitOfWork();

            var currentUser = new Entitities.User();

            currentUser.UserName = person.Username;
            currentUser.Password = person.Password;
            currentUser.Email = person.Email;
            currentUser.CreatedBy = "SA";
            currentUser.CreatedDate = DateTime.Now;
            currentUser.CurrentStake = 0;
            currentUser.CurrentUser = false;
            currentUser.ForfitChanceToPlay = true;
            currentUser.GameName = string.Empty;
            currentUser.IsActive = true;
            currentUser.IsComputer = false;
            currentUser.IsGameController = false;
            currentUser.LastLoggedInDate = DateTime.Now;
            currentUser.LoosingMessage = string.Empty;
            currentUser.Male = true;
            currentUser.ModifiedDate = DateTime.Now;
            currentUser.ModifiedBy = "SA";
            currentUser.PlayBalance = 10000;
            currentUser.playingNow = false;
            currentUser.playingSeq = 0;
            currentUser.PrevUserName = person.Username;
            currentUser.RealMoneyBalance = 10000;
            currentUser.ShowFinish = 0;
            currentUser.StartDate = DateTime.Now;
            currentUser.Status = "LIVE";
            currentUser.UserBalance = 10000;
            currentUser.UserPictureName = "user_anonymous.png";
            currentUser.WinningMessage = string.Empty;
            currentUser.PrevUserName = Guid.NewGuid().ToString();
            //unitOfWork.UserRepository.Insert(currentUser);
            unitOfWork.Save();
        }

        
        [HttpGet]
        public ActionResult SwapStaff(bool? sendMessage, int? tableId, int? guestId)
        {
            HotelMenuModel hmm = new HotelMenuModel();

            var guestTablesAll = _guestOrderService.GetAll("Guest,BarTable,Person").Where(x => x.IsActive).ToList();

            var waiters = guestTablesAll.Select(x => x.Person).Distinct().ToList();

            var allWaiters = _personService.GetAll(1).Where(x => x.PersonTypeId == (int)PersonTypeEnum.SalesAssistant || x.PersonTypeId == (int)PersonTypeEnum.Cashier  ||x.PersonTypeId == (int)PersonTypeEnum.Bartender).ToList();

            hmm.CanSelectListPersons = GetAvailablePersonsStaff(null, waiters);

            hmm.CanSelectListPersonsExistingTable = hmm.CanSelectListPersons;

            hmm.AllSelectListWaiters = GetAvailablePersonsStaff(null, allWaiters);

            return View(hmm);
        }

        [HttpGet]
        public ActionResult SwapTables(bool? sendMessage, int? tableId, int? guestId)
        {
            var personId = PersonId.Value;

            var tableNumbers = "1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50";

            try
            {
                tableNumbers = System.Configuration.ConfigurationManager.AppSettings["TableNumbers"].ToString();
            }
            catch
            {
                tableNumbers = "1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50";
            }

            var tables = tableNumbers.Split(',').ToList();

            var guestTablesAll = _guestOrderService.GetAll("Guest,BarTable").Where(x => x.IsActive && x.PersonId == PersonId.Value).ToList();

            var guestTables = guestTablesAll.Where(x => !x.PersonId.HasValue).ToList();

            //var tablesWithCustomers = guestTables.Select(x => x.TableId.ToString()).ToList();//.Distinct(new GuestOrderComparer())

            var tablesWithCustomers = guestTables.Select(x => x.BarTable.TableId).ToList();

            //var availableTables = guestTables.Select(x => x.TableId.ToString()).ToList();

            var availableTables = guestTables.Select(x => x.BarTable.TableId.ToString()).ToList();

            HotelMenuModel hmm = new HotelMenuModel();

            hmm.AvailableTables = availableTables;

            hmm.MyTables = guestTablesAll.Where(x => x.PersonId == personId).ToList();

            hmm.AvailableTablesSelectList = GetAvailableTables(null, availableTables);

            var noneTable = guestTablesAll.Select(x => x.BarTable).ToList();

            var openTables = tables.ToList();

            var personIds = guestTablesAll.Select(x => x.Guest.PersonId).ToList();

            hmm.CanSelectList = GetAvailableTables(null, openTables);

            hmm.CanSelectListExistingTable = GetAvailableTablesNew(null, noneTable);

            hmm.CanSelectListPersons = GetAvailablePersons(null, personIds);

            hmm.CanSelectListPersonsExistingTable = hmm.CanSelectListPersons;

            return View(hmm);
        }

        [HttpGet]
        public ActionResult WaitressTablesCashier(bool? sendMessage, int? tableId, int? guestId)
        {
            var personId = PersonId.Value;

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

            HotelMenuModel hmm = new HotelMenuModel();

            hmm.AvailableTables = availableTables;

            hmm.MyTables = guestTablesAll.Where(x => x.PersonId == personId).ToList();

            hmm.AvailableTablesSelectList = GetAvailableTables(null, availableTables);

            var noneTable = guestTablesAll.Distinct(new GuestOrderComparer()).Select(x => x.BarTable.TableId.ToString()).ToList();

            var openTables = tables.Except(noneTable).ToList();

            var personIds = guestTablesAll.Select(x => x.Guest.PersonId).ToList();

            hmm.CanSelectList = GetAvailableTables(null, openTables);

            hmm.CanSelectListExistingTable = GetAvailableTables(null, noneTable);

            hmm.CanSelectListPersons = GetAvailablePersons(null, personIds);

            hmm.CanSelectListPersonsExistingTable = hmm.CanSelectListPersons;

            return PartialView("_TableOpeningSelect",hmm);
        }

        
        [HttpGet]
        public ActionResult WaitressTables(bool? sendMessage, int? tableId, int? guestId)
        {
            var personId = PersonId.Value;

            var tableNumbers = "1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50";

            try
            {
                tableNumbers = System.Configuration.ConfigurationManager.AppSettings["TableNumbers"].ToString();
            }
            catch
            {
                tableNumbers = "1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50";
            }

            var tables = tableNumbers.Split(',').ToList();

            var guestTablesAll = _guestOrderService.GetAll("Guest,BarTable").Where(x => x.IsActive).ToList();

            var guestTables = guestTablesAll.Where(x => !x.PersonId.HasValue).ToList();

            //var tablesWithCustomers = guestTables.Select(x => x.TableId.ToString()).ToList();//.Distinct(new GuestOrderComparer())

            var tablesWithCustomers = guestTables.Select(x => x.BarTable.TableId).ToList();

            //var availableTables = guestTables.Select(x => x.TableId.ToString()).ToList();

            var availableTables = guestTables.Select(x => x.BarTable.TableId.ToString()).ToList();

            HotelMenuModel hmm = new HotelMenuModel();

            hmm.AvailableTables = availableTables;

            hmm.MyTables = guestTablesAll.Where(x => x.PersonId == personId).ToList();

            hmm.AvailableTablesSelectList = GetAvailableTables(null, availableTables);

            var noneTable = guestTablesAll.Distinct( new GuestOrderComparer()).Select(x => x.BarTable.TableId.ToString()).ToList();

            var openTables = tables.Except(noneTable).ToList();

            var personIds = guestTablesAll.Select(x => x.Guest.PersonId).ToList();

            hmm.CanSelectList = GetAvailableTables(null, openTables);

            hmm.CanSelectListExistingTable = GetAvailableTables(null, noneTable);

            hmm.CanSelectListPersons = GetAvailablePersons(null, personIds);

            //CanSelectListExistingTable

            hmm.CanSelectListPersonsExistingTable = hmm.CanSelectListPersons;

            if (sendMessage.HasValue && tableId.HasValue && guestId.HasValue)
            {
                var guest = _guestService.GetById(guestId.Value);

                hmm.SendFinalConfirmation = 1;

                if (guest.Person == null)
                {
                    hmm.GuestCredentials = guest.Email;
                }
                else
                {
                    hmm.GuestCredentials = guest.Person.Email;
                }

                hmm.TableId = tableId.Value;
            }


            return View(hmm);
        }

        [HttpGet]
        public ActionResult InviteGuest(int? id)
        {
            var personId = PersonId.Value;

            var thisPerson = _personService.GetById(personId);

            var owner = thisPerson.Guests.FirstOrDefault(x => x.PersonId == personId);

            var stranger = _guestChatService.GetAll().FirstOrDefault(x => x.GuestId == id.Value).Guest;

            var model = new HotelMenuModel();

            var chatMessagesWhereIAmOwner = _guestChatMessageService.GetAll().Where(x => x.StrangerId == stranger.Id && x.OwnerId == owner.Id).ToList();

            var chatMessagesWhereIAmStranger = _guestChatMessageService.GetAll().Where(x => x.StrangerId == owner.Id && x.OwnerId == stranger.Id).ToList();

            GuestChatMessage gcm = new GuestChatMessage();

            int sendInvite = 0;

            var invitedGuest = "";

            if (!chatMessagesWhereIAmOwner.Any() && !chatMessagesWhereIAmStranger.Any())
            {
                gcm.OwnerId = owner.Id;
                gcm.DateCreated = DateTime.Now;
                gcm.Message = "Do you mind joing me for a chat?";
                gcm.StrangerId = stranger.Id;
                gcm.OwnerId = owner.Id;
                gcm.MessageBy = User.Identity.Name;
                gcm.PicturePath = owner.Person.PicturePath;
                _guestChatMessageService.Create(gcm);
                model.Owner = owner;
                sendInvite = 1;
                model.Stranger = stranger;
                invitedGuest = stranger.Email;
            }
            else
            {
                if (chatMessagesWhereIAmOwner.Any())
                {
                    var fod = chatMessagesWhereIAmOwner.FirstOrDefault();
                    model.Owner = fod.Guest;
                    model.Stranger = fod.Guest1;
                }
                else
                {
                    var fod = chatMessagesWhereIAmStranger.FirstOrDefault();
                    model.Owner = fod.Guest;
                    model.Stranger = fod.Guest1;
                }
            }

            model.Messages = chatMessagesWhereIAmOwner.Where(x => !string.IsNullOrEmpty(x.Message)).ToList();

            model.Messages.AddRange(chatMessagesWhereIAmStranger.Where(x => !string.IsNullOrEmpty(x.Message)).ToList());

            model.Messages = model.Messages.OrderByDescending(x => x.DateCreated).ToList();

            model.NewMember = 0;

            model.Joining = 1;

            model.SendInvite = sendInvite;

            model.InvitedGuest = invitedGuest;
           

            return View("Chat", model);
        }

        public ActionResult AcceptGuest(int? id)
        {
            var personId = PersonId.Value;

            var stranger = _guestService.GetAll(1).FirstOrDefault(x => x.PersonId == personId);

            var owner = _guestChatService.GetAll().FirstOrDefault(x => x.GuestId == id.Value).Guest;

            var model = new HotelMenuModel();

            var chatMessagesWhereIAmStranger = _guestChatMessageService.GetAll().Where(x => x.StrangerId == stranger.Id && x.OwnerId == owner.Id && x.MessageBy == User.Identity.Name).ToList();

            if (chatMessagesWhereIAmStranger.Count == 0)
            {
                GuestChatMessage gcm = new GuestChatMessage();
                gcm.OwnerId = owner.Id;
                gcm.DateCreated = DateTime.Now;
                gcm.Message = "I have accepted to chat with you..";
                gcm.StrangerId = stranger.Id;
                gcm.OwnerId = owner.Id;
                gcm.MessageBy = User.Identity.Name;
                gcm.PicturePath = stranger.Person.PicturePath;
                _guestChatMessageService.Create(gcm);

            }
         

            model.NewMember = 0;

            model.Joining = 1;

            model.SendInvite = 0;

            model.InvitedGuest = "";

            model.AcceptedToChat = 1;

            model.Messages = _guestChatMessageService.GetAll().Where(x => x.StrangerId == stranger.Id && x.OwnerId == owner.Id).OrderByDescending(x => x.DateCreated).ToList();

            model.Owner = owner;

            model.Stranger = stranger;

            var allPendingOffers = _guestChatMessageService.GetAll().Where(x => x.StrangerId == stranger.Id).ToList();

            var gBy = allPendingOffers.GroupBy(x => x.Guest1).Select(x => new EmployeeGroupByModel { MsgList = x.ToList(), Guest = x.Key }).ToList();

            var allPending = gBy.Where(x => x.MsgList.Count > 1).Select(x => x.Guest).ToList();

            model.PendingRequests = allPending.Where(x => x.Id != stranger.Id).ToList();

           
            return View("Chat", model);
        }

        [HttpGet]
        public ActionResult ChatRoom()
        {
            var personId = PersonId.Value;

            var guest = _guestService.GetAll(1).FirstOrDefault(x => x.PersonId == personId);

            var InitialAll = _guestChatService.GetAll().Where(x => x.IsActive).ToList();

            var gcExisting = InitialAll.FirstOrDefault(x => x.GuestId == guest.Id);

            var newMember = 0;

            GuestChat gc = new GuestChat();

            if(gcExisting == null)
            {
                gc.CreatedDate = DateTime.Now;

                gc.IsActive = true;

                gc.GuestId = guest.Id;

                _guestChatService.Create(gc);

                newMember = 1;
            }
            else
            {
                gc = gcExisting;
                newMember = 2;
            }

            var allChatters = InitialAll;

            var model = new HotelMenuModel();

            model.GuestChatId = gc.Id;

            model.PicturePath = guest.Person.PicturePath;

            model.AllChatters = allChatters.Where(x => x.GuestId != guest.Id).ToList();

            var allPendingOffers = _guestChatMessageService.GetAll().Where(x => x.StrangerId == guest.Id).ToList();

            var gBy = allPendingOffers.GroupBy(x => x.Guest1).Select(x => new EmployeeGroupByModel { MsgList = x.ToList(), Guest = x.Key }).ToList();

            var allPending = gBy.Where(x => x.MsgList.Count > 1).Select(x => x.Guest).ToList();

            model.PendingRequests = allPending.Where(x => x.Id != guest.Id).ToList();

            model.NewMember = newMember;

            return View(model);
        }


        [HttpGet]
        public ActionResult Chat()
        {
            return View(new HotelMenuModel());
        }

        
        [HttpGet]
        public ActionResult SelfService()
        {
             var personId = PersonId.Value;

             var catyList = CategoryList.ToList();

            var everything = _guestOrderService.GetAll("Guest,GuestRequestItems,GuestRequestItems.StockItem,GuestOrderItems,GuestOrderItems.StockItem,BarTable");

            var guest = _guestService.GetAll(1).FirstOrDefault(x => x.PersonId == personId);

            if (guest == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if(guest != null && (guest.GuestRooms.Count == 0))
            {
                return RedirectToAction("Index","Home");
            }

            var existingGuestOrder = everything.FirstOrDefault(x => x.IsActive && x.GuestId == guest.Id);

            if(existingGuestOrder == null)
            {
                try
                {
                    var barTable = new BarTable();
                    barTable.GuestId = guest.Id;
                    barTable.IsActive = true;
                    barTable.StaffId = _AdminID; //ADMIN
                    barTable.TableAlias = "Room " + guest.GuestRooms.FirstOrDefault().RoomNumber;
                    barTable.TableId = 1000 + guest.Id;
                    barTable.TableName = barTable.TableAlias;
                    barTable.CreatedDate = DateTime.Now;
                    var newBarTable = _barTableService.Create(barTable);

                    if (newBarTable.Id > 0)
                    {
                        existingGuestOrder = new GuestOrder();
                        existingGuestOrder.TableId = newBarTable.Id;
                        existingGuestOrder.GuestId = guest.Id;
                        existingGuestOrder.PreparedByWaitress = false;
                        existingGuestOrder.PersonId = 2;
                        existingGuestOrder.Paid = false;
                        existingGuestOrder.Name = guest.FullName;
                        existingGuestOrder.IsActive = true;
                        existingGuestOrder.CreatedDate = DateTime.Now;
                        _guestOrderService.Create(existingGuestOrder);

                        everything = _guestOrderService.GetAll("Guest,GuestRequestItems,GuestRequestItems.StockItem,GuestOrderItems,GuestOrderItems.StockItem,BarTable");
                    }
                }
                catch
                {

                }
            }

            var allOrders = everything;

            var guestTables = allOrders.Where(x => x.IsActive).ToList();

            var guestTable = guestTables.LastOrDefault(x => x.IsActive && x.GuestId == guest.Id);

            HotelMenuModel hmm = new HotelMenuModel();

            var items = _posItemService.GetAllInclude("StockItem").Distinct(new PosItemComparer()).GroupBy(x => x.StockItem.CategoryId).Select(x => new MenuModel { Id = x.Key, CategoryName = GetCategoryName(x.Key, catyList), Items = x.ToList() }).ToList();

            hmm.Menu = items;

            hmm.TableId = guestTable.TableId;
            hmm.GuestOrderId = guestTable.Id;
            hmm.GuestId = guest.Id;


            var lst = allOrders.FirstOrDefault(x => x.Id == guestTable.Id && x.IsActive).GuestOrderItems.ToList();

            var lstRequest = allOrders.FirstOrDefault(x => x.Id == guestTable.Id && x.IsActive).GuestRequestItems.ToList();

            hmm.RequestedItems = lstRequest.Where(x => x.IsActive).ToList();

            hmm.GuestOrderItems = lst.Where(x => x.IsActive).ToList();

            hmm.TabOrderModelList = hmm.GuestOrderItems.GroupBy(x => x.ItemId).Select(y => new BarAndRestaurantMate.Models.TabOrderModel { Items = y.Take(1).ToList(), Quantity = y.ToList().Sum(z => z.Quantity), FullPrice = GetFullPrice(y.ToList()) }).ToList();

            if (hmm.RequestedItems.All(x => x.RequestBy == User.Identity.Name))
            {
                hmm.CanAddItems = true;
                hmm.CanaddItemInt = 1;
            }

            int catId = 0;

            var cats = catyList;

            var catNames = items.Select(x => x.CategoryName).Distinct();

            var categories = cats.Where(x => catNames.Contains(x.Name)).ToList();

            categories.Insert(0, new CategoryModel { Name = "-- All --", Id = 0 });

            IEnumerable<SelectListItem> selectList =
                from c in categories
                select new SelectListItem
                {
                    Selected = (c.Id == catId),
                    Text = c.Name,
                    Value = c.Id.ToString()
                };

            hmm.selectList = selectList;

            if (guestTable != null)
            {
                if (!guestTable.PreparedByWaitress)
                {
                    hmm.CanSeeProcessButton = true;
                }
            }

            hmm.WaitressId = guestTable.PersonId.Value;

            hmm.TableAlias = guestTable.BarTable.TableName;

            return View("SignalROrderSelf", hmm);
        }



        public ActionResult GetMyOrders()
        {
            var allOrders = _guestOrderService.GetAll("Guest,GuestRequestItems,GuestRequestItems.StockItem,GuestOrderItems,GuestOrderItems.StockItem,BarTable");

            var personId = PersonId.Value;

            var guest = _guestService.GetAll(1).FirstOrDefault(x => x.PersonId == personId);

            var guestTables = allOrders.Where(x => x.IsActive).ToList();

            var guestTable = guestTables.LastOrDefault(x => x.IsActive && x.GuestId == guest.Id);

            HotelMenuModel hmm = new HotelMenuModel();

            if (guestTable == null)
            {
                return PartialView("_NoPurchaseMade");
            }
            else
            {
                if(!guestTable.GuestOrderItems.Any())
                {
                   return PartialView("_NoPurchaseMade");
                }
                var guestOrderId = guestTable.GuestOrderItems.LastOrDefault().GuestOrderId;
                KitchenInfoRepository objRepo = new KitchenInfoRepository();
                var info = objRepo.GetData(guestOrderId);

                hmm.TableId = guestTable.TableId;
                hmm.GuestOrderId = guestTable.Id;
                hmm.GuestId = guest.Id;


                var lst = allOrders.FirstOrDefault(x => x.Id == guestTable.Id && x.IsActive).GuestOrderItems.ToList();
                var lstRequest = allOrders.FirstOrDefault(x => x.Id == guestTable.Id && x.IsActive).GuestRequestItems.ToList();

                hmm.RequestedItems = lstRequest.Where(x => x.IsActive).ToList();

                hmm.GuestOrderItems = lst.Where(x => x.IsActive).ToList();

                hmm.TabOrderModelList = hmm.GuestOrderItems.GroupBy(x => x.ItemId).Select(y => new BarAndRestaurantMate.Models.TabOrderModel { Items = y.Take(1).ToList(), Quantity = y.ToList().Sum(z => z.Quantity), FullPrice = GetFullPrice(y.ToList()) }).ToList();


                hmm.TableAlias = guestTable.BarTable.TableName;

                return PartialView("_ViewMenuResponsiveOrder", hmm);
            }
        }


        [HttpGet]
        public ActionResult MyOrder()
        {
            return View("SignalROrderNotify");
        }

        
        [HttpGet]
        public ActionResult MyOrderB4()
        {
            var catyList = CategoryList.ToList();

            var allOrders = _guestOrderService.GetAll("Guest,GuestRequestItems,GuestRequestItems.StockItem,GuestOrderItems,GuestOrderItems.StockItem,BarTable");

            var personId = PersonId.Value;

            var guest = _guestService.GetAll(1).FirstOrDefault(x => x.PersonId == personId);

            var guestTables = allOrders.Where(x => x.IsActive).ToList();

            var guestTable = guestTables.LastOrDefault(x => x.IsActive && x.GuestId == guest.Id);

            HotelMenuModel hmm = new HotelMenuModel();

            if(guestTable == null)
            {
                var tableNumbers = "1,2,3,4,5,6,7,8,9,10";

                try
                {
                    tableNumbers = System.Configuration.ConfigurationManager.AppSettings["TableNumbers"].ToString();
                }
                catch
                {
                    tableNumbers = "1,2,3,4,5,6,7,8,9,10";
                }

                var tables = tableNumbers.Split(',').ToList();
                var tablesWithCustomers = guestTables.Select(x => x.TableId.ToString()).ToList();
                var availableTables = tables.Except(tablesWithCustomers).ToList();
                hmm.AvailableTables = availableTables;
                hmm.AvailableTablesSelectList = GetAvailableTables(null, availableTables);
                hmm.AwaitingATable = 1;

                return View("TableSecurity", hmm);
            }

            var items = _posItemService.GetAllInclude("StockItem").Distinct(new PosItemComparer()).GroupBy(x => x.StockItem.CategoryId).Select(x => new MenuModel { Id = x.Key, CategoryName = GetCategoryName(x.Key, catyList), Items = x.ToList() }).ToList();

            hmm.Menu = items;

            hmm.TableId = guestTable.TableId;
            hmm.GuestOrderId = guestTable.Id;
            hmm.GuestId = guest.Id;

            
            var lst = allOrders.FirstOrDefault(x => x.Id == guestTable.Id && x.IsActive).GuestOrderItems.ToList();
            //var lstBill = allOrders.FirstOrDefault(x => x.Id == guestTable.Id && x.IsActive).GuestBillItems.ToList();
            var lstRequest = allOrders.FirstOrDefault(x => x.Id == guestTable.Id && x.IsActive).GuestRequestItems.ToList();

            hmm.RequestedItems = lstRequest.Where(x => x.IsActive).ToList();

            hmm.GuestOrderItems = lst.Where(x => x.IsActive).ToList();

            hmm.TabOrderModelList = hmm.GuestOrderItems.GroupBy(x => x.ItemId).Select(y => new BarAndRestaurantMate.Models.TabOrderModel { Items = y.Take(1).ToList(), Quantity = y.ToList().Sum(z => z.Quantity), FullPrice = GetFullPrice(y.ToList()) }).ToList();


            if (hmm.RequestedItems.All(x => x.RequestBy == User.Identity.Name))
            {
                hmm.CanAddItems = true;
                hmm.CanaddItemInt = 1;
            }

            int catId = 0;

            var cats = catyList;

            var catNames = items.Select(x => x.CategoryName).Distinct();

            var categories = cats.Where(x => catNames.Contains(x.Name)).ToList();

            categories.Insert(0, new CategoryModel { Name = "-- All --", Id = 0 });

            IEnumerable<SelectListItem> selectList =
                from c in categories
                select new SelectListItem
                {
                    Selected = (c.Id == catId),
                    Text = c.Name,
                    Value = c.Id.ToString()
                };

            hmm.selectList = selectList;

            if (guestTable != null)
            {
                if(!guestTable.PreparedByWaitress)
                {
                    hmm.CanSeeProcessButton = true;
                }
            }

            hmm.WaitressId = guestTable.PersonId.Value;

            hmm.TableAlias = guestTable.BarTable.TableName;

            return View("SignalROrderNotify",hmm);
        }


        
        [HttpPost]
        public ActionResult OpenTableExistingByCashier(int? existingTableId, string guestName)
        {
            int? existingTableMemberId = null;

            existingTableMemberId = CreateNewUserByCashier(guestName, "");

            if (!existingTableId.HasValue || !existingTableMemberId.HasValue)
            {
                return RedirectToAction("Index", "Pos");
            }

            int? newlyCreatedTable = null;

            var guest = _guestService.GetAll(HotelID).FirstOrDefault(x => x.PersonId == existingTableMemberId.Value);

            if (guest == null)
            {
                return RedirectToAction("WaitressTables");
            }

            var guestOrder = new GuestOrder();

            var barTable = _barTableService.GetAll().FirstOrDefault(x => x.GuestId == guest.Id);

            BarTable newBarTable = null;

            if (barTable == null)
            {
                var bt = new BarTable();
                bt.GuestId = guest.Id;
                bt.IsActive = true;
                bt.TableAlias = "Table " + existingTableId.Value.ToString() + "_" + guest.Email;
                bt.TableId = existingTableId.Value;
                bt.TableName = existingTableId.Value.ToString() + "_" + guest.Email;
                bt.StaffId = PersonId.Value;
                bt.CreatedDate = DateTime.Now;
                bt = _barTableService.Create(bt);
                newBarTable = bt;
                existingTableId = bt.Id;
            }
            else
            {
                newBarTable = barTable;
                existingTableId = barTable.Id;
            }

            var waitressId = PersonId;

            guestOrder.CreatedDate = DateTime.Now;
            guestOrder.GuestId = guest.Id;
            guestOrder.IsActive = true;
            guestOrder.Name = guest.Email + "_" + existingTableId.Value.ToString();
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
                    var existingGuestTable = allGuestOrders.FirstOrDefault(x => x.IsActive && x.GuestId == guest.Id);

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
                        newlyCreatedTable = 1;
                    }
                }
                else
                {
                    guestTable = _guestOrderService.Create(guestOrder);
                    newlyCreatedTable = 1;
                }
            }
            else
            {
                guestTable = exist;
            }

            return RedirectToAction("IndexNew", "Pos", new { tableId = existingTableId });

        }

        [HttpPost]
        public ActionResult OpenTableExisting(int? existingTableId, int? existingTableMemberId)
        {
            var catyList = CategoryList.ToList();

            if (!existingTableId.HasValue || !existingTableMemberId.HasValue)
            {
                return RedirectToAction("WaitressTables");
            }

            int? newlyCreatedTable = null;

            var guest = _guestService.GetAll(HotelID).FirstOrDefault(x => x.PersonId == existingTableMemberId.Value);

            if (guest == null)
            {
                return RedirectToAction("WaitressTables");
            }

            var guestOrder = new GuestOrder();

            var barTable = _barTableService.GetAll().FirstOrDefault(x => x.GuestId == guest.Id);

            BarTable newBarTable = null;

            if (barTable == null)
            {
                var bt = new BarTable();
                bt.GuestId = guest.Id;
                bt.IsActive = true;
                bt.TableAlias = "Table " +  existingTableId.Value.ToString() + "_" + guest.Email;
                bt.TableId = existingTableId.Value;
                bt.TableName = existingTableId.Value.ToString() + "_" + guest.Email;
                bt.StaffId = PersonId.Value;
                bt.CreatedDate = DateTime.Now;
                bt = _barTableService.Create(bt);
                newBarTable = bt;
                existingTableId = bt.Id;
            }
            else
            {
                newBarTable = barTable;
                existingTableId = barTable.Id;
            }

            //var personId = guest.PersonId;
            var waitressId = PersonId;

            guestOrder.CreatedDate = DateTime.Now;
            guestOrder.GuestId = guest.Id;
            guestOrder.IsActive = true;
            guestOrder.Name = guest.Email + "_" + existingTableId.Value.ToString();
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
                    var existingGuestTable = allGuestOrders.FirstOrDefault(x => x.IsActive && x.GuestId == guest.Id);

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
                        newlyCreatedTable = 1;
                    }
                }
                else
                {
                    guestTable = _guestOrderService.Create(guestOrder);
                    newlyCreatedTable = 1;
                }
            }
            else
            {
                guestTable = exist;
            }

            HotelMenuModel hmm = new HotelMenuModel();

            int? distributionPointId = CashierDistributionPointId();

            var menuItems = _posItemService.GetAllInclude("StockItem").ToList();

            var items = new List<MenuModel>();

            if (distributionPointId.HasValue)
            {
                items = menuItems.Where(x => x.DistributionPointId == distributionPointId.Value && x.Remaining > 0).GroupBy(x => x.StockItem.CategoryId).Select(x => new MenuModel { Id = x.Key, CategoryName = GetCategoryName(x.Key, catyList), Items = x.ToList() }).ToList();
            }
            else
            {
                items = menuItems.Distinct(new PosItemComparer()).GroupBy(x => x.StockItem.CategoryId).Select(x => new MenuModel { Id = x.Key, CategoryName = GetCategoryName(x.Key, catyList), Items = x.ToList() }).ToList();
            }

            //var items = _posItemService.GetAllInclude("StockItem").Distinct(new PosItemComparer()).GroupBy(x => x.StockItem.CategoryId).Select(x => new MenuModel { Id = x.Key, CategoryName = GetCategoryName(x.Key, catyList), Items = x.ToList() }).ToList();

            //hmm.Menu = items;

            hmm.TableId = guestTable.TableId;

            hmm.GuestOrderId = guestTable.Id;

            hmm.GuestId = guestTable.GuestId;

            var lst = guestTable.GuestOrderItems.ToList();

            var lstRequest = guestTable.GuestRequestItems.ToList();

            hmm.RequestedItems = lstRequest.Where(x => x.IsActive).ToList();

            hmm.GuestOrderItems = lst.Where(x => x.IsActive).ToList();

            hmm.TabOrderModelList = hmm.GuestOrderItems.GroupBy(x => x.ItemId).Select(y => new BarAndRestaurantMate.Models.TabOrderModel { Items = y.Take(1).ToList(), Quantity = y.ToList().Sum(z => z.Quantity), FullPrice = GetFullPrice(y.ToList()) }).ToList();


            if (lstRequest.All(x => x.RequestBy == User.Identity.Name))
            {
                hmm.CanAddItems = true;
                hmm.CanaddItemInt = 1;
            }

            if (newlyCreatedTable.HasValue)
                hmm.NewlyCreatedTable = newlyCreatedTable.Value;

            int catId = 0;

            var cats = catyList;

            if (cats.Any())
                _startCategoryID = cats.LastOrDefault().Id;

            var catNames = items.Select(x => x.CategoryName).Distinct();

            var categories = cats.Where(x => catNames.Contains(x.Name)).ToList();

            categories.Insert(0, new CategoryModel { Name = "-- All --", Id = 0 });

            if (guest.Person == null)
            {
                hmm.GuestCredentials = guest.Email;
            }
            else
            {
                hmm.GuestCredentials = guest.Person.Email;
            }

            IEnumerable<SelectListItem> selectList =
                from c in categories
                select new SelectListItem
                {
                    Selected = (c.Id == catId),
                    Text = c.Name,
                    Value = c.Id.ToString()
                };

            hmm.selectList = selectList;

            hmm.WaitressId = PersonId.Value;

            hmm.ProcessingComplete = 0;

            hmm.IamAWaitress = 1;

            hmm.JustOpenedTable = 1;

            hmm.TableAlias = newBarTable.TableAlias;

            hmm.Menu = items.Where(x => x.Id == _startCategoryID).ToList();

            return View("ViewWaitressTable", hmm);
        }

        
        [HttpPost]
        public ActionResult SwapExistingStaff(int? existingStaffId, int? toThisStaff)
        {
            var guestOrders = _guestOrderService.GetAll("").Where(x => x.PersonId == existingStaffId.Value).Select(x => x.Id).ToList();
            var staff = _personService.GetById(toThisStaff.Value);

            foreach(var go in guestOrders)
            {
                var guestOrder = _guestOrderService.GetByIdWithItems(go, "GuestRequestItems,BarTable,BarTable.TableItems");
                guestOrder.BarTable.TableItems.ForEach(x => x.Cashier = toThisStaff.Value);
                guestOrder.PersonId = toThisStaff.Value;
                guestOrder.BarTable.StaffId = toThisStaff.Value;
                guestOrder.GuestRequestItems.ForEach(x => x.RequestBy = staff.Email);
                _guestOrderService.Update(guestOrder);
            }

            var barTables = _barTableService.GetAll().Where(x => x.TableId == 0 && x.StaffId == existingStaffId.Value).ToList();

            foreach(var t in barTables)
            {
                t.StaffId = toThisStaff.Value;
                t.TableItems.ForEach(x => x.Cashier = toThisStaff.Value);
                _barTableService.Update(t);
            }

            return RedirectToAction("Index","Home");
        }

        [HttpPost]
        public ActionResult SwapExistingTable(int? realtableId, int? existingTableId)
        {
            var guestOrders = _guestOrderService.GetAll("BarTable,BarTable.Guest,BarTable.TableItems").ToList();
            var guestToSwap = guestOrders.FirstOrDefault(x => x.TableId == existingTableId.Value);
            var existingTable = guestToSwap.BarTable;

            //BarTable br = new BarTable { GuestId = existingTable.GuestId, IsActive = true, StaffId = existingTable.StaffId, TableName = "",  };

            int? newTableId = null;

            if (existingTable != null)
            {
                var bt = new BarTable();
                bt.GuestId = existingTable.GuestId;
                bt.IsActive = true;
                bt.TableAlias = "Table " +  realtableId.Value.ToString() + "_" + existingTable.Guest.Email;
                bt.TableId = realtableId.Value;
                bt.TableName = realtableId.Value.ToString() + "_" + existingTable.Guest.Email;
                bt.StaffId = existingTable.StaffId;
                bt.CreatedDate = DateTime.Now;
                bt = _barTableService.Create(bt);
                newTableId = bt.Id;
            }

            if(newTableId.HasValue)
            {
                var tableItems = guestToSwap.BarTable.TableItems.Select(x => x.Id).ToList();

                foreach (var ti in tableItems)
                {
                    var tableItem = _tableItemService.GetById(ti);
                    tableItem.TableId = newTableId.Value;
                    _tableItemService.Update(tableItem);
                }

                guestToSwap.TableId = newTableId.Value;
                _guestOrderService.Update(guestToSwap);

                var tableToDelete = _barTableService.GetById(existingTable.Id);
                _barTableService.Delete(tableToDelete);
            }

            return RedirectToAction("WaitressTables");
        }

        
        [HttpPost]
        public ActionResult OpenTableByCashier(int? realtableId, string guestName)
        {

            int? memberId = CreateNewUserByCashier(guestName, "");

            if (!realtableId.HasValue || !memberId.HasValue)
            {
                return RedirectToAction("Index","Pos");
            }

            var guest = _guestService.GetAll(HotelID).FirstOrDefault(x => x.PersonId == memberId.Value);

            var guestOrder = new GuestOrder();

            var barTable = _barTableService.GetAll().FirstOrDefault(x => x.GuestId == guest.Id);

            BarTable newBarTable = null;

            if (barTable == null)
            {
                var bt = new BarTable();
                bt.GuestId = guest.Id;
                bt.IsActive = true;
                bt.TableAlias = "Table " +  realtableId.Value.ToString() + "_" + guest.Email;
                bt.TableId = realtableId.Value;
                bt.TableName = realtableId.Value.ToString() + "_" + guest.Email;
                bt.StaffId = PersonId.Value;
                bt.CreatedDate = DateTime.Now;
                bt = _barTableService.Create(bt);
                newBarTable = bt;
                realtableId = bt.Id;
            }
            else
            {
                newBarTable = barTable;
                realtableId = barTable.Id;
            }

            //var personId = guest.PersonId;
            var waitressId = PersonId;

            guestOrder.CreatedDate = DateTime.Now;
            guestOrder.GuestId = guest.Id;
            guestOrder.IsActive = true;
            guestOrder.Name = guest.Email + "_" + realtableId.Value.ToString();
            guestOrder.Paid = false;
            guestOrder.PersonId = waitressId;
            guestOrder.TableId = realtableId.Value;
            guestOrder.PreparedByWaitress = true;

            var allGuestOrders = _guestOrderService.GetAll("GuestRequestItems,GuestRequestItems.StockItem").ToList();

            GuestOrder guestTable = null;

            var exist = allGuestOrders.FirstOrDefault(x => x.IsActive && x.TableId == realtableId.Value);

            if (exist == null)
            {
                if (memberId.HasValue)
                {
                    var existingGuestTable = allGuestOrders.FirstOrDefault(x => x.IsActive && x.GuestId == guest.Id);

                    if (existingGuestTable != null)
                    {
                        existingGuestTable.TableId = realtableId.Value;
                        existingGuestTable.PersonId = waitressId;
                        _guestOrderService.Update(existingGuestTable);
                        guestTable = existingGuestTable;
                    }
                    else
                    {
                        guestTable = _guestOrderService.Create(guestOrder);
                    }
                }
                else
                {
                    guestTable = _guestOrderService.Create(guestOrder);
                }
            }

            return RedirectToAction("IndexNew", "Pos", new { tableId = realtableId });

        }

        [HttpPost]
        public ActionResult OpenTable(int? realtableId, int? memberId)
        {
            var catyList = CategoryList.ToList();

            if(!realtableId.HasValue || !memberId.HasValue)
            {
                return RedirectToAction("WaitressTables");
            }

            int? newlyCreatedTable = null;

            var guest = _guestService.GetAll(HotelID).FirstOrDefault(x => x.PersonId == memberId.Value);

            if(guest == null)
            {
                return RedirectToAction("WaitressTables");
            }

            var guestOrder = new GuestOrder();

            var barTable = _barTableService.GetAllInclude("Person").FirstOrDefault(x => x.GuestId == guest.Id && x.IsActive);

            if(barTable != null && barTable.Person.PersonID != PersonId)
            {
                return RedirectToAction("WaitressTables");
            }

            BarTable newBarTable = null;

            if(barTable == null)
            {
                var bt = new BarTable();
                bt.GuestId = guest.Id;
                bt.IsActive = true;
                bt.TableAlias = "Table " + realtableId.Value.ToString() + "_" + guest.Email;
                bt.TableId = realtableId.Value;
                bt.TableName = realtableId.Value.ToString() + "_" + guest.Email;
                bt.StaffId = PersonId.Value;
                bt.CreatedDate = DateTime.Now;
                bt = _barTableService.Create(bt);
                newBarTable = bt;
                realtableId = bt.Id;
            }
            else
            {
                newBarTable = barTable;
                realtableId = barTable.Id;
            }

            //var personId = guest.PersonId;
            var waitressId = PersonId;

            guestOrder.CreatedDate = DateTime.Now;
            guestOrder.GuestId = guest.Id;
            guestOrder.IsActive = true;
            guestOrder.Name = guest.Email + "_" + realtableId.Value.ToString();
            guestOrder.Paid = false;
            guestOrder.PersonId = waitressId;
            guestOrder.TableId = realtableId.Value;
            guestOrder.PreparedByWaitress = true;

            var allGuestOrders = _guestOrderService.GetAll("GuestRequestItems,GuestRequestItems.StockItem").ToList();

            GuestOrder guestTable = null;

            var exist = allGuestOrders.FirstOrDefault(x => x.IsActive && x.TableId == realtableId.Value);

            if (exist == null)
            {
                if (memberId.HasValue)
                {
                    var existingGuestTable = allGuestOrders.FirstOrDefault(x => x.IsActive && x.GuestId == guest.Id);

                    if (existingGuestTable != null)
                    {
                        existingGuestTable.TableId = realtableId.Value;
                        existingGuestTable.PersonId = waitressId;
                        _guestOrderService.Update(existingGuestTable);
                        guestTable = existingGuestTable;
                    }
                    else
                    {
                        guestTable = _guestOrderService.Create(guestOrder);
                        newlyCreatedTable = 1;
                    }
                }
                else
                {
                    guestTable = _guestOrderService.Create(guestOrder);
                    newlyCreatedTable = 1;
                }
            }
            else
            {
                guestTable = exist;
            }

            HotelMenuModel hmm = new HotelMenuModel();

            int? distributionPointId = CashierDistributionPointId();

            var menuItems = _posItemService.GetAllInclude("StockItem").ToList();

            var items = new List<MenuModel>();

            if (distributionPointId.HasValue)
            {
                items = menuItems.Where(x => x.DistributionPointId == distributionPointId.Value && x.Remaining > 0).GroupBy(x => x.StockItem.CategoryId).Select(x => new MenuModel { Id = x.Key, CategoryName = GetCategoryName(x.Key, catyList), Items = x.ToList() }).ToList();
            }
            else
            {
                items = menuItems.Distinct(new PosItemComparer()).GroupBy(x => x.StockItem.CategoryId).Select(x => new MenuModel { Id = x.Key, CategoryName = GetCategoryName(x.Key, catyList), Items = x.ToList() }).ToList();
            }

            //var items = _posItemService.GetAllInclude("StockItem").Distinct(new PosItemComparer()).GroupBy(x => x.StockItem.CategoryId).Select(x => new MenuModel { Id = x.Key, CategoryName = GetCategoryName(x.Key, catyList), Items = x.ToList() }).ToList();

            hmm.TableId = guestTable.TableId;

            hmm.GuestOrderId = guestTable.Id;

            hmm.GuestId = guestTable.GuestId;

            var lst = guestTable.GuestOrderItems.ToList();

            var lstRequest = guestTable.GuestRequestItems.ToList();

            hmm.RequestedItems = lstRequest.Where(x => x.IsActive).ToList();

            hmm.GuestOrderItems = lst.Where(x => x.IsActive).ToList();

            hmm.TabOrderModelList = hmm.GuestOrderItems.GroupBy(x => x.ItemId).Select(y => new BarAndRestaurantMate.Models.TabOrderModel { Items = y.Take(1).ToList(), Quantity = y.ToList().Sum(z => z.Quantity), FullPrice = GetFullPrice(y.ToList()) }).ToList();


            if (lstRequest.All(x => x.RequestBy == User.Identity.Name))
            {
                hmm.CanAddItems = true;
                hmm.CanaddItemInt = 1;
            }

            if (newlyCreatedTable.HasValue)
                hmm.NewlyCreatedTable = newlyCreatedTable.Value;

            int catId = 0;

            var cats = catyList;

            if (cats.Any())
                _startCategoryID = cats.LastOrDefault().Id;

            var catNames = items.Select(x => x.CategoryName).Distinct();

            var categories = cats.Where(x => catNames.Contains(x.Name)).ToList();

            categories.Insert(0, new CategoryModel { Name = "-- All --", Id = 0 });

            if (guest.Person == null)
            {
                hmm.GuestCredentials = guest.Email;
            }
            else
            {
                hmm.GuestCredentials = guest.Person.Email;
            }

            IEnumerable<SelectListItem> selectList =
                from c in categories
                select new SelectListItem
                {
                    Selected = (c.Id == catId),
                    Text = c.Name,
                    Value = c.Id.ToString()
                };

            hmm.selectList = selectList;

            hmm.WaitressId = PersonId.Value;

            hmm.ProcessingComplete = 0;

            hmm.IamAWaitress = 1;

            hmm.JustOpenedTable = 1;

            hmm.TableAlias = newBarTable.TableAlias;

            hmm.Menu = items.Where(x => x.Id == _startCategoryID).ToList();

            return View("ViewWaitressTable", hmm);
        }

        private Guest CreateNewGuest(int? memberId)
        {

            if(memberId.HasValue && memberId.Value > 0)
            {
                var existingPerson = _personService.GetById(memberId.Value);

                if (existingPerson != null)
                {
                    if (existingPerson.Guests.Any())
                    {
                        return existingPerson.Guests.FirstOrDefault();
                    }
                }

            }


            Person person = new Person();
            var gu = Guid.NewGuid().ToString();
            person.HotelId = HotelID;
            person.DisplayName = gu;
            person.Email = gu;
            person.FirstName = gu;
            person.LastName = gu;
            person.Password = gu;
            person.Username = gu;
            person.PersonTypeId = (int)PersonTypeEnum.Guest;
            person.IsActive = true;
            person.Address = gu;

            person.Title = gu;
            person.MiddleName = gu;
            person.StartDate = DateTime.Now;
            person.EndDate = DateTime.Now;

            person.Guardian = gu;
            person.GuardianAddress = gu;
            person.PreviousEmployer = gu;
            person.ReasonForLeaving = gu;
            person.Notes = gu;
            person.BirthDate = DateTime.Now;
            person.PreviousEmployerStartDate = DateTime.Now;
            person.PreviousEmployerEndDate = DateTime.Now;
            person.IdNumber = "123456789";
            

            person.PicturePath = "";

            person = _personService.Create(person);

            var guest = new Guest();
            guest.FullName = person.FirstName + " " + person.LastName;
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

            return guest;
        }
        
        [HttpPost]
        public ActionResult MyTableOrder(int? tableId)
        {
            var catyList = CategoryList.ToList();

            var personId = PersonId.Value;

            var allGuestOrders = _guestOrderService.GetAll("");

            var guestOrder = allGuestOrders.FirstOrDefault(x => x.TableId == tableId.Value && x.IsActive);

            guestOrder.PersonId = personId;

            _guestOrderService.Update(guestOrder);

            var guestTables = allGuestOrders.Where(x => x.IsActive).ToList();

            var guestTable = guestTables.FirstOrDefault(x => x.TableId == tableId.Value && x.PersonId == personId);

            HotelMenuModel hmm = new HotelMenuModel();

            var items = _posItemService.GetAllInclude("StockItem").Distinct(new PosItemComparer()).GroupBy(x => x.StockItem.CategoryId).Select(x => new MenuModel { Id = x.Key, CategoryName = GetCategoryName(x.Key, catyList), Items = x.ToList() }).ToList();

            hmm.Menu = items;

            hmm.TableId = guestTable.TableId;

            hmm.GuestOrderId = guestTable.Id;

            int catId = 0;

            var cats = catyList;

            var catNames = items.Select(x => x.CategoryName).Distinct();

            var categories = cats.Where(x => catNames.Contains(x.Name)).ToList();

            categories.Insert(0, new CategoryModel { Name = "-- All --", Id = 0 });

            var guest = _guestService.GetById(guestTable.GuestId);

            if (guest.Person == null)
            {
                hmm.GuestCredentials = guest.Email;
            }
            else
            {
                hmm.GuestCredentials = guest.Person.Email;
            }


            IEnumerable<SelectListItem> selectList =
                from c in categories
                select new SelectListItem
                {
                    Selected = (c.Id == catId),
                    Text = c.Name,
                    Value = c.Id.ToString()
                };

            hmm.selectList = selectList;

            return View("ViewMyTable", hmm);
        }


        public ActionResult ViewSelfTableProcessed(int? id, int? guestId, string guestOrderNote)
        {
            var catyList = CategoryList.ToList();

            var guest = _guestService.GetById(guestId);

            var allOrder = _guestOrderService.GetAll("GuestRequestItems,GuestOrderItems,GuestRequestItems.StockItem,GuestOrderItems.StockItem,BarTable").ToList();

            var guestTables = allOrder.Where(x => x.IsActive).ToList();

            var guestTable = guestTables.FirstOrDefault(x => x.TableId == id.Value && x.GuestId == guestId);

            HotelMenuModel hmm = new HotelMenuModel();

            int? distributionPointId = CashierDistributionPointId();

            var items = new List<MenuModel>();

            if (distributionPointId.HasValue)
            {
                items = _posItemService.GetAllInclude("StockItem").Where(x => x.DistributionPointId == distributionPointId.Value && x.Remaining > 0).GroupBy(x => x.StockItem.CategoryId).Select(x => new MenuModel { Id = x.Key, CategoryName = GetCategoryName(x.Key, catyList), Items = x.ToList() }).ToList();
            }
            else
            {
                items = _posItemService.GetAllInclude("StockItem").Distinct(new PosItemComparer()).GroupBy(x => x.StockItem.CategoryId).Select(x => new MenuModel { Id = x.Key, CategoryName = GetCategoryName(x.Key, catyList), Items = x.ToList() }).ToList();
            }

            //hmm.Menu = items;

            hmm.TableId = guestTable.TableId;

            hmm.GuestOrderId = guestTable.Id;

            hmm.GuestId = guestTable.GuestId;

            var lst = allOrder.FirstOrDefault(x => x.Id == guestTable.Id && x.IsActive).GuestOrderItems.ToList();

            var lstRequest = allOrder.FirstOrDefault(x => x.Id == guestTable.Id && x.IsActive).GuestRequestItems.ToList();

            hmm.RequestedItems = lstRequest.Where(x => x.IsActive).ToList();

            hmm.GuestOrderItems = lst.Where(x => x.IsActive).ToList();

            hmm.TabOrderModelList = hmm.GuestOrderItems.GroupBy(x => x.ItemId).Select(y => new BarAndRestaurantMate.Models.TabOrderModel { Items = y.Take(1).ToList(), Quantity = y.ToList().Sum(z => z.Quantity), FullPrice = GetFullPrice(y.ToList()) }).ToList();


            if (hmm.RequestedItems.All(x => x.RequestBy == User.Identity.Name))
            {
                hmm.CanAddItems = true;
                hmm.CanaddItemInt = 1;
            }

            int catId = 0;

            var cats = catyList;

            if (cats.Any())
                _startCategoryID = cats.LastOrDefault().Id;

            var catNames = items.Select(x => x.CategoryName).Distinct();

            var categories = cats.Where(x => catNames.Contains(x.Name)).ToList();

            categories.Insert(0, new CategoryModel { Name = "-- All --", Id = 0 });

            if (guest.Person == null)
            {
                hmm.GuestCredentials = guest.Email;
            }
            else
            {
                hmm.GuestCredentials = guest.Person.Email;
            }

            IEnumerable<SelectListItem> selectList =
                from c in categories
                select new SelectListItem
                {
                    Selected = (c.Id == catId),
                    Text = c.Name,
                    Value = c.Id.ToString()
                };

            hmm.selectList = selectList;

            hmm.WaitressId = PersonId.Value;

            hmm.ProcessingComplete = 1;

            hmm.IamAWaitress = 1;

            hmm.TableAlias = guestTable.BarTable.TableName;

            items = items.Where(x => x.Id == _startCategoryID).ToList();

            hmm.Menu = items;

            PushBillToTable(hmm.GuestOrderItems, hmm.TableId, hmm.WaitressId, guestOrderNote);

            return View("ViewSelfTable", hmm);
        }


        public ActionResult ViewWaitressTableProcessed(int? id, int? guestId, string guestOrderNote)
        {
            var catyList = CategoryList.ToList();

            var guest = _guestService.GetById(guestId);

            var allOrder = _guestOrderService.GetAll("GuestRequestItems,GuestOrderItems,GuestRequestItems.StockItem,GuestOrderItems.StockItem,BarTable").ToList();

            var guestTables = allOrder.Where(x => x.IsActive).ToList();

            var guestTable = guestTables.FirstOrDefault(x => x.TableId == id.Value && x.GuestId == guestId);

            HotelMenuModel hmm = new HotelMenuModel();

          
            int? distributionPointId = CashierDistributionPointId();

            var items = new List<MenuModel>();

            if (distributionPointId.HasValue)
            {
                items = _posItemService.GetAllInclude("StockItem").Where(x => x.DistributionPointId == distributionPointId.Value && x.Remaining > 0).GroupBy(x => x.StockItem.CategoryId).Select(x => new MenuModel { Id = x.Key, CategoryName = GetCategoryName(x.Key, catyList), Items = x.ToList() }).ToList();
            }
            else
            {
                items = _posItemService.GetAllInclude("StockItem").Distinct(new PosItemComparer()).GroupBy(x => x.StockItem.CategoryId).Select(x => new MenuModel { Id = x.Key, CategoryName = GetCategoryName(x.Key, catyList), Items = x.ToList() }).ToList();
            }

            //hmm.Menu = items;

            hmm.TableId = guestTable.TableId;

            hmm.GuestOrderId = guestTable.Id;

            hmm.GuestId = guestTable.GuestId;

            var lst = allOrder.FirstOrDefault(x => x.Id == guestTable.Id && x.IsActive).GuestOrderItems.ToList();

            var lstRequest = allOrder.FirstOrDefault(x => x.Id == guestTable.Id && x.IsActive).GuestRequestItems.ToList();

            hmm.RequestedItems = lstRequest.Where(x => x.IsActive).ToList();

            hmm.GuestOrderItems = lst.Where(x => x.IsActive).ToList();

            hmm.TabOrderModelList = hmm.GuestOrderItems.GroupBy(x => x.ItemId).Select(y => new BarAndRestaurantMate.Models.TabOrderModel { Items = y.Take(1).ToList(), Quantity = y.ToList().Sum(z => z.Quantity), FullPrice = GetFullPrice(y.ToList()) }).ToList();


            if (hmm.RequestedItems.All(x => x.RequestBy == User.Identity.Name))
            {
                hmm.CanAddItems = true;
                hmm.CanaddItemInt = 1;
            }

            int catId = 0;

            var cats = catyList;

            if (cats.Any())
                _startCategoryID = cats.LastOrDefault().Id;

            var catNames = items.Select(x => x.CategoryName).Distinct();

            var categories = cats.Where(x => catNames.Contains(x.Name)).ToList();

            categories.Insert(0, new CategoryModel { Name = "-- All --", Id = 0 });

            if (guest.Person == null)
            {
                hmm.GuestCredentials = guest.Email;
            }
            else
            {
                hmm.GuestCredentials = guest.Person.Email;
            }

            IEnumerable<SelectListItem> selectList =
                from c in categories
                select new SelectListItem
                {
                    Selected = (c.Id == catId),
                    Text = c.Name,
                    Value = c.Id.ToString()
                };

            hmm.selectList = selectList;

            hmm.WaitressId = PersonId.Value;

            hmm.ProcessingComplete = 1;

            hmm.IamAWaitress = 1;

            hmm.TableAlias = guestTable.BarTable.TableName;

            items = items.Where(x => x.Id == _startCategoryID).ToList();

            hmm.Menu = items;

            PushBillToTable(hmm.GuestOrderItems, hmm.TableId, hmm.WaitressId, guestOrderNote);

            return View("ViewWaitressTable", hmm);
        }

        private void PushBillToTable(List<GuestOrderItem> list, int tableId, int personId, string guestOrderNote)
        {
            var existingItemIds = _tableItemService.GetAll().Where(x => x.TableId == tableId).Select(x => x.GuestOrderItemId).ToList();

            list = list.Where(x => !existingItemIds.Contains(x.Id)).ToList();

            foreach(var item in list)
            {
                TableItem ti = new TableItem { Note = guestOrderNote, Cashier = personId, DateSold = DateTime.Now, ItemId = item.ItemId, Qty = item.Quantity, TableId = tableId, GuestOrderItemId = item.Id,
                    Collected = false, IsActive = true, Completed = false, CollectedTime = null, CompletedTime = null, Fulfilled = false};
                _tableItemService.Create(ti);
            }
        }



        private void SendToPosPrinter(List<POSService.Entities.StockItem> lst, string tableName, bool pleasePrint, int? guestOrderId, string guestOrderNote = "")
        {
            ArrayList ar = new ArrayList();
            ArrayList arSD = new ArrayList();
            ArrayList arVat = new ArrayList();

            var totalAmount = decimal.Zero;

            //string strTableTime = MyPadright(tableName, 5) + MyPadright(DateTime.Now.ToShortTimeString(), 5);
            string strTableTime = TruncateAt(tableName.PadRight(21), 21) + TruncateAt(DateTime.Now.ToShortTimeString().PadLeft(10), 10);

            var groupList = lst.GroupBy(x => x.Id).Select(x => new PrintStockItemModel { Quantity = x.Sum(y => y.Quantity), Description = x.FirstOrDefault().Description, UnitPrice = x.FirstOrDefault().UnitPrice }).ToList();

            //var groupList = lst.GroupBy(x => x.Id).Select(x => new PrintStockItemModel { Quantity = x.ToList().Count, Description = x.FirstOrDefault().Description, UnitPrice  = x.FirstOrDefault().UnitPrice}).ToList();

            ar.Add(strTableTime);

            ar.Add("");

            if (!string.IsNullOrEmpty(guestOrderNote))
                ar.Add(guestOrderNote);

            ar.Add("============");

            foreach (var si in groupList)
            {
                var amount = si.UnitPrice * si.Quantity;
                totalAmount += amount;
                string str = TruncateAt(si.Description.PadRight(31), 31) + TruncateAt(si.Quantity.ToString().PadLeft(3), 3);
                ar.Add(str);
            }

            //var isFullPos = IsFullPos();

            double dTotal = 0;

            double.TryParse(totalAmount.ToString(), out dTotal);
            
            //PRINT PRESENT BILLL
            var receiptNumber = DateTime.Now.ToString().GetHashCode().ToString("x");

            var printer = ConfigurationManager.AppSettings["NetworkPrinterName"].ToString();

            var initialList = lst;

            if(!printer.StartsWith("Star"))
            {

                PrintReceiptRaw(lst, dTotal, 0, 0, receiptNumber, false, tableName, true, (int)PaymentMethodEnum.Cash, "LATEST ORDER", false, guestOrderNote);
                //PRINT ALL GUEST ORDER

                if (guestOrderId.HasValue && guestOrderId.Value > 0)
                {
                    var llst = _guestOrderService.GetById(guestOrderId.Value).GuestOrderItems.OrderBy(x => x.CreatedDate).ToList();

                    lst = new List<POSService.Entities.StockItem>();

                    var totalBill = decimal.Zero;

                    foreach (var ti in llst)
                    {
                        var thisProduct = ProductsList.FirstOrDefault(x => x.Id == ti.ItemId);

                        var price = thisProduct.UnitPrice;

                        var qty = ti.Quantity;

                        totalBill += (price * qty);

                        var itemDescription = thisProduct.StockItemName;

                        lst.Add(new POSService.Entities.StockItem { Id = thisProduct.Id, Quantity = qty, UnitPrice = price, Description = itemDescription });

                    }
                }

                totalAmount = decimal.Zero;

                //string strTableTime = MyPadright(tableName, 5) + MyPadright(DateTime.Now.ToShortTimeString(), 5);
                strTableTime = TruncateAt(tableName.PadRight(21), 21) + TruncateAt(DateTime.Now.ToShortTimeString().PadLeft(10), 10);

                groupList = lst.GroupBy(x => x.Id).Select(x => new PrintStockItemModel { Quantity = x.Sum(y => y.Quantity), Description = x.FirstOrDefault().Description, UnitPrice = x.FirstOrDefault().UnitPrice }).ToList();

                //var groupList = lst.GroupBy(x => x.Id).Select(x => new PrintStockItemModel { Quantity = x.ToList().Count, Description = x.FirstOrDefault().Description, UnitPrice  = x.FirstOrDefault().UnitPrice}).ToList();
                ar = new ArrayList();

                ar.Add(strTableTime);

                ar.Add("");

                if (!string.IsNullOrEmpty(guestOrderNote))
                    ar.Add(guestOrderNote);

                ar.Add("============");

                foreach (var si in groupList)
                {
                    var amount = si.UnitPrice * si.Quantity;
                    totalAmount += amount;
                    string str = TruncateAt(si.Description.PadRight(31), 31) + TruncateAt(si.Quantity.ToString().PadLeft(3), 3);
                    ar.Add(str);
                }

                //var isFullPos = IsFullPos();

                dTotal = 0;

                double.TryParse(totalAmount.ToString(), out dTotal);

                //PRINT PRESENT BILLL
                receiptNumber = DateTime.Now.ToString().GetHashCode().ToString("x");

                PrintReceiptRaw(lst, dTotal, 0, 0, receiptNumber, false, tableName, true, (int)PaymentMethodEnum.Cash, "COMPLETE ORDER", false, guestOrderNote);

                 
            }
            else
            {
                PrintReceiptStar(initialList, dTotal, 0, 0, receiptNumber, tableName, "LATEST ORDER", guestOrderNote);
            }
       

            //if (pleasePrint)
            //{
            //    DoPrintJob(arSD, ar, arVat);
            //}

            //if(!isFullPos)
            //{
            //    DoPrintJob(arSD, ar, arVat);
            //}
        }

        private void PrintReceiptStar(List<POSService.Entities.StockItem> lst, double total, int tax, int discount, string tableName, string receiptNumber, string footerText, string guestOrderNote)
        {
            PosPrinter printer = null;

            try
            {
                printer = GetReceiptPrinter();

                ConnectToPrinter(printer);

                string[] splitDetails = null;

                var thisUserName = User.Identity.Name;

                PrintReceiptHeaderNoCompany(printer, tableName, receiptNumber, DateTime.Now, thisUserName);

                foreach (var item in lst)
                {
                    //var total = item.UnitPrice * item.Quantity;
                    PrintLineItem(printer, item.Description, item.Quantity, double.Parse(item.UnitPrice.ToString()));
                }

                PrintReceiptFooter(printer, total, tax, discount, footerText, guestOrderNote);
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

        private void PrintReceiptRaw(List<POSService.Entities.StockItem> lst, double total, double tax, double discount, string receiptNumber, bool? addRestaurantGuestExtraTax, string guestTableNumber, bool printOnly, int paymentMethodId, string footerText, bool showHeader = true, string guestOrderNote = "")
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

                if(showHeader)
                {
                    if (splitDetails != null)
                    {
                        PrintReceiptHeaderRaw(printer, splitDetails[0].Trim(), splitDetails[1].Trim(), splitDetails[2].Trim(), splitDetails[3].Trim(), DateTime.Now, thisUserName, receiptNumber, guestTableNumber);
                    }
                    else
                    {
                        PrintReceiptHeaderRaw(printer, "ABCDEF Pte. Ltd.", "123 My Street, My City,", "My State, My Country", "012-3456789", DateTime.Now, thisUserName, receiptNumber, guestTableNumber);
                    }
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

                PrintReceiptFooterRaw(printer, total, tax, discount, anyTaxDetails, footerText, printOnly, paymentMethodId, guestOrderNote);

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

        private void PrintReceiptFooterRaw(string printer, double subTotal, double tax, double discount, string anyTaxDetails, string footerText, bool printOnly, int paymentMethodId, string guestOrderNote = "")
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

            if (!string.IsNullOrEmpty(guestOrderNote))
            {
                PrintTextLineRaw(printer, offSetString + String.Format("NOTE -     {0}", guestOrderNote));
            }

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


        private void PrintLineItemRaw(string printer, string itemCode, int quantity, double unitPrice)
        {
            PrintTextRaw(printer, TruncateAt(itemCode.PadRight(11), 11));
            PrintTextRaw(printer, TruncateAt(quantity.ToString("#0.00").PadLeft(9), 9));
            PrintTextRaw(printer, TruncateAt(unitPrice.ToString("#0.00").PadLeft(10), 10));
            PrintTextLineRaw(printer, TruncateAt((quantity * unitPrice).ToString("#0.00").PadLeft(10), 10));
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



        private void PrintLineItem(PosPrinter printer, string itemCode, int quantity)
        {
            PrintText(printer, TruncateAt(itemCode.PadRight(31), 31));
            PrintText(printer, TruncateAt(quantity.ToString("#0.00").PadLeft(9), 9));
            //PrintText(printer, TruncateAt(unitPrice.ToString("#0.00").PadLeft(10), 10));
            //PrintTextLine(printer, TruncateAt((quantity * unitPrice).ToString("#0.00").PadLeft(10), 10));
        }

        private bool PrintKitchenDocket(List<POSService.Entities.StockItem> lst, string note, string tableName)
        {
            PosPrinter printer = GetReceiptPrinter();

            var groupList = lst.GroupBy(x => x.Id).Select(x => new PrintStockItemModel { Quantity = x.Sum(y => y.Quantity), Description = x.FirstOrDefault().Description, UnitPrice = x.FirstOrDefault().UnitPrice }).ToList();

            var returnVal = false;

            try
            {
                ConnectToPrinter(printer);

                var thisUserName = User.Identity.Name;

                PrintLineItem(printer, tableName);

                foreach (var item in groupList)
                {
                    PrintLineItem(printer, item.Description, item.Quantity);
                }

                PrintReceiptFooter(printer, 0, 0, 0, note);

                returnVal = true;
            }
            finally
            {
                DisconnectFromPrinter(printer);
            }

            return returnVal;
        }


        public void DoPrintJob(ArrayList arShopDetails, ArrayList arItemList, ArrayList arVatChange)
        {
            var printerName = ConfigurationManager.AppSettings["PrinterName"].ToString();
            var networkPrinterName = ConfigurationManager.AppSettings["NetworkPrinterName"].ToString();


            //printerName = "EPSON TM-T20II Receipt";

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

                for (int i = 0; i < arItemList.Count; i++)
                {
                    var str = string.Empty;
                    try
                    {
                        str = arItemList[i].ToString();
                    }
                    catch
                    {
                        str = string.Empty;
                    }

                    if(str.Contains("="))
                    {
                        int RecLineChars = 42;
                        PrintTextLineRawNew(printerName, new string('=', RecLineChars));
                        PrintTextLineRawNew(printerName, string.Empty);
                        RawPrinterHelper.DoSomeThing(printerName, DrawerOpen5); //LINE FEED
                    }
                    else
                    {

                        RawPrinterHelper.DoSomeThing(printerName, DrawerOpen5); //LINE FEED
                        PrintTextLineRawNew(printerName, str);
                        RawPrinterHelper.SendStringToPrinter(printerName, string.Empty);
                        RawPrinterHelper.DoSomeThing(printerName, DrawerOpen5); //LINE FEED
                    }
                }

                RawPrinterHelper.SendStringToPrinter(printerName, string.Empty);

                RawPrinterHelper.DoSomeThing(printerName, DrawerOpen5); //LINE FEED
                RawPrinterHelper.DoSomeThing(printerName, DrawerOpen5); //LINE FEED

                RawPrinterHelper.FullCut(printerName);
                //RawPrinterHelper.OpenCashDrawer1(printerName);

            }
            catch (Exception)
            {
                // MessageBox.Show(ex.Message);
            }
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

        private void PrintTextLineRawNew(string printer, string text)
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

        public static void DoPrintJobNotWorking(ArrayList arShopDetails, ArrayList arItemList, ArrayList arVatChange)
        {
            var printerName = ConfigurationManager.AppSettings["PrinterName"].ToString();
            var networkPrinterName = ConfigurationManager.AppSettings["NetworkPrinterName"].ToString();


            //printerName = "EPSON TM-T20II Receipt";

            try
            {
                byte[] DrawerOpen5 = { 0xA };

                char V = 'a';
                byte[] DrawerOpen = { 0x1B, Convert.ToByte(V), 1 };
                ////RawPrinterHelper.DoSomeThing(printerName, DrawerOpen);

                V = '!';
                byte[] DrawerOpen1 = { 0x1B, Convert.ToByte(V), 0 };
                ////RawPrinterHelper.DoSomeThing(printerName, DrawerOpen1);

                //for (int i = 0; i < arShopDetails.Count; i++)
                //{
                //    ////RawPrinterHelper.SendStringToPrinter(printerName, arShopDetails[i].ToString());
                //    ////RawPrinterHelper.DoSomeThing(printerName, DrawerOpen5); //LINE FEED
                //}


                V = 'd';
                byte[] DrawerOpen2 = { 0x1B, Convert.ToByte(V), 3 };
                ////RawPrinterHelper.DoSomeThing(printerName, DrawerOpen2);

                V = 'a';
                byte[] DrawerOpen3 = { 0x1B, Convert.ToByte(V), 0 };
                ////RawPrinterHelper.DoSomeThing(printerName, DrawerOpen3);

                V = '!';
                byte[] DrawerOpen4 = { 0x1B, Convert.ToByte(V), 1 };
                ////RawPrinterHelper.DoSomeThing(printerName, DrawerOpen4);

                for (int i = 0; i < arItemList.Count; i++)
                {
                    ////RawPrinterHelper.SendStringToPrinter(printerName, arItemList[i].ToString());
                    ////RawPrinterHelper.DoSomeThing(printerName, DrawerOpen5); //LINE FEED
                }


                //for (int i = 0; i < arVatChange.Count; i++)
                //{
                //    if (i == 0)
                //    {
                //        V = '!';
                //        byte[] DrawerOpen6 = { 0x1B, Convert.ToByte(V), 17 };
                //        ////RawPrinterHelper.DoSomeThing(printerName, DrawerOpen6);
                //    }

                //    ////RawPrinterHelper.SendStringToPrinter(printerName, arVatChange[i].ToString());
                //    ////RawPrinterHelper.DoSomeThing(printerName, DrawerOpen5); //LINE FEED


                //    if (i == 0)
                //    {
                //        V = '!';
                //        byte[] DrawerOpen7 = { 0x1B, Convert.ToByte(V), 0 };
                //        ////RawPrinterHelper.DoSomeThing(printerName, DrawerOpen7);
                //    }
                //}

                ////RawPrinterHelper.DoSomeThing(printerName, DrawerOpen5); //LINE FEED
                ////RawPrinterHelper.DoSomeThing(printerName, DrawerOpen5); //LINE FEED

                ////RawPrinterHelper.FullCut(printerName);
                ////RawPrinterHelper.OpenCashDrawer1(printerName);

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

        private void PrintBillOnlyNewByTableItem(List<TableItem> list, int? tableId, int personId, int? guestOrderId, string guestOrderNote = "")
        {
            BarTable bt = _barTableService.GetById(tableId.Value);

            var tableList = list.Where(x => x.IsActive).ToList();

            List<POSService.Entities.StockItem> lst = new List<POSService.Entities.StockItem>();

            var totalBill = decimal.Zero;

            foreach (var ti in tableList)
            {
                var thisProduct = ProductsList.FirstOrDefault(x => x.Id == ti.ItemId);

                var price = thisProduct.UnitPrice;

                var qty = ti.Qty;

                totalBill += (price * qty);

                var itemDescription = thisProduct.StockItemName;

                lst.Add(new POSService.Entities.StockItem { Id = thisProduct.Id, Quantity = qty, UnitPrice = price, Description = itemDescription });

            }

            SendToPosPrinter(lst, bt.TableAlias, true, guestOrderId, guestOrderNote);
        }

        private void PrintBillOnlyNew(List<GuestOrderItem> list, int? tableId, int personId, int? guestOrderId, string guestOrderNote = "")
        {
            BarTable bt = _barTableService.GetById(tableId.Value);

            var tableList = list.Where(x => x.IsActive).ToList();

            List<POSService.Entities.StockItem> lst = new List<POSService.Entities.StockItem>();

            var totalBill = decimal.Zero;

            foreach (var ti in tableList)
            {
                var thisProduct = ProductsList.FirstOrDefault(x => x.Id == ti.ItemId);

                var price = thisProduct.UnitPrice;

                var qty = ti.Quantity;

                totalBill += (price * qty);

                var itemDescription = thisProduct.StockItemName;

                lst.Add(new POSService.Entities.StockItem { Id = thisProduct.Id, Quantity = qty, UnitPrice = price, Description = itemDescription });

            }

            SendToPosPrinter(lst, bt.TableAlias, false, guestOrderId, guestOrderNote);
        }

        private void PrintBillOnly(List<GuestOrderItem> list, int? tableId, int personId)
        {
            var tableList = list.Where(x => x.IsActive).ToList();

            List<POSService.Entities.StockItem> lst = new List<POSService.Entities.StockItem>();

            var totalBill = decimal.Zero;

            foreach (var ti in tableList)
            {
                var thisProduct = ProductsList.FirstOrDefault(x => x.Id == ti.ItemId);

                var price = thisProduct.UnitPrice;

                var qty = ti.Quantity;

                totalBill += (price * qty);

                var itemDescription = thisProduct.StockItemName;

                lst.Add(new POSService.Entities.StockItem { Id = thisProduct.Id, Quantity = qty, UnitPrice = price, Description = itemDescription });

            }

            var conn = ConfigurationManager.ConnectionStrings[1].ConnectionString;

            double dTotal = 0;

            double.TryParse(totalBill.ToString(), out dTotal);

            try
            {
                PrintReceipt(lst, dTotal, 0, 0);
            }
            catch (Exception)
            {
            }


        }


        private void PrintBillNew(List<GuestOrderItem> list, int? tableId, int personId, int? guestOrderId, string guestOrderNote = "")
        {
            PrintBillOnlyNew(list, tableId, personId, guestOrderId, guestOrderNote);
        }

        private void PrintBill(List<GuestOrderItem> list, int? tableId, int personId)
        {
            var tableList = list.Where(x => x.IsActive).ToList();

            List<POSService.Entities.StockItem> lst = new List<POSService.Entities.StockItem>();

            var totalBill = decimal.Zero;

            foreach (var ti in tableList)
            {
                var thisProduct = ProductsList.FirstOrDefault(x => x.Id == ti.ItemId);

                var price = thisProduct.UnitPrice;

                var qty = ti.Quantity;

                totalBill += (price * qty);

                var itemDescription = thisProduct.StockItemName;

                lst.Add(new POSService.Entities.StockItem { Id = thisProduct.Id, Quantity = qty, UnitPrice = price, Description = itemDescription });

            }

            var conn = ConfigurationManager.ConnectionStrings[1].ConnectionString;

            double dTotal = 0;

            double.TryParse(totalBill.ToString(), out dTotal);

            try
            {
                PrintReceipt(lst, dTotal, 0, 0);

                if (tableId.HasValue)
                    StockItemService.DeleteTableItems(tableId.Value, conn, personId);
            }
            catch (Exception)
            {
            }
        }

        private void PrintReceiptHalfPOS(List<POSService.Entities.StockItem> lst, double total, int tax, int discount, string note, string tableName)
        {
            PosPrinter printer = GetReceiptPrinter();

            try
            {
                ConnectToPrinter(printer);

                var thisUserName = User.Identity.Name;

                PrintLineItem(printer, tableName);

                foreach (var item in lst)
                {
                    //var total = item.UnitPrice * item.Quantity;
                    PrintLineItem(printer, item.Description, item.Quantity, double.Parse(item.UnitPrice.ToString()));

                }

                PrintReceiptFooter(printer, total, tax, discount, note);
            }
            finally
            {
                DisconnectFromPrinter(printer);

            }
        }

        private void PrintReceipt(List<POSService.Entities.StockItem> lst, double total, int tax, int discount)
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
                catch (Exception)
                {

                }

                if (splitDetails != null)
                {
                    PrintReceiptHeader(printer, splitDetails[0].Trim(), splitDetails[1].Trim(), splitDetails[2].Trim(), splitDetails[3].Trim(), DateTime.Now, thisUserName);
                }
                else
                {
                    PrintReceiptHeader(printer, "ABCDEF Pte. Ltd.", "123 My Street, My City,", "My State, My Country", "012-3456789", DateTime.Now, thisUserName);
                }

                foreach (var item in lst)
                {
                    //var total = item.UnitPrice * item.Quantity;
                    PrintLineItem(printer, item.Description, item.Quantity, double.Parse(item.UnitPrice.ToString()));

                }

                //PrintLineItem(printer, "Item 1", 10, 99.99);
                //PrintLineItem(printer, "Item 2", 101, 0.00);
                //PrintLineItem(printer, "Item 3", 9, 0.1);
                //PrintLineItem(printer, "Item 4", 1000, 1);

                PrintReceiptFooter(printer, total, tax, discount, "THANK YOU FOR YOUR PATRONAGE.");
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


        private void PrintReceiptFooter(PosPrinter printer, double subTotal, double tax, double discount, string footerText, string gueestOrderNote = "")
        {
            string offSetString = new string(' ', printer.RecLineChars / 2);

            PrintTextLine(printer, new string('-', (printer.RecLineChars / 3) * 2));
            PrintTextLine(printer, offSetString + String.Format("SUB-TOTAL  {0}", subTotal.ToString("#0.00")));
            PrintTextLine(printer, offSetString + String.Format("TAX        {0}", tax.ToString("#0.00")));
            PrintTextLine(printer, offSetString + String.Format("DISCOUNT   {0}", discount.ToString("#0.00")));
            PrintTextLine(printer, offSetString + new string('-', (printer.RecLineChars / 3)));
            PrintTextLine(printer, offSetString + String.Format("TOTAL      {0}", (subTotal - (tax + discount)).ToString("#0.00")));
            PrintTextLine(printer, offSetString + new string('-', (printer.RecLineChars / 3)));
            PrintTextLine(printer, String.Empty);

            if (!string.IsNullOrEmpty(gueestOrderNote))
            {
                PrintTextLine(printer, System.Text.ASCIIEncoding.ASCII.GetString(new byte[] { 27, (byte)'|', (byte)'c', (byte)'A' }) + gueestOrderNote);
            }

            

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
            PrintText(printer, TruncateAt( DateTime.Now.ToShortTimeString().PadLeft(9), 9));
            PrintTextLine(printer, String.Empty);
            PrintTextLine(printer, String.Empty);
            PrintTextLine(printer, new string('-', (printer.RecLineChars / 3) * 2));
        }

        private void PrintLineItem(PosPrinter printer, string itemCode, int quantity, double unitPrice)
        {
            PrintText(printer, TruncateAt(itemCode.PadRight(11), 11));
            PrintText(printer, TruncateAt(quantity.ToString("#0.00").PadLeft(9), 9));
            PrintText(printer, TruncateAt(unitPrice.ToString("#0.00").PadLeft(10), 10));
            PrintTextLine(printer, TruncateAt((quantity * unitPrice).ToString("#0.00").PadLeft(10), 10));
        }


        private void PrintReceiptHeaderNoCompany(PosPrinter printer,string table, string taxNumber, DateTime dateTime, string cashierName)
        {
           
            PrintTextLine(printer, new string('-', printer.RecLineChars / 2));
            PrintTextLine(printer, String.Format("DATE : {0}", dateTime.ToShortDateString()));
            PrintTextLine(printer, String.Format("CASHIER : {0}", cashierName));
            PrintTextLine(printer, String.Format("TABLE : {0}", table));

            PrintTextLine(printer, String.Empty);
            PrintText(printer, "Item             ");
            PrintText(printer, "Qty  ");
            PrintText(printer, "Unit Price ");
            PrintTextLine(printer, "Total      ");
            PrintTextLine(printer, new string('=', printer.RecLineChars));
            PrintTextLine(printer, String.Empty);

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

        public ActionResult AddNewGuestMesage(int? id)
        {
            HotelMenuModel hmm = new HotelMenuModel();
            hmm.GuestId = id.Value;
            hmm.Message = "";
            return PartialView("_AddMessage", hmm);
        }

        [HttpPost]
        public ActionResult CreateGuestMessage(string message, int? guestId)
        {
            var newId = 0;

            if(!string.IsNullOrEmpty(message) && guestId.HasValue)
            {
                var now = DateTime.Now;
                var guestMessage = new GuestMessage();
                guestMessage.MessageDate = now;
                guestMessage.Message = message;
                guestMessage.IsActive = true;
                var gm = _guestMessageService.Create(guestMessage);
                newId = gm.Id;
            }

            HotelMenuModel hmm = new HotelMenuModel();
            List<GuestMessage> guestMessages = new List<GuestMessage>();
            guestMessages = _guestMessageService.GetAllInclude("Guest").Where(x => x.MessageDate > DateTime.Today.AddDays(-1)).OrderByDescending(x => x.MessageDate).ToList();
            hmm.GuestMessages = guestMessages;
            hmm.Messaging = 1;
            hmm.IamAWaitress = 1;
            hmm.RepliedACustomer = 1;
            hmm.GuestId = guestId.Value;
            hmm.MessageId = newId;

            var guest = _guestService.GetById(guestId.Value);

            if(guest != null)
            {
                hmm.GuestCredentials = guest.Email;
            }

            

            return View("MessageWaiter", hmm);
        }

        public ActionResult MessageWaiter()
        {
            var personId = PersonId;
            var person = _personService.GetById(personId);

            HotelMenuModel hmm = new HotelMenuModel();

            List<GuestMessage> guestMessages = new List<GuestMessage>();

            if(person.PersonTypeId == (int)PersonTypeEnum.Staff)
            {
                guestMessages = _guestMessageService.GetAllInclude("Guest").Where(x => x.MessageDate > DateTime.Today.AddDays(-1)).OrderByDescending(x => x.MessageDate).ToList();
                hmm.GuestMessages = guestMessages;
                hmm.Messaging = 1;
                hmm.IamAWaitress = 1;
                hmm.GuestCredentials = User.Identity.Name;
                return View(hmm);
            }
            else
            {
                var guest = _guestService.GetAllInclude("").FirstOrDefault(x => x.PersonId == person.PersonID);

                if (guest != null)
                {
                    guestMessages = _guestMessageService.GetAll().Where(x => x.MessageDate > DateTime.Today.AddDays(-1)).OrderByDescending(x => x.MessageDate).ToList();
                }

                hmm.GuestMessages = guestMessages;
                hmm.Messaging = 1;
                hmm.GuestId = guest.Id;
                hmm.IamAWaitress = 0;
                hmm.GuestCredentials = User.Identity.Name;
                return View(hmm);
            }
        }

        private int? CashierDistributionPointId()
        {
            var user = _personService.GetById(GetPersonId().Value);
            return user.DistributionPointId;
        }

        private decimal GetFullPrice(List<GuestOrderItem> lst)
        {
            int totalQty = lst.Sum(x => x.Quantity);

            var fullPrice = decimal.Zero;

            var item = new GuestOrderItem();

            if (lst.Any())
            {
                item = lst.FirstOrDefault();
                var price = (item.StockItem.Discounted && item.StockItem.ClubPrice.HasValue && item.StockItem.ClubPrice.Value > 0)
                    ? Decimal.Round(item.StockItem.ClubPrice.Value, 2)
                    : Decimal.Round(item.StockItem.UnitPrice.Value, 2);
                //fullPrice = price * lst.Count;
                fullPrice = price * totalQty;

            }

            return Decimal.Round((fullPrice), 2);
        }

        public ActionResult ViewWaitressTable(int? id, int? newlyCreatedTable)
        {
            var catyList = CategoryList.ToList();

            var personId = PersonId.Value;

            var allGuestOrders = _guestOrderService.GetAll("GuestRequestItems,GuestRequestItems.StockItem,GuestOrderItems,GuestOrderItems.StockItem,BarTable").ToList();

            var guestTables = allGuestOrders.Where(x => x.IsActive).ToList();

            var guestTable = guestTables.FirstOrDefault(x => x.TableId == id.Value && x.PersonId == personId);

            HotelMenuModel hmm = new HotelMenuModel();

            int? distributionPointId = CashierDistributionPointId();

            var menuItems = _posItemService.GetAllInclude("StockItem").ToList();

            var items = new List<MenuModel>();

            if(distributionPointId.HasValue)
            {
                items = menuItems.Where(x => x.DistributionPointId == distributionPointId.Value && x.Remaining > 0).GroupBy(x => x.StockItem.CategoryId).Select(x => new MenuModel { Id = x.Key, CategoryName = GetCategoryName(x.Key, catyList), Items = x.ToList() }).ToList();
            }
            else
            {
                items = menuItems.Distinct(new PosItemComparer()).GroupBy(x => x.StockItem.CategoryId).Select(x => new MenuModel { Id = x.Key, CategoryName = GetCategoryName(x.Key, catyList), Items = x.ToList() }).ToList();
            }
             
            

            hmm.TableId = guestTable.TableId;

            hmm.GuestOrderId = guestTable.Id;

            hmm.GuestId = guestTable.GuestId;

            var lst = allGuestOrders.FirstOrDefault(x => x.Id == guestTable.Id && x.IsActive).GuestOrderItems.ToList();

            var lstRequest = allGuestOrders.FirstOrDefault(x => x.Id == guestTable.Id && x.IsActive).GuestRequestItems.ToList();

            hmm.RequestedItems = lstRequest.Where(x => x.IsActive).ToList();

            hmm.GuestOrderItems = lst.Where(x => x.IsActive).ToList();

            hmm.TabOrderModelList = hmm.GuestOrderItems.GroupBy(x => x.ItemId).Select(y => new BarAndRestaurantMate.Models.TabOrderModel { Items = y.Take(1).ToList(), Quantity = y.ToList().Sum(z => z.Quantity), FullPrice = GetFullPrice(y.ToList()) }).ToList();

            if (lstRequest.All(x => x.RequestBy == User.Identity.Name))
            {
                hmm.CanAddItems = true;
                hmm.CanaddItemInt = 1;
            }

            if (newlyCreatedTable.HasValue)
                hmm.NewlyCreatedTable = newlyCreatedTable.Value;

            int catId = 0;

            var cats = catyList;

            if (cats.Any())
                _startCategoryID = cats.LastOrDefault().Id;

            var catNames = items.Select(x => x.CategoryName).Distinct();

            var categories = cats.Where(x => catNames.Contains(x.Name)).ToList();

            categories.Insert(0, new CategoryModel { Name = "-- All --", Id = 0 });

            var guest = _guestService.GetById(guestTable.GuestId);

            if (guest.Person == null)
            {
                hmm.GuestCredentials = guest.Email;
            }
            else
            {
                hmm.GuestCredentials = guest.Person.Email;
            }

            IEnumerable<SelectListItem> selectList =
                from c in categories
                select new SelectListItem
                {
                    Selected = (c.Id == catId),
                    Text = c.Name,
                    Value = c.Id.ToString()
                };

            hmm.selectList = selectList;

            hmm.WaitressId = PersonId.Value;

            hmm.ProcessingComplete = 0;

            hmm.IamAWaitress = 1;

            items = items.Where(x => x.Id == _startCategoryID).ToList();

            hmm.Menu = items;

            hmm.TableAlias = guestTable.BarTable.TableName;

            return View(hmm);
        }



        
        [HttpPost]
        public ActionResult PayBillDoneByCustomer(int? tableId, int? guestId)
        {
            var personId = PersonId.Value;

            var guestOrder = _guestOrderService.GetAll("GuestRequestItems,GuestOrderItems,GuestOrderItems.StockItem").FirstOrDefault(x => x.TableId == tableId.Value && x.GuestId == guestId && x.IsActive);

            var newPerson = _personService.GetById(personId);

            PaymentOrder po = new PaymentOrder();
            po.Email = newPerson.Email;
            po.PaymentDate = DateTime.Now;
            po.TransactionStatus = false;
            po.GuestId = guestOrder.GuestId;
            po.GuestOrderId = guestOrder.Id;
            //po.TotalAmount = guestOrder.GuestOrderItems.Sum(x => x.Price);
            po.TotalAmount = (guestOrder.GuestOrderItems.Any() && guestOrder.GuestOrderItems.FirstOrDefault().StockItem.Discounted && guestOrder.GuestOrderItems.FirstOrDefault().StockItem.ClubPrice.HasValue && guestOrder.GuestOrderItems.FirstOrDefault().StockItem.ClubPrice.Value > 0) ? Decimal.Round(guestOrder.GuestOrderItems.Sum(x => x.StockItem.ClubPrice.Value), 2) : Decimal.Round(guestOrder.GuestOrderItems.Sum(x => x.Price), 2);

            //po.TotalAmount = guestOrder.GuestOrderItems.Sum(x => x.Price);

            _paymentOrderService.Create(po);

            var amountOut = decimal.Round(po.TotalAmount.Value, 2);

            PaymentViewModel am = new PaymentViewModel();
            am.OrderId = "000" + po.Id.ToString();
            am.CustomerEmail = newPerson.Email;
            am.Amount = decimal.Round(po.TotalAmount.Value, 2).ToString();
            am.AmountToPay = am.Amount;
            am.amt = amountOut;
            am.MerchantId = _merchantID;

            return View(am);
        }

        [HttpPost]
        public ActionResult PlaceOrderDoneByCustomer(int? tableId, int? guestId)
        {
            var personId = PersonId.Value;

            var guestOrder = _guestOrderService.GetAll("GuestRequestItems").FirstOrDefault(x => x.TableId == tableId.Value && x.GuestId == guestId && x.IsActive);

            var newPerson = _personService.GetById(guestOrder.PersonId);

            var newOrderPlaced = 0;

            if (guestOrder != null)
            {
                guestOrder.GuestRequestItems.ForEach(x => x.Delivered = false);
                guestOrder.GuestRequestItems.ForEach(x => x.Confirmed = false);
                //guestOrder.GuestRequestItems.ForEach(x => x.IsActive = true);
                guestOrder.GuestRequestItems.ForEach(x => x.Paid = false);
                guestOrder.GuestRequestItems.ForEach(x => x.RequestBy = newPerson.Email);

               
                _guestOrderService.Update(guestOrder);
                newOrderPlaced = 1;
            }

            return RedirectToAction("SignalROrder", new { id = tableId, newOrderPlaced });
        }

        
        [HttpPost]
        public ActionResult DeleteTable(int? tableId, int? guestId, int? guestOrderId)
        {
            var personId = PersonId.Value;

            var guestOrder = _guestOrderService.GetAll("GuestRequestItems").FirstOrDefault(x => x.Id == guestOrderId && x.IsActive);

            if (guestOrder != null)
            {
                using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("DeleteGuestOrderComplete", myConnection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        myConnection.Open();

                        SqlParameter custId = cmd.Parameters.AddWithValue("@GuestOrderId", guestOrder.Id);
                        SqlParameter tId = cmd.Parameters.AddWithValue("@TableId", tableId.Value);

                        try
                        {
                            cmd.ExecuteNonQuery();
                        }
                        catch(Exception)
                        {
                        }


                        
                    }
                }
            }

            return RedirectToAction("UnusedTables");
        }

        
        [HttpPost]
        public ActionResult PlaceOrderByManagerCanDeleteTable(int? tableId, int? guestId, int? guestOrderId, string waitressProcess)
        {
            var personId = PersonId.Value;

            var guestOrder = _guestOrderService.GetAll("GuestRequestItems").FirstOrDefault(x => x.Id == guestOrderId && x.IsActive);

            if (waitressProcess == "Delete Guest Table")
            {
                if (guestOrder != null)
                {
                    using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
                    {
                        using (SqlCommand cmd = new SqlCommand("DeleteGuestOrderComplete", myConnection))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            myConnection.Open();

                            SqlParameter custId = cmd.Parameters.AddWithValue("@GuestOrderId", guestOrder.Id);
                            SqlParameter tId = cmd.Parameters.AddWithValue("@TableId", tableId.Value);


                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                return RedirectToAction("AllOrdersEmpty");

            }
            else
            {

                if (guestOrder != null)
                {
                    foreach (var item in guestOrder.GuestRequestItems.Where(x => x.IsActive).ToList())
                    {
                        guestOrder.GuestOrderItems.Add(new GuestOrderItem { CreatedDate = DateTime.Now, IsActive = true, ItemId = item.ItemId, Price = item.Price, Quantity = item.Quantity, Paid = false, Confirmed = false, Delivered = true });
                    }

                    guestOrder.GuestRequestItems.ForEach(x => x.Delivered = false);
                    guestOrder.GuestRequestItems.ForEach(x => x.Confirmed = false);
                    guestOrder.GuestRequestItems.ForEach(x => x.IsActive = false);
                    guestOrder.GuestRequestItems.ForEach(x => x.Paid = false);

                    _guestOrderService.Update(guestOrder);
                }
            }

            return RedirectToAction("ViewWaitressTableProcessed", new { id = tableId, guestId = guestId });
        }

        
        [HttpPost]
        public ActionResult PlaceOrderByManagerCanDelete(int? tableId, int? guestId, int? guestOrderId, string waitressProcess)
        {
            var personId = PersonId.Value;

            var guestOrder = _guestOrderService.GetAll("GuestRequestItems").FirstOrDefault(x => x.Id == guestOrderId && x.IsActive);

            if (waitressProcess == "Delete Guest Order")
            {
                if (guestOrder != null)
                {
                    using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
                    {
                        using (SqlCommand cmd = new SqlCommand("DeleteGuestOrderComplete", myConnection))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            myConnection.Open();

                            SqlParameter custId = cmd.Parameters.AddWithValue("@GuestOrderId", guestOrder.Id);
                            SqlParameter tId = cmd.Parameters.AddWithValue("@TableId", tableId.Value);


                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                return RedirectToAction("AllOrders");

            }
            else
            {

                if (guestOrder != null)
                {
                    foreach (var item in guestOrder.GuestRequestItems.Where(x => x.IsActive).ToList())
                    {
                        guestOrder.GuestOrderItems.Add(new GuestOrderItem { CreatedDate = DateTime.Now, IsActive = true, ItemId = item.ItemId, Price = item.Price, Quantity = item.Quantity, Paid = false, Confirmed = false, Delivered = true });
                    }

                    guestOrder.GuestRequestItems.ForEach(x => x.Delivered = false);
                    guestOrder.GuestRequestItems.ForEach(x => x.Confirmed = false);
                    guestOrder.GuestRequestItems.ForEach(x => x.IsActive = false);
                    guestOrder.GuestRequestItems.ForEach(x => x.Paid = false);

                    _guestOrderService.Update(guestOrder);
                }
            }

            return RedirectToAction("ViewWaitressTableProcessed", new { id = tableId, guestId = guestId });
        }


        [HttpPost]
        public ActionResult PlaceOrderByWaitressCanDelete(int? tableId, int? guestId, int? guestOrderId, string waitressProcess)
        {
            var personId = PersonId.Value;

            var guestOrder = _guestOrderService.GetAll("GuestRequestItems").FirstOrDefault(x => x.Id == guestOrderId && x.IsActive);

            if (waitressProcess == "Delete Guest Order")
            {
                if (guestOrder != null)
                {
                    using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
                    {
                        using (SqlCommand cmd = new SqlCommand("DeleteGuestOrderComplete", myConnection))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            myConnection.Open();

                            SqlParameter custId = cmd.Parameters.AddWithValue("@GuestOrderId", guestOrder.Id);
                            SqlParameter tId = cmd.Parameters.AddWithValue("@TableId", tableId.Value);


                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                return RedirectToAction("WaitressOrders");

            }
            else
            {

                if (guestOrder != null)
                {
                    foreach (var item in guestOrder.GuestRequestItems.Where(x => x.IsActive).ToList())
                    {
                        guestOrder.GuestOrderItems.Add(new GuestOrderItem { CreatedDate = DateTime.Now, IsActive = true, ItemId = item.ItemId, Price = item.Price, Quantity = item.Quantity, Paid = false, Confirmed = false, Delivered = true });
                    }

                    guestOrder.GuestRequestItems.ForEach(x => x.Delivered = false);
                    guestOrder.GuestRequestItems.ForEach(x => x.Confirmed = false);
                    guestOrder.GuestRequestItems.ForEach(x => x.IsActive = false);
                    guestOrder.GuestRequestItems.ForEach(x => x.Paid = false);

                    _guestOrderService.Update(guestOrder);
                }
            }

            return RedirectToAction("ViewWaitressTableProcessed", new { id = tableId, guestId = guestId });
        }


        public ActionResult WaiterCollections()
        {
            TableItemService tis = new TableItemService();
            var kitchenlist = tis.GetAll("BarTable,StockItem,BarTable.Person,BarTable.Person.DistributionPoint").Where(x => x.DateSold > GetStartDateTime() && x.Completed && !x.Collected).OrderByDescending(x => x.DateSold).ToList().GroupBy(x => x.BarTable).Select(x => new KitchenModel { List = x.ToList(), ValueIds = x.ToList().Select(y => y.Id.ToString()).ToDelimitedString(","), BarTab = x.Key }).ToList();
            return View(new BaseViewModel { Kitchenlist = kitchenlist });
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


        
        [HttpPost]
        public ActionResult PlaceOrderDoneByCashier(string valueIds)
        {
            if (string.IsNullOrEmpty(valueIds))
                return RedirectToAction("IndexCashier", "Guest");

            var vals = valueIds.Split(',');

            int tableId = 0;
            int personId = 0;

            List<int> lstIds = new List<int>();

            foreach (var v in vals)
            {
                var ti = _tableItemService.GetById(int.Parse(v));

                if(ti != null)
                {
                    lstIds.Add(ti.GuestOrderItemId.Value);
                    ti.SentToPOS = true;
                    tableId = ti.TableId;
                    personId = ti.Cashier;
                    _tableItemService.Update(ti);
                }
                
            }

            if (tableId > 0 && personId > 0)
            {
                var allGuestOrderItems = _guestOrderItemService.GetAllInclude("StockItem").Where(x => lstIds.Contains(x.Id)).ToList();
                PrintBill(allGuestOrderItems, tableId, personId);
            }

            return RedirectToAction("IndexCashier", "Guest");
        }

        

        [HttpPost]
        public ActionResult PlaceOrderDoneByCollection(string valueIds)
        {
            if (string.IsNullOrEmpty(valueIds))
                return RedirectToAction("Index", "Home");

            var vals = valueIds.Split(',');

            foreach (var v in vals)
            {
                var ti = _tableItemService.GetById(int.Parse(v));
                ti.Collected = true;
                ti.CollectedTime = DateTime.Now;
                _tableItemService.Update(ti);
            }

            return RedirectToAction("Index", "Home");
        }

        
        [HttpPost]
        public ActionResult PlaceOrderDoneByRoomService(string valueIds, string printOrComplete)
        {
            if (string.IsNullOrEmpty(valueIds))
                return RedirectToAction("Index", "Home");

            var vals = valueIds.Split(',');

            if (printOrComplete.ToUpper().StartsWith("PRINT"))
            {
                List<int> lstIds = new List<int>();

                int tableId = 0;
                int personId = 0;

                foreach (var v in vals)
                {
                    var ti = _tableItemService.GetById(int.Parse(v));

                    if (ti != null)
                    {
                        
                        lstIds.Add(ti.GuestOrderItemId.Value);
                        tableId = ti.TableId;
                        personId = ti.Cashier;
                    }
                }

                if (tableId > 0 && personId > 0)
                {
                    var allGuestOrderItems = _guestOrderItemService.GetAllInclude("StockItem").Where(x => lstIds.Contains(x.Id)).ToList();
                    PrintBillOnlyNew(allGuestOrderItems, tableId, personId,null);
                }
            }
            else
            {
                List<POSService.Entities.StockItem> lst = new List<POSService.Entities.StockItem>();
                var totalBill = Decimal.Zero;
                var cashierId = PersonId.Value;
                int? tableId = 0;

                int transactionId = PersonId.Value;
                int guestId = 0;
                int paymentMethodId = (int)PaymentMethodEnum.POSTBILL;
                string paymentMethodNote = "Room Servive - " + DateTime.Now.ToString();
                var conn = ConfigurationManager.ConnectionStrings[1].ConnectionString;
                var timeOfSale = DateTime.Now;
                int? _distributionPointId = null;
                var isHotel = true;
                var receiptNumber = "";
                var discountedSum = decimal.Zero;
                var terminalId = (int)RoomPaymentTypeEnum.RoomService;
                int? guestRoomId = null;
                var thisUser = _personService.GetById(cashierId);
               

                foreach (var v in vals)
                {
                    var ti = _tableItemService.GetById(int.Parse(v));
                    ti.StoreFulfilled = true;
                    ti.StoreFulfilledTime = DateTime.Now;
                    ti.SentToPOS = true;
                    _tableItemService.Update(ti);

                    tableId = ti.TableId;
                    guestId = ti.BarTable.GuestId;

                    var thisProduct = ProductsList.FirstOrDefault(x => x.Id == ti.ItemId);

                    var price = thisProduct.UnitPrice;

                    price = (thisProduct.Discounted && thisProduct.ClubPrice.HasValue && thisProduct.ClubPrice.Value > 0) ? thisProduct.ClubPrice.Value : thisProduct.UnitPrice;

                    var qty = ti.Qty;

                    totalBill += (price * qty);

                    var itemDescription = thisProduct.StockItemName;

                    lst.Add(new POSService.Entities.StockItem { Id = thisProduct.Id, Quantity = qty, UnitPrice = price, Description = itemDescription });
                }

                if(lst.Count > 0)
                {
                    var guest = _guestService.GetById(guestId);

                    if(guest != null)
                    {
                        try
                        {
                            guestRoomId = guest.GuestRooms.FirstOrDefault().Id;
                            _distributionPointId = thisUser.DistributionPointId.Value;
                        }
                        catch
                        {

                        }
                    }

                    if (guestRoomId.HasValue && _distributionPointId.HasValue)
                    {
                        StockItemService.UpdateSales(lst, transactionId, guestId, PersonId.Value, 1, guestRoomId.Value, conn, paymentMethodId, paymentMethodNote, timeOfSale, _distributionPointId.Value, isHotel,
                        receiptNumber, terminalId, discountedSum, cashierId);

                        if (tableId.HasValue)
                        {
                            if (tableId.Value > 0)
                            {
                                StockItemService.DeleteTableItems(tableId.Value, conn, PersonId.Value);
                            }
                        }

                    }
                }
            }

            return RedirectToAction("IndexRoomService");
        }


        
        [HttpPost]
        public ActionResult PlaceOrderDoneByStore(string valueIds, string printOrComplete)
        {
            if (string.IsNullOrEmpty(valueIds))
                return RedirectToAction("Index", "Home");

            var vals = valueIds.Split(',');

            if (printOrComplete.ToUpper().StartsWith("PRINT"))
            {
                List<int> lstIds = new List<int>();

                int tableId = 0;
                int personId = 0;

                foreach (var v in vals)
                {
                    var ti = _tableItemService.GetById(int.Parse(v));

                    if (ti != null)
                    {
                        lstIds.Add(ti.GuestOrderItemId.Value);
                        tableId = ti.TableId;
                        personId = ti.Cashier;
                    }
                }

                if (tableId > 0 && personId > 0)
                {
                    var allGuestOrderItems = _guestOrderItemService.GetAllInclude("StockItem").Where(x => lstIds.Contains(x.Id)).ToList();
                    PrintBillOnlyNew(allGuestOrderItems, tableId, personId,null);
                }
            }
            else
            {
                foreach (var v in vals)
                {
                    var ti = _tableItemService.GetById(int.Parse(v));
                    ti.StoreFulfilled = true;
                    ti.StoreFulfilledTime = DateTime.Now;
                    _tableItemService.Update(ti);
                }
            }

            return RedirectToAction("PresentStoreOrders");
        }

        [HttpPost]
        public ActionResult PlaceOrderDoneByKitchen(string valueIds, string printOrComplete)
        {
            if(string.IsNullOrEmpty(valueIds))
            return RedirectToAction("Index", "Home");

            var vals = valueIds.Split(',');

            if (printOrComplete.ToUpper().StartsWith("PRINT"))
            {
                List<TableItem> lstIds = new List<TableItem>();
                int tableId = 0;
                int personId = 0;
                //int guestOrderId = 0;
                string note = string.Empty;

                foreach (var v in vals)
                {
                    var ti = _tableItemService.GetById(int.Parse(v));


                    if (ti != null)
                    {
                        lstIds.Add(ti);
                        tableId = ti.TableId;
                        personId = ti.Cashier;
                        //guestOrderId = ti.BarTable.
                        note = ti.Note;
                    }
                }

                if (tableId > 0 && personId > 0 && lstIds.Any())
                {
                    //var allGuestOrderItems = _guestOrderItemService.GetAllInclude("StockItem").Where(x => lstIds.Contains(x.Id)).ToList();
                    PrintBillOnlyNewByTableItem(lstIds, tableId, personId, null, note);
                }
            }
            else
            {
                foreach (var v in vals)
                {
                    var ti = _tableItemService.GetById(int.Parse(v));
                    ti.Fulfilled = true;
                    ti.Completed = true;
                    ti.CompletedTime = DateTime.Now;
                    _tableItemService.Update(ti);
                }
            }
           

            return RedirectToAction("PresentKitchenOrders");
        }


        public ActionResult CompletedOrderByRoomServiceAll()
        {
            TableItemService tis = new TableItemService();
            var notify = _posItemService.GetAllInclude("StockItem").Where(x => x.Remaining <= x.StockItem.NotNumber).Any();

            var kitchenlist = tis.GetAll("BarTable,StockItem,BarTable.Person,BarTable.Person.DistributionPoint")
                .Where(x => x.BarTable.StaffId == _AdminID && x.DateSold > GetStartDateTime() && x.StoreFulfilled).OrderByDescending(x => x.DateSold).ToList()
                .GroupBy(x => x.BarTable).Select(x => new KitchenModel { List = x.ToList(), ValueIds = x.ToList().Select(y => y.Id.ToString()).ToDelimitedString(","), BarTab = x.Key }).ToList();
            return View("IndexRoomServiceCompleted", new BaseViewModel { Notify = notify, Storelist = kitchenlist });
        }

        public ActionResult IndexRoomService()
        {
            TableItemService tis = new TableItemService();
            var notify = _posItemService.GetAllInclude("StockItem").Where(x => x.Remaining <= x.StockItem.NotNumber).Any();
            var kitchenlistthisOne = tis.GetAll("BarTable,StockItem,BarTable.Person,BarTable.Person.DistributionPoint").Where(x => x.BarTable.StaffId == _AdminID && 
                x.DateSold > GetStartDateTime()).OrderByDescending(x => x.DateSold).ToList();
                
             var kitchenlist  = kitchenlistthisOne.GroupBy(x => x.BarTable)
                .Select(x => new KitchenModel { Note = x.ToList().FirstOrDefault().Note, List = x.ToList(),
                    ValueIds = x.ToList().Select(y => y.Id.ToString()).ToDelimitedString(","), BarTab = x.Key }).ToList();

            return View("IndexRoomServiceNew", new BaseViewModel { Notify = notify, Storelist = kitchenlist });
        }


        public ActionResult CompletedOrderByStore()
        {
            TableItemService tis = new TableItemService();
            var notify = _posItemService.GetAllInclude("StockItem").Where(x => !x.StockItem.CookedFood && x.Remaining <= x.StockItem.NotNumber).Any();
            var kitchenlist = tis.GetAll("BarTable,StockItem,BarTable.Person,BarTable.Person.DistributionPoint").Where(x => x.BarTable.StaffId != _AdminID && x.DateSold > GetStartDateTime() && !x.StoreFulfilled && !x.StockItem.CookedFood).OrderByDescending(x => x.DateSold).ToList().GroupBy(x => x.BarTable)
                .Select(x => new KitchenModel { Note = x.ToList().FirstOrDefault().Note, List = x.ToList(), ValueIds = x.ToList().Select(y => y.Id.ToString()).ToDelimitedString(","), BarTab = x.Key }).ToList();
            return View("IndexStore", new BaseViewModel { Notify = notify, Storelist = kitchenlist });
        }

        //LowStock
        public ActionResult LowStock()
        {
            var notifyList = _posItemService.GetAllInclude("StockItem").Where(x => !x.StockItem.CookedFood && x.Remaining <= x.StockItem.NotNumber).ToList();
            //var kitchenlist = tis.GetAll("BarTable,StockItem").Where(x => x.DateSold > GetStartDateTime() && x.StoreFulfilled && !x.StockItem.CookedFood).OrderByDescending(x => x.DateSold).ToList().GroupBy(x => x.BarTable).Select(x => new KitchenModel { List = x.ToList(), ValueIds = x.ToList().Select(y => y.Id.ToString()).ToDelimitedString(","), BarTab = x.Key }).ToList();
            return View("IndexStoreStock", new BaseViewModel { NotifyList = notifyList });
        }

        public ActionResult CompletedOrderByStoreAll()
        {
            TableItemService tis = new TableItemService();
            var notify = _posItemService.GetAllInclude("StockItem").Where(x => !x.StockItem.CookedFood && x.Remaining <= x.StockItem.NotNumber).Any();

            var kitchenlist = tis.GetAll("BarTable,StockItem,BarTable.Person,BarTable.Person.DistributionPoint")
                .Where(x => x.BarTable.StaffId != _AdminID && x.DateSold > GetStartDateTime() && x.StoreFulfilled && !x.StockItem.CookedFood).OrderByDescending(x => x.DateSold).ToList()
                .GroupBy(x => x.BarTable).Select(x => new KitchenModel { List = x.ToList(), ValueIds = x.ToList().Select(y => y.Id.ToString()).ToDelimitedString(","), BarTab = x.Key }).ToList();
            return View("IndexStore", new BaseViewModel { Notify = notify, Storelist = kitchenlist });
        }

        public ActionResult PresentKitchenOrders()
        {
            TableItemService tis = new TableItemService();
            var kitchenlist = tis.GetAll("BarTable,StockItem,BarTable.Person,BarTable.Person.DistributionPoint")
                .Where(x => x.BarTable.StaffId != _AdminID && x.DateSold > GetStartDateTime() && !x.Fulfilled && x.StockItem.CookedFood).OrderByDescending(x => x.DateSold).ToList().GroupBy(x => x.BarTable)
                .Select(x => new KitchenModel { Note = x.ToList().FirstOrDefault().Note, List = x.ToList(), ValueIds = x.ToList().Select(y => y.Id.ToString()).ToDelimitedString(","), BarTab = x.Key }).ToList();
            return View("IndexKitchenNew", new BaseViewModel { Kitchenlist = kitchenlist, HotelName = "", ExpiryDate = DateTime.Now, FutureReservationCount = 0, CheckinDate = DateTime.Today.Date, CheckoutDate = DateTime.Today.AddDays(1).Date });
             
        }

        public ActionResult PresentStoreOrders()
        {
            TableItemService tis = new TableItemService();
            var kitchenlist = tis.GetAll("BarTable,StockItem,BarTable.Person,BarTable.Person.DistributionPoint")
                .Where(x => x.BarTable.StaffId != _AdminID && x.DateSold > GetStartDateTime() && !x.Fulfilled && !x.StockItem.CookedFood).OrderByDescending(x => x.DateSold).ToList().GroupBy(x => x.BarTable)
                .Select(x => new KitchenModel { Note = x.ToList().FirstOrDefault().Note, List = x.ToList(), ValueIds = x.ToList().Select(y => y.Id.ToString()).ToDelimitedString(","), BarTab = x.Key }).ToList();
            return View("IndexStoreNew", new BaseViewModel { Kitchenlist = kitchenlist, HotelName = "", ExpiryDate = DateTime.Now, FutureReservationCount = 0, CheckinDate = DateTime.Today.Date, CheckoutDate = DateTime.Today.AddDays(1).Date });

        }


        public ActionResult GetKitchenOrdersPrevious()
        {
            TableItemService tis = new TableItemService();
            var kitchenlist = tis.GetAll("BarTable,StockItem,BarTable.Person,BarTable.Person.DistributionPoint").Where(x => x.BarTable.StaffId != _AdminID && x.DateSold > GetStartDateTime()
                && x.Fulfilled && x.StockItem.CookedFood).OrderByDescending(x => x.DateSold).ToList().GroupBy(x => x.BarTable)
                .Select(x => new KitchenModel { Note = x.ToList().FirstOrDefault().Note, List = x.ToList(), ValueIds = x.ToList().Select(y => y.Id.ToString()).ToDelimitedString(","), BarTab = x.Key }).ToList();

            string kitchenlistStr = RenderRazorViewToString("_KitchenPC", new BaseViewModel { Kitchenlist = kitchenlist });

            return Json(new { RingAlarm = 0, kitchenlistStr }, JsonRequestBehavior.AllowGet);
            //return PartialView("_KitchenPC", new BaseViewModel { Kitchenlist = kitchenlist });
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


        public ActionResult CompletedOrderByKitchenAll()
        {
            TableItemService tis = new TableItemService();
            var kitchenlist = tis.GetAll("BarTable,StockItem,BarTable.Person,BarTable.Person.DistributionPoint").Where(x => x.BarTable.StaffId != _AdminID && x.DateSold > GetStartDateTime() 
                && x.Fulfilled && x.StockItem.CookedFood).OrderByDescending(x => x.DateSold).ToList().GroupBy(x => x.BarTable)
                .Select(x => new KitchenModel { Note = x.ToList().FirstOrDefault().Note, List = x.ToList(), ValueIds = x.ToList().Select(y => y.Id.ToString()).ToDelimitedString(","), BarTab = x.Key }).ToList();
            return View("IndexKitchenCompleted", new BaseViewModel { Kitchenlist = kitchenlist });
        }


        public ActionResult CompletedOrderByCahierAll()
        {
            TableItemService tis = new TableItemService();
            var cashierlist = tis.GetAll("BarTable,StockItem").Where(x => x.BarTable.StaffId != _AdminID && x.DateSold > GetStartDateTime()).OrderByDescending(x => x.DateSold).ToList().GroupBy(x => x.BarTable).Select(x => new KitchenModel { List = x.ToList(), ValueIds = x.ToList().Select(y => y.Id.ToString()).ToDelimitedString(","), BarTab = x.Key }).ToList();
            return View("IndexCashier", new BaseViewModel { Notify = false, CashierList = cashierlist });
        }


        public ActionResult IndexCashier()
        {
            TableItemService tis = new TableItemService();
            var cashierlist = tis.GetAll("BarTable,StockItem").Where(x => x.BarTable.StaffId != _AdminID && x.DateSold > GetStartDateTime() && !x.SentToPOS).OrderByDescending(x => x.DateSold).ToList().GroupBy(x => x.BarTable).Select(x => new KitchenModel { List = x.ToList(), ValueIds = x.ToList().Select(y => y.Id.ToString()).ToDelimitedString(","), BarTab = x.Key }).ToList();
            return View("IndexCashier", new BaseViewModel { Notify = false, CashierList = cashierlist });
        }

        
        public ActionResult CompletedOrderByKitchen()
        {
            TableItemService tis = new TableItemService();
            var kitchenlist = tis.GetAll("BarTable,StockItem,BarTable.Person,BarTable.Person.DistributionPoint")
                .Where(x => x.BarTable.StaffId != _AdminID && x.DateSold > GetStartDateTime() && !x.Fulfilled && x.StockItem.CookedFood).OrderByDescending(x => x.DateSold).ToList()
                .GroupBy(x => x.BarTable).Select(x => new KitchenModel { Note = x.ToList().FirstOrDefault().Note, List = x.ToList(), ValueIds = x.ToList().Select(y => y.Id.ToString()).ToDelimitedString(","), BarTab = x.Key }).ToList();
            return View("IndexKitchen", new BaseViewModel { Kitchenlist = kitchenlist });
        }


        
        [HttpPost]
        public ActionResult PlaceOrderBySelf(int? tableId, int? guestId, int? guestOrderId, string guestOrderNote)
        {
            var personId = PersonId.Value;

            var guestOrder = _guestOrderService.GetAll("GuestRequestItems").FirstOrDefault(x => x.Id == guestOrderId && x.IsActive);

            var listToPrint = guestOrder.GuestRequestItems.Where(x => x.IsActive).ToList();

            if (guestOrder != null)
            {

                foreach (var item in guestOrder.GuestRequestItems.Where(x => x.IsActive).ToList())
                {
                    guestOrder.GuestOrderItems.Add(new GuestOrderItem { CreatedDate = DateTime.Now, IsActive = true, ItemId = item.ItemId, Price = item.Price, Quantity = item.Quantity, Paid = false, Confirmed = false, Delivered = true });
                }

                guestOrder.GuestRequestItems.ForEach(x => x.Delivered = false);
                guestOrder.GuestRequestItems.ForEach(x => x.Confirmed = false);
                guestOrder.GuestRequestItems.ForEach(x => x.IsActive = false);
                guestOrder.GuestRequestItems.ForEach(x => x.Paid = false);
                _guestOrderService.Update(guestOrder);
            }


            List<GuestOrderItem> goiList = listToPrint.Select(x => new GuestOrderItem { ItemId = x.ItemId, IsActive = true, Quantity = x.Quantity, StockItem = x.StockItem, Price = x.Price }).ToList();

            PrintBillNew(goiList, tableId, personId,guestOrderId, guestOrderNote);

            return RedirectToAction("ViewSelfTableProcessed", new { id = tableId, guestId = guestId, guestOrderNote = guestOrderNote });
        }

        [HttpPost]
        public ActionResult PlaceOrderByWaitress(int? tableId, int? guestId, int? guestOrderId, string guestOrderNote)
        {
            var personId = PersonId.Value;

            var guestOrder = _guestOrderService.GetAll("GuestRequestItems").FirstOrDefault(x => x.Id == guestOrderId && x.IsActive);

            var listToPrint = guestOrder.GuestRequestItems.Where(x => x.IsActive).ToList();

            if (guestOrder != null)
            {

                foreach (var item in guestOrder.GuestRequestItems.Where(x => x.IsActive).ToList())
                {
                    guestOrder.GuestOrderItems.Add(new GuestOrderItem { CreatedDate = DateTime.Now, IsActive = true, ItemId = item.ItemId, Price = item.Price, Quantity = item.Quantity, Paid = false, Confirmed = false, Delivered = true });
                }

                guestOrder.GuestRequestItems.ForEach(x => x.Delivered = false);
                guestOrder.GuestRequestItems.ForEach(x => x.Confirmed = false);
                guestOrder.GuestRequestItems.ForEach(x => x.IsActive = false);
                guestOrder.GuestRequestItems.ForEach(x => x.Paid = false);


                _guestOrderService.Update(guestOrder);
            }

            List<GuestOrderItem> goiList = listToPrint.Select(x => new GuestOrderItem { ItemId = x.ItemId, IsActive = true, Quantity = x.Quantity, StockItem = x.StockItem, Price = x.Price }).ToList();


            if (tableId.HasValue && goiList.Any(x => x.StockItem.KitchenOnly))
            {
                _printerTableService.Create(new PrinterTable { TableId = tableId.Value, DateTime = DateTime.Now, IsActive = true });
            }


            //List<GuestOrderItem> goiList = listToPrint.Select(x => new GuestOrderItem { ItemId = x.ItemId, IsActive = true, Quantity = x.Quantity, StockItem = x.StockItem, Price = x.Price }).ToList();
            try
            {
                PrintBillNew(goiList, tableId, personId,guestOrderId, guestOrderNote);
            }
            catch
            {

            }

            return RedirectToAction("ViewWaitressTableProcessed", new { id = tableId, guestId = guestId, guestOrderNote = guestOrderNote });
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
        
        [HttpPost]
        public ActionResult Paid(int? tableId, int? guestId, string waitressProcessBill, int? paymentMethodId)
        {
            var personId = PersonId.Value;

            var guestOrder = _guestOrderService.GetAll("").FirstOrDefault(x => x.TableId == tableId.Value && x.PersonId == personId && x.GuestId == guestId && x.IsActive);

            if (waitressProcessBill == "Delete Bill")
            {
                if (guestOrder != null)
                {
                    using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
                    {
                        using (SqlCommand cmd = new SqlCommand("DeleteBillComplete", myConnection))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            myConnection.Open();

                            SqlParameter custId = cmd.Parameters.AddWithValue("@GuestOrderId", guestOrder.Id);
                            SqlParameter tId = cmd.Parameters.AddWithValue("@TableId", tableId.Value);

                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                return RedirectToAction("WaitressBills");
            }
            else
            { 
                if (guestOrder != null)
                {
                    using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
                    {
                        using (SqlCommand cmd = new SqlCommand("DeleteGuestOrder", myConnection))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            myConnection.Open();

                            SqlParameter custId = cmd.Parameters.AddWithValue("@GuestOrderId", guestOrder.Id);
                            SqlParameter tId = cmd.Parameters.AddWithValue("@TableId", tableId.Value);


                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }

            return RedirectToAction("WaitressTables", new { sendMessage = true, tableId, guestId });
        }
        
        [HttpPost]
        public ActionResult MyOrder(int? tableId)
        {
            var personId = PersonId.Value;

            var guest = _guestService.GetAll(1).FirstOrDefault(x => x.PersonId == personId);

            var guestOrder = new GuestOrder();
            guestOrder.CreatedDate = DateTime.Now;
            guestOrder.GuestId = guest.Id;
            guestOrder.IsActive = true;
            guestOrder.Name = User.Identity.Name + "_" + tableId.Value.ToString();
            guestOrder.Paid = false;
            guestOrder.TableId = tableId.Value;
            guestOrder.PreparedByWaitress = true;

            var exist = _guestOrderService.GetAll("").FirstOrDefault(x => x.IsActive && x.TableId == tableId.Value);

            if (exist == null)
                _guestOrderService.Create(guestOrder);

            return RedirectToAction("SignalROrder", tableId);
        }

        public ActionResult SignalROrder(int? id, int? newOrderPlaced)
        {
            var personId = PersonId.Value;

            var guest = _guestService.GetAll(1).FirstOrDefault(x => x.PersonId == personId);

            if(guest == null)
            {
                return RedirectToAction("WaitressOrders");
            }

            //var allOrders = _guestOrderService.GetAll("").ToList();
            var allOrders = _guestOrderService.GetAll("GuestRequestItems,GuestOrderItems,GuestRequestItems.StockItem,BarTable").ToList();

            var guestTables = allOrders.Where(x => x.IsActive).ToList();

            var guestTable = guestTables.Where(x => x.IsActive && x.GuestId == guest.Id).LastOrDefault();
            var catyList = CategoryList.ToList();

            if(guestTable == null)
            {
                return RedirectToAction("MyOrder");
            }

            HotelMenuModel hmm = new HotelMenuModel();

            var items = _posItemService.GetAllInclude("StockItem").Distinct(new PosItemComparer()).GroupBy(x => x.StockItem.CategoryId).Select(x => new MenuModel { Id = x.Key, CategoryName = GetCategoryName(x.Key, catyList), Items = x.ToList() }).ToList();

            hmm.Menu = items;

            hmm.TableId = guestTable.TableId;

            hmm.GuestOrderId = guestTable.Id;

            hmm.GuestId = guest.Id;

            var lst = allOrders.FirstOrDefault(x => x.Id == guestTable.Id && x.IsActive).GuestOrderItems.Where(x => x.IsActive).ToList();

            var lstRequest = allOrders.FirstOrDefault(x => x.Id == guestTable.Id && x.IsActive).GuestRequestItems.Where(x => x.IsActive).ToList();

            hmm.RequestedItems = lstRequest.Where(x => x.IsActive).ToList();

            hmm.GuestOrderItems = lst.Where(x => x.IsActive).ToList();

            hmm.TabOrderModelList = hmm.GuestOrderItems.GroupBy(x => x.ItemId).Select(y => new BarAndRestaurantMate.Models.TabOrderModel { Items = y.Take(1).ToList(), Quantity = y.ToList().Sum(z => z.Quantity), FullPrice = GetFullPrice(y.ToList()) }).ToList();


            if (hmm.RequestedItems.All(x => x.RequestBy == User.Identity.Name))
            {
                hmm.CanAddItems = true;
                hmm.CanaddItemInt = 1;
            }

            if(newOrderPlaced.HasValue && newOrderPlaced.Value == 1)
            {
                hmm.CanAddItems = false;
                hmm.CanaddItemInt = 0;
            }

            int catId = 0;

            var cats = catyList;

            var catNames = items.Select(x => x.CategoryName).Distinct();

            var categories = cats.Where(x => catNames.Contains(x.Name)).ToList();

            categories.Insert(0, new CategoryModel { Name = "-- All --", Id = 0 });

            IEnumerable<SelectListItem> selectList =
                from c in categories
                select new SelectListItem
                {
                    Selected = (c.Id == catId),
                    Text = c.Name,
                    Value = c.Id.ToString()
                };

            hmm.selectList = selectList;

            if (guestTable != null)
            {
                if (!guestTable.PreparedByWaitress)
                {
                    hmm.CanSeeProcessButton = true;
                }
            }

            if (newOrderPlaced.HasValue)
            {
                hmm.NewOrderPlaced = newOrderPlaced.Value;
            }

            hmm.WaitressId = guestTable.PersonId.Value;

            hmm.TableAlias = guestTable.BarTable.TableName;

            return View(hmm);

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
                selectModels = tables.Select(x => new SelectListModel { Name = x, Id = int.Parse(x) }).ToList();
            }
            catch
            {
                selectModels = new List<SelectListModel>();
            }

            //selectModels.Insert(0, new SelectListModel { Name = "-- Please Select --", Id = 0 });
            return selectModels.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString(), Selected = x.Id == selectedId });
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
            var persons = _personService.GetAll(HotelID).Where(x => !guestIds.Contains(x.PersonID) && x.Email.ToUpper() != "GUEST"  && x.PersonTypeId == (int)PersonTypeEnum.Guest).ToList();

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
      

        private string GetCategoryName(int id,List<CategoryModel> categoryList)
        {
            var categories = categoryList;

            var model = categories.FirstOrDefault(x => x.Id == id);

            if (model != null)
                return model.Name;

            return string.Empty;
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

        private static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["Core"].ConnectionString;
        }

        ////[OutputCache(Duration = int.MaxValue, VaryByParam = "roomId")]
        public ActionResult ExpressCheckout(int? roomId)
        {
            var id = PersonId;

            var guest = _guestService.GetAll(1).FirstOrDefault(x => x.PersonId == id.Value);

            GuestRoom mainGuestRoom = null;

            if (roomId.HasValue)
            {
                mainGuestRoom = guest.GuestRooms.FirstOrDefault(x => x.RoomId == roomId);
            }

            if (mainGuestRoom == null)
            {
                mainGuestRoom = guest.GuestRooms.FirstOrDefault(x => x.GroupBookingMainRoom) ?? guest.GuestRooms.FirstOrDefault();
            }

            var gravm = new GuestRoomAccountViewModel
            {
                Room = mainGuestRoom.Room,
                Guest = guest,
                RoomId = mainGuestRoom.Room.Id,
                PaymentTypeId = 0,
                Rooms = guest.GuestRooms.Select(x => x.Room).ToList(),
                GuestRoomAccount = new GuestRoomAccount { Amount = decimal.Zero }
            };

            return View(gravm);

        }

        
        public ActionResult PlayMovie()
        {
            int? vidId = 108;
            CinemaModel model = new CinemaModel();
            model.VideoPath = vidId.Value.ToString() + ".mp4";
            return View(model);
        }

        //[OutputCache(Duration = int.MaxValue, VaryByParam = "id,type")]
        public ActionResult OceansClip(int? id, string type)
        {
            var movie = _movieService.GetById(id.Value);

            string origPath = string.Empty;

            if (movie != null)
                origPath = movie.Filename;

            FileInfo oceansClipInfo = null;

            string oceansClipMimeType = String.Format("videos/{0}", type);

            var path = "~/videos/Movies/" + origPath;
            //var path = "~/App_Data/videos/" + origPath;


            switch (type)
            {
                case "mp4":
                    oceansClipInfo = new FileInfo(Server.MapPath(path));
                    break;
                case "avi":
                    oceansClipInfo = new FileInfo(Server.MapPath(path));
                    break;
                case "webm":
                    oceansClipInfo = new FileInfo(Server.MapPath("~/Content/video/oceans-clip.webm"));
                    break;
                case "ogg":
                    oceansClipInfo = new FileInfo(Server.MapPath("~/Content/video/oceans-clip.ogv"));
                    break;
            }

            return null;// new RangeFilePathResult(oceansClipMimeType, oceansClipInfo.FullName, oceansClipInfo.LastWriteTimeUtc, oceansClipInfo.Length);
            //return new RangeFileStreamResult(oceansClipInfo.OpenRead(), oceansClipMimeType, oceansClipInfo.Name, oceansClipInfo.LastWriteTimeUtc);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AddRemoveChildren(CinemaModel model)
        {
            var g = _guestService.GetById(model.GuestId); //PersonId.Guests.FirstOrDefault(x => x.Id == model.GuestId);

            var allMusic = _movieService.GetAll().Where(x => x.CategoryId == 8).ToList();

            var selectedVideos = new List<Movie>();

            if (model.CurrentBuildingIds != null && model.CurrentBuildingIds.Count() > 0)
            {
                foreach (var id in model.CurrentBuildingIds.Distinct())
                {
                    selectedVideos.Add(allMusic.FirstOrDefault(x => x.Id == id));
                }
            }

            if (model.PlayListId == 0)
            {
                GuestPlaylist gpl = new GuestPlaylist();
                gpl.Name = model.PlaylistName;
                gpl.Description = model.PlaylistDescription;
                gpl.GuestId = g.Id;
                gpl.DateCreated = DateTime.Now;

                if(string.IsNullOrEmpty(gpl.Description))
                {
                    gpl.Description = "Anonymous";
                }

                if (selectedVideos.Count > 0)
                  _guestPlaylistService.Create(gpl,selectedVideos);

            }
            else
            {
                var allP = _guestPlaylistService.GetAllInclude("Movies");

                g.GuestPlaylists = allP;

                var existingGpl = g.GuestPlaylists.FirstOrDefault(x => x.Id == model.PlayListId);
                existingGpl.Name = model.PlaylistName;
                existingGpl.Description = model.PlaylistDescription;
                existingGpl.DateCreated = DateTime.Now;


                if (string.IsNullOrEmpty(existingGpl.Description))
                {
                    existingGpl.Description = "Anonymous";
                }

                if (selectedVideos.Count > 0)
                  _guestPlaylistService.Update(existingGpl, selectedVideos);
            }

            return RedirectToAction("MyPlayList");
        }

        [HttpGet]
        public ActionResult CreatePlayList(int? id)
        {
            var personId = PersonId.Value;

            var g = _guestService.GetAll(1).FirstOrDefault(x => x.PersonId == personId);

            CinemaModel model = new CinemaModel();

            var everyMusicVideo = _movieService.GetAll().Where(x => x.CategoryId == 8).ToList();

            if (g.IsChild)
            {
                everyMusicVideo = everyMusicVideo.Where(x => !x.AdultOnly).ToList();
            }

            model.CurrentBuildings = From(new List<Movie>());

            model.AllBuildings = From(everyMusicVideo);

            model.GuestId = g.Id;

            GuestPlaylist gp = new GuestPlaylist();

            model.PlayListId = 0;

            var allP = _guestPlaylistService.GetAllInclude("Movies");

            g.GuestPlaylists = allP;

            if (id.HasValue && id.Value > 0)
            {
                gp = g.GuestPlaylists.FirstOrDefault(x => x.Id == id.Value);
                model.PlayListId = id.Value;
                model.PlaylistDescription = gp.Description;
                model.PlaylistName = gp.Name;
                model.CurrentBuildings = From(gp.Movies);
            }

            return View(model);
        }

        public IEnumerable<SelectListItem> From(IEnumerable<Movie> movies)
        {
            if (null == movies)
            {
                return new List<SelectListItem>();
            }

            return movies.Select(movie => new SelectListItem
            {
                Value = movie.Id.ToString(CultureInfo.InvariantCulture),
                Text = movie.Name
            });
        }

        public IEnumerable<SelectListItem> From(IEnumerable<MovieCategory> movieCategories)
        {
            if (null == movieCategories)
            {
                return new List<SelectListItem>();
            }

            return movieCategories.Select(movieCategory => new SelectListItem
            {
                Value = movieCategory.Id.ToString(CultureInfo.InvariantCulture),
                Text = movieCategory.Name
            });
        }

        [HttpPost]
        public ActionResult AddVideo(CinemaModel model, HttpPostedFileBase[] files)
        {
            if (files.Count() < 2)
                return RedirectToAction("Joromi");

            var idForPoster = 0;

            var movieHasBeenCreated = false;

            foreach (var file in files)
            {

                if (file != null && file.ContentLength > 0)
                {
                    // extract only the fielname
                    var fileName = Path.GetFileName(file.FileName);
                    string ext = Path.GetExtension(fileName);

                    if (ext.EndsWith("mp4"))
                    {
                        Movie m = new Movie();

                        if (!movieHasBeenCreated)
                        {

                            m.AdultOnly = model.AdultOnly;
                            m.CategoryId = model.CategoryId;
                            m.Name = model.VideoName;
                            m.Starring = model.MovieName;
                            m.Year = "2015";
                            m.Filename = ".mp4";

                            m = _movieService.Create(m);
                            idForPoster = m.Id;
                            m.Filename = m.Id.ToString() + ".mp4";

                            _movieService.Update(m);
                            movieHasBeenCreated = true;

                        }
                        else
                        {
                            m.Filename = idForPoster.ToString() + ".mp4";
                        }


                        var path = @"C:\inetpub\wwwroot\PublishHotelMotelFinally\Videos\Movies\" + m.Filename;

                        try
                        {
                            path = ConfigurationManager.AppSettings["MoviesStorage"].ToString() + m.Filename;
                        }
                        catch
                        {

                        }

                        file.SaveAs(path);

                    }
                    else if (ext.EndsWith(".jpg"))
                    {
                        //var path = Path.Combine(Server.MapPath("~/App_Data/Uploads"), m.Filename);

                        Movie m = new Movie();


                        if (!movieHasBeenCreated)
                        {

                            m.AdultOnly = model.AdultOnly;
                            m.CategoryId = model.CategoryId;
                            m.Name = model.VideoName;
                            m.Starring = model.MovieName;
                            m.Year = "2015";
                            m.Filename = ".mp4";

                            m = _movieService.Create(m);
                            idForPoster = m.Id;
                            m.Filename = m.Id.ToString() + ".jpg";

                            _movieService.Update(m);
                            movieHasBeenCreated = true;

                        }
                        else
                        {
                            m.Filename = idForPoster.ToString() + ".jpg";
                        }

                        var path = @"C:\inetpub\wwwroot\PublishHotelMotelFinally\Videos\Films\" + idForPoster.ToString() + ".jpg";

                        try
                        {
                            path = ConfigurationManager.AppSettings["FilmPictureStorage"].ToString() + idForPoster.ToString() + ".jpg";
                        }
                        catch
                        {

                        }

                        file.SaveAs(path);
                    }

                }
            }

            return RedirectToAction("MyPlaylist");
        }

        public ActionResult AddVideo()
        {
            CinemaModel cm = new CinemaModel();
            Movie m = new Movie();
            cm.Categories = From(_movieCategoryService.GetAll());
            cm.Movie = m;
            return View(cm);
        }

        [OutputCache(Duration = int.MaxValue, VaryByParam = "id")]
        public ActionResult PlayPlaylist(int? id)
        {
            CinemaModel model = new CinemaModel();

            var pl = _guestPlaylistService.GetAllInclude("Movies").FirstOrDefault(x => x.Id == id.Value);

            var allMovies = pl.Movies.ToList();

            var movie = allMovies.FirstOrDefault();
       
            if (movie != null)
            {
                model.MovieName = movie.Name;
            }

            model.FileName = movie.Filename;

            model.MusicVideos = allMovies.Select(x => new MusicVideoModel { Id = x.Id, MovieName = x.Name }).ToList();

            model.PlaylistName = pl.Name;

            model.MovieName = pl.Name;

            model.FileName = movie.Filename;

            model.PlayListId = id.Value;

            var vodMoviePath = GetMoviewPath();

            if (System.Configuration.ConfigurationManager.AppSettings["IsDebug"].ToString() == "1")
            {
                vodMoviePath = @"http://127.0.0.1:5080/vod/";
            }

            if (movie != null)
            {
                model.MovieName = movie.Name;
                model.FileName = vodMoviePath + movie.Filename;
                model.PlayListId = id.Value;
                model.VideoPath = vodMoviePath;
                model.Id = movie.Id;
            }

            return View("ShowMusicPlaylist", model);
        }

        private string GetMoviewPathXX()
        {
            try
            {
                return @"http://127.0.0.1:5080/vod/";
            }
            catch
            {
                return @"http://127.0.0.1:5080/vod/";
            }
        }

        public ActionResult MyPlaylist()
        {
            var id = PersonId;

            var g = _guestService.GetAll(1).FirstOrDefault(x => x.PersonId == id.Value);

            var allP = _guestPlaylistService.GetAllInclude("Movies");

            g.GuestPlaylists.Clear();

            g.GuestPlaylists = allP;

            CinemaModel model = new CinemaModel();

            if (g != null)
            {
                model.GuestName = g.FullName;
            }
            else
            {
                model.GuestName = "GUEST";
            }

            var lst = g.GuestPlaylists.Count;

            if (lst == 0)
                return RedirectToAction("CreatePlayList");

            var pls = g.GuestPlaylists.Count;

            var allMovies = g.GuestPlaylists.Where(x => x.Movies.Any()).FirstOrDefault().Movies.ToList();

            if (!allMovies.Any())
            {
                return RedirectToAction("CreatePlayList");
            }

            if (pls > 1)
            {
                model.PlaylistList = g.GuestPlaylists.OrderByDescending(x => x.DateCreated).ToList();
                return View("DisplayPlaylist", model);
            }

            model.PlaylistName = g.GuestPlaylists.FirstOrDefault().Name;
            model.PlayListId = g.GuestPlaylists.FirstOrDefault().Id;


            model.MovieName = model.PlaylistName;

            var movie = allMovies.FirstOrDefault();

            if (movie != null)
            {
                model.MovieName = movie.Name;
                model.OflaFileName = movie.Id.ToString() + ".html";
                model.FileName = movie.Filename;
            }

            model.MusicVideos = allMovies.Where(x => x.Id != movie.Id).Select(x => new MusicVideoModel { Id = x.Id, MovieName = x.Name }).ToList();

            model.FileName = movie.Filename;

            return View("ShowMusicPlaylist", model);

        }

        [HttpGet]
        public ActionResult GetNextPlay(int? counter, int? playListId)
        {
            var id = PersonId;

            var thisPlaylist  = _guestPlaylistService.GetById(playListId);

            var nextPlay = thisPlaylist.Movies.Skip(counter.Value).FirstOrDefault();

            var nextPlayId = thisPlaylist.Movies.FirstOrDefault().Id;

            if (nextPlay != null)
                nextPlayId = nextPlay.Id;

            return Json(new { PlayId = nextPlayId }, JsonRequestBehavior.AllowGet);
        }

        //url: "/Guest//",
        //    data: { Counter : count, PlayListId: pId },

        
        [OutputCache(Duration = int.MaxValue, VaryByParam = "id")]
        public ActionResult ShowEscort(int? id)
        {
            var item = _escortService.GetById(id);

            HotelMenuModel hmm = new HotelMenuModel();

            hmm.EscortItem = item;


            return View(hmm);
        }

        
        [OutputCache(Duration = int.MaxValue, VaryByParam = "id")]
        public ActionResult ShowFilmAjax(int? id)
        {
            CinemaModel model = new CinemaModel();

            model.FilmId = id.Value;

            var movie = _movieService.GetById(id.Value);

            var vodMoviePath = GetMoviewPath();

            if (System.Configuration.ConfigurationManager.AppSettings["IsDebug"].ToString() == "1")
            {
                vodMoviePath = @"http://127.0.0.1:5080/vod/";
            }

            if (movie != null)
            {
                model.MovieName = movie.Name;
                model.FileName = vodMoviePath + movie.Filename;
                model.Id = id.Value;
                model.VideoPath = vodMoviePath;
            }

            var movieToPlay = new MusicVideoModel { Id = movie.Id, MovieName = movie.Name };

            model.MusicVideos = new List<MusicVideoModel> { movieToPlay };

            model.PlaylistName = movie.Name;

            model.OflaFileName = id.Value.ToString() + ".html";

            return PartialView("_MusicPlayerVodServer", model);
        }

        [OutputCache(Duration = int.MaxValue, VaryByParam = "id")]
        public ActionResult ShowFilm(int? id)
        {
            
            CinemaModel model = new CinemaModel();

            model.FilmId = id.Value;

            var movie = _movieService.GetById(id.Value);

            var vodMoviePath = GetMoviewPath();

            if (System.Configuration.ConfigurationManager.AppSettings["IsDebug"].ToString() == "1")
            {
                vodMoviePath = @"http://127.0.0.1:5080/vod/";
            }
           

            if (movie != null)
            {
                model.MovieName = movie.Name;
                model.FileName = vodMoviePath  + movie.Filename;
                model.Id = id.Value;
                model.VideoPath = vodMoviePath;
            }

            var movieToPlay = new MusicVideoModel { Id = movie.Id, MovieName = movie.Name };

            model.GoBackLink = Url.Action("ShowFilms", "Guest", new { id = movie.CategoryId }).ToString();
            

            if(movie.CategoryId == 8)
            {

                var allMusicVideos = _movieService.GetAll().Where(x => x.CategoryId == 8 && x.Id != movie.Id).Select(x => new MusicVideoModel { Id = x.Id, MovieName = x.Name }).ToList();

                model.MusicVideos = allMusicVideos;

                model.PlaylistName = movie.Name;

                model.OflaFileName = id.Value.ToString() + ".html";

                return View("ShowMusic", model);

            }
            else if (movie.CategoryId == 9)
            {

                var allMusicVideos = _movieService.GetAll().Where(x => x.CategoryId == 9 && x.Id != movie.Id).Select(x => new MusicVideoModel { Id = x.Id, MovieName = x.Name }).ToList();

                model.MusicVideos = allMusicVideos;

                model.PlaylistName = movie.Name;

                model.OflaFileName = id.Value.ToString() + ".html";

                return View("ShowMusic", model);

            }
            else
            {

                model.MusicVideos = new List<MusicVideoModel> { movieToPlay };

                model.GoBackLink = Url.Action("ShowFilms", "Guest", new { id = movie.CategoryId }).ToString();

                model.PlaylistName = movie.Name;

                model.OflaFileName = id.Value.ToString() + ".html";
                model.PosterPath = id.Value.ToString() + ".jpg";

                return View(model);

            }
        }

        private string GetMoviewPath()
        {
            try
            {
                return System.Configuration.ConfigurationManager.AppSettings["VODIPPath"].ToString();
            }
            catch
            {
                return @"http://127.0.0.1:5080/vod/";
            }
        }

        private string GetMovieIP()
        {
            var ipDetails = @"http://localhost/";

            try
            {
                ipDetails = ConfigurationManager.AppSettings["MovieIP"].ToString();
            }
            catch
            { }

            return ipDetails;
        }


        public string GetFullPathIP(int id)
        {
            var ipPath = GetMovieIP();
            return ipPath + @"Videos/Movies/" + id.ToString() + ".mp4";
        }

        public string GetFullPathWebmIP(int id)
        {
            var ipPath = GetMovieIP();
            return ipPath + @"Videos/Movies/" + id.ToString() + ".webm";
        }

        public string GetFullPathImageIP(int id)
        {
            var ipPath = GetMovieIP();
            return ipPath + @"Videos/Films/" + id.ToString() + ".jpg";
        }

        public string GetFullPath(int id)
        {
            return @"http://localhost/Videos/Movies/" + id.ToString() + ".mp4";
        }

        public string GetFullPathWebm(int id)
        {
            return @"http://localhost/Videos/Movies/" + id.ToString() + ".webm";
        }

        public string GetFullPathImage(int id)
        {
            return @"http://localhost/Videos/Films/" + id.ToString() + ".jpg";
        }

        [OutputCache(Duration = int.MaxValue, VaryByParam = "id")]
        public ActionResult MusicVideos(int? id)
        {
            if (!id.HasValue)
            {
                id = 8;
            }

            var allMovies = _movieService.GetAllInclude("MovieCategory").Where(x => x.CategoryId == id.Value).ToList();

            var allCategories = allMovies.Select(x => x.MovieCategory.Name).ToList();

            var lst = new List<CinemaModel>();

            lst = allMovies.Select(x => new CinemaModel { MovieName = x.Name, FilmName = x.Filename, Year = x.Year, Genre = "Action", Starring = x.Starring, Id = x.Id }).ToList();

            var gravm = new GuestRoomAccountViewModel
            {
                CinemaList = lst,
                Categories = allCategories
            };

            return View(gravm);
        }

        [OutputCache(Duration = int.MaxValue, VaryByParam = "id")]
        public ActionResult ShowFilms(int? id)
        {
            var allMovies = _movieService.GetAllInclude("MovieCategory").ToList();

            var allCategories = allMovies.Select(x => x.MovieCategory.Name).ToList();

            var personId = PersonId.Value;

            var guest = _guestService.GetAll(1).FirstOrDefault(x => x.PersonId == personId);

            if (guest != null && guest.IsChild)
            {
                allMovies = allMovies.Where(x => !x.AdultOnly).ToList();
            }

            List<int> intList = new List<int>();

            intList = GetAllExistingMovies();

            var lst = new List<CinemaModel>();

            //var allExistingMovies = allMovies.Where(x => intList.Contains(x.Id)).ToList();
            var allExistingMovies = allMovies.ToList();


            if(allExistingMovies.Any())
            {
                lst = allExistingMovies.Where(x => x.CategoryId == id.Value).Select(x => new CinemaModel { MovieName = x.Name, FilmName = x.Filename, Year = x.Year, Genre = "Action", Starring = x.Starring, Id = x.Id }).ToList();
            }
            else
            {
                lst = allMovies.Where(x => x.CategoryId == id.Value).Select(x => new CinemaModel { MovieName = x.Name, FilmName = x.Filename, Year = x.Year, Genre = "Action", Starring = x.Starring, Id = x.Id }).ToList();
            }

            var gravm = new GuestRoomAccountViewModel
            {
                CinemaList = lst,
                Categories = allCategories
            };

            return View(gravm);
        }

        private List<int> GetAllExistingMovies()
        {
            List<int> intList = new List<int>();

            try
            {
                var path = @"C:\Program Files (x86)\Red5\webapps\vod";

                var allFilenames = Directory.EnumerateFiles(path, "*.mp4").Select(p => Path.GetFileName(p)).ToList();

                foreach (var f in allFilenames)
                {
                    var realName = Path.GetFileNameWithoutExtension(f).ToString();

                    int idF = 0;

                    int.TryParse(realName.ToString(), out idF);

                    if (idF > 0)
                        intList.Add(idF);
                }
            }
            catch
            {

            }

            return intList;
        }

        [OutputCache(Duration = int.MaxValue, VaryByParam = "movie,catId")]
        public ActionResult GetMoviesList(string movie, int? catId)
        {

            var allMovies = _movieService.GetAll().ToList();

            if(!string.IsNullOrEmpty(movie))
            {
                allMovies = allMovies.Where(x => x.Name.ToUpper().Contains(movie.ToUpper())).ToList();
            }

            if(catId.HasValue)
            {
                catId = 8;
                allMovies = allMovies.Where(x => x.CategoryId == catId.Value).ToList();
            }
            else
            {
                catId = 8;
                allMovies = allMovies.Where(x => x.CategoryId != catId.Value).ToList();
            }
            
            var personId = PersonId.Value;

            var guest = _guestService.GetAll(1).FirstOrDefault(x => x.PersonId == personId);

            if (guest != null && guest.IsChild)
            {
                allMovies = allMovies.Where(x => !x.AdultOnly && x.Name.Contains(movie)).ToList();
            }

            var lst = new List<CinemaModel>();

            lst = allMovies.Select(x => new CinemaModel { MovieName = x.Name, FilmName = x.Filename, Year = x.Year, Genre = "Action", Starring = x.Starring, Id = x.Id }).ToList();

            var gravm = new GuestRoomAccountViewModel
            {
                CinemaList = lst
            };

            return PartialView("_Movies", gravm);

        }

        [OutputCache(Duration = int.MaxValue)]
        public ActionResult Joromi()
        {
            var allMovies = _movieService.GetAllInclude("MovieCategory").ToList();
            var allCategories = allMovies.Select(x => x.MovieCategory.Name).ToList();

            var lst = new List<CinemaModel>();
            lst.Add(new CinemaModel { FilmName = "The Vampire Diaries", Year = "2012", Genre = "Action", Starring = "Nia Farrow, Spike Lee", Id = 1 });
            lst.Add(new CinemaModel { FilmName = "Enter The Dragon", Year = "1977", Genre = "Action", Starring = "Bruce Lee", Id = 1 });
            lst.Add(new CinemaModel { FilmName = "Pretty Woman", Year = "1987", Genre = "Romance", Starring = "Richard Gere, Julia Roberts", Id = 1 });
            lst.Add(new CinemaModel { FilmName = "Osawe vs 12 Badits", Year = "1997", Genre = "Action", Starring = "Peter Jack", Id = 1 });
            lst.Add(new CinemaModel { FilmName = "Johnny Wicks", Year = "2015", Genre = "Action", Starring = "Keanu Reeves", Id = 1 });
            lst.Add(new CinemaModel { FilmName = "History Of Violence", Year = "2007", Genre = "Action", Starring = "Von Hoight", Id = 1 });

            var gravm = new GuestRoomAccountViewModel
            {
                CinemaList = lst,
                Categories = allCategories
            };

            //gravm.AppDataPath = GetMovieCategoryPath();

            return View("CategoryView", gravm);
        }

        private string GetMovieCategoryPath()
        {
            //
            var movieCategory = string.Empty;

            try
            {
                movieCategory = ConfigurationManager.AppSettings["MovieCategory"].ToString();
            }
            catch
            {
                movieCategory = "";
            }

            return movieCategory;
        }

        public ActionResult Feedback()
        {
            var id = PersonId;

            var guest = _guestService.GetAll(1).FirstOrDefault(x => x.PersonId == id.Value);

            var gravm = new GuestRoomAccountViewModel
            {
                Guest = guest
            };

            return View(gravm);
        }

        [HttpPost]
        public ActionResult GuestFeedback(GuestRoomAccountViewModel model)
        {
            var id = PersonId;

            var guest = _guestService.GetAll(1).FirstOrDefault(x => x.PersonId == id.Value);

            if(ModelState.IsValid && !string.IsNullOrEmpty(model.Feedback))
            {
                IGuestFeedBackService gfs = new GuestFeedBackService();
                GuestFeedBack gfb = new GuestFeedBack();
                gfb.Comments = model.Feedback;
                gfb.DateCreated = DateTime.Now;
                gfb.GuestId = guest.Id;
                gfs.Create(gfb);
            }

            var gravm = new GuestRoomAccountViewModel
            {

            };

            return View(gravm);
        }

        ////[OutputCache(Duration = 3600, VaryByParam = "roomId")]
        public ActionResult ViewAccount(int? roomId)
        {
            var id = PersonId;

            var g = _guestService.GetAll(1).FirstOrDefault(x => x.PersonId == id.Value);

            if (g == null)
            {
                return View("IncorrectGuestDetails", new HotelMenuModel());
            }

            var guestId = g.Id;

            var guest = _guestService.GetById(guestId);

            GuestRoom mainGuestRoom = null;

            if (roomId.HasValue)
            {
                mainGuestRoom = guest.GuestRooms.FirstOrDefault(x => x.RoomId == roomId);
            }

            if (mainGuestRoom == null)
            {
                mainGuestRoom = guest.GuestRooms.FirstOrDefault(x => x.GroupBookingMainRoom) ?? guest.GuestRooms.FirstOrDefault();
            }

            var allItemisedItems = guest.SoldItemsAlls.Where(x => x.PaymentMethodId == (int)PaymentMethodEnum.POSTBILL).OrderByDescending(x => x.DateSold).ToList();

            if(mainGuestRoom == null)
            {
                return View("NoGuestAccout");
            }

            var gravm = new GuestRoomAccountViewModel
            {
                Room = mainGuestRoom.Room,
                Guest = guest,
                RoomId = mainGuestRoom.Room.Id,
                PaymentTypeId = 0,
                Rooms = guest.GuestRooms.Select(x => x.Room).ToList(),
                GuestRoomAccount = new GuestRoomAccount { Amount = decimal.Zero },
                ItemmisedItems = allItemisedItems

            };

            return View(gravm);
        }
    }
}