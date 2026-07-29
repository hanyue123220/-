using Cognex.VisionPro;
using Cognex.VisionPro.ToolBlock;
using System;
using System.Drawing;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace 视觉检测系统.UI界面.加载程序
{
    public partial class LoadLocalVppUi : Form
    {

        LoadLocalVpp loader;

        private Form1 mainForm;
        public LoadLocalVppUi(Form1 form)
        {
            InitializeComponent();

            mainForm = form;
            loader = form.loader;
            if (mainForm.newtoolBlock == null)
            {
                cogToolBlockEditV2Local.Subject =
                 mainForm.toolBlock;
            }
            else
            {
                cogToolBlockEditV2Local.Subject =
                mainForm.newtoolBlock;
            }
         
           
            this.Size = new Size(800, 500);
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            if (loader.SaveVppFile(mainForm.toolBlock)||loader.SaveVppFile(mainForm.newtoolBlock))
            {
                MessageBox.Show("保存vpp成功");
            }
        }
        private void cogToolBlockEditV2Local_Load(object sender, System.EventArgs e)
        {
            if (mainForm.newtoolBlock != null)
            {
                mainForm.newtoolBlock.Inputs["Input"].Value = null;
            }
            if (mainForm.toolBlock != null)
            {
                mainForm.toolBlock.Inputs["Input"].Value = null;
            }
           

        }
        public void RefreshToolBlockImage(ICogImage image)
        {
            if (mainForm.newtoolBlock!=null)
            {
                mainForm.newtoolBlock.Inputs["Input"].Value = image;
                cogToolBlockEditV2Local.Subject = null;
                cogToolBlockEditV2Local.Subject = mainForm.newtoolBlock;
                mainForm.newtoolBlock.Run();
                return;
            }
            if (mainForm.toolBlock!=null)
            {
                mainForm.toolBlock.Inputs["Input"].Value = image;
                cogToolBlockEditV2Local.Subject = null;
                cogToolBlockEditV2Local.Subject = mainForm.toolBlock;
                mainForm.toolBlock.Run();
                return;
            }
         
        }
       
    }
}