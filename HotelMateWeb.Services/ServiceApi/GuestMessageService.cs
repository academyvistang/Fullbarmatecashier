using HotelMateWeb.Dal;
using HotelMateWeb.Dal.DataCore;
using HotelMateWeb.Services.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelMateWeb.Services.ServiceApi
{

    public class GuestMessageService : IGuestMessageService
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();

        public IList<GuestMessage> GetAll()
        {
            return _unitOfWork.GuestMessageRepository.Get().ToList();
        }

        public IList<GuestMessage> GetAllInclude(string includes)
        {
            return _unitOfWork.GuestMessageRepository.Get(null,null,includes).ToList();
        }

        public GuestMessage GetById(int? id)
        {
            return _unitOfWork.GuestMessageRepository.GetByID(id.Value);
        }

        public GuestMessage Update(GuestMessage guestMessage)
        {
            _unitOfWork.GuestMessageRepository.Update(guestMessage);
            _unitOfWork.Save();
            return guestMessage;
        }

        public void Delete(GuestMessage guestMessage)
        {
            _unitOfWork.GuestMessageRepository.Delete(guestMessage);
            _unitOfWork.Save();
        }

        public GuestMessage Create(GuestMessage guestMessage)
        {
            _unitOfWork.GuestMessageRepository.Insert(guestMessage);
            _unitOfWork.Save();
            return guestMessage;
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


        public IQueryable<GuestMessage> GetByQuery()
        {
            return _unitOfWork.GuestMessageRepository.GetByQuery();
        }
    }
}
