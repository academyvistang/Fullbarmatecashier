using HotelMateWeb.Dal;
using HotelMateWeb.Dal.DataCore;
using HotelMateWeb.Services.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelMateWeb.Services.ServiceApi
{

    public class GuestOrderService : IGuestOrderService
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();

        public IList<GuestOrder> GetAll(string includes)
        {
            return _unitOfWork.GuestOrderRepository.Get(null, null, includes).ToList();
        }

        public GuestOrder GetByIdWithItems(int guestOrderId, string includes)
        {
            return _unitOfWork.GuestOrderRepository.Get(null, null, includes).FirstOrDefault(x => x.Id == guestOrderId);
        }


        public GuestOrder GetById(int? id)
        {
            return _unitOfWork.GuestOrderRepository.GetByID(id.Value);
        }

        public GuestOrder Update(GuestOrder guestOrder)
        {
            _unitOfWork.GuestOrderRepository.Update(guestOrder);
            _unitOfWork.Save();
            return guestOrder;
        }

        public void Delete(GuestOrder guestOrder)
        {
            _unitOfWork.GuestOrderRepository.Delete(guestOrder);
            _unitOfWork.Save();
        }

        public GuestOrder Create(GuestOrder guestOrder)
        {
            _unitOfWork.GuestOrderRepository.Insert(guestOrder);
            _unitOfWork.Save();
            return guestOrder;
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


        public IQueryable<GuestOrder> GetByQuery()
        {
            return _unitOfWork.GuestOrderRepository.GetByQuery();
        }
    }
}
