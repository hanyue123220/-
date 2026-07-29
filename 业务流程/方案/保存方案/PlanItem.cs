using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 视觉检测系统.业务流程.方案.保存方案
{
    public class PlanItem
    {
        /// <summary>
        /// 方案名称（唯一）
        /// </summary>
        public string PlanName { get; set; }

        /// <summary>
        /// VPP 文件全路径
        /// </summary>
        public string VppPath { get; set; }

        /// <summary>
        /// 保存时间
        /// </summary>
        public string SaveTime { get; set; }
    }
}
