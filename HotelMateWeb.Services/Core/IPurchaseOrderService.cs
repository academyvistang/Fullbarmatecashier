using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HotelMateWeb.Dal.DataCore;

namespace HotelMateWeb.Services.Core
{

    public interface IPurchaseOrderService
    {
        IList<PurchaseOrder> GetAll();
        PurchaseOrder GetById(int? id);
        PurchaseOrder Update(PurchaseOrder PurchaseOrder);
        void Delete(PurchaseOrder PurchaseOrder);
        PurchaseOrder Create(PurchaseOrder PurchaseOrder);
        IQueryable<PurchaseOrder> GetByQuery();
        IList<PurchaseOrder> GetAll(string includes);
        void Dispose();

        PurchaseOrder GetByIdInclude(int id, string include);
        IList<PurchaseOrder> GetAllByInclude(string includes);
    }

}
