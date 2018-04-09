using System;
using System.Collections.Generic;
using System.Text;

namespace TableStoreWebApp
{
    public class ByteIntHelper
    {
        ///// <summary>
        ///// 低位在前，高位在后
        ///// </summary>
        ///// <param name="value">整数</param>
        ///// <param name="length">数组长度</param>
        ///// <returns></returns>
        //public static byte[] intToBytes(int value, int length)
        //{
        //    //byte[] src = new byte[4];
        //    //src[3] = (byte)(value >> 24 & 0xFF);
        //    //src[2] = (byte)(value >> 16 & 0xFF);
        //    //src[1] = (byte)(value >> 8 & 0xFF);
        //    //src[0] = (byte)(value & 0xFF);
        //    //return src;

        //    byte[] bt = new byte[length];
        //    bt[0] = (byte)(value & 0xFF);
        //    for (int i = 1; i < length; i++)
        //    {
        //        int offset = i * 8;
        //        bt[i] = (byte)(value >> offset & 0xFF);
        //    }
        //    return bt;
        //}
        ///// <summary>
        ///// 低位在前，高位在后
        ///// </summary>
        ///// <param name="value">整数</param>
        ///// <returns></returns>
        //public static byte[] intToBytes(int value)
        //{
        //    byte[] src = new byte[4];
        //    src[3] = (byte)(value >> 24 & 0xFF);
        //    src[2] = (byte)(value >> 16 & 0xFF);
        //    src[1] = (byte)(value >> 8 & 0xFF);
        //    src[0] = (byte)(value & 0xFF);
        //    return src;
        //}

        /// <summary>
        /// 高位在前，低位在后
        /// </summary>
        /// <param name="value">整数</param>
        /// <param name="length">数组长度</param>
        /// <returns></returns>
        public static byte[] intToBytes2(long value, int length)
        {
            byte[] bt = new byte[length];
            for (int i = 0; i < length; i++)
            {
                if (i == (length - i - 1))
                {
                    bt[i] = (byte)(value & 0xFF);
                }
                else
                {
                    bt[i] = (byte)(value >> 8 * (length - i - 1) & 0xFF);
                }
            }
            return bt;
        }
        /// 高位在前，低位在后
        /// </summary>
        /// <param name="value">整数</param>
        /// <returns></returns>
        public static byte[] intToBytes2(int value)
        {
            byte[] src = new byte[4];
            src[0] = (byte)((value >> 24) & 0xFF);
            src[1] = (byte)((value >> 16) & 0xFF);
            src[2] = (byte)((value >> 8) & 0xFF);
            src[3] = (byte)(value & 0xFF);
            return src;
        }

        ///// <summary>
        ///// 低位在前，高位在后
        ///// </summary>
        ///// <param name="src">byte数组</param>
        ///// <param name="offset">从数组的第offset位开始</param>
        ///// <returns>int数值 </returns>
        //public static int bytesToInt(byte[] src, int offset)
        //{
        //    int value;
        //    value = (int)((src[offset] & 0xFF)
        //            | ((src[offset + 1] & 0xFF) << 8)
        //            | ((src[offset + 2] & 0xFF) << 16)
        //            | ((src[offset + 3] & 0xFF) << 24));
        //    return value;
        //}

        ///// <summary>
        ///// 低位在前，高位在后
        ///// </summary>
        ///// <param name="src">byte数组</param>
        ///// <param name="offset">从数组的第offset位开始</param>
        ///// <param name="length">位数长度</param>
        ///// <returns>int数值 </returns>
        //public static int bytesToInt(byte[] src, int offset, int length)
        //{
        //    var by = (src[offset] & 0xFF);
        //    for (int i = 1; i < length; i++)
        //    {
        //        by = by | ((src[offset + i] & 0xFF) << 8 * i);
        //    }
        //    return (int)by;
        //}

        /// <summary>
        /// 高位在前，低位在后[默认4个长度]
        /// </summary>
        /// <param name="src">byte数组</param>
        /// <param name="offset">从数组的第offset位开始</param>
        /// <returns>int数值</returns>
        public static int bytesToInt2(byte[] src, int offset)
        {
            int value;
            value = (int)(((src[offset] & 0xFF) << 24)
                    | ((src[offset + 1] & 0xFF) << 16)
                    | ((src[offset + 2] & 0xFF) << 8)
                    | (src[offset + 3] & 0xFF));
            return value;
        }


