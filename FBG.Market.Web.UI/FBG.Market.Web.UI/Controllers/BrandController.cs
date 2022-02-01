using FBG.Market.Web.Identity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;

namespace FBG.Market.Web.Identity.Controllers
{
    [Authorize]
    public class BrandController : Controller
    {
        public const string EditResultKey = "EditResult";
        public const string EditErrorKey = "EditError";


        FBGMarketEntities marketEntities = new FBGMarketEntities();
        IMapper mapper;

        public ActionResult Index(string VID)
        {
            ViewData["VID"] = VID;
            var vendorId = Convert.ToInt32(VID);

            return PartialView("_VendorBrands", GetBrands(vendorId));
        }

        public ActionResult EditBrand(BrandViewModel model, int VID)
        {
            ViewData["VID"] = VID;

            try
            {
                var brand = GetBrand(model.BID);

                if (brand is null)
                {
                    brand = new Brand();
                    brand.VID = VID;
                }

                brand.BrandSubCategory = model.BrandSubCategory;
                brand.BID = model.BID;
                brand.BrandName = model.BrandName;
                brand.BrandCategory = model.BrandCategory;
                brand.BrandLogo = model.BrandLogo;
                brand.BrandNotes = model.BrandNotes;
                brand.BrandSubCategory = model.BrandSubCategory;
                brand.BrandWebSite = model.BrandWebSite;

                if (model.BID != 0)
                {
                    this.TryUpdateModel(brand);
                    marketEntities.SaveChanges();

                    ViewData[EditResultKey] = "Brand is updated successfully.";
                }
                else
                {
                    marketEntities.Brands.Add(brand);
                    marketEntities.SaveChanges();

                    ViewData[EditResultKey] = "Brand is added successfully.";
                }
            }
            catch (Exception e)
            {
                ViewData[EditErrorKey] = e.Message;
            }

            return PartialView("_VendorBrands", GetBrands(VID));
        }

        public ActionResult DeleteBrand(int VID, int BID)
        {
            ViewData["VID"] = VID;

            bool hasProducts = CheckIfBrandContainsProducts(BID);

            if (hasProducts)
            {
                ViewData[EditErrorKey] = "This brand contains products, please remove products before deleting this brand.";
            }
            else if (BID >= 0)
            {
                try
                {
                    var brand = GetBrand(BID);
                    if (brand != null)
                        marketEntities.Brands.Remove(brand);

                    marketEntities.SaveChanges();
                    ViewData[EditResultKey] = "Brand was deleted successfully.";
                }
                catch (Exception e)
                {
                    ViewData[EditErrorKey] = e.Message;
                }
            }

            return PartialView("_VendorBrands", GetBrands(VID));
        }

        private bool CheckIfBrandContainsProducts(int BID)
        {
            return marketEntities.Products.Any(p => p.BID == BID);
        }

        private List<BrandViewModel> GetBrands(int Vid)
        {
            var brands = marketEntities.Brands;

            var config = new MapperConfiguration(cfg => cfg.CreateMap<Brand, BrandViewModel>());
            mapper = config.CreateMapper();

            return brands.Where(it => it.VID == Vid).ToList().Select(item => mapper.Map<BrandViewModel>(item)).ToList();
        }

        private Brand GetBrand(int brandId)
        {
            return marketEntities.Brands.FirstOrDefault(it => it.BID == brandId);
        }
    }
}