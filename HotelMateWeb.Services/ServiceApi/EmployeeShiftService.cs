using HotelMateWeb.Dal;
using HotelMateWeb.Dal.DataCore;
using HotelMateWeb.Services.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelMateWeb.Services.ServiceApi
{
    public class EmployeeShiftService : IEmployeeShiftService
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();

        public IList<EmployeeShift> GetAll(int hotelId)
        {
            return _unitOfWork.EmployeeShiftRepository.Get().ToList();
        }

        public EmployeeShift GetById(int? id)
        {
            return _unitOfWork.EmployeeShiftRepository.GetByID(id.Value);
        }

        public EmployeeShift Update(EmployeeShift employeeShift)
        {
            _unitOfWork.EmployeeShiftRepository.Update(employeeShift);
            _unitOfWork.Save();
            return employeeShift;
        }

        public void Delete(EmployeeShift employeeShift)
        {
            _unitOfWork.EmployeeShiftRepository.Delete(employeeShift);
            _unitOfWork.Save();
        }

        public EmployeeShift Create(EmployeeShift employeeShift)
        {
            _unitOfWork.EmployeeShiftRepository.Insert(employeeShift);
            _unitOfWork.Save();
            return employeeShift;
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

        public IQueryable<EmployeeShift> GetByQuery(int hotelId)
        {
            return _unitOfWork.EmployeeShiftRepository.GetByQuery();
        }
    }
}
