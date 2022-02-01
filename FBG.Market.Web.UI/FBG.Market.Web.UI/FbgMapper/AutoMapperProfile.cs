using AutoMapper;
using FBG.Market.Web.Identity.Models;
using FBG.Market.Web.Identity.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FBG.Market.Web.Identity.FbgMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Product, ProductViewModel>();
            CreateMap<Brand, BrandViewModel>();
            CreateMap<Vendor, VendorViewModel>();
        }
    }
}