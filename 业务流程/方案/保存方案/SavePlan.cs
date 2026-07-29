using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using 视觉检测系统.业务流程.方案.保存方案;

namespace 视觉检测系统.业务流程
{
    // 单个方案的数据模型,会被序列化进 XML
    [Serializable]

  
    internal class SavePlan
    {
       
        // 方案文件统一放在程序目录下的 Plans 文件夹,避免污染用户其它目录
        private static readonly string PlanDir = Path.Combine(Application.StartupPath, "Plans");
        private static readonly string PlanFile = Path.Combine(PlanDir, "plans.xml");

        public SavePlan()
        {
            if (!Directory.Exists(PlanDir))
                Directory.CreateDirectory(PlanDir);
        }

        /// <summary>
        /// 保存当前 VPP 路径为方案,会弹出输入框让用户填方案名
        /// </summary>
        public bool Save(string currentVppPath)
        {
            if (string.IsNullOrEmpty(currentVppPath) || !File.Exists(currentVppPath))
            {
                MessageBox.Show("当前没有有效的 VPP 文件路径,请先加载或选择 VPP", "提示");
                return false;
            }

            // 弹框输入方案名
            string name = InputDialog.Show("请输入方案名称:", "保存方案", "");
            if (string.IsNullOrWhiteSpace(name))
                return false;

            var list = LoadAll();

            // 同名方案提示是否覆盖
            if (list.Exists(p => p.PlanName == name))
            {
                if (MessageBox.Show($"已存在同名方案「{name}」,是否覆盖?", "提示",
                    MessageBoxButtons.YesNo) != DialogResult.Yes)
                    return false;
                list.RemoveAll(p => p.PlanName == name);
            }

            var item = new PlanItem
            {
                PlanName = name,
                VppPath = currentVppPath,
                SaveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            list.Add(item);

            return SaveList(list);
        }

        /// <summary>
        /// 读取所有已保存的方案,用于列表展示
        /// </summary>
        public List<PlanItem> LoadAll()
        {
            if (!File.Exists(PlanFile))
                return new List<PlanItem>();

            try
            {
                var serializer = new XmlSerializer(typeof(List<PlanItem>));
                using (var fs = new FileStream(PlanFile, FileMode.Open, FileAccess.Read))
                {
                    return (List<PlanItem>)serializer.Deserialize(fs);
                }
            }
            catch
            {
                // 文件损坏等情况返回空列表,不让程序崩
                return new List<PlanItem>();
            }
        }

        /// <summary>
        /// 按方案名直接拿到 VPP 路径,调用方不用再弹选择框
        /// </summary>
        public string FindVppPath(string planName)
        {
            var item = LoadAll().Find(p => p.PlanName == planName);
            if (item == null)
            {
                MessageBox.Show($"未找到方案「{planName}」", "提示");
                return null;
            }
            if (!File.Exists(item.VppPath))
            {
                MessageBox.Show($"方案「{planName}」对应的 VPP 文件不存在:\n{item.VppPath}\n可能已被移动或删除",
                    "提示");
                return null;
            }
            return item.VppPath;
        }

        /// <summary>
        /// 删除某个方案
        /// </summary>
        public bool Delete(string planName)
        {
            var list = LoadAll();
            int removed = list.RemoveAll(p => p.PlanName == planName);
            if (removed > 0)
            {
                SaveList(list);
                return true;
            }
            return false;
        }

        // 内部:把列表序列化写进 XML
        private bool SaveList(List<PlanItem> list)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(List<PlanItem>));
                using (var fs = new FileStream(PlanFile, FileMode.Create, FileAccess.Write))
                {
                    serializer.Serialize(fs, list);
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("保存方案失败:" + ex.Message, "错误");
                return false;
            }
        }
        /// <summary>
        /// 弹出选择框，选择已保存方案，返回选中方案名，取消返回null
        /// </summary>
        public string ShowSelectPlanDialog()
        {
            var planList = LoadAll();
            if (planList.Count == 0)
            {
                MessageBox.Show("暂无保存的VPP方案，请先保存方案", "提示");
                return null;
            }

            // 构造下拉选择弹窗
            var selectForm = new Form
            {
                Text = "选择已保存方案",
                Size = new Size(350, 180),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var lblTip = new Label
            {
                Text = "请选择要加载的方案：",
                Location = new Point(20, 20),
                AutoSize = true
            };

            var cboPlan = new ComboBox
            {
                Location = new Point(20, 50),
                Size = new Size(280, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // 绑定方案名称
            foreach (var plan in planList)
            {
                cboPlan.Items.Add($"{plan.PlanName} 【保存时间：{plan.SaveTime:yyyy-MM-dd HH:mm}】");
            }
            cboPlan.SelectedIndex = 0;

            var btnConfirm = new Button
            {
                Text = "确定加载",
                Location = new Point(70, 100),
                Size = new Size(90, 30)
            };

            var btnCancel = new Button
            {
                Text = "取消",
                Location = new Point(180, 100),
                Size = new Size(90, 30)
            };

            string selectPlanName = null;

            btnConfirm.Click += (s, e) =>
            {
                if (cboPlan.SelectedItem == null) return;
                // 截取前面纯方案名
                string fullText = cboPlan.SelectedItem.ToString();
                selectPlanName = fullText.Split('【')[0].Trim();
                selectForm.DialogResult = DialogResult.OK;
                selectForm.Close();
            };

            btnCancel.Click += (s, e) =>
            {
                selectForm.DialogResult = DialogResult.Cancel;
                selectForm.Close();
            };

            selectForm.Controls.AddRange(new Control[] { lblTip, cboPlan, btnConfirm, btnCancel });

            if (selectForm.ShowDialog() == DialogResult.OK)
            {
                return selectPlanName;
            }
            return null;
        }
    }

    // 简单的输入对话框辅助类,WinForms 没有自带 InputBox,这里自己拼一个
    public static class InputDialog
    {
        public static string Show(string text, string caption, string defaultValue)
        {
            Form f = new Form
            {
                Width = 360,
                Height = 160,
                Text = caption,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false
            };
            Label lbl = new Label { Left = 12, Top = 12, Width = 320, Text = text };
            TextBox tb = new TextBox { Left = 12, Top = 40, Width = 320, Text = defaultValue };
            Button ok = new Button { Text = "确定", Left = 176, Top = 70, Width = 75, DialogResult = DialogResult.OK };
            Button cancel = new Button { Text = "取消", Left = 257, Top = 70, Width = 75, DialogResult = DialogResult.Cancel };
            f.Controls.AddRange(new Control[] { lbl, tb, ok, cancel });
            f.AcceptButton = ok;
            f.CancelButton = cancel;
            return f.ShowDialog() == DialogResult.OK ? tb.Text.Trim() : null;
        }
    }
}
