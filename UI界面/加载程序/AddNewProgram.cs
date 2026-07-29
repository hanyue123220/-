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
                mainForm.newtoolBlock = newToolBlock;
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
    }
}
