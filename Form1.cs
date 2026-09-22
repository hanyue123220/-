using Cognex.VisionPro;
using Cognex.VisionPro.Display;
using Cognex.VisionPro.ToolBlock;
using Hikvision.WinForms;
using Plc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using 视觉检测系统.UI界面.加载程序;
using 视觉检测系统.UI界面.设置;
using 视觉检测系统.UI界面.设置.Plc地址设置;
using 视觉检测系统.业务流程;
using 视觉检测系统.业务流程.加载程序.新建程序;
using 视觉检测系统.业务流程.本地测试;
using 视觉检测系统.通用函数层;
using static System.Net.Mime.MediaTypeNames;
namespace 视觉检测系统
{
    public partial class Form1 : Form
    {
        加载文件夹 Getfile = new 加载文件夹();
        /// 

        /// 传递显示的原图
        /// 
        public ICogImage CurrentImage
        {
            get
            {
                return cogRecordDisplayStart.Image;
            }
        }
        public CogRecordDisplay displayStart
        {
            get
            {
                return cogRecordDisplayStart;
            }
        }
        public CogRecordDisplay displayEnd
        {
            get
            {
                return cogRecordDisplayEnd;
            }
        }
        public Label label
        {
            get
            {
                return label1;
            }
        }


	public static bool _plcConnect;
        public IPlcClient Plc { get; set; }
        public Form1()
        {
            InitializeComponent();
            this.Size = new Size(1200, 800);
            cogRecordDisplayStart.BackColor = Color.White;
            cogRecordDisplayEnd.BackColor = Color.White;
            camera.Initialize(System.Windows.Forms.Application.StartupPath);
        }
        private async void Form1_Load(object sender, EventArgs e)
        {
            if (setForm == null || setForm.IsDisposed)
            {
                setForm = new SettingForm(this);
            }
            await Init();
        }
        async Task Init()
        {
            bool connectsuccess = await setForm.InitPlcConnect(setForm.textBox.Text);
            if (connectsuccess)
            {
                _plcConnect = true;
                UpdateUi.Update(label1, () =>
                {
                    label1.Text = "通讯已连接";
                    label1.ForeColor = Color.Green;
                });
                AddLog.WriteLog("通讯已连接");
                StartPlcMonitor();

            }
            else
            {
                _plcConnect = false;

            }
        }
        private CancellationTokenSource _plcCts;
        private Task _plcMonitorTask;

