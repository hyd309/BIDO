using System;
using System.Collections.Generic;
using System.Text;

using log4net;
using Aliyun.OTS;
using Aliyun.OTS.Request;
using Aliyun.OTS.DataModel;
using System.Data;
using System.Data.SqlClient;
using TableStoreWebApp.Models;
using System.Threading.Tasks;
using Aliyun.OTS.Response;

namespace TableStoreWebApp
{
    public class Event
    {
        private ILog log = LogManager.GetLogger(Startup.repository.Name, typeof(Event));

        OTSClient oTSClient;// = OTSHelper.GetOTSClient();
        BatchWriteRowRequest batchRequest = new BatchWriteRowRequest();

        private string TableName = "E_100000000";
        private string SqlTableName = "Device_Alarm_";//Device_Alarm_201610
        public Event(TableStoreModel tableStoreModel)
        {
            oTSClient = OTSHelper.GetOTSClientEvent(tableStoreModel);
            //oTSClient = new OTSClient(tableStoreModel.PublicEnvironment == "1" ? tableStoreModel.endPointPublic : tableStoreModel.endPointPrivate, tableStoreModel.accessKeyID, tableStoreModel.accessKeySecret, tableStoreModel.instanceName_Event);
        }
        public void GetEventData(object dateFromTo)
        {
            string[] date = dateFromTo.ToString().Split('T');
            DateTime dateFrom = Convert.ToDateTime(date[0] + " 00:00:00");
            DateTime dateTo = Convert.ToDateTime(date[1] + " 00:00:00").AddMonths(1);
            while (dateFrom < dateTo)
            {
                string tableName = SqlTableName + dateFrom.ToString("yyyyMM");
                DataTable dt = new DataTable();
                long start = 0;
                bool nextId = true;
                int indexStep = 100000;//***每次过滤id 10万的范围，id可能不是连续的
                while (nextId)
                {
                    using (SqlConnection conn = new SqlConnection(Startup.SqlConnecting))
                    {
                        try
                        {
                            string sql = "select  top " + indexStep + " id,device_code,alarm_no,alarm_time,alarm_parameter,create_time,latitude,longitude,speed,direct from " + tableName
                                + " where id>"+ start+ " and device_code > 99999999999999 ORDER BY id";
                            log.Debug(sql);
                            SqlCommand cmd = new SqlCommand(sql, conn);
                            conn.Open();
                            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd);
                            DataSet ds = new DataSet();
                            sqlDataAdapter.Fill(ds);
                            cmd.Dispose();
                            conn.Close();
                            nextId = ds.Tables[0].Rows.Count > 0;
                            if (nextId)
                            {
                                start = Convert.ToInt64(ds.Tables[0].Rows[ds.Tables[0].Rows.Count - 1]["id"]);
                                TableStoreAddLocation(ds.Tables[0].Rows);
                            }
                            else
                            {
                                log.Debug(tableName + "表所有记录处理完成！");
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            log.Error("GetEventData=>访问数据库出错：" + ex.Message);
                        }
                    }
                }
                dateFrom = dateFrom.AddMonths(1);
            }
            log.Debug("--------End-------");
        }

