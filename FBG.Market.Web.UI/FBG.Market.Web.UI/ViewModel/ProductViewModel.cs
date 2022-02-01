using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FBG.Market.Web.Identity.ViewModel
{
    public class ProductViewModel
    {
        public const int MyInt_Default = 1;
        [Required(ErrorMessage = "Brand is required")]
        [DefaultValue(MyInt_Default)]
        [Range(1, int.MaxValue)]
        public Nullable<int> BID { get; set; } = 14;
        public int PID { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9_-]{1,20}$", ErrorMessage = "SKU Code is not valid, should be (1-20) characters")]
        public string SKUCode { get; set; }
        [RegularExpression(@"^[a-zA-Z0-9_-]{1,13}$", ErrorMessage = "UPC Code is not valid,should be (1-13) characters")]
        public string UPCCode { get; set; }
        [Required(ErrorMessage = "Style is required")]
        public string PName { get; set; }
        [Required(ErrorMessage = "Color is required")]
        public string PColor { get; set; }
        public string PSize { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [DefaultValue(MyInt_Default)]
        [Range(1, int.MaxValue)]
        public short PCategory { get; set; } = MyInt_Default;
        public string PSubCategory { get; set; }
        [DefaultValue(MyInt_Default)]
        public Nullable<int> NRFColorCodeID { get; set; } = MyInt_Default;
        [DefaultValue(MyInt_Default)]
        public Nullable<int> SID { get; set; } = MyInt_Default;
        public string PDescription { get; set; }
        public string PSpecs { get; set; }
        public string PCoutryofOrigin { get; set; }
        public bool PDiscontinued { get; set; }
        public byte[] SSMA_TimeStamp { get; set; }
        public Nullable<decimal> PFOBCost { get; set; }
        public Nullable<decimal> PLandedCost { get; set; }
        public decimal PWholesalePrice { get; set; }
        public Nullable<decimal> PMSRPPrice { get; set; }
        public string PPicture { get; set; }
        public string Message { get; set; }
        public Nullable<int> ProductStatusId { get; set; } = MyInt_Default;
        [Required(ErrorMessage = "Vendor is required")]
        [DefaultValue(MyInt_Default)]
        [Range(1, int.MaxValue)]
        public Nullable<int> VID { get; set; } = MyInt_Default;
        [DefaultValue(MyInt_Default)]
        public Nullable<int> ColorCategoryId { get; set; } = MyInt_Default;
        public string ShopifyPicUrl { get; set; }
        public string VendorName { get; set; }
        public Nullable<long> GroupId { get; set; }
        [DefaultValue(MyInt_Default)]
        public Nullable<int> BrandCategoryId { get; set; } = 14;
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}