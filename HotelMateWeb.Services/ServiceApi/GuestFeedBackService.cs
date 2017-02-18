using HotelMateWeb.Dal;
using HotelMateWeb.Dal.DataCore;
using HotelMateWeb.Services.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelMateWeb.Services.ServiceApi
{

    public class GuestFeedBackService : IGuestFeedBackService
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();

        public IList<GuestFeedBack> GetAll()
        {
            return _unitOfWork.GuestFeedBackRepository.GetByQuery(null,null,"Guest").ToList();
        }

        public GuestFeedBack GetById(int? id)
        {
            return _unitOfWork.GuestFeedBackRepository.GetByID(id.Value);
        }

        public GuestFeedBack Update(GuestFeedBack guestFeedBack)
        {
            _unitOfWork.GuestFeedBackRepository.Update(guestFeedBack);
            _unitOfWork.Save();
            return guestFeedBack;
        }

        public void Delete(GuestFeedBack guestFeedBack)
        {
            _unitOfWork.GuestFeedBackRepository.Delete(guestFeedBack);
            _unitOfWork.Save();
        }

        public GuestFeedBack Create(GuestFeedBack guestFeedBack)
        {
            _unitOfWork.GuestFeedBackRepository.Insert(guestFeedBack);
            _unitOfWork.Save();
            return guestFeedBack;
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


        public IQueryable<GuestFeedBack> GetByQuery()
        {
            return _unitOfWork.GuestFeedBackRepository.GetByQuery();
        }
    }
}
