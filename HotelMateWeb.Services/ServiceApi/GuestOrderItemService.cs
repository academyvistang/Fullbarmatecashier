using HotelMateWeb.Dal;
using HotelMateWeb.Dal.DataCore;
using HotelMateWeb.Services.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelMateWeb.Services.ServiceApi
{

    public class GuestOrderItemService : IGuestOrderItemService
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();

        public IList<GuestOrderItem> GetAllInclude(string include)
        {
            return _unitOfWork.GuestOrderItemRepository.GetByQuery(null,null,include).ToList();
        }


        public IList<GuestOrderItem> GetAll()
        {
            return _unitOfWork.GuestOrderItemRepository.Get().ToList();
        }

        public GuestOrderItem GetById(int? id)
        {
            return _unitOfWork.GuestOrderItemRepository.GetByID(id.Value);
        }

        public GuestOrderItem Update(GuestOrderItem guestOrderItem)
        {
            _unitOfWork.GuestOrderItemRepository.Update(guestOrderItem);
            _unitOfWork.Save();
            return guestOrderItem;
        }

        public void Delete(GuestOrderItem guestOrderItem)
        {
            _unitOfWork.GuestOrderItemRepository.Delete(guestOrderItem);
            _unitOfWork.Save();
        }

        public GuestOrderItem Create(GuestOrderItem guestOrderItem)
        {
            _unitOfWork.GuestOrderItemRepository.Insert(guestOrderItem);
            _unitOfWork.Save();
            return guestOrderItem;
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


        public IQueryable<GuestOrderItem> GetByQuery()
        {
            return _unitOfWork.GuestOrderItemRepository.GetByQuery();
        }
    }

}
