using HotelMateWeb.Dal;
using HotelMateWeb.Dal.DataCore;
using HotelMateWeb.Services.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelMateWeb.Services.ServiceApi
{

    public class POSItemService : IPOSItemService
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();

        public IList<POSItem> GetAllInclude(string includeProperties)
        {
            return _unitOfWork.POSItemRepository.GetByQuery(x => x.IsActive, null, includeProperties).ToList();
        }

        public IList<POSItem> GetAll()
        {
            return _unitOfWork.POSItemRepository.Get(x => x.IsActive).ToList();
        }

        public POSItem GetById(int? id)
        {
            return _unitOfWork.POSItemRepository.GetByID(id.Value);
        }

        public POSItem Update(POSItem pOSItem)
        {
            _unitOfWork.POSItemRepository.Update(pOSItem);
            _unitOfWork.Save();
            return pOSItem;
        }

        public void Delete(POSItem pOSItem)
        {
            _unitOfWork.POSItemRepository.Delete(pOSItem);
            _unitOfWork.Save();
        }

        public POSItem Create(POSItem pOSItem)
        {
            _unitOfWork.POSItemRepository.Insert(pOSItem);
            _unitOfWork.Save();
            return pOSItem;
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


        public IQueryable<POSItem> GetByQuery()
        {
            return _unitOfWork.POSItemRepository.GetByQuery();
        }
    }
}
