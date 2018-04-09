using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TableStoreWebApp.Controllers
{
    using Microsoft.Extensions.Options;
    using TableStoreWebApp.Models;
    using log4net;
    using System.Threading;
    using Aliyun.OTS;
    using Aliyun.OTS.Request;
    using Aliyun.OTS.Response;
    using Aliyun.OTS.DataModel;
    using System.Text;

    public class OTSSyncController : Controller
    {
        private ILog log = LogManager.GetLogger(Startup.repository.Name, typeof(OTSSyncController));
        public TableStoreModel _tableStoreModel;
        public Services.IUserServices _IUserServices;


        public OTSSyncController(IOptions<TableStoreModel> option, Services.IUserServices iUserServices)
        {
            _tableStoreModel = option.Value;
            _IUserServices = iUserServices;
        }

        // GET: OTSSync
        public ActionResult Index()
        {
            log.Info("OTSSync=>index()");
            _IUserServices.GetUserName(1);
            return View();
        }

        public ActionResult SyncData()
        {
            string routeName = Request.Form["route"];
            log.Debug("routeName="+ routeName);
            if (!string.IsNullOrEmpty(routeName.Trim()))
            {
                Route _route = new Route(_tableStoreModel);
                Thread thread = new Thread(new ParameterizedThreadStart(_route.GetRoutenData));
                thread.Start(routeName);
            }

            string locationName = Request.Form["location"];
            log.Debug("locationName=" + locationName);
            if (!string.IsNullOrEmpty(locationName.Trim()))
            {
                Location location = new Location(_tableStoreModel);
                Thread thread = new Thread(new ParameterizedThreadStart(location.GetLocationData));
                thread.Start(locationName);
            }


            string eventName = Request.Form["event"];
            log.Debug("eventName=" + eventName);
            if (!string.IsNullOrEmpty(eventName.Trim()))
            {
                Event _event = new Event(_tableStoreModel);
                Thread thread = new Thread(new ParameterizedThreadStart(_event.GetEventData));
                thread.Start(eventName);
            }

            return RedirectToAction("Success");
        }

        public ActionResult Success()
        {
            return View();
        }

        public ActionResult Search()
        {
            return View();
        }

        public ActionResult SearchLocation()
        {
            OTSClient _oTSClient=OTSHelper.GetOTSClient(_tableStoreModel);
            PrimaryKey pk = new PrimaryKey();
            pk.Add("d",new ColumnValue(Convert.ToInt64(Request.Form["d"])));
            pk.Add("t", new ColumnValue(Convert.ToInt64(TimeHelper.GetTimeStamp(Convert.ToDateTime(Request.Form["t"])))));
            GetRowRequest getRowRequest = new GetRowRequest("L_100000000", pk);
            GetRowResponse response = _oTSClient.GetRow(getRowRequest);
            StringBuilder sbAttributeColumns = new StringBuilder();
            foreach (var item in response.Attribute)
            {
                if (item.Key == "l")
                {
                    byte[] lbyte = item.Value.BinaryValue;
                    Dictionary<string, int> dictionary = ByteIntHelper.GetLocationByByte(lbyte);
                    foreach (var dic in dictionary)
                    {
                        sbAttributeColumns.Append(dic.Key + ":" + dic.Value + ";");
                    }
                }
            }

            ViewData["msg"] = sbAttributeColumns.ToString();
            return View("Search", ViewBag);
        }

        public ActionResult SearchEvent()
        {
            OTSClient _oTSClient = OTSHelper.GetOTSClient(_tableStoreModel);
            PrimaryKey pk = new PrimaryKey();
            pk.Add("d", new ColumnValue(Convert.ToInt64(Request.Form["d"])));
            pk.Add("et", new ColumnValue(Convert.ToInt64(TimeHelper.GetTimeStamp(Convert.ToDateTime(Request.Form["et"])))));
            pk.Add("ei", new ColumnValue(Convert.ToInt32(Request.Form["ei"])));
            GetRowRequest getRowRequest = new GetRowRequest("E_100000000", pk);
            GetRowResponse response = _oTSClient.GetRow(getRowRequest);
            StringBuilder sbAttributeColumns = new StringBuilder();
            foreach (var item in response.Attribute)
            {
                switch (item.Key)
                {
                    case "ep":
                        //事件参数字段暂不做处理
                        //byte[] ep = item.Value.BinaryValue;
                        break;
                    case "t":
                        sbAttributeColumns.Append(item.Key + ":" + item.Value.IntegerValue + "【" + TimeHelper.ConvertStringToDateTime(item.Value.IntegerValue.ToString()) + "】;");
                        break;
                    case "l":
                        byte[] lbyte = item.Value.BinaryValue;
                        Dictionary<string, int> dictionary = ByteIntHelper.GetLocationByByte(lbyte);
                        foreach (var dic in dictionary)
                        {
                            sbAttributeColumns.Append(dic.Key + ":" + dic.Value + ";");
                        }
                        break;
                }
            }

            ViewData["msg"] = sbAttributeColumns.ToString();
            return View("Search", ViewBag);
        }

        public ActionResult SearchRoute()
        {
            OTSClient _oTSClient = OTSHelper.GetOTSClient(_tableStoreModel);
            PrimaryKey pk = new PrimaryKey();
            pk.Add("d", new ColumnValue(Convert.ToInt64(Request.Form["d"])));
            pk.Add("s", new ColumnValue(Convert.ToInt64(TimeHelper.GetTimeStamp(Convert.ToDateTime(Request.Form["s"])))));
            GetRowRequest getRowRequest = new GetRowRequest("Route", pk);
            GetRowResponse response = _oTSClient.GetRow(getRowRequest);
            StringBuilder sbAttributeColumns = new StringBuilder();
            foreach (var item in response.Attribute)
            {
                switch (item.Key)
                {
                    case "e":
                        sbAttributeColumns.Append(item.Key + ":" + item.Value.IntegerValue + "【" + TimeHelper.ConvertStringToDateTime(item.Value.IntegerValue.ToString()) + "】;");
                        break;
                    case "r":
                        byte[] lbyte = item.Value.BinaryValue;
                        Dictionary<string, int> dictionary = ByteIntHelper.GetRouteByByte(lbyte);
                        foreach (var dic in dictionary)
                        {
                            sbAttributeColumns.Append(dic.Key + ":" + dic.Value + ";");
                        }
                        break;
                    case "ds":
                        byte[] ds = item.Value.BinaryValue;
                        Dictionary<string, int> dsDic = ByteIntHelper.GetDurationstatsByByte(ds);
                        foreach (var dic in dsDic)
                        {
                            sbAttributeColumns.Append(dic.Key + ":" + dic.Value + ";");
                        }
                        break;
                    case "es":
                        byte[] es = item.Value.BinaryValue;
                        Dictionary<string, int> esDic = ByteIntHelper.GetEventStatsByByte(es);
                        foreach (var dic in esDic)
                        {
                            sbAttributeColumns.Append(dic.Key + ":" + dic.Value + ";");
                        }
                        break;
                }
            }

            ViewData["msg"] = sbAttributeColumns.ToString();
            return View("Search", ViewBag);
        }
    }
}