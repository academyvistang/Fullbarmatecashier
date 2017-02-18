using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BarAndRestaurantMate.Models
{
    
    public class ExpenseTypeModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter a description")]
        public string Description { get; set; }


        [Required(ErrorMessage = "Please enter a name")]
        public string Name { get; set; }


        public bool? Saved { get; set; }
        public bool IsActive { get; internal set; }
    }
        public class ExpenseModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter a description")]
        public string Description { get; set; }

         [Required(ErrorMessage = "Please enter a date")]
        public DateTime ExpenseDate { get; set; }

        public bool IsActive { get; set; }

        [DisplayName("Expense Type")]
        [Required(ErrorMessage = "Please enter an Expense Type")]
        [Range(1, 99999999.99, ErrorMessage = "Value must be between 0 - 9")]
        public int ExpenseTypeId { get; set; }

        //public IEnumerable<SelectListExpense> selectList { get; set; }

        public bool? Saved { get; set; }

        public decimal Amount { get; set; }

        public IEnumerable<SelectListItem> selectList { get; set; }

        public int StaffId { get; set; }
        public string ExpenseTypeName { get; internal set; }
        public string DisplayName { get; internal set; }
    }

    public class ExpenseIndexModel
    {
        public IEnumerable<ExpenseModel> ExpenseList { get; set; }
        public IEnumerable<CategoryModel> ExpenseTypeList { get; internal set; }
        public string FileToDownloadPath { get; internal set; }
        public string ReportName { get; internal set; }
    }
}