using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HotelMateWeb.Dal.DataCore;

namespace BarAndRestaurantMate.Models
{
    public class MenuModel
    {
        public int Id { get; set; }

        public List<HotelMateWeb.Dal.DataCore.POSItem> Items { get; set; }

        public object CategoryName { get; set; }
    }

    public class GuestOrderModel
    {
        public HotelMateWeb.Dal.DataCore.GuestOrder ActualGuestOrder { get; set; }

        public List<HotelMateWeb.Dal.DataCore.GuestRequestItem> Items { get; set; }

        public HotelMateWeb.Dal.DataCore.Guest ActualGuest { get; set; }

        public int Id { get; set; }

        public List<HotelMateWeb.Dal.DataCore.GuestOrderItem> BillItems { get; set; }

        public HotelMateWeb.Dal.DataCore.GuestOrder ActualGuestBill { get; set; }

        public List<HotelMateWeb.Dal.DataCore.GuestOrderItem> OrderItems { get; set; }

        public HotelMateWeb.Dal.DataCore.BarTable RealTable { get; set; }
    }

    public class HotelMenuModel
    {

        public IEnumerable<SelectListItem> selectList { get; set; }

        public int CategoryId { get; set; }

        public int NewCashierId { get; set; }
        

        public int Id { get; set; }

        public List<HotelMateWeb.Dal.DataCore.POSItem> Items { get; set; }

        public List<MenuModel> Menu { get; set; }

        public HotelMateWeb.Dal.DataCore.POSItem MenuItem { get; set; }

        public List<HotelMateWeb.Dal.DataCore.Taxi> Taxis { get; set; }

        public HotelMateWeb.Dal.DataCore.Taxi CarItem { get; set; }

        public List<HotelMateWeb.Dal.DataCore.Adventure> Adventures { get; set; }

        public HotelMateWeb.Dal.DataCore.Adventure Adventure { get; set; }

        public string BookingAgentNumber { get; set; }

        public int RecargeCardPrice { get; set; }

        public List<HotelMateWeb.Dal.DataCore.Escort> Escorts { get; set; }

        public HotelMateWeb.Dal.DataCore.Escort EscortItem { get; set; }

        public ItemModel Item { get; set; }

        public ItemModel ItemModel { get; set; }

        public List<ItemModel> ItemsOrdered { get; set; }

        public List<HotelMateWeb.Dal.DataCore.GuestOrderItem> ActualItems { get; set; }

        public List<string> AvailableTables { get; set; }

        public IEnumerable<System.Web.Mvc.SelectListItem> AvailableTablesSelectList { get; set; }

        [Required(ErrorMessage="Please select a table")]
        [Range(double.MinValue, double.MaxValue,ErrorMessage = "Please select a table")]
        public int TableId { get; set; }

        [Required(ErrorMessage = "Please select a table")]
        [Range(double.MinValue, double.MaxValue, ErrorMessage = "Please select a table")]
        public int RealTableId { get; set; }

        [Required(ErrorMessage = "Please enter a Guest Name")]
        public string GuestName { get; set; }

        [Required(ErrorMessage = "Please select a table")]
        [Range(double.MinValue, double.MaxValue, ErrorMessage = "Please select a table")]
        public int ExistingTableId { get; set; }

        public int GuestOrderId { get; set; }

        public int GuestId { get; set; }

        public List<HotelMateWeb.Dal.DataCore.GuestOrder> MyTables { get; set; }

        public IEnumerable<SelectListItem> CanSelectList { get; set; }

        public IEnumerable<SelectListItem> CanSelectListExistingTable { get; set; }


        public IEnumerable<SelectListItem> CanSelectListPersons { get; set; }

        public IEnumerable<SelectListItem> CanSelectListPersonsExistingTable { get; set; }

        public int MemberId { get; set; }

        public int ExistingTableMemberId { get; set; }


        public string GuestCredentials { get; set; }

        public int NewlyCreatedTable { get; set; }

