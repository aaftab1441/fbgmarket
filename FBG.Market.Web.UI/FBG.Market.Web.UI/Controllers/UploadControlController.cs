using DevExpress.Web.Mvc;
using System.Collections.Generic;
using DevExpress.Web;
using System.Web.Mvc;
using FBG.Market.Web.Identity;

namespace FBG.Market.Web.Identity.Controllers {


    public class UploadControlController : Controller
    {
        public ActionResult MultiFileSelection()
        {
         
            return View("MultiFileSelection");
        }
        public ActionResult MultiSelectionImageUpload([ModelBinder(typeof(MultiFileSelectionDemoBinder))] IEnumerable<UploadedFile> ucMultiSelection)
        {
            return null;
        }

        public ActionResult UploadedFilesContainer(bool useExtendedPopup)
        {
            ViewData["UseExtendedPopup"] = useExtendedPopup;
            return PartialView("UploadedFilesContainer");            
        }

    }


    public class MultiFileSelectionDemoBinder : DevExpressEditorsBinder
    {
        public MultiFileSelectionDemoBinder()
        {
            UploadControlBinderSettings.ValidationSettings.Assign(UploadControlHelper.UploadValidationSettings);
            UploadControlBinderSettings.FileUploadCompleteHandler = UploadControlHelper.ucMultiSelection_FileUploadComplete;
        }
    }
}
