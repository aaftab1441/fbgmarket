//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FBG.Market.Web.Identity.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class OrderDetail
    {
        public int ODID { get; set; }
        public int ODOID { get; set; }
        public Nullable<int> ODProductID { get; set; }
        public short ODQuantity { get; set; }
        public Nullable<decimal> ODUnitPrice { get; set; }
        public Nullable<double> ODDiscount { get; set; }
        public Nullable<decimal> ODMSRP { get; set; }
        public Nullable<decimal> ODWholesale { get; set; }
        public byte[] SSMA_TimeStamp { get; set; }
    
        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
    }
}
