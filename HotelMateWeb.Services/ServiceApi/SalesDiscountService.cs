using HotelMateWeb.Dal;
using HotelMateWeb.Dal.DataCore;
using HotelMateWeb.Services.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelMateWeb.Services.ServiceApi
{

    public class SalesDiscountService : ISalesDiscountService
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();

        public IList<SalesDiscount> GetAll()
        {
            return _unitOfWork.SalesDiscountRepository.Get().ToList();
        }

        public SalesDiscount GetById(int? id)
        {
            return _unitOfWork.SalesDiscountRepository.GetByID(id.Value);
        }

        public SalesDiscount Update(SalesDiscount salesDiscount)
        {
            _unitOfWork.SalesDiscountRepository.Update(salesDiscount);
            _unitOfWork.Save();
            return salesDiscount;
        }

        public void Delete(SalesDiscount salesDiscount)
        {
            _unitOfWork.SalesDiscountRepository.Delete(salesDiscount);
            _unitOfWork.Save();
        }

        public SalesDiscount Create(SalesDiscount salesDiscount)
        {
            _unitOfWork.SalesDiscountRepository.Insert(salesDiscount);
            _unitOfWork.Save();
            return salesDiscount;
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


        public IQueryable<SalesDiscount> GetByQuery()
        {
            return _unitOfWork.SalesDiscountRepository.GetByQuery();
        }
    }
}
