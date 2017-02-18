using HotelMateWeb.Dal.DataCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HotelMateWeb.Services.Core
{
    public interface IGuestOrderService
    {
        IList<GuestOrder> GetAll(string includes);
        GuestOrder GetById(int? id);
        GuestOrder Update(GuestOrder guestOrder);
        void Delete(GuestOrder guestOrder);
        GuestOrder Create(GuestOrder guestOrder);
        IQueryable<GuestOrder> GetByQuery();
        void Dispose();
        GuestOrder GetByIdWithItems(int guestOrderId, string includes);
    }
}
