using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using HotelMateWeb.Dal.DataCore;

using BarAndRestaurantMate.Helpers.Enums;


namespace BarAndRestaurantMate.Helpers
{
    public static class ExtensionMethods
    {
        //GetCompanyBalanceColourRestaurant
        //GetCompanyBalanceRestaurant

        public static decimal GetCompanyBalanceRestaurant(this BusinessAccount company)
        {
            var totalBills = company.Payments.Where(x => x.PaymentMethodId == (int)PaymentMethodEnum.POSTBILL).Sum(x => x.Total);
            var paidIn = company.BusinessCorporateAccounts.Where(x => x.PaymentMethodId == (int)PaymentMethodEnum.Cash).Sum(x => x.Amount);
            return paidIn - totalBills;
        }

        public static string GetCompanyBalanceColourRestaurant(this BusinessAccount company)
        {
            return company.GetCompanyBalanceRestaurant() > 0 ? "Green;" : "Red;";
        }
        public static decimal GetTotalHotelRecievable(this Person person, DateTime salesDate)
        {
            return person.GuestRoomAccounts.Where(x => x.TransactionDate.ToShortDateString().Equals(salesDate.ToShortDateString()) &&
                (x.PaymentMethodId == (int)PaymentMethodEnum.Cash)).Sum(x => x.Amount);
        }

        public static decimal GetTotalBarRecievable(this Person person, DateTime salesDate)
        {
            return person.SoldItemsAlls.Where(x => (x.TillOpen) && (x.PaymentMethodId == (int)PaymentMethodEnum.Cash)).Sum(x => x.TotalPrice);
        }

        public static decimal TotalAccounts(this BusinessAccount company)
        {
            var accounts = company.GuestRooms.SelectMany(x => x.GuestRoomAccounts);
            return accounts.Where(x => (x.PaymentMethodId == (int)PaymentMethodEnum.POSTBILL) 
                || (x.PaymentTypeId == (int)RoomPaymentTypeEnum.CashDeposit || x.PaymentTypeId == (int)RoomPaymentTypeEnum.InitialDeposit 
                || x.PaymentTypeId == (int)RoomPaymentTypeEnum.ReservationDeposit 
                )).Summation();
        }


        public static decimal TotalAccounts(this Guest guest)
        {
            var accounts = guest.GuestRooms.SelectMany(x => x.GuestRoomAccounts);
            return accounts.Where(x => (x.PaymentMethodId == (int)PaymentMethodEnum.POSTBILL) 
                || (x.PaymentTypeId == (int)RoomPaymentTypeEnum.CashDeposit 
                || x.PaymentTypeId == (int)RoomPaymentTypeEnum.InitialDeposit 
                || x.PaymentTypeId == (int)RoomPaymentTypeEnum.ReservationDeposit 
                )).Summation();
            //return decimal.Zero;
        }
        public static string GetRoomGuest(this Room room)
        {
            var gr = room.GuestRooms.Where(x => x.Guest.IsActive && x.IsActive).OrderByDescending(x => x.CheckinDate).ToList();
            return gr.Count == 0 ? "EMPTY ROOM" : gr.FirstOrDefault().Guest.FullName;
        }

        public static Guest GetActualRoomGuest(this Room room)
        {
            var gr = room.GuestRooms.Where(x => x.Guest.IsActive && x.IsActive).OrderByDescending(x => x.CheckinDate).ToList();
            return gr.Count == 0 ? null : gr.FirstOrDefault().Guest;
        }

        public static long TicksNonNeg(this DateTime dt)
        {
            var ticks = dt.Ticks;
            return ticks < 0 ? ticks*(-1) : ticks;
        }

        public static string ToDelimitedString(this IEnumerable<string> list, string delimiter)
        {
            return list == null ? string.Empty : string.Join(delimiter, list.ToArray());
        }

        public static bool IsBetween(this DateTime valToCheck, DateTime startdate, DateTime endDate)
        {
            var hotelAccountsTime = 14;
            int.TryParse(ConfigurationManager.AppSettings["HotelAccountsTime"].ToString(CultureInfo.InvariantCulture), out hotelAccountsTime);
            valToCheck = new DateTime(valToCheck.Year, valToCheck.Month, valToCheck.Day, hotelAccountsTime, 1, 0);
            startdate = new DateTime(startdate.Year, startdate.Month, startdate.Day, hotelAccountsTime, 0, 0);
            endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, hotelAccountsTime, 0, 0);
            var conflicting = valToCheck >= startdate && valToCheck <= endDate;
            //var otherConflicts = startdate <= valToCheck && endDate <= valToCheck;

