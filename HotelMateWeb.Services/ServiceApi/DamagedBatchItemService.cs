﻿using HotelMateWeb.Dal;
using HotelMateWeb.Dal.DataCore;
using HotelMateWeb.Services.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelMateWeb.Services.ServiceApi
{

    public class DamagedBatchItemService : IDamagedBatchItemService
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();

        public IList<DamagedBatchItem> GetAll()
        {
            return _unitOfWork.DamagedBatchItemRepository.Get().ToList();
        }

        public DamagedBatchItem GetById(int? id)
        {
            return _unitOfWork.DamagedBatchItemRepository.GetByID(id.Value);
        }

        public DamagedBatchItem Update(DamagedBatchItem damagedBatchItem)
        {
            _unitOfWork.DamagedBatchItemRepository.Update(damagedBatchItem);
            _unitOfWork.Save();
            return damagedBatchItem;
        }

        public void Delete(DamagedBatchItem damagedBatchItem)
        {
            _unitOfWork.DamagedBatchItemRepository.Delete(damagedBatchItem);
            _unitOfWork.Save();
        }

        public DamagedBatchItem Create(DamagedBatchItem damagedBatchItem)
        {
            _unitOfWork.DamagedBatchItemRepository.Insert(damagedBatchItem);
            _unitOfWork.Save();
            return damagedBatchItem;
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


        public IQueryable<DamagedBatchItem> GetByQuery()
        {
            return _unitOfWork.DamagedBatchItemRepository.GetByQuery();
        }
    }
}
