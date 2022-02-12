using DevExpress.Web.Mvc;
using FBG.Market.Web.Identity.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Web;
using System.Web.Mvc;
using System.IO;
using System.Net;
using System.Drawing;
using System.Drawing.Imaging;
using FBG.Market.Web.Identity.Helpers;
using DevExpress.Data.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using DevExpress.Utils;
using AutoMapper;
using FBG.Market.Web.Identity.ViewModel;

namespace FBG.Market.Web.Identity.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        public const string EditResultKey = "EditResult";
        public const string EditErrorKey = "EditError";
        FBGMarketEntities db = new FBGMarketEntities();
        Utils Utils = new Utils();
        public ActionResult Index()
        {
            var brands = db.Brands.Select(brand => new BrandLocal
            {
                BrandID = brand.BID,
                BrandName = brand.BrandName + "_" + db.RefCategories.Where(s => s.ID == brand.BrandCategory).FirstOrDefault().Category,
            });

            return View(brands.ToList());
        }

        [HttpGet]
        public ActionResult Add()
        {
            var model = new ProductViewModel();
            return View("Add", model);
        }

        [HttpPost]
        public async Task<ActionResult> Add(ProductViewModel model)
        {
            if (!string.IsNullOrEmpty(model.SKUCode))
            {
                var skuRetVal = db.Products.FirstOrDefault(item => item.SKUCode.ToLower() == model.SKUCode.ToLower());
                if (skuRetVal != null)
                {
                    ModelState.AddModelError("Error", "SKU Code already exists");
                    return PartialView("Add", model);
                }
            }
            if (!string.IsNullOrEmpty(model.UPCCode))
            {
                var upcRetVal = db.Products.FirstOrDefault(item => item.UPCCode.ToLower() == model.UPCCode.ToLower());
                if (upcRetVal != null)
                {
                    ModelState.AddModelError("Error", "UPC Code already exists");
                    return PartialView("Add", model);
                }
            }
            if (model.ProductStatusId == 2 || model.ProductStatusId == 3)
            {
                if (string.IsNullOrEmpty(model.SKUCode) || string.IsNullOrEmpty(model.UPCCode)
                    || model.PWholesalePrice <= 0 || model.PMSRPPrice <= 0 || model.PFOBCost <= 0 || model.PLandedCost <= 0)
                {
                    ModelState.AddModelError("Error", "SKU code, UPC code, Wholesale, MSRP, FOB, and Landed cost are required for Final-wholesale & Final-consumer");
                }
                return PartialView("Add", model);
            }


            if (string.IsNullOrEmpty(model.SKUCode))
                model.SKUCode = string.Empty;
            if (string.IsNullOrEmpty(model.UPCCode))
                model.UPCCode = string.Empty;

            try
            {
                var config = new MapperConfiguration(cfg => cfg.CreateMap<ProductViewModel, Product>());
                var mapper = config.CreateMapper();
                var product = mapper.Map<ProductViewModel, Product>(model);

                product.CreateDate = DateTime.UtcNow;

                db.Products.Add(product);
                await db.SaveChangesAsync();
                ViewData[EditResultKey] = "Product is added successfully.";
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;

                return PartialView("Add", model);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int Id)
        {
            if (Id == -1)
                return null;

            var product = db.Products.Where(p => p.PID == Id).FirstOrDefault();
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Product, ProductViewModel>());
            var mapper = config.CreateMapper();
            var productVM = mapper.Map<Product, ProductViewModel>(product);

            ViewData["VID"] = product.VID;

            if (product != null)
            {
                var images = new List<string>();
                try
                {
                    if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(product.PPicture)))
                    {
                        product.PPicture = $"~/brands/{product.BID}/{product.PID}/{Guid.NewGuid()}";
                    }

                    if (!string.IsNullOrEmpty(product.PDescription))
                    {
                        var htmlDoc = new HtmlDocument();
                        htmlDoc.LoadHtml(product.PDescription);

                        var imageNodes = htmlDoc.DocumentNode.SelectNodes("//img/@src");
                        if (imageNodes != null)
                        {
                            foreach (HtmlNode node in htmlDoc.DocumentNode.SelectNodes("//img/@src"))
                            {
                                images.Add(node.Attributes["src"].Value);
                            }
                        }
                    }

                    if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(product.PPicture)))
                    {
                        Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(product.PPicture));
                    }

                    //var totalImages = Directory.GetFiles(System.Web.HttpContext.Current.Server.MapPath(product.PPicture), "*.*", SearchOption.AllDirectories).Length;
                    //if (totalImages == 0)
                    //{
                    //    string imageUrl = product.PPicture;

                    //    if (!string.IsNullOrEmpty(product.ShopifyPicUrl))
                    //    {
                    //        await DownloadProductImage(product.ShopifyPicUrl, imageUrl, product.PID);
                    //    }
                    //    if (images.Count > 0)
                    //    {
                    //        foreach (var image in images)
                    //        {
                    //            await DownloadProductImage(image, imageUrl, product.PID);
                    //        }
                    //    }
                    //}
                    if (images.Count > 0)
                    {
                        await UpdateProductNewAsync(productVM);
                    }

                    ViewData["PDescription"] = product.PDescription;
                    return View("Edit", productVM);
                }
                catch (Exception ex)
                {
                    ViewData["PDescription"] = ex.Message;

                    return View("Edit", new ProductViewModel());
                }
            }
            else
            {
                ViewData["PDescription"] = "No product selected";
                return View("Edit", new ProductViewModel());
            }
        }

        [HttpPost]
        public async Task<ActionResult> Edit(ProductViewModel model)
        {
            var productsImagePath = System.Web.HttpContext.Current.Request.Params["products-path"];
            model.PID = Convert.ToInt32(System.Web.HttpContext.Current.Request.Params["PID"]);

            if (!string.IsNullOrEmpty(model.SKUCode))
            {
                var skuRetVal = db.Products.FirstOrDefault(item => item.SKUCode.ToLower() == model.SKUCode.ToLower() && item.PID != model.PID);
                if (skuRetVal != null)
                {
                    ModelState.AddModelError("Error", $"SKU Code {model.SKUCode}, already exists");
                    return PartialView("Edit", model);
                }
            }
            if (!string.IsNullOrEmpty(model.UPCCode))
            {
                var upcRetVal = db.Products.FirstOrDefault(item => item.UPCCode.ToLower() == model.UPCCode.ToLower() && item.PID != model.PID);
                if (upcRetVal != null)
                {
                    ModelState.AddModelError("Error", $"UPC Code {model.UPCCode}, already exists");
                    return PartialView("Edit", model);
                }
            }

            if (string.IsNullOrEmpty(model.SKUCode))
                model.SKUCode = string.Empty;
            if (string.IsNullOrEmpty(model.UPCCode))
                model.UPCCode = string.Empty;

            if (model.ProductStatusId == 2 || model.ProductStatusId == 3)
            {
                if (string.IsNullOrEmpty(model.SKUCode) || string.IsNullOrEmpty(model.UPCCode)
                    || model.PWholesalePrice <= 0 || model.PMSRPPrice <= 0 || model.PFOBCost <= 0 || model.PLandedCost <= 0)
                {
                    ModelState.AddModelError("Error", "SKU code, UPC code, Wholesale, MSRP, FOB, and Landed cost fields are required for Final-wholesale & Final-consumer");
                }
                return PartialView("Edit", model);
            }

            try
            {
                var product = db.Products.FirstOrDefault(it => it.PID == model.PID);
                if (product != null)
                {
                    product.BID = model.BID;
                    product.BrandCategoryId = model.BID;
                    product.VID = model.VID;
                    product.PCategory = model.PCategory;
                    product.PSubCategory = model.PSubCategory;
                    product.PSize = model.PSize;
                    product.NRFColorCodeID = model.NRFColorCodeID;
                    product.PColor = model.PColor;
                    product.VendorName = model.VendorName;
                    product.SID = model.SID;
                    product.NRFColorCodeID = model.NRFColorCodeID;
                    product.PCoutryofOrigin = model.PCoutryofOrigin;
                    product.PDescription = model.PDescription;
                    product.PSpecs = model.PSpecs;
                    //modelItem.PFID = product.PFID;
                    if (User.IsInRole("SuperAdmin") || User.IsInRole("Admin"))
                    {
                        product.PFOBCost = model.PFOBCost;
                        product.PLandedCost = model.PLandedCost;
                    }
                    product.PName = model.PName;
                    product.PPicture = model.PPicture;
                    product.SKUCode = model.SKUCode;
                    product.UPCCode = model.UPCCode;
                    product.PMSRPPrice = model.PMSRPPrice;
                    product.PWholesalePrice = model.PWholesalePrice;
                    product.ProductStatusId = model.ProductStatusId;
                    product.UpdatedDate = DateTime.UtcNow;
                    this.TryUpdateModel(product);
                    await db.SaveChangesAsync();
                    ViewData[EditResultKey] = "Product is updated successfully.";
                }
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }

            return RedirectToAction("Index");
        }

        public ActionResult Import()
        {
            return View();
        }

        public ActionResult ExportTo(string selectedIDsHF)
        {
            if (string.IsNullOrEmpty(selectedIDsHF))
            {
                var modelDb = db.Products;
                var modelDBList = modelDb.ToList();
                return RedirectToAction("Index");
            }
            else
            {
                var tokens = selectedIDsHF.Split(',');
                var modelDb = db.Products.Where(item => tokens.ToList().Contains(item.PID.ToString())).ToList();
                return GridViewExtension.ExportToXlsx(GetGridSettings(), modelDb.ToList());
            }
        }

        public async Task<ActionResult> BatchEditingUpdateModelAsync(MVCxGridViewBatchUpdateValues<ProductViewModel, int> updateValues)
        {
            foreach (var product in updateValues.Insert)
            {
                if (updateValues.IsValid(product))
                    await UpdateProductNewAsync(product);
            }
            foreach (var product in updateValues.Update)
            {
                if (updateValues.IsValid(product))
                    await UpdateProductNewAsync(product);
            }

            return GetProductPartialView(-1);
        }

        public ActionResult ProductPartialView(int brandId = -1)
        {
            return GetProductPartialView(brandId);
        }

        private ActionResult GetProductPartialView(int brandId)
        {
            var userId = UserHelper.GetUserId();
            var user = db.AspNetUsers.Where(item => item.Id == userId).FirstOrDefault();

            if (user is null)
                return Content("You don't have a vendor assigned to your user, please ask your admin to assign you a vendor.");

            ViewData["BrandId"] = brandId;

            if (user != null)
                ViewData["VendorId"] = user?.VID ?? 0;

            if (!isAdminUser())
            {
                Session["BrandId"] = brandId;
                Session["VendorId"] = user.VID;

                var products = db.Products.OrderByDescending(x => x.CreateDate).Select(product => new ProductViewModel
                {
                    BID = product.BID,
                    ColorCategoryId = product.ColorCategoryId,
                    NRFColorCodeID = product.NRFColorCodeID,
                    PCategory = product.PCategory,
                    PColor = product.PColor,
                    PCoutryofOrigin = product.PCoutryofOrigin,
                    PDescription = product.PDescription,
                    PDiscontinued = product.PDiscontinued,
                    PFOBCost = product.PFOBCost,
                    PID = product.PID,
                    PLandedCost = product.PLandedCost,
                    PMSRPPrice = product.PMSRPPrice,
                    PName = product.PName,
                    PPicture = product.PPicture,
                    ProductStatusId = product.ProductStatusId,
                    PSize = product.PSize,
                    PSpecs = product.PSpecs,
                    PSubCategory = product.PSubCategory,
                    PWholesalePrice = product.PWholesalePrice,
                    ShopifyPicUrl = product.ShopifyPicUrl,
                    SID = product.SID,
                    SKUCode = product.SKUCode,
                    SSMA_TimeStamp = product.SSMA_TimeStamp,
                    UPCCode = product.UPCCode,
                    VID = product.VID,
                    BrandCategoryId = product.BrandCategoryId,
                    VendorName = product.Vendor.VendorName,
                    CreateDate = product.CreateDate,
                    GroupId = product.GroupId
                });

                if (!isSuperAdminUser())
                    products = products.Where(pro => pro.VID == user.VID);

                return PartialView("_Products", products.ToList());
            }

            return PartialView("_Products", GetEntityServerModeSource(brandId));
        }
        [HttpPost]
        public ActionResult Delete(int productID, string operation, string item, string path)
        {
            if (operation == "Delete")
            {
                var imageFileName = Path.GetFileName(item);
                int paramIndex = imageFileName.IndexOf("?");

                if (paramIndex > 0)
                {
                    imageFileName = imageFileName.Substring(0, paramIndex);
                }

                string fullFileName = Server.MapPath($"{path}/") + imageFileName;
                if (System.IO.File.Exists(fullFileName))
                {
                    System.IO.File.Delete(fullFileName);
                }
            }
            return Redirect(Request.UrlReferrer.ToString());
        }
        public PartialViewResult ImageZoomNavigator(string productPath)
        {
            return PartialView("ImageZoomNavigator", productPath);
        }
        public ActionResult HtmlEditorPartialImageSelectorUpload()
        {
            HtmlEditorExtension.SaveUploadedImage("ProductDescriptionHtmlEditor", PDescHtmlEditorSettings.ImageSelectorSettings);
            return null;
        }
        public ActionResult HtmlEditorPartialImageUpload()
        {
            HtmlEditorExtension.SaveUploadedFile("ProductDescriptionHtmlEditor", PDescHtmlEditorSettings.ImageUploadValidationSettings, PDescHtmlEditorSettings.ImageUploadDirectory);
            return null;
        }
        public ActionResult HtmlEditorPartial(string PDescription)
        {
            return PartialView("_HtmlEditorPartial", PDescription);
        }
        [Authorize]
        public PartialViewResult MultiFileSelection(string productPath)
        {
            return PartialView("~/Views/UploadControl/MultiFileSelection.cshtml", productPath);
        }

        #region utility functions
        private Boolean isAdminUser()
        {
            if (!User.Identity.IsAuthenticated)
                return false;

            ApplicationDbContext context = new ApplicationDbContext();
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            var roles = UserManager.GetRoles(User.Identity.GetUserId());

            if (roles is null || roles?.Count == 0)
                return false;

            if (roles.Contains("Admin"))
                return true;

            return false;
        }
        private Boolean isSuperAdminUser()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity;
                ApplicationDbContext context = new ApplicationDbContext();
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                var id = user.GetUserId();
                var s = UserManager.GetRoles(user.GetUserId());
                if (s.Count == 0)
                    return false;
                if (s[0].ToString() == "SuperAdmin")
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

        private EntityServerModeSource GetEntityServerModeSource(int brandId)
        {
            EntityServerModeSource esms = new EntityServerModeSource();
            esms.KeyExpression = "PID";
            esms.QueryableSource = db.Products;

            return esms;
        }
        private GridViewSettings GetGridSettings()
        {
            var gridViewSettings = new GridViewSettings();
            gridViewSettings.Name = "Liveproducts";
            gridViewSettings.CallbackRouteValues = new { Controller = "Product", Action = "ProductPartialView" };

            // Export-specific settings 
            gridViewSettings.SettingsExport.ExportSelectedRowsOnly = false;
            gridViewSettings.SettingsExport.FileName = "live_products.xlsx";

            gridViewSettings.Columns.Add(col =>
            {
                col.Visible = true;
                col.FieldName = "PID";
                col.Width = System.Web.UI.WebControls.Unit.Pixel(80);
            });
            gridViewSettings.Columns.Add(c =>
            {
                //c.GroupIndex = 0;
                c.FieldName = "VID";
                c.Caption = "Vendor";
                c.Width = System.Web.UI.WebControls.Unit.Pixel(100);
                c.Settings.AutoFilterCondition = AutoFilterCondition.Contains;
                c.EditorProperties().ComboBox(p =>
                {
                    p.TextField = "VendorName";
                    p.ValueField = "VendorId";
                    p.ValueType = typeof(int);
                    p.DataSource = Utils.GetVendors();
                });
            });

            gridViewSettings.Columns.Add(c =>
            {
                c.FieldName = "BID";
                c.Caption = "Brand";
                c.Width = System.Web.UI.WebControls.Unit.Pixel(100);
                c.Settings.AutoFilterCondition = AutoFilterCondition.Contains;
                c.EditorProperties().ComboBox(p =>
                {
                    p.TextField = "BrandName";
                    p.ValueField = "BrandID";
                    p.ValueType = typeof(int);
                    p.DataSource = Utils.GetBrands();
                });
            });
            gridViewSettings.Columns.Add(c =>
            {

                //c.GroupIndex = 0;
                c.FieldName = "BrandCategoryId";
                c.Caption = "Brand & Category";
                c.Width = System.Web.UI.WebControls.Unit.Pixel(130);
                c.Settings.AutoFilterCondition = AutoFilterCondition.Contains;
                c.EditorProperties().ComboBox(p =>
                {
                    p.TextField = "BrandName";
                    p.ValueField = "BrandID";
                    p.ValueType = typeof(int);
                    p.DataSource = Utils.GetBrandsCategories();
                });
            });
            gridViewSettings.Columns.Add(c =>
            {
                c.Settings.AutoFilterCondition = AutoFilterCondition.Contains;

                c.FieldName = "PCategory";
                c.Caption = "Category";
                c.EditorProperties().ComboBox(p =>
                {
                    p.TextField = "Name";
                    p.ValueField = "Id";
                    p.ValueType = typeof(int);
                    p.DataSource = Utils.GetCategories();
                });
            });
            //PSubCategory
            gridViewSettings.Columns.Add(col =>
            {
                col.Settings.AutoFilterCondition = AutoFilterCondition.Contains;

                col.FieldName = "PSubCategory";
                col.Caption = "Sub Category";

            });
            gridViewSettings.Columns.Add(col =>
            {
                col.Settings.AutoFilterCondition = AutoFilterCondition.Contains;

                col.FieldName = "VendorName";
                col.Caption = "Vendor Style #";

            });
            gridViewSettings.Columns.Add(col =>
            {
                col.Settings.AutoFilterCondition = AutoFilterCondition.Contains;

                col.Width = System.Web.UI.WebControls.Unit.Pixel(100);

                col.FieldName = "PName";
                col.Caption = "Style";

            });
            gridViewSettings.Columns.Add(col =>
            {
                col.Settings.AutoFilterCondition = AutoFilterCondition.Contains;
                col.Width = System.Web.UI.WebControls.Unit.Pixel(80);
                col.FieldName = "PColor";
                col.Caption = "Color";

            });
            gridViewSettings.Columns.Add(c =>
            {
                c.Settings.AutoFilterCondition = AutoFilterCondition.Contains;

                c.FieldName = "NRFColorCodeID";
                c.Caption = "NRF Color";
                c.EditorProperties().ComboBox(p =>
                {
                    p.TextField = "Name";
                    p.ValueField = "Id";
                    p.ValueType = typeof(int);
                    p.DataSource = Utils.GetColors();
                });
            });
            gridViewSettings.Columns.Add(columnSettings =>
            {
                columnSettings.Settings.AutoFilterCondition = AutoFilterCondition.Contains;
                columnSettings.Width = System.Web.UI.WebControls.Unit.Pixel(130);
                columnSettings.FieldName = "SKUCode";
                columnSettings.Caption = "SKU Code";
            });

            gridViewSettings.Columns.Add(columnSettings =>
            {
                columnSettings.Settings.AutoFilterCondition = AutoFilterCondition.Contains;
                columnSettings.FieldName = "UPCCode";
                columnSettings.Caption = "UPC Code";
                columnSettings.Width = System.Web.UI.WebControls.Unit.Pixel(100);
            });

            gridViewSettings.Columns.Add(c =>
            {
                c.Settings.AutoFilterCondition = AutoFilterCondition.Contains;
                c.Width = System.Web.UI.WebControls.Unit.Pixel(80);

                c.FieldName = "SID";
                c.Caption = "Season";
                c.EditorProperties().ComboBox(p =>
                {
                    p.TextField = "SeasonName";
                    p.ValueField = "SeasonID";
                    p.ValueType = typeof(int);
                    p.DataSource = Utils.GetSeasons();
                });
            });

            //PSize
            gridViewSettings.Columns.Add(col =>
            {
                col.Settings.AutoFilterCondition = AutoFilterCondition.Contains;
                col.Width = System.Web.UI.WebControls.Unit.Pixel(80);

                col.FieldName = "PSize";
                col.Caption = "Product Size";

            });

            //PCoutryofOrigin
            gridViewSettings.Columns.Add(col =>
            {
                col.Settings.AutoFilterCondition = AutoFilterCondition.Contains;

                col.FieldName = "PCoutryofOrigin";
                col.Caption = "Country of Origin";

            });

            {
                gridViewSettings.Columns.Add(col =>
                {
                    col.Settings.AutoFilterCondition = AutoFilterCondition.Contains;

                    col.FieldName = "PFOBCost";
                    col.Caption = "FOB Cost";
                    col.PropertiesEdit.DisplayFormatString = "{0:C}";
                });
            }
            {
                gridViewSettings.Columns.Add(col =>
                {
                    col.Settings.AutoFilterCondition = AutoFilterCondition.Contains;

                    col.FieldName = "PLandedCost";
                    col.Caption = "Landed Cost";
                    col.PropertiesEdit.DisplayFormatString = "{0:C}";
                });
            }
            {
                gridViewSettings.Columns.Add(col =>
                {
                    col.Settings.AutoFilterCondition = AutoFilterCondition.Contains;

                    col.FieldName = "PWholesalePrice";
                    col.Caption = "Wholesale Price";
                    col.PropertiesEdit.DisplayFormatString = "{0:C}";

                });
            }
            {
                gridViewSettings.Columns.Add(col =>
                {
                    col.Settings.AllowAutoFilter = DefaultBoolean.False;

                    col.FieldName = "PMSRPPrice";
                    col.Caption = "MSRP";
                    col.PropertiesEdit.DisplayFormatString = "{0:C}";
                });
            }
            gridViewSettings.Columns.Add(c =>
            {
                c.Settings.AutoFilterCondition = AutoFilterCondition.Contains;

                c.FieldName = "ProductStatusId";
                c.Caption = "Status";
                c.EditorProperties().ComboBox(p =>
                {
                    p.TextField = "Status";
                    p.ValueField = "Id";
                    p.ValueType = typeof(int);
                    p.DataSource = Utils.GetProductStatus();
                });
            });
            gridViewSettings.Columns.Add(col =>
            {
                col.Settings.AutoFilterCondition = AutoFilterCondition.Contains;

                col.FieldName = "PPicture";
                col.Caption = "Picture";

            });
            //PDescription
            gridViewSettings.Columns.Add(col =>
            {
                col.Settings.AutoFilterCondition = AutoFilterCondition.Contains;

                col.FieldName = "PDescription";
                col.Caption = "Description";

            });
            //PSpecs
            gridViewSettings.Columns.Add(col =>
            {
                col.Settings.AutoFilterCondition = AutoFilterCondition.Contains;

                col.FieldName = "PSpecs";
                col.Caption = "Specs";

            });
            gridViewSettings.SettingsExport.BeforeExport = (sender, e) =>
            {
                MVCxGridView gridView = sender as MVCxGridView;
            };
            return gridViewSettings;
        }

        private async Task UpdateProductNewAsync(ProductViewModel model)
        {
            var product = db.Products.FirstOrDefault(p => p.PID == model.PID);
            product.BID = model.BID;
            product.BrandCategoryId = model.BrandCategoryId;
            product.ColorCategoryId = model.ColorCategoryId;
            product.GroupId = model.GroupId;
            product.Message = model.Message;
            product.NRFColorCodeID = model.NRFColorCodeID;
            product.PCategory = model.PCategory;
            product.PColor = model.PColor;
            product.PCoutryofOrigin = model.PCoutryofOrigin;
            product.PDescription = model.PDescription;
            product.PDiscontinued = model.PDiscontinued;
            product.PFOBCost = model.PFOBCost;
            product.PID = model.PID;
            product.PLandedCost = model.PLandedCost;
            product.PMSRPPrice = model.PMSRPPrice;
            product.PName = model.PName;
            product.PPicture = model.PPicture;
            product.ProductStatusId = model.ProductStatusId;
            product.PSize = model.PSize;
            product.PSpecs = model.PSpecs;
            product.PSubCategory = model.PSubCategory;
            product.PWholesalePrice = model.PWholesalePrice;
            product.ShopifyPicUrl = model.ShopifyPicUrl;
            product.SID = model.SID;
            product.SKUCode = model.SKUCode;
            product.SSMA_TimeStamp = model.SSMA_TimeStamp;
            product.UPCCode = model.UPCCode;
            product.UpdatedDate = model.UpdatedDate;
            product.VendorName = model.VendorName;
            product.VID = model.VID;
            this.TryUpdateModel(product);
            await db.SaveChangesAsync();
            ViewData[EditResultKey] = "Product is updated successfully.";
        }

        private async Task DownloadProductImage(string url, string path, int pid)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    var uri = new Uri(url);
                    byte[] data = await webClient.DownloadDataTaskAsync(uri);

                    using (MemoryStream mem = new MemoryStream(data))
                    {
                        using (var yourImage = Image.FromStream(mem))
                        {

                            var guid = Guid.NewGuid();
                            var imagePath = Path.Combine(Server.MapPath(path), $"{guid}-product-image-{pid}.png");
                            if (System.IO.File.Exists(imagePath))
                            {
                                System.IO.File.Delete(imagePath);
                            }
                            // If you want it as Png
                            yourImage.Save(imagePath, ImageFormat.Png);
                        }
                    }

                }
            }
            catch (Exception ex)
            {

            }
        }
        #endregion
    }
    public class PDescHtmlEditorSettings
    {
        public const string ImageUploadDirectory = "~/brands/brand-1/";
        public const string ImageSelectorThumbnailDirectory = "~/Thumb/";

        public static DevExpress.Web.UploadControlValidationSettings ImageUploadValidationSettings = new DevExpress.Web.UploadControlValidationSettings()
        {
            AllowedFileExtensions = new string[] { ".jpg", ".jpeg", ".jpe", ".gif", ".png" },
            MaxFileSize = 4000000
        };

        static MVCxHtmlEditorImageSelectorSettings imageSelectorSettings;
        public static MVCxHtmlEditorImageSelectorSettings ImageSelectorSettings
        {
            get
            {
                if (imageSelectorSettings == null)
                {
                    imageSelectorSettings = new MVCxHtmlEditorImageSelectorSettings(null);
                    imageSelectorSettings.Enabled = true;
                    imageSelectorSettings.UploadCallbackRouteValues = new { Controller = "Product", Action = "HtmlEditorPartialImageSelectorUpload" };
                    imageSelectorSettings.CommonSettings.RootFolder = ImageUploadDirectory;
                    imageSelectorSettings.CommonSettings.ThumbnailFolder = ImageSelectorThumbnailDirectory;
                    imageSelectorSettings.CommonSettings.AllowedFileExtensions = new string[] { ".jpg", ".jpeg", ".jpe", ".gif" };
                    imageSelectorSettings.UploadSettings.Enabled = true;
                }
                return imageSelectorSettings;
            }
        }
    }
}