using HotelMateWeb.Dal.DataCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HotelMateWeb.Services.Core
{
    public interface IGuestMessageService
    {
        IList<GuestMessage> GetAll();
        GuestMessage GetById(int? id);
        GuestMessage Update(GuestMessage guestMessage);
        void Delete(GuestMessage guestMessage);
        GuestMessage Create(GuestMessage guestMessage);
        IQueryable<GuestMessage> GetByQuery();
        void Dispose();
        IList<GuestMessage> GetAllInclude(string includes);
    }
}
