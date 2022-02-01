using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FBG.Market.Web.Identity.Models
{
    public class VendorViewModel
    {

        public int VID { get; set; }
        [DataType(DataType.Text, ErrorMessage = "Please enter name"), Required, Display(Name = "Vendor Name"), MaxLength(100)]
        public string VendorName { get; set; }
        public string VendorAcronym { get; set; }
        [DataType(DataType.Text), Display(Name = "Vendor Address")]
        public string VendorAddress { get; set; }
        public string VendorCityState { get; set; }
        public string VendorCountry { get; set; }
        [DataType(DataType.PhoneNumber)]
        public string VendorPhone { get; set; }
        [DataType(DataType.EmailAddress, ErrorMessage = "Please enter valid email address.")]
        public string VendorEmail { get; set; }
        [DataType(DataType.Url, ErrorMessage ="Please enter a valid vendor website.")]
        public string VendorWebsite { get; set; }
        [DataType(DataType.PhoneNumber)]
        public string VendorContact { get; set; }
        public string VendorLogo { get; set; }
        public byte[] Photo { get; set; }
        public string UserId { get; set; }
    }

}