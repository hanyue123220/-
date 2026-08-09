using Cognex.VisionPro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using 视觉检测系统.UI界面.设置.存图设置;
using 视觉检测系统.业务流程.本地测试;
using 视觉检测系统.存图;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace 视觉检测系统.UI界面.设置
{
 
    public partial class SettingForm : Form
    {
        SaveImage saveImage = new SaveImage();

        Form1 Mainform=null;
     


        public SettingForm(Form1 form)
        {
            Mainform = form;
            InitializeComponent();
        }
        public SettingForm()
        {
       
            InitializeComponent();
        }
        private void SettingForm_Load(object sender, EventArgs e)
        {
            checkBox1.Checked = saveImgSet.isSaveOkSourceImg;
            checkBox2.Checked = saveImgSet.isSaveOkResultImg;
            checkBox3.Checked = saveImgSet.isSaveNGSourceImg;
            checkBox4.Checked = saveImgSet.isSaveNGResultImg;
          
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
          
        }
      
        private void SettingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            saveImgSet.isSaveOkSourceImg = checkBox1.Checked;
            saveImgSet.isSaveOkResultImg = checkBox2.Checked;
            saveImgSet.isSaveNGSourceImg = checkBox3.Checked;
            saveImgSet.isSaveNGResultImg = checkBox4.Checked;
            MessageBox.Show("保存设置成功");
            AddLog.WriteLog("保存设置成功");
        }
    }
}
