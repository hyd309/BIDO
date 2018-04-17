using System;
using System.Collections.Generic;
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
            //_IUserServices.GetUserName(1);

            return View();
        }

        public ActionResult SyncData()
        {
            try
            {
                string eventFrom = Request.Form["eventFrom"];
                log.Debug("eventFrom=" + eventFrom);
                if (!string.IsNullOrEmpty(eventFrom.Trim()))
                {
                    string eventTo = Request.Form["eventTo"];
                    Event _event = new Event(_tableStoreModel);
                    if (string.IsNullOrEmpty(eventTo))
                    {
                        eventTo = DateTime.Now.ToString("yyyyMMdd");
                    }
                    if (Convert.ToDateTime(eventFrom + "-01") > Convert.ToDateTime(eventTo + "-01"))
                    {
                        ViewData["msg"] = "Event 开始日期大于结束日期";
                        return View("Index", ViewBag);
                    }
                    else
                    {
                        Thread thread = new Thread(new ParameterizedThreadStart(_event.GetEventData));
                        log.Debug("GetEventData()线程启动开始！");
                        thread.Start(eventFrom + "-01" + "T" + eventTo + "-01");
                    }
                }

                string locationFrom = Request.Form["locationFrom"];
                log.Debug("locationFrom=" + locationFrom);
                if (!string.IsNullOrEmpty(locationFrom.Trim()))
                {
                    string locationTo = Request.Form["locationTo"];
                    Location location = new Location(_tableStoreModel);
                    if (string.IsNullOrEmpty(locationTo))
                    {
                        locationTo = DateTime.Now.ToString("yyyy-MM-dd");
                    }
                    if (Convert.ToDateTime(locationFrom) > Convert.ToDateTime(locationTo))
                    {
                        ViewData["msg"] = "Location开始日期大于结束日期";
                        return View("Index", ViewBag);
                    }
                    else
                    {
                        Thread thread = new Thread(new ParameterizedThreadStart(location.GetLocationData));
                        log.Debug("GetLocationData()线程启动开始！");
                        thread.Start(locationFrom + "T" + locationTo);
                    }
                }
                string routeName = Request.Form["routeName"];
                log.Debug("routeName=" + routeName);
                if (!string.IsNullOrEmpty(routeName.Trim()))
                {
                    Route _route = new Route(_tableStoreModel);
                    Thread thread = new Thread(new ParameterizedThreadStart(_route.GetRoutenData));
                    log.Debug("GetRoutenData()线程启动开始！");
                    thread.Start(routeName);
                }

                return RedirectToAction("Success");
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                log.Error(ex.StackTrace);
                return View();
            }
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
            OTSClient _oTSClient=OTSHelper.GetOTSClientLocation(_tableStoreModel);
            PrimaryKey pk = new PrimaryKey();
            pk.Add("d",new ColumnValue(Convert.ToInt64(Request.Form["d"])));
            pk.Add("t", new ColumnValue(TimeHelper.ConvertDateTimeToInt(Convert.ToDateTime(Request.Form["t"]))));
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
                        sbAttributeColumns.Append(dic.Key + ":" + dic.Value + "； ");
                    }
                }
            }
            ViewData["pk"] = "设备：" + Request.Form["d"] + " 时间：" + Request.Form["t"];
            ViewData["att"] = sbAttributeColumns.ToString();
            return View("Search", ViewBag);
        }

        public ActionResult SearchEvent()
        {
            OTSClient _oTSClient = OTSHelper.GetOTSClientEvent(_tableStoreModel);
            PrimaryKey pk = new PrimaryKey();
            pk.Add("d", new ColumnValue(Convert.ToInt64(Request.Form["d"])));
            pk.Add("et", new ColumnValue(TimeHelper.ConvertDateTimeToInt(Convert.ToDateTime(Request.Form["et"]))));
            pk.Add("ei", new ColumnValue(ByteIntHelper.intToBytes2(Convert.ToInt64(Request.Form["ei"]), 1)));
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
                        sbAttributeColumns.Append(item.Key + ":" + item.Value.IntegerValue + "【" + TimeHelper.ConvertStringToDateTime(item.Value.IntegerValue.ToString()).ToString("yyyy-MM-dd HH:mm:ss fff") + "】；");
                        break;
                    case "l":
                        byte[] lbyte = item.Value.BinaryValue;
                        Dictionary<string, int> dictionary = ByteIntHelper.GetLocationByByte(lbyte);
                        foreach (var dic in dictionary)
                        {
                            sbAttributeColumns.Append(dic.Key + ":" + dic.Value + "； ");
                        }
                        break;
                }
            }

            ViewData["pk"] = "设备：" + Request.Form["d"] + " 事件时间：" + Request.Form["et"] + " 事件ID：" + Request.Form["ei"];
            ViewData["att"] = sbAttributeColumns.ToString();
            return View("Search", ViewBag);
        }

        public ActionResult SearchRoute()
        {
            OTSClient _oTSClient = OTSHelper.GetOTSClientRoute(_tableStoreModel);
            PrimaryKey pk = new PrimaryKey();
            pk.Add("d", new ColumnValue(Convert.ToInt64(Request.Form["d"])));
            pk.Add("s", new ColumnValue(TimeHelper.ConvertDateTimeToInt(Convert.ToDateTime(Request.Form["s"]))));
            GetRowRequest getRowRequest = new GetRowRequest("Route", pk);
            GetRowResponse response = _oTSClient.GetRow(getRowRequest);
            StringBuilder sbAttributeColumns = new StringBuilder();
            foreach (var item in response.Attribute)
            {
                switch (item.Key)
                {
                    case "e":
                        sbAttributeColumns.Append(item.Key + ":" + item.Value.IntegerValue + "【" + TimeHelper.ConvertStringToDateTime(item.Value.IntegerValue.ToString()).ToString("yyyy-MM-dd HH:mm:ss fff") + "】;");
                        break;
                    case "r":
                        byte[] lbyte = item.Value.BinaryValue;
                        Dictionary<string, int> dictionary = ByteIntHelper.GetRouteByByte(lbyte);
                        foreach (var dic in dictionary)
                        {
                            sbAttributeColumns.Append(dic.Key + ":" + dic.Value + "； ");
                        }
                        break;
                    case "ds":
                        byte[] ds = item.Value.BinaryValue;
                        Dictionary<string, int> dsDic = ByteIntHelper.GetDurationstatsByByte(ds);
                        foreach (var dic in dsDic)
                        {
                            sbAttributeColumns.Append(dic.Key + ":" + dic.Value + "； ");
                        }
                        break;
                    case "es":
                        byte[] es = item.Value.BinaryValue;
                        Dictionary<string, int> esDic = ByteIntHelper.GetEventStatsByByte(es);
                        foreach (var dic in esDic)
                        {
                            sbAttributeColumns.Append(dic.Key + ":" + dic.Value + "； ");
                        }
                        break;
                }
            }
            ViewData["pk"] = "设备：" + Request.Form["d"] + " 开始时间：" + Request.Form["s"];
            ViewData["att"] = sbAttributeColumns.ToString();
            return View("Search", ViewBag);
        }
    }
}