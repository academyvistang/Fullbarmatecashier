using HotelMateWeb.Dal.DataCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HotelMateWeb.Services.Core
{
    public interface IGuestFeedBackService
    {
        IList<GuestFeedBack> GetAll();
        GuestFeedBack GetById(int? id);
        GuestFeedBack Update(GuestFeedBack guestFeedBack);
        void Delete(GuestFeedBack guestFeedBack);
        GuestFeedBack Create(GuestFeedBack guestFeedBack);
        IQueryable<GuestFeedBack> GetByQuery();
        void Dispose();
    }
}
