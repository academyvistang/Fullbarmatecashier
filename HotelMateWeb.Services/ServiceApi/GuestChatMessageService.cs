using HotelMateWeb.Dal;
using HotelMateWeb.Dal.DataCore;
using HotelMateWeb.Services.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelMateWeb.Services.ServiceApi
{

    public class GuestChatMessageService : IGuestChatMessageService
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();

        public IList<GuestChatMessage> GetAll()
        {
            return _unitOfWork.GuestChatMessageRepository.Get().ToList();
        }

        public GuestChatMessage GetById(int? id)
        {
            return _unitOfWork.GuestChatMessageRepository.GetByID(id.Value);
        }

        public GuestChatMessage Update(GuestChatMessage guestChatMessage)
        {
            _unitOfWork.GuestChatMessageRepository.Update(guestChatMessage);
            _unitOfWork.Save();
            return guestChatMessage;
        }

        public void Delete(GuestChatMessage guestChatMessage)
        {
            _unitOfWork.GuestChatMessageRepository.Delete(guestChatMessage);
            _unitOfWork.Save();
        }

        public GuestChatMessage Create(GuestChatMessage guestChatMessage)
        {
            _unitOfWork.GuestChatMessageRepository.Insert(guestChatMessage);
            _unitOfWork.Save();
            return guestChatMessage;
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


        public IQueryable<GuestChatMessage> GetByQuery()
        {
            return _unitOfWork.GuestChatMessageRepository.GetByQuery();
        }
    }
}
