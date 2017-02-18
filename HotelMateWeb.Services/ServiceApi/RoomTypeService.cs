using HotelMateWeb.Dal;
using HotelMateWeb.Dal.DataCore;
using HotelMateWeb.Services.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelMateWeb.Services.ServiceApi
{
    public class RoomTypeService : IRoomTypeService
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();

        public IList<RoomType> GetAll(int hotelId)
        {
            return _unitOfWork.RoomTypeRepository.Get(x => x.HotelId == hotelId).ToList();
        }

        public RoomType GetById(int? id)
        {
            return _unitOfWork.RoomTypeRepository.GetByID(id.Value);
        }

        public RoomType Update(RoomType roomType)
        {
            _unitOfWork.RoomTypeRepository.Update(roomType);
            _unitOfWork.Save();
            return roomType;
        }

        public void Delete(RoomType roomType)
        {
            _unitOfWork.RoomTypeRepository.Delete(roomType);
            _unitOfWork.Save();
        }

        public RoomType Create(RoomType roomType)
        {
            _unitOfWork.RoomTypeRepository.Insert(roomType);
            _unitOfWork.Save();
            return roomType;
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

        public IQueryable<RoomType> GetByQuery(int hotelId)
        {
            return _unitOfWork.RoomTypeRepository.GetByQuery(x => x.HotelId == hotelId);
        }
    }
}
