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
    
    public partial class DamagedBatchItem
    {
        public int Id { get; set; }
        public int NumberDamaged { get; set; }
        public int ItemId { get; set; }
        public Nullable<int> StorePointId { get; set; }
        public Nullable<int> DistributionPointId { get; set; }
        public System.DateTime DateReported { get; set; }
        public string Description { get; set; }
    
        public virtual DistributionPoint DistributionPoint { get; set; }
        public virtual StockItem StockItem { get; set; }
        public virtual StorePoint StorePoint { get; set; }
    }
}
