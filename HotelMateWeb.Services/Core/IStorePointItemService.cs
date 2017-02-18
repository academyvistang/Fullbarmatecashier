using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HotelMateWeb.Dal.DataCore;

namespace HotelMateWeb.Services.Core
{

    public interface IStorePointItemService
    {
        IList<StorePointItem> GetAll();
        StorePointItem GetById(int? id);
        StorePointItem Update(StorePointItem StorePointItem);
        void Delete(StorePointItem StorePointItem);
        StorePointItem Create(StorePointItem StorePointItem);
        IQueryable<StorePointItem> GetByQuery();
        void Dispose();
    }

}
