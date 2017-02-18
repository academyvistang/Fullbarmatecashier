using HotelMateWeb.Dal;
using HotelMateWeb.Dal.DataCore;
using HotelMateWeb.Services.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelMateWeb.Services.ServiceApi
{

    public class StorePointItemService : IStorePointItemService
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();

        public IList<StorePointItem> GetAll()
        {
            return _unitOfWork.StorePointItemRepository.Get().ToList();
        }

        public StorePointItem GetById(int? id)
        {
            return _unitOfWork.StorePointItemRepository.GetByID(id.Value);
        }

        public StorePointItem Update(StorePointItem StorePointItem)
        {
            _unitOfWork.StorePointItemRepository.Update(StorePointItem);
            _unitOfWork.Save();
            return StorePointItem;
        }

        public void Delete(StorePointItem StorePointItem)
        {
            _unitOfWork.StorePointItemRepository.Delete(StorePointItem);
            _unitOfWork.Save();
        }

        public StorePointItem Create(StorePointItem StorePointItem)
        {
            _unitOfWork.StorePointItemRepository.Insert(StorePointItem);
            _unitOfWork.Save();
            return StorePointItem;
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


        public IQueryable<StorePointItem> GetByQuery()
        {
            return _unitOfWork.StorePointItemRepository.GetByQuery();
        }
    }
}
