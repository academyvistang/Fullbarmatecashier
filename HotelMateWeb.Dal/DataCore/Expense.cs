//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HotelMateWeb.Dal.DataCore
{
    using System;
    using System.Collections.Generic;
    
    public partial class Expense
    {
        public int Id { get; set; }
        public System.DateTime ExpenseDate { get; set; }
        public bool IsActive { get; set; }
        public decimal Amount { get; set; }
        public int StaffId { get; set; }
        public Nullable<int> RoomId { get; set; }
        public string GuestSignature { get; set; }
        public int ExpenseTypeId { get; set; }
        public string Description { get; set; }
    
        public virtual Person Person { get; set; }
        public virtual ExpensesType ExpensesType { get; set; }
    }
}