using HotelMateWeb.Dal;
using HotelMateWeb.Dal.DataCore;
using HotelMateWeb.Services.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelMateWeb.Services.ServiceApi
{

    public class PrinterTableService : IPrinterTableService
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();

        public IList<PrinterTable> GetAllInclude(string includes)
        {
            return _unitOfWork.PrinterTableRepository.Get(null, null, includes).ToList();
        }

        public IList<PrinterTable> GetAll()
        {
            return _unitOfWork.PrinterTableRepository.Get().ToList();
        }

        public PrinterTable GetById(int? id)
        {
            return _unitOfWork.PrinterTableRepository.GetByID(id.Value);
        }

        public PrinterTable Update(PrinterTable PrinterTable)
        {
            _unitOfWork.PrinterTableRepository.Update(PrinterTable);
            _unitOfWork.Save();
            return PrinterTable;
        }

        public void Delete(PrinterTable PrinterTable)
        {
            _unitOfWork.PrinterTableRepository.Delete(PrinterTable);
            _unitOfWork.Save();
        }

        public PrinterTable Create(PrinterTable PrinterTable)
        {
            _unitOfWork.PrinterTableRepository.Insert(PrinterTable);
            _unitOfWork.Save();
            return PrinterTable;
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


        public IQueryable<PrinterTable> GetByQuery()
        {
            return _unitOfWork.PrinterTableRepository.GetByQuery();
        }
    }
}
