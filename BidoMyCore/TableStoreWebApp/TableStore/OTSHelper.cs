using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Aliyun.OTS;
using Aliyun.OTS.Request;
using Aliyun.OTS.DataModel;
using Microsoft.Extensions.Options;
using TableStoreWebApp.Models;

namespace TableStoreWebApp
{
    public class OTSHelper
    {
        public static OTSClient _oTSClient;
        public static OTSClient GetOTSClient(TableStoreModel tableStoreModel)
        {
            if (_oTSClient==null)
            {
                _oTSClient = new OTSClient(tableStoreModel.PublicEnvironment == "1" ? tableStoreModel.endPointPublic : tableStoreModel.endPointPrivate,
                    tableStoreModel.accessKeyID, tableStoreModel.accessKeySecret, tableStoreModel.instanceName);
            }
            return _oTSClient;
        }
    }
}
