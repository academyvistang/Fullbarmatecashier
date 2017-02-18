using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using HotelMateWeb.Services.ServiceApi;
using System.ComponentModel.DataAnnotations;
using BarAndRestaurantMate.Helpers.Enums;
using BarAndRestaurantMate.Helpers;
using HotelMateWeb.Dal.DataCore;

namespace BarAndRestaurantMate.Models
{
    public class BaseViewModel
    {
        public BaseViewModel(int hotelId = 1)
        {
            
        }

        //[Required(ErrorMessage="Please enter a username")]
        public string UserName { get; set; }

        public int VacantRooms { get; set; }

        public int OccupiedRooms { get; set; }

        public int ReservedRooms { get; set; }


        public bool? ItemSaved { get; set; }
        public bool? GuestTransferComplete { get; set; }
        

        //[Required(ErrorMessage = "Please enter a password")]

        public string Password { get; set; }

        public int FutureReservationCount { get; set; }

        public DateTime? CheckinDate { get; set; }
        public DateTime? CheckoutDate { get; set; }
        public IEnumerable<SelectListItem> GlobalRoomTypeList { get; set; }

        public bool? LoginFailed { get; set; }

        public int room_select;

        public DateTime ExpiryDate { get; set; }

        public string HotelName { get; set; }

        public List<HotelMateWeb.Dal.DataCore.GuestFeedBack> Feedbacks { get; set; }

        public string Errormsg { get; set; }

        public List<KitchenModel> Kitchenlist { get; set; }

        public List<KitchenModel> Storelist { get; set; }

        public bool Notify { get; set; }

        public List<HotelMateWeb.Dal.DataCore.POSItem> NotifyList { get; set; }

        public List<KitchenModel> CashierList { get; set; }

        public List<POSService.Entities.StockItem> StarBuysList { get; set; }

        public List<StarBuyModel> StarBuysTableItems { get; set; }

        public bool IsCheckedInGuest { get; set; }

        public bool IsRestaturantGuest { get; set; }

        public string StrAuth { get; set; }

        public List<HotelMateWeb.Dal.DataCore.GuestMessage> TodaysMessages { get; set; }

        public HotelMateWeb.Dal.DataCore.Guest ActualGuest { get; set; }

        public string FullGuestName { get; set; }

        public bool IsPresent { get; set; }
        public List<BarTable> AllOpenTables { get; internal set; }
    }
}