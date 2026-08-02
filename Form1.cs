using Cognex.VisionPro;
using Cognex.VisionPro.Display;
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
using 视觉检测系统.UI界面.加载程序;
using 视觉检测系统.业务流程;
using 视觉检测系统.业务流程.加载程序.新建程序;
using 视觉检测系统.业务流程.本地测试;
using static System.Net.Mime.MediaTypeNames;

namespace 视觉检测系统
{
	public partial class Form1 : Form
	{
		加载文件夹 Getfile = new 加载文件夹();
		/// <summary>
		/// 传递显示的原图
		/// </summary>
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
		public Form1()
		{
			InitializeComponent();
			this.Size = new Size(1000, 500);

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
							AddLog.WriteLog("新建vpp检测完成");
							check.showResultImg(cogRecordDisplayEnd, newtoolBlock);
						}
						else
						{
							AddLog.WriteLog("新建vpp检测失败");
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
							AddLog.WriteLog("检测完成");
							check.showResultImg(cogRecordDisplayEnd, toolBlock);
						}
						else
						{
							AddLog.WriteLog("检测失败");
						}
					}
				}
				// 情况2：只有新建VPP
				else if (newtoolBlock != null)
				{
					if (check.checkLocalImg(newtoolBlock, image))
					{
						AddLog.WriteLog("新建vpp检测完成");
						check.showResultImg(cogRecordDisplayEnd, newtoolBlock);
					}
					else
					{
						AddLog.WriteLog("新建vpp检测失败");
					}
				}
				// 情况3：只有原有VPP
				else if (toolBlock != null)
				{
					if (check.checkLocalImg(toolBlock, image))
					{
						AddLog.WriteLog("检测完成");
						check.showResultImg(cogRecordDisplayEnd, toolBlock);
					}
					else
					{
						AddLog.WriteLog("检测失败");
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
				AddLog.WriteLog("打开newtoolblock");
				newUi = new AddNewProgram(this);
				newUi.Show();
				newUi.RefreshToolBlockImage(CurrentImage);//同步打开newtoolBlock窗口的输入图片

				return;
			}
			if (toolBlock != null)
			{
				AddLog.WriteLog("打开toolBlock");
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


	}
}
