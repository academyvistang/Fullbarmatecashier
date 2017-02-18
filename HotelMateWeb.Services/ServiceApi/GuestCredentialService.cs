using HotelMateWeb.Dal;
using HotelMateWeb.Dal.DataCore;
using HotelMateWeb.Services.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelMateWeb.Services.ServiceApi
{

    public class GuestCredentialService : IGuestCredentialService
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();

        public IList<GuestCredential> GetAll()
        {
            return _unitOfWork.GuestCredentialRepository.Get(null, null,"Person").ToList();
        }

        public GuestCredential GetById(int? id)
        {
            return _unitOfWork.GuestCredentialRepository.GetByID(id.Value);
        }

        public GuestCredential Update(GuestCredential guestCredential)
        {
            _unitOfWork.GuestCredentialRepository.Update(guestCredential);
            _unitOfWork.Save();
            return guestCredential;
        }

        public void Delete(GuestCredential guestCredential)
        {
            _unitOfWork.GuestCredentialRepository.Delete(guestCredential);
            _unitOfWork.Save();
        }

        public GuestCredential Create(GuestCredential guestCredential)
        {
            _unitOfWork.GuestCredentialRepository.Insert(guestCredential);
            _unitOfWork.Save();
            return guestCredential;
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


        public IQueryable<GuestCredential> GetByQuery()
        {
            return _unitOfWork.GuestCredentialRepository.GetByQuery();
        }
    }
}
