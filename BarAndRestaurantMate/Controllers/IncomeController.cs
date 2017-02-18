﻿

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

namespace BarAndRestaurantMate.Controllers
{
    [HandleError(View = "CustomErrorView")]
    public class IncomeController : Controller
    {

        private readonly IPersonService _personService = null;

        public IncomeController()
        {
            _personService = new PersonService();
        }
        public IEnumerable<IncomeModel> GetAllItems()
        {
            List<IncomeModel> lst = new List<IncomeModel>();

            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Income", myConnection))
                {
                    myConnection.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            int id = dr.GetInt32(0);    // Weight int
                            DateTime IncomeDate = dr.GetDateTime(1);    // Weight int
                            bool isActive = dr.GetBoolean(2); // Breed string 
                            decimal amount = dr.GetDecimal(3);
                            int staffId = dr.GetInt32(4);
                            int IncomeTypeId = dr.GetInt32(7);
                            string description = dr.GetString(8);


                            yield return new IncomeModel
                            {
                                Id = id,
                                IncomeDate = IncomeDate,
                                IsActive = isActive,
                                Amount = amount,
                                Description = description,
                                IncomeTypeId = IncomeTypeId
                            };

                        }
                    }
                }
            }
        }

        private static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["Core"].ConnectionString;
        }

        //[OutputCache(Duration = 3600, VaryByParam = "id")]
        public ActionResult Delete(int? id)
        {
            var cats = GetAllItems();
            IncomeModel cm = cats.FirstOrDefault(x => x.Id == id.Value);
            return View(cm);
        }

        [HttpPost]
        public ActionResult Delete(IncomeModel cm)
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

            int catId = item.IncomeTypeId;

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


            IncomeModel cm = item;

            cm.selectList = selectList;
            cm.Saved = saved;

            return View("Create", cm);
        }

        [HttpPost]
        public ActionResult Edit(IncomeModel cm, string[] orderNumbers)
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
                    cm.IncomeDate = DateTime.Now;
                    id = InsertItem(cm);
                }

                bool saved = true;

                return RedirectToAction("Edit", new { id, saved });
            }

            int catId = cm.IncomeTypeId;
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
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM IncomeSTYPE", myConnection))
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
                            string description = dr.GetString(9);  // Name string                           
                            bool isActive = dr.GetBoolean(3); // Breed string 
                            yield return new CategoryModel { Id = id, Description = description, IsActive = isActive, Name = name };

                        }
                    }
                }
            }

            //return lst;

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

            if (id.HasValue)
            {
                var items = GetAllItems().Where(x => x.IncomeDate >= startDate && x.IncomeDate <= endDate);
                items = items.ToList();
                IncomeIndexModel cim = new IncomeIndexModel { IncomeList = items };
                return View("ProductAlerts", cim);
            }

            var items1 = GetAllItems().Where(x => x.IncomeDate >= startDate && x.IncomeDate <= endDate);

            IncomeIndexModel cim1 = new IncomeIndexModel { IncomeList = items1 };

            return View(cim1);
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

            IncomeModel cm = new IncomeModel { Id = 0, selectList = selectList };

            return View(cm);
        }

        [HttpPost]
        public ActionResult Create(IncomeModel cm, string[] orderNumbers)
        {
            int? id = null;

            if (cm.IncomeTypeId == 0)
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
                    cm.IncomeDate = DateTime.Now;
                    id = InsertItem(cm);
                }

                bool saved = true;

                return RedirectToAction("Edit", new { id, saved });
            }

            int catId = cm.IncomeTypeId;
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

        private int InsertItem(IncomeModel cm)
        {
            int id = 0;

            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("IncomeInsert", myConnection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    myConnection.Open();

                    cmd.Parameters.AddWithValue("@IncomeDate", cm.IncomeDate);
                    cmd.Parameters.AddWithValue("@IsActive", cm.IsActive);
                    cmd.Parameters.AddWithValue("@Amount", cm.Amount);
                    cmd.Parameters.AddWithValue("@Description", cm.Description);
                    cmd.Parameters.AddWithValue("@StaffId", cm.StaffId);
                    cmd.Parameters.AddWithValue("@IncomeTypeId", cm.IncomeTypeId);

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

        private int UpdateItem(IncomeModel cm)
        {
            int id = 0;

            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("IncomeUpdate", myConnection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    myConnection.Open();

                    //SqlParameter custId = cmd.Parameters.AddWithValue("@CustomerId", 10);
                    cmd.Parameters.AddWithValue("@Id", cm.Id);
                    cmd.Parameters.AddWithValue("@IsActive", cm.IsActive);
                    cmd.Parameters.AddWithValue("@Amount", cm.Amount);
                    cmd.Parameters.AddWithValue("@Description", cm.Description);
                    cmd.Parameters.AddWithValue("@StaffId", cm.StaffId);
                    cmd.Parameters.AddWithValue("@IncomeTypeId", cm.IncomeTypeId);

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