using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;
using TableStoreConsole.Data;
using log4net.Repository;
using Aliyun.OTS.DataModel;

namespace TableStoreConsole
{
    public class TableStoreModel
    {
        public static string endPointPublic { get; set; }
        public static string endPointPrivate { get; set; }
        public static string accessKeyID { get; set; }
        public static string accessKeySecret { get; set; }
        public static string instanceName { get; set; }

        public static string SqlServerConnection { get; set; }

        

        public static BidoDBContext _dbContext;
        public static ILoggerRepository repository { get; set; }

        public TableStoreModel()
        {
            DbContextOptions<BidoDBContext> dbContext = new DbContextOptions<BidoDBContext>();
            DbContextOptionsBuilder<BidoDBContext> dbContextBuilder = new DbContextOptionsBuilder<BidoDBContext>();
            _dbContext = new BidoDBContext(dbContextBuilder.UseSqlServer(TableStoreModel.SqlServerConnection).Options);
        }

        public static string PrintColumnValue(ColumnValue value)
        {
            switch (value.Type)
            {
                case ColumnValueType.String: return value.StringValue;
                case ColumnValueType.Integer: return value.IntegerValue.ToString();
                case ColumnValueType.Boolean: return value.BooleanValue.ToString();
                case ColumnValueType.Double: return value.DoubleValue.ToString();
                case ColumnValueType.Binary: return value.BinaryValue.ToString();
            }

            throw new Exception("Unknow type.");
        }
    }
}
