using Cognex.VisionPro;
using Cognex.VisionPro.ImageFile;
using Cognex.VisionPro.ToolBlock;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 视觉检测系统.存图
{
    internal class SaveImage
    {
        public bool isSave { set; get; }=false;
        /// <summary>
        /// 保存VisionPro图像到文件
        /// </summary>
        private void SaveVisionImage(CogRecordDisplay display,bool isOk)
        {
            string rootDir = Path.Combine(Application.StartupPath, "Image");
            // 2. 日期文件夹 yyyyMMdd
            string dateDir = Path.Combine(rootDir, DateTime.Now.ToString("yyyyMMdd"));
            // 3. OK / NG 子文件夹
            string resultDir = Path.Combine(dateDir, isOk ? "OK" : "NG");

            // 逐级创建目录（不存在自动新建）
            if (!Directory.Exists(resultDir))
            {
                Directory.CreateDirectory(resultDir);
            }

            // 文件名：时间戳，精确到毫秒防止重名
            string fileName = $"img_{DateTime.Now:yyyyMMdd_HHmmss_fff}.bmp";
            string fullSavePath = Path.Combine(resultDir, fileName);

            // VisionPro保存图像
            CogImageFileTool saveTool = new CogImageFileTool();
            saveTool.InputImage = display.Image;    // ✅ 输入图像

            saveTool.Run();

            AddLog.WriteLog($"图像保存成功，路径：{fullSavePath}");
        }
    }
}
