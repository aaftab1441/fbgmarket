using System;
using System.ComponentModel.DataAnnotations;

namespace FBG.Market.Web.Identity.ViewModel
{
    public class ImportViewModel
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Brand is required")]
        [Range(1, int.MaxValue)]
        public Nullable<int> BID { get; set; }
        public string PID { get; set; }
        public string SKUCode { get; set; }
        public string UPCCode { get; set; }
        [Required(ErrorMessage = "Style is required")]
        public string PName { get; set; }
        public string VendorName { get; set; }
        [Required(ErrorMessage = "Color is required")]
        public string PColor { get; set; }
        public string PSize { get; set; }
        [Required(ErrorMessage = "Category is required")]
        [Range(1, int.MaxValue)]
        public Nullable<short> PCategory { get; set; }
        public string PSubCategory { get; set; }
        public Nullable<int> NRFColorCodeID { get; set; }
        public Nullable<int> SID { get; set; }
        public string PDescription { get; set; }
        public string PSpecs { get; set; }
        public string PCoutryofOrigin { get; set; }
        public bool PDiscontinued { get; set; }
        public string PFOBCost { get; set; }
        public string PLandedCost { get; set; }
        public string PWholesalePrice { get; set; }
        public string PMSRPPrice { get; set; }
        public string PPicture { get; set; }
        public Nullable<int> ProductStatusId { get; set; }
        [Range(1, int.MaxValue)]
        public Nullable<int> VID { get; set; }
        public Nullable<int> ColorCategoryId { get; set; }
        public string ShopifyPicUrl { get; set; }
        public string Brand_Category { get; set; }
        public Nullable<long> GroupId { get; set; }
    }
}
