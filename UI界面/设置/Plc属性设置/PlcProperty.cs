using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 视觉检测系统.UI界面.设置.Plc地址设置
{
    public static class PlcProperty
    {
        public static string resultAddress { set; get; } = "DB1.DBW1";
        public static string triggerAddress { set; get; } = "DB1.DBW0";
        public static string heartAddress { set; get; } = "DB1.DBW2";
        public static int delayTime { set; get; } = 500;
        public static bool isStartHeart { set; get; } = false;

    }
}
