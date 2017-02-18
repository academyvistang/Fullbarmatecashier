using HotelMateWeb.Dal;
using HotelMateWeb.Dal.DataCore;
using HotelMateWeb.Services.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelMateWeb.Services.ServiceApi
{

    public class StoreService : IStoreService
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();

        public IList<Store> GetAll()
        {
            return _unitOfWork.StoreRepository.Get().ToList();
        }

        public Store GetById(int? id)
        {
            return _unitOfWork.StoreRepository.GetByID(id.Value);
        }

        public Store Update(Store store)
        {
            _unitOfWork.StoreRepository.Update(store);
            _unitOfWork.Save();
            return store;
        }

        public void Delete(Store store)
        {
            _unitOfWork.StoreRepository.Delete(store);
            _unitOfWork.Save();
        }

        public Store Create(Store store)
        {
            _unitOfWork.StoreRepository.Insert(store);
            _unitOfWork.Save();
            return store;
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


        public IQueryable<Store> GetByQuery()
        {
            return _unitOfWork.StoreRepository.GetByQuery();
        }
    }
}
