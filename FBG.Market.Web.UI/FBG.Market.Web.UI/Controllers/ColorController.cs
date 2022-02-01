using System;
using System.Linq;
using System.Web.Mvc;

namespace FBG.Market.Web.Identity.Controllers
{
    [Authorize]
    public class ColorController : Controller
    {
        public const string EditResultKey = "EditResult";
        public const string EditErrorKey = "EditError";
        // GET: Color
        public ActionResult Index()
        {
            return View();
        }

        Models.FBGMarketEntities db = new Models.FBGMarketEntities();

        public ActionResult ColorPartialView()
        {
            var model = db.RefNRFColorCodes;
            return PartialView("_Colors", model.ToList());
        }

        [HttpPost]
        public ActionResult Add(Models.RefNRFColorCode item)
        {
            var model = db.RefNRFColorCodes;
            if (ModelState.IsValid)
            {
                try
                {
                    model.Add(item);
                    db.SaveChanges();
                    ViewData[EditResultKey] = "Color is added successfully.";
                }
                catch (Exception e)
                {
                    ViewData["EditErrorKey"] = e.Message;
                }
            }
            else
                ViewData["EditErrorKey"] = "Please, correct all errors.";
            return PartialView("_Colors", model.ToList());
        }
        [HttpPost]
        public ActionResult Update(Models.RefNRFColorCode item)
        {
            var model = db.RefNRFColorCodes;
            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = model.FirstOrDefault(it => it.NRFColorCodeID == item.NRFColorCodeID);
                    if (modelItem != null)
                    {
                        this.UpdateModel(modelItem);
                        db.SaveChanges();
                        ViewData[EditResultKey] = "Color is updated successfully.";
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditErrorKey"] = e.Message;
                }
            }
            else
                ViewData["EditErrorKey"] = "Please, correct all errors.";
            return PartialView("_Colors", model.ToList());
        }
        [HttpPost]
        public ActionResult Delete(int NRFColorCodeID)
        {
            var model = db.RefNRFColorCodes;
            if (NRFColorCodeID >= 0)
            {
                try
                {
                    var item = model.FirstOrDefault(it => it.NRFColorCodeID == NRFColorCodeID);
                    if (item != null)
                        model.Remove(item);
                    db.SaveChanges();
                    ViewData[EditResultKey] = "Color is deleted successfully.";
                }
                catch (Exception e)
                {
                    ViewData["EditErrorKey"] = e.Message;
                }
            }
            return PartialView("_Colors", model.ToList());
        }
    }
}