using HotelMateWeb.Dal;
using HotelMateWeb.Dal.DataCore;
using HotelMateWeb.Services.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelMateWeb.Services.ServiceApi
{

    public class SuspendItemService : ISuspendItemService
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();

        public IList<SuspendItem> GetAll(string includes)
        {
            return _unitOfWork.SuspendItemRepository.Get(null, null, includes).ToList();
        }

        public IList<SuspendItem> GetAll()
        {
            return _unitOfWork.SuspendItemRepository.Get().ToList();
        }

        public SuspendItem GetById(int? id)
        {
            return _unitOfWork.SuspendItemRepository.GetByID(id.Value);
        }

        public SuspendItem Update(SuspendItem suspendItem)
        {
            _unitOfWork.SuspendItemRepository.Update(suspendItem);
            _unitOfWork.Save();
            return suspendItem;
        }

        public void Delete(SuspendItem suspendItem)
        {
            _unitOfWork.SuspendItemRepository.Delete(suspendItem);
            _unitOfWork.Save();
        }

        public SuspendItem Create(SuspendItem suspendItem)
        {
            _unitOfWork.SuspendItemRepository.Insert(suspendItem);
            _unitOfWork.Save();
            return suspendItem;
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


        public IQueryable<SuspendItem> GetByQuery()
        {
            return _unitOfWork.SuspendItemRepository.GetByQuery();
        }


    }
}
