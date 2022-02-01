using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FBG.Market.Web.Identity.Models
{
    public class BrandViewModel
    {
        public int VID { get; set; }
        public int BID { get; set; }
        [Required(ErrorMessage = "Please enter brand name.")]
        [DataType(DataType.Text, ErrorMessage = "Please enter name"), Display(Name = "Brand Name"), MaxLength(50)]
        public string BrandName { get; set; }
        [Required(ErrorMessage = "Please enter brand category.")]
        public short BrandCategory { get; set; }
        public string BrandSubCategory { get; set; }
        [DataType(DataType.EmailAddress, ErrorMessage = "Please enter valid email address.")]
        public string BrandWebSite { get; set; }
        public string BrandNotes { get; set; }
        public string BrandLogo { get; set; }
        public byte[] Photo { get; set; }

        public byte[] SSMA_TimeStamp { get; set; }
    }
}