        /// <summary>
        /// 高位在前，低位在后
        /// </summary>
        /// <param name="src">byte数组</param>
        /// <param name="offset">从数组的第offset位开始</param>
        /// <param name="length">位数长度</param>
        /// <returns>int数值</returns>
        public static int bytesToInt2(byte[] src, int offset, int length)
        {
            var by = ((src[offset] & 0xFF) << 8 * (length - 1));
            for (int i = 1; i < length; i++)
            {
                if (i == length - 1)
                {
                    by = by | ((src[offset + i] & 0xFF));
                }
                else
                {
                    by = by | ((src[offset + i] & 0xFF) << 8 * (length - i - 1));
                }
            }
            return (int)by;
        }

        public static long bytesToInt64(byte[] src, int offset, int length)
        {
            var by = ((src[offset] & 0xFF) << 8 * (length - 1));
            for (int i = 1; i < length; i++)
            {
                if (i == length - 1)
                {
                    by = by | ((src[offset + i] & 0xFF));
                }
                else
                {
                    by = by | ((src[offset + i] & 0xFF) << 8 * (length - i - 1));
                }
            }
            return Convert.ToInt64(by);
        }


        /// <summary>
        /// 获得14位 定位数据
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="speed"></param>
        /// <param name="direct"></param>
        /// <param name="height"></param>
        /// <returns>byte[14]数组</returns>
        public static byte[] GetLocationByte(object latitude, object longitude, int speed, int direct, int height)
        {
            byte[] lbyte = new byte[14];

            byte[] temp = intToBytes2(Convert.ToInt64(latitude), 4);
            long aa=bytesToInt64(temp,0,4);
            lbyte[0] = temp[0];
            lbyte[1] = temp[1];
            lbyte[2] = temp[2];
            lbyte[3] = temp[3];
            temp = intToBytes2(Convert.ToInt64(longitude), 4);
            lbyte[4] = temp[0];
            lbyte[5] = temp[1];
            lbyte[6] = temp[2];
            lbyte[7] = temp[3];
            temp = intToBytes2(Convert.ToInt64(speed), 2);
            lbyte[8] = temp[0];
            lbyte[9] = temp[1];
            temp = intToBytes2(Convert.ToInt64(direct), 2);
            lbyte[10] = temp[0];
            lbyte[11] = temp[1];
            temp = intToBytes2(Convert.ToInt64(height), 2);
            lbyte[12] = temp[0];
            lbyte[13] = temp[1];
            return lbyte;
        }

        public static Dictionary<string,int> GetLocationByByte(byte[] lbyte)
        {
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            dictionary.Add("latitude", bytesToInt2(lbyte,0,4));
            dictionary.Add("longitude", bytesToInt2(lbyte, 4, 4));
            dictionary.Add("speed", bytesToInt2(lbyte, 8, 2));
            dictionary.Add("direct", bytesToInt2(lbyte, 10, 2));
            dictionary.Add("height", bytesToInt2(lbyte, 12, 2));
            return dictionary;
        }

        public static byte[] GetRouteByte(object startLongitude, object startLatitude, object endLongitude, object endLatitude, object drivingTime, object mileage, object topSpeed)
        {
            byte[] lbyte = new byte[26];
            byte[] temp = intToBytes2(Convert.ToInt64(startLongitude), 4);
            lbyte[0] = temp[0];
            lbyte[1] = temp[1];
            lbyte[2] = temp[2];
            lbyte[3] = temp[3];
            temp = intToBytes2(Convert.ToInt64(startLatitude), 4);
            lbyte[4] = temp[0];
            lbyte[5] = temp[1];
            lbyte[6] = temp[2];
            lbyte[7] = temp[3];
            temp = intToBytes2(Convert.ToInt64(endLongitude), 4);
            lbyte[8] = temp[0];
            lbyte[9] = temp[1];
            lbyte[10] = temp[2];
            lbyte[11] = temp[3];
            temp = intToBytes2(Convert.ToInt64(endLatitude), 4);
            lbyte[12] = temp[0];
            lbyte[13] = temp[1];
            lbyte[14] = temp[2];
            lbyte[15] = temp[3];
            temp = intToBytes2(Convert.ToInt32(drivingTime), 4);
            lbyte[16] = temp[0];
            lbyte[17] = temp[1];
            lbyte[18] = temp[2];
            lbyte[19] = temp[3];
            temp = intToBytes2(Convert.ToInt32(mileage), 4);
            lbyte[20] = temp[0];
            lbyte[21] = temp[1];
            lbyte[22] = temp[2];
            lbyte[23] = temp[3];
            temp = intToBytes2(Convert.ToInt32(topSpeed), 2);
            lbyte[24] = temp[0];
            lbyte[25] = temp[1];
            return lbyte;
        }

