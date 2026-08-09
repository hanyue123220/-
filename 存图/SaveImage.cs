using Cognex.VisionPro;
using Cognex.VisionPro.ImageFile;
using Cognex.VisionPro.ToolBlock;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using 视觉检测系统.UI界面.设置.存图设置;

namespace 视觉检测系统.存图
{
    internal class SaveImage
    {
        public bool isSave { set; get; }=false;
        /// <summary>
        /// 保存VisionPro图像到文件
        /// </summary>


        public void SaveVisionImage(CogRecordDisplay display, bool isOk, bool isSave, string typeTag)
        {
            if (!isSave)
                return;

            if (display == null || display.Image == null)
            {
                AddLog.WriteLog("保存失败：Display图像为空");
                return;
            }

            string rootDir = Path.Combine(Application.StartupPath, "Image");
            string dateDir = Path.Combine(rootDir, DateTime.Now.ToString("yyyyMMdd"));
            string typeTagDir = Path.Combine(dateDir, typeTag);
            string resultDir = Path.Combine(typeTagDir, isOk ? "OK" : "NG");

            if (!Directory.Exists(resultDir))
            {
                Directory.CreateDirectory(resultDir);
            }

            string fileName = $"img_{DateTime.Now:yyyyMMdd_HHmmss_fff}.png";
            string fullSavePath = Path.Combine(resultDir, fileName);

            try
            {
                if (typeTag == "Source")
                {
                    using (CogImageFileTool saveTool = new CogImageFileTool())
                    {
                        saveTool.InputImage = display.Image;
                        saveTool.Operator.Open(fullSavePath, CogImageFileModeConstants.Write);
                        saveTool.Run();
                    }
                    AddLog.WriteLog("原图保存成功");
                }
                else if (typeTag == "Result")
                {
                    using (Bitmap bmp = display.CreateContentBitmap(
                        Cognex.VisionPro.Display.CogDisplayContentBitmapConstants.Image) as Bitmap)
                    {
                        if (bmp == null)
                        {
                            AddLog.WriteLog("保存失败，无法生成画面Bitmap");
                            return;
                        }
                        bmp.Save(fullSavePath, ImageFormat.Png);
                    }
                    AddLog.WriteLog("结果图保存成功");
                }
                else
                {
                    AddLog.WriteLog($"未知的typeTag:{typeTag}，终止保存");
                }
            }
            catch (Exception ex)
            {
                AddLog.WriteLog($"图像保存失败：{ex.Message}");
            }
        }

        // ================================================================
        // 重载2：用ICogImage直接存图（原图，不需要Display控件）
        // 用于：批量检测存图线程（后台STA线程，无窗体控件可用）
        // ================================================================
        public void SaveVisionImage(ICogImage image, bool isOk, bool isSave, string typeTag)
        {
            if (!isSave)
                return;

            if (image == null)
            {
                AddLog.WriteLog("保存失败：图像为空");
                return;
            }

            string rootDir = Path.Combine(Application.StartupPath, "Image");
            string dateDir = Path.Combine(rootDir, DateTime.Now.ToString("yyyyMMdd"));
            string typeTagDir = Path.Combine(dateDir, typeTag);
            string resultDir = Path.Combine(typeTagDir, isOk ? "OK" : "NG");

            if (!Directory.Exists(resultDir))
            {
                Directory.CreateDirectory(resultDir);
            }

            string fileName = $"img_{DateTime.Now:yyyyMMdd_HHmmss_fff}.png";
            string fullSavePath = Path.Combine(resultDir, fileName);

            try
            {
                using (CogImageFileTool saveTool = new CogImageFileTool())
                {
                    saveTool.InputImage = image;
                    saveTool.Operator.Open(fullSavePath, CogImageFileModeConstants.Write);
                    saveTool.Run();
                }
                AddLog.WriteLog($"{typeTag}图保存成功");
            }
            catch (Exception ex)
            {
                AddLog.WriteLog($"图像保存失败：{ex.Message}");
            }
        }
    }
}
