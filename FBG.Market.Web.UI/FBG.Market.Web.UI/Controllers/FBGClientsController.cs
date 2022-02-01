using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FBG.Market.Web.Identity.Controllers
{
    [Authorize]
    public class FBGClientsController : Controller
    {
        public const string EditResultKey = "EditResult";
        public const string EditErrorKey = "EditError";
        // GET: Clients
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        FBG.Market.Web.Identity.Models.FBGMarketEntities db = new FBG.Market.Web.Identity.Models.FBGMarketEntities();

        [Authorize]
        [ValidateInput(false)]
        public ActionResult ClientsGridViewPartial()
        {
            var model = db.Clients;
            return PartialView("_ClientsGridViewPartial", model.ToList());
        }

        [Authorize]
        [HttpPost, ValidateInput(false)]
        public ActionResult ClientsGridViewPartialAddNew(FBG.Market.Web.Identity.Models.Client item)
        {
            var model = db.Clients;
            if (ModelState.IsValid)
            {
                try
                {
                    model.Add(item);
                    db.SaveChanges();
                    ViewData[EditResultKey] = "Client is added successfully.";
                }
                catch (Exception e)
                {
                    ViewData["EditErrorKey"] = e.Message;
                }
            }
            else
                ViewData["EditErrorKey"] = "Please, correct all errors.";
            return PartialView("_ClientsGridViewPartial", model.ToList());
        }
        [Authorize]
        [HttpPost, ValidateInput(false)]
        public ActionResult ClientsGridViewPartialUpdate(FBG.Market.Web.Identity.Models.Client item)
        {
            var model = db.Clients;
            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = model.FirstOrDefault(it => it.CID == item.CID);
                    if (modelItem != null)
                    {
                        this.UpdateModel(modelItem);
                        db.SaveChanges();
                        ViewData[EditResultKey] = "Client is updated successfully.";
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditErrorKey"] = e.Message;
                }
            }
            else
                ViewData["EditErrorKey"] = "Please, correct all errors.";
            return PartialView("_ClientsGridViewPartial", model.ToList());
        }
        [Authorize]
        [HttpPost, ValidateInput(false)]
        public ActionResult ClientsGridViewPartialDelete(System.Int32 CID)
        {
            var model = db.Clients;
            if (CID >= 0)
            {
                try
                {
                    var item = model.FirstOrDefault(it => it.CID == CID);
                    if (item != null)
                        model.Remove(item);
                    db.SaveChanges();
                    ViewData[EditResultKey] = "Client is deleted successfully.";
                }
                catch (Exception e)
                {
                    ViewData["EditErrorKey"] = e.Message;
                }
            }
            return PartialView("_ClientsGridViewPartial", model.ToList());
        }
    }
}