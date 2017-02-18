using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HotelMateWeb.Dal.DataCore;

namespace HotelMateWeb.Services.Core
{

    public interface IUsedStockItemService
    {
        IList<UsedStockItem> GetAll();
        UsedStockItem GetById(int? id);
        UsedStockItem Update(UsedStockItem usedStockItem);
        void Delete(UsedStockItem usedStockItem);
        UsedStockItem Create(UsedStockItem usedStockItem);
        IQueryable<UsedStockItem> GetByQuery();
        void Dispose();
    }

}
