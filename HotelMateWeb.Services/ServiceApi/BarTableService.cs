using HotelMateWeb.Dal;
using HotelMateWeb.Dal.DataCore;
using HotelMateWeb.Services.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelMateWeb.Services.ServiceApi
{

    public class BarTableService : IBarTableService
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();

        public IList<BarTable> GetAllInclude(string includes)
        {
            return _unitOfWork.BarTableRepository.Get(null,null,includes).ToList();
        }

        public IList<BarTable> GetAll()
        {
            return _unitOfWork.BarTableRepository.Get().ToList();
        }

        public BarTable GetById(int? id)
        {
            return _unitOfWork.BarTableRepository.GetByID(id.Value);
        }

        public BarTable Update(BarTable barTable)
        {
            _unitOfWork.BarTableRepository.Update(barTable);
            _unitOfWork.Save();
            return barTable;
        }

        public void Delete(BarTable barTable)
        {
            _unitOfWork.BarTableRepository.Delete(barTable);
            _unitOfWork.Save();
        }

        public BarTable Create(BarTable barTable)
        {
            _unitOfWork.BarTableRepository.Insert(barTable);
            _unitOfWork.Save();
            return barTable;
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


        public IQueryable<BarTable> GetByQuery()
        {
            return _unitOfWork.BarTableRepository.GetByQuery();
        }
    }
}
