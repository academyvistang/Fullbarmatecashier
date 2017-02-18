using HotelMateWeb.Dal;
using HotelMateWeb.Dal.DataCore;
using HotelMateWeb.Services.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelMateWeb.Services.ServiceApi
{

    public class PaymentOrderService : IPaymentOrderService
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();

        public IList<PaymentOrder> GetAll()
        {
            return _unitOfWork.PaymentOrderRepository.Get().ToList();
        }

        public PaymentOrder GetById(int? id)
        {
            return _unitOfWork.PaymentOrderRepository.GetByID(id.Value);
        }

        public PaymentOrder Update(PaymentOrder item)
        {
            _unitOfWork.PaymentOrderRepository.Update(item);
            _unitOfWork.Save();
            return item;
        }

        public void Delete(PaymentOrder item)
        {
            _unitOfWork.PaymentOrderRepository.Delete(item);
            _unitOfWork.Save();
        }

        public PaymentOrder Create(PaymentOrder item)
        {
            _unitOfWork.PaymentOrderRepository.Insert(item);
            _unitOfWork.Save();
            return item;
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


        public IQueryable<PaymentOrder> GetByQuery()
        {
            return _unitOfWork.PaymentOrderRepository.GetByQuery();
        }
    }
}
