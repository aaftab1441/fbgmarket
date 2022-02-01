using FBG.Market.Web.Identity.Filters;
using System.Web;
using System.Web.Mvc;

namespace FBG.Market.Web.Identity {
    public class FilterConfig {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
            filters.Add(new HandleErrorAttribute());
            //filters.Add(new CompressAttribute());
        }
    }
}