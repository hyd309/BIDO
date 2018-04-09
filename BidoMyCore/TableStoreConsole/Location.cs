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

namespace TableStoreConsole
{
    public class Location
    {
        private ILog log = LogManager.GetLogger(TableStoreModel.repository.Name, typeof(Location));

        OTSClient oTSClient = new OTSClient(TableStoreModel.endPointPublic, TableStoreModel.accessKeyID, TableStoreModel.accessKeySecret, TableStoreModel.instanceName);
        BatchWriteRowRequest batchRequest = new BatchWriteRowRequest();

        private string TableName = "L_100000000";

        public void GetLocationData(object tableName)
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
                        TableStoreAddLocation(ds.Tables[0].Rows);
                    }
                    catch (Exception ex)
                    {
                        log.Error("GetLocationData=>访问数据库出错：" + ex.Message);
                    }
                }
            }
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
                    var primaryKey = new PrimaryKey();
                    primaryKey.Add("d", new ColumnValue(Convert.ToInt64(dr["device_code"])));
                    primaryKey.Add("t", new ColumnValue(TimeHelper.ConvertDateTimeToInt(Convert.ToDateTime(dr["gps_time"]))));

                    string primarykey = dr["device_code"].ToString() + ";" + dr["gps_time"].ToString();
                    if (list.Contains(primarykey))
                    {
                        log.Info("发现重复记录" + primarykey + "数据库记录：");
                        continue;
                    }
                    list.Add(primarykey);

                    var attribute = new AttributeColumns();
                    //定位数据
                    attribute.Add("l", new ColumnValue(ByteIntHelper.GetLocationByte(dr["latitude"], dr["longitude"], Convert.ToInt32(dr["speed"]), Convert.ToInt32(dr["direct"]), 0)));

                    rowChanges.AddPut(new Condition(RowExistenceExpectation.IGNORE), primaryKey, attribute);
                    batchCount++;
                    i++;
                    if (batchCount == 200)//200行数据进行批量提交一次
                    {
                        batchRequest.Add(TableName, rowChanges);
                        BatchWriteRowResponse bwResponse= oTSClient.BatchWriteRow(batchRequest);
                        

                        if (bwResponse.IsAllSucceed)
                        {
                            //所有数据插入成功
                            log.Info(i + "批量提交成功");
                        }
                        else
                        {
                            log.Error(i + "批量提交有出错记录");
                            //把批量提交的数据，赋值给新的list，进行异常插入处理方法
                            //分析具体是哪个index行数据出错
                            
                            var tableRows = bwResponse.TableRespones;
                            var rows = tableRows[TableName];
                            for (int j = 0; j < rows.PutResponses.Count; j++)
                            {
                                if (rows.PutResponses[j].IsOK)
                                {

                                }
                                else
                                {
                                    log.Error("批量提交 问题数据在第【"+j+"】行");
                                    Task task = new Task(() => {
                                        var errorRow = rowChanges.PutOperations[j];
                                        //异步处理错误数据行
                                        PutErrorHandle putError = new PutErrorLocation();
                                        putError.HandlePutError(errorRow, TableName, log);
                                    });
                                    task.Start();
                                }
                            }
                        }
                        rowChanges = new RowChanges();
                        batchRequest = new BatchWriteRowRequest();
                        batchCount = 0;
                        list = new List<string>();
                    }
                }
                if (rowChanges.PutOperations.Count > 0)
                {
                    batchRequest.Add("L_100000000", rowChanges);
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
