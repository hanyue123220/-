using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 视觉检测系统.通用函数层
{
    internal class UpdateUi
    {
        /// <summary>
        /// 异步更新ui
        /// </summary>
        /// <param name="control"></param>
        /// <param name="action"></param>
        public static void Update(Control control, Action action)
        {
            if (control == null || control.IsDisposed)
                return;
            try
            {
                if (control.InvokeRequired)
                {
                    control.BeginInvoke(action);
                }
                else
                {
                    action();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
