using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HotelMateWeb.Dal.DataCore;

namespace HotelMateWeb.Services.Core
{

    public interface ISuspendItemService
    {
        IList<SuspendItem> GetAll();
        IList<SuspendItem> GetAll(string includes);
        SuspendItem GetById(int? id);
        SuspendItem Update(SuspendItem SuspendItem);
        void Delete(SuspendItem SuspendItem);
        SuspendItem Create(SuspendItem SuspendItem);
        IQueryable<SuspendItem> GetByQuery();
        void Dispose();
    }

}
