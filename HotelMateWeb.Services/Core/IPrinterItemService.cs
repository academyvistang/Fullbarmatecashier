using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HotelMateWeb.Dal.DataCore;

namespace HotelMateWeb.Services.Core
{

    public interface IPrinterTableService
    {
        IList<PrinterTable> GetAll();
        PrinterTable GetById(int? id);
        PrinterTable Update(PrinterTable PrinterTable);
        void Delete(PrinterTable PrinterTable);
        PrinterTable Create(PrinterTable PrinterTable);
        IQueryable<PrinterTable> GetByQuery();
        void Dispose();
        IList<PrinterTable> GetAllInclude(string includes);
    }

}
