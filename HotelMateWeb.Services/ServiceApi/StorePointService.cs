using HotelMateWeb.Dal;
using HotelMateWeb.Dal.DataCore;
using HotelMateWeb.Services.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelMateWeb.Services.ServiceApi
{

    public class StorePointService : IStorePointService
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();

        public IList<StorePoint> GetAll()
        {
            return _unitOfWork.StorePointRepository.Get().ToList();
        }

        public StorePoint GetById(int? id)
        {
            return _unitOfWork.StorePointRepository.GetByID(id.Value);
        }

        public StorePoint Update(StorePoint StorePoint)
        {
            _unitOfWork.StorePointRepository.Update(StorePoint);
            _unitOfWork.Save();
            return StorePoint;
        }

        public void Delete(StorePoint StorePoint)
        {
            _unitOfWork.StorePointRepository.Delete(StorePoint);
            _unitOfWork.Save();
        }

        public StorePoint Create(StorePoint StorePoint)
        {
            _unitOfWork.StorePointRepository.Insert(StorePoint);
            _unitOfWork.Save();
            return StorePoint;
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


        public IQueryable<StorePoint> GetByQuery()
        {
            return _unitOfWork.StorePointRepository.GetByQuery();
        }
    }
}
