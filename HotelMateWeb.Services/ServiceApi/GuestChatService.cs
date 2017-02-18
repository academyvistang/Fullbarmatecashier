using HotelMateWeb.Dal;
using HotelMateWeb.Dal.DataCore;
using HotelMateWeb.Services.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelMateWeb.Services.ServiceApi
{

    public class GuestChatService : IGuestChatService
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();

        public IList<GuestChat> GetAll()
        {
            return _unitOfWork.GuestChatRepository.Get().ToList();
        }

        public GuestChat GetById(int? id)
        {
            return _unitOfWork.GuestChatRepository.GetByID(id.Value);
        }

        public GuestChat Update(GuestChat guestChat)
        {
            _unitOfWork.GuestChatRepository.Update(guestChat);
            _unitOfWork.Save();
            return guestChat;
        }

        public void Delete(GuestChat guestChat)
        {
            _unitOfWork.GuestChatRepository.Delete(guestChat);
            _unitOfWork.Save();
        }

        public GuestChat Create(GuestChat guestChat)
        {
            _unitOfWork.GuestChatRepository.Insert(guestChat);
            _unitOfWork.Save();
            return guestChat;
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


        public IQueryable<GuestChat> GetByQuery()
        {
            return _unitOfWork.GuestChatRepository.GetByQuery();
        }
    }
}
