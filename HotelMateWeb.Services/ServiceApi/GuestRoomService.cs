using System;
using System.Collections.Generic;
using System.Linq;
using HotelMateWeb.Dal;
using HotelMateWeb.Dal.DataCore;
using HotelMateWeb.Services.Core;

namespace HotelMateWeb.Services.ServiceApi
{

    public class GuestRoomService : IGuestRoomService
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();

        public IList<GuestRoom> GetAll(int hotelId)
        {
            return _unitOfWork.GuestRoomRepository.Get(x => x.Guest.HotelId == hotelId).ToList();
        }

        public GuestRoom GetById(int? id)
        {
            return _unitOfWork.GuestRoomRepository.GetByID(id.Value);
        }

        public GuestRoom Update(GuestRoom guestRoom)
        {
            _unitOfWork.GuestRoomRepository.Update(guestRoom);
            _unitOfWork.Save();
            return guestRoom;
        }

        public void Delete(GuestRoom guestRoom)
        {
            _unitOfWork.GuestRoomRepository.Delete(guestRoom);
            _unitOfWork.Save();
        }

        public GuestRoom Create(GuestRoom guestRoom)
        {
            _unitOfWork.GuestRoomRepository.Insert(guestRoom);
            _unitOfWork.Save();
            return guestRoom;
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

        public IQueryable<GuestRoom> GetByQuery(int hotelId)
        {
            return _unitOfWork.GuestRoomRepository.GetByQuery(x => x.Guest.HotelId == hotelId);
        }
    }
}
