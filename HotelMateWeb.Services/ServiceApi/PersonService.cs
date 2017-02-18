using HotelMateWeb.Dal;
using HotelMateWeb.Dal.DataCore;
using HotelMateWeb.Services.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelMateWeb.Services.ServiceApi
{
    public class PersonService : IPersonService
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();

        public IList<Person> GetAllInclude(string includeProperties)
        {
            return _unitOfWork.PersonRepository.GetByQuery(null,null,includeProperties).ToList();
        }

        public Person GetUserByPasswordOnly(string password)
        {
            return _unitOfWork.PersonRepository.GetByQuery(null, null, "Guests").FirstOrDefault(x => x.Password == password);
        }



        public IList<Person> GetAllForLogin()
        {
            return _unitOfWork.PersonRepository.GetByQuery(null, null, "Guests").ToList();
        }
          
        public IList<Person> GetAll(int hotelId)
        {
            return _unitOfWork.PersonRepository.Get(x => x.HotelId == hotelId).ToList();
        }

        public Person GetPersonByUserNameAndPassword(string domainUsername, string password)
        {
            domainUsername = domainUsername.Trim();
            password = password.Trim();
            var allPersons = _unitOfWork.PersonRepository.Get().ToList();
            //var bar = allPersons.FirstOrDefault(x => x.PersonID == 2);
            //var username = bar.Username;
            //var passwordNew = bar.Password;
            var thisUser = allPersons.FirstOrDefault(x => x.Username.Trim().Equals(domainUsername) && x.Password.Trim().Equals(password));
            return thisUser;
            //return allPersons.FirstOrDefault(x => x.Username.Trim().Equals(domainUsername) && x.Password.Trim().Equals(password));
        }


        public Person GetById(int? id)
        {
            return _unitOfWork.PersonRepository.GetByID(id.Value);
        }

        public Person Update(Person person)
        {
            _unitOfWork.PersonRepository.Update(person);
            _unitOfWork.Save();
            return person;
        }

        public void Delete(Person person)
        {
            _unitOfWork.PersonRepository.Delete(person);
            _unitOfWork.Save();
        }

        public Person Create(Person person)
        {
            _unitOfWork.PersonRepository.Insert(person);
            _unitOfWork.Save();
            return person;
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


        public IQueryable<Person> GetByQuery(int hotelId)
        {
            return _unitOfWork.PersonRepository.GetByQuery(x => x.HotelId == hotelId);
        }
    }
}
