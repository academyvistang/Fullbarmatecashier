
using Excel;
using BarAndRestaurantMate.Importer.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace BarAndRestaurantMate.Importer
{

    public class ExcelImporter : IExcelImporter
    {

        IExcelDataReader excelReader = null;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (excelReader != null)
                {
                    excelReader.Dispose();
                    excelReader = null;
                }
            }
        }

        public DataSet GetDataSet(Stream fileStream, string filename)
        {
            var extension = Path.GetExtension(filename);

            try
            {

                if (extension != ".xls")
                {
                    excelReader = ExcelReaderFactory.CreateOpenXmlReader(fileStream);
                }
                else
                {
                    excelReader = ExcelReaderFactory.CreateBinaryReader(fileStream);
                }

                var dt = excelReader.AsDataSet();

                excelReader.IsFirstRowAsColumnNames = true;

                excelReader.Close();

                return dt;

            }
            catch
            {

            }

            return null;
        }
    }
}
