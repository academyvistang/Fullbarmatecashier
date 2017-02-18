using HotelMateWeb.Dal;
using HotelMateWeb.Dal.DataCore;
using HotelMateWeb.Services.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelMateWeb.Services.ServiceApi
{

    public class StockActualItemService : IStockItemService
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();

        public IList<StockItem> GetAll()
        {
            return _unitOfWork.StockItemRepository.Get().ToList();
        }

        public StockItem GetById(int? id)
        {
            return _unitOfWork.StockItemRepository.GetByID(id.Value);
        }

        public StockItem Update(StockItem stockItem)
        {
            _unitOfWork.StockItemRepository.Update(stockItem);
            _unitOfWork.Save();
            return stockItem;
        }

        public void Delete(StockItem stockItem)
        {
            _unitOfWork.StockItemRepository.Delete(stockItem);
            _unitOfWork.Save();
        }

        public StockItem Create(StockItem stockItem)
        {
            _unitOfWork.StockItemRepository.Insert(stockItem);
            _unitOfWork.Save();
            return stockItem;
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


        public IQueryable<StockItem> GetByQuery()
        {
            return _unitOfWork.StockItemRepository.GetByQuery();
        }
    }
}
