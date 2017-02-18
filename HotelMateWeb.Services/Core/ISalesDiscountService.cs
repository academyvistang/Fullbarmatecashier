using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HotelMateWeb.Dal.DataCore;

namespace HotelMateWeb.Services.Core
{

    public interface ISalesDiscountService
    {
        IList<SalesDiscount> GetAll();
        SalesDiscount GetById(int? id);
        SalesDiscount Update(SalesDiscount salesDiscount);
        void Delete(SalesDiscount salesDiscount);
        SalesDiscount Create(SalesDiscount salesDiscount);
        IQueryable<SalesDiscount> GetByQuery();
        void Dispose();
    }

}
