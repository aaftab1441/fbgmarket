using DevExpress.Web.Mvc;
using FBG.Market.Web.Identity.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FBG.Market.Web.Identity.Controllers
{
	[Authorize]
	public class RoleController : Controller
	{
		public const string EditResultKey = "EditResult";
		public const string EditErrorKey = "EditError";
		ApplicationDbContext context;

		public RoleController()
		{
			context = new ApplicationDbContext();
		}

		/// <summary>
		/// Get All Roles
		/// </summary>
		/// <returns></returns>
		[Authorize]
		public ActionResult Index()
		{
			var Roles = context.Roles.ToList();
			return View(Roles);

		}
		public Boolean isAdminUser()
		{
			if (User.Identity.IsAuthenticated)
			{
				var user = User.Identity;
				var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
				var s = UserManager.GetRoles(user.GetUserId());
				if (s[0].ToString() == "Admin")
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			return false;
		}
		/// <summary>
		/// Create  a New role
		/// </summary>
		/// <returns></returns>
		public ActionResult Create()
		{
			if (User.Identity.IsAuthenticated)
			{


				if (!isAdminUser())
				{
					return RedirectToAction("Index", "Home");
				}
			}
			else
			{
				return RedirectToAction("Index", "Home");
			}

			var Role = new IdentityRole();
			return View(Role);
		}

		/// <summary>
		/// Create a New Role
		/// </summary>
		/// <param name="Role"></param>
		/// <returns></returns>
		[HttpPost]
		public ActionResult Create(IdentityRole Role)
		{
			if (User.Identity.IsAuthenticated)
			{
				if (!isAdminUser())
				{
					return RedirectToAction("Index", "Home");
				}
			}
			else
			{
				return RedirectToAction("Index", "Home");
			}

			context.Roles.Add(Role);
			context.SaveChanges();
			ViewData[EditResultKey] = "Role is added successfully.";
			return RedirectToAction("Index");
		}

        FBG.Market.Web.Identity.Models.FBGMarketEntities db = new FBG.Market.Web.Identity.Models.FBGMarketEntities();

        [ValidateInput(false)]
        public ActionResult RoleGridViewPartial()
        {
            var model = db.AspNetRoles;
            return PartialView("_RoleGridViewPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RoleGridViewPartialAddNew(FBG.Market.Web.Identity.Models.AspNetRole item)
        {
            var model = db.AspNetRoles;
			item.Id = Guid.NewGuid().ToString();
            if (ModelState.IsValid)
            {
                try
                {
                    model.Add(item);
                    db.SaveChanges();
					ViewData[EditResultKey] = "Role is added successfully.";
				}
                catch (Exception e)
                {
                    ViewData["EditErrorKey"] = e.Message;
                }
            }
            else
                ViewData["EditErrorKey"] = "Please, correct all errors.";
            return PartialView("_RoleGridViewPartial", model.ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult RoleGridViewPartialUpdate(FBG.Market.Web.Identity.Models.AspNetRole item)
        {
            var model = db.AspNetRoles;
            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = model.FirstOrDefault(it => it.Id == item.Id);
                    if (modelItem != null)
                    {
                        this.UpdateModel(modelItem);
                        db.SaveChanges();
						ViewData[EditResultKey] = "Role is updated successfully.";
					}
                }
                catch (Exception e)
                {
                    ViewData["EditErrorKey"] = e.Message;
                }
            }
            else
                ViewData["EditErrorKey"] = "Please, correct all errors.";
            return PartialView("_RoleGridViewPartial", model.ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult RoleGridViewPartialDelete(System.String Id)
        {
            var model = db.AspNetRoles;
            if (Id != null)
            {
                try
                {
                    var item = model.FirstOrDefault(it => it.Id == Id);
                    if (item != null)
                        model.Remove(item);
                    db.SaveChanges();
					ViewData[EditResultKey] = "Role is deleted successfully.";
				}
                catch (Exception e)
                {
                    ViewData["EditErrorKey"] = e.Message;
                }
            }
            return PartialView("_RoleGridViewPartial", model.ToList());
        }
    }

}