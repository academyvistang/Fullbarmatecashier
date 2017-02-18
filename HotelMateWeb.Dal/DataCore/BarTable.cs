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
    
    public partial class BarTable
    {
        public BarTable()
        {
            this.GuestOrders = new HashSet<GuestOrder>();
            this.PrinterTables = new HashSet<PrinterTable>();
            this.TableItems = new HashSet<TableItem>();
        }
    
        public int Id { get; set; }
        public string TableName { get; set; }
        public string TableAlias { get; set; }
        public bool IsActive { get; set; }
        public int GuestId { get; set; }
        public int TableId { get; set; }
        public int StaffId { get; set; }
        public System.DateTime CreatedDate { get; set; }
    
        public virtual Guest Guest { get; set; }
        public virtual ICollection<GuestOrder> GuestOrders { get; set; }
        public virtual ICollection<PrinterTable> PrinterTables { get; set; }
        public virtual ICollection<TableItem> TableItems { get; set; }
        public virtual Person Person { get; set; }
    }
}