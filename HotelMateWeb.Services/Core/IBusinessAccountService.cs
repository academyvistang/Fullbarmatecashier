using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HotelMateWeb.Dal.DataCore;

namespace HotelMateWeb.Services.Core
{

    public interface IBusinessAccountService
    {
        IList<BusinessAccount> GetAll(int hotelId);
        IQueryable<BusinessAccount> GetByQuery(int hotelId);
        BusinessAccount GetById(int? id);
        BusinessAccount Update(BusinessAccount businessAccount);
        void Delete(BusinessAccount businessAccount);
        BusinessAccount Create(BusinessAccount businessAccount);
        void Dispose();

        IList<BusinessAccount> GetAllincludes(string include);
    }
}
