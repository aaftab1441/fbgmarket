using DevExpress.Web.Mvc;
using FBG.Market.Web.Identity.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;
using DevExpress.Web;
using DevExpress.Utils;
using FBG.Market.Web.Identity.ViewModel;
using AutoMapper;

namespace FBG.Market.Web.Identity.Controllers
{
    public class ImportController : Controller
    {
        FBGMarketEntities db = new FBGMarketEntities();
        Utils Utils = new Utils();
        [Authorize]
        // GET: Import
        public ActionResult Index()
        {
            return View();
        }

        // This action method sends a PDF document with the exported Grid to response.
        public ActionResult ExportTo(string selectedIDsHF)
        {
            if (string.IsNullOrEmpty(selectedIDsHF))
            {
                //var modelDb = db.Products;
                //var modelDBList = modelDb.ToList();
                //return RedirectToAction("Index");
                return GridViewExtension.ExportToXlsx(GetGridSettings(), new List<ProductsImport>());
            }
            else
            {
                //var model = db.ProductsImports;
                var tokens = selectedIDsHF.Split(',');
                var modelDb = db.ProductsImports.Where(item => tokens.ToList().Contains(item.ID.ToString())).ToList();


                // New science

                // Automapper brilliance
                var config = new MapperConfiguration(cfg => cfg.CreateMap<ProductsImport, ProductImportViewModel>());
                var mapper = config.CreateMapper();

                // Load Related Data
                List<Brand> Brands = db.Brands.ToList();
                List<RefNRFColorCode> RefNRFColorCodes = db.RefNRFColorCodes.ToList();
                List<RefSeason> RefSeasons = db.RefSeasons.ToList();
                List<ProductStatu> ProductStatus = db.ProductStatus.ToList();
                List<Vendor> Vendors = db.Vendors.ToList();
                List<RefCategory> RefCategories = db.RefCategories.ToList();
                List<ColorCategory> ColorCategories = db.ColorCategories.ToList();

                List<ProductImportViewModel> productsVM = new List<ProductImportViewModel>();
                foreach (var product in modelDb)
                {
                    var productImportVM = mapper.Map<ProductsImport, ProductImportViewModel>(product);

                    productImportVM.BID = Brands.FirstOrDefault(item => item.BID == product.BID)?.BrandName;

                    var colorDetails = RefNRFColorCodes.FirstOrDefault(item => item.NRFColorCodeID == product.NRFColorCodeID);

                    productImportVM.NRFColorCodeID =
                        ColorCategories.FirstOrDefault(item => item.ColorCategoryID == colorDetails.ColorCategoryID)?.ColorCategoryName + " " +
                        colorDetails?.NRFColorCode + " " +
                        colorDetails?.NRFColorName;

                    productImportVM.SID = RefSeasons.FirstOrDefault(item => item.SID == product.SID)?.Season;
                    productImportVM.ProductStatusId = ProductStatus.FirstOrDefault(item => item.Id == product.ProductStatusId)?.Status;
                    productImportVM.VID = Vendors.FirstOrDefault(item => item.VID == product.VID)?.VendorName;
                    productImportVM.PCategory = RefCategories.FirstOrDefault(item => item.ID == product.PCategory)?.Category;
                    productImportVM.BrandCategoryId = Brands.FirstOrDefault(item => item.BID == product.BID)?.BrandName + "_"
                        + db.RefCategories.FirstOrDefault(s => s.ID == product.PCategory)?.Category;

                    productsVM.Add(productImportVM);
                }

                return GridViewExtension.ExportToXlsx(GetGridSettings(), productsVM.ToList());

                // End New Science
            }
        }
        // Returns the settings of the exported GridView.
        private GridViewSettings GetGridSettings()
        {
            var gridViewSettings = new GridViewSettings();
            gridViewSettings.Name = "productsimport_L";
            gridViewSettings.CallbackRouteValues = new { Controller = "Import", Action = "GridViewPartial" };

            // Export-specific settings 
            gridViewSettings.SettingsExport.ExportSelectedRowsOnly = false;
            gridViewSettings.SettingsExport.FileName = "imported_products.xlsx";
            gridViewSettings.Columns.Add(col =>
            {
                col.Visible = false;
                col.FieldName = "ID";
                //col.Caption = "Product Id";
                col.Width = System.Web.UI.WebControls.Unit.Pixel(80);
            });
            gridViewSettings.Columns.Add(col =>
            {
                col.Visible = true;
                col.FieldName = "PID";
                //col.Caption = "Product Id";
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
                    p.TextField = "Name";
                    p.ValueField = "Id";
                    p.ValueType = typeof(int);
                    p.DataSource = Utils.GetVendors();
                });
            });