        public static Dictionary<string, int> GetRouteByByte(byte[] lbyte)
        {
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            dictionary.Add("startLongitude", bytesToInt2(lbyte, 0, 4));
            dictionary.Add("startLatitude", bytesToInt2(lbyte, 4, 4));
            dictionary.Add("endLongitude", bytesToInt2(lbyte, 8, 4));
            dictionary.Add("endLatitude", bytesToInt2(lbyte, 12, 4));
            dictionary.Add("drivingTime", bytesToInt2(lbyte, 16, 4));
            dictionary.Add("mileage", bytesToInt2(lbyte, 20, 4));
            dictionary.Add("topSpeed", bytesToInt2(lbyte, 24, 2));
            return dictionary;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="speedingTime">超速行驶时间(>120km/h)(秒)</param>
        /// <param name="highSpeedTime">高速行驶的时间(80km/h-120km/h)</param>
        /// <param name="mediumSpeedTime">中速行驶的时间</param>
        /// <param name="lowSpeedTime">低速行驶的时间(1km/h-40km/h)</param>
        /// <param name="idleTime">怠速</param>
        /// <returns></returns>
        public static byte[] GetDurationstatsByte(object speedingTime, object highSpeedTime, object mediumSpeedTime, object lowSpeedTime,object idleTime)
        {
            byte[] lbyte = new byte[18];
            byte[] temp = intToBytes2(Convert.ToInt32(idleTime), 2);//怠速
            lbyte[0] = temp[0];
            lbyte[1] = temp[1];
            temp = intToBytes2(Convert.ToInt32(lowSpeedTime), 2);//1km/h-40km/h
            lbyte[4] = temp[0];
            lbyte[5] = temp[1];
            temp = intToBytes2(Convert.ToInt32(mediumSpeedTime), 2);//40-80km/h
            lbyte[8] = temp[0];
            lbyte[9] = temp[1];
            temp = intToBytes2(Convert.ToInt32(highSpeedTime), 2);//80km/h-120km/h
            lbyte[12] = temp[0];
            lbyte[13] = temp[1];
            temp = intToBytes2(Convert.ToInt32(speedingTime), 2);//>120km/h
            lbyte[16] = temp[0];
            lbyte[17] = temp[1];
            return lbyte;
        }

        public static Dictionary<string, int> GetDurationstatsByByte(byte[] lbyte)
        {
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            dictionary.Add("idleTime", bytesToInt2(lbyte, 0, 2));
            dictionary.Add("lowSpeedTime", bytesToInt2(lbyte, 4, 2));
            dictionary.Add("mediumSpeedTime", bytesToInt2(lbyte, 8, 2));
            dictionary.Add("highSpeedTime", bytesToInt2(lbyte, 12, 2));
            dictionary.Add("speedingTime", bytesToInt2(lbyte, 16, 2));
            return dictionary;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rapidAccelerationTimes">急加速次数</param>
        /// <param name="rapidDecelerationTimes">急减速次数</param>
        /// <param name="sharpTurnTimes">急转弯次数</param>
        /// <returns></returns>
        public static byte[] GetEventStatsByte(object rapidAccelerationTimes, object rapidDecelerationTimes, object sharpTurnTimes)
        {            
            List<byte> list = new List<byte>();
            if (rapidAccelerationTimes!=null && Convert.ToInt32(rapidAccelerationTimes)>0)
            {
                list.Add(intToBytes2(18, 1)[0]);//18 为急加速事件代码，1位转为的byte数组长度
                //int as3= bytesToInt2(list.ToArray(),0,1);
                byte[] temp = intToBytes2(Convert.ToInt32(rapidAccelerationTimes), 2);
                list.Add(temp[0]);
                list.Add(temp[1]);
            }
            if (rapidDecelerationTimes != null && Convert.ToInt32(rapidDecelerationTimes) > 0)
            {
                list.Add(intToBytes2(19, 1)[0]);//19急减速的事件代码
                byte[] temp = intToBytes2(Convert.ToInt32(rapidDecelerationTimes), 2);
                list.Add(temp[0]);
                list.Add(temp[1]);
            }
            if (sharpTurnTimes != null && Convert.ToInt32(sharpTurnTimes) > 0)
            {
                list.Add(intToBytes2(20, 1)[0]);//20急转弯的事件代码
                byte[] temp = intToBytes2(Convert.ToInt32(sharpTurnTimes), 2);
                list.Add(temp[0]);
                list.Add(temp[1]);
            }
            byte[] lbyte = list.ToArray();
            return lbyte;
        }

        public static Dictionary<string, int> GetEventStatsByByte(byte[] lbyte)
        {
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            for (int i = 0; i < lbyte.Length; i=i+3)
            {
                dictionary.Add("Code", bytesToInt2(lbyte, i*3, 1));
                dictionary.Add("Num", bytesToInt2(lbyte, (i * 3)+1, 2));
            }
            return dictionary;
        }
    }
}
