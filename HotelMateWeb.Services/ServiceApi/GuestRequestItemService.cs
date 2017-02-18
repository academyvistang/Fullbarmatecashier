using HotelMateWeb.Dal;
using HotelMateWeb.Dal.DataCore;
using HotelMateWeb.Services.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelMateWeb.Services.ServiceApi
{

    public class GuestRequestItemService : IGuestRequestItemService
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();

        public IList<GuestRequestItem> GetAllInclude(string includeProperties)
        {
            return _unitOfWork.GuestRequestItemRepository.GetByQuery(null, null, includeProperties).ToList();
        }

        public IList<GuestRequestItem> GetAll()
        {
            return _unitOfWork.GuestRequestItemRepository.Get().ToList();
        }

        public GuestRequestItem GetById(int? id)
        {
            return _unitOfWork.GuestRequestItemRepository.GetByID(id.Value);
        }

        public GuestRequestItem Update(GuestRequestItem guestRequestItem)
        {
            _unitOfWork.GuestRequestItemRepository.Update(guestRequestItem);
            _unitOfWork.Save();
            return guestRequestItem;
        }

        public void Delete(GuestRequestItem guestRequestItem)
        {
            _unitOfWork.GuestRequestItemRepository.Delete(guestRequestItem);
            _unitOfWork.Save();
        }

        public GuestRequestItem Create(GuestRequestItem guestRequestItem)
        {
            _unitOfWork.GuestRequestItemRepository.Insert(guestRequestItem);
            _unitOfWork.Save();
            return guestRequestItem;
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


        public IQueryable<GuestRequestItem> GetByQuery()
        {
            return _unitOfWork.GuestRequestItemRepository.GetByQuery();
        }
    }

}
