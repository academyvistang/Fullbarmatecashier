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
    
    public partial class RoomPaymentTypeStatu
    {
        public RoomPaymentTypeStatu()
        {
            this.RoomPaymentTypes = new HashSet<RoomPaymentType>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    
        public virtual ICollection<RoomPaymentType> RoomPaymentTypes { get; set; }
    }
}
