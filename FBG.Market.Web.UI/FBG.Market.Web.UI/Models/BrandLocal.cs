using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FBG.Market.Web.Identity.Models
{
    public class BrandLocal
        {
            public int BrandID { get; set; }
            public string BrandName { get; set; }
            public string Description { get; set; }
            public string Picture { get; set; }
    }

    public class ProductFamilyLocal
    {
        public int FamilyId { get; set; }
        public string FamilyName { get; set; }
    }
}