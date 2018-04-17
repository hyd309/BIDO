using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Aliyun.OTS;
using Aliyun.OTS.Request;
using Aliyun.OTS.DataModel;

namespace TableStoreWebApp.Models
{
    public class TableStoreModel
    {
        public string PublicEnvironment{get;set;}
        public string endPointPublic { get; set; }
        public string endPointPrivate { get; set; }
        public string accessKeyID { get; set; }
        public string accessKeySecret { get; set; }
        public string instanceName_Location { get; set; }
        public string instanceName_Event { get; set; }
        public string instanceName_Route { get; set; }
    }
}
