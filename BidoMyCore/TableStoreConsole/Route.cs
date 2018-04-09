using System;
using System.Collections.Generic;
using System.Text;

namespace TableStoreConsole
{
    using log4net;
    using Aliyun.OTS;
    using Aliyun.OTS.Request;
    using Aliyun.OTS.DataModel;
    using System.Data;
    using System.Data.SqlClient;
    using Aliyun.OTS.Response;

    public class Route
    {
        private ILog log = LogManager.GetLogger(TableStoreModel.repository.Name, typeof(Route));

        OTSClient oTSClient = new OTSClient(TableStoreModel.endPointPublic, TableStoreModel.accessKeyID, TableStoreModel.accessKeySecret, TableStoreModel.instanceName);
        BatchWriteRowRequest batchRequest = new BatchWriteRowRequest();

        public void GetRoutenData(object tableName)
        {
            DataTable dt = new DataTable();
            int start = 0;
            bool nextId = true;
            int indexStep = 100000;//***每次过滤id 10万的范围，id可能不是连续的
            while (nextId)
            {
                using (SqlConnection conn = new SqlConnection(TableStoreModel.SqlServerConnection))
                {
                    try
                    {
                        /* 
                         * drivingTime 驾驶时间 （秒）
                         * mileage 里程 （米）
                         * topSpeed 最大速度(HM/ H)
                         * speedingTime 超速行驶时间(> 120km / h)(秒)
                         * highSpeedTime   高速行驶的时间(80km / h - 120km / h)
                         * mediumSpeedTime 中速行驶的时间
                         * lowSpeedTime 低速行驶的时间(1km / h - 40km / h)
                         * idleTime 怠速时间
                         */
                        string sql = @"select distinct device_code,startTime,endTime,startLatitude,endLatitude,startLongitude,endLongitude,drivingTime,mileage,topSpeed" +
                            ",speedingTime,highSpeedTime,mediumSpeedTime,lowSpeedTime,idleTime,rapidAccelerationTimes,rapidDecelerationTimes,sharpTurnTimes from " + tableName
                            + " where id between " + start + " and " + (start + indexStep);
                        log.Debug(sql);
                        start += indexStep;
                        SqlCommand cmd = new SqlCommand(sql, conn);
                        conn.Open();
                        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        sqlDataAdapter.Fill(ds);
                        cmd.Dispose();
                        conn.Close();
                        nextId = ds.Tables[0].Rows.Count > 0;
                        TableStoreAddLocation(ds.Tables[0].Rows);


                        //并发处理
                        //Task[] tasks = new Task[] {
                        //    Task.Run(() => TableStoreAddLocation(ds.Tables[0].Rows)),
                        //    Task.Run(() => TableStoreAddLocation(ds.Tables[0].Rows)),
                        //    Task.Run(() => TableStoreAddLocation(ds.Tables[0].Rows))
                        //};
                        //var result = Task.WhenAll(tasks);

                        //并行处理
                        //Thread thread1 = new Thread(new ParameterizedThreadStart(TableStoreAddLocation));
                        //thread1.Start(ds.Tables[0].Rows);
                    }
                    catch (Exception ex)
                    {
                        log.Error("GetRoutenData=>访问数据库出错：" + ex.Message);
                    }
                }
            }
        }

        public void TableStoreAddLocation(DataRowCollection drs)
        {
            if (drs.Count == 0)
            {
                return;
            }
            int batchCount = 0;
            int i = 0;
            List<string> list = new List<string>();//存储批量上传的主键字符串，用于判断批量上传数据中是否有重复
            try
            {
                RowChanges rowChanges = new RowChanges();
                foreach (DataRow dr in drs)
                {
                    var primaryKey = new PrimaryKey();
                    primaryKey.Add("d", new ColumnValue(Convert.ToInt64(dr["device_code"])));
                    primaryKey.Add("s", new ColumnValue(TimeHelper.ConvertDateTimeToInt(Convert.ToDateTime(dr["startTime"]))));

                    string primarykey = dr["device_code"].ToString() + ";" + dr["startTime"].ToString();
                    if (list.Contains(primarykey))
                    {
                        log.Info("发现重复记录" + primarykey + "数据库记录：");
                        continue;
                    }
                    list.Add(primarykey);
                    var attribute = new AttributeColumns();
                    attribute.Add("e", new ColumnValue(TimeHelper.ConvertDateTimeToInt(Convert.ToDateTime(dr["endTime"]))));
                    //行驶数据
                    attribute.Add("r", new ColumnValue(ByteIntHelper.GetRouteByte(dr["startLongitude"],dr["startLatitude"], dr["endLongitude"], dr["endLatitude"], dr["drivingTime"], dr["mileage"], dr["topSpeed"])));
                    //行驶时长统计
                    attribute.Add("ds", new ColumnValue(ByteIntHelper.GetDurationstatsByte(dr["speedingTime"], dr["highSpeedTime"], dr["mediumSpeedTime"], dr["lowSpeedTime"], dr["idleTime"])));
                    //事件次数统计
                    attribute.Add("es", new ColumnValue(ByteIntHelper.GetEventStatsByte(dr["rapidAccelerationTimes"], dr["rapidDecelerationTimes"], dr["sharpTurnTimes"])));

                    rowChanges.AddPut(new Condition(RowExistenceExpectation.IGNORE), primaryKey, attribute);
                    batchCount++;
                    i++;
                    if (batchCount == 200)//200行数据进行批量提交一次
                    {
                        batchRequest.Add("Route", rowChanges);
                        BatchWriteRowResponse batchWriteRowResponse =oTSClient.BatchWriteRow(batchRequest);
                        foreach (var item in batchWriteRowResponse.TableRespones)
                        {
                            BatchWriteRowResponseForOneTable bwrfo = item.Value;
                            BatchWriteRowResponseItem responseItem= bwrfo.PutResponses[0];
                            bool aa =responseItem.IsOK;
                        }
                        rowChanges = new RowChanges();
                        batchRequest = new BatchWriteRowRequest();
                        batchCount = 0;
                        list = new List<string>();
                        log.Info(i + "批量提交成功");
                    }
                }
                if (rowChanges.PutOperations.Count > 0)
                {
                    batchRequest.Add("Route", rowChanges);
                    oTSClient.BatchWriteRow(batchRequest);
                    log.Info(i + "批量提交成功");
                }
                batchRequest = new BatchWriteRowRequest();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
        }
    }
}
