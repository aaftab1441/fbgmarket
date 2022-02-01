using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FBG.Market.Web.Identity.Models
{
    public class CategoryLocal
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Picture { get; set; }
    }

    public class ProductStatusLocal
    {

        public int Id { get; set; }
        public string Status { get; set; }
    }
}