using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HotelMateWeb.Dal.DataCore;

namespace HotelMateWeb.Services.Core
{

    public interface IStorePointService
    {
        IList<StorePoint> GetAll();
        StorePoint GetById(int? id);
        StorePoint Update(StorePoint StorePoint);
        void Delete(StorePoint StorePoint);
        StorePoint Create(StorePoint StorePoint);
        IQueryable<StorePoint> GetByQuery();
        void Dispose();
    }

}
