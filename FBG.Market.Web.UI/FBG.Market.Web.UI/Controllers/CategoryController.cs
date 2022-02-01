using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FBG.Market.Web.Identity.Controllers
{
    public class CategoryController : Controller
    {
        public const string EditResultKey = "EditResult";
        public const string EditErrorKey = "EditError";
        // GET: Category
        public ActionResult Index()
        {
            return View();
        }

        FBG.Market.Web.Identity.Models.FBGMarketEntities db = new FBG.Market.Web.Identity.Models.FBGMarketEntities();

        [ValidateInput(false)]
        public ActionResult CategoryGridViewPartial()
        {
            var model = db.RefCategories;
            return PartialView("_CategoryGridViewPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CategoryGridViewPartialAddNew(FBG.Market.Web.Identity.Models.RefCategory item)
        {
            var model = db.RefCategories;
            if (ModelState.IsValid)
            {
                try
                {
                    model.Add(item);
                    db.SaveChanges();
                    ViewData[EditResultKey] = "Category is added successfully.";
                }
                catch (Exception e)
                {
                    ViewData["EditErrorKey"] = e.Message;
                }
            }
            else
                ViewData["EditErrorKey"] = "Please, correct all errors.";
            return PartialView("_CategoryGridViewPartial", model.ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult CategoryGridViewPartialUpdate(FBG.Market.Web.Identity.Models.RefCategory item)
        {
            var model = db.RefCategories;
            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = model.FirstOrDefault(it => it.ID == item.ID);
                    if (modelItem != null)
                    {
                        this.UpdateModel(modelItem);
                        db.SaveChanges();
                        ViewData[EditResultKey] = "Category is updated successfully.";
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditErrorKey"] = e.Message;
                }
            }
            else
                ViewData["EditErrorKey"] = "Please, correct all errors.";
            return PartialView("_CategoryGridViewPartial", model.ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult CategoryGridViewPartialDelete(System.Int32 ID)
        {
            var model = db.RefCategories;
            if (ID >= 0)
            {
                try
                {
                    var item = model.FirstOrDefault(it => it.ID == ID);
                    if (item != null)
                        model.Remove(item);
                    db.SaveChanges();
                    ViewData[EditResultKey] = "Category is deleted successfully.";
                }
                catch (Exception e)
                {
                    ViewData["EditErrorKey"] = e.Message;
                }
            }
            return PartialView("_CategoryGridViewPartial", model.ToList());
        }
    }
}