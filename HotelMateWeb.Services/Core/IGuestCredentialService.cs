using HotelMateWeb.Dal.DataCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HotelMateWeb.Services.Core
{
    public interface IGuestCredentialService
    {
        IList<GuestCredential> GetAll();
        GuestCredential GetById(int? id);
        GuestCredential Update(GuestCredential guestCredential);
        void Delete(GuestCredential TableItem);
        GuestCredential Create(GuestCredential guestCredential);
        IQueryable<GuestCredential> GetByQuery();
        void Dispose();
    }
}
