﻿using System.Web.Mvc;
using HotelMateWeb.Dal.DataCore;
using System.Collections.Generic;
using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;


namespace BarAndRestaurantMate.Models
{
    public class RoomBookingViewModel : BaseViewModel, IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (TaxiAmount > 0 && string.IsNullOrEmpty(TaxiReg))
                yield return new ValidationResult("Please enter the registration number of the taxi.");
        }


        public IEnumerable<SelectListItem> BusinessAccounts;

        public IList<Room> RoomsList { get; set; }

        public RoomViewModel Room { get; set; }

        public GuestViewModel Guest { get; set; }

        public BusinessAccount Company { get; set; }

        public GuestRoom GuestRoom { get; set; }

        public string[] RoomBookingSelectedValues { get; set; }

        public bool GroupBooking { get; set; }

        public IEnumerable<SelectListItem> RoomBookingRooms { get; set; }

        public string PinCode { get; set; }

        public string selectedRoomIds { get; set; }
        
        public string SelectedRoomDisplay { get; set; }

        public List<Guest> GuestList { get; set; }

        public decimal InitialDeposit { get; set; }

        public decimal TaxiAmount { get; set; }
        
        public string TaxiReg { get; set; }

        public decimal ReservationDeposit { get; set; }

        public decimal DiscountedRate { get; set; }

        public int GuestId { get; set; }

        public int NumberOfNights { get; set; }

        public List<GuestRoom> GuestsRoomsList { get; set; }

        public decimal GuestRefund { get; set; }

        public decimal MaxRefund { get; set; }

        public bool NoRemoveableRooms { get; set; }

        public int RoomId { get; set; }

        public string ErrorMessage { get; set; }

        public decimal BusinessRoomRate { get; set; }

        public bool? AddToRoomCompleted { get; set; }

        public List<HotelMateWeb.Dal.DataCore.Room> RoomsMatrixList { get; set; }

        public System.DateTime StartOfMonth { get; set; }

        public System.DateTime EndOfMonth { get; set; }

        public int MonthId { get; set; }

        public List<RoomType> RoomTypesList { get; set; }

        public DateTime ThisMonth { get; set; }

        //[Required(ErrorMessage = "Please select a payment method")]
        //[Range(1, double.MaxValue, ErrorMessage = "Please select a payment method")]
        public int PaymentMethodId { get; set; }

        public string PaymentMethodNote { get; set; }


        private List<PaymentMethod> PaymentMethodList = new List<PaymentMethod>{ new PaymentMethod{ Id = 1, Description = "CASH" }, new PaymentMethod{ Id = 2, Description = "CHEQUE"},
                                                                                 new PaymentMethod{ Id = 3, Description = "POS"}, new PaymentMethod{ Id = 4, Description = "POST BILL"}};

         public IEnumerable<SelectListItem> PaymentMethods
        {
            get
            {
                var numbers = (from p in PaymentMethodList.Take(3).ToList()
                               select new SelectListItem
                                {
                                    Text = p.Description.ToString(),
                                    Value = p.Id.ToString()
                                });
                return numbers.ToList();
            }           
        }

         public IEnumerable<SelectListItem> PaymentMethodsAll
         {
             get
             {
                 var numbers = (from p in PaymentMethodList.OrderByDescending(x => x.Id).ToList()
                                select new SelectListItem
                                {
                                    Text = p.Description.ToString(),
                                    Value = p.Id.ToString()
                                });                 

                 return numbers.ToList();
             }
         }


         public bool ShowDialog { get; set; }

         public bool ComplimentaryRooms { get; set; }

         public int PendingRequestOrders { get; set; }

         public int PendingCollections { get; set; }
    }
}