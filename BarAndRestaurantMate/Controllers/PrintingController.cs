using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using HotelMateWeb.Dal.DataCore;
using HotelMateWeb.Services.Core;
using HotelMateWeb.Services.ServiceApi;
using BarAndRestaurantMate.Helpers;
using BarAndRestaurantMate.Models;
using ReportManagement;
using Microsoft.PointOfService;
using System.Collections;
using System.Configuration;
using BarAndRestaurantMate.Helpers.Enums;

namespace BarAndRestaurantMate.Controllers
{
    [Authorize()]
    [HandleError(View = "CustomErrorView")]
    public class PrintingController : PdfViewController
    {
        private readonly IGuestRoomAccountService _guestRoomAccountService;
        private readonly IGuestService _guestService;
        private readonly IRoomService _roomService;

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
            var user = _personService.GetAllForLogin().FirstOrDefault(x => x.Username.Equals(username, StringComparison.CurrentCultureIgnoreCase));
            return user.HotelId;
        }

        public void PrintToPos()
        {
            PosExplorer explorer = null;
            DeviceInfo _device;
            PosPrinter _oposPrinter;
            string LDN = "";

            explorer = new PosExplorer();
            _device = explorer.GetDevice(DeviceType.PosPrinter, LDN);
            _oposPrinter = (PosPrinter)explorer.CreateInstance(_device);
            _oposPrinter.Open();
            _oposPrinter.Claim(10000);
            _oposPrinter.DeviceEnabled = true;
            // normal print
            //_oposPrinter.PrintNormal(PrinterStation.Receipt, "yourprintdata"); 
            // pulse the cash drawer pin  pulseLength-> 1 = 100ms, 2 = 200ms, pin-> 0 = pin2, 1 = pin5
            // _oposPrinter.PrintNormal(PrinterStation.Receipt, (char)16 + (char)20 + (char)1 + (char)pin + (char)pulseLength); 

             // cut the paper
             //_oposPrinter.PrintNormal(PrinterStation.Receipt, (char)29 + (char)86 + (char)66);

             // print stored bitmap
             //_oposPrinter.PrintNormal(PrinterStation.Receipt, (char)29 + (char)47 + (char)0);
        }

       


        


        public PrintingController()
        {
            _personService = new PersonService();
           _guestRoomAccountService = new GuestRoomAccountService();
           _guestService = new GuestService();
            _roomService = new RoomService();
        }

        [HttpGet]
        //[OutputCache(Duration = 3600, VaryByParam = "id")]
        public ActionResult PrintGuestAccount(int? id, bool checkout = false)
        {
            var guest = _guestService.GetById(id.Value);

            var allItemisedItems = guest.SoldItemsAlls.Where(x => x.PaymentMethodId == (int)PaymentMethodEnum.POSTBILL).OrderByDescending(x => x.DateSold).ToList();

            var gravm = new GuestRoomAccountViewModel
            {
                Guest = guest,
                RoomId = 0,
                PaymentTypeId = 0,
                Rooms = guest.GuestRooms.Select(x => x.Room).ToList(),
                GuestRoomAccount = new GuestRoomAccount { Amount = decimal.Zero },
                ItemmisedItems = allItemisedItems                
            };

            gravm = PopulateModel(gravm, guest, checkout);

            string url = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"));
            gravm.ImageUrl = url + "images/" + "LakehouseLogoNEW4.png";
            return this.ViewPdf("Guest  Bill", "_GuestBillPrinter", gravm);
        }

        [HttpGet]
        //[OutputCache(Duration = 3600, VaryByParam = "id")]

        public ActionResult PrintGuestAccountCheckIn(int? id)
        {
            var guest = _guestService.GetById(id.Value);

            var gravm = new GuestRoomAccountViewModel
            {
                Guest = guest,
                RoomId = 0,
                PaymentTypeId = 0,
                Rooms = guest.GuestRooms.Select(x => x.Room).ToList(),
                GuestRoomAccount = new GuestRoomAccount { Amount = decimal.Zero }
            };

            gravm = PopulateModel(gravm, guest);

            string url = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"));
            gravm.ImageUrl = url + "images/" + "LakehouseLogoNEW4.png";
            return this.ViewPdf("Guest CheckIn Reciept", "_GuestBillPrinterCheckin", gravm);
        }

        private GuestRoomAccountViewModel PopulateModel(GuestRoomAccountViewModel gravm, Guest guest, bool checkout = false)
        {
            var ticks = (int)DateTime.Now.Ticks;

            var transactionId = ticks < 0 ? ticks*(-1) : ticks;

            var mainGuestRoom = guest.GuestRooms.FirstOrDefault(x => x.GroupBookingMainRoom && x.IsActive) ?? guest.GuestRooms.FirstOrDefault(x => x.IsActive);

            if (mainGuestRoom == null)
            {
                mainGuestRoom = guest.GuestRooms.FirstOrDefault(x => x.GroupBookingMainRoom && !x.IsActive) ?? guest.GuestRooms.FirstOrDefault(x => !x.IsActive);
            }

            if (null == mainGuestRoom) return gravm;

            gravm.GuestRoomNumber = guest.GuestRooms.Select(x => x.Room.RoomNumber).ToDelimitedString(",");
            gravm.GuestName = guest.FullName;
            gravm.ArrivalDate = mainGuestRoom.CheckinDate.ToShortDateString();
            gravm.DepartureDate = mainGuestRoom.CheckoutDate.ToShortDateString();

            if (mainGuestRoom.CheckoutDate < DateTime.Today)
                gravm.DepartureDate = DateTime.Now.ToString();

            gravm.NoOfNight = mainGuestRoom.GetNumberOfNights().ToString(CultureInfo.InvariantCulture);

            if (!guest.IsActive)
            {
                gravm.NoOfNight = mainGuestRoom.GetNumberOfNightsFutureBooking().ToString(CultureInfo.InvariantCulture);
                gravm.DepartureDate = mainGuestRoom.CheckoutDate.ToShortDateString();

                if (mainGuestRoom.CheckoutDate < DateTime.Today)
                    gravm.DepartureDate = DateTime.Now.ToString();
            }

            var discount = (mainGuestRoom.Room.Price - mainGuestRoom.RoomRate) * mainGuestRoom.GetNumberOfNights();

            gravm.NoOfPersons = mainGuestRoom.Occupants.ToString(CultureInfo.InvariantCulture);
            gravm.Currency = "NGN";
            gravm.Discounts = discount.ToString();
            gravm.RoomRate = mainGuestRoom.RoomRate.ToString(CultureInfo.InvariantCulture);
            gravm.BillNo = transactionId.ToString(CultureInfo.InvariantCulture);

            return gravm;
        }

        //public ActionResult ShowCapitalisationPdf(int? id)
        //{
        //    //EventViewData evd = new EventViewData();
        //    //evd.ProjectReports = projectReportRepository.Index().ToList();
        //    //evd.PassTemplateList = GetCapitalisationDataForPdf(11);
        //    PrintingViewModel pvm
        //    string url = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"));
        //    evd.ImageUrl = url + "images/" + "custom-reports.png";

        //    return this.ViewPdf("Capitalisation  report", "ReportsViewerCapitalisation", evd);
        //}
	}
}