using HotelMateWeb.Dal.DataCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HotelMateWeb.Services.Core
{
    public interface IGuestChatMessageService
    {
        IList<GuestChatMessage> GetAll();
        GuestChatMessage GetById(int? id);
        GuestChatMessage Update(GuestChatMessage guestChatMessage);
        void Delete(GuestChatMessage guestChatMessage);
        GuestChatMessage Create(GuestChatMessage guestChatMessage);
        IQueryable<GuestChatMessage> GetByQuery();
        void Dispose();
    }
}
