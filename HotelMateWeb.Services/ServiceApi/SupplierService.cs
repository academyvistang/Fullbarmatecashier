using HotelMateWeb.Dal;
using HotelMateWeb.Dal.DataCore;
using HotelMateWeb.Services.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelMateWeb.Services.ServiceApi
{
    public class SupplierService : ISupplierService
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();

        public IList<Supplier> GetAll(int hotelId)
        {
            return _unitOfWork.SupplierRepository.Get().ToList();
        }

        public Supplier GetById(int? id)
        {
            return _unitOfWork.SupplierRepository.GetByID(id.Value);
        }

        public Supplier Update(Supplier supplier)
        {
            _unitOfWork.SupplierRepository.Update(supplier);
            _unitOfWork.Save();
            return supplier;
        }

        public void Delete(Supplier supplier)
        {
            _unitOfWork.SupplierRepository.Delete(supplier);
            _unitOfWork.Save();
        }

        public Supplier Create(Supplier supplier)
        {
            _unitOfWork.SupplierRepository.Insert(supplier);
            _unitOfWork.Save();
            return supplier;
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

        public IQueryable<Supplier> GetByQuery(int hotelId)
        {
            return _unitOfWork.SupplierRepository.GetByQuery();
        }
    }
}
