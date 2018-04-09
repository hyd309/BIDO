using System;
using System.Collections.Generic;
using System.Text;

using log4net;
using Aliyun.OTS;
using Aliyun.OTS.Request;
using Aliyun.OTS.DataModel;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Aliyun.OTS.Response;
using TableStoreWebApp.Models;

namespace TableStoreWebApp
{
    public class Location
    {
        private ILog log = LogManager.GetLogger(Startup.repository.Name, typeof(Location));

        OTSClient oTSClient;// = OTSHelper.GetOTSClient();
        BatchWriteRowRequest batchRequest = new BatchWriteRowRequest();

        private string TableName = "L_100000000";
        public Location(TableStoreModel tableStoreModel)
        {
            oTSClient = new OTSClient(tableStoreModel.PublicEnvironment == "1" ? tableStoreModel.endPointPublic : tableStoreModel.endPointPrivate, tableStoreModel.accessKeyID, tableStoreModel.accessKeySecret, tableStoreModel.instanceName);
        }

        public void GetLocationData(object tableName)
        {
            DataTable dt = new DataTable();
            int start = 1;
            bool nextId = true;
            int indexStep = 100000;//***每次过滤id 10万的范围，id可能不是连续的
            while (nextId)
            {
                using (SqlConnection conn = new SqlConnection(Startup.SqlConnecting))
                {
                    try
                    {
                        string sql = "select distinct device_code,gps_time,latitude,longitude,speed,direct from " + tableName
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
                        if (nextId)
                        {
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
                        log.Error("GetLocationData=>访问数据库出错：" + ex.Message);
                    }
                }
            }
            log.Debug("--------End-------");
        }

        public void TableStoreAddLocation(DataRowCollection drs)
        {
            if (drs.Count==0)
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
                    i++;
                    var primaryKey = new PrimaryKey();
                    primaryKey.Add("d", new ColumnValue(Convert.ToInt64(dr["device_code"])));
                    primaryKey.Add("t", new ColumnValue(TimeHelper.ConvertDateTimeToInt(Convert.ToDateTime(dr["gps_time"]))));

                    string primarykey = dr["device_code"].ToString() + ";" + dr["gps_time"].ToString();
                    if (list.Contains(primarykey))
                    {
                        log.Info(i+"发现重复记录" + primarykey + "数据库记录：");
                        continue;
                    }
                    list.Add(primarykey);

                    var attribute = new AttributeColumns();
                    //定位数据
                    attribute.Add("l", new ColumnValue(ByteIntHelper.GetLocationByte(dr["latitude"], dr["longitude"], Convert.ToInt32(dr["speed"]), Convert.ToInt32(dr["direct"]), 0)));

                    rowChanges.AddPut(new Condition(RowExistenceExpectation.IGNORE), primaryKey, attribute);
                    batchCount++;
                    if (batchCount == 200)//200行数据进行批量提交一次
                    {
                        batchRequest.Add(TableName, rowChanges);
                        BatchWriteRowResponse bwResponse= oTSClient.BatchWriteRow(batchRequest);
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
                        Task task = new Task(() => {
                            var errorRow = rowChanges.PutOperations[j];
                            //异步处理错误数据行
                            PutErrorHandle putError = new PutErrorLocation();
                            putError.HandlePutError(errorRow, TableName);
                        });
                        task.Start();
                    }
                }
            }
        }
    }
}
