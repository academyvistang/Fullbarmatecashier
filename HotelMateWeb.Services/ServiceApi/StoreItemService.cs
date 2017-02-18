﻿using HotelMateWeb.Dal;
using HotelMateWeb.Dal.DataCore;
using HotelMateWeb.Services.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelMateWeb.Services.ServiceApi
{

    public class StoreItemService : IStoreItemService
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();

        public IList<StoreItem> GetAll()
        {
            return _unitOfWork.StoreItemRepository.Get().ToList();
        }

        public StoreItem GetById(int? id)
        {
            return _unitOfWork.StoreItemRepository.GetByID(id.Value);
        }

        public StoreItem Update(StoreItem storeItem)
        {
            _unitOfWork.StoreItemRepository.Update(storeItem);
            _unitOfWork.Save();
            return storeItem;
        }

        public void Delete(StoreItem storeItem)
        {
            _unitOfWork.StoreItemRepository.Delete(storeItem);
            _unitOfWork.Save();
        }

        public StoreItem Create(StoreItem storeItem)
        {
            _unitOfWork.StoreItemRepository.Insert(storeItem);
            _unitOfWork.Save();
            return storeItem;
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


        public IQueryable<StoreItem> GetByQuery()
        {
            return _unitOfWork.StoreItemRepository.GetByQuery();
        }
    }
}
