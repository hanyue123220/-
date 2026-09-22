using System;
using 视觉检测系统.Plc;

namespace Plc
{
    public static class PlcFactory
    {
        public static IPlcClient Create(
            PlcProtocol protocol,
            string ip)
        {
            switch (protocol)
            {
                case PlcProtocol.S7:
                    return new S7Client(ip);

                case PlcProtocol.MC:
                    return new McClient(ip);

                default:
                    throw new NotSupportedException(
                        $"不支持的 PLC 协议：{protocol}"
                    );
            }
        }
    }
}