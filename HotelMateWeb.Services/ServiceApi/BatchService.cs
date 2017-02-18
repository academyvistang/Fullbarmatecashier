using HotelMateWeb.Dal;
using HotelMateWeb.Dal.DataCore;
using HotelMateWeb.Services.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelMateWeb.Services.ServiceApi
{

    public class BatchService : IBatchService
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();

        public IList<Batch> GetAll()
        {
            return _unitOfWork.BatchRepository.Get(null, null, "").ToList();
        }

        public Batch GetById(int? id)
        {
            return _unitOfWork.BatchRepository.GetByID(id.Value);
        }

        public Batch Update(Batch batch)
        {
            _unitOfWork.BatchRepository.Update(batch);
            _unitOfWork.Save();
            return batch;
        }

        public void Delete(Batch batch)
        {
            _unitOfWork.BatchRepository.Delete(batch);
            _unitOfWork.Save();
        }

        public Batch Create(Batch batch)
        {
            _unitOfWork.BatchRepository.Insert(batch);
            _unitOfWork.Save();
            return batch;
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


        public IQueryable<Batch> GetByQuery()
        {
            return _unitOfWork.BatchRepository.GetByQuery();
        }
    }
}
