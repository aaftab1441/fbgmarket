using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FBG.Market.Web.Identity.Controllers
{
    public class SeasonController : Controller
    { 
        public const string EditResultKey = "EditResult";
        public const string EditErrorKey = "EditError";
        // GET: Season
        public ActionResult Index()
        {
            return View();
        }

        FBG.Market.Web.Identity.Models.FBGMarketEntities db = new FBG.Market.Web.Identity.Models.FBGMarketEntities();

        [ValidateInput(false)]
        public ActionResult SeasonGridViewPartial()
        {
            var model = db.RefSeasons;
            return PartialView("_SeasonGridViewPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SeasonGridViewPartialAddNew(FBG.Market.Web.Identity.Models.RefSeason item)
        {
            var model = db.RefSeasons;
            if (ModelState.IsValid)
            {
                try
                {
                    model.Add(item);
                    db.SaveChanges();
                    ViewData[EditResultKey] = "Season is added successfully.";
                }
                catch (Exception e)
                {
                    ViewData["EditErrorKey"] = e.Message;
                }
            }
            else
                ViewData["EditErrorKey"] = "Please, correct all errors.";
            return PartialView("_SeasonGridViewPartial", model.ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult SeasonGridViewPartialUpdate(FBG.Market.Web.Identity.Models.RefSeason item)
        {
            var model = db.RefSeasons;
            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = model.FirstOrDefault(it => it.SID == item.SID);
                    if (modelItem != null)
                    {
                        this.UpdateModel(modelItem);
                        db.SaveChanges();
                        ViewData[EditResultKey] = "Season is updated successfully.";
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditErrorKey"] = e.Message;
                }
            }
            else
                ViewData["EditErrorKey"] = "Please, correct all errors.";
            return PartialView("_SeasonGridViewPartial", model.ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult SeasonGridViewPartialDelete(System.Int32 SID)
        {
            var model = db.RefSeasons;
            if (SID >= 0)
            {
                try
                {
                    var item = model.FirstOrDefault(it => it.SID == SID);
                    if (item != null)
                        model.Remove(item);
                    db.SaveChanges();
                    ViewData[EditResultKey] = "Season is deleted successfully.";
                }
                catch (Exception e)
                {
                    ViewData["EditErrorKey"] = e.Message;
                }
            }
            return PartialView("_SeasonGridViewPartial", model.ToList());
        }
    }
}