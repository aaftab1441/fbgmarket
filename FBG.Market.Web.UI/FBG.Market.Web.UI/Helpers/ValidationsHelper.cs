using System;
using System.Web.Mvc;
using DevExpress.Web;
using DevExpress.Web.Mvc;

namespace FBG.Market.Web.Identity.Helpers
{
    public class ValidationsHelper
    {
        static ValidationSettings nameValidationSettings;
        public static ValidationSettings NameValidationSettings
        {
            get
            {
                if (nameValidationSettings == null)
                {
                    //nameValidationSettings = ValidationSettings.CreateValidationSettings();
                    nameValidationSettings.Display = Display.Dynamic;
                    nameValidationSettings.ErrorDisplayMode = ErrorDisplayMode.ImageWithText;
                    nameValidationSettings.ErrorText = "Name is required";
                }
                return nameValidationSettings;
            }
        }
        public static void OnNameValidation(object sender, ValidationEventArgs e)
        {
            if (e.Value == null)
            {
                e.IsValid = false;
                return;
            }
            var name = e.Value.ToString();
            if (name == string.Empty)
                e.IsValid = false;
            if (name.Length > 50)
            {
                e.IsValid = false;
                e.ErrorText = "Must be under 50 characters";
            }
        }
    }
}