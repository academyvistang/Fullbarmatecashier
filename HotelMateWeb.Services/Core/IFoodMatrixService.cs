using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HotelMateWeb.Dal.DataCore;

namespace HotelMateWeb.Services.Core
{

    public interface IFoodMatrixService
    {
        IList<FoodMatrix> GetAll();
        FoodMatrix GetById(int? id);
        FoodMatrix Update(FoodMatrix foodMatrix);
        void Delete(FoodMatrix foodMatrix);
        FoodMatrix Create(FoodMatrix foodMatrix);
        IQueryable<FoodMatrix> GetByQuery();
        void Dispose();
    }

}
