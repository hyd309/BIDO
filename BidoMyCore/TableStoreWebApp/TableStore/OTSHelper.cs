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
        public static OTSClient _oTSClient_Event;
        public static OTSClient _oTSClient_Location;
        public static OTSClient _oTSClient_Route;
        public static OTSClient GetOTSClientEvent(TableStoreModel tableStoreModel)
        {
            if (_oTSClient_Event == null)
            {
                _oTSClient_Event = new OTSClient(GetEndPoint(tableStoreModel,tableStoreModel.instanceName_Event),
                    tableStoreModel.accessKeyID, tableStoreModel.accessKeySecret, tableStoreModel.instanceName_Event);
            }
            return _oTSClient_Event;
        }

        public static OTSClient GetOTSClientLocation(TableStoreModel tableStoreModel)
        {
            if (_oTSClient_Location == null)
            {
                _oTSClient_Location = new OTSClient(GetEndPoint(tableStoreModel, tableStoreModel.instanceName_Location),
                    tableStoreModel.accessKeyID, tableStoreModel.accessKeySecret, tableStoreModel.instanceName_Location);
            }
            return _oTSClient_Location;
        }

        public static OTSClient GetOTSClientRoute(TableStoreModel tableStoreModel)
        {
            if (_oTSClient_Route == null)
            {
                _oTSClient_Route = new OTSClient(GetEndPoint(tableStoreModel, tableStoreModel.instanceName_Route),
                    tableStoreModel.accessKeyID, tableStoreModel.accessKeySecret, tableStoreModel.instanceName_Route);
            }
            return _oTSClient_Route;
        }

        private static string GetEndPoint(TableStoreModel tableStoreModel,string keyName)
        {
            string endPoint = tableStoreModel.PublicEnvironment == "1" ? tableStoreModel.endPointPublic : tableStoreModel.endPointPrivate;
            return string.Format(endPoint,keyName);
        }
    }
}
