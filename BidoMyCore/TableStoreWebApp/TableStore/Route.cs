using System;
using System.Collections.Generic;
using System.Text;

namespace TableStoreWebApp
{
    using log4net;
    using Aliyun.OTS;
    using Aliyun.OTS.Request;
    using Aliyun.OTS.DataModel;
    using System.Data;
    using System.Data.SqlClient;
    using Aliyun.OTS.Response;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;
    using TableStoreWebApp.Models;

    public class Route
    {
        private ILog log = LogManager.GetLogger(Startup.repository.Name, typeof(Route));

        OTSClient oTSClient;// = OTSHelper.GetOTSClient();
        BatchWriteRowRequest batchRequest = new BatchWriteRowRequest();
        private string TableName = "Route";
        private string SqlTableName = "Vehicle_Route";
        public Route(TableStoreModel tableStoreModel)
        {
            oTSClient = new OTSClient(tableStoreModel.PublicEnvironment=="1"? tableStoreModel.endPointPublic: tableStoreModel.endPointPrivate, tableStoreModel.accessKeyID, tableStoreModel.accessKeySecret, tableStoreModel.instanceName_Route);
        }

        public void GetRoutenData(object tableName)
        {
            DataTable dt = new DataTable();
            int start = 0;
            bool nextId = true;
            int indexStep = 100000;//***每次过滤id 10万的范围，id可能不是连续的
            while (nextId)
            {
                using (SqlConnection conn = new SqlConnection(Startup.SqlConnecting))
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
                            ",speedingTime,highSpeedTime,mediumSpeedTime,lowSpeedTime,idleTime,rapidAccelerationTimes,rapidDecelerationTimes,sharpTurnTimes from " + SqlTableName.ToString()
                            + " where id between " + start + " and " + (start + indexStep) + " and device_code > 99999999999999 ";
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
                        if (nextId)
                        {
                            TableStoreAddLocation(ds.Tables[0].Rows);
                        }
                        else
                        {
                            log.Debug(SqlTableName + "表所有记录处理完成！");
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error("GetRoutenData=>访问数据库出错：" + ex.Message);
                    }
                }
            }
            log.Debug("--------End-------");
        }

        public void TableStoreAddLocation(DataRowCollection drs)
        {
            int batchCount = 0;
            int i;
            List<string> list = new List<string>();//存储批量上传的主键字符串，用于判断批量上传数据中是否有重复
            try
            {
                RowChanges rowChanges = new RowChanges();
                for (i = 0; i < drs.Count; i++)
                {
                    DataRow dr = drs[i];
                    i++;
                    var primaryKey = new PrimaryKey();
                    primaryKey.Add("d", new ColumnValue(Convert.ToInt64(dr["device_code"])));
                    primaryKey.Add("s", new ColumnValue(TimeHelper.ConvertDateTimeToInt(Convert.ToDateTime(dr["startTime"]))));

                    string primarykey = dr["device_code"].ToString() + ";" + dr["startTime"].ToString();
                    if (list.Contains(primarykey))
                    {
                        log.Info(i + "发现重复记录" + primarykey + "数据库记录：");
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
                    if (batchCount == 200)//200行数据进行批量提交一次
                    {
                        batchRequest.Add(TableName, rowChanges);
                        BatchWriteRowResponse batchWriteRowResponse =oTSClient.BatchWriteRow(batchRequest);
                        BatchWriteResult(batchWriteRowResponse,i,rowChanges);
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

        private void BatchWriteResult(BatchWriteRowResponse batchWriteRowResponse,int index, RowChanges rowChanges)
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
                        log.Error(index + " 批量提交 问题数据在第【" +j + "】行");
                        Task task = new Task(() => {
                            var errorRow = rowChanges.PutOperations[j];
                            //异步处理错误数据行
                            PutErrorHandle putError = new PutErrorRoute();
                            putError.HandlePutError(errorRow, TableName);
                        });
                        task.Start();
                    }
                }
            }
        }
    }
}
