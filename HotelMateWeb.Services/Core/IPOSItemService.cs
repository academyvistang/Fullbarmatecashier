using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HotelMateWeb.Dal.DataCore;

namespace HotelMateWeb.Services.Core
{

    public interface IPOSItemService
    {
        IList<POSItem> GetAll();
        POSItem GetById(int? id);
        POSItem Update(POSItem POSItem);
        void Delete(POSItem POSItem);
        POSItem Create(POSItem POSItem);
        IQueryable<POSItem> GetByQuery();
        IList<POSItem> GetAllInclude(string includeProperties);
        void Dispose();

        //IList<POSItem> GetByInclude(string p);
    }

}
