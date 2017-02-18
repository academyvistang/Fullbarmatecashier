using HotelMateWeb.Dal.DataCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HotelMateWeb.Services.Core
{
    public interface IGuestBillItemService
    {
        IList<GuestBillItem> GetAll();
        GuestBillItem GetById(int? id);
        GuestBillItem Update(GuestBillItem guestBillItem);
        void Delete(GuestBillItem guestBillItem);
        GuestBillItem Create(GuestBillItem guestBillItem);
        IQueryable<GuestBillItem> GetByQuery();
        void Dispose();
    }
}