        public void TableStoreAddLocation(DataRowCollection drs)
        {
            int batchCount = 0;
            int i = 0;
            List<string> list = new List<string>();//存储批量上传的主键字符串，用于判断批量上传数据中是否有重复
            try
            {
                RowChanges rowChanges = new RowChanges();
                foreach (DataRow dr in drs)
                {
                    i++;
                    int alarm_no = dr["alarm_no"] == null ? -1 : Convert.ToInt32(dr["alarm_no"]);
                    if (alarm_no==202 && dr["alarm_parameter"].ToString()=="0")
                    {
                        //【逻辑判断】当事件id=202且参数=0则设置ID=212
                        alarm_no = 212;
                    }
                    else if (alarm_no == 200)
                    {
                        if (dr["alarm_parameter"].ToString() == "3")
                        {
                            //【逻辑判断】当事件id=200且参数=3则设置ID=25
                            alarm_no = 25;
                        }
                        else
                        {
                            //【逻辑判断】当事件id=200且参数=4则设置ID=26
                            alarm_no = 26;
                        }
                    }
                    string primarykey = dr["device_code"].ToString()+";" + dr["alarm_time"].ToString() + ";" + alarm_no;
                    if (list.Contains(primarykey))
                    {
                        log.Info(i+"发现重复记录"+ primarykey +"数据库记录：id="+dr["id"]);
                        continue;
                    }
                    list.Add(primarykey);
                    var primaryKey = new PrimaryKey();
                    primaryKey.Add("d", new ColumnValue(Convert.ToInt64(dr["device_code"])));
                    primaryKey.Add("et", new ColumnValue(TimeHelper.ConvertDateTimeToInt(Convert.ToDateTime(dr["alarm_time"]))));//事件事件（毫秒）
                    primaryKey.Add("ei", new ColumnValue(ByteIntHelper.intToBytes2(alarm_no,1)));//事件ID

                    var attribute = new AttributeColumns();
                    #region 事件参数暂时不传递，默认空
                    
                    //if (dr["alarm_parameter"] != null)//事件参数 不为空才添加
                    //{
                    //    byte[] temp= ByteIntHelper.intToBytes(Convert.ToInt32(dr["alarm_parameter"]), 4);
                    //    int startIndex = 0;
                    //    foreach (var item in temp)
                    //    {
                    //        if (Convert.ToInt32(item) == 0)
                    //        {
                    //            startIndex++;
                    //            continue;
                    //        }
                    //        else {
                    //            break;
                    //        }
                    //    }
                    //    byte[] byteAlarmNo = new byte[4-startIndex];
                    //    for (int index = 0; index < byteAlarmNo.Length; index++)
                    //    {
                    //        byteAlarmNo[index] = temp[startIndex+index];
                    //    }
                    //    attribute.Add("ep", new ColumnValue(byteAlarmNo)); //事件参数
                    //}
                    #endregion
                    //定位时间
                    attribute.Add("t", new ColumnValue(TimeHelper.ConvertDateTimeToInt(Convert.ToDateTime(dr["create_time"]))));
                    //定位数据
                    attribute.Add("l", new ColumnValue(ByteIntHelper.GetLocationByte(dr["latitude"], dr["longitude"], Convert.ToInt32(dr["speed"]), Convert.ToInt32(dr["direct"]), 0)));

                    rowChanges.AddPut(new Condition(RowExistenceExpectation.IGNORE), primaryKey, attribute);
                    batchCount++;
                    if (batchCount == 200)//200行数据进行批量提交一次
                    {
                        batchRequest.Add(TableName, rowChanges);
                        BatchWriteRowResponse bwResponse = oTSClient.BatchWriteRow(batchRequest);
                        BatchWriteResult(bwResponse, i, rowChanges);
                        rowChanges = new RowChanges();
                        batchRequest = new BatchWriteRowRequest();
                        batchCount = 0;
                        list = new List<string>();
                    }
                }
                if (rowChanges.PutOperations.Count > 0)
                {
                    batchRequest.Add(TableName, rowChanges);
                    BatchWriteRowResponse batchWriteRowResponse = oTSClient.BatchWriteRow(batchRequest);
                    BatchWriteResult(batchWriteRowResponse, i, rowChanges);
                }
                batchRequest = new BatchWriteRowRequest();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
        }


        private void BatchWriteResult(BatchWriteRowResponse batchWriteRowResponse, int index, RowChanges rowChanges)
        {
            try
            {
                if (batchWriteRowResponse.IsAllSucceed)
                {
                    log.Info(index + " 行数据批量提交成功！");
                }
                else
                {
                    //把批量提交的数据，赋值给新的list，进行异常插入处理方法
                    //分析具体是哪个index行数据出错
                    var tableRows = batchWriteRowResponse.TableRespones;
                    var rows = tableRows[TableName];
                    for (int j = 0; j < rows.PutResponses.Count; j++)
                    {
                        if (rows.PutResponses[j].IsOK)
                        {
                            continue;
                        }
                        else
                        {
                            log.Error(index + " 批量提交 问题数据在第【" + j + "】行");
                            Task task = new Task(() =>
                            {
                                var errorRow = rowChanges.PutOperations[j];
                                //异步处理错误数据行
                                PutErrorHandle putError = new PutErrorEvent();
                                putError.HandlePutError(errorRow, TableName);
                            });
                            task.Start();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("BatchWriteResult=>"+ex.Message);
            }
        }
    }
}
