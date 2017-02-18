using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HotelMateWeb.Dal.DataCore;

namespace HotelMateWeb.Services.Core
{
    public interface IPaymentOrderService
    {
        IList<PaymentOrder> GetAll();
        PaymentOrder GetById(int? id);
        PaymentOrder Update(PaymentOrder item);
        void Delete(PaymentOrder item);
        PaymentOrder Create(PaymentOrder item);
        IQueryable<PaymentOrder> GetByQuery();
        void Dispose();
    }
}