        public List<HotelMateWeb.Dal.DataCore.GuestBillItem> GuestBillItems { get; set; }

        public int AwaitingATable { get; set; }

        public int ActualItemsCount { get; set; }

        public int SendFinalConfirmation { get; set; }

        public List<HotelMateWeb.Dal.DataCore.GuestOrder> AllOpenOrders { get; set; }

        public bool CanSeeProcessButton { get; set; }

        public List<HotelMateWeb.Dal.DataCore.GuestRequestItem> RequestedItems { get; set; }

        public bool CanAddItems { get; set; }

        public List<HotelMateWeb.Dal.DataCore.GuestOrderItem> GuestOrderItems { get; set; }

        public List<GuestOrderModel> AllOpenOrdersGroupBy { get; set; }

        public int CanaddItemInt { get; set; }

        public int NewOrderPlaced { get; set; }

        public int WaitressId { get; set; }

        public int GuestChatId { get; set; }

        public string PicturePath { get; set; }

        public List<HotelMateWeb.Dal.DataCore.GuestChat> AllChatters { get; set; }

        public int NewMember { get; set; }

        public HotelMateWeb.Dal.DataCore.Guest Owner { get; set; }

        public HotelMateWeb.Dal.DataCore.Guest Stranger { get; set; }

        public List<HotelMateWeb.Dal.DataCore.GuestChatMessage> Messages { get; set; }

        public int Joining { get; set; }

        public List<GuestOrderModel> AllOpenBillsGroupBy { get; set; }

        public int SendInvite { get; set; }

        public string InvitedGuest { get; set; }

        public List<HotelMateWeb.Dal.DataCore.Guest> PendingRequests { get; set; }

        public int AcceptedToChat { get; set; }

        public int SendViewToGuest { get; set; }

        public int RequestOrderId { get; set; }

        public int ProcessingComplete { get; set; }

        public int DontJoin { get; set; }

        public int TableJustOpened { get; set; }

        public int IamAWaitress { get; set; }

        public int WaitressAddItem { get; set; }

        public int ExistingMember { get; set; }

        public List<HotelMateWeb.Dal.DataCore.GuestMessage> GuestMessages { get; set; }

        public int Messaging { get; set; }

        public string Message { get; set; }

        public string GuestOrderNote { get; set; }

        public int RepliedACustomer { get; set; }

        public int MessageId { get; set; }

        public int JustOpenedTable { get; set; }

        public string TableAlias { get; set; }

        public List<HotelMateWeb.Dal.DataCore.GuestOrder> UnusedTables { get; set; }

        public List<ReceiptModel> Receipts { get; set; }

        public List<POSService.Entities.BusinessAccount> Accountlst { get; set; }

        public string ReceiptNumber { get; set; }

        public int CompanyId { get; set; }

        public List<POSService.Entities.BusinessAccount> CurrentBusinessAccounts { get; set; }

        public List<HotelMateWeb.Dal.DataCore.SchoolPicture> Pictures { get; set; }

        public int ExistingStaffId { get; set; }

        public int ToThisStaff { get; set; }

        public IEnumerable<SelectListItem> AllSelectListWaiters { get; set; }

        public List<TabOrderModel> TabOrderModelList { get; set; }

        public HotelMateWeb.Dal.DataCore.TableItem TableItem { get; set; }

        public List<LatestRoomViewModel> RoomsTypes { get; set; }

        public List<POSService.Entities.Guest> CurrentGuests { get; set; }

        public bool CashierCanOpenTable { get; set; }

        public bool ClubTime { get; set; }

        public List<HotelMateWeb.Dal.DataCore.BarTable> AllOpenTables { get; set; }
        public IEnumerable<SelectListItem> AvailableCashiers { get; set; }
    }

    public class EscortModel
    {
        [Required(ErrorMessage="Please enter a name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter a telephone number")]
        public string Telephone { get; set; }

        public string Mobile { get; set; }

        [Required(ErrorMessage = "Please enter a Description")]
        public string Description { get; set; }

        public bool Saved { get; set; }
    }
}