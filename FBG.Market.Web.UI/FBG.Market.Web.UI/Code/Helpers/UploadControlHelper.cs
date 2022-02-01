﻿using System.Drawing;
using System.IO;
using System.Web;
using System.Web.UI;
using DevExpress.Web;
using DevExpress.Web.Internal;
using FBG.Market.Web.Identity;

namespace FBG.Market.Web.Identity {
    public class UploadControlHelper {
        //public const string UploadDirectory = "~/Content/UploadControl/UploadFolder/";
        public const string UploadDirectory = "~/brands/brand-1/prod-1/";
        public const string DocumentsDirectory = "~/Content/UploadControl/UploadDocuments/";
        public const string TempDirectory = "~/Content/UploadControl/Temp/";
        public const string ThumbnailFormat = "Thumbnail{0}{1}";
        
        public static readonly UploadControlValidationSettings UploadValidationSettings = new UploadControlValidationSettings {
            AllowedFileExtensions = new string[] { ".jpg", ".jpeg", ".gif", ".png" },
            MaxFileSize = 4194304
           
        };
        
        public static void ucDragAndDrop_FileUploadComplete(object sender, FileUploadCompleteEventArgs e) {
            if(e.UploadedFile.IsValid) {
                string fileName = Path.ChangeExtension(Path.GetRandomFileName(), ".jpg");
                string resultFilePath = UploadDirectory + fileName;
                using(Image original = Image.FromStream(e.UploadedFile.FileContent))
                using (Image thumbnail = new ImageThumbnailCreator(original).CreateImageThumbnail(new Size(350, 350)))
                    ImageUtils.SaveToJpeg(thumbnail, HttpContext.Current.Request.MapPath(resultFilePath));
                UploadingUtils.RemoveFileWithDelay(fileName, HttpContext.Current.Request.MapPath(resultFilePath), 5);
                IUrlResolutionService urlResolver = sender as IUrlResolutionService;
                if(urlResolver != null)
                    e.CallbackData = urlResolver.ResolveClientUrl(resultFilePath);
            }
        }

        public static void ucMultiSelection_FileUploadComplete(object sender, FileUploadCompleteEventArgs e) {
            var UploadDirectory = System.Web.HttpContext.Current.Request.Params["products-path"];
            string resultFileName = Path.GetRandomFileName() + "_" + e.UploadedFile.FileName;
            
            //string resultFileUrl = UploadDirectory + resultFileName;
            string resultFileUrl = UploadDirectory + "/"+ resultFileName;
            
            string resultFilePath = HttpContext.Current.Request.MapPath(resultFileUrl);
            e.UploadedFile.SaveAs(resultFilePath);

            //UploadingUtils.RemoveFileWithDelay(resultFileName, resultFilePath, 5);

            IUrlResolutionService urlResolver = sender as IUrlResolutionService;
            if(urlResolver != null) {
                string url = urlResolver.ResolveClientUrl(resultFileUrl);
                e.CallbackData = GetCallbackData(e.UploadedFile, url);
            }
        }
        static string GetCallbackData(UploadedFile uploadedFile, string fileUrl) {
            string name = uploadedFile.FileName;
            long sizeInKilobytes = uploadedFile.ContentLength / 1024;
            string sizeText = sizeInKilobytes.ToString() + " KB";

            return name + "|" + fileUrl + "|" + sizeText;
        }

     
    }
}
