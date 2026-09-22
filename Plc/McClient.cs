using HslCommunication.Profinet.Melsec;
using System;

namespace Plc
{
    /// <summary>
    /// 三菱 MC 协议通讯
    /// </summary>
    public class McClient : IPlcClient
    {
        private MelsecMcNet _plc;

        public string Ip { get; set; }

        public int Port { get; set; } = 6000;

        public bool IsConnected { get; private set; }

        public McClient(string ip)
        {
            Ip = ip;

            _plc = new MelsecMcNet(Ip, Port);
        }

        public bool Connect()
        {
            try
            {
                _plc.IpAddress = Ip;
                _plc.Port = Port;

                var result = _plc.ConnectServer();

                IsConnected = result.IsSuccess;

                return IsConnected;
            }
            catch
            {
                IsConnected = false;
                return false;
            }
        }

        public void Disconnect()
        {
            try
            {
                _plc.ConnectClose();
            }
            catch
            {
            }

            IsConnected = false;
        }

        public bool ReadBool(string address)
        {
            var result = _plc.ReadBool(address);

            if (!result.IsSuccess)
                throw new Exception(result.Message);

            return result.Content;
        }

        public bool WriteBool(string address, bool value)
        {
            var result = _plc.Write(address, value);

            return result.IsSuccess;
        }

        public short ReadInt16(string address)
        {
            var result = _plc.ReadInt16(address);

            if (!result.IsSuccess)
                throw new Exception(result.Message);

            return result.Content;
        }

        public bool WriteInt16(string address, short value)
        {
            var result = _plc.Write(address, value);

            return result.IsSuccess;
        }

        public int ReadInt32(string address)
        {
            var result = _plc.ReadInt32(address);

            if (!result.IsSuccess)
                throw new Exception(result.Message);

            return result.Content;
        }

        public bool WriteInt32(string address, int value)
        {
            var result = _plc.Write(address, value);

            return result.IsSuccess;
        }

        public float ReadFloat(string address)
        {
            var result = _plc.ReadFloat(address);

            if (!result.IsSuccess)
                throw new Exception(result.Message);

            return result.Content;
        }

        public bool WriteFloat(string address, float value)
        {
            var result = _plc.Write(address, value);

            return result.IsSuccess;
        }
    }
}