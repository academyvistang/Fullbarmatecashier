using AutoMapper;
using HotelMateWeb.Dal.DataCore;
using HotelMateWeb.Services.Core;
using HotelMateWeb.Services.ServiceApi;
using BarAndRestaurantMate.Helpers.Enums;
using BarAndRestaurantMate.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BarAndRestaurantMate.Helpers;

namespace BarAndRestaurantMate.Controllers
{
    [HandleError(View = "CustomErrorView")]
    public class CompanyAccountController : Controller
    {        
        private readonly IGuestService _guestService;
        private readonly IRoomService _roomService;
        private readonly IGuestRoomService _guestRoomService;
        private readonly IGuestReservationService _guestReservationService;
        private readonly IGuestRoomAccountService _guestRoomAccountService;
        private readonly int _hotelAccountsTime = 14;
        private readonly IBusinessAccountService _businessAccountService;
        private readonly IPersonService _personService = null;
        private readonly IPersonTypeService _personTypeService = null;
        private readonly IBusinessAccountCorporateService _businessAccountCorporateService = null;
        private readonly IPaymentService _paymentService = null;

        private int? _hotelId;
        private int HotelID
        {
            get { return _hotelId ?? GetHotelId(); }
            set { _hotelId = value; }
        }

        public object Id { get; private set; }

        private int GetHotelId()
        {
            var username = User.Identity.Name;
            var user = _personService.GetAllForLogin().FirstOrDefault(x => x.Username.Equals(username, StringComparison.CurrentCultureIgnoreCase));
            return user.HotelId;
        }


        [HttpPost]
        public ActionResult New(CompanyViewModel model)
        {
            //SearchFor Existing NameValueCollectionExtensions

            var alreadyCreated = _businessAccountService.GetAll(1).FirstOrDefault(x => x.Name.ToUpper().Equals(model.Name.ToUpper()));

            if(model.Id == 0)
            {
                if (alreadyCreated != null)
                {
                    ModelState.AddModelError("_FORM", "This Account name already exists. Please reenter account name");
                }
            }
            else
            {
                if (alreadyCreated != null && alreadyCreated.Id != model.Id)
                {
                    ModelState.AddModelError("_FORM", "This Account name already exists. Please reenter account name");
                }

            }

           

            if (ModelState.IsValid)
            {
                Mapper.CreateMap<CompanyViewModel, BusinessAccount>();
                BusinessAccount company = Mapper.Map<CompanyViewModel, BusinessAccount>(model);

                if (model.Id == 0)
                {
                    company.HotelId = HotelID;
                    company.CompanyTelephone = company.Telephone;
                    company.CompanyAddress = company.CompanyAddress;
                    company.Status = "LIVE";
                    company.Debtor = false;
                    _businessAccountService.Create(company);
                }
                else
                {
                    var existingCompany = _businessAccountService.GetById(model.Id);
                    existingCompany.Email = model.Email;
                    existingCompany.Name = model.Name;
                    existingCompany.ContactName = model.ContactName;
                    existingCompany.Address = model.Address;
                    existingCompany.CompanyAddress = model.CompanyAddress;
                    existingCompany.CompanyName = model.CompanyName;
                    existingCompany.CompanyTelephone = model.CompanyTelephone;
                    existingCompany.ContactName = model.ContactName;
                    existingCompany.ContactNumber = model.ContactNumber;
                    existingCompany.NatureOfBusiness = model.NatureOfBusiness;
                    existingCompany.Telephone = model.Telephone;
                    existingCompany.Mobile = model.Mobile;

                    _businessAccountService.Update(existingCompany);
                }

                return RedirectToAction("Edit", new { id = company.Id, itemSaved = true });
            }

            
            return View(model);
        }

