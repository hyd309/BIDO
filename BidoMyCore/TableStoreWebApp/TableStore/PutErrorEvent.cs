using System;
using System.Collections.Generic;
using System.Text;

using log4net;
using Aliyun.OTS;
using Aliyun.OTS.Request;
using Aliyun.OTS.Response;
using Aliyun.OTS.DataModel;

namespace TableStoreWebApp
{
    public class PutErrorEvent : PutErrorHandle
    {
        private ILog log = LogManager.GetLogger(Startup.repository.Name, typeof(PutErrorEvent));
        public override void HandlePutError(Tuple<Condition, PrimaryKey, AttributeColumns> row, string tableName)
        {
            try
            {
                OTSClient otsClient = OTSHelper._oTSClient;
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
                    if (item.Key == "et")//日期字段处理
                    {
                        sbPrimaryKey.Append(item.Key + ":" + item.Value.IntegerValue + "【" + TimeHelper.ConvertStringToDateTime(item.Value.IntegerValue.ToString()) + "】;");
                    }
                    else if (item.Key == "ei")//事件id二进制字段处理
                    {
                        sbPrimaryKey.Append(item.Key + ":" + ByteIntHelper.bytesToInt2(item.Value.BinaryValue,0,1) + ";");
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

                log.Error(string.Format("错误行重试PUT失败:" + ex.Message + "--参数信息：PrimaryKey：{0}，AttributeColumns{1}", sbPrimaryKey.ToString(), sbAttributeColumns.ToString()));
            }
        }
    }
}