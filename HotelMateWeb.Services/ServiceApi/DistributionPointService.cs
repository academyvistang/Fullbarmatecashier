using HotelMateWeb.Dal;
using HotelMateWeb.Dal.DataCore;
using HotelMateWeb.Services.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelMateWeb.Services.ServiceApi
{

    public class DistributionPointService : IDistributionPointService
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();

        public IList<DistributionPoint> GetAll()
        {
            return _unitOfWork.DistributionPointRepository.Get().ToList();
        }

        public DistributionPoint GetById(int? id)
        {
            return _unitOfWork.DistributionPointRepository.GetByID(id.Value);
        }

        public DistributionPoint Update(DistributionPoint distributionPoint)
        {
            _unitOfWork.DistributionPointRepository.Update(distributionPoint);
            _unitOfWork.Save();
            return distributionPoint;
        }

        public void Delete(DistributionPoint distributionPoint)
        {
            _unitOfWork.DistributionPointRepository.Delete(distributionPoint);
            _unitOfWork.Save();
        }

        public DistributionPoint Create(DistributionPoint distributionPoint)
        {
            _unitOfWork.DistributionPointRepository.Insert(distributionPoint);
            _unitOfWork.Save();
            return distributionPoint;
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


        public IQueryable<DistributionPoint> GetByQuery()
        {
            return _unitOfWork.DistributionPointRepository.GetByQuery();
        }
    }
}
