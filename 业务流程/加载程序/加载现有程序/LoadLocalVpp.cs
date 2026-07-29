using Cognex.VisionPro;
using Cognex.VisionPro.ToolBlock;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using 视觉检测系统;
using 视觉检测系统.UI界面.加载程序;
using 视觉检测系统.通用函数层;


public class LoadLocalVpp
{

    // VPP路径
    public string vppPath { get; set; } = string.Empty;
    CogToolBlock toolblock;
    Form1 mainform;
  public LoadLocalVpp(Form1 form)
    {
        mainform = form;

    }
    public LoadLocalVpp()
    {
    }
    /// <summary>
    /// 选择VPP文件
    /// </summary>
    public bool SelectVppFile()
    {
        using (OpenFileDialog openDlg = new OpenFileDialog())
        {
            openDlg.Filter = "VPP文件|*.vpp";
            openDlg.Title = "请选择VPP文件";


            if (openDlg.ShowDialog() == DialogResult.OK)
            {
                vppPath = openDlg.FileName;
                return true;
            }

            return false;
        }
    }
    //初始化加载本地vpptoolblock
    public async Task<CogToolBlock> IniLoad()
    {

        if (!SelectVppFile())
        {
            return null;
        }
        try
        {

            CogToolBlock toolBlock =
                await Task.Run(() =>
                {
                    return CogSerializer.LoadObjectFromFile(vppPath) as CogToolBlock;
                });//异步加载，防止卡顿

            return toolBlock;

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            return null;
        }

    }
    /// <summary>
    /// 加载保存的方案vpp
    /// </summary>
    /// <returns></returns>
    public async Task<CogToolBlock> IniLoadSave()
    {
        try
        {

            CogToolBlock toolBlock =
                await Task.Run(() =>
                {
                    return CogSerializer.LoadObjectFromFile(vppPath) as CogToolBlock;
                });//异步加载，防止卡顿

            return toolBlock;

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            return null;
        }

    }
    /// <summary>
    /// 保存VPP，覆盖当前加载的VPP文件
    /// </summary>
    /// <param name="toolBlock">当前ToolBlock对象</param>
    /// <returns>保存是否成功</returns>
    public bool SaveVppFile(CogToolBlock toolBlock)
    {

        if (toolBlock == null)
        {
            MessageBox.Show("ToolBlock为空，无法保存！");
            return false;
        }


        if (string.IsNullOrEmpty(vppPath))
        {
            MessageBox.Show("没有VPP路径，无法保存！");
            return false;
        }


        try
        {

            // 保存覆盖原来的VPP文件
            CogSerializer.SaveObjectToFile(
                toolBlock,
                vppPath
            );


            AddLog.WriteLog("VPP保存成功：" + vppPath);

            return true;

        }
        catch (Exception ex)
        {

            MessageBox.Show(
                $"保存VPP失败:{ex.Message}"
            );


            AddLog.WriteLog(
                "VPP保存失败：" + ex.Message
            );


            return false;
        }

    }
    public void RefreshImage(CogToolBlock toolBlock)
    {
        
        toolBlock.Inputs["Input"].Value = mainform.CurrentImage;
        toolBlock.Run();
    }
}