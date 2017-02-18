using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarAndRestaurantMate.Importer.Core
{
    public interface IExcelImporter
    {     
        DataSet GetDataSet(Stream fileStream, string filename);
        void Dispose();
    }
}
