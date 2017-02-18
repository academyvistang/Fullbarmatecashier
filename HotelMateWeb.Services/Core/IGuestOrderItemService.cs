using HotelMateWeb.Dal.DataCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HotelMateWeb.Services.Core
{
    public interface IGuestOrderItemService
    {
        IList<GuestOrderItem> GetAll();
        GuestOrderItem GetById(int? id);
        GuestOrderItem Update(GuestOrderItem guestOrderItem);
        void Delete(GuestOrderItem guestOrderItem);
        GuestOrderItem Create(GuestOrderItem guestOrderItem);
        IQueryable<GuestOrderItem> GetByQuery();
        void Dispose();

        IList<GuestOrderItem> GetAllInclude(string include);
    }
}
