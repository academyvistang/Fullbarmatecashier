using System;
using System.Collections.Generic;
using System.Linq;
using HotelMateWeb.Dal;
using HotelMateWeb.Dal.DataCore;
using HotelMateWeb.Services.Core;

namespace HotelMateWeb.Services.ServiceApi
{
    public class GuestRoomAccountService : IGuestRoomAccountService
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();

        public IList<GuestRoomAccount> GetAll(string includes)
        {
            return _unitOfWork.GuestRoomAccountRepository.Get(null,null,includes).ToList();
        }


        public IQueryable<GuestRoomAccount> GetAllForGuestByType(int? guestId, int? paymentTypeId)
        {
            return _unitOfWork.GuestRoomAccountRepository.GetByQuery(x => x.GuestRoom.GuestId == guestId.Value && x.PaymentTypeId == paymentTypeId.Value).OrderByDescending(x => x.TransactionDate);
        }

        public IList<GuestRoomAccount> GetAll(int hotelId)
        {
            return _unitOfWork.GuestRoomAccountRepository.Get(x => x.GuestRoom.Guest.HotelId == hotelId).ToList();
        }

        public GuestRoomAccount GetById(int? id)
        {
            return _unitOfWork.GuestRoomAccountRepository.GetByID(id.Value);
        }

        public GuestRoomAccount Update(GuestRoomAccount guestRoomAccount)
        {
            _unitOfWork.GuestRoomAccountRepository.Update(guestRoomAccount);
            _unitOfWork.Save();
            return guestRoomAccount;
        }

        public void Delete(GuestRoomAccount guestRoomAccount)
        {
            _unitOfWork.GuestRoomAccountRepository.Delete(guestRoomAccount);
            _unitOfWork.Save();
        }

        public GuestRoomAccount Create(GuestRoomAccount guestRoomAccount)
        {
            _unitOfWork.GuestRoomAccountRepository.Insert(guestRoomAccount);
            _unitOfWork.Save();
            return guestRoomAccount;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_unitOfWork != null)
                {
                    _unitOfWork.Dispose();
                    _unitOfWork = null;
                }
            }
        }


        public IQueryable<GuestRoomAccount> GetByQuery(int hotelId)
        {
            return _unitOfWork.GuestRoomAccountRepository.GetByQuery(x => x.GuestRoom.Guest.HotelId == hotelId);
        }
    }
}
