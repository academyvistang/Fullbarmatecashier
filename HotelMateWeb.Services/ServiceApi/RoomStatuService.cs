using HotelMateWeb.Dal;
using HotelMateWeb.Dal.DataCore;
using HotelMateWeb.Services.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelMateWeb.Services.ServiceApi
{
    public class RoomStatuService : IRoomStatuService
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();

        public IList<RoomStatu> GetAll(int hotelId)
        {
            return _unitOfWork.RoomStatusRepository.Get().ToList();
        }

        public RoomStatu GetById(int? id)
        {
            return _unitOfWork.RoomStatusRepository.GetByID(id.Value);
        }

        public RoomStatu Update(RoomStatu roomStatu)
        {
            _unitOfWork.RoomStatusRepository.Update(roomStatu);
            _unitOfWork.Save();
            return roomStatu;
        }

        public void Delete(RoomStatu roomStatu)
        {
            _unitOfWork.RoomStatusRepository.Delete(roomStatu);
            _unitOfWork.Save();
        }

        public RoomStatu Create(RoomStatu roomStatu)
        {
            _unitOfWork.RoomStatusRepository.Insert(roomStatu);
            _unitOfWork.Save();
            return roomStatu;
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

        public IQueryable<RoomStatu> GetByQuery(int hotelId)
        {
            return _unitOfWork.RoomStatusRepository.GetByQuery();
        }
    }
}
