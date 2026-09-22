using HslCommunication.Profinet.Siemens;
using System;

namespace Plc
{
    /// <summary>
    /// 西门子 S7 PLC 通讯
    /// </summary>
    public class S7Client : IPlcClient
    {
        private SiemensS7Net _plc;

        public string Ip { get; set; }

        public int Port { get; set; } = 102;

        public bool IsConnected { get; private set; }

        public S7Client(string ip )
        {
            Ip = ip;

            _plc = new SiemensS7Net(
                SiemensPLCS.S1200,
                Ip
            );

            _plc.Port = Port;
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