        public ActionResult View(int? id, bool? itemSaved)
        {
            if (System.Configuration.ConfigurationManager.AppSettings["IsHotel"].ToString() == "1")
            {
                var existingCompany = _businessAccountService.GetById(id.Value);
                var allItemisedItems = existingCompany.Guests.SelectMany(x => x.SoldItemsAlls.Where(y => y.IsActive && y.PaymentMethodId == (int)PaymentMethodEnum.POSTBILL).OrderByDescending(y => y.DateSold).ToList()).ToList();
                var allPaymentsMade = _businessAccountCorporateService.GetAll(HotelID).Where(x => x.BusinessAccountId == id.Value).ToList();

                var model = new SearchViewModel
                {
                    Company = existingCompany,
                    AllPaymentsMade = allPaymentsMade.Where(x => x.PaymentMethodId == (int)PaymentMethodEnum.Cash).ToList(),
                    RoomAccounts = existingCompany.GuestRooms.SelectMany(x => x.GuestRoomAccounts).ToList(),
                    ItemmisedItems = allItemisedItems
                };

                return View(model);
            }
            else
            {
                var existingCompany = _businessAccountService.GetAllincludes("SoldItemsAlls,SoldItemsAlls.StockItem,SoldItemsAlls.Person,BusinessCorporateAccounts").FirstOrDefault(x => x.Id == id.Value);
                var allItemisedItems = existingCompany.SoldItemsAlls.Where(y => y.IsActive).ToList();
                var allPaymentsMade = _businessAccountCorporateService.GetAll(HotelID).Where(x => x.BusinessAccountId == id.Value).ToList();

                var model = new SearchViewModel
                {
                    Company = existingCompany,
                    AllPaymentsMade = allPaymentsMade.Where(x => x.PaymentMethodId == (int)PaymentMethodEnum.Cash).ToList(),
                    RoomAccounts = existingCompany.GuestRooms.SelectMany(x => x.GuestRoomAccounts).ToList(),
                    ItemmisedItems = allItemisedItems
                };

                return View("RestauantView",model);
            }
           
 
        }

        [HttpGet]
        public ActionResult Edit(int? id, bool? itemSaved)
        {
            var existingCompany = _businessAccountService.GetById(id.Value);
            
            Mapper.CreateMap<BusinessAccount, CompanyViewModel>();
            var pvm = Mapper.Map<BusinessAccount, CompanyViewModel>(existingCompany);
            pvm.ItemSaved = itemSaved;
          
            return View("New", pvm);
        }

        public CompanyAccountController()
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
            _businessAccountCorporateService = new BusinessCorporateAccountService();
            _paymentService = new PaymentService();
        }



