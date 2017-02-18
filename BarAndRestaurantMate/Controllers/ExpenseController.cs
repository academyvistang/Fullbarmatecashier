

using HotelMateWeb.Services.Core;
using HotelMateWeb.Services.ServiceApi;
using BarAndRestaurantMate.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ClosedXML.Excel;

namespace BarAndRestaurantMate.Controllers
{
    [HandleError(View = "CustomErrorView")]
    public class ExpenseController : Controller
    {

        private readonly IPersonService _personService = null;
        private readonly IExpenseService _expenseService = null;


        public ExpenseController()
        {
            _personService = new PersonService();
            _expenseService = new ExpenseService();
        }
        public IEnumerable<ExpenseModel> GetAllItems()
        {

            return _expenseService.GetAll().Select(x => new ExpenseModel { Amount = x.Amount, Description = x.Description, ExpenseDate = x.ExpenseDate,
              ExpenseTypeId =x.ExpenseTypeId, Id = x.Id, IsActive = x.IsActive, StaffId = x.StaffId, DisplayName = x.Person.DisplayName, ExpenseTypeName = x.ExpensesType.Name} ).ToList();

            

            //using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            //{
            //    using (SqlCommand cmd = new SqlCommand("SELECT * FROM EXPENSE", myConnection))
            //    {
            //        myConnection.Open();

            //        using (SqlDataReader dr = cmd.ExecuteReader())
            //        {
            //            while (dr.Read())
            //            {
            //                int id = dr.GetInt32(0);    // Weight int
            //                DateTime expenseDate = dr.GetDateTime(1);    // Weight int
            //                bool isActive = dr.GetBoolean(2); // Breed string 
            //                decimal amount = dr.GetDecimal(3);
            //                int staffId = dr.GetInt32(4);
            //                int expenseTypeId = dr.GetInt32(7);
            //                string description = dr.GetString(8);


            //                yield return new ExpenseModel
            //                {
            //                    Id = id,
            //                    ExpenseDate = expenseDate,
            //                    IsActive = isActive,
            //                    Amount = amount,
            //                    Description = description,
            //                    ExpenseTypeId = expenseTypeId
            //                };

            //            }
            //        }
            //    }
            //}
        }

        private static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["Core"].ConnectionString;
        }

        //[OutputCache(Duration = 3600, VaryByParam = "id")]
        public ActionResult Delete(int? id)
        {
            var cats = GetAllItems();
            ExpenseModel cm = cats.FirstOrDefault(x => x.Id == id.Value);
            return View(cm);
        }

        [HttpPost]
        public ActionResult Delete(ExpenseModel cm)
        {
            int id = cm.Id;
            var cats = GetAllItems();
            var cm1 = cats.FirstOrDefault(x => x.Id == id);
            cm1.IsActive = false;
           

           

            id = UpdateItem(cm1);

            return RedirectToAction("Index");
        }

        //[OutputCache(Duration = 3600, VaryByParam = "searchText")]

        

        public void GetAllImages(string searchText)
        {

        }
    
        public ActionResult Edit(int? id, bool? saved)
        {
            var items = GetAllItems();

            var item = items.FirstOrDefault(x => x.Id == id.Value);

            int catId = item.ExpenseTypeId;

            var cats = GetAllCategories();

            var categories = cats.ToList();

            categories.Insert(0, new CategoryModel { Name = "-- Please Select--", Id = 0 });

            IEnumerable<SelectListItem> selectList =
                from c in categories
                select new SelectListItem
                {
                    Selected = (c.Id == catId),
                    Text = c.Name,
                    Value = c.Id.ToString()
                };


            ExpenseModel cm = item;

            cm.selectList = selectList;
            cm.Saved = saved;

            return View("Create", cm);
        }

