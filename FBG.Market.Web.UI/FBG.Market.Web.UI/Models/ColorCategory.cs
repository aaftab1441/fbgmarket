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
    
    public partial class ColorCategory
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ColorCategory()
        {
            this.RefNRFColorCodes = new HashSet<RefNRFColorCode>();
        }
    
        public int ColorCategoryID { get; set; }
        public string ColorCategoryName { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RefNRFColorCode> RefNRFColorCodes { get; set; }
    }
}
