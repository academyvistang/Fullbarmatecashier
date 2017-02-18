using HotelMateWeb.Dal;
using HotelMateWeb.Dal.DataCore;
using HotelMateWeb.Services.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelMateWeb.Services.ServiceApi
{

    public class InvoiceStatusTypeService : IInvoiceStatusTypeService
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();

        public IList<InvoiceStatusType> GetAll()
        {
            return _unitOfWork.InvoiceStatusTypeRepository.Get().ToList();
        }

        public InvoiceStatusType GetById(int? id)
        {
            return _unitOfWork.InvoiceStatusTypeRepository.GetByID(id.Value);
        }

        public InvoiceStatusType Update(InvoiceStatusType invoiceStatusType)
        {
            _unitOfWork.InvoiceStatusTypeRepository.Update(invoiceStatusType);
            _unitOfWork.Save();
            return invoiceStatusType;
        }

        public void Delete(InvoiceStatusType invoiceStatusType)
        {
            _unitOfWork.InvoiceStatusTypeRepository.Delete(invoiceStatusType);
            _unitOfWork.Save();
        }

        public InvoiceStatusType Create(InvoiceStatusType invoiceStatusType)
        {
            _unitOfWork.InvoiceStatusTypeRepository.Insert(invoiceStatusType);
            _unitOfWork.Save();
            return invoiceStatusType;
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


        public IQueryable<InvoiceStatusType> GetByQuery()
        {
            return _unitOfWork.InvoiceStatusTypeRepository.GetByQuery();
        }
    }
}
