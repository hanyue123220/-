using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plc
{
    public interface IPlcClient
    {
        /// <summary>
        /// PLC IP
        /// </summary>
        string Ip { get; set; }

        /// <summary>
        /// PLC 端口
        /// </summary>
        int Port { get; set; }

        /// <summary>
        /// 连接 PLC
        /// </summary>
        bool Connect();

        /// <summary>
        /// 断开 PLC
        /// </summary>
        void Disconnect();

        /// <summary>
        /// 是否连接
        /// </summary>
       bool IsConnected { get;  }

        /// <summary>
        /// 读取 Bool
        /// </summary>
        bool ReadBool(string address);

        /// <summary>
        /// 写入 Bool
        /// </summary>
        bool WriteBool(string address, bool value);

        /// <summary>
        /// 读取 Int16
        /// </summary>
        short ReadInt16(string address);

        /// <summary>
        /// 写入 Int16
        /// </summary>
        bool WriteInt16(string address, short value);

        /// <summary>
        /// 读取 Int32
        /// </summary>
        int ReadInt32(string address);

        /// <summary>
        /// 写入 Int32
        /// </summary>
        bool WriteInt32(string address, int value);

        /// <summary>
        /// 读取 Float
        /// </summary>
        float ReadFloat(string address);

        /// <summary>
        /// 写入 Float
        /// </summary>
        bool WriteFloat(string address, float value);
    }
}