        [HttpPost]
        public ActionResult Edit(ExpenseModel cm, string[] orderNumbers)
        {
            int id = 0;

            var url = string.Empty;

            var extension = string.Empty;

            var imageName = string.Empty; 


            if (ModelState.IsValid)
            {
                var username = User.Identity.Name.ToUpper();
                
                var thisUser = _personService.GetAllForLogin().Where(x => x.Username.ToUpper().Equals(username)).FirstOrDefault();
                cm.StaffId = thisUser.PersonID;


                if (cm.Id > 0)
                {
                    cm.IsActive = true;  
                    id = UpdateItem(cm);                   
                }
                else
                {
                    cm.IsActive = true;
                    cm.ExpenseDate = DateTime.Now;
                    id = InsertItem(cm);
                }

                bool saved = true;

                return RedirectToAction("Edit", new { id, saved });
            }

            int catId = cm.ExpenseTypeId;
            var cats = GetAllCategories();
            var categories = cats.ToList();

            categories.Insert(0, new CategoryModel { Name = "-- Please Select--", Id = 0 });

            IEnumerable<SelectListItem> selectList =
                from c in categories
                select new SelectListItem
                {
                    Selected = (c.Id == catId),
                    Text = c.Name,
                    Value = c.Id.ToString()
                };


            cm.selectList = selectList;

            return View(cm);
        }