            return conflicting;// || otherConflicts;
        }

        public static bool IsBetweenStartEnd(this DateTime valToCheck, DateTime valToCheckEnd, DateTime startdate, DateTime endDate)
        {
            var hotelAccountsTime = 14;
            int.TryParse(ConfigurationManager.AppSettings["HotelAccountsTime"].ToString(CultureInfo.InvariantCulture), out hotelAccountsTime);
            valToCheck = new DateTime(valToCheck.Year, valToCheck.Month, valToCheck.Day, hotelAccountsTime, 1, 0);
            startdate = new DateTime(startdate.Year, startdate.Month, startdate.Day, hotelAccountsTime, 0, 0);
            endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, hotelAccountsTime, 0, 0);
            //var conflicting = valToCheck >= startdate && valToCheck <= endDate;
            //var otherConflicts = startdate <= valToCheck && endDate <= valToCheck;

            if (valToCheck <= startdate && valToCheckEnd >= endDate)
                return true;

            return false;// || otherConflicts;
        }

        

        public static bool ReportIsBetween(this DateTime valToCheck, DateTime startdate, DateTime endDate)
        {
            var conflicting = valToCheck >= startdate && valToCheck <= endDate;
            return conflicting;
        }

        public static IList<GuestReservation> SelectAvailable(this IEnumerable<GuestReservation> guestReservations, DateTime startDateTime, DateTime endDateTime, int? roomTypeId)
        {       

            if (roomTypeId.HasValue && roomTypeId.Value > 0)
            {
                var conflictingReservations = guestReservations.Where(x => ((x.Guest.IsActive && x.IsActive) && (startDateTime.IsBetween(x.StartDate, x.EndDate) || endDateTime.IsBetween(x.StartDate, x.EndDate))) && x.Room.RoomType == roomTypeId).ToList();
                return conflictingReservations;
            }
            else
            {
                var conflictingReservations = guestReservations.Where(x => (x.Guest.IsActive && x.IsActive) && (startDateTime.IsBetween(x.StartDate, x.EndDate) || endDateTime.IsBetween(x.StartDate, x.EndDate))).ToList();
                return conflictingReservations;
            }
        }

        public static string GetSymbolPath(this GuestRoomAccount guestRoomAccount, string scheme, string authourity, string content)
        {
            
            string url = string.Format("{0}://{1}{2}", scheme,authourity,content);
            string strPathPlus = url + "images/" + "plus_16.png";
            string strPathMinus = url + "images/" + "minus_16.png";
            return guestRoomAccount.RoomPaymentType.PaymentStatusId == (int)RoomPaymentStatusEnum.Credit ? strPathPlus : strPathMinus;
        }


        public static string GetSymbol(this GuestRoomAccount guestRoomAccount)
        {
            return guestRoomAccount.RoomPaymentType.PaymentStatusId == (int)RoomPaymentStatusEnum.Credit ? "plus_16.png" : "minus_16.png";
        }

        public static bool GuestRoomsBeRemoved(this Guest guest)
        {
            var cancellationHours = 4;
            int.TryParse(ConfigurationManager.AppSettings["HotelCancellationHours"].ToString(CultureInfo.InvariantCulture), out cancellationHours);
            var removeableGuestRooms = guest.GuestRooms.Count(x => !x.GuestRoomAccounts.Any() && (x.CheckinDate.AddHours(cancellationHours) > DateTime.Now));
            return removeableGuestRooms > 0;
        }

        public static IList<GuestReservation> RoomAvailability(this Room room, DateTime startDateTime, DateTime endDateTime, int? roomTypeId)
        {
            if (roomTypeId.HasValue && roomTypeId.Value > 0)
            {
                var conflictingReservations =
                   room.GuestReservations.Where(
                       x =>
                       ((x.IsActive) &&
                       (startDateTime.IsBetween(x.StartDate, x.EndDate) ||
                        endDateTime.IsBetween(x.StartDate, x.EndDate))) && x.Room.RoomType == roomTypeId.Value).ToList();

                if (conflictingReservations.Count > 0)
                return conflictingReservations;

                conflictingReservations =
                   room.GuestReservations.Where(
                       x =>
                       ((x.IsActive) &&
                       (startDateTime.IsBetweenStartEnd(endDateTime, x.StartDate, x.EndDate))) && x.Room.RoomType == roomTypeId.Value).ToList();

                return conflictingReservations;
            }
            else
            {
                var conflictingReservations =
                    room.GuestReservations.Where(
                        x =>
                        (x.IsActive) &&
                        (startDateTime.IsBetween(x.StartDate, x.EndDate) ||
                         endDateTime.IsBetween(x.StartDate, x.EndDate))).ToList();

                if (conflictingReservations.Count > 0)
                    return conflictingReservations;

                conflictingReservations =
                   room.GuestReservations.Where(
                       x =>
                       ((x.IsActive) &&
                       (startDateTime.IsBetweenStartEnd(endDateTime, x.StartDate, x.EndDate)))).ToList();

                return conflictingReservations;
            }
        }

        public static IList<GuestReservation> RoomAvailability(this Room room, DateTime startDateTime, DateTime endDateTime, int guestId, int? roomTypeId)
        {
            if (roomTypeId.HasValue && roomTypeId.Value > 0)
            {
                var conflictingReservations =
                    room.GuestReservations.Where(
                        x =>
                        (((x.IsActive) &&
                         (startDateTime.IsBetween(x.StartDate, x.EndDate) ||
                          endDateTime.IsBetween(x.StartDate, x.EndDate))) && x.GuestId != guestId) && roomTypeId == x.Room.RoomType).ToList();

                if (conflictingReservations.Count > 0)
                    return conflictingReservations;

                conflictingReservations =
                   room.GuestReservations.Where(
                       x =>
                       ((x.IsActive) &&
                       (startDateTime.IsBetweenStartEnd(endDateTime, x.StartDate, x.EndDate))) && x.Room.RoomType == roomTypeId.Value).ToList();

                return conflictingReservations;
            }
            else
            {
                var conflictingReservations =
                   room.GuestReservations.Where(
                       x =>
                       ((x.IsActive) &&
                        (startDateTime.IsBetween(x.StartDate, x.EndDate) ||
                         endDateTime.IsBetween(x.StartDate, x.EndDate))) && x.GuestId != guestId).ToList();
                if (conflictingReservations.Count > 0)
                    return conflictingReservations;

                conflictingReservations =
                   room.GuestReservations.Where(
                       x =>
                       ((x.IsActive) &&
                       (startDateTime.IsBetweenStartEnd(endDateTime, x.StartDate, x.EndDate)))).ToList();

                return conflictingReservations;
            }
        }

        public static string BookRoom(this Room room)
        {
            if (room.GuestRooms.Any(x => x.IsActive))
                return "Update Guest Details For Room " + room.RoomNumber;
            return "Book New Guest Into Room " + room.RoomNumber;
        }

        
        public static string GetStatusBaccGroundColor(this Room room)
        {
            var statusId = room.RoomStatu.Id;

            switch (statusId)
            {
                case (int)RoomStatusEnum.Repair:
                    return "#ff0000";
                case (int)RoomStatusEnum.Dirty:
                    return "#778899";
                case (int)RoomStatusEnum.Unknown:
                    return "#778899";
                default:
                    return "";
            }
        }

        public static string GetCompanyBalanceColour(this BusinessAccount company)
        {
            var balance = company.GetGuestBalance();

            if (balance < 0)
                return "#ff0000";
            else if (balance == 0)
                return "#7aa37a";
            else
                return "#99f893";
        }

        public static string GetGuestBalanceColour(this BusinessAccount company)
        {
            var balance = company.GetGuestBalance();

            if (balance < 0)
                return "#ff0000";
            else if (balance == 0)
                return "#7aa37a";
            else
                return "#99f893";
        }


        public static string GetGuestBalanceWithTaxColour(this Guest guest)
        {
            var balance = guest.GetGuestBalanceWithFullTax();

            if (balance < 0)
                return "#ff0000";
            else if (balance == 0)
                return "#7aa37a";
            else
                return "#99f893";
        }

        public static string GetGuestBalanceColour(this Guest guest)
        {
            var balance = guest.GetGuestBalance();

            if (balance < 0)
                return "#ff0000";
            else if (balance == 0)
                return "#7aa37a";
            else
                return "#99f893";
        }

        
        public static decimal TotalSpent(this Guest guest)
        {
            var totalRooms = guest.GuestRooms.DebitSummation();
            return totalRooms;
        }

        public static decimal TotalPaidSoFarCash(this Guest guest)
        {
            var totalRooms = guest.GuestRooms.CreditSummationCash();
            return totalRooms;
        }

        public static decimal TotalPaidSoFar(this Guest guest)
        {
            var totalRooms = guest.GuestRooms.CreditSummation();
            return totalRooms;
        }

        public static decimal GetGuestRoomBalance(this Guest guest)
        {
            var totalRooms = guest.GuestRooms.Summation();
            return totalRooms;
        }

        public static decimal GetGuestReservationBalance(this Guest guest)
        {
            
            var accountTotal = decimal.Zero;

            foreach (var rm in guest.GuestRooms.Where(x => x.GuestRoomAccounts.Sum(y => y.Amount) > 0))
            {
                accountTotal += rm.GuestRoomAccounts.Where(x => x.PaymentTypeId == (int)RoomPaymentTypeEnum.ReservationDeposit).Sum(x => x.Amount);
            }

            var total = accountTotal;

            return total;
        }

        public static decimal GetCompanyBalanceWithTax(this BusinessAccount company)
        {
            return company.Payments.Where(x => x.PaymentMethodId == (int)PaymentMethodEnum.POSTBILL).Sum(x => x.Total);
        }

        public static decimal GetCompanyBalance(this BusinessAccount company)
        {
            var totalPaid = company.BusinessCorporateAccounts.Where(x => x.PaymentMethodId == (int)PaymentMethodEnum.Cash).Sum(x => x.Amount);

            var total = company.GetCompanyBalanceWithTax();

            return totalPaid - total;
        }


        public static decimal GetGuestBalanceWithFullTax(this Guest guest)
        {
            return guest.Payments.Where(x => x.PaymentMethodId == (int)PaymentMethodEnum.POSTBILL).Sum(x => x.Total);
        }

        public static decimal GetGuestBalance(this BusinessAccount company)
        {
            return company.GetCompanyBalanceWithTax();
        }

        private static int GetHotelTax()
        {
            int hTax = 0;

            try
            {
                int.TryParse(ConfigurationManager.AppSettings["HotelTax"].ToString(), out hTax);
            }
            catch
            {
                hTax = 0;
            }

            return hTax;
        }

        private static int GetRestaurantTax()
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

        public static decimal GetGuestBalanceOld(this Guest guest)
        {
            var totalRooms = guest.GuestRooms.Summation();

            var accountTotal = decimal.Zero;

            foreach (var rm in guest.GuestRooms.Where(x => x.GuestRoomAccounts.Sum(y => y.Amount) > 0))
            {
                accountTotal += rm.GuestRoomAccounts.Where(x => (x.PaymentMethodId == (int)PaymentMethodEnum.POSTBILL) 
                    || (x.PaymentTypeId == (int)RoomPaymentTypeEnum.CashDeposit 
                    || x.PaymentTypeId == (int)RoomPaymentTypeEnum.InitialDeposit 
                    || x.PaymentTypeId == (int)RoomPaymentTypeEnum.ReservationDeposit 
                    || x.PaymentTypeId == (int)RoomPaymentTypeEnum.HalfDay)).Summation();
            }

            var total = accountTotal - totalRooms;

            return total;
        }

        public static decimal GetGuestBalance(this Guest guest)
        {
            return guest.GetGuestBalanceWithFullTax();
        }

        public static decimal GetGuestTotalPaid(this Guest guest)
        {
            var totalRooms = guest.GuestRooms.Summation();

            var accountTotal = decimal.Zero;

            foreach (var rm in guest.GuestRooms.Where(x => x.GuestRoomAccounts.Sum(y => y.Amount) > 0))
            {
                accountTotal += rm.GuestRoomAccounts.Where(x => (x.PaymentMethodId == (int)PaymentMethodEnum.POSTBILL) 
                    || (x.PaymentTypeId == (int)RoomPaymentTypeEnum.CashDeposit 
                    || x.PaymentTypeId == (int)RoomPaymentTypeEnum.InitialDeposit 
                    || x.PaymentTypeId == (int)RoomPaymentTypeEnum.ReservationDeposit 
                    || x.PaymentTypeId == (int)RoomPaymentTypeEnum.HalfDay)).Summation();
            }

            var total = accountTotal - totalRooms;

            return total;
        }

        public static string GetBalance(this Guest guest)
        {
            //var totalRooms = guest.GuestRooms.Summation();

            //var accountTotal = decimal.Zero;

            //foreach (var rm in guest.GuestRooms.Where(x => x.GuestRoomAccounts.Sum(y => y.Amount) > 0))
            //{
            //    accountTotal += rm.GuestRoomAccounts.Where(x => (x.PaymentMethodId == (int)PaymentMethodEnum.POSTBILL) 
            //        || (x.PaymentTypeId == (int)RoomPaymentTypeEnum.CashDeposit 
            //        || x.PaymentTypeId == (int)RoomPaymentTypeEnum.InitialDeposit 
            //        || x.PaymentTypeId == (int)RoomPaymentTypeEnum.ReservationDeposit 
            //        || x.PaymentTypeId == (int)RoomPaymentTypeEnum.HalfDay)).Summation();
            //}

            //var total = accountTotal - totalRooms;

            var total = guest.GetGuestBalanceWithFullTax();

            if(total < 0)
                return guest.FullName + " has to pay a balance of NGN " + decimal.Negate(total).ToString(CultureInfo.InvariantCulture);
            else
            {
                if (total == 0)
                    return "Check out " + guest.FullName;

                return "Refund NGN " + total.ToString(CultureInfo.InvariantCulture) + " and Check out " + guest.FullName;
            }
        }

        public static decimal Summation(this IEnumerable<GuestRoomAccount> roomAccounts)
        {
            var total = decimal.Zero;

            foreach (var guestRoomAccount in roomAccounts.Where(x => (x.PaymentMethodId == (int)PaymentMethodEnum.POSTBILL)
                || (x.PaymentTypeId == (int)RoomPaymentTypeEnum.CashDeposit 
                || x.PaymentTypeId == (int)RoomPaymentTypeEnum.InitialDeposit 
                || x.PaymentTypeId == (int)RoomPaymentTypeEnum.ReservationDeposit 
                || x.PaymentTypeId == (int)RoomPaymentTypeEnum.HalfDay)))
            {
                if (guestRoomAccount.RoomPaymentType.PaymentStatusId == (int) RoomPaymentStatusEnum.Credit)
                    total += guestRoomAccount.Amount;
                else
                    total -= guestRoomAccount.Amount;
            }

            return total;
        }

        public static int GetNumberOfNightsFutureBooking(this GuestRoom guestRoom)
        {
            var hotelAccountsTime = 14;

            int.TryParse(ConfigurationManager.AppSettings["HotelAccountsTime"].ToString(CultureInfo.InvariantCulture), out hotelAccountsTime);

            var dtCheckoutDate = DateTime.Now;

            
            dtCheckoutDate = guestRoom.CheckoutDate;
            

            var extraDay = 0;

            if (DateTime.Now.Hour > hotelAccountsTime)
                extraDay = 1;

            //Use Exact Times to Calculate exact lenght of stay

            var exactCheckingDate = new DateTime(guestRoom.CheckinDate.Year, guestRoom.CheckinDate.Month, guestRoom.CheckinDate.Day, hotelAccountsTime, 0, 0);
            var exactCheckoutDate = new DateTime(dtCheckoutDate.Year, dtCheckoutDate.Month, dtCheckoutDate.Day, hotelAccountsTime, 0, 0);


            var totalNumberOfDays = exactCheckoutDate.Subtract(exactCheckingDate).Days + extraDay;

            totalNumberOfDays = (totalNumberOfDays == 0) ? 1 : totalNumberOfDays;

            return totalNumberOfDays;
        }

        public static int GetNumberOfNights(this GuestRoom guestRoom)
        {
            var hotelAccountsTime = 14;

            int.TryParse(ConfigurationManager.AppSettings["HotelAccountsTime"].ToString(CultureInfo.InvariantCulture), out hotelAccountsTime);

            var dtCheckoutDate = DateTime.Now;

            if (!guestRoom.IsActive)
            {
                dtCheckoutDate = guestRoom.CheckoutDate;
            }

            var extraDay = 0;

            if (DateTime.Now.Hour > hotelAccountsTime)
                extraDay = 1;

            //Use Exact Times to Calculate exact lenght of stay

            var exactCheckingDate = new DateTime(guestRoom.CheckinDate.Year, guestRoom.CheckinDate.Month, guestRoom.CheckinDate.Day, hotelAccountsTime, 0, 0);
            var exactCheckoutDate = new DateTime(dtCheckoutDate.Year, dtCheckoutDate.Month, dtCheckoutDate.Day, hotelAccountsTime, 0, 0);


            var totalNumberOfDays = exactCheckoutDate.Subtract(exactCheckingDate).Days + extraDay;

            totalNumberOfDays = (totalNumberOfDays == 0) ? 1 : totalNumberOfDays;

            return totalNumberOfDays;
        }

        public static decimal RoomCharge(this GuestRoom guestRoom)
        {            
            var hotelAccountsTime = 14;
        
            int.TryParse(ConfigurationManager.AppSettings["HotelAccountsTime"].ToString(CultureInfo.InvariantCulture), out hotelAccountsTime);

            var dtCheckoutDate = DateTime.Now;

            var guestIsStillActive = true;

            if (!guestRoom.IsActive)
            {
                dtCheckoutDate = guestRoom.CheckoutDate;
                guestIsStillActive = false;
            }

            var extraDay = 0;

            if(DateTime.Now.Hour > hotelAccountsTime)
                extraDay = 1;

            //Use Exact Times to Calculate exact lenght of stay
            var exactCheckingDate = new DateTime(guestRoom.CheckinDate.Year, guestRoom.CheckinDate.Month, guestRoom.CheckinDate.Day, hotelAccountsTime, 0, 0);

            var exactCheckoutDate = new DateTime(dtCheckoutDate.Year, dtCheckoutDate.Month, dtCheckoutDate.Day, hotelAccountsTime, 0, 0);

            var totalNumberOfDays = exactCheckoutDate.Subtract(exactCheckingDate).Days + extraDay;

            if (guestIsStillActive)
            {
                totalNumberOfDays = (totalNumberOfDays == 0) ? 1 : totalNumberOfDays;
            }

            return totalNumberOfDays * guestRoom.RoomRate;
        }


        public static decimal DebitSummation(this IEnumerable<GuestRoom> guestRooms)
        {
            return guestRooms.Sum(guestRoom => guestRoom.GuestRoomAccounts.Where(x => x.RoomPaymentType.PaymentStatusId == (int)RoomPaymentStatusEnum.Debit && (x.PaymentMethodId == (int)PaymentMethodEnum.POSTBILL) || (x.PaymentTypeId == (int)RoomPaymentTypeEnum.CashDeposit || x.PaymentTypeId == (int)RoomPaymentTypeEnum.InitialDeposit || x.PaymentTypeId == (int)RoomPaymentTypeEnum.ReservationDeposit)).Summation());
        }

        //
        public static decimal PercentageOccupancy(this IEnumerable<GuestRoom> guestRooms)
        {
            int totalNumberOfNights = guestRooms.Sum(guestRoom => guestRoom.GetNumberOfNights());
            if (totalNumberOfNights == 0)
                return decimal.Zero;

            decimal ave = totalNumberOfNights / 365 ;
            return ave;
        }

        public static decimal CreditSummation(this IEnumerable<GuestRoom> guestRooms)
        {
            return guestRooms.Sum(guestRoom => guestRoom.GuestRoomAccounts.Where(x => x.RoomPaymentType.PaymentStatusId == (int)RoomPaymentStatusEnum.Credit && (x.PaymentMethodId == (int)PaymentMethodEnum.POSTBILL) || (x.PaymentTypeId == (int)RoomPaymentTypeEnum.CashDeposit || x.PaymentTypeId == (int)RoomPaymentTypeEnum.InitialDeposit || x.PaymentTypeId == (int)RoomPaymentTypeEnum.ReservationDeposit)).Summation());
        }

        public static decimal CreditSummationCash(this IEnumerable<GuestRoom> guestRooms)
        {
            return guestRooms.Sum(guestRoom => guestRoom.GuestRoomAccounts.Where(x => x.RoomPaymentType.PaymentStatusId == (int)RoomPaymentStatusEnum.Credit && (x.PaymentMethodId == (int)PaymentMethodEnum.Cash) || (x.PaymentTypeId == (int)RoomPaymentTypeEnum.CashDeposit || x.PaymentTypeId == (int)RoomPaymentTypeEnum.InitialDeposit || x.PaymentTypeId == (int)RoomPaymentTypeEnum.ReservationDeposit)).Summation());
        }


        public static decimal Summation(this IEnumerable<GuestRoom> guestRooms)
        {
            return guestRooms.Sum(guestRoom => guestRoom.RoomCharge());
        }
    }
}