        //public static string _addressDB;
        //public static string _ResultaddressDB;
        private bool _lastTriggerValue = false;
        bool imageisOk;
        // 防止检测还没结束，PLC又来了一个触发信号
        private bool _isDetecting = false;
        /// <summary>
        /// 轮询检测plc信号,触发检测
        /// </summary>
        public void StartPlcMonitor()
        {
            // 防止重复启动
            if (_plcMonitorTask != null && !_plcMonitorTask.IsCompleted)
                return;
            _plcCts = new CancellationTokenSource();
            _plcMonitorTask = Task.Run(async () =>
            {
                // PLC连接成功后，先主动复位为0
                try
                {
                    if (Plc != null && Plc.IsConnected)
                    {
                        Plc.WriteInt32(PlcProperty.triggerAddress, 0);
                        AddLog.WriteLog("PLC触发信号已复位为0");
                    }
                }
                catch (Exception ex)
                {
                    AddLog.WriteLog("PLC初始复位失败：" + ex.Message);
                }
                while (!_plcCts.Token.IsCancellationRequested)
                {
                    try
                    {
                        if (Plc == null || !Plc.IsConnected)
                        {
                            await Task.Delay(500, _plcCts.Token);
                            continue;
                        }
                        // 读取PLC触发寄存器
                        int value = Plc.ReadInt32(PlcProperty.triggerAddress);
                        // PLC变成1 → 触发检测
                        if (value == 1)
                        {
                            AddLog.WriteLog("================================");
                            AddLog.WriteLog("收到PLC拍照触发信号");
                            // =========================
                            // 第一步：触发相机拍照
                            // =========================
                            //AddLog.WriteLog("开始触发相机拍照");
                            //bool photoSuccess = await TriggerCamera();
                            //if (!photoSuccess)
                            //{
                            //    AddLog.WriteLog("相机拍照失败");
                            //    // 拍照失败也复位PLC
                            //    Plc.WriteInt32(_addressDB, 0);
                            //    await Task.Delay(50, _plcCts.Token);
                            //    continue;
                            //}
                            //AddLog.WriteLog("相机拍照完成");
                            //// =========================
                            //// 第二步：视觉检测
                            //// =========================

                            AddLog.WriteLog("开始视觉检测");
                            bool detectSuccess = await Task.Run(() => LocalDetect(CurrentImage));
                            if (detectSuccess)
                            {
                                //检测完成后发送信号
                                AddLog.WriteLog("视觉检测完成");
                                if (imageisOk)
                                {
                                    Plc.WriteInt32(PlcProperty.resultAddress, 1);
                                    AddLog.WriteLog("成功发送OK信号");
                                    await Task.Delay(PlcProperty.delayTime);
                                    Plc.WriteInt32(PlcProperty.resultAddress, 0);
                                }
                                else
                                {
                                    Plc.WriteInt32(PlcProperty.resultAddress, 2);
                                    AddLog.WriteLog("成功发送NG信号");
                                    await Task.Delay(PlcProperty.delayTime);
                                    Plc.WriteInt32(PlcProperty.resultAddress, 0);
                                }

                                AddLog.WriteLog("成功复原结果信号为0");
                            }
                            else
                            {
                                AddLog.WriteLog("视觉检测失败");
                            }
                            bool resetSuccess =
                                Plc.WriteInt32(PlcProperty.triggerAddress, 0);
                            if (resetSuccess)
                            {
                                AddLog.WriteLog("视觉检测流程完毕，PLC触发信号已复位为0");
                            }
                            else
                            {
                                AddLog.WriteLog("PLC触发信号复位失败");
                            }
                            // 等待一下，避免马上重复读取
                            await Task.Delay(50, _plcCts.Token);
                            continue;
                        }
                        // PLC为0，继续等待
                        await Task.Delay(50, _plcCts.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        AddLog.WriteLog("PLC监听异常：" + ex.Message);
                        await Task.Delay(500, _plcCts.Token);
                    }
                }
            });
        }
        private async Task<bool> DetectImage(ICogImage image)
        {
            check.mainform = this;
            try
            {
                if (image == null)
                {
                    AddLog.WriteLog("检测失败：图像为空");
                    return false;
                }
                // 两个VPP都为空
                if (newtoolBlock == null && toolBlock == null)
                {
                    AddLog.WriteLog("检测失败：没有加载VPP");
                    return false;
                }
                // 两个VPP同时存在
                if (newtoolBlock != null && toolBlock != null)
                {
                    AddLog.WriteLog("旧vpp和新vpp同时存在，请选择保留方案");
                    DialogResult dr = MessageBox.Show(
                        "检测到同时存在原有VPP与新建VPP\n\n" +
                        "【是】使用新建VPP\n" +
                        "【否】使用本地加载VPP",
                        "VPP冲突选择",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);
                    if (dr == DialogResult.Yes)
                    {
                        toolBlock.Dispose();
                        toolBlock = null;
                        AddLog.WriteLog("已选择新建VPP");
                        bool result = await Task.Run(() =>
                            check.checkLocalImg(newtoolBlock, image));
                        if (result)
                        {
                            imageisOk = true;
                            AddLog.WriteLog("新建VPP检测完成，结果为OK");
                        }
                        else
                        {
                            imageisOk = false;
                            AddLog.WriteLog("新建VPP检测完成，结果为NG");
                        }
                        check.showResultImg(cogRecordDisplayEnd, newtoolBlock);
                        return result;
                    }
                    else
                    {
                        newtoolBlock.Dispose();
                        newtoolBlock = null;
                        AddLog.WriteLog("已选择原有VPP");
                        bool result = await Task.Run(() =>
                            check.checkLocalImg(toolBlock, image));
                        if (result)
                        {
                            imageisOk = true;
                            AddLog.WriteLog("检测完成，结果为OK");
                        }
                        else
                        {
                            imageisOk = false;
                            AddLog.WriteLog("检测完成，结果为NG");
                        }
                        check.showResultImg(cogRecordDisplayEnd, toolBlock);
                        return result;
                    }
                }
                // 只有新VPP
                if (newtoolBlock != null)
                {
                    bool result = await Task.Run(() =>
                        check.checkLocalImg(newtoolBlock, image));
                    if (result)
                    {
                        imageisOk = true;
                        AddLog.WriteLog("新建VPP检测完成，结果为OK");
                    }
                    else
                    {
                        imageisOk = false;
                        AddLog.WriteLog("新建VPP检测完成，结果为NG");
                    }
                    check.showResultImg(cogRecordDisplayEnd, newtoolBlock);
                    return result;
                }
                // 只有旧VPP
                if (toolBlock != null)
                {
                    bool result = await Task.Run(() =>
                        check.checkLocalImg(toolBlock, image));
                    if (result)
                    {
                        imageisOk = true;
                        AddLog.WriteLog("检测完成，结果为OK");
                    }
                    else
                    {
                        imageisOk = false;
                        AddLog.WriteLog("检测完成，结果为NG");
                    }
                    check.showResultImg(cogRecordDisplayEnd, toolBlock);
                    return result;
                }
                return false;
            }
            catch (Exception ex)
            {
                AddLog.WriteLog("检测异常：" + ex.ToString());
                return false;
            }
        }
        private async Task<bool> LocalDetect(ICogImage image)
        {
            try
            {
                if (image == null)
                {
                    AddLog.WriteLog("本地模拟检测失败：当前没有图片");
                    return false;
                }
                // AddLog.WriteLog("开始本地模拟图片检测");
                //           bool result = await DetectImage(image);
                //           if (result)
                //           {
                //              // AddLog.WriteLog("本地模拟检测完成：OK");
                //return true;
                //           }
                //           else
                //           {
                //               //AddLog.WriteLog("本地模拟检测完成：NG");
                //               return false;
                //           }
                try
                {
                      await DetectImage(image);

                    return true;

                }
                catch (Exception ex)
                {
                    AddLog.WriteLog("本地模拟检测异常：" + ex.ToString());
                    return false;

                }
            }
            catch (Exception ex)
            {
                AddLog.WriteLog("本地模拟检测异常：" + ex.ToString());
                return false;
            }
        }
        private async Task<bool> TriggerVisionDetect()
        {
            throw new NotImplementedException();
        }
        private async Task<bool> TriggerCamera()
        {
            throw new NotImplementedException();
        }
        private void 日志ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddLog.OpenLogForm();
        }
        private void 加载文件夹ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddLog.WriteLog("进入加载文件夹");
            if (Getfile.ChooseFolder())
            {
                AddLog.WriteLog("加载文件夹成功");
                Getfile.ShowCurrentImage(cogRecordDisplayStart);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Getfile.PrevImage();
            Getfile.ShowCurrentImage(cogRecordDisplayStart);
            if (toolBlock != null)
            {
                if (loadUi != null && loadUi.Visible)//可以看见在刷新UI
                {
                    loadUi.RefreshToolBlockImage(CurrentImage);
                }
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Getfile.NextImage();
            Getfile.ShowCurrentImage(cogRecordDisplayStart);
            if (toolBlock != null)
            {
                if (loadUi != null && loadUi.Visible)
                {
                    loadUi.RefreshToolBlockImage(CurrentImage);
                }
            }
        }
        private void 加载程序ToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }
        AddNewProgram newProgram;
        private void 新建程序ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loader = new LoadLocalVpp(this);
            if (CurrentImage == null) { MessageBox.Show("当前没有图片,请先添加图片"); return; }
            if (newtoolBlock != null)
            {
                newtoolBlock.Dispose();
                newtoolBlock = null;
            }
            if (newProgram == null || newProgram.IsDisposed)
            {
                newProgram = new AddNewProgram(this);
                newProgram.Show();
            }
            else
            {
                if (!newProgram.Visible) newProgram.Show();
            }
        }
        private void 本地测试ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
        }
        public LoadLocalVpp loader = null;//加载本地vpp工具统一类
        private async void 加载已有程序ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loader = new LoadLocalVpp(this);
            if (toolBlock != null)
            {
                toolBlock.Dispose();
                toolBlock = null;
            }
            toolBlock = await loader.IniLoad();
            if (toolBlock != null)
            {
                AddLog.WriteLog("加载VPP成功");
                MessageBox.Show("加载程序成功");
            }
        }
        public CogToolBlock toolBlock = null;//统一的toolblock
        public CogToolBlock newtoolBlock = null;//统一的newtoolblock
        Check check = new Check();
        private void 检测_button_Click(object sender, EventArgs e)
        {
            check.mainform = this;
            try
            {
                ICogImage image = CurrentImage;
                if (image == null)
                {
                    MessageBox.Show("请先加载图片");
                    return;
                }
                // 两个都为空
                if (newtoolBlock == null && toolBlock == null)
                {
                    MessageBox.Show("请先加载Vpp");
                    return;
                }
                // 情况1：新旧VPP同时存在，弹窗选择
                if (newtoolBlock != null && toolBlock != null)
                {
                    AddLog.WriteLog("旧vpp和新vpp同时存在，请选择保留方案");
                    DialogResult dr = MessageBox.Show("检测到同时存在原有VPP与新建VPP\n\n【是】使用新建VPP\n【否】使用本地加载VPP",
                        "VPP冲突选择", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dr == DialogResult.Yes)
                    {
                        if (toolBlock != null)
                        {
                            toolBlock.Dispose();
                            toolBlock = null;
                            AddLog.WriteLog("旧vpp资源已释放");
                        }
                        AddLog.WriteLog("已选择新建VPP");
                        if (check.checkLocalImg(newtoolBlock, image))
                        {
                            AddLog.WriteLog("新建vpp检测完成,结果为OK");
                            check.showResultImg(cogRecordDisplayEnd, newtoolBlock);
                        }
                        else
                        {
                            AddLog.WriteLog("新建vpp检测失败,结果为NG");
                            check.showResultImg(cogRecordDisplayEnd, newtoolBlock);
                        }
                    }
                    else
                    {
                        if (newtoolBlock != null)
                        {
                            newtoolBlock.Dispose();
                            newtoolBlock = null;
                            AddLog.WriteLog("新建vpp资源已释放");
                        }
                        AddLog.WriteLog("已选择原有VPP");
                        if (check.checkLocalImg(toolBlock, image))
                        {
                            AddLog.WriteLog("================================");
                            AddLog.WriteLog("检测完成,结果为OK");
                            check.showResultImg(cogRecordDisplayEnd, toolBlock);
                        }
                        else
                        {
                            AddLog.WriteLog("================================");
                            AddLog.WriteLog("检测失败,结果为NG");
                            check.showResultImg(cogRecordDisplayEnd, toolBlock);
                        }
                    }
                }
                // 情况2：只有新建VPP
                else if (newtoolBlock != null)
                {
                    if (check.checkLocalImg(newtoolBlock, image))
                    {
                        AddLog.WriteLog("================================");
                        AddLog.WriteLog("新建vpp检测完成,结果为OK");
                        check.showResultImg(cogRecordDisplayEnd, newtoolBlock);
                    }
                    else
                    {
                        AddLog.WriteLog("================================");
                        AddLog.WriteLog("新建vpp检测失败,结果为NG");
                        check.showResultImg(cogRecordDisplayEnd, newtoolBlock);
                    }
                }
                // 情况3：只有原有VPP
                else if (toolBlock != null)
                {
                    if (check.checkLocalImg(toolBlock, image))
                    {
                        AddLog.WriteLog("================================");
                        AddLog.WriteLog("检测完成,结果为OK");
                        check.showResultImg(cogRecordDisplayEnd, toolBlock);
                    }
                    else
                    {
                        AddLog.WriteLog("================================");
                        AddLog.WriteLog("检测失败,结果为NG");
                        check.showResultImg(cogRecordDisplayEnd, toolBlock);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                AddLog.WriteLog("检测异常：" + ex.ToString());
            }
        }
        LoadLocalVppUi loadUi;
        AddNewProgram newUi;
        private void 打开程序ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (newtoolBlock != null)
            {
                //AddLog.WriteLog("打开newtoolblock");
                newUi = new AddNewProgram(this);
                newUi.Show();
                newUi.RefreshToolBlockImage(CurrentImage);//同步打开newtoolBlock窗口的输入图片
                return;
            }
            if (toolBlock != null)
            {
                //AddLog.WriteLog("打开toolBlock");
                loadUi = new LoadLocalVppUi(this);
                loadUi.Show();
                loadUi.RefreshToolBlockImage(CurrentImage);//同步打开toolblcok窗口的输入图片
                return;
            }
            MessageBox.Show("请先加载程序");
        }
        private void button4_Click(object sender, EventArgs e)
        {
            Check check1 = new Check(this);
            if (toolBlock != null)
            {
                if (check1.mutileCheckLocalImg(cogRecordDisplayStart, toolBlock, Getfile, cogRecordDisplayEnd))
                {
                }
            }
            if (newtoolBlock != null)
            {
                if (check1.mutileCheckLocalImg(cogRecordDisplayStart, newtoolBlock, Getfile, cogRecordDisplayEnd))
                {
                }
            }
        }
        SavePlan savePlan = new SavePlan();//方案管理
        public AddNewVpp addnewVpp = new AddNewVpp();
        private void 保存方案ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(addnewVpp?.newToolBlockpath))
                {
                    savePlan.Save(addnewVpp.newToolBlockpath);
                    MessageBox.Show("保存方案成功");
                    return;
                }
                if (loader != null && !string.IsNullOrEmpty(loader.vppPath))
                {
                    savePlan.Save(loader.vppPath);
                    MessageBox.Show("保存方案成功");
                    return;
                }
                MessageBox.Show("您没有打开任何vpp文件，无法保存方案", "提示",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                AddLog.WriteLog($"保存方案失败: {ex.Message}");
                AddLog.WriteLog($"堆栈信息: {ex.StackTrace}");
                MessageBox.Show($"保存方案失败: {ex.Message}", "错误",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async void 加载方案ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (toolBlock != null)
            {
                toolBlock.Dispose();
                toolBlock = null;
                AddLog.WriteLog("旧vpp资源已释放");
            }
            loader = new LoadLocalVpp(this);
            string choosePlanName = savePlan.ShowSelectPlanDialog();
            if (string.IsNullOrEmpty(choosePlanName)) return;
            // 根据选中方案名自动获取VPP路径
            string targetVppPath = savePlan.FindVppPath(choosePlanName);
            if (string.IsNullOrEmpty(targetVppPath)) return;
            // 赋值给你的加载类，自动加载，无需手动选文件
            loader.vppPath = targetVppPath;
            toolBlock = await loader.IniLoadSave();
            MessageBox.Show($"已自动加载方案：{choosePlanName}\nVPP路径：{targetVppPath}", "加载成功");
        }
        SettingForm setForm = null;
        private void 设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (setForm == null || setForm.IsDisposed)
            {
                setForm = new SettingForm(this);
            }
            setForm.Show();
        }
        private HikvisionCameraClient camera = new HikvisionCameraClient();
        private void 相机采图ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }


}
