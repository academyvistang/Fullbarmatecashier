using HotelMateWeb.Dal;
using HotelMateWeb.Dal.DataCore;
using HotelMateWeb.Services.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelMateWeb.Services.ServiceApi
{

    public class FoodMatrixService : IFoodMatrixService
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();

        public IList<FoodMatrix> GetAll()
        {
            return _unitOfWork.FoodMatrixRepository.GetByQuery(null, null, "StockItem,StockItem1").ToList();
        }

        public FoodMatrix GetById(int? id)
        {
            return _unitOfWork.FoodMatrixRepository.GetByID(id.Value);
        }

        public FoodMatrix Update(FoodMatrix foodMatrix)
        {
            _unitOfWork.FoodMatrixRepository.Update(foodMatrix);
            _unitOfWork.Save();
            return foodMatrix;
        }

        public void Delete(FoodMatrix foodMatrix)
        {
            _unitOfWork.FoodMatrixRepository.Delete(foodMatrix);
            _unitOfWork.Save();
        }

        public FoodMatrix Create(FoodMatrix foodMatrix)
        {
            _unitOfWork.FoodMatrixRepository.Insert(foodMatrix);
            _unitOfWork.Save();
            return foodMatrix;
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


        public IQueryable<FoodMatrix> GetByQuery()
        {
            return _unitOfWork.FoodMatrixRepository.GetByQuery();
        }
    }
}