        public ActionResult TopUpAccount(int? id, bool? itemSaved, int? paymentMethodId, string paymentMethodNote)
        {
            var model = new CorporateModel
            {
                Company = _businessAccountService.GetAll(HotelID).FirstOrDefault(x => x.Id == id.Value),
                ItemSaved = itemSaved,
                PaymentMethodId = PaymentMethodEnum.Cash,
                PaymentMethodNote = paymentMethodNote,
                BusinessCorporateAccount = new BusinessCorporateAccount()
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult TopUpAccount(CorporateModel model, int? paymentMethodId, string paymentMethodNote)
        {
            var company = _businessAccountService.GetAll(HotelID).FirstOrDefault(x => x.Id == model.Company.Id);
            var username = User.Identity.Name;
            var user = _personService.GetAllForLogin().FirstOrDefault(x => x.Username.Equals(username, StringComparison.CurrentCultureIgnoreCase));

            if (ModelState.IsValid)
            {
                BusinessCorporateAccount bca = new BusinessCorporateAccount{

                    Amount = model.BusinessCorporateAccount.Amount,
                    TransactionDate = DateTime.Now,
                    TransactionId = user.PersonID,
                    PaymentMethodId = paymentMethodId.HasValue ? paymentMethodId.Value : 1, PaymentMethodNote = paymentMethodNote,
                    BusinessAccountId = company.Id
                };


                _businessAccountCorporateService.Create(bca);

                return RedirectToAction("TopUpAccount", "CompanyAccount", new { id = model.Company.Id, itemSaved = true, paymentMethodId, paymentMethodNote });
            }

            var newModel = new CorporateModel
            {
                Company = company,
                ItemSaved = false,
                PaymentMethodId = PaymentMethodEnum.Cash,
                PaymentMethodNote = paymentMethodNote,
                BusinessCorporateAccount = new BusinessCorporateAccount()
            };

            return View(newModel);
        }

        //id = rm.GuestId, roomId = rm.Room.Id, companyId = Model.Company.Id

        public ActionResult CheckOutGuestCreateDebtorAccount(int? id, int? roomId, int? companyId)
        {
            var guest = _guestService.GetById(id.Value);           

            if (companyId.Value > 0)
            {

                var guestBalance = guest.GetGuestBalance();

                //At some point record the refund if any made to guest
                var reservationIds = guest.GuestReservations.Select(x => x.Id).ToList();
                var guestRoomsIds = guest.GuestRooms.Select(x => x.Id).ToList();
                var roomIds = guest.GuestReservations.Select(x => x.RoomId).ToList();

                //Create a new GuestRoom Account for any refund

                foreach (var existingReservation in reservationIds.Select(rsId => _guestReservationService.GetById(rsId)))
                {
                    existingReservation.EndDate = DateTime.Now;
                    existingReservation.IsActive = false;
                    _guestReservationService.Update(existingReservation);
                }

                foreach (var existingGuestRoom in guestRoomsIds.Select(gsId => _guestRoomService.GetById(gsId)))
                {
                    existingGuestRoom.CheckoutDate = DateTime.Now;
                    existingGuestRoom.IsActive = false;
                    _guestRoomService.Update(existingGuestRoom);
                }

                foreach (var existingRoom in roomIds.Select(rmId => _roomService.GetById(rmId)))
                {
                    existingRoom.StatusId = (int)RoomStatusEnum.Dirty;
                    _roomService.Update(existingRoom);
                }

                guest.IsActive = false;

                guest.Status = "PAST";

                guest.CompanyId = companyId;

                _guestService.Update(guest);

                return RedirectToAction("PrintLandingForGuest", "Home", new { id = id.Value });
            }
            else
            {
                return RedirectToAction("CheckOutGuest", "Home", new { id = id.Value, roomId = roomId.Value });
            }
        }

        public ActionResult TransferGuestBill(int? id, int? roomId, int? companyId)
        {
            var model = new SearchViewModel
            {
                Company = _businessAccountService.GetAll(HotelID).FirstOrDefault(x => x.Id == id.Value),
                GuestRoomsList = _guestRoomService.GetAll(HotelID).Where(x => x.IsActive).ToList()
            };

            return View(model);
        }
        public ActionResult TransferBill(int? id)
        {
            var model = new SearchViewModel
            {
                Company = _businessAccountService.GetAll(HotelID).FirstOrDefault(x => x.Id == id.Value),
                GuestRoomsList = _guestRoomService.GetAll(HotelID).Where(x => x.IsActive).ToList()
            };

            return View(model);
        }

        
        public ActionResult AddPayment(int? id)
        {
            var model = new SearchViewModel
            {
                Id = id.Value,
                ReceiptNumber = string.Empty
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult AddPayment(int? id, string receiptNumber)
        {
            if (!string.IsNullOrEmpty(receiptNumber) && id.HasValue)
            {
                var lst = _paymentService.GetAll().Where(x => x.ReceiptNumber.ToUpper().Equals(receiptNumber.ToUpper())).ToList();

                foreach (var p in lst)
                {
                    p.BusinessAccountId = id.Value;
                    _paymentService.Update(p);
                }
            }

            return RedirectToAction("Index");
        }

        public ActionResult IndexTransfer()
        {
            var model = new SearchViewModel
            {
                CompanyList = _businessAccountService.GetAll(HotelID).ToList()
            };

            return View(model);
        }
        public ActionResult Index()
        {
            var model = new SearchViewModel
            {
                CompanyList = _businessAccountService.GetAll(HotelID).OrderBy(x => x.CompanyName).ToList()
            };

            return View(model);
        }


        public ActionResult New()
        {
            Mapper.CreateMap<BusinessAccount, CompanyViewModel>();
            var cvm = Mapper.Map<BusinessAccount, CompanyViewModel>(new BusinessAccount { Id = 0 });            
            return View(cvm);
        }
	}
}