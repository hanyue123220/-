using Cognex.VisionPro;
using Plc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using 视觉检测系统.Plc;
using 视觉检测系统.UI界面.设置.Plc地址设置;
using 视觉检测系统.UI界面.设置.存图设置;
using 视觉检测系统.业务流程.本地测试;
using 视觉检测系统.存图;
using 视觉检测系统.通用函数层;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace 视觉检测系统.UI界面.设置
{
 
    public partial class SettingForm : Form
    {
        SaveImage saveImage = new SaveImage();

        Form1 Mainform=null;
     
        PlcProtocol protocol;
        public TextBox textBox
        {
            get
            {
                return textBox2;
            }
        }
        public TextBox readBox
        {
           
            get
            {
                return textBox6;
            }
        }

        public SettingForm(Form1 form)
        {
            Mainform = form;
            InitializeComponent();
            comboBox1.Items.Add("S7");
            comboBox1.Items.Add("MC");

            comboBox1.SelectedIndex = 0;
            textBox2.Text = "127.0.0.1";
            textBox3.Text = "DB1.DBW0";
            textBox6.Text = "DB1.DBW0";
            textBox7.Text = "DB1.DBW1";
            textBox8.Text = PlcProperty.resultAddress;
           
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
            textBox7.Text = PlcProperty.resultAddress;
            textBox6.Text = PlcProperty.triggerAddress;
            textBox9.Text = PlcProperty.delayTime.ToString();
            checkBox5.Checked = PlcProperty.isStartHeart;
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
            PlcProperty.resultAddress= textBox7.Text;
            PlcProperty.triggerAddress= textBox6.Text;
            PlcProperty.delayTime= Convert.ToInt32(textBox9.Text);
            PlcProperty.isStartHeart= checkBox5.Checked;
            MessageBox.Show("保存设置成功");
            AddLog.WriteLog("保存设置成功");
        }
        private void button4_Click(object sender, EventArgs e)
        {
            string ip = textBox2.Text.Trim();

            if (string.IsNullOrEmpty(ip))
            {
                MessageBox.Show("请输入PLC IP");
                return;
            }

            // 根据下拉框选择协议
        

            if (comboBox1.SelectedItem.ToString() == "S7")
            {
                protocol = PlcProtocol.S7;
            }
            else if (comboBox1.SelectedItem.ToString() == "MC")
            {
                protocol = PlcProtocol.MC;
            }
            else
            {
                MessageBox.Show("请选择PLC协议");
                return;
            }
            Mainform.Plc = PlcFactory.Create(protocol, ip);
            // 连接
            bool success = Mainform.Plc.Connect();

            if (success)
            {
                
                MessageBox.Show("连接成功");
                AddLog.WriteLog("通讯已连接");
                UpdateUi.Update(Mainform.label, () =>
                {
                    Mainform.label.Text = "通讯已连接";
                    Mainform.label.ForeColor = Color.Green;
                }
                   
                    );
                Form1._plcConnect = true;
                Mainform.StartPlcMonitor();
            }
            else
            {
             
                UpdateUi.Update(Mainform.label, () =>
                {
                    Mainform.label.Text = "通讯未连接";
                    Mainform.label.ForeColor = Color.Red;
                });
                Form1._plcConnect = false;
                MessageBox.Show("通讯未连接");
            }
            try
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show("连接异常：" + ex.Message);
            }
        }
        // 异步方法，调用时 await InitPlcConnect(ip)
        public async Task<bool> InitPlcConnect(string ip)
        {
            // 释放上一个连接
            if (Mainform.Plc != null)
            {
                if (Mainform.Plc.IsConnected)
                    Mainform.Plc.Disconnect();
                Mainform.Plc = null;
            }

            // 放到线程池执行连接（耗时操作）
            bool success = await Task.Run(() =>
            {
                Mainform.Plc = PlcFactory.Create(protocol, ip);
                return Mainform.Plc.Connect();
            });

            // 这里已经回到UI主线程，直接弹框无异常
            if (!success)
            {

                MessageBox.Show("通讯未连接");
            }
            else
            {

            }



            return success;
        }



        private void button5_Click(object sender, EventArgs e)
        {
            int val;
            if (!int.TryParse(textBox4.Text,out val)||!Form1._plcConnect)
            {
                MessageBox.Show("输入数据错误,或者没有连接");
                return;
            }
            bool success = Mainform.Plc.WriteInt32(textBox3.Text, val);
            if (success)
            {
                MessageBox.Show("发送成功");
            }
            else
            {
                MessageBox.Show("发送失败");
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            int val;
            if (Mainform.Plc == null)
            {
                return;
            }
            val = Mainform.Plc.ReadInt32(textBox8.Text);
            UpdateUi.Update(textBox5, () =>
            {
                string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                textBox5.AppendText($"读取到的数据：{val}  时间：{time}{Environment.NewLine}");
                // 自动滚动到末尾
                textBox5.SelectionStart = textBox5.TextLength;
                textBox5.ScrollToCaret();
            });


        }
    }
}
