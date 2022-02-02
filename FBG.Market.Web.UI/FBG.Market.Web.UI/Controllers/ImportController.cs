using DevExpress.Web.Internal;
using DevExpress.Web.Mvc;
using FBG.Market.Web.Identity.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using DevExpress.Web;
using DevExpress.Utils;

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
            //if (Session["DataTableModel"] == null)
            //    Session["DataTableModel"] = InMemoryModel.OpenExcelFile("");
            var modelDb = db.ProductsImports;
            var modelDBList = modelDb.ToList();
            modelDBList.ForEach(item => item.PDiscontinued = false);
            var dupes = modelDBList.GroupBy(x => new { x.SKUCode, x.UPCCode, x.BID, x.VID, x.PCategory, x.PColor, x.PName })
                   .Where(x => x.Skip(1).Any());
            var hasDupes = modelDBList.GroupBy(x => new { x.SKUCode, x.UPCCode, x.BID, x.VID, x.PCategory, x.PColor, x.PName })
                   .Where(x => x.Skip(1).Any()).Any();
            if (dupes!=null && dupes.Any())
            {
                foreach (var dup in dupes)
                {
                    modelDBList = modelDBList.Select(n => {
                        if (n.SKUCode == dup.Key.SKUCode
&& n.UPCCode == dup.Key.UPCCode
&& n.BID == dup.Key.BID
&& n.VID == dup.Key.VID
&& n.PColor == dup.Key.PColor
&& n.PName == dup.Key.PName) { n.PDiscontinued = true; }
                        return n;
                    }).ToList();
                }
            }
            return PartialView(modelDBList);
        }

        // This action method sends a PDF document with the exported Grid to response.
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
                //var model = db.ProductsImports;
                var tokens = selectedIDsHF.Split(',');
                var modelDb = db.ProductsImports.Where(item => tokens.ToList().Contains(item.ID.ToString())).ToList();
                return GridViewExtension.ExportToXlsx(GetGridSettings(), modelDb.ToList());
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
                    p.TextField = "VendorName";
                    p.ValueField = "VendorId";
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
                /*columnSettings.SetDataItemTemplateContent(c =>
                {
                    Html.DevExpress().HyperLink(hyperLinkSettings =>
                    {
                        hyperLinkSettings.Properties.Text = (string)DataBinder.Eval(c.DataItem, "SKUCode");
                        hyperLinkSettings.NavigateUrl = Url.Action("ExternalEditFormEdit", "Product", new { ProductID = DataBinder.Eval(c.DataItem, "PID") });

                    }).Render()
                });*/

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
                //VID
                //gridView.Columns["VID"].Visible = true;

                //ID
                //gridView.Columns["ID"].Visible = false;
                //PPicture
                //gridView.Columns["Message"].Visible = false;

            };
            //settings.SettingsExport.PaperKind = System.Drawing.Printing.PaperKind.A4;
            /*gridViewSettings.KeyFieldName = "BID";
            gridViewSettings.KeyFieldName = "PID";
            gridViewSettings.KeyFieldName = "SKUCode";
            gridViewSettings.KeyFieldName = "UPCCode";
            gridViewSettings.KeyFieldName = "PName";
            gridViewSettings.KeyFieldName = "VendorName";
            gridViewSettings.KeyFieldName = "PColor";
            gridViewSettings.KeyFieldName = "PSize";
            gridViewSettings.KeyFieldName = "PCategory";
            gridViewSettings.KeyFieldName = "PSubCategory";
            gridViewSettings.KeyFieldName = "NRFColorCodeID";
            gridViewSettings.KeyFieldName = "SID";
            gridViewSettings.KeyFieldName = "PDescription";
            gridViewSettings.KeyFieldName = "PSpecs";
            gridViewSettings.KeyFieldName = "PCoutryofOrigin";
            gridViewSettings.KeyFieldName = "PDiscontinued";
            gridViewSettings.KeyFieldName = "PFOBCost";
            gridViewSettings.KeyFieldName = "PLandedCost";
            gridViewSettings.KeyFieldName = "PWholesalePrice";
            gridViewSettings.KeyFieldName = "PMSRPPrice";
            gridViewSettings.KeyFieldName = "PPicture";
            gridViewSettings.KeyFieldName = "ProductStatusId";
            gridViewSettings.KeyFieldName = "VID";
            gridViewSettings.KeyFieldName = "ColorCategoryId";
            gridViewSettings.KeyFieldName = "ShopifyPicUrl";
            gridViewSettings.KeyFieldName = "ID";
            */
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
                        model.Remove(item);
                    db.SaveChanges();
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
                return PartialView("Index", modelDb.ToList());
            }
        }
        [Authorize]
        public ActionResult GroupProducts(string selectedIDsHFToGroup)
        {
            if (string.IsNullOrEmpty(selectedIDsHFToGroup))
            {
                //var modelDb = db.ProductsImports;
                //var modelDBList = modelDb.ToList();
                //return Content("<script language='javascript' type='text/javascript'>alert('Thanks for Feedback!');</script>");
                //return Content("No product selected");
                return Json(new { success = false, message = "No product selected", status = 500 });
            }
            else
            {
                return Json(new { success = true, message = "Product(s) grouped successfully", status = 200 });

            }
        }
                [Authorize]
        public ActionResult Publish(string selectedIDsHFPublish)
        {
            if (string.IsNullOrEmpty(selectedIDsHFPublish))
            {
                //var modelDb = db.ProductsImports;
                //var modelDBList = modelDb.ToList();
                //return Content("<script language='javascript' type='text/javascript'>alert('Thanks for Feedback!');</script>");
                //return Content("No product selected");
                return Json(new { success = false, message = "No product selected", status = 500  });
            }
            else
            {
                var prodStatusId = 1;
                var prodStatus = db.ProductStatus.FirstOrDefault(item => item.Status.ToLower() == "draft");
                if (prodStatus != null)
                    prodStatusId = prodStatus.Id;
                var tokens = selectedIDsHFPublish.Split(',');
                var modelDb = db.ProductsImports.Where(item => tokens.ToList().Contains(item.ID.ToString())).ToList();
                var modelDBList = modelDb.ToList();
                foreach (var prod in modelDBList)
                {
                    if (string.IsNullOrEmpty(prod.PColor) || prod.VID <= 0 || prod.BID <= 0 || prod.PCategory == null
                        || prod.PCategory <= 0 || string.IsNullOrEmpty(prod.PName) || string.IsNullOrEmpty(prod.PColor))
                        continue;
                    //check numberic values for the msrp, fob, landed and wholesale prices
                    decimal decimalValue;
                    bool isDecimal = Decimal.TryParse(prod.PWholesalePrice, out decimalValue);
                    if (!isDecimal)
                    {
                        continue;
                    }
                    isDecimal = Decimal.TryParse(prod.PMSRPPrice, out decimalValue);
                    if (!isDecimal)
                    {
                        continue;
                    }
                    isDecimal = Decimal.TryParse(prod.PFOBCost, out decimalValue);
                    if (!isDecimal)
                    {
                        continue;
                    }
                    isDecimal = Decimal.TryParse(prod.PLandedCost, out decimalValue);
                    if (!isDecimal)
                    {
                        continue;
                    }
                    //x.SKUCode, x.UPCCode, x.BID, x.VID, x.PCategory, x.PColor, x.PName
                    var retVal = db.Products.Where(pro => pro.UPCCode.ToLower() == prod.UPCCode.ToLower() &&
                    pro.SKUCode.ToLower() == prod.SKUCode.ToLower() &&
                    pro.BID == prod.BID &&
                    pro.VID == prod.VID &&
                     pro.PCategory == prod.PCategory &&
                     pro.PColor.ToLower() == prod.PColor.ToLower() &&
                     pro.PName.ToLower() == prod.PName.ToLower()
                     ).FirstOrDefault();
                    //var retVal = db.Products.Where(pro => pro.UPCCode.ToLower() == prod.UPCCode.ToLower() || pro.SKUCode.ToLower() == prod.SKUCode.ToLower()).FirstOrDefault();
                    //Product does not exists in the product DB, insert it
                    if (retVal==null)
                    {
                        //Insert a new product
                        var model = db.Products;
                        Product product = new Product
                        {
                            //PID = Convert.ToInt32(prod.PID),
                            PName = prod.PName,
                            PPicture = prod.PPicture,
                            SKUCode = prod.SKUCode,
                            UPCCode = prod.UPCCode,
                            BID = prod.BID,
                            SID = prod.SID,
                            VendorName=prod.VendorName,
                            PCategory = Convert.ToInt16(prod.PCategory),
                            PSubCategory = prod.PSubCategory,
                            NRFColorCodeID = prod.NRFColorCodeID,
                            PCoutryofOrigin = prod.PCoutryofOrigin,
                            PDescription = prod.PDescription,
                            VID = prod.VID,
                            PSize = prod.PSize,
                            PFOBCost = Convert.ToDecimal(prod.PFOBCost),
                            PLandedCost = Convert.ToDecimal(prod.PLandedCost),
                            PWholesalePrice = Convert.ToDecimal(prod.PWholesalePrice),
                            PMSRPPrice = Convert.ToDecimal(prod.PMSRPPrice),
                            //ProductStatusId = prod.ProductStatusId,
                            ProductStatusId = prodStatusId, // Draft for all
                            PColor = prod.PColor,
                            PSpecs = prod.PSpecs,
                            CreateDate = DateTime.UtcNow,
                        };
                        try
                        {
                            model.Add(product);
                            db.SaveChanges();
                            DeleteProductImport(prod.ID);
                        } catch (Exception ex)
                        {
                            return Json(new { success = false, message = ex.Message, status = 500 });
                            /*if (!string.IsNullOrEmpty(product.SKUCode) && !string.IsNullOrEmpty(product.UPCCode))
                            {
                                var pr = db.Products.Where(pro => pro.SKUCode == product.SKUCode).FirstOrDefault();
                                if (pr != null)
                                {
                                    product.PID = pr.PID;
                                    UpdateProduct(product);
                                }
                            }*/
                        }
                    }
                    //Update the existing product with PID
                    else
                    {
                        Product product = new Product
                        {
                            PID = Convert.ToInt32(retVal.PID),
                            PName = prod.PName,
                            PPicture = prod.PPicture,
                            SKUCode = prod.SKUCode,
                            UPCCode = prod.UPCCode,
                            BID = prod.BID,
                            VID = prod.VID,
                            SID = prod.SID,
                            VendorName = prod.VendorName,
                            PCategory = Convert.ToInt16(prod.PCategory),
                            PSubCategory = prod.PSubCategory,
                            NRFColorCodeID = prod.NRFColorCodeID,
                            PCoutryofOrigin = prod.PCoutryofOrigin,
                            PSize = prod.PSize,
                            PFOBCost = Convert.ToDecimal(prod.PFOBCost),
                            PLandedCost = Convert.ToDecimal(prod.PLandedCost),
                            PWholesalePrice = Convert.ToDecimal(prod.PWholesalePrice),
                            PMSRPPrice = Convert.ToDecimal(prod.PMSRPPrice),
                            //ProductStatusId = prod.ProductStatusId,
                            ProductStatusId = prodStatusId, // Draft for all
                            PColor = prod.PColor,
                            PDescription = prod.PDescription,
                            PSpecs = prod.PSpecs,
                        };
                        var retValU = UpdateProduct(product);
                        if (retValU)
                            DeleteProductImport(prod.ID);

                    }
                }
                return Json(new { success = true, message = "Product(s) published successfully", status = 200 });
                //return PartialView("Index", modelDBList);
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
                "Brand_Category",
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
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    if (GridColumnExists(myDataTable, "Color"))
                        myDataTable.Columns["Color"].ColumnName = "PColor";
                    if (GridColumnExists(myDataTable, "Brand"))
                        myDataTable.Columns["Brand"].ColumnName = "BID";
                    if (GridColumnExists(myDataTable, "Vendor"))
                        myDataTable.Columns["Vendor"].ColumnName = "VID";
                    //myDataTable.Columns["SKU Code"].ColumnName = "SKUCode";
                    if (GridColumnExists(myDataTable, "SKU Code"))
                        myDataTable.Columns["SKU Code"].ColumnName = "SKUCode";
                    if (GridColumnExists(myDataTable, "UPC Code"))
                        myDataTable.Columns["UPC Code"].ColumnName = "UPCCode";
                    if (GridColumnExists(myDataTable, "Style"))
                        myDataTable.Columns["Style"].ColumnName = "PName";
                    if (GridColumnExists(myDataTable, "Color"))
                        myDataTable.Columns["Color"].ColumnName = "PColor";

                    if (GridColumnExists(myDataTable, "NRF Color"))
                        myDataTable.Columns["NRF Color"].ColumnName = "NRFColorCodeID";

                    if (GridColumnExists(myDataTable, "Season"))

                        myDataTable.Columns["Season"].ColumnName = "SID";

                    if (GridColumnExists(myDataTable, "Status"))

                        myDataTable.Columns["Status"].ColumnName = "ProductStatusId";
                    if (GridColumnExists(myDataTable, "Picture"))

                        myDataTable.Columns["Picture"].ColumnName = "PPicture";
                    if (GridColumnExists(myDataTable, "Product Size"))

                        myDataTable.Columns["Product Size"].ColumnName = "PSize";

                    if (GridColumnExists(myDataTable, "Vendor Style #"))

                        myDataTable.Columns["Vendor Style #"].ColumnName = "VendorName";

                    if (GridColumnExists(myDataTable, "Category"))

                        myDataTable.Columns["Category"].ColumnName = "PCategory";

                    if (GridColumnExists(myDataTable, "Sub Category"))

                        myDataTable.Columns["Sub Category"].ColumnName = "PSubCategory";

                    if (GridColumnExists(myDataTable, "Country of Origin"))

                        myDataTable.Columns["Country of Origin"].ColumnName = "PCoutryofOrigin";

                    if (GridColumnExists(myDataTable, "FOB Cost"))

                        myDataTable.Columns["FOB Cost"].ColumnName = "PFOBCost";
                    if (GridColumnExists(myDataTable, "Landed Cost"))
                        myDataTable.Columns["Landed Cost"].ColumnName = "PLandedCost";

                    if (GridColumnExists(myDataTable, "MSRP"))

                        myDataTable.Columns["MSRP"].ColumnName = "PMSRPPrice";

                    if (GridColumnExists(myDataTable, "Wholesale Price"))

                        myDataTable.Columns["Wholesale Price"].ColumnName = "PWholesalePrice";

                    if (GridColumnExists(myDataTable, "Description"))

                        myDataTable.Columns["Description"].ColumnName = "PDescription";
                    if (GridColumnExists(myDataTable, "Specs"))

                        myDataTable.Columns["Specs"].ColumnName = "PSpecs";

                    //Remove the extra columns
                    var toRemove = myDataTable.Columns.Cast<DataColumn>().Select(x => x.ColumnName).Except(listOfColumns).ToList();

                    foreach (var col in toRemove) myDataTable.Columns.Remove(col);
                    myDataTable.AcceptChanges();

                    bulkCopy.ColumnMappings.Clear();
                    foreach (DataColumn c in myDataTable.Columns)
                    {
                        bulkCopy.ColumnMappings.Add(c.ColumnName, c.ColumnName);
                    }
                    var Brands = db.Brands.ToList();
                    var RefNRFColorCodes = db.RefNRFColorCodes.ToList();
                    var RefSeasons = db.RefSeasons.ToList();
                    var ProductStatus = db.ProductStatus.ToList();
                    var Vendors = db.Vendors.ToList();
                    var RefCategories = db.RefCategories.ToList();

                    foreach (DataRow row in myDataTable.Rows)
                    {
                        if (GridColumnExists(myDataTable, "BID"))
                        {
                            var inputStr = row["BID"].ToString().ToLower();
                            var retVal = Brands.FirstOrDefault(item => inputStr.Contains(item.BrandName.ToLower()));
                            if (retVal != null)
                                row["BID"] = retVal.BID;
                            else
                            {
                                row["BID"] = 0;
                            }
                        }
                        if (GridColumnExists(myDataTable, "NRFColorCodeID"))
                        {
                            var inputStr = row["NRFColorCodeID"].ToString().ToLower();
                            var retVal = RefNRFColorCodes.FirstOrDefault(item => inputStr.Contains(item.NRFColorName.ToLower()));
                            if (retVal != null)
                                row["NRFColorCodeID"] = retVal.NRFColorCodeID;
                            else
                            {
                                row["NRFColorCodeID"] = 0;
                            }
                            
                        }
                        if (GridColumnExists(myDataTable, "SID"))
                        {
                            var inputStr = row["SID"].ToString().ToLower();
                            var retVal = RefSeasons.FirstOrDefault(item => inputStr.Contains(item.Season.ToLower()));
                            if (retVal != null)
                                row["SID"] = retVal.SID;
                            else
                            {
                                row["SID"] = 0;
                            }
                        }
                        if (GridColumnExists(myDataTable, "ProductStatusId"))
                        {
                            var inputStr = row["ProductStatusId"].ToString().ToLower();
                            var retVal = ProductStatus.FirstOrDefault(item => inputStr.Contains(item.Status.ToLower()));
                            if (retVal != null)
                                row["ProductStatusId"] = retVal.Id;
                            else
                            {
                                row["ProductStatusId"] = 0;
                            }
                        }
                        if (GridColumnExists(myDataTable, "VID"))
                        {
                            var inputStr = row["VID"].ToString().ToLower();

                            var retVal = Vendors.FirstOrDefault(item => inputStr.Contains(item.VendorName.ToLower()));
                            if (retVal != null)
                                row["VID"] = retVal.VID;
                            else
                            {
                                row["VID"] = 0;
                            }
                        }
                        if (GridColumnExists(myDataTable, "PCategory"))
                        {
                            var inputStr = row["PCategory"].ToString().ToLower();

                            var retVal = RefCategories.FirstOrDefault(item=>inputStr.Contains(item.Category.ToLower()));
                            if (retVal != null)
                                row["PCategory"] = retVal.ID;
                            else
                            {
                                row["PCategory"] = 0;
                            }
                        }
                    }
                    bulkCopy.DestinationTableName = "ProductsImport";//myDataTable.TableName;
                    try
                    {
                        bulkCopy.WriteToServer(myDataTable);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }

        [Authorize]
        [ValidateInput(false)]
        public ActionResult GridViewPartial(string path)
        {
            var model = Session["DataTableModel"];
            if (!string.IsNullOrEmpty(path))
            {
                model = InMemoryModel.OpenExcelFile(path);
                BulkImport((DataTable)model);
                Session["DataTableModel"] = model;
            }
            //return PartialView(model);
            var modelL = db.ProductsImports;
            return PartialView("_ImportGridViewPartial", modelL.ToList());
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
            decimal decimalValue;

            if (!string.IsNullOrEmpty(path))
            {
                var model = InMemoryModel.OpenExcelFile(path);
                BulkImport((DataTable)model);
                Session["DataTableModel"] = model;
            }
            var modelL = db.ProductsImports;
            foreach(var importProd in modelL.ToList())
            {
                StringBuilder sb = new StringBuilder();
                if (importProd.BID == null || importProd.BID <= 0)
                {
                    sb.Append("*Invalid brand*").AppendLine();
                }
                if (importProd.VID==null || importProd.VID<=0 )
                {
                    sb.Append("*Invalid vendor*").AppendLine();
                }
                if (importProd.PCategory == null || importProd.PCategory <= 0)
                {
                    sb.Append("*Invalid category*").AppendLine();
                }
                if (string.IsNullOrEmpty(importProd.PColor))
                {
                    sb.AppendLine();
                    sb.Append("*Invalid product color*").AppendLine();
                }
                if (importProd.PCategory<=0)
                {
                    sb.AppendLine();
                    sb.Append("*Invalid product category*").AppendLine();
                }
                if (string.IsNullOrEmpty(importProd.PName))
                {
                    sb.AppendLine();
                    sb.Append("*Invalid product name*").AppendLine();
                }
                if (!String.IsNullOrEmpty((importProd.PWholesalePrice)))
                {
                    bool isDecimal = Decimal.TryParse(importProd.PWholesalePrice, out decimalValue);
                    if (isDecimal)
                    {
                        if (Convert.ToDecimal(importProd.PWholesalePrice) < 0)
                        {
                            sb.AppendLine();
                            sb.Append("*Invalid wholesale price*").AppendLine();
                        }
                    }
                    else
                    {
                        sb.AppendLine();
                        sb.Append("*Invalid wholesale price*").AppendLine();
                    }
                }
                if (!String.IsNullOrEmpty((importProd.PMSRPPrice)))
                {
                    bool isDecimal = Decimal.TryParse(importProd.PMSRPPrice, out decimalValue);
                    if (isDecimal)
                    {
                        if (Convert.ToDecimal(importProd.PMSRPPrice) < 0)
                        {
                            sb.AppendLine();
                            sb.Append("*Invalid MSRP price*").AppendLine();
                        }
                    }
                    else
                    {
                        sb.AppendLine();
                        sb.Append("*Invalid MSRP price*").AppendLine();
                    }
                }
                if (!String.IsNullOrEmpty((importProd.PLandedCost)))
                {
                    bool isDecimal = Decimal.TryParse(importProd.PLandedCost, out decimalValue);
                    if (isDecimal)
                    {
                        if (Convert.ToDecimal(importProd.PLandedCost) < 0)
                        {
                            sb.AppendLine();
                            sb.Append("*Invalid landed cost*").AppendLine();
                        }
                    }
                    else
                    {
                        sb.AppendLine();
                        sb.Append("*Invalid landed cost*").AppendLine();
                    }
                }
                if (!String.IsNullOrEmpty((importProd.PFOBCost)))
                {
                    bool isDecimal = Decimal.TryParse(importProd.PFOBCost, out decimalValue);
                    if (isDecimal)
                    {
                        if (Convert.ToDecimal(importProd.PFOBCost) < 0)
                        {
                            sb.AppendLine();
                            sb.Append("*Invalid FOB cost*").AppendLine();
                        }
                    }
                    else
                    {
                        sb.AppendLine();
                        sb.Append("*Invalid FOB cost*").AppendLine();
                    }
                }
                /*if (importProd.NRFColorCodeID <= 0)
                {
                    sb.AppendLine();
                    sb.Append("**Invalid NRF color code id*").AppendLine();
                }*/
                importProd.Message = sb.ToString();
            }
            var modelLL = modelL.ToList();
            modelLL.ForEach(item => item.PDiscontinued = false);
            var dupes = modelLL.GroupBy(x => new { x.SKUCode, x.UPCCode, x.BID, x.VID, x.PCategory, x.PColor, x.PName })
                  .Where(x => x.Skip(1).Any());
            var hasDupes = modelLL.GroupBy(x => new { x.SKUCode, x.UPCCode, x.BID, x.VID, x.PCategory, x.PColor, x.PName })
                   .Where(x => x.Skip(1).Any()).Any();
            if (dupes != null && dupes.Any())
            {
                foreach (var dup in dupes)
                {
                    modelLL = modelLL.Select(n => { if (n.SKUCode == dup.Key.SKUCode 
                        && n.UPCCode == dup.Key.UPCCode
                        && n.BID == dup.Key.BID
                        && n.VID == dup.Key.VID
                        && n.PColor == dup.Key.PColor
                        && n.PName == dup.Key.PName) { n.PDiscontinued = true; n.Message += "*duplicate product*"; } return n; }).ToList();
                }
            }
            //modelL.ToList().ForEach(x => x.Message = "Test message goes here");
            return PartialView("_ImportGridViewPartial", modelLL);
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
            //product.PID = Convert.ToInt32(System.Web.HttpContext.Current.Request.Params["PID"]);
            var model = db.ProductsImports;
            //var temp = model.ToList();
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
            foreach (var productID in updateValues.DeleteKeys)
            {
                //DeleteProduct(productID, updateValues);
            }
            return RedirectToAction("Index");
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