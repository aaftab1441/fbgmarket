using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using FBG.Market.Databackfiller.BlobStorage;
using FBG.Market.Databackfiller.Helpers;
using FBG.Market.Web.Identity.Models;

namespace FBG.Market.Databackfiller
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Check all existing elements from Blob Storage
            // AzureBlobClient blobClient1 = new AzureBlobClient();
            // blobClient1.ListAllBlobs();

            // Initialize
            FBGMarketEntities db = new FBGMarketEntities();
            List<ShopifyRecord> shopifyRecords = new List<ShopifyRecord>();

            // Read CSV
            using (var reader = new StreamReader("Source/Full_export_v2.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                // Get All records
                shopifyRecords = csv.GetRecords<ShopifyRecord>().ToList();

                // Insert Vendors
                InsertVendors(db);
                InsertBrands(GetShopifyRecordsDeepCopy(shopifyRecords), db);

                // Insert Categories
                InsertCategories(GetShopifyRecordsDeepCopy(shopifyRecords), db);

                // Backfill Missing rowData
                shopifyRecords = FillMissingData(shopifyRecords.ToList());

                // Insert Products
                foreach (var item in shopifyRecords)
                {
                    try
                    {
                        Product record = new Product();

                        record.Vendor = db.Vendors.FirstOrDefault(c => c.VendorName == item.Vendor);
                        record.PDescription = item.Body_HTML;
                        if (!string.IsNullOrEmpty(item.CustomProductType))
                            record.PCategory = Convert.ToInt16(db.RefCategories.FirstOrDefault(c => c.Category == item.CustomProductType).ID);

                        if (item.Option1Name == "Size")
                            record.PSize = item.Option1Value;
                        else
                            record.PColor = item.Option1Value;

                        record.SKUCode = item.VariantSKU.Replace(@"'", "").Trim();
                        record.UPCCode = item.VariantBarcode.Replace(@"'", "").Trim();

                        // Required not in Source File
                        record.SSMA_TimeStamp = Encoding.Unicode.GetBytes("");
                        // DateTime.Now.ToShortDateString()
                        record.PWholesalePrice = 0;
                        record.PDiscontinued = false;
                        record.PName = item.Title;

                        db.Products.Add(record);
                        db.SaveChanges();
                        Console.WriteLine("Record Inserted : " + item.Handle);

                        // Upload Image to Blob
                        if (!string.IsNullOrEmpty(item.ImageSrc))
                        {
                            string path = record.Vendor.VendorName + "/" + record.PID.ToString();
                            string fullpath = Path.GetFullPath("DownloadedImages/" + path);
                            if (!Directory.Exists(fullpath))
                            {
                                Directory.CreateDirectory(fullpath);
                            }

                            DownloadShopifyImage(item.ImageSrc, fullpath, record.PID.ToString());
                            AzureBlobClient blobClient = new AzureBlobClient();
                            string blobURI = blobClient.UploadBlob(record.PID.ToString(), path);

                            // Image src
                            record.ProductImages = new List<ProductImage>();
                            record.ProductImages.Add(new ProductImage { ImageUrl = blobURI, Id = new Random(Guid.NewGuid().GetHashCode()).Next() });
                            db.SaveChanges();

                            Console.WriteLine("Blob uploaded : " + blobURI);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Record skipped");
                        Console.WriteLine(ex.Message);
                        continue;
                    }
                }
            }
        }

        private static void DownloadShopifyImage(string imageURL, string location, string ProductID)
        {
            string fullpath = location + "/" + ProductID + ".jpg";
            using (WebClient client = new WebClient())
            {
                client.DownloadFile(new Uri(imageURL), fullpath);
            }
        }

        private static List<ShopifyRecord> FillMissingData(List<ShopifyRecord> shopifyRecords)
        {
            // Output collection
            List<ShopifyRecord> filledInRecords = new List<ShopifyRecord>();

            // Extract Distinct Handles
            List<string> handles = shopifyRecords.Select(c => c.Handle).Distinct().ToList();

            // For Each Handle backfill data
            foreach (string handle in handles)
            {
                if ((shopifyRecords.Where(k => k.Handle == handle).ToList().Count()) == 1)
                {
                    // Occurs only once so add them directly
                    filledInRecords.Add(shopifyRecords.FirstOrDefault(c => c.Handle == handle));
                }
                else
                {
                    // Occurs more than once so need to substitute data

                    // Pick up all records with Handle
                    var items = shopifyRecords.Where(k => k.Handle == handle).ToList();

                    // Select Source
                    var sourceItem = items.First();

                    // Add source to Result
                    filledInRecords.Add(sourceItem);

                    // Remove source from Records to be backfilled
                    items.RemoveAt(0);

                    // Backfill missing Data
                    foreach (var item in items)
                    {
                        var clonedItem = GetShopifyRecordDeepCopy(item);

                        if (string.IsNullOrEmpty(clonedItem.Vendor))
                            clonedItem.Vendor = sourceItem.Vendor;
                        if (string.IsNullOrEmpty(clonedItem.Body_HTML))
                            clonedItem.Body_HTML = sourceItem.Body_HTML;
                        if (string.IsNullOrEmpty(clonedItem.CustomProductType))
                            clonedItem.CustomProductType = sourceItem.CustomProductType;
                        if (string.IsNullOrEmpty(clonedItem.Option1Value))
                            clonedItem.Option1Value = sourceItem.Option1Value;
                        if (string.IsNullOrEmpty(clonedItem.VariantSKU))
                            clonedItem.VariantSKU = sourceItem.VariantSKU;
                        if (string.IsNullOrEmpty(clonedItem.VariantBarcode))
                            clonedItem.VariantBarcode = sourceItem.VariantBarcode;
                        if (string.IsNullOrEmpty(clonedItem.Title))
                            clonedItem.Title = sourceItem.Title;

                        filledInRecords.Add(clonedItem);
                    }
                }
            }


            return filledInRecords;
        }

        private static List<ShopifyRecord> GetShopifyRecordsDeepCopy(List<ShopifyRecord> originals)
        {
            List<ShopifyRecord> copy = new List<ShopifyRecord>();
            foreach (var item in originals)
            {
                copy.Add(GetShopifyRecordDeepCopy(item));
            }

            return copy;
        }

        private static ShopifyRecord GetShopifyRecordDeepCopy(ShopifyRecord original)
        {
            ShopifyRecord copy = new ShopifyRecord();
            copy.Handle = original.Handle;
            copy.ImageSrc = original.ImageSrc;
            copy.Body_HTML = original.Body_HTML;
            copy.Vendor = original.Vendor;
            copy.StandardizedProductType = original.StandardizedProductType;
            copy.CustomProductType = original.CustomProductType;
            copy.Tags = original.Tags;
            copy.Published = original.Published;
            copy.Option1Name = original.Option1Name;
            copy.Option1Value = original.Option1Value;
            copy.Option2Name = original.Option2Name;
            copy.Option2Value = original.Option2Value;
            copy.Option3Name = original.Option3Name;
            copy.Option3Value = original.Option3Value;
            copy.VariantSKU = original.VariantSKU;
            copy.VariantGrams = original.VariantGrams;
            copy.VariantInventoryTracker = original.VariantInventoryTracker;
            copy.VariantInventoryPolicy = original.VariantInventoryPolicy;
            copy.VariantFulfillmentService = original.VariantFulfillmentService;
            copy.VariantPrice = original.VariantPrice;
            copy.VariantCompareAtPrice = original.VariantCompareAtPrice;
            copy.VariantRequiresShipping = original.VariantRequiresShipping;
            copy.VariantTaxable = original.VariantTaxable;
            copy.VariantBarcode = original.VariantBarcode;
            copy.ImageSrc = original.ImageSrc;
            copy.ImagePosition = original.ImagePosition;
            copy.ImageAltText = original.ImageAltText;
            copy.GiftCard = original.GiftCard;
            copy.SEOTitle = original.SEOTitle;
            copy.SEODescription = original.SEODescription;
            copy.GoogleShoppingGoogleProductCategory = original.GoogleShoppingGoogleProductCategory;
            copy.GoogleShoppingGender = original.GoogleShoppingGender;
            copy.GoogleShoppingAgeGroup = original.GoogleShoppingAgeGroup;
            copy.GoogleShoppingMPN = original.GoogleShoppingMPN;
            copy.GoogleShoppingAdWordsGrouping = original.GoogleShoppingAdWordsGrouping;
            copy.GoogleShoppingAdWordsLabels = original.GoogleShoppingAdWordsLabels;
            copy.GoogleShoppingCondition = original.GoogleShoppingCondition;
            copy.GoogleShoppingCustomProduct = original.GoogleShoppingCustomProduct;
            copy.GoogleShoppingCustomLabel0 = original.GoogleShoppingCustomLabel0;
            copy.GoogleShoppingCustomLabel1 = original.GoogleShoppingCustomLabel1;
            copy.GoogleShoppingCustomLabel2 = original.GoogleShoppingCustomLabel2;
            copy.GoogleShoppingCustomLabel3 = original.GoogleShoppingCustomLabel3;
            copy.GoogleShoppingCustomLabel4 = original.GoogleShoppingCustomLabel4;
            copy.VariantImage = original.VariantImage;
            copy.VariantWeightUnit = original.VariantWeightUnit;
            copy.VariantTaxCode = original.VariantTaxCode;
            copy.Costperitem = original.Costperitem;
            copy.Status = original.Status;

            return copy;
        }

        private static void InsertCategories(List<ShopifyRecord> shopifyRecords, FBGMarketEntities db)
        {
            IEnumerable<string> categories = shopifyRecords.Select(c => c.CustomProductType).Distinct().ToList();
            foreach (string category in categories)
            {
                RefCategory fbgCategory = new RefCategory();
                fbgCategory.Category = category;

                if (!db.RefCategories.Any(c => c.Category == category) && !string.IsNullOrEmpty(category))
                    db.RefCategories.Add(fbgCategory);
            }

            db.SaveChanges();
        }

        private static void InsertVendors(FBGMarketEntities db)
        {
            var vendor = new Vendor
            {
                VID = 1,
                VendorName = "Nasign",
                VendorAcronym = "NSN",
                VendorAddress = "서울시 광진구 동일로 286-1(군자동)",
                VendorCityState = "Seoul",
                VendorCountry = "Korea",
                VendorEmail = "info@FutureBrandGroup.com",
                VendorWebsite = "http://www.oryany.co.kr/",
                VendorLogo = "oryany.png",
                UserId = "b74a6497-15bc-41e8-abe8-fb822dc42474"
            };
            if (!db.Brands.Any(c => c.BrandName == vendor.VendorName))
                db.Vendors.Add(vendor);

            vendor = new Vendor
            {
                VID = 2,
                VendorName = "KIWI",
                VendorAcronym = "KIW",
                VendorEmail = "info@FutureBrandGroup.com",
                VendorWebsite = "http://fbgmarket.com/",
                VendorLogo = "oryany.png",
                UserId = "b74a6497-15bc-41e8-abe8-fb822dc42474"
            };
            if (!db.Brands.Any(c => c.BrandName == vendor.VendorName))
                db.Vendors.Add(vendor);

            vendor = new Vendor
            {
                VID = 3,
                VendorName = "BALIBALI",
                VendorAcronym = "BALI",
                VendorAddress = "Jl. Meliling No.kangin, Meliling, Kec",
                VendorEmail = "info@FutureBrandGroup.com",
                VendorWebsite = "http://fbgmarket.com/",
                VendorLogo = "logobalibali.png",
                UserId = "b74a6497-15bc-41e8-abe8-fb822dc42474"
            };
            if (!db.Brands.Any(c => c.BrandName == vendor.VendorName))
                db.Vendors.Add(vendor);

            vendor = new Vendor
            {
                VID = 4,
                VendorName = "FFC",
                VendorAcronym = "FFC",
                VendorEmail = "info@FutureBrandGroup.com",
                VendorWebsite = "http://www.oryany.co.kr/",
                UserId = "b74a6497-15bc-41e8-abe8-fb822dc42474"
            };
            if (!db.Brands.Any(c => c.BrandName == vendor.VendorName))
                db.Vendors.Add(vendor);

            vendor = new Vendor
            {
                VID = 5,
                VendorName = "FUTURE",
                VendorAcronym = "FBG",
                VendorAddress = "23-12 41st Street",
                VendorCityState = "Astoria, new York",
                VendorCountry = "USA",
                VendorPhone = "212-665-9900",
                VendorEmail = "info@FutureBrandGroup.com",
                VendorWebsite = "http://www.oryany.co.kr/",
                VendorLogo = "oryany.png",
                UserId = "b74a6497-15bc-41e8-abe8-fb822dc42474"
            };
            if (!db.Brands.Any(c => c.BrandName == vendor.VendorName))
                db.Vendors.Add(vendor);

            vendor = new Vendor
            {
                VID = 6,
                VendorName = "Unplugged",
                VendorAcronym = "UPLG",
                VendorCityState = "Houston",
                VendorCountry = "USA",
                VendorPhone = "212-665-9900",
                VendorEmail = "info@FutureBrandGroup.com",
                VendorWebsite = "http://www.oryany.co.kr/",
                UserId = "b74a6497-15bc-41e8-abe8-fb822dc42474"
            };
            if (!db.Brands.Any(c => c.BrandName == vendor.VendorName))
                db.Vendors.Add(vendor);

            vendor = new Vendor
            {
                VID = 7,
                VendorName = "Good People",
                VendorAcronym = "GPPL",
                VendorCityState = "Paris",
                VendorCountry = "France",
                VendorPhone = "(233) 434-3234",
                VendorEmail = "info@goodpeople.com",
                VendorWebsite = "http://www.goodpeople.com/",
                VendorLogo = "oryany.png",
                UserId = "b74a6497-15bc-41e8-abe8-fb822dc42474"
            };
            if (!db.Brands.Any(c => c.BrandName == vendor.VendorName))
                db.Vendors.Add(vendor);

            vendor = new Vendor
            {
                VID = 8,
                VendorName = "Chanour",
                UserId = "b74a6497-15bc-41e8-abe8-fb822dc42474"
            };
            if (!db.Brands.Any(c => c.BrandName == vendor.VendorName))
                db.Vendors.Add(vendor);

            vendor = new Vendor
            {
                VID = 9,
                VendorName = "Code917",
                UserId = "b74a6497-15bc-41e8-abe8-fb822dc42474"
            };
            if (!db.Brands.Any(c => c.BrandName == vendor.VendorName))
                db.Vendors.Add(vendor);

            vendor = new Vendor
            {
                VID = 10,
                VendorName = "Katrina Szish x JELAVU",
                UserId = "b74a6497-15bc-41e8-abe8-fb822dc42474"
            };
            if (!db.Brands.Any(c => c.BrandName == vendor.VendorName))
                db.Vendors.Add(vendor);

            vendor = new Vendor
            {
                VID = 11,
                VendorName = "Lancaster",
                UserId = "b74a6497-15bc-41e8-abe8-fb822dc42474"
            };
            if (!db.Brands.Any(c => c.BrandName == vendor.VendorName))
                db.Vendors.Add(vendor);

            vendor = new Vendor
            {
                VID = 12,
                VendorName = "Jelavu",
                UserId = "b74a6497-15bc-41e8-abe8-fb822dc42474"
            };
            if (!db.Brands.Any(c => c.BrandName == vendor.VendorName))
                db.Vendors.Add(vendor);

            vendor = new Vendor
            {
                VID = 13,
                VendorName = "Oryany",
                UserId = "b74a6497-15bc-41e8-abe8-fb822dc42474"
            };
            if (!db.Brands.Any(c => c.BrandName == vendor.VendorName))
                db.Vendors.Add(vendor);

            vendor = new Vendor
            {
                VID = 14,
                VendorName = "Saint G",
                UserId = "b74a6497-15bc-41e8-abe8-fb822dc42474"
            };
            if (!db.Brands.Any(c => c.BrandName == vendor.VendorName))
                db.Vendors.Add(vendor);

            vendor = new Vendor
            {
                VID = 15,
                VendorName = "TMRW STUDIO",
                UserId = "b74a6497-15bc-41e8-abe8-fb822dc42474"
            };
            if (!db.Brands.Any(c => c.BrandName == vendor.VendorName))
                db.Vendors.Add(vendor);

            vendor = new Vendor
            {
                VID = 16,
                VendorName = "UN BILLION",
                UserId = "b74a6497-15bc-41e8-abe8-fb822dc42474"
            };
            if (!db.Brands.Any(c => c.BrandName == vendor.VendorName))
                db.Vendors.Add(vendor);

            vendor = new Vendor
            {
                VID = 17,
                VendorName = "FFC NEW YORK",
                UserId = "b74a6497-15bc-41e8-abe8-fb822dc42474"
            };
            if (!db.Brands.Any(c => c.BrandName == vendor.VendorName))
                db.Vendors.Add(vendor);

            vendor = new Vendor
            {
                VID = 18,
                VendorName = "TMRW STUDIO",
                UserId = "b74a6497-15bc-41e8-abe8-fb822dc42474"
            };
            if (!db.Brands.Any(c => c.BrandName == vendor.VendorName))
                db.Vendors.Add(vendor);

            db.SaveChanges();
        }

        private static void InsertBrands(List<ShopifyRecord> shopifyRecords, FBGMarketEntities db)
        {
            IEnumerable<string> brands = shopifyRecords.Select(c => c.Vendor).Distinct().ToList();
            foreach (string brand in brands)
            {
                Brand fbgBrand = new Brand();
                fbgBrand.BrandName = brand;
                if (fbgBrand.BrandName.ToLower().Contains("saint"))
                {
                    fbgBrand.VID = db.Vendors.Where(v => v.VendorName.ToLower() == "kiwi").FirstOrDefault().VID;
                    fbgBrand.BrandCategory = 2;
                }
                else if(fbgBrand.BrandName.ToLower().Contains("oryany"))
                {
                    fbgBrand.VID = db.Vendors.Where(v => v.VendorName.ToLower() == "nasign").FirstOrDefault().VID;
                    fbgBrand.BrandCategory = 1;
                }
                else if (fbgBrand.BrandName.ToLower().Contains("tmrw") || fbgBrand.BrandName.ToLower().Contains("billion")
                    || fbgBrand.BrandName.ToLower().Contains("ffc") || fbgBrand.BrandName.ToLower().Contains("unknown"))
                {
                    fbgBrand.VID = db.Vendors.Where(v => v.VendorName.ToLower() == "ffc").FirstOrDefault().VID;
                    fbgBrand.BrandCategory = 2;
                }
                else if (!fbgBrand.BrandName.ToLower().Contains("katrina") && (fbgBrand.BrandName.ToLower().Contains("code") || fbgBrand.BrandName.ToLower().Contains("jelavu")))
                {
                    fbgBrand.VID = db.Vendors.Where(v => v.VendorName.ToLower() == "future").FirstOrDefault().VID;
                    fbgBrand.BrandCategory = 5;
                }
                else if (fbgBrand.BrandName.ToLower().Contains("plugged"))
                {
                    fbgBrand.VID = db.Vendors.Where(v => v.VendorName.ToLower() == "unplugged").FirstOrDefault().VID;
                    fbgBrand.BrandCategory = 2;
                }
                else if (fbgBrand.BrandName.ToLower().Contains("bali"))
                {
                    fbgBrand.VID = db.Vendors.Where(v => v.VendorName.ToLower() == "balibali").FirstOrDefault().VID;
                    fbgBrand.BrandCategory = 2;
                }
                else if (fbgBrand.BrandName.ToLower().Contains("chanour"))
                {
                    fbgBrand.VID = db.Vendors.Where(v => v.VendorName.ToLower() == "chanour").FirstOrDefault().VID;
                    fbgBrand.BrandCategory = 2;
                }
                else if (fbgBrand.BrandName.ToLower().Contains("good"))
                {
                    fbgBrand.VID = db.Vendors.Where(v => v.VendorName == "Good People").FirstOrDefault().VID;
                    fbgBrand.BrandCategory = 2;
                }
                else if (fbgBrand.BrandName.ToLower().Contains("katrina"))
                {
                    fbgBrand.VID = db.Vendors.Where(v => v.VendorName.ToLower() == "katrina szish x jelavu").FirstOrDefault().VID;
                    fbgBrand.BrandCategory = 2;
                }
                else if (fbgBrand.BrandName.ToLower().Contains("lancaster"))
                {
                    fbgBrand.VID = db.Vendors.Where(v => v.VendorName.ToLower() == "lancaster").FirstOrDefault().VID;
                    fbgBrand.BrandCategory = 2;
                }
                else
                {
                    fbgBrand.VID = db.Vendors.Where(v => v.VendorName.ToLower() == "future").FirstOrDefault().VID;
                    fbgBrand.BrandCategory = 5;
                }

                if (!db.Brands.Any(c => c.BrandName == brand) && !string.IsNullOrEmpty(brand))
                    db.Brands.Add(fbgBrand);
            }

            db.SaveChanges();
        }
    }
}
