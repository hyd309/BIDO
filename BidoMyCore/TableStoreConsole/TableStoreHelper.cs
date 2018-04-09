using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;
using TableStoreConsole.Data;
using System.Data.SqlClient;
using System.Data;
using log4net;
using System.Threading.Tasks;
using System.Threading;
namespace TableStoreConsole
{
    public class TableStoreHelper
    {
        private ILog log = LogManager.GetLogger(TableStoreModel.repository.Name,typeof(TableStoreHelper));
        public void MainData()
        {
            try
            {
                ////多线程调用
                Thread[] thread = new Thread[3];

                Location location = new Location();
                //location.GetLocationData("Device_Location_20170214");
                thread[0] = new Thread(new ParameterizedThreadStart(location.GetLocationData));
                thread[0].Start("Device_Location_20170214");

                Event eventInfo = new Event();
                ////eventInfo.GetEventData("Device_Alarm_201702");
                thread[1] = new Thread(new ParameterizedThreadStart(eventInfo.GetEventData));
                thread[1].Start("Device_Alarm_201702");

                //Route route = new Route();
                ////route.GetRoutenData("Vehicle_Route");
                //thread[2] = new Thread(new ParameterizedThreadStart(route.GetRoutenData));
                //thread[2].Start("Vehicle_Route");


                ////异步调用
                //Action<string> action = new Action<string>(location.GetLocationData);
                //AsyncCallback callback = new AsyncCallback(
                //    r =>
                //    log.Info("Device_Location_20170214异步回到完成"));
                //action.BeginInvoke("Device_Location_20170214", callback, "Status");

            }
            catch (Exception ex)
            {
                log.Error("TableStoreHelper=>主函数报错" + ex.Message);
            }
        }
    }
}
