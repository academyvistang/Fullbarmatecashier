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
    
    public partial class StorePointItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public int Remaining { get; set; }
        public int StorePointId { get; set; }
        public bool Invinsible { get; set; }
        public Nullable<int> PreviousQty { get; set; }
        public Nullable<int> PreviousRemaining { get; set; }
        public bool IsActive { get; set; }
        public int Damaged { get; set; }
        public int PurchaseOrderItemId { get; set; }
    
        public virtual PurchaseOrderItem PurchaseOrderItem { get; set; }
        public virtual StorePoint StorePoint { get; set; }
    }
}
