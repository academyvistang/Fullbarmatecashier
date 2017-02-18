using System;
using System.Collections.Generic;
using System.Linq;
using HotelMateWeb.Dal;
using HotelMateWeb.Dal.DataCore;
using HotelMateWeb.Services.Core;

namespace HotelMateWeb.Services.ServiceApi
{
    public class BusinessAccountService : IBusinessAccountService
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();

        public IList<BusinessAccount> GetAll(int hotelId)
        {
            return _unitOfWork.BusinessAccountRepository.Get(x => x.HotelId == hotelId).ToList();
        }

        public IList<BusinessAccount> GetAllincludes(string include)
        {
            return _unitOfWork.BusinessAccountRepository.GetByQuery(null, null, include).ToList();
        }


        public BusinessAccount GetById(int? id)
        {
            return _unitOfWork.BusinessAccountRepository.GetByID(id.Value);
        }

        public BusinessAccount Update(BusinessAccount businessAccount)
        {
            _unitOfWork.BusinessAccountRepository.Update(businessAccount);
            _unitOfWork.Save();
            return businessAccount;
        }

        public void Delete(BusinessAccount businessAccount)
        {
            _unitOfWork.BusinessAccountRepository.Delete(businessAccount);
            _unitOfWork.Save();
        }

        public BusinessAccount Create(BusinessAccount businessAccount)
        {
            _unitOfWork.BusinessAccountRepository.Insert(businessAccount);
            _unitOfWork.Save();
            return businessAccount;
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


        public IQueryable<BusinessAccount> GetByQuery(int hotelId)
        {
            return _unitOfWork.BusinessAccountRepository.GetByQuery(x => x.HotelId == hotelId);
        }
    }
}
