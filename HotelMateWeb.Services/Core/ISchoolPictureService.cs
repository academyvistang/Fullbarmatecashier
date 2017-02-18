using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HotelMateWeb.Dal.DataCore;

namespace HotelMateWeb.Services.Core
{

    public interface ISchoolPictureService
    {
        IList<SchoolPicture> GetAll();
        SchoolPicture GetById(int? id);
        SchoolPicture Update(SchoolPicture SchoolPicture);
        void Delete(SchoolPicture SchoolPicture);
        SchoolPicture Create(SchoolPicture SchoolPicture);
        IQueryable<SchoolPicture> GetByQuery();
        void Dispose();
    }

}
