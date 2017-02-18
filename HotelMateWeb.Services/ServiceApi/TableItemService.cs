using HotelMateWeb.Dal;
using HotelMateWeb.Dal.DataCore;
using HotelMateWeb.Services.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelMateWeb.Services.ServiceApi
{

    public class TableItemService : ITableItemService
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();

        public IList<TableItem> GetAllEvery(string includes)
        {
            return _unitOfWork.TableItemRepository.Get(null, null, includes).ToList();
        }



        public IList<TableItem> GetAll(string includes)
        {
            return _unitOfWork.TableItemRepository.Get(null, null, includes).Where(x => x.IsActive).ToList();
        }

        public IList<TableItem> GetAll()
        {
            return _unitOfWork.TableItemRepository.Get().Where(x => x.IsActive).ToList();
        }

        public TableItem GetById(int? id)
        {
            return _unitOfWork.TableItemRepository.GetByID(id.Value);
        }

        public TableItem Update(TableItem tableItem)
        {
            _unitOfWork.TableItemRepository.Update(tableItem);
            _unitOfWork.Save();
            return tableItem;
        }

        public void Delete(TableItem tableItem)
        {
            _unitOfWork.TableItemRepository.Delete(tableItem);
            _unitOfWork.Save();
        }

        public TableItem Create(TableItem tableItem)
        {
            _unitOfWork.TableItemRepository.Insert(tableItem);
            _unitOfWork.Save();
            return tableItem;
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


        public IQueryable<TableItem> GetByQuery()
        {
            return _unitOfWork.TableItemRepository.GetByQuery();
        }

       
    }
}
