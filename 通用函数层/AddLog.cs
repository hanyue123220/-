using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

internal class AddLog
{
    // 日志文件夹
    private static readonly string LogFolder = Application.StartupPath + "\\Log";

    // 当前程序运行的日志文件路径
    private static string LogPath;


    // 静态构造函数
    // 类第一次使用时自动执行
    static AddLog()
    {
        try
        {
            // 创建日志文件夹
            if (!Directory.Exists(LogFolder))
            {
                Directory.CreateDirectory(LogFolder);
            }


            // 创建本次运行日志文件
            string fileName =
                $"SystemLog_{DateTime.Now:yyyyMMdd_HHmmss}.txt";


            LogPath = Path.Combine(
                LogFolder,
                fileName
            );


            // 创建文件
            File.Create(LogPath).Close();


        }
        catch
        {

        }
    }



    #region 写入日志
    /// <summary>
    /// 写入日志信息
    /// </summary>
    /// <param name="msg">日志内容</param>
    public static void WriteLog(string msg)
    {
        try
        {
            string content =
                $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]  {msg}\r\n";


            // 写入当前运行日志
            File.AppendAllText(
                LogPath,
                content
            );

        }
        catch
        {

        }
    }
    #endregion



    #region 打开日志查看窗口
    /// <summary>
    /// 弹出日志查看窗体
    /// </summary>
    public static void OpenLogForm()
    {

        if (string.IsNullOrEmpty(LogPath))
            return;


        Form logForm = new Form
        {
            Text = "系统运行日志",
            Size = new Size(800, 500),
            StartPosition = FormStartPosition.CenterScreen
        };


        TextBox txtLog = new TextBox
        {
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical,
            Dock = DockStyle.Fill,
            Font = new Font("Consolas", 9)
        };


        // 只读取本次程序启动后的日志
        if (File.Exists(LogPath))
        {
            txtLog.Text = File.ReadAllText(LogPath);
        }


        logForm.Controls.Add(txtLog);

        logForm.ShowDialog();

    }
    #endregion
}