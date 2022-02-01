using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FBG.Market.Web.Identity.Models
{
    public class ImageSliderFeaturesDemoOptions
    {
        ImageSliderImageAreaSettings settingsImageArea;
        ImageSliderNavigationBarSettings settingsNavigationBar;
        ImageSliderBehaviorSettings settingsBehavior;

        public ImageSliderFeaturesDemoOptions()
        {
            this.settingsImageArea = new ImageSliderImageAreaSettings(null)
            {
                ImageSizeMode = ImageSizeMode.ActualSizeOrFit,
                AnimationType = AnimationType.Slide,
                NavigationDirection = NavigationDirection.Horizontal,
                ItemTextVisibility = ElementVisibilityMode.OnMouseOver,
                NavigationButtonVisibility = ElementVisibilityMode.OnMouseOver
            };
            this.settingsNavigationBar = new ImageSliderNavigationBarSettings(null)
            {
                Position = NavigationBarPosition.Bottom,
                Mode = NavigationBarMode.Thumbnails,
                ThumbnailsModeNavigationButtonVisibility = ElementVisibilityMode.OnMouseOver
            };
            this.settingsBehavior = new ImageSliderBehaviorSettings(null)
            {
                EnablePagingGestures = true,
                EnablePagingByClick = AutoBoolean.Auto,
                ImageLoadMode = ImageLoadMode.DynamicLoadAndCache,
                ExtremeItemClickMode = ExtremeItemClickMode.SelectAndSlide
            };
            ShowNavigationBar = true;
            PredefinedScenario = "Default";
        }

        public ImageSliderImageAreaSettings SettingsImageArea { get { return settingsImageArea; } }
        public ImageSliderNavigationBarSettings SettingsNavigationBar { get { return settingsNavigationBar; } }
        public ImageSliderBehaviorSettings SettingsBehavior { get { return settingsBehavior; } }
        public bool ShowNavigationBar { get; set; }
        public string PredefinedScenario { get; set; }
    }
}