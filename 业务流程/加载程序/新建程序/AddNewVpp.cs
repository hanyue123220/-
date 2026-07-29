using Cognex.VisionPro;
using Cognex.VisionPro.ToolBlock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 视觉检测系统.业务流程.加载程序.新建程序
{
   public class AddNewVpp
    {
        public string newToolBlockpath { set; get; } = "";//用于记录新建vpp保存的路径
  
        public void SaveToolBlockToVpp(CogToolBlock saveToolBlock)
        {
            if (saveToolBlock == null)
            {
                MessageBox.Show("流程对象为空，无法保存");
                return;
            }

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "VPP流程文件|*.vpp";
            dlg.Title = "保存VPP流程文件";
            dlg.DefaultExt = ".vpp";
            dlg.RestoreDirectory = true;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // 核心序列化保存
                    CogSerializer.SaveObjectToFile(saveToolBlock, dlg.FileName);
                    newToolBlockpath = dlg.FileName;
                    AddLog.WriteLog(dlg.FileName);
                    AddLog.WriteLog("VPP流程保存成功");
                    MessageBox.Show("VPP流程保存成功");
                }
                catch (Exception ex)
                {
                    AddLog.WriteLog($"保存失败：{ex.Message}");
                    MessageBox.Show($"保存失败：{ex.Message}");
                }
            }
        }

    }
}
