using HotelMateWeb.Dal;
using HotelMateWeb.Dal.DataCore;
using HotelMateWeb.Services.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelMateWeb.Services.ServiceApi
{

    public class SchoolPictureService : ISchoolPictureService
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();

        public IList<SchoolPicture> GetAll()
        {
            return _unitOfWork.SchoolPictureRepository.Get().ToList();
        }

        public SchoolPicture GetById(int? id)
        {
            return _unitOfWork.SchoolPictureRepository.GetByID(id.Value);
        }

        public SchoolPicture Update(SchoolPicture schoolPicture)
        {
            _unitOfWork.SchoolPictureRepository.Update(schoolPicture);
            _unitOfWork.Save();
            return schoolPicture;
        }

        public void Delete(SchoolPicture schoolPicture)
        {
            _unitOfWork.SchoolPictureRepository.Delete(schoolPicture);
            _unitOfWork.Save();
        }

        public SchoolPicture Create(SchoolPicture schoolPicture)
        {
            _unitOfWork.SchoolPictureRepository.Insert(schoolPicture);
            _unitOfWork.Save();
            return schoolPicture;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_unitOfWork != null)
                {
                    _unitOfWork.Dispose();
                    _unitOfWork = null;
                }
            }
        }


        public IQueryable<SchoolPicture> GetByQuery()
        {
            return _unitOfWork.SchoolPictureRepository.GetByQuery();
        }
    }
}
