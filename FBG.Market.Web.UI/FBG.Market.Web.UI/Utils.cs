using FBG.Market.Web.Identity.Models;
using FBG.Market.Web.Identity.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace FBG.Market.Web.Identity
{
    
    public class Utils
    {
        //public static IEnumerable<TSource> DistinctBy<TSource, TKey>
        //(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        //{
        //    HashSet<TKey> seenKeys = new HashSet<TKey>();
        //    foreach (TSource element in source)
        //    {
        //        if (seenKeys.Add(keySelector(element)))
        //        {
        //            yield return element;
        //        }
        //    }
        //}
        FBGMarketEntities db = new FBGMarketEntities();

        public List<Product> Products
        {
            get
            {
                return db.Products.ToList();
            }
        }

        public List<ProductFamilyLocal> GetProductFamilies()
        {
        var query = from family in db.RefProductFamilies
                        select new ProductFamilyLocal
                        {
                            FamilyId = family.PFID,
                            FamilyName = family.PFName,
                            
                        };
            return query.ToList();
        }
        public List<ColorViewModel> GetColors()
        {
            var query = from colorCode in db.RefNRFColorCodes
                        select new ColorViewModel
                        {
                            Id = colorCode.NRFColorCodeID,
                            Name = db.ColorCategories.Where(s => s.ColorCategoryID == colorCode.ColorCategoryID).FirstOrDefault().ColorCategoryName + " " + colorCode.NRFColorCode + " " + colorCode.NRFColorName,
                        };
            return query.OrderBy(item=>item.Name).ToList();
        }
        public List<ColorCategoryViewModel> GetColorCategories()
        {
            var query = from colorCode in db.ColorCategories
                        select new ColorCategoryViewModel
                        {
                            Id = colorCode.ColorCategoryID,
                            Name = colorCode.ColorCategoryName,
                        };
            return query.OrderBy(item => item.Name).ToList();
        }
        public List<BrandLocal> GetBrandsCategories()
        {
            var query = from brand in db.Brands
                        select new BrandLocal
                        {
                            BrandID = brand.BID,
                            BrandName = brand.BrandName + "_" + db.RefCategories.Where(s => s.ID == brand.BrandCategory).FirstOrDefault().Category,
                            Description = brand.BrandNotes,
                            Picture = brand.BrandLogo
                        };
            return query.ToList();
        }
        public List<BrandLocal> GetBrandsDistinct()
        {
            var collection = db.Brands
    .Select(o =>
    new BrandLocal
    {
        BrandID = o.BID,
        BrandName = o.BrandName
    });

            return collection.ToList();
        }
        public List<BrandLocal> GetBrands()
        {
            var query = from brand in db.Brands
                            select new BrandLocal
                            {
                                BrandID = brand.BID,
                                BrandName = brand.BrandName,
                                Description = brand.BrandNotes,
                                Picture = brand.BrandLogo
                            };
            
            return query.ToList();
        }

        public List<ProductStatusLocal> GetProductStatus()
        {
            var query = from prodStatus in db.ProductStatus
                        select new ProductStatusLocal
                        {
                            Id = prodStatus.Id,
                            Status = prodStatus.Status,

                        };
            return query.ToList();
        }

        public List<SeasonLocal> GetSeasons()
        {
            var query = from seasons in db.RefSeasons
                        select new SeasonLocal
                        {
                            SeasonID = seasons.SID,
                            SeasonName = seasons.Season,
                        };
            return query.ToList();
        }
        public List<CategoryLocal> GetCategories()
        {
            var query = from category in db.RefCategories
                        select new CategoryLocal
                        {
                            Id = category.ID,
                            Name = category.Category,
                        };
            return query.ToList();
        }

        public List<VendorLocal> GetVendors()
        {
            var query = from vendor in db.Vendors
                        select new VendorLocal
                        {
                            VendorId = vendor.VID,
                            VendorName = vendor.VendorName,

                        };
            return query.ToList();
        }

        public static byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, imageIn.RawFormat);
                return ms.ToArray();
            }
        }
        public List<AspNetUsersLocal> GetRoles(int vendorId)
        {
            var vendorName = db.Vendors.Where(vendor => vendor.VID == vendorId).FirstOrDefault().VendorName;
            if(vendorName.ToLower()=="future")
            {
                return db.AspNetRoles.Where(item => item.Name.ToLower() == "admin").Select(role =>
                new AspNetUsersLocal
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                }).ToList();
            }
            else
            {
                return db.AspNetRoles.Where(item=>item.Name.ToLower()!="admin").Select(role =>
                new AspNetUsersLocal
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                }).ToList();
            }

        }
        public List<AspNetUsersLocal> GetAllRoles()
        {
            var query = from role in db.AspNetRoles
                        select new AspNetUsersLocal
                        {
                            RoleId = role.Id,
                            RoleName = role.Name,

                        };
            var abc = query.ToList();
            return query.ToList();
        }
    }
}