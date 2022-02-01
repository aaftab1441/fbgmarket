using AutoMapper;
using DevExpress.Web.Mvc;
using FBG.Market.Web.Identity.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FBG.Market.Web.Identity.Controllers
{
    [Authorize]
    public class VendorController : Controller
    {
        public const string EditResultKey = "EditResult";
        public const string EditErrorKey = "EditError";

        FBGMarketEntities marketEntities = new FBGMarketEntities();
        
        IMapper mapper;

        // GET: Vendor
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Vendors()
        {
            return PartialView("_Vendors", GetVendors());
        }

        public ActionResult BinaryImageColumnPhotoUpdate()
        {
            return BinaryImageEditExtension.GetCallbackResult();
        }

        [HttpPost]
        public ActionResult EditVendor(VendorViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var vendor = GetVendor(model.VID);

                    if(vendor is null)
                    {
                        vendor = new Vendor();
                    }

                    vendor.VendorCountry = model.VendorCountry;
                    vendor.VendorName = model.VendorName;
                    vendor.VendorAcronym = model.VendorAcronym;
                    vendor.VendorEmail = model.VendorEmail;
                    vendor.VendorCityState = model.VendorCityState;
                    vendor.VendorContact = model.VendorContact;
                    vendor.VendorWebsite = model.VendorWebsite;
                    vendor.VendorAddress = model.VendorAddress;
                    vendor.VendorCountry = model.VendorCountry;
                    vendor.VendorLogo = model.VendorLogo;
                    vendor.VendorPhone = model.VendorPhone;
                    vendor.UserId = string.IsNullOrEmpty(model.UserId) ? GetUserId() : model.UserId;

                    if (model.VID != 0)
                    {
                        this.TryUpdateModel(vendor);
                        marketEntities.SaveChanges();
                        ViewData[EditResultKey] = "Vendor is updated successfully.";
                    }
                    else
                    {
                        marketEntities.Vendors.Add(vendor);
                        marketEntities.SaveChanges();
                        ViewData[EditResultKey] = "Vendor is added successfully.";
                    }
                }
                catch (Exception e)
                {
                    ViewData[EditErrorKey] = e.Message;
                }
            }
            else
                ViewData[EditErrorKey] = "Please, correct all errors.";

            return PartialView("_Vendors", GetVendors());
        }

        [HttpPost]
        public ActionResult DeleteVendor(int VID)
        {
            if(marketEntities.Brands.Any(b=>b.VID == VID))
            {
                ViewData[EditErrorKey] = "This vendor contains brands, please delete brands before deleting this vendor.";
            }
            else if (VID >= 0)
            {
                try
                {
                    var vendor = GetVendor(VID);
                    
                    if (vendor != null)
                        marketEntities.Vendors.Remove(vendor);
                    marketEntities.SaveChanges();

                    ViewData[EditResultKey] = "Vendor was deleted successfully";
                }
                catch (Exception e)
                {
                    ViewData[EditErrorKey] = e.Message;
                }
            }

            return PartialView("_Vendors", GetVendors());
        }

        #region Utility Methods
        private List<VendorViewModel> GetVendors()
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Vendor, VendorViewModel>());
            mapper = config.CreateMapper();

            var vendors = marketEntities.Vendors.ToList().Select(vendor => mapper.Map<VendorViewModel>(vendor));

            return vendors.ToList();
        }

        private Vendor GetVendor(int vendorId)
        {
            return marketEntities.Vendors.FirstOrDefault(it => it.VID == vendorId);
        }

        private string GetUserId()
        {
            return User.Identity.GetUserId();
;        }
        #endregion
    }
}