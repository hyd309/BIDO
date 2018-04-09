using System;
using System.Collections.Generic;
using System.Text;

using log4net;
using Aliyun.OTS;
using Aliyun.OTS.Request;
using Aliyun.OTS.Response;
using Aliyun.OTS.DataModel;

namespace TableStoreConsole
{
    public class PutErrorRoute : PutErrorHandle
    {
        public override void HandlePutError(Tuple<Condition, PrimaryKey, AttributeColumns> row, string tableName, ILog log)
        {
            try
            {
                OTSClient otsClient = new OTSClient(TableStoreModel.endPointPublic, TableStoreModel.accessKeyID, TableStoreModel.accessKeySecret, TableStoreModel.instanceName);
                var request = new PutRowRequest(tableName, row.Item1, row.Item2, row.Item3);
                PutRowResponse rowResponse = otsClient.PutRow(request);
                log.Info("数据，操作成功！");
            }
            catch (Exception ex)
            {
                //错误行重试失败,记录主键信息
                StringBuilder sbPrimaryKey = new StringBuilder();
                foreach (var item in row.Item2)
                {
                    if (item.Key == "s")//日期字段处理
                    {
                        sbPrimaryKey.Append(item.Key + ":" + item.Value.IntegerValue + "【" + TimeHelper.ConvertStringToDateTime(item.Value.IntegerValue.ToString()) + "】;");
                    }
                    else
                    {
                        sbPrimaryKey.Append(item.Key + ":" + item.Value.IntegerValue + ";");
                    }
                }

                //记录位置信息【属性列】
                StringBuilder sbAttributeColumns = new StringBuilder();
                foreach (var item in row.Item3)
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

                log.Error(string.Format("错误行重试PUT失败:" + ex.Message + "--参数信息：PrimaryKey：{0}，AttributeColumns{1}", sbPrimaryKey.ToString(), sbAttributeColumns.ToString()));
            }
        }
    }
}
