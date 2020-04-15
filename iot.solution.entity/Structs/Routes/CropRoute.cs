using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity.Structs.Routes
{
    public struct CropRoute
    {
        public struct Name
        {
            public const string Add = "crop.add";
            public const string GetList = "crop.list";
            public const string GetById = "crop.getbyid";
            public const string Delete = "crop.deletecrop";
            public const string UpdateStatus = "crop.updatestatus";
            public const string BySearch = "crop.search";
            public const string GetCrops = "crop.getcrops";
        }

        public struct Route
        {
            public const string Global = "api/crop";
            public const string Manage = "manage";            
            public const string GetList = "";
            public const string BySearch = "search";
            public const string GetById = "{id}";
            public const string GetCrops = "crops/{greenHouseId}";
            public const string Delete = "delete/{id}";
            public const string UpdateStatus = "updatestatus/{id}/{status}";
        }
    }
}
