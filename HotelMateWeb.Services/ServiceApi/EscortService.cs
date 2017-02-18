using HotelMateWeb.Dal;
using HotelMateWeb.Dal.DataCore;
using HotelMateWeb.Services.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelMateWeb.Services.ServiceApi
{
    public class EscortService : IEscortService
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();

        public IList<Escort> GetAllForLogin()
        {
            return _unitOfWork.EscortRepository.Get().ToList();
        }

        public IList<Escort> GetAll(int hotelId)
        {
            return _unitOfWork.EscortRepository.Get().ToList();
        }

        public Escort GetEscortByUserNameAndPassword(string domainUsername, string password)
        {
            return null;
        }


        public Escort GetById(int? id)
        {
            return _unitOfWork.EscortRepository.GetByID(id.Value);
        }

        public Escort Update(Escort escort)
        {
            _unitOfWork.EscortRepository.Update(escort);
            _unitOfWork.Save();
            return escort;
        }

        public void Delete(Escort escort)
        {
            _unitOfWork.EscortRepository.Delete(escort);
            _unitOfWork.Save();
        }

        public Escort Create(Escort escort)
        {
            _unitOfWork.EscortRepository.Insert(escort);
            _unitOfWork.Save();
            return escort;
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


        public IQueryable<Escort> GetByQuery(int hotelId)
        {
            return _unitOfWork.EscortRepository.GetByQuery();
        }
    }
}