            gridViewSettings.Columns.Add(c =>
            {

                //c.GroupIndex = 0;
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
                c.FieldName = "BID";
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

                //col.Visible = false;
                //col.Settings.AllowAutoFilter = DefaultBoolean.False;
                col.FieldName = "PSubCategory";
                col.Caption = "Sub Category";

            });
            gridViewSettings.Columns.Add(col =>
            {
                col.Settings.AutoFilterCondition = AutoFilterCondition.Contains;

                //col.Visible = false;
                //col.Settings.AllowAutoFilter = DefaultBoolean.False;
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
                //col.Visible = false;
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

            //gridViewSettings.Columns.Add("UPCCode").Caption = "UPC Code";
            gridViewSettings.Columns.Add(columnSettings =>
            {
                columnSettings.Settings.AutoFilterCondition = AutoFilterCondition.Contains;
                columnSettings.FieldName = "UPCCode";
                columnSettings.Caption = "UPC Code";
                columnSettings.Width = System.Web.UI.WebControls.Unit.Pixel(100);
                /*columnSettings.SetDataItemTemplateContent(c =>
                {
                    Html.DevExpress().HyperLink(hyperLinkSettings =>
                    {
                        hyperLinkSettings.Properties.Text = (string)DataBinder.Eval(c.DataItem, "UPCCode");
                        hyperLinkSettings.NavigateUrl = Url.Action("ExternalEditFormEdit", "Product", new { ProductID = DataBinder.Eval(c.DataItem, "PID") });
                    }).Render();
                });*/

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

                //col.Visible = false;
                //col.Settings.AllowAutoFilter = DefaultBoolean.False;
                col.FieldName = "PSize";
                col.Caption = "Product Size";

            });

            //PCoutryofOrigin
            gridViewSettings.Columns.Add(col =>
            {
                col.Settings.AutoFilterCondition = AutoFilterCondition.Contains;


                //col.Visible = false;
                //col.Settings.AllowAutoFilter = DefaultBoolean.False;
                col.FieldName = "PCoutryofOrigin";
                col.Caption = "Country of Origin";

            });

            //
            //if (roleBasedVisible)
            {
                gridViewSettings.Columns.Add(col =>
                {
                    col.Settings.AutoFilterCondition = AutoFilterCondition.Contains;

                    //col.Visible = roleBasedVisible;
                    //col.Settings.AllowAutoFilter = DefaultBoolean.False;
                    col.FieldName = "PFOBCost";
                    col.Caption = "FOB Cost";
                    col.PropertiesEdit.DisplayFormatString = "{0:C}";
                });
            }
            //if (roleBasedVisible)
            {
                gridViewSettings.Columns.Add(col =>
                {
                    col.Settings.AutoFilterCondition = AutoFilterCondition.Contains;

                    //col.Visible = roleBasedVisible;
                    //col.Settings.AllowAutoFilter = DefaultBoolean.False;
                    col.FieldName = "PLandedCost";
                    col.Caption = "Landed Cost";
                    col.PropertiesEdit.DisplayFormatString = "{0:C}";
                });
            }
            //if (roleBasedVisible)
            {
                gridViewSettings.Columns.Add(col =>
                {
                    col.Settings.AutoFilterCondition = AutoFilterCondition.Contains;

                    //col.Visible = roleBasedVisible;
                    //col.Settings.AllowAutoFilter = DefaultBoolean.False;
                    col.FieldName = "PWholesalePrice";
                    col.Caption = "Wholesale Price";
                    col.PropertiesEdit.DisplayFormatString = "{0:C}";

                });
            }
            //if (roleBasedVisible)
            {
                gridViewSettings.Columns.Add(col =>
                {
                    //col.Visible = roleBasedVisible;
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
                //c.Settings.FilterMode = ColumnFilterMode.DisplayText;
            });
            gridViewSettings.Columns.Add(col =>
            {
                //col.Visible = false;
                col.Settings.AutoFilterCondition = AutoFilterCondition.Contains;

                //col.Settings.AllowAutoFilter = DefaultBoolean.False;
                //col.FieldName = "ShopifyPicUrl";
                col.FieldName = "PPicture";
                col.Caption = "Picture";

            });
            //PDescription
            gridViewSettings.Columns.Add(col =>
            {
                col.Settings.AutoFilterCondition = AutoFilterCondition.Contains;

                //col.Visible = false;
                //col.Settings.AllowAutoFilter = DefaultBoolean.False;
                col.FieldName = "PDescription";
                col.Caption = "Description";

            });
            //PSpecs
            gridViewSettings.Columns.Add(col =>
            {
                col.Settings.AutoFilterCondition = AutoFilterCondition.Contains;

                //col.Visible = false;
                //col.Settings.AllowAutoFilter = DefaultBoolean.False;
                col.FieldName = "PSpecs";
                col.Caption = "Specs";

            });
            gridViewSettings.SettingsExport.BeforeExport = (sender, e) =>
            {
                MVCxGridView gridView = sender as MVCxGridView;
            };

            return gridViewSettings;
        }

        private void DeleteProductImport(int Id)
        {
            var model = db.ProductsImports;
            if (Id >= 0)
            {
                try
                {
                    var item = model.FirstOrDefault(it => it.ID == Id);
                    if (item != null)
                    {
                        model.Remove(item);
                        db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
        }
        [Authorize]
        public ActionResult Delete(string selectedIDsHF)
        {
            if (string.IsNullOrEmpty(selectedIDsHF))
            {
                var modelDb = db.ProductsImports;
                var modelDBList = modelDb.ToList();
                return PartialView("Index", modelDBList);
            }
            else
            {
                var tokens = selectedIDsHF.Split(',');
                var modelDb = db.ProductsImports.Where(item => tokens.ToList().Contains(item.ID.ToString())).ToList();
                foreach (var prod in modelDb.ToList())
                {
                    DeleteProductImport(prod.ID);
                }
                return Json(new { success = true, message = "Product(s) deleted successfully", status = 200 });
            }
        }
        [Authorize]
        public ActionResult GroupProducts(string selectedIDsHFToGroup)
        {
            if (string.IsNullOrEmpty(selectedIDsHFToGroup))
            {
                return Json(new { success = false, message = "No product selected", status = 500 });
            }
            else
            {
                return Json(new { success = true, message = "Product(s) grouped successfully", status = 200 });

            }
        }

        private List<string> ValidateImportRecord(ProductsImport model)
        {
            List<string> errors = new List<string>();

            if (string.IsNullOrEmpty(model.PColor) || model.VID is null || model.VID == 0 || model.BID is null || model.BID == 0 || model.PCategory is null
                || model.PCategory == 0 || string.IsNullOrEmpty(model.PName) || string.IsNullOrEmpty(model.PColor))
            {
                errors.Add("Please fix data issue in the selected row(s) before proceeding with publish.");
            }

            // Check Numberic values for the MSRP, FOB, Landed and Wholesale Prices
            decimal decimalValue;
            bool isDecimal = Decimal.TryParse(model.PWholesalePrice.Replace("$", ""), out decimalValue);
            if (!isDecimal)
            {
                errors.Add("Please enter valid whole sale price.");
            }
            isDecimal = Decimal.TryParse(model.PMSRPPrice.Replace("$", ""), out decimalValue);
            if (!isDecimal)
            {
                errors.Add("Please enter valid MSRP price.");
            }
            isDecimal = Decimal.TryParse(model.PFOBCost.Replace("$", ""), out decimalValue);
            if (!isDecimal)
            {
                errors.Add("Please enter valid FOB cost.");
            }
            isDecimal = Decimal.TryParse(model.PLandedCost.Replace("$", ""), out decimalValue);
            if (!isDecimal)
            {
                errors.Add("Please enter valid landed Cost price.");
            }

            // DB existing Data Check
            if (!string.IsNullOrEmpty(model.SKUCode))
            {
                var skuRetVal = db.Products.FirstOrDefault(item => item.SKUCode.ToLower() == model.SKUCode.ToLower());
                if (skuRetVal != null)
                {
                    errors.Add("SKU Code already exists.");
                }
            }

            if (!string.IsNullOrEmpty(model.UPCCode))
            {
                var upcRetVal = db.Products.FirstOrDefault(item => item.UPCCode.ToLower() == model.UPCCode.ToLower());
                if (upcRetVal != null)
                {
                    errors.Add("UPC Code already exists.");
                }
            }

            if (model.ProductStatusId == 2 || model.ProductStatusId == 3)
            {
                if (string.IsNullOrEmpty(model.SKUCode) || string.IsNullOrEmpty(model.UPCCode))
                {
                    errors.Add("SKU code, UPC code are required for Final-wholesale & Final-consumer.");
                }
            }

            return errors;
        }

        [Authorize]
        public ActionResult Publish(string selectedIDsHFPublish)
        {
            if (string.IsNullOrEmpty(selectedIDsHFPublish))
            {
                return Json(new { success = false, message = "No product selected", status = 500 });
            }
            else
            {
                var prodStatusId = 1;
                var prodStatus = db.ProductStatus.FirstOrDefault(item => item.Status.ToLower() == "draft");
                if (prodStatus != null)
                    prodStatusId = prodStatus.Id;
                var tokens = selectedIDsHFPublish.Split(',');

                var productsImport = db.ProductsImports.Where(item => tokens.ToList().Contains(item.ID.ToString())).ToList();
                foreach (var import in productsImport)
                {
                    var validationErrors = ValidateImportRecord(import);
                    if (validationErrors.Count() > 0 || !ModelState.IsValid)
                    {
                        var errorMessage = "";
                        foreach (var item in validationErrors)
                        {
                            errorMessage += item + "\n";
                        }
                        return Json(new { success = false, message = errorMessage, status = 500 });
                    }

                    //x.SKUCode, x.UPCCode, x.BID, x.VID, x.PCategory, x.PColor, x.PName
                    var retVal = db.Products.Where(pro => pro.UPCCode.ToLower() == import.UPCCode.ToLower() &&
                    pro.SKUCode.ToLower() == import.SKUCode.ToLower() &&
                    pro.BID == import.BID &&
                    pro.VID == import.VID &&
                     pro.PCategory == import.PCategory &&
                     pro.PColor.ToLower() == import.PColor.ToLower() &&
                     pro.PName.ToLower() == import.PName.ToLower()
                     ).FirstOrDefault();
                    //var retVal = db.Products.Where(pro => pro.UPCCode.ToLower() == prod.UPCCode.ToLower() || pro.SKUCode.ToLower() == prod.SKUCode.ToLower()).FirstOrDefault();
                    //Product does not exists in the product DB, insert it
                    var skuOrUpc = db.Products.Where(pro => pro.UPCCode.ToLower() == import.UPCCode.ToLower() ||
                   pro.SKUCode.ToLower() == import.SKUCode.ToLower()).FirstOrDefault();
                    if (retVal == null && skuOrUpc == null)
                    {
                        var products = db.Products;
                        Product product = new Product
                        {
                            //PID = Convert.ToInt32(prod.PID),
                            PName = import.PName,
                            PPicture = import.PPicture,
                            SKUCode = import.SKUCode,
                            UPCCode = import.UPCCode,
                            BID = import.BID,
                            SID = import.SID,
                            VendorName = import.VendorName,
                            PCategory = Convert.ToInt16(import.PCategory),
                            PSubCategory = import.PSubCategory,
                            NRFColorCodeID = import.NRFColorCodeID,
                            PCoutryofOrigin = import.PCoutryofOrigin,
                            PDescription = import.PDescription,
                            VID = import.VID,
                            PSize = import.PSize,
                            PFOBCost = Convert.ToDecimal(import.PFOBCost.Replace("$", "")),
                            PLandedCost = Convert.ToDecimal(import.PLandedCost.Replace("$", "")),
                            PWholesalePrice = Convert.ToDecimal(import.PWholesalePrice.Replace("$", "")),
                            PMSRPPrice = Convert.ToDecimal(import.PMSRPPrice.Replace("$", "")),
                            //ProductStatusId = prod.ProductStatusId,
                            ProductStatusId = prodStatusId, // Draft for all
                            PColor = import.PColor,
                            PSpecs = import.PSpecs,
                            CreateDate = DateTime.UtcNow,
                        };
                        try
                        {
                            Utils.CreateBrandProdDir(product);
                            products.Add(product);
                            db.SaveChanges();
                            DeleteProductImport(import.ID);
                        }
                        catch (Exception ex)
                        {
                            return Json(new { success = false, message = ex.Message, status = 500 });
                        }
                    }
                    //Update the existing product with PID
                    else
                    {
                        Product product = new Product
                        {
                            PID = Convert.ToInt32(retVal.PID),
                            PName = import.PName,
                            PPicture = import.PPicture,
                            SKUCode = import.SKUCode,
                            UPCCode = import.UPCCode,
                            BID = import.BID,
                            VID = import.VID,
                            SID = import.SID,
                            VendorName = import.VendorName,
                            PCategory = Convert.ToInt16(import.PCategory),
                            PSubCategory = import.PSubCategory,
                            NRFColorCodeID = import.NRFColorCodeID,
                            PCoutryofOrigin = import.PCoutryofOrigin,
                            PSize = import.PSize,
                            PFOBCost = Convert.ToDecimal(import.PFOBCost),
                            PLandedCost = Convert.ToDecimal(import.PLandedCost),
                            PWholesalePrice = Convert.ToDecimal(import.PWholesalePrice),
                            PMSRPPrice = Convert.ToDecimal(import.PMSRPPrice),
                            //ProductStatusId = prod.ProductStatusId,
                            ProductStatusId = prodStatusId, // Draft for all
                            PColor = import.PColor,
                            PDescription = import.PDescription,
                            PSpecs = import.PSpecs,
                        };
                        var retValU = UpdateProduct(product);
                        if (retValU)
                        {
                            DeleteProductImport(import.ID);
                        }

                    }
                }
                return Json(new { success = true, message = "Product(s) published successfully", status = 200 });
            }
        }
        [Authorize]
        private bool UpdateProduct(Product product)
        {
            var productsImagePath = System.Web.HttpContext.Current.Request.Params["products-path"];
            //product.PID = Convert.ToInt32(System.Web.HttpContext.Current.Request.Params["PID"]);
            var model = db.Products;
            //var temp = model.ToList();
            {
                try
                {
                    var modelItem = model.FirstOrDefault(it => it.PID == product.PID);
                    if (modelItem != null)
                    {
                        modelItem.BID = product.BID;

                        modelItem.PCategory = product.PCategory;
                        modelItem.PSubCategory = product.PSubCategory;
                        modelItem.PSize = product.PSize;
                        modelItem.PSpecs = product.PSpecs;
                        modelItem.NRFColorCodeID = product.NRFColorCodeID;
                        modelItem.PColor = product.PColor;
                        modelItem.VendorName = product.VendorName;

                        modelItem.PMSRPPrice = product.PMSRPPrice;

                        modelItem.NRFColorCodeID = product.NRFColorCodeID;
                        modelItem.PCoutryofOrigin = product.PCoutryofOrigin;
                        modelItem.SID = product.SID;
                        modelItem.NRFColorCodeID = product.NRFColorCodeID;
                        modelItem.PCoutryofOrigin = product.PCoutryofOrigin;

                        modelItem.PDescription = product.PDescription;
                        //modelItem.PFID = product.PFID;
                        modelItem.PFOBCost = product.PFOBCost;
                        modelItem.PName = product.PName;
                        //modelItem.PPicture = productsImagePath;
                        modelItem.PLandedCost = product.PLandedCost;
                        modelItem.SKUCode = product.SKUCode;
                        modelItem.UPCCode = product.UPCCode;
                        modelItem.PWholesalePrice = product.PWholesalePrice;
                        modelItem.ProductStatusId = product.ProductStatusId;
                        modelItem.UpdatedDate = DateTime.UtcNow;
                        this.TryUpdateModel(modelItem);
                        db.SaveChanges();

                    }
                    else
                        return false;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                    return false;

                }
            }
            return true;
        }
        private bool GridColumnExists(DataTable myDataTable, string columnName)
        {
            if (myDataTable.Columns.Contains(columnName))
                return true;
            else
                return false;
        }
        private void BulkImport(DataTable myDataTable)
        {
            var listOfColumns = new List<string>(){
            "BrandCategoryId",
            "BID",
            "VID",
            "VendorName",
            "SKUCode",
            "UPCCode",
            "PName",
            "PColor",
            "PCategory",
            "NRFColorCodeID",
            "SID",
            "ProductStatusId",
            "PPicture",
            "PSize",
            "PSubCategory",
            "PCoutryofOrigin",
            "PFOBCost",
            "PLandedCost",
            "PWholesalePrice",
            "PMSRPPrice",
            "PDescription",
            "PSpecs"};


            string connectionString = db.Database.Connection.ConnectionString;
            try
            {
                db.Database.ExecuteSqlCommand("TRUNCATE TABLE dbo.[ProductsImport]");
            }
            catch (Exception ex) { }

            // Load Related Data
            List<Brand> Brands = db.Brands.ToList();
            List<RefNRFColorCode> RefNRFColorCodes = db.RefNRFColorCodes.ToList();
            List<RefSeason> RefSeasons = db.RefSeasons.ToList();
            List<ProductStatu> ProductStatus = db.ProductStatus.ToList();
            List<Vendor> Vendors = db.Vendors.ToList();
            List<RefCategory> RefCategories = db.RefCategories.ToList();

            foreach (DataRow row in myDataTable.Rows)
            {

                try
                {

                    // Create Product
                    ProductsImport productsImport = new ProductsImport();

                    // String Fields
                    if (GridColumnExists(myDataTable, "Color"))
                        productsImport.PColor = row["Color"].ToString();

                    if (GridColumnExists(myDataTable, "SKU Code"))
                        productsImport.SKUCode = row["SKU Code"].ToString();

                    if (GridColumnExists(myDataTable, "UPC Code"))
                        productsImport.UPCCode = row["UPC Code"].ToString();

                    if (GridColumnExists(myDataTable, "Style"))
                        productsImport.PName = row["Style"].ToString();

                    if (GridColumnExists(myDataTable, "Color"))
                        productsImport.PColor = row["Color"].ToString();

                    if (GridColumnExists(myDataTable, "Picture"))
                        productsImport.PPicture = row["Picture"].ToString();

                    if (GridColumnExists(myDataTable, "Product Size"))
                        productsImport.PSize = row["Product Size"].ToString();

                    if (GridColumnExists(myDataTable, "Vendor Style #"))
                        productsImport.VendorName = row["Vendor Style #"].ToString();

                    if (GridColumnExists(myDataTable, "Sub Category"))
                        productsImport.PSubCategory = row["Sub Category"].ToString();

                    if (GridColumnExists(myDataTable, "Country of Origin"))
                        productsImport.PCoutryofOrigin = row["Country of Origin"].ToString();

                    if (GridColumnExists(myDataTable, "FOB Cost"))
                        productsImport.PFOBCost = row["FOB Cost"].ToString();

                    if (GridColumnExists(myDataTable, "Landed Cost"))
                        productsImport.PLandedCost = row["Landed Cost"].ToString();

                    if (GridColumnExists(myDataTable, "MSRP"))
                        productsImport.PMSRPPrice = row["MSRP"].ToString();

                    if (GridColumnExists(myDataTable, "Wholesale Price"))
                        productsImport.PWholesalePrice = row["Wholesale Price"].ToString();

                    if (GridColumnExists(myDataTable, "Description"))
                        productsImport.PDescription = row["Description"].ToString();


                    // Navigation fields
                    if (GridColumnExists(myDataTable, "Brand"))
                    {
                        try
                        {
                            var retVal = Brands.FirstOrDefault(item => item.BrandName == row["Brand"]?.ToString());
                            if (retVal != null)
                                productsImport.BID = retVal.BID;
                        }
                        catch (Exception)
                        {
                            productsImport.BID = null;
                        }
                    }

                    if (GridColumnExists(myDataTable, "NRF Color"))
                    {
                        try
                        {
                            var retVal = RefNRFColorCodes.FirstOrDefault(item => item.NRFColorName == row["NRF Color"]?.ToString());
                            if (retVal != null)
                                productsImport.NRFColorCodeID = retVal.NRFColorCodeID;
                        }
                        catch (Exception)
                        {
                            productsImport.NRFColorCodeID = null;
                        }
                    }

                    if (GridColumnExists(myDataTable, "Season"))
                    {
                        try
                        {
                            var retVal = RefSeasons.FirstOrDefault(item => item.Season == row["Season"]?.ToString());
                            if (retVal != null)
                                productsImport.SID = retVal.SID;
                        }
                        catch (Exception)
                        {
                            productsImport.SID = null;
                        }
                    }

                    if (GridColumnExists(myDataTable, "Status"))
                    {
                        try
                        {
                            var retVal = ProductStatus.FirstOrDefault(item => item.Status == row["Status"]?.ToString());
                            if (retVal != null)
                                productsImport.ProductStatusId = retVal.Id;
                        }
                        catch (Exception)
                        {
                            productsImport.ProductStatusId = null;
                        }
                    }

                    if (GridColumnExists(myDataTable, "Vendor"))
                    {
                        try
                        {
                            var retVal = Vendors.FirstOrDefault(item => item.VendorName == row["Vendor"]?.ToString());
                            if (retVal != null)
                                productsImport.VID = retVal.VID;
                        }
                        catch (Exception)
                        {
                            productsImport.VID = null;
                        }
                    }

                    if (GridColumnExists(myDataTable, "Category"))
                    {
                        try
                        {
                            var retVal = RefCategories.FirstOrDefault(item => item.Category == row["Category"]?.ToString());
                            if (retVal != null)
                                productsImport.PCategory = (short)retVal.ID;
                        }
                        catch (Exception)
                        {
                            productsImport.PCategory = null;
                        }
                    }

                    //var config = new MapperConfiguration(cfg => cfg.CreateMap<ProductImportViewModel, ProductsImport>());
                    //var mapper = config.CreateMapper();
                    //var productImport = mapper.Map<ProductImportViewModel, ProductsImport>(productsImportVM);

                    db.ProductsImports.Add(productsImport);

                }
                catch (Exception ex)
                {
                    // Todo : Log and then skip and move on to the next Product
                    continue;
                }

            }

            db.SaveChanges();

        }

        [Authorize]
        [ValidateInput(false)]
        public ActionResult GridViewPartial()
        {
            return ImportGridViewPartial("");
        }

        [Authorize]
        [HttpPost, ValidateInput(false)]
        public ActionResult GridViewPartialAddNew(ProductsImport item)
        {
            var model = db.ProductsImports;
            if (ModelState.IsValid)
            {
                try
                {
                    model.Add(item);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("_GridViewPartial", model.ToList());
        }
        [Authorize]
        [HttpPost, ValidateInput(false)]
        public ActionResult GridViewPartialUpdate(Models.ProductsImport item)
        {
            var model = db.ProductsImports;
            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = model.FirstOrDefault(it => it.PID == item.PID);
                    if (modelItem != null)
                    {
                        this.UpdateModel(modelItem);
                        db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("_GridViewPartial", model.ToList());
        }
        [Authorize]
        [HttpPost, ValidateInput(false)]
        public ActionResult GridViewPartialDelete(System.Int32 PID)
        {
            var model = db.ProductsImports;
            if (PID >= 0)
            {
                try
                {
                    var item = model.FirstOrDefault(it => Convert.ToInt32(it.PID) == PID);
                    if (item != null)
                        model.Remove(item);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            return PartialView("_GridViewPartial", model.ToList());
        }
        [Authorize]
        public ActionResult UploadControlUpload()
        {
            UploadControlExtension.GetUploadedFiles("UploadControlFile", ImportControllerUploadControlSettings.UploadValidationSettings, ImportControllerUploadControlSettings.FileUploadComplete);
            return null;
        }

        [Authorize]
        [ValidateInput(false)]
        public ActionResult ImportGridViewPartial(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var model = InMemoryModel.OpenExcelFile(path);
                BulkImport((DataTable)model);
                Session["DataTableModel"] = model;
            }
            var products = db.ProductsImports.Select(product => new ImportViewModel
            {
                ID = product.ID,
                BID = product.BID,
                ColorCategoryId = product.ColorCategoryId,
                NRFColorCodeID = product.NRFColorCodeID,
                PCategory = product.PCategory,
                PColor = product.PColor,
                PCoutryofOrigin = product.PCoutryofOrigin,
                PDescription = product.PDescription,
                PDiscontinued = false,
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
                UPCCode = product.UPCCode,
                VID = product.VID,
                Brand_Category = product.Brand_Category,
                VendorName = product.VendorName,
                GroupId = product.GroupId
            }).ToList();

            var dupes = products.GroupBy(x => new { x.SKUCode, x.UPCCode, x.BID, x.VID, x.PCategory, x.PColor, x.PName })
                   .Where(x => x.Skip(1).Any());
            var hasDupes = products.GroupBy(x => new { x.SKUCode, x.UPCCode, x.BID, x.VID, x.PCategory, x.PColor, x.PName })
                   .Where(x => x.Skip(1).Any()).Any();
            if (dupes != null && dupes.Any())
            {
                foreach (var dup in dupes)
                {
                    products = products.Select(n =>
                    {
                        if (n.SKUCode == dup.Key.SKUCode && n.UPCCode == dup.Key.UPCCode
                            && n.BID == dup.Key.BID && n.VID == dup.Key.VID && n.PColor == dup.Key.PColor
                            && n.PName == dup.Key.PName)
                        {
                            n.PDiscontinued = true;
                        }
                        return n;
                    }).ToList();
                }
            }

            return PartialView("_ImportGridViewPartial", products);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ImportGridViewPartialAddNew(FBG.Market.Web.Identity.Models.ProductsImport item)
        {
            var model = db.ProductsImports;
            if (ModelState.IsValid)
            {
                try
                {
                    model.Add(item);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("_ImportGridViewPartial", model.ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ImportGridViewPartialUpdate(FBG.Market.Web.Identity.Models.ProductsImport item)
        {
            var model = db.ProductsImports;
            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = model.FirstOrDefault(it => it.ID == item.ID);
                    if (modelItem != null)
                    {
                        this.UpdateModel(modelItem);
                        db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("_ImportGridViewPartial", model.ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ImportGridViewPartialDelete(System.Int32 ID)
        {
            var model = db.ProductsImports;
            if (ID >= 0)
            {
                try
                {
                    var item = model.FirstOrDefault(it => it.ID == ID);
                    if (item != null)
                        model.Remove(item);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            return PartialView("_ImportGridViewPartial", model.ToList());
        }

        [Authorize]
        private void UpdateProductImport(ProductsImport product)
        {
            var productsImagePath = System.Web.HttpContext.Current.Request.Params["products-path"];
            var model = db.ProductsImports;
            {
                try
                {
                    var modelItem = model.FirstOrDefault(it => it.ID == product.ID);
                    if (modelItem != null)
                    {
                        modelItem.PID = product.PID;
                        modelItem.BID = product.BID;
                        modelItem.VID = product.VID;
                        modelItem.PCategory = product.PCategory;
                        modelItem.PSubCategory = product.PSubCategory;
                        modelItem.VendorName = product.VendorName;
                        modelItem.NRFColorCodeID = product.NRFColorCodeID;
                        modelItem.PCoutryofOrigin = product.PCoutryofOrigin;
                        modelItem.PDescription = product.PDescription;
                        modelItem.PSpecs = product.PSpecs;

                        //modelItem.PFID = product.PFID;
                        modelItem.PFOBCost = product.PFOBCost;
                        modelItem.PName = product.PName;
                        modelItem.PColor = product.PColor;
                        modelItem.PSize = product.PSize;

                        modelItem.SID = product.SID;

                        //modelItem.PPicture = productsImagePath;
                        modelItem.PLandedCost = product.PLandedCost;
                        modelItem.SKUCode = product.SKUCode;
                        modelItem.UPCCode = product.UPCCode;
                        modelItem.PWholesalePrice = product.PWholesalePrice;
                        modelItem.PMSRPPrice = product.PMSRPPrice;

                        modelItem.ProductStatusId = product.ProductStatusId;
                        this.TryUpdateModel(modelItem);
                        db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
        }
        [Authorize]
        [ValidateInput(false)]
        public ActionResult BatchEditingUpdateModel(MVCxGridViewBatchUpdateValues<ProductsImport, int> updateValues)
        {
            foreach (var product in updateValues.Insert)
            {
                if (updateValues.IsValid(product))
                    //InsertProduct(product, updateValues);
                    UpdateProductImport(product);
            }
            foreach (var product in updateValues.Update)
            {
                if (updateValues.IsValid(product))
                    UpdateProductImport(product);
                //UpdateProduct(product, updateValues);
            }

            return PartialView("_ImportGridViewPartial", db.ProductsImports.ToList());
        }
    }
    public class ImportControllerUploadControlSettings
    {
        public const string UploadDirectory = "~/brands/brand-1/prod-1/";

        public static DevExpress.Web.UploadControlValidationSettings UploadValidationSettings = new DevExpress.Web.UploadControlValidationSettings()
        {
            AllowedFileExtensions = new string[] { ".xlsx", ".xls" },
            MaxFileSize = 4194304
        };
        public static void FileUploadComplete(object sender, DevExpress.Web.FileUploadCompleteEventArgs e)
        {
            const string UploadDirectory = "~/Content/UploadedFiles/";
            if (e.UploadedFile.IsValid)
            {
                // Save uploaded file to some location
                MemoryStream ms = new MemoryStream();
                string resultExtension = Path.GetExtension(e.UploadedFile.FileName);
                string resultFileName = Path.ChangeExtension(Path.GetRandomFileName(), resultExtension);
                string resultFileUrl = UploadDirectory + resultFileName;
                string resultFilePath = System.Web.HttpContext.Current.Server.MapPath("~/") + e.UploadedFile.FileName;
                e.UploadedFile.SaveAs(resultFilePath);
                e.CallbackData = resultFilePath;
            }
        }
    }
}