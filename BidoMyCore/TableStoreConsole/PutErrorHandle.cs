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
    public abstract class PutErrorHandle
    {
        public abstract void HandlePutError(Tuple<Condition, PrimaryKey, AttributeColumns> row, string tableName, ILog log);
    }
}
