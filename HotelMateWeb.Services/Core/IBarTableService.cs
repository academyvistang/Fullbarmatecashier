using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HotelMateWeb.Dal.DataCore;

namespace HotelMateWeb.Services.Core
{

    public interface IBarTableService
    {
        IList<BarTable> GetAll();
        BarTable GetById(int? id);
        BarTable Update(BarTable barTable);
        void Delete(BarTable barTable);
        BarTable Create(BarTable barTable);
        IQueryable<BarTable> GetByQuery();
        void Dispose();
        IList<BarTable> GetAllInclude(string includes);
    }

}