        public IEnumerable<CategoryModel> GetAllCategories()
        {
            List<CategoryModel> lst = new List<CategoryModel>();

            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM EXPENSESTYPE", myConnection))
                {
                    //cmd.CommandType = CommandType.StoredProcedure;
                    myConnection.Open();

                    //SqlParameter custId = cmd.Parameters.AddWithValue("@CustomerId", 10);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            int id = dr.GetInt32(0);    // Weight int
                            string name = dr.GetString(1); // Breed string 
                            string description = dr.GetString(3);  // Name string                           
                            bool isActive = dr.GetBoolean(2); // Breed string 
                            yield return new CategoryModel { Id = id, Description = description, IsActive = isActive, Name = name };

                        }
                    }
                }
            }
        }

        
        public ActionResult IndexType()
        {
            var expensesTypeList = GetAllCategories();
            ExpenseIndexModel cim1 = new ExpenseIndexModel { ExpenseTypeList = expensesTypeList };
            return View(cim1);
        }

        //[OutputCache(Duration = 3600, VaryByParam = "id")]
        public ActionResult Index(int? id, DateTime? startDate, DateTime? endDate)
        {
            if (startDate.HasValue && endDate.HasValue)
            {
                if (startDate.Value == endDate.Value)
                {
                    var newDate = startDate.Value.AddDays(1);
                    endDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 0, 0, 1);
                }
            }

            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Now.AddMonths(1);

            ExpenseIndexModel cim = new ExpenseIndexModel();

            cim.ReportName = "Discounts";

            if (id.HasValue)
            {
                var items = GetAllItems().Where(x => x.ExpenseDate >= startDate && x.ExpenseDate <= endDate);
                items = items.ToList();
                cim.ExpenseList = items;
                cim.FileToDownloadPath = GenerateExcelSheetExpenses(cim, cim.ReportName);
                return View("ProductAlerts", cim);
            }

            var items1 = GetAllItems().Where(x => x.ExpenseDate >= startDate && x.ExpenseDate <= endDate);
            cim.ExpenseList = items1;
            cim.FileToDownloadPath = GenerateExcelSheetExpenses(cim, cim.ReportName);
            return View(cim);
        }

       

        private string GenerateExcelSheetExpenses(ExpenseIndexModel model, string reportName)
        {
          
            DataTable dt = new DataTable();

            dt.Columns.AddRange(new DataColumn[5] {
                                new DataColumn("Date", typeof(string)),
                                new DataColumn("Description", typeof(string)),
                                new DataColumn("Staff", typeof(string)),
                                new DataColumn("Status",typeof(string)),
                                new DataColumn("Amount (NGN)",typeof(string))
            });

            int p = 1;

            foreach (var ru in model.ExpenseList.OrderByDescending(x => x.ExpenseDate))
            {
                dt.Rows.Add(ru.ExpenseDate, ru.Description, ru.DisplayName, ru.ExpenseTypeName, ru.Amount);
                p++;
            }

            dt.Rows.Add("Total", "", "", "", model.ExpenseList.Sum(x => x.Amount));

            var fileName = "_" + reportName;

            var fileNameToUse = fileName + ".xlsx";

            var path = Path.Combine(Server.MapPath("~/Products"), fileNameToUse);

            //Codes for the Closed XML
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt, reportName);
                wb.SaveAs(path);
            }

            return fileName;
        }

        //NewExpenseType

        [HttpGet]
        public ActionResult NewExpenseType(int? id, bool? saved)
        {
            ExpenseTypeModel cm = new ExpenseTypeModel { Id = 0, Name = "", Description = "" };

            if(id.HasValue && id.Value > 0)
            {
                var existingExpenseType = GetAllCategories().FirstOrDefault(x => x.Id == id.Value);

                if(existingExpenseType != null)
                {
                    cm = new ExpenseTypeModel { Id = existingExpenseType.Id, Name = existingExpenseType.Name, Description = existingExpenseType.Description };
                }

            }

            cm.Saved = saved;

            return View(cm);
        }

        

        [HttpGet]
        //[OutputCache(Duration = 3600, VaryByParam = "none")]

        public ActionResult Create()
        {
            int catId = 0;
            var cats = GetAllCategories();
            var categories = cats.ToList();

            categories.Insert(0, new CategoryModel { Name = "-- Please Select--", Id = 0 });

            IEnumerable<SelectListItem> selectList =
                from c in categories
                select new SelectListItem
                {
                    Selected = (c.Id == catId),
                    Text = c.Name,
                    Value = c.Id.ToString()
                };

            ExpenseModel cm = new ExpenseModel { Id = 0, selectList = selectList};

            return View(cm);
        }
        

        [HttpPost]
        public ActionResult NewExpenseType(ExpenseTypeModel model)
        {
            var allExpensesType = GetAllCategories().ToList();

             var expenseType = allExpensesType.FirstOrDefault(x => x.Name.ToUpper().Equals(model.Name.ToUpper()));

            if(expenseType != null && model.Id == 0)
            {
                ModelState.AddModelError("_Form", "This Expense Type Already Exist");
            }

            if (ModelState.IsValid)
            {
                int existingId = 0;

                if(model.Id > 0)
                {
                    expenseType = allExpensesType.FirstOrDefault(x => x.Id == model.Id);
                }

                if (expenseType != null )
                {
                    expenseType.Name = model.Name;
                    expenseType.Description = model.Description;
                    existingId = UpdateItemType(expenseType);
                }
                else
                {
                    expenseType = new CategoryModel();
                    expenseType.IsActive = true;
                    expenseType.Name = model.Name;
                    expenseType.Description = model.Description;
                    expenseType.Id = 0;
                    existingId = UpdateItemType(expenseType);
                }

                bool saved = true;
                return RedirectToAction("NewExpenseType", new { id = existingId, saved });
            }

           

            return View(model);
        }

       

        [HttpPost]
        public ActionResult Create(ExpenseModel cm, string[] orderNumbers)
        {
            int? id = null;

            if (cm.ExpenseTypeId == 0)
            {
                ModelState.AddModelError("CategoryId", "Please select a category");
            }

            var username = User.Identity.Name.ToUpper();

            var url = string.Empty;

            var extension = string.Empty;

            var imageName = string.Empty;

            if (ModelState.IsValid)
            {
                int? existingId = null;

                var thisUser = _personService.GetAllForLogin().Where(x => x.Username.ToUpper().Equals(username)).FirstOrDefault();
                cm.StaffId = thisUser.PersonID;

                if (existingId.HasValue && cm.Id == 0)
                {
                    cm.IsActive = true;                   
                    cm.Id = existingId.Value;

                    id = UpdateItem(cm);                    

                }
                else if (cm.Id > 0)
                {
                    cm.IsActive = true;
                    id = UpdateItem(cm);
                    
                }
                else
                {
                    cm.IsActive = true;
                    cm.ExpenseDate = DateTime.Now;
                    id = InsertItem(cm);                   
                }

                bool saved = true;

                return RedirectToAction("Edit", new { id, saved });
            }

            int catId = cm.ExpenseTypeId;
            var cats = GetAllCategories();
            var categories = cats.ToList();

            categories.Insert(0, new CategoryModel { Name = "-- Please Select--", Id = 0 });

            IEnumerable<SelectListItem> selectList =
                from c in categories
                select new SelectListItem
                {
                    Selected = (c.Id == catId),
                    Text = c.Name,
                    Value = c.Id.ToString()
                };


            cm.selectList = selectList;

            return View(cm);
        }

        public static string RemoveWhitespace(string input)
        {
            StringBuilder output = new StringBuilder(input.Length);

            for (int index = 0; index < input.Length; index++)
            {
                if (!Char.IsWhiteSpace(input, index))
                {
                    output.Append(input[index]);
                }
            }

            return output.ToString();
        }



        public int? GetExistingItem(string name)
        {
            int itemId = 0;

            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("Select Id FROM STOCKITEM WHERE StockItemName = '" + name + "'", myConnection))
                {
                    //cmd.CommandType = CommandType.StoredProcedure;
                    myConnection.Open();
                    try
                    {
                        int.TryParse(cmd.ExecuteScalar().ToString(), out itemId);
                    }
                    catch
                    {

                    }

                }
            }

            if (itemId > 0)
                return itemId;
            else
                return null;
        }

        private int UpdateItemType(CategoryModel cm)
        {
            int id = 0;

            if(cm.Id == 0)
            {
                using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("ExpenseTypeInsert", myConnection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        myConnection.Open();
                        
                        cmd.Parameters.AddWithValue("@IsActive", true);
                        cmd.Parameters.AddWithValue("@Name", cm.Name);
                        cmd.Parameters.AddWithValue("@Description", cm.Description);

                        try
                        {
                            int.TryParse(cmd.ExecuteScalar().ToString(), out id);
                        }
                        catch
                        {

                        }
                    }
                }

            }
            else
            {
                using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("ExpenseTypeUpdate", myConnection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        myConnection.Open();

                        cmd.Parameters.AddWithValue("@Id", cm.Id);
                        cmd.Parameters.AddWithValue("@Name", cm.Name);
                        cmd.Parameters.AddWithValue("@Description", cm.Description);
                        cmd.Parameters.AddWithValue("@IsActive", true);


                        try
                        {
                            int.TryParse(cmd.ExecuteScalar().ToString(), out id);
                        }
                        catch(Exception ex)
                        {
                         
                        }
                    }
                }

            }



            

            return id;
        }

        private int InsertItem(ExpenseModel cm)
        {
            int id = 0;

            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("ExpenseInsert", myConnection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    myConnection.Open();

                    cmd.Parameters.AddWithValue("@ExpenseDate", cm.ExpenseDate);
                    cmd.Parameters.AddWithValue("@IsActive", cm.IsActive);
                    cmd.Parameters.AddWithValue("@Amount", cm.Amount);
                    cmd.Parameters.AddWithValue("@Description", cm.Description);
                    cmd.Parameters.AddWithValue("@StaffId", cm.StaffId);
                    cmd.Parameters.AddWithValue("@ExpenseTypeId", cm.ExpenseTypeId);

                    try
                    {
                        int.TryParse(cmd.ExecuteScalar().ToString(), out id);
                    }
                    catch
                    {

                    }
                }
            }

            return id;
        }

        private int UpdateItem(ExpenseModel cm)
        {
            int id = 0;

            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("ExpenseUpdate", myConnection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    myConnection.Open();

                    //SqlParameter custId = cmd.Parameters.AddWithValue("@CustomerId", 10);
                    cmd.Parameters.AddWithValue("@Id", cm.Id);
                    cmd.Parameters.AddWithValue("@IsActive", cm.IsActive);
                    cmd.Parameters.AddWithValue("@Amount", cm.Amount);
                    cmd.Parameters.AddWithValue("@Description", cm.Description);
                    cmd.Parameters.AddWithValue("@StaffId", cm.StaffId);
                    cmd.Parameters.AddWithValue("@ExpenseTypeId", cm.ExpenseTypeId);

                    try
                    {
                        int.TryParse(cmd.ExecuteScalar().ToString(), out id);
                    }
                    catch
                    {

                    }
                }
            }

            return id;
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}