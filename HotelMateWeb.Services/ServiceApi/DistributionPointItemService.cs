﻿using HotelMateWeb.Dal;
using HotelMateWeb.Dal.DataCore;
using HotelMateWeb.Services.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelMateWeb.Services.ServiceApi
{

    public class DistributionPointItemService : IDistributionPointItemService
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();

        public IList<DistributionPointItem> GetAll()
        {
            return _unitOfWork.DistributionPointItemRepository.Get().ToList();
        }

        public DistributionPointItem GetById(int? id)
        {
            return _unitOfWork.DistributionPointItemRepository.GetByID(id.Value);
        }

        public DistributionPointItem Update(DistributionPointItem distributionPointItem)
        {
            _unitOfWork.DistributionPointItemRepository.Update(distributionPointItem);
            _unitOfWork.Save();
            return distributionPointItem;
        }

        public void Delete(DistributionPointItem distributionPointItem)
        {
            _unitOfWork.DistributionPointItemRepository.Delete(distributionPointItem);
            _unitOfWork.Save();
        }

        public DistributionPointItem Create(DistributionPointItem distributionPointItem)
        {
            _unitOfWork.DistributionPointItemRepository.Insert(distributionPointItem);
            _unitOfWork.Save();
            return distributionPointItem;
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


        public IQueryable<DistributionPointItem> GetByQuery()
        {
            return _unitOfWork.DistributionPointItemRepository.GetByQuery();
        }
    }
}
