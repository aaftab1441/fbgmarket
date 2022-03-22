using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FBG.Market.Web.Identity.ViewModel
{

    public class ProductImportViewModel
    {
        public int ID { get; set; }
        public string BID { get; set; }
        public string PID { get; set; }
        public string SKUCode { get; set; }
        public string UPCCode { get; set; }
        public string PName { get; set; }
        public string VendorName { get; set; }
        public string PColor { get; set; }
        public string PSize { get; set; }
        public string PCategory { get; set; }
        public string PSubCategory { get; set; }
        public string NRFColorCodeID { get; set; }
        public string SID { get; set; }
        public string PDescription { get; set; }
        public string PSpecs { get; set; }
        public string PCoutryofOrigin { get; set; }
        public bool PDiscontinued { get; set; }
        public string PFOBCost { get; set; }
        public string PLandedCost { get; set; }
        public string PWholesalePrice { get; set; }
        public string PMSRPPrice { get; set; }
        public string PPicture { get; set; }
        public string ProductStatusId { get; set; }
        public string VID { get; set; }
        public string ColorCategoryId { get; set; }
        public string ShopifyPicUrl { get; set; }
        public string BrandCategoryId { get; set; }
        public string Message { get; set; }
        public string GroupId { get; set; }
    }
}

