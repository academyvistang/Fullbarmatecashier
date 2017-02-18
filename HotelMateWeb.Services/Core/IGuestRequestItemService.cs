using HotelMateWeb.Dal.DataCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HotelMateWeb.Services.Core
{
    public interface IGuestRequestItemService
    {
        IList<GuestRequestItem> GetAll();
        GuestRequestItem GetById(int? id);
        GuestRequestItem Update(GuestRequestItem guestRequestItem);
        void Delete(GuestRequestItem guestRequestItem);
        GuestRequestItem Create(GuestRequestItem guestRequestItem);
        IQueryable<GuestRequestItem> GetByQuery();

        IList<GuestRequestItem> GetAllInclude(string includeProperties);
        void Dispose();
    }
}
