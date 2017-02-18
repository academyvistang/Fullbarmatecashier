using HotelMateWeb.Dal;
using HotelMateWeb.Dal.DataCore;
using HotelMateWeb.Services.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelMateWeb.Services.ServiceApi
{

    public class PurchaseOrderService : IPurchaseOrderService
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();

        public IList<PurchaseOrder> GetAll()
        {
            return _unitOfWork.PurchaseOrderRepository.Get(x => !x.Completed).ToList();
        }

        public IList<PurchaseOrder> GetAll(string includes)
        {
            return _unitOfWork.PurchaseOrderRepository.Get().ToList();
        }

        public IList<PurchaseOrder> GetAllByInclude(string includes)
        {
            return _unitOfWork.PurchaseOrderRepository.GetByQuery(null,null,includes).ToList();
        }


        public PurchaseOrder GetByIdInclude(int id, string include)
        {
            return _unitOfWork.PurchaseOrderRepository.GetByQuery(null, null, include).FirstOrDefault(x => x.Id == id);
        }


        public PurchaseOrder GetById(int? id)
        {
            return _unitOfWork.PurchaseOrderRepository.GetByID(id.Value);
        }

        public PurchaseOrder Update(PurchaseOrder purchaseOrder)
        {
            _unitOfWork.PurchaseOrderRepository.Update(purchaseOrder);
            _unitOfWork.Save();
            return purchaseOrder;
        }

        public void Delete(PurchaseOrder purchaseOrder)
        {
            _unitOfWork.PurchaseOrderRepository.Delete(purchaseOrder);
            _unitOfWork.Save();
        }

        public PurchaseOrder Create(PurchaseOrder purchaseOrder)
        {
            _unitOfWork.PurchaseOrderRepository.Insert(purchaseOrder);
            _unitOfWork.Save();
            return purchaseOrder;
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


        public IQueryable<PurchaseOrder> GetByQuery()
        {
            return _unitOfWork.PurchaseOrderRepository.GetByQuery();
        }
    }
}
