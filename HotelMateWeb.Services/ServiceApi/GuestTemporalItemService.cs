using HotelMateWeb.Dal;
using HotelMateWeb.Dal.DataCore;
using HotelMateWeb.Services.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelMateWeb.Services.ServiceApi
{

    public class GuestBillItemService : IGuestBillItemService
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();

        public IList<GuestBillItem> GetAll()
        {
            return _unitOfWork.GuestBillItemRepository.Get().ToList();
        }

        public GuestBillItem GetById(int? id)
        {
            return _unitOfWork.GuestBillItemRepository.GetByID(id.Value);
        }

        public GuestBillItem Update(GuestBillItem guestBillItem)
        {
            _unitOfWork.GuestBillItemRepository.Update(guestBillItem);
            _unitOfWork.Save();
            return guestBillItem;
        }

        public void Delete(GuestBillItem guestBillItem)
        {
            _unitOfWork.GuestBillItemRepository.Delete(guestBillItem);
            _unitOfWork.Save();
        }

        public GuestBillItem Create(GuestBillItem guestBillItem)
        {
            _unitOfWork.GuestBillItemRepository.Insert(guestBillItem);
            _unitOfWork.Save();
            return guestBillItem;
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


        public IQueryable<GuestBillItem> GetByQuery()
        {
            return _unitOfWork.GuestBillItemRepository.GetByQuery();
        }
    }

}
