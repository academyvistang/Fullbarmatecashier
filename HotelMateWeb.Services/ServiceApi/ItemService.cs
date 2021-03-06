﻿using HotelMateWeb.Dal;
using HotelMateWeb.Dal.DataCore;
using HotelMateWeb.Services.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelMateWeb.Services.ServiceApi
{

    public class ItemService : IItemService
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();

        public IList<Item> GetAll()
        {
            return _unitOfWork.ItemRepository.Get().ToList();
        }

        public Item GetById(int? id)
        {
            return _unitOfWork.ItemRepository.GetByID(id.Value);
        }

        public Item Update(Item item)
        {
            _unitOfWork.ItemRepository.Update(item);
            _unitOfWork.Save();
            return item;
        }

        public void Delete(Item item)
        {
            _unitOfWork.ItemRepository.Delete(item);
            _unitOfWork.Save();
        }

        public Item Create(Item item)
        {
            _unitOfWork.ItemRepository.Insert(item);
            _unitOfWork.Save();
            return item;
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


        public IQueryable<Item> GetByQuery()
        {
            return _unitOfWork.ItemRepository.GetByQuery();
        }
    }
}
