using HotelMateWeb.Dal.DataCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HotelMateWeb.Services.Core
{
    public interface IGuestChatService
    {
        IList<GuestChat> GetAll();
        GuestChat GetById(int? id);
        GuestChat Update(GuestChat guestChat);
        void Delete(GuestChat guestChat);
        GuestChat Create(GuestChat guestChat);
        IQueryable<GuestChat> GetByQuery();
        void Dispose();
    }
}
