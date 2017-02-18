using System;
using System.Collections.Generic;
using System.Linq;
using HotelMateWeb.Dal;
using HotelMateWeb.Dal.DataCore;
using HotelMateWeb.Services.Core;

namespace HotelMateWeb.Services.ServiceApi
{
    public class BusinessCorporateAccountService : IBusinessAccountCorporateService
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();

        public IQueryable<BusinessCorporateAccount> GetAllForCompanyByType(int? companyId, int? paymentmethodId)
        {
            return null;
        }

        public IList<BusinessCorporateAccount> GetAllInclde(string include)
        {
            return _unitOfWork.BusinessCorporateAccountRepository.GetByQuery(null, null, include).ToList();
        }

        public IList<BusinessCorporateAccount> GetAll(int hotelId)
        {
            return _unitOfWork.BusinessCorporateAccountRepository.GetByQuery(null, null, "PaymentMethod,Person").ToList();
        }

        public BusinessCorporateAccount GetById(int? id)
        {
            return _unitOfWork.BusinessCorporateAccountRepository.GetByID(id.Value);
        }

        public BusinessCorporateAccount Update(BusinessCorporateAccount businessCorporateAccount)
        {
            _unitOfWork.BusinessCorporateAccountRepository.Update(businessCorporateAccount);
            _unitOfWork.Save();
            return businessCorporateAccount;
        }

        public void Delete(BusinessCorporateAccount businessCorporateAccount)
        {
            _unitOfWork.BusinessCorporateAccountRepository.Delete(businessCorporateAccount);
            _unitOfWork.Save();
        }

        public BusinessCorporateAccount Create(BusinessCorporateAccount businessCorporateAccount)
        {
            _unitOfWork.BusinessCorporateAccountRepository.Insert(businessCorporateAccount);
            _unitOfWork.Save();
            return businessCorporateAccount;
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


        public IQueryable<BusinessCorporateAccount> GetByQuery(int hotelId)
        {
            return _unitOfWork.BusinessCorporateAccountRepository.GetByQuery(x => x.BusinessAccount.HotelId == hotelId);
        }
    }
}
