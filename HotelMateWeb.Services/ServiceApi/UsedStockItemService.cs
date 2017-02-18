using HotelMateWeb.Dal;
using HotelMateWeb.Dal.DataCore;
using HotelMateWeb.Services.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelMateWeb.Services.ServiceApi
{

    public class UsedStockItemService : IUsedStockItemService
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();

        public IList<UsedStockItem> GetAll()
        {
            return _unitOfWork.UsedStockItemRepository.Get().ToList();
        }

        public UsedStockItem GetById(int? id)
        {
            return _unitOfWork.UsedStockItemRepository.GetByID(id.Value);
        }

        public UsedStockItem Update(UsedStockItem usedStockUsedStockItem)
        {
            _unitOfWork.UsedStockItemRepository.Update(usedStockUsedStockItem);
            _unitOfWork.Save();
            return usedStockUsedStockItem;
        }

        public void Delete(UsedStockItem usedStockUsedStockItem)
        {
            _unitOfWork.UsedStockItemRepository.Delete(usedStockUsedStockItem);
            _unitOfWork.Save();
        }

        public UsedStockItem Create(UsedStockItem usedStockUsedStockItem)
        {
            _unitOfWork.UsedStockItemRepository.Insert(usedStockUsedStockItem);
            _unitOfWork.Save();
            return usedStockUsedStockItem;
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


        public IQueryable<UsedStockItem> GetByQuery()
        {
            return _unitOfWork.UsedStockItemRepository.GetByQuery();
        }
    }
}
