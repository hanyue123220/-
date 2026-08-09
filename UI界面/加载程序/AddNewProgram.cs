using Cognex.VisionPro;
using Cognex.VisionPro.ToolBlock;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using 视觉检测系统.业务流程.加载程序.新建程序;

namespace 视觉检测系统.UI界面.加载程序
{
    public partial class AddNewProgram : Form
    {
        private Form1 mainForm;
        private CogToolBlock newToolBlock=new CogToolBlock();
        LoadLocalVpp loader;
      public  AddNewVpp addNewVpp=new AddNewVpp() ;
        bool Result { set; get; } = true;
        public AddNewProgram( Form1 form)
        {
            InitializeComponent();
            mainForm = form;
            mainForm.addnewVpp = addNewVpp;
            loader = form.loader;
            this.Size = new Size(800, 500);
        }

        private void button1_Click(object sender, EventArgs e)
        {
         
            if (mainForm.newtoolBlock != null)
            {
                addNewVpp.SaveToolBlockToVpp(mainForm.newtoolBlock);
            }
        
        }

        private void cogToolBlockEditV2New_Load(object sender, EventArgs e)
        {
            if (mainForm.newtoolBlock == null)
            {
                CreteNewToolBlock();
            }
         
        }
        public CogToolBlock CreteNewToolBlock()
        {
            mainForm.newtoolBlock = newToolBlock;

            // 获取当前显示图片
            ICogImage image = mainForm.CurrentImage;
            try
            {
                // 第一步：创建终端对象，指定名称和类型
                // 0 = Input, 1 = Output
                newToolBlock.Inputs.Add(new CogToolBlockTerminal("Input", image));
                //newToolBlock.Outputs.Add(new CogToolBlockTerminal("OutputImage", image));
                //newToolBlock.Outputs["OutputImage"].Value = null;   // ← 关键:清空值
                newToolBlock.Outputs.Add(new CogToolBlockTerminal("Result", Result));
                newToolBlock.Outputs["Result"].Value = null;
                newToolBlock.Inputs["Input"].Value = image;
                cogToolBlockEditV2New.Subject = newToolBlock;
           
                AddLog.WriteLog("新建VPP成功，已绑定当前图片");
                return newToolBlock;
            }
            catch (Exception ex)
            {
              
                MessageBox.Show(
                    ex.Message
                );
                return null;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (newToolBlock != null)
            {
                newToolBlock.Run();
               // mainForm.newtoolBlock = newToolBlock;
                AddLog.WriteLog("已运行,输出终端已更新");
            }
        }
        public void RefreshToolBlockImage(ICogImage image)
        {
            if (mainForm.newtoolBlock != null)
            {
                mainForm.newtoolBlock.Inputs["Input"].Value = image;
               cogToolBlockEditV2New.Subject = null;
                cogToolBlockEditV2New.Subject = mainForm.newtoolBlock;
                mainForm.newtoolBlock.Run();
                return;
            }
          

        }

        private void AddNewProgram_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 无前置弹窗，自动执行校验
            CogImage8Grey greyImage = null;
            CogImage24PlanarColor colorImage = null;
            string greyTerminalName = "";
            string colorTerminalName = "";
            bool checkPass = false;

            // 获取编辑控件绑定的ToolBlock
            CogToolBlock toolBlock = cogToolBlockEditV2New.Subject;
            if (toolBlock == null)
            {
                DialogResult dr = MessageBox.Show("当前未加载任何工具！\r\n是否强制关闭窗口？", "校验告警", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                if (dr == DialogResult.No)
                {
                    e.Cancel = true;
                }
                return;
            }

            // 遍历ToolBlock输出端子查找图像
            foreach (CogToolBlockTerminal t in toolBlock.Outputs)
            {
                if (t.Value == null)
                    continue;

                if (t.Value is CogImage8Grey grey)
                {
                    greyImage = grey;
                    greyTerminalName = t.Name;
                }
                else if (t.Value is CogImage24PlanarColor color24)
                {
                    colorImage = color24;
                    colorTerminalName = t.Name;
                }
            }

            if (colorImage != null)
            {
                AddLog.WriteLog($"【关闭校验】找到彩色输出图像，终端名称：{colorTerminalName}");
                checkPass = true;
            }
            else if (greyImage != null)
            {
                AddLog.WriteLog($"【关闭校验】找到灰度输出图像，终端名称：{greyTerminalName}");
                checkPass = true;
            }
            else
            {
                AddLog.WriteLog("【关闭校验】输出终端未找到灰度图/彩色图，尝试读取脚本DisplayImageKey图像");
                // 尝试读取脚本UserData里的图像记录方案
                //if (toolBlock.RunStatus.Result == CogToolResultConstants.Accept)
                //{
                    string key = "";
                    try
                    {
                        key = toolBlock.UserData["DisplayImageKey"].ToString();
                        ICogRecord record = toolBlock.CreateLastRunRecord();
                     
                        if (key != null)
                        {
                            AddLog.WriteLog($"【关闭校验】成功找到脚本记录图像，Key:{key}");
                            checkPass = true;
                        }
                        else
                        {
                            AddLog.WriteLog("【关闭校验】图像记录为空");
                        }
                    }
                    catch
                    {
                        // 捕获DisplayImageKey不存在异常，标记校验不通过
                        AddLog.WriteLog("【关闭校验】未设置DisplayImageKey");
                    }
                //}
            }

            // 校验不通过，弹出选择框：强制关闭 / 取消关闭
            if (!checkPass)
            {
                DialogResult dr = MessageBox.Show("图像校验失败！\r\n既没有输出端子图像，也找不到脚本记录图像。\r\n\r\n【是】强制关闭窗口 ｜【否】取消关闭",
                    "校验不通过", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (dr == DialogResult.No)
                {
                    e.Cancel = true;
                    AddLog.WriteLog("【关闭校验】用户选择取消关闭窗口");
                }
                else
                {
                    AddLog.WriteLog("【关闭校验】校验失败，用户选择强制关闭窗口");
                }
            }
            else
            {
                AddLog.WriteLog("【关闭校验】图像校验通过，允许关闭窗口");
            }
        }
    }